using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Plugin;
using HtcSharp.Core.Plugin.Abstractions;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RemoteGitDeploy.Core;
using RemoteGitDeploy.Extensions;
using RemoteGitDeploy.Manager;
using RemoteGitDeploy.Models.Internal;
using RemoteGitDeploy.Models.New;
using RemoteGitDeploy.Mvc;
using RemoteGitDeploy.Security;
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
        internal static Config Config;

        public async Task Load(PluginServerContext pluginServerContext, ILogger logger) {
            PluginServerContext = pluginServerContext;
            Logger = logger;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            string path = Path.Combine(PluginServerContext.PluginsPath, "RemoteGitDeploy.json");
            if (!File.Exists(path)) {
                await using var writeFileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                await using var streamWriter = new StreamWriter(writeFileStream);
                await streamWriter.WriteAsync(JsonConvert.SerializeObject(new Config(), Formatting.Indented));
            }
            await using var readFileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var streamReader = new StreamReader(readFileStream);
            string json = await streamReader.ReadToEndAsync();
            Config = JsonConvert.DeserializeObject<Config>(json);

            RgdContext.SetConnectionString($"Server={Config.Database.Host};Port={Config.Database.Port};Database={Config.Database.Database};Uid={Config.Database.Username};Pwd={Config.Database.Password};");

            await using var context = new RgdContext();
            if (await context.Database.EnsureCreatedAsync()) {
                string password = await Password.GeneratePassword("admin");
                await context.Accounts.AddAsync(new Account("admin", "admin", "admin", "admin", password, Permission.All));
                await context.SaveChangesAsync();
            }

            RepositoryManager = new RepositoryManager(Path.GetFullPath(Config.Git.RepositoriesDirectory));

            RedisConnection = await ConnectionMultiplexer.ConnectAsync(Config.Redis.ConnectionString);
            Redis = RedisConnection.GetDatabase(Config.Redis.Database);

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
                                Logger.LogError($"[{httpContext.Connection.Id}]", ex);
                                await httpContext.Response.SendInternalErrorAsync(500, $"[{httpContext.Connection.Id}] An internal failure occurred. Please try again later.");
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
