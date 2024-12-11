using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItiUmplemFrigiderul.Data.Migrations
{
    /// <inheritdoc />
    public partial class M3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Farms",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Farms_UserId",
                table: "Farms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_AspNetUsers_UserId",
                table: "Farms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_AspNetUsers_UserId",
                table: "Farms");

            migrationBuilder.DropIndex(
                name: "IX_Farms_UserId",
                table: "Farms");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Farms");
        }
    }
}
