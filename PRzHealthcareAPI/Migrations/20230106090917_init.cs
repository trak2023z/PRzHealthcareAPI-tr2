using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRzHealthcareAPI.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    Aty_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aty_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.Aty_Id);
                });

            migrationBuilder.CreateTable(
                name: "BinData",
                columns: table => new
                {
                    Bin_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bin_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Bin_Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bin_InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Bin_InsertedAccId = table.Column<int>(type: "int", nullable: false),
                    Bin_ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Bin_ModifiedAccId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinData", x => x.Bin_Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTypes",
                columns: table => new
                {
                    Nty_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nty_Name = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false),
                    Nty_Template = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypes", x => x.Nty_Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Acc_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Acc_AtyId = table.Column<int>(type: "int", nullable: false),
                    Acc_Login = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Acc_Password = table.Column<string>(type: "nvarchar(2047)", maxLength: 2047, nullable: false),
                    Acc_PhotoId = table.Column<int>(type: "int", nullable: false),
                    Acc_Firstname = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Acc_Secondname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acc_Lastname = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Acc_DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Acc_Pesel = table.Column<int>(type: "int", nullable: false),
                    Acc_Email = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false),
                    Acc_ContactNumber = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Acc_IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Acc_InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Acc_ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Acc_Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountTypes_Acc_AtyId",
                        column: x => x.Acc_AtyId,
                        principalTable: "AccountTypes",
                        principalColumn: "Aty_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vaccinations",
                columns: table => new
                {
                    Vac_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vac_Name = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false),
                    Vac_Description = table.Column<string>(type: "nvarchar(2047)", maxLength: 2047, nullable: false),
                    Vac_PhotoId = table.Column<int>(type: "int", nullable: false),
                    Vac_DaysBetweenVacs = table.Column<int>(type: "int", nullable: false),
                    Vac_IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Vac_InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Vac_InsertedAccId = table.Column<int>(type: "int", nullable: false),
                    Vac_ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Vac_ModifiedAccId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccinations", x => x.Vac_Id);
                    table.ForeignKey(
                        name: "FK_Vaccinations_BinData_Vac_PhotoId",
                        column: x => x.Vac_PhotoId,
                        principalTable: "BinData",
                        principalColumn: "Bin_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Cer_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cer_AccId = table.Column<int>(type: "int", nullable: false),
                    Cer_BinId = table.Column<int>(type: "int", nullable: false),
                    Cer_IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Cer_InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cer_InsertedAccId = table.Column<int>(type: "int", nullable: false),
                    Cer_ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cer_ModifiedAccId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Cer_Id);
                    table.ForeignKey(
                        name: "FK_Certificates_Accounts_Cer_AccId",
                        column: x => x.Cer_AccId,
                        principalTable: "Accounts",
                        principalColumn: "Acc_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Certificates_BinData_Cer_BinId",
                        column: x => x.Cer_BinId,
                        principalTable: "BinData",
                        principalColumn: "Bin_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Eve_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Eve_AccId = table.Column<int>(type: "int", nullable: false),
                    Eve_TimeFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Eve_TimeTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Eve_Type = table.Column<int>(type: "int", nullable: false),
                    Eve_DoctorId = table.Column<int>(type: "int", nullable: false),
                    Eve_VacId = table.Column<int>(type: "int", nullable: false),
                    Eve_Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Eve_IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Eve_InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Eve_InsertedAccId = table.Column<int>(type: "int", nullable: false),
                    Eve_ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Eve_ModifiedAccId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Eve_Id);
                    table.ForeignKey(
                        name: "FK_Events_Accounts_Eve_AccId",
                        column: x => x.Eve_AccId,
                        principalTable: "Accounts",
                        principalColumn: "Acc_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_Vaccinations_Eve_VacId",
                        column: x => x.Eve_VacId,
                        principalTable: "Vaccinations",
                        principalColumn: "Vac_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Not_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Not_EveId = table.Column<int>(type: "int", nullable: false),
                    Not_NtyId = table.Column<int>(type: "int", nullable: false),
                    Not_SendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Not_IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Not_InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Not_InsertedAccId = table.Column<int>(type: "int", nullable: false),
                    Not_ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Not_ModifiedAccId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Not_Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Events_Not_EveId",
                        column: x => x.Not_EveId,
                        principalTable: "Events",
                        principalColumn: "Eve_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_NotificationTypes_Not_NtyId",
                        column: x => x.Not_NtyId,
                        principalTable: "NotificationTypes",
                        principalColumn: "Nty_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Acc_AtyId",
                table: "Accounts",
                column: "Acc_AtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_Cer_AccId",
                table: "Certificates",
                column: "Cer_AccId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_Cer_BinId",
                table: "Certificates",
                column: "Cer_BinId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Eve_AccId",
                table: "Events",
                column: "Eve_AccId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Eve_VacId",
                table: "Events",
                column: "Eve_VacId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Not_EveId",
                table: "Notifications",
                column: "Not_EveId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Not_NtyId",
                table: "Notifications",
                column: "Not_NtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Vaccinations_Vac_PhotoId",
                table: "Vaccinations",
                column: "Vac_PhotoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "NotificationTypes");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Vaccinations");

            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropTable(
                name: "BinData");
        }
    }
}
