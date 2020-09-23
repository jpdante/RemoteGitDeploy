﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RemoteGitDeploy;

namespace RemoteGitDeploy.Migrations
{
    [DbContext(typeof(RgdContext))]
    [Migration("20200923025411_AddPermission")]
    partial class AddPermission
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("RemoteGitDeploy.Model.New.AccessHistory", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("AccessDate")
                        .HasColumnType("TIMESTAMP");

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("varchar(39) CHARACTER SET utf8mb4")
                        .HasMaxLength(39);

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("AccessHistory");
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.Account", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TIMESTAMP");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("Guid")
                        .IsRequired()
                        .HasColumnType("varchar(36) CHARACTER SET utf8mb4")
                        .HasMaxLength(36);

                    b.Property<DateTime>("LastAccess")
                        .HasColumnType("TIMESTAMP");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Permissions")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("varchar(64) CHARACTER SET utf8mb4")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Guid")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.ActionHistory", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("Guid")
                        .IsRequired()
                        .HasColumnType("varchar(36) CHARACTER SET utf8mb4")
                        .HasMaxLength(36);

                    b.Property<string>("Log")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Parameters")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<long>("RepositoryId")
                        .HasColumnType("bigint");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Guid")
                        .IsUnique();

                    b.HasIndex("RepositoryId");

                    b.ToTable("ActionHistory");
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.Repository", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("Branch")
                        .IsRequired()
                        .HasColumnType("varchar(64) CHARACTER SET utf8mb4")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TIMESTAMP");

                    b.Property<long>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(10000);

                    b.Property<string>("Git")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("Guid")
                        .IsRequired()
                        .HasColumnType("varchar(36) CHARACTER SET utf8mb4")
                        .HasMaxLength(36);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(64) CHARACTER SET utf8mb4")
                        .HasMaxLength(64);

                    b.Property<long>("TeamId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("Guid")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("TeamId");

                    b.ToTable("Repositories");
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.Snippet", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TIMESTAMP");

                    b.Property<long>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(100000);

                    b.Property<string>("Guid")
                        .IsRequired()
                        .HasColumnType("varchar(36) CHARACTER SET utf8mb4")
                        .HasMaxLength(36);

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("Guid")
                        .IsUnique();

                    b.ToTable("Snippets");
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.SnippetFile", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TIMESTAMP");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("varchar(32) CHARACTER SET utf8mb4")
                        .HasMaxLength(32);

                    b.Property<long>("SnippetId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SnippetId");

                    b.ToTable("SnippetFiles");
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.Team", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TIMESTAMP");

                    b.Property<long>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(10000);

                    b.Property<string>("Guid")
                        .IsRequired()
                        .HasColumnType("varchar(36) CHARACTER SET utf8mb4")
                        .HasMaxLength(36);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(64) CHARACTER SET utf8mb4")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("Guid")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.TeamMember", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint");

                    b.Property<long>("TeamId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("TeamId", "AccountId")
                        .IsUnique();

                    b.ToTable("TeamMembers");
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.AccessHistory", b =>
                {
                    b.HasOne("RemoteGitDeploy.Model.New.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.ActionHistory", b =>
                {
                    b.HasOne("RemoteGitDeploy.Model.New.Repository", "Repository")
                        .WithMany()
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.Repository", b =>
                {
                    b.HasOne("RemoteGitDeploy.Model.New.Account", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RemoteGitDeploy.Model.New.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.Snippet", b =>
                {
                    b.HasOne("RemoteGitDeploy.Model.New.Account", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.SnippetFile", b =>
                {
                    b.HasOne("RemoteGitDeploy.Model.New.Snippet", "Snippet")
                        .WithMany()
                        .HasForeignKey("SnippetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.Team", b =>
                {
                    b.HasOne("RemoteGitDeploy.Model.New.Account", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RemoteGitDeploy.Model.New.TeamMember", b =>
                {
                    b.HasOne("RemoteGitDeploy.Model.New.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RemoteGitDeploy.Model.New.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
