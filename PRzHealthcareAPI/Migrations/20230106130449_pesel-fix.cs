using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRzHealthcareAPI.Migrations
{
    public partial class peselfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Acc_Pesel",
                table: "Accounts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Acc_Pesel",
                table: "Accounts",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
