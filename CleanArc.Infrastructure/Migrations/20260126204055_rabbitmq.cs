using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class rabbitmq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AspNetUsers_Userid",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_Userid",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_Useridreq",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_Userid",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_Useridreq",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Animals_Userid",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Userid",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AdoptionAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnimalId = table.Column<int>(type: "int", nullable: false),
                    AnimalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnimalType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AdopterId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AdopterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AdopterEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PreviousOwnerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PreviousOwnerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AdoptedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAuditLogs_AdoptedAt",
                table: "AdoptionAuditLogs",
                column: "AdoptedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAuditLogs_AdopterId",
                table: "AdoptionAuditLogs",
                column: "AdopterId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAuditLogs_AnimalId",
                table: "AdoptionAuditLogs",
                column: "AnimalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionAuditLogs");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Userid",
                table: "Animals",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Userid",
                table: "Requests",
                column: "Userid");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Useridreq",
                table: "Requests",
                column: "Useridreq");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_Userid",
                table: "Animals",
                column: "Userid");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AspNetUsers_Userid",
                table: "Animals",
                column: "Userid",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_Userid",
                table: "Requests",
                column: "Userid",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_Useridreq",
                table: "Requests",
                column: "Useridreq",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
