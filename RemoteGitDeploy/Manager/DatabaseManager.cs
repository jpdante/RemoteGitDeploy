using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using MySql.Data.MySqlClient;

namespace RemoteGitDeploy.Manager {
    public class DatabaseManager {

        private readonly string _connectString;

        public DatabaseManager(string connString) {
            _connectString = connString;
        }

        public async Task<MySqlConnection> GetConnectionAsync() {
            try {
                var conn = new MySqlConnection(_connectString);
                await conn.OpenAsync();
                return conn;
            } catch (Exception ex) {
                HtcPlugin.Logger.LogTrace("Could not connect to the database!", ex);
                await Task.Delay(TimeSpan.FromSeconds(3));
                return null;
            }
        }

        public async Task CloseConnectionAsync(MySqlConnection connection) {
            await connection.CloseAsync();
            await connection.DisposeAsync();
        }

    }
}
