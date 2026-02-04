using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "Userid",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
