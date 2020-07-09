using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Features.Interfaces;

namespace RemoteGitDeploy.Security {
    public class Session : ISession {

        public bool IsAvailable { get; private set; }
        public string Id { get; }
        public IEnumerable<string> Keys { get; }

        public Session(HttpContext httpContext) {
            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var value)) return;
            string[] data = value.ToString().Split(" ", 2);
            if (data.Length == 2 && data[0].Equals("Bearer")) {
                Id = data[1];
            }
        }

        public async Task LoadAsync(CancellationToken cancellationToken = new CancellationToken()) {
            if (!string.IsNullOrEmpty(Id)) {
                IsAvailable = await HtcPlugin.CacheManager.IsValidSessionAsync(Id);
            }
        }

        public Task CommitAsync(CancellationToken cancellationToken = new CancellationToken()) {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out byte[] value) {
            throw new NotImplementedException();
        }

        public void Set(string key, byte[] value) {
            throw new NotImplementedException();
        }

        public void Remove(string key) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }
    }
}
