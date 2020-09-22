using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using Newtonsoft.Json;

namespace RemoteGitDeploy.Extensions {
    public static class JsonConvertExt {

        public static bool TryDeserializeObject(string data, Type type, out object value) {
            try {
                value = JsonConvert.DeserializeObject(data, type);
                return value != null;
            } catch {
                value = null;
                return false;
            }
        }

        public static async Task<(bool, T)> TryDeserializeObject<T>(Stream stream) {
            try {
                using var streamReader = new StreamReader(stream, Encoding.UTF8);
                return (true, JsonConvert.DeserializeObject<T>(await streamReader.ReadToEndAsync()));
            } catch(Exception ex) {
                HtcPlugin.Logger.LogError(ex);
                return (false, default(T));
            }
        }
    }
}