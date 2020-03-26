using Microsoft.EntityFrameworkCore.Migrations;

namespace MaterialeManager.Migrations
{
    public partial class AddedChangesToCaseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Case");

            migrationBuilder.AddColumn<string>(
                name: "PhotographerID",
                table: "Case",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Case_PhotographerID",
                table: "Case",
                column: "PhotographerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Case_AspNetUsers_PhotographerID",
                table: "Case",
                column: "PhotographerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Case_AspNetUsers_PhotographerID",
                table: "Case");

            migrationBuilder.DropIndex(
                name: "IX_Case_PhotographerID",
                table: "Case");

            migrationBuilder.DropColumn(
                name: "PhotographerID",
                table: "Case");

            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "Case",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
