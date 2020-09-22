using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Features;
using StackExchange.Redis;

namespace RemoteGitDeploy.Core {
    public class Session : ISession {

        public bool IsAvailable { get; private set; }

        public string Id { get; private set; }

        public IEnumerable<string> Keys { get; private set; }

        private Dictionary<string, RedisValue> _data;

        private List<HashEntry> _updateData;

        private List<RedisValue> _removeKeys;

        private bool _clearAllData;

        public Session(HttpContext httpContext) {
            IsAvailable = false;
            Keys = null;
            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var value)) return;
            string[] data = value.ToString().Split(" ", 2);
            if (data.Length == 2 && data[0].Equals("Bearer")) {
                Id = data[1];
            }
        }

        public async Task LoadAsync(CancellationToken cancellationToken = new CancellationToken()) {
            if (string.IsNullOrEmpty(Id)) return;
            IsAvailable = await HtcPlugin.Redis.KeyExistsAsync($"session:{Id}");
            if (IsAvailable) {
                HashEntry[] data = await HtcPlugin.Redis.HashGetAllAsync($"session:{Id}");
                _data = new Dictionary<string, RedisValue>();
                foreach (var entry in data) _data.Add(entry.Name, entry.Value);
                Keys = _data.Keys.AsEnumerable();
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = new CancellationToken()) {
            Id ??= Guid.NewGuid().ToString("N");
            if (_clearAllData) await HtcPlugin.Redis.KeyDeleteAsync($"session:{Id}");
            if (_removeKeys != null) await HtcPlugin.Redis.HashDeleteAsync("session." + Id, _removeKeys.ToArray());
            if (_updateData != null) {
                await HtcPlugin.Redis.HashSetAsync($"session:{Id}", _updateData.ToArray());
                await HtcPlugin.Redis.KeyExpireAsync($"session:{Id}", TimeSpan.FromDays(1));
            }
        }

        public bool TryGetValue(string key, out object value) {
            bool tryGet = _data.TryGetValue(key, out var data);
            value = data;
            return tryGet;
        }

        public void Set(string key, object value) {
            _updateData ??= new List<HashEntry>();
            if (value is RedisValue redisValue) {
                _updateData.Add(new HashEntry(key, value.ToString()));
            } else {
                _updateData.Add(new HashEntry(key, value.ToString()));
            }
        }

        public void Remove(string key) {
            _removeKeys ??= new List<RedisValue>();
            if (_data.TryGetValue(key, out var value)) _removeKeys.Add(value);
        }

        public void Clear() {
            _clearAllData = true;
        }
    }
}