using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using MySql.Data.MySqlClient;
using RemoteGitDeploy.Model.Database;
using RemoteGitDeploy.Utils;

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

        public async Task<MySqlTransaction> GetTransactionAsync(MySqlConnection conn) {
            return await conn.BeginTransactionAsync();
        }

        public async Task CloseConnectionAsync(MySqlConnection connection) {
            await connection.CloseAsync();
            await connection.DisposeAsync();
        }

        #region Account

        public async Task<bool> NewAccountAsync(string firstName, string lastName, string email, string username, string password, string salt, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("INSERT INTO accounts (id, first_name, last_name, email, username, password, salt) VALUES (@id, @firstName, @lastName, @email, @username, @password, @salt);", conn);
            cmd.Parameters.AddWithValue("id", StaticData.IdGenerator.CreateId());
            cmd.Parameters.AddWithValue("firstName", firstName);
            cmd.Parameters.AddWithValue("lastName", lastName);
            cmd.Parameters.AddWithValue("email", email);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("password", password);
            cmd.Parameters.AddWithValue("salt", salt);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> HasUsernameAsync(string username, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("SELECT username FROM accounts WHERE username = @username;", conn);
            cmd.Parameters.AddWithValue("username", username);
            await using var reader = await cmd.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public async Task<bool> HasEmailAsync(string email, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("SELECT email FROM accounts WHERE email = @email;", conn);
            cmd.Parameters.AddWithValue("email", email);
            await using var reader = await cmd.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public async Task<Account> GetAccountAsync(string email, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("SELECT id, password, salt, username FROM accounts WHERE accounts.email = @email;", conn);
            cmd.Parameters.AddWithValue("email", email);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return null;
            while (await reader.ReadAsync()) {
                return new Account(reader);
            }
            return null;
        }

        #endregion

        #region Team

        public async Task<bool> NewTeamAsync(string name, string description, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("INSERT INTO teams (id, name, description) VALUES (@id, @name, @description);", conn);
            cmd.Parameters.AddWithValue("id", StaticData.IdGenerator.CreateId());
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("description", description);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> HasTeamAsync(string name, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("SELECT id FROM teams WHERE name = @name;", conn);
            cmd.Parameters.AddWithValue("name", name);
            await using var reader = await cmd.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public async Task<long> HasTeamWithResultAsync(string name, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("SELECT id FROM teams WHERE name = @name;", conn);
            cmd.Parameters.AddWithValue("name", name);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return -1;
            while (await reader.ReadAsync()) {
                return reader.GetInt64(0);
            }
            return -1;
        }

        public async Task<Team[]> GetTeamsAsync(MySqlConnection conn) {
            var teams = new List<Team>();
            await using var cmd = new MySqlCommand("SELECT id, name, description FROM teams;", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return teams.ToArray();
            while (await reader.ReadAsync()) {
                teams.Add(new Team(reader));
            }
            return teams.ToArray();
        }

        public async Task<Team> GetTeamAsync(string name, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("SELECT id, name, description FROM teams WHERE name = @name;", conn);
            cmd.Parameters.AddWithValue("name", name);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return null;
            while (await reader.ReadAsync()) {
                return new Team(reader);
            }
            return null;
        }

        public async Task<Team> GetTeamAsync(long id, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("SELECT id, name, description FROM teams WHERE id = @id;", conn);
            cmd.Parameters.AddWithValue("id", id);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return null;
            while (await reader.ReadAsync()) {
                return new Team(reader);
            }
            return null;
        }

        #endregion

        #region Snippet

        public async Task<bool> NewSnippetAsync(long id, string guid, long account, string description, MySqlConnection conn, MySqlTransaction transaction = null) {
            await using var cmd = new MySqlCommand("INSERT INTO snippets (id, guid, account, description) VALUES (@id, @guid, @account, @description);", conn, transaction);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("guid", guid);
            cmd.Parameters.AddWithValue("account", account);
            cmd.Parameters.AddWithValue("description", description);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        /*public async Task<long> GetSnippetIdAsync(string guid, MySqlConnection conn, MySqlTransaction transaction = null) {
            await using var cmd = new MySqlCommand("SELECT id FROM snippets WHERE guid = @guid;", conn, transaction);
            cmd.Parameters.AddWithValue("guid", guid);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return -1;
            while (await reader.ReadAsync()) {
                return reader.GetInt64(0);
            }
            return -1;
        }*/

        public async Task<bool> NewSnippetFileAsync(long snippet, string filename, string code, string language, MySqlConnection conn, MySqlTransaction transaction = null) {
            await using var cmd = new MySqlCommand("INSERT INTO snippet_files (id, snippet, filename, code, language) VALUES (@id, @snippet, @filename, @code, @language);", conn, transaction);
            cmd.Parameters.AddWithValue("id", StaticData.IdGenerator.CreateId());
            cmd.Parameters.AddWithValue("snippet", snippet);
            cmd.Parameters.AddWithValue("filename", filename);
            cmd.Parameters.AddWithValue("code", code);
            cmd.Parameters.AddWithValue("language", language);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<Snippet[]> GetSnippetsAsync(MySqlConnection conn) {
            var snippets = new List<Snippet>();
            await using var cmd = new MySqlCommand("SELECT id, guid, account, description FROM snippets;", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return snippets.ToArray();
            while (await reader.ReadAsync()) {
                snippets.Add(new Snippet(reader));
            }
            return snippets.ToArray();
        }

        public async Task<Snippet> GetSnippetAsync(string guid, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("SELECT id, guid, account, description FROM snippets WHERE guid = @guid;", conn);
            cmd.Parameters.AddWithValue("guid", guid);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return null;
            while (await reader.ReadAsync()) {
                return new Snippet(reader);
            }
            return null;
        }

        public async Task<string[]> GetSnippetsFileNamesAsync(long snippet, MySqlConnection conn) {
            var fileNames = new List<string>();
            await using var cmd = new MySqlCommand("SELECT filename FROM snippet_files WHERE snippet = @snippet;", conn);
            cmd.Parameters.AddWithValue("snippet", snippet);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return fileNames.ToArray();
            while (await reader.ReadAsync()) {
                fileNames.Add(reader.GetString(0));
            }
            return fileNames.ToArray();
        }

        public async Task<SnippetFile[]> GetSnippetFilesAsync(long snippet, MySqlConnection conn) {
            var snippetFiles = new List<SnippetFile>();
            await using var cmd = new MySqlCommand("SELECT id, filename, code, language FROM snippet_files WHERE snippet = @snippet;", conn);
            cmd.Parameters.AddWithValue("snippet", snippet);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return snippetFiles.ToArray();
            while (await reader.ReadAsync()) {
                snippetFiles.Add(new SnippetFile(reader, snippet));
            }
            return snippetFiles.ToArray();
        }

        #endregion

        #region Repository

        public async Task<bool> NewRepositoryAsync(long id, string guid, long creator, string name, string git, long team, string description, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("INSERT INTO repositories (id, guid, creator, name, git, team, description) VALUES (@id, @guid, @creator, @name, @git, @team, @description);", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("guid", guid);
            cmd.Parameters.AddWithValue("creator", creator);
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("git", git);
            cmd.Parameters.AddWithValue("team", team);
            cmd.Parameters.AddWithValue("description", description);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> HasRepositoryAsync(string name, MySqlConnection conn) {
            await using var cmd = new MySqlCommand("SELECT id FROM repositories WHERE name = @name;", conn);
            cmd.Parameters.AddWithValue("name", name);
            await using var reader = await cmd.ExecuteReaderAsync();
            return reader.HasRows;
        }

        #endregion
    }
}
