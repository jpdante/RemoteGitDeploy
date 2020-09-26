using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemoteGitDeploy.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Guid = table.Column<string>(maxLength: 36, nullable: false),
                    FirstName = table.Column<string>(maxLength: 255, nullable: false),
                    LastName = table.Column<string>(maxLength: 255, nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    Username = table.Column<string>(maxLength: 64, nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Permissions = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    LastAccess = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AccountId = table.Column<long>(nullable: false),
                    Ip = table.Column<string>(maxLength: 39, nullable: false),
                    AccessDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessHistory_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Snippets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Guid = table.Column<string>(maxLength: 36, nullable: false),
                    CreatorId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(maxLength: 100000, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snippets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Snippets_Accounts_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Guid = table.Column<string>(maxLength: 36, nullable: false),
                    CreatorId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 10000, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Accounts_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SnippetFiles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    SnippetId = table.Column<long>(nullable: false),
                    Filename = table.Column<string>(maxLength: 255, nullable: false),
                    Content = table.Column<string>(nullable: false),
                    Language = table.Column<string>(maxLength: 32, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnippetFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnippetFiles_Snippets_SnippetId",
                        column: x => x.SnippetId,
                        principalTable: "Snippets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Guid = table.Column<string>(maxLength: 36, nullable: false),
                    OwnerId = table.Column<long>(nullable: false),
                    Username = table.Column<string>(maxLength: 64, nullable: false),
                    PersonalAccessToken = table.Column<string>(maxLength: 40, nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Git = table.Column<string>(maxLength: 255, nullable: false),
                    Branch = table.Column<string>(maxLength: 64, nullable: false),
                    TeamId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(maxLength: 10000, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repositories_Accounts_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repositories_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    TeamId = table.Column<long>(nullable: false),
                    AccountId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActionHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Guid = table.Column<string>(maxLength: 36, nullable: false),
                    RepositoryId = table.Column<long>(nullable: false),
                    Icon = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Parameters = table.Column<string>(nullable: true),
                    Log = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    FinishTime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionHistory_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessHistory_AccountId",
                table: "AccessHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Guid",
                table: "Accounts",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionHistory_Guid",
                table: "ActionHistory",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionHistory_RepositoryId",
                table: "ActionHistory",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_Guid",
                table: "Repositories",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_Name",
                table: "Repositories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_OwnerId",
                table: "Repositories",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_TeamId",
                table: "Repositories",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SnippetFiles_SnippetId",
                table: "SnippetFiles",
                column: "SnippetId");

            migrationBuilder.CreateIndex(
                name: "IX_Snippets_CreatorId",
                table: "Snippets",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Snippets_Guid",
                table: "Snippets",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_AccountId",
                table: "TeamMembers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId_AccountId",
                table: "TeamMembers",
                columns: new[] { "TeamId", "AccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CreatorId",
                table: "Teams",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Guid",
                table: "Teams",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessHistory");

            migrationBuilder.DropTable(
                name: "ActionHistory");

            migrationBuilder.DropTable(
                name: "SnippetFiles");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "Repositories");

            migrationBuilder.DropTable(
                name: "Snippets");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
