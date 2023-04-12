using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRzHealthcareAPI.Migrations
{
    public partial class databasecleanup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Accounts_Eve_AccId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Vaccinations_BinData_Vac_PhotoId",
                table: "Vaccinations");

            migrationBuilder.AlterColumn<int>(
                name: "Vac_PhotoId",
                table: "Vaccinations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Eve_DoctorId",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Eve_AccId",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Acc_PhotoId",
                table: "Accounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Accounts_Eve_AccId",
                table: "Events",
                column: "Eve_AccId",
                principalTable: "Accounts",
                principalColumn: "Acc_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vaccinations_BinData_Vac_PhotoId",
                table: "Vaccinations",
                column: "Vac_PhotoId",
                principalTable: "BinData",
                principalColumn: "Bin_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Accounts_Eve_AccId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Vaccinations_BinData_Vac_PhotoId",
                table: "Vaccinations");

            migrationBuilder.AlterColumn<int>(
                name: "Vac_PhotoId",
                table: "Vaccinations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Eve_DoctorId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Eve_AccId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Acc_PhotoId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Accounts_Eve_AccId",
                table: "Events",
                column: "Eve_AccId",
                principalTable: "Accounts",
                principalColumn: "Acc_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vaccinations_BinData_Vac_PhotoId",
                table: "Vaccinations",
                column: "Vac_PhotoId",
                principalTable: "BinData",
                principalColumn: "Bin_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
