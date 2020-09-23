using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Plugin;
using HtcSharp.Core.Plugin.Abstractions;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RemoteGitDeploy.Core;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Manager;
using RemoteGitDeploy.Mvc;
using StackExchange.Redis;

namespace RemoteGitDeploy {
    public class HtcPlugin : IPlugin, IHttpEvents {

        public string Name => "RemoteGitDeploy";
        public string Version => RemoteGitDeploy.Version.GetVersion();

        private Dictionary<string, (HttpMethodAttribute, bool, Type, MethodInfo)> _routes;

        internal static PluginServerContext PluginServerContext;
        internal static RepositoryManager RepositoryManager;
        internal static ConnectionMultiplexer RedisConnection;
        internal static IDatabase Redis;
        internal static ILogger Logger;
        internal static string Domain { get; private set; }
        internal static string DevKey { get; private set; }
        internal static string RepositorySecreteKey { get; private set; }

        public async Task Load(PluginServerContext pluginServerContext, ILogger logger) {
            PluginServerContext = pluginServerContext;
            Logger = logger;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            string path = Path.Combine(PluginServerContext.PluginsPath, "RemoteGitDeploy.json");
            if (!File.Exists(path)) {
                await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                await using var streamWriter = new StreamWriter(fileStream);
                await streamWriter.WriteAsync(JsonUtils.SerializeObject(new {
                    Domain = "some-domain.com",
                    RedisString = "localhost",
                    RedisDatabase = 0,
                    DatabaseString = "Server=127.0.0.1;Port=3306;Database=remotegitdeploy;Uid=root;Pwd=root;",
                    GitUsername = "username",
                    GitPersonalAccessToken = "token",
                    GitRepositoriesDirectory = "./RemoteGitDeploy/",
                    DevKey = Guid.NewGuid().ToString("N"),
                    RepositorySecreteKey = Guid.NewGuid().ToString("N"),
                }, true));
            }
            var config = JsonUtils.GetJsonFile(path);

            Domain = config.GetValue("Domain", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            DevKey = config.GetValue("DevKey", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            RepositorySecreteKey = config.GetValue("RepositorySecreteKey", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();

            var gitUsername = config.GetValue("GitUsername", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            var gitPersonalAccessToken = config.GetValue("GitPersonalAccessToken", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            var gitRepositoriesDirectory = config.GetValue("GitRepositoriesDirectory", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            RepositoryManager = new RepositoryManager(gitUsername, gitPersonalAccessToken, gitRepositoriesDirectory);

            var redisString = config.GetValue("RedisString", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            RedisConnection = await ConnectionMultiplexer.ConnectAsync(redisString);

            var redisDatabase = config.GetValue("RedisDatabase", StringComparison.CurrentCultureIgnoreCase)!.Value<int>();
            Redis = RedisConnection.GetDatabase(redisDatabase);

            var databaseString = config.GetValue("DatabaseString", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            RgdContext.SetConnectionString(databaseString);

            LoadControllers();
        }

        public async Task Enable() {
            await RepositoryManager.Start();
        }

        public Task Disable() {
            RepositoryManager.Stop();
            return Task.CompletedTask;
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }

        private void LoadControllers() {
            _routes = new Dictionary<string, (HttpMethodAttribute, bool, Type, MethodInfo)>();
            MethodInfo[] methods = Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(HttpMethodAttribute), false).Length > 0)
                .ToArray();
            foreach (var method in methods) {
                if (method.ReturnType != typeof(Task) || !method.IsStatic) continue;
                ParameterInfo[] parameters = method.GetParameters();
                var attribute = method.GetCustomAttributes(typeof(HttpMethodAttribute), false).First() as HttpMethodAttribute;
                if (attribute == null) continue;
                if (parameters.Length < 1 || parameters[0].ParameterType != typeof(HttpContext)) continue;
                if (parameters.Length == 2 && !parameters[1].ParameterType.GetInterfaces().Contains(typeof(IHttpJsonObject))) continue;
                Logger.LogInfo(parameters.Length == 2 ? $"Registering route: {attribute.Path}, JsonObject" : $"Registering route: {attribute.Path}");
                _routes.Add(attribute.Path, (attribute, parameters.Length == 2, parameters.Length == 2 ? parameters[1].ParameterType : null, method));
                UrlMapper.RegisterPluginPage(attribute.Path, this);
            }
        }

        public async Task OnHttpPageRequest(HttpContext httpContext, string filename) {
            try {
                if (_routes.TryGetValue(filename, out var value)) {
                    if (httpContext.Request.Method.Equals(value.Item1.Method)) {
                        httpContext.Session = new Session(httpContext);
                        await httpContext.Session.LoadAsync();
                        if (value.Item1.RequireSession && !httpContext.Session.IsAvailable) {
                            await httpContext.Response.SendRequestErrorAsync(403, "Invalid or non-existent session.");
                            return;
                        }

                        if (value.Item1.RequireContentType != null) {
                            if (httpContext.Request.ContentType == null || httpContext.Request.ContentType.Split(";")[0] != value.Item1.RequireContentType) {
                                await httpContext.Response.SendRequestErrorAsync(415, "Content-Type invalid or not recognized.");
                                return;
                            }
                        }

                        try {
                            object[] data = null;
                            if (value.Item2) {
                                using var streamReader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
                                if (JsonConvertExt.TryDeserializeObject(await streamReader.ReadToEndAsync(), value.Item3, out var obj)) {
                                    if (obj is IHttpJsonObject httpJsonObject && await httpJsonObject.ValidateData(httpContext))
                                        data = new object[] { httpContext, obj };
                                    else {
                                        if (!httpContext.Response.HasStarted) await httpContext.Response.SendDecodeErrorAsync();
                                        return;
                                    }
                                } else {
                                    await httpContext.Response.SendDecodeErrorAsync();
                                }

                                streamReader.Close();
                            } else {
                                data = new object[] { httpContext };
                            }

                            // ReSharper disable once PossibleNullReferenceException
                            await (Task)value.Item4.Invoke(null, data);
                        } catch (HttpException ex) {
                            await httpContext.Response.SendErrorAsync(ex.Status, ex.Message);
                        } catch (Exception ex) {
                            if (!httpContext.Response.HasStarted) {
                                var guid = Guid.NewGuid();
                                Logger.LogError(guid, ex);
                                await httpContext.Response.SendInternalErrorAsync(500, $"[{guid}] An internal failure occurred. Please try again later.");
                            }
                        }
                    } else {
                        await httpContext.Response.SendInvalidRequestMethodErrorAsync(value.Item1.Method);
                    }
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
        }

        public Task OnHttpExtensionRequest(HttpContext httpContext, string filename, string extension) {
            return Task.CompletedTask;
        }
    }
}
