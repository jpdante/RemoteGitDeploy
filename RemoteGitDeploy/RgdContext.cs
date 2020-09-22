using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RemoteGitDeploy.Model.New;

namespace RemoteGitDeploy {
    public class RgdContext : DbContext {

        private static string _connectionString = "server=127.0.0.1;port=3306;database=remotegitdeploy;user=root;password=root";

        public DbSet<AccessHistory> AccessHistory { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ActionHistory> ActionHistory { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Snippet> Snippets { get; set; }
        public DbSet<SnippetFile> SnippetFiles { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }

        public static void SetConnectionString(string connectionString) {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseMySql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AccessHistory>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.AccountId);
            });

            modelBuilder.Entity<Account>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.Guid).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            modelBuilder.Entity<ActionHistory>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.Guid).IsUnique();
                entity.HasIndex(e => e.RepositoryId);
            });

            modelBuilder.Entity<Repository>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.Guid).IsUnique();
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.CreatorId);
            });

            modelBuilder.Entity<Snippet>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.Guid).IsUnique();
                entity.HasIndex(e => e.CreatorId);
            });

            modelBuilder.Entity<SnippetFile>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.SnippetId);
            });

            modelBuilder.Entity<Team>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.Guid).IsUnique();
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.CreatorId);
            });

            modelBuilder.Entity<TeamMember>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => new { e.TeamId, e.AccountId }).IsUnique();
            });
        }
    }
}
