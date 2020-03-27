using Microsoft.EntityFrameworkCore.Migrations;

namespace MaterialeManager.Migrations
{
    public partial class AddedErrorDescriptionToCaseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorDescription",
                table: "Case",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorDescription",
                table: "Case");
        }
    }
}
