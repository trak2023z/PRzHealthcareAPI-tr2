using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRzHealthcareAPI.Migrations
{
    public partial class dateofbirthfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Acc_DateOfBirth",
                table: "Accounts",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Acc_DateOfBirth",
                table: "Accounts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
