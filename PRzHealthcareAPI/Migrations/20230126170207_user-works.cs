using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRzHealthcareAPI.Migrations
{
    public partial class userworks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Acc_RegistrationHash",
                table: "Accounts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Acc_ReminderExpire",
                table: "Accounts",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Acc_ReminderHash",
                table: "Accounts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acc_RegistrationHash",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Acc_ReminderExpire",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Acc_ReminderHash",
                table: "Accounts");
        }
    }
}
