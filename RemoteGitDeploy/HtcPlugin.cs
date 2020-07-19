using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Plugin;
using HtcSharp.Core.Plugin.Abstractions;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using Newtonsoft.Json.Linq;
using RemoteGitDeploy.API;
using RemoteGitDeploy.Manager;
using RemoteGitDeploy.Security;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy {
    public class HtcPlugin : IPlugin, IHttpEvents {

        public string Name => "RemoteGitDeploy";
        public string Version => RemoteGitDeploy.Version.GetVersion();

        internal static PluginServerContext PluginServerContext;
        internal static RepositoryManager RepositoryManager;
        internal static DatabaseManager DatabaseManager;
        internal static CacheManager CacheManager;
        internal static ILogger Logger;
        internal static List<IAPI> ApiPages;
        internal static string Domain { get; private set; }
        internal static string DevKey { get; private set; }

        public Task Load(PluginServerContext pluginServerContext, ILogger logger) {
            PluginServerContext = pluginServerContext;
            Logger = logger;
            string path = Path.Combine(PluginServerContext.PluginsPath, "RemoteGitDeploy.json");
            if (!File.Exists(path)) {
                using var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                using var streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(JsonUtils.SerializeObject(new {
                    Domain = "some-domain.com",
                    CacheString = "localhost",
                    DatabaseString = "Server=127.0.0.1;Port=3306;Database=remotegitdeploy;Uid=root;Pwd=root;",
                    GitUsername = "username",
                    GitPersonalAccessToken = "token",
                    GitRepositoriesDirectory = "./RemoteGitDeploy/",
                    DevKey = SessionGenerator.Create(),
                }, true));
            }
            var config = JsonUtils.GetJsonFile(path);

            Domain = config.GetValue("Domain", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            DevKey = config.GetValue("DevKey", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();

            var gitUsername = config.GetValue("GitUsername", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            var gitPersonalAccessToken = config.GetValue("GitPersonalAccessToken", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            var gitRepositoriesDirectory = config.GetValue("GitRepositoriesDirectory", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            RepositoryManager = new RepositoryManager(gitUsername, gitPersonalAccessToken, gitRepositoriesDirectory);

            var cacheString = config.GetValue("CacheString", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            CacheManager = new CacheManager(cacheString);

            var databaseString = config.GetValue("DatabaseString", StringComparison.CurrentCultureIgnoreCase)!.Value<string>();
            DatabaseManager = new DatabaseManager(databaseString);

            return Task.CompletedTask;
        }

        public async Task Enable() {
            await CacheManager.ConnectAsync();
            ApiPages = new List<IAPI>();
            var interfaceType = typeof(IAPI);
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => interfaceType.IsAssignableFrom(p));
            foreach (var type in types) {
                if (type.IsInterface || type.IsAbstract) continue;
                var page = (IAPI)Activator.CreateInstance(type);
                ApiPages.Add(page);
                UrlMapper.RegisterPluginPage(page.FileName, this);
            }
            await RepositoryManager.Start();
        }

        public async Task Disable() {
            foreach (var page in ApiPages) {
                UrlMapper.UnRegisterPluginPage(page.FileName);
            }
            ApiPages.Clear();
            await CacheManager.DisconnectAsync();
            RepositoryManager.Stop();
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }

        public async Task OnHttpPageRequest(HttpContext httpContext, string filename) {
            try {
                if (filename == null) {
                    await DefaultResponse.InternalError(httpContext);
                    return;
                }
                if (!httpContext.Request.Host.ToString().Equals(Domain)) return;
                foreach (var page in ApiPages.Where(page => filename.Equals(page.FileName))) {
                    Logger.LogDebug($"{httpContext.Request.Method} {filename}");
                    if (!httpContext.Request.Method.Equals(page.RequestMethod, StringComparison.CurrentCultureIgnoreCase)) continue;
                    if (page.RequestContentType != null) {
                        if (httpContext.Request.ContentType == null || !httpContext.Request.ContentType.Split(";", 2)[0].Equals(page.RequestContentType, StringComparison.CurrentCultureIgnoreCase)) {
                            await DefaultResponse.InvalidContentType(httpContext, httpContext.Request.ContentType);
                            return;
                        }
                    }
                    if (page.NeedAuthentication) {
                        httpContext.Session = new Session(httpContext);
                        await httpContext.Session.LoadAsync();
                        if (!httpContext.Session.IsAvailable) {
                            await DefaultResponse.InvalidSession(httpContext);
                            return;
                        }
                    }
                    httpContext.Response.ContentType = ContentType.JSON.ToValue();
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;
                    await page.OnRequest(httpContext);
                    return;
                }
                if (!httpContext.Response.HasStarted) {
                    await DefaultResponse.UnknownApi(httpContext, filename);
                }
            } catch (Exception ex) {
                if (!httpContext.Response.HasStarted) {
                    await DefaultResponse.InternalError(httpContext);
                }
#if DEBUG
                Logger.LogTrace($"Failed to process request [{httpContext.Connection.Id}] {ex.Message}\n{ex.StackTrace}", ex);
#else
                Logger.LogError($"Failed to process request [{httpContext.Connection.Id}]", ex);
#endif
            }
        }

        public Task OnHttpExtensionRequest(HttpContext httpContext, string filename, string extension) {
            return Task.CompletedTask;
        }
    }
}
