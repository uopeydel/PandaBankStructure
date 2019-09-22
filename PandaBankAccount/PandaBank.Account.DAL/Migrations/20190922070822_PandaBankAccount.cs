using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PandaBank.Account.DAL.Migrations
{
    public partial class PandaBankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PandaAccount",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Balances = table.Column<double>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PandaAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PandaStatement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PandaAccountId = table.Column<string>(nullable: false),
                    Balances = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PandaStatement", x => new { x.PandaAccountId, x.Id });
                    table.UniqueConstraint("AK_PandaStatement_Id_PandaAccountId", x => new { x.Id, x.PandaAccountId });
                    table.ForeignKey(
                        name: "FK_PandaStatement_PandaAccount_PandaAccountId",
                        column: x => x.PandaAccountId,
                        principalTable: "PandaAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                columns: table => new
                {
                    PandaUserId = table.Column<long>(nullable: false),
                    PandaAccountId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccount", x => new { x.PandaAccountId, x.PandaUserId });
                    table.ForeignKey(
                        name: "FK_UserAccount_PandaAccount_PandaAccountId",
                        column: x => x.PandaAccountId,
                        principalTable: "PandaAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PandaStatement");

            migrationBuilder.DropTable(
                name: "UserAccount");

            migrationBuilder.DropTable(
                name: "PandaAccount");
        }
    }
}
