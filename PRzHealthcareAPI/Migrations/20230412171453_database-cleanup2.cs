using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRzHealthcareAPI.Migrations
{
    public partial class databasecleanup2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Vaccinations_Eve_VacId",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "Eve_VacId",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Vaccinations_Eve_VacId",
                table: "Events",
                column: "Eve_VacId",
                principalTable: "Vaccinations",
                principalColumn: "Vac_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Vaccinations_Eve_VacId",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "Eve_VacId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Vaccinations_Eve_VacId",
                table: "Events",
                column: "Eve_VacId",
                principalTable: "Vaccinations",
                principalColumn: "Vac_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
