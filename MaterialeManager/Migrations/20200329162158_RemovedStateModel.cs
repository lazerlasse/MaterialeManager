using Microsoft.EntityFrameworkCore.Migrations;

namespace MaterialeManager.Migrations
{
    public partial class RemovedStateModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Case_CaseStates_CaseStateID",
                table: "Case");

            migrationBuilder.DropTable(
                name: "CaseStates");

            migrationBuilder.DropIndex(
                name: "IX_Case_CaseStateID",
                table: "Case");

            migrationBuilder.DropColumn(
                name: "CaseStateID",
                table: "Case");

            migrationBuilder.AddColumn<int>(
                name: "CaseState",
                table: "Case",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseState",
                table: "Case");

            migrationBuilder.AddColumn<int>(
                name: "CaseStateID",
                table: "Case",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CaseStates",
                columns: table => new
                {
                    CaseStateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseStates", x => x.CaseStateID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Case_CaseStateID",
                table: "Case",
                column: "CaseStateID");

            migrationBuilder.AddForeignKey(
                name: "FK_Case_CaseStates_CaseStateID",
                table: "Case",
                column: "CaseStateID",
                principalTable: "CaseStates",
                principalColumn: "CaseStateID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
