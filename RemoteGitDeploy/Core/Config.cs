using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteGitDeploy.Core {
    public class Config {

        public DatabaseConfig Database;
        public RedisConfig Redis;
        public GitConfig Git;
        public OtherConfig Other;

        public Config() {
            Database = new DatabaseConfig();
            Redis = new RedisConfig();
            Git = new GitConfig();
            Other = new OtherConfig();
        }

        public class DatabaseConfig {

            public string Host { get; set; }
            public int Port { get; set; }
            public string Database { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }

            public DatabaseConfig() {
                Host = "127.0.0.1";
                Port = 3306;
                Database = "remotegitdeploy";
                Username = "root";
                Password = "root";
            }
        }

        public class RedisConfig {

            public string ConnectionString;
            public int Database;

            public RedisConfig() {
                ConnectionString = "localhost";
                Database = 0;
            }
        }

        public class GitConfig {

            public string RepositoriesDirectory;

            public GitConfig() {
                RepositoriesDirectory = "./rgd/";
            }
        }

        public class OtherConfig {

            public string Domain;
            public string SecretKey;

            public OtherConfig() {
                Domain = "domain.com";
                SecretKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            }
        }
    }
}
