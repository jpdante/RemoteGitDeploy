using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions;
using Newtonsoft.Json.Linq;

namespace RemoteGitDeploy.Utils {
    public class JsonData : Dictionary<string, string> {

        public async Task<JsonData> Load(HttpContext httpContext) {
            string json = null;
            try {
                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 2048, true);
                json = await reader.ReadToEndAsync();
                var data = JObject.Parse(json);
                foreach ((string key, var value) in data) {
                    DoJsonAdd(key, value);
                }
            } catch (Exception ex) {
                if (json != null) HtcPlugin.Logger.LogDebug(json);
                HtcPlugin.Logger.LogError(ex, ex);
            }
            return this;
        }

        public void DoJsonAdd(string key, JToken obj) {
            switch (obj.Type) {
                case JTokenType.Array:
                    var array = (JArray)obj;
                    DoJsonAdd($"{key}.length", array.Count);
                    for (var i = 0; i < array.Count; i++) {
                        DoJsonAdd($"{key}.{i}", array[i]);
                    }
                    break;
                case JTokenType.Boolean:
                    this.Add(key, obj.ToObject<bool>().ToString());
                    break;
                case JTokenType.Bytes:
                    this.Add(key, obj.ToObject<object>()?.ToString());
                    break;
                case JTokenType.Comment:
                    this.Add(key, obj.ToObject<object>()?.ToString());
                    break;
                case JTokenType.Constructor:
                    this.Add(key, obj.ToObject<object>()?.ToString());
                    break;
                case JTokenType.Date:
                    this.Add(key, obj.ToObject<DateTime>().ToString(CultureInfo.InvariantCulture));
                    break;
                case JTokenType.Float:
                    this.Add(key, obj.ToObject<float>().ToString(CultureInfo.InvariantCulture));
                    break;
                case JTokenType.Guid:
                    this.Add(key, obj.ToObject<Guid>().ToString());
                    break;
                case JTokenType.Integer:
                    this.Add(key, obj.ToObject<int>().ToString());
                    break;
                case JTokenType.None:
                    this.Add(key, "");
                    break;
                case JTokenType.Null:
                    this.Add(key, null);
                    break;
                case JTokenType.Object:
                    var obj2 = obj.ToObject<JObject>();
                    if (obj2 != null) {
                        foreach ((string s, var value) in obj2) {
                            DoJsonAdd($"{key}.{s}", value);
                        }
                    }
                    this.Add(key, obj.ToObject<JObject>()?.ToString());
                    break;
                case JTokenType.Property:
                    this.Add(key, obj.ToObject<object>()?.ToString());
                    break;
                case JTokenType.Raw:
                    this.Add(key, obj.ToObject<object>()?.ToString());
                    break;
                case JTokenType.String:
                    this.Add(key, obj.ToObject<string>());
                    break;
                case JTokenType.TimeSpan:
                    this.Add(key, obj.ToObject<TimeSpan>().ToString());
                    break;
                case JTokenType.Undefined:
                    this.Add(key, obj.ToObject<object>()?.ToString());
                    break;
                case JTokenType.Uri:
                    this.Add(key, obj.ToObject<Uri>()?.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
