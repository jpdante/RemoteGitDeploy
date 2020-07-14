using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RemoteGitDeploy.Manager {
    public class CacheManager {

        private readonly string _configuration;
        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _database;

        public CacheManager(string configuration) {
            _configuration = configuration;
        }

        public async Task ConnectAsync() {
            _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(_configuration);
            _database = _connectionMultiplexer.GetDatabase(0);
        }

        public async Task DisconnectAsync() {
            await _connectionMultiplexer.CloseAsync();
            _connectionMultiplexer.Dispose();
        }

        #region Session

        public async Task CreateSessionAsync(string token, long userId, TimeSpan expiry) {
            await _database.StringSetAsync("session." + token, userId.ToString(), expiry);
        }

        public async Task<bool> IsValidSessionAsync(string token) {
            return await _database.KeyExistsAsync("session." + token);
        }

        public async Task<bool> DeleteSessionAsync(string token) {
            return await _database.KeyDeleteAsync("session." + token);
        }

        public async Task<long> GetUserIdFromSessionAsync(string token) {
            string data = await _database.StringGetAsync("session." + token);
            return long.TryParse(data, out long result) ? result : -1;
        }

        #endregion
    }
}
