﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PRzHealthcareAPI.Models;

#nullable disable

namespace PRzHealthcareAPI.Migrations
{
    [DbContext(typeof(HealthcareDbContext))]
    partial class HealthcareDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("PRzHealthcareAPI.Models.Account", b =>
                {
                    b.Property<int>("Acc_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Acc_Id"), 1L, 1);

                    b.Property<int>("Acc_AtyId")
                        .HasColumnType("int");

                    b.Property<string>("Acc_ContactNumber")
                        .IsRequired()
                        .HasMaxLength(63)
                        .HasColumnType("nvarchar(63)");

                    b.Property<DateTime>("Acc_DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("Acc_Email")
                        .IsRequired()
                        .HasMaxLength(127)
                        .HasColumnType("nvarchar(127)");

                    b.Property<string>("Acc_Firstname")
                        .IsRequired()
                        .HasMaxLength(63)
                        .HasColumnType("nvarchar(63)");

                    b.Property<DateTime>("Acc_InsertedDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("Acc_IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Acc_Lastname")
                        .IsRequired()
                        .HasMaxLength(63)
                        .HasColumnType("nvarchar(63)");

                    b.Property<string>("Acc_Login")
                        .IsRequired()
                        .HasMaxLength(63)
                        .HasColumnType("nvarchar(63)");

                    b.Property<DateTime>("Acc_ModifiedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Acc_Password")
                        .IsRequired()
                        .HasMaxLength(2047)
                        .HasColumnType("nvarchar(2047)");

                    b.Property<long>("Acc_Pesel")
                        .HasColumnType("bigint");

                    b.Property<int>("Acc_PhotoId")
                        .HasColumnType("int");

                    b.Property<string>("Acc_RegistrationHash")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("Acc_ReminderExpire")
                        .HasColumnType("datetime");

                    b.Property<string>("Acc_ReminderHash")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Acc_Secondname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Acc_Id");

                    b.HasIndex("Acc_AtyId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.AccountType", b =>
                {
                    b.Property<int>("Aty_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Aty_Id"), 1L, 1);

                    b.Property<string>("Aty_Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Aty_Id");

                    b.ToTable("AccountTypes");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.BinData", b =>
                {
                    b.Property<int>("Bin_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Bin_Id"), 1L, 1);

                    b.Property<string>("Bin_Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Bin_InsertedAccId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Bin_InsertedDate")
                        .HasColumnType("datetime");

                    b.Property<int>("Bin_ModifiedAccId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Bin_ModifiedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Bin_Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Bin_Id");

                    b.ToTable("BinData");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.Certificate", b =>
                {
                    b.Property<int>("Cer_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Cer_Id"), 1L, 1);

                    b.Property<int>("Cer_AccId")
                        .HasColumnType("int");

                    b.Property<int>("Cer_BinId")
                        .HasColumnType("int");

                    b.Property<int>("Cer_InsertedAccId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Cer_InsertedDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("Cer_IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("Cer_ModifiedAccId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Cer_ModifiedDate")
                        .HasColumnType("datetime");

                    b.HasKey("Cer_Id");

                    b.HasIndex("Cer_AccId");

                    b.HasIndex("Cer_BinId");

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.Event", b =>
                {
                    b.Property<int>("Eve_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Eve_Id"), 1L, 1);

                    b.Property<int>("Eve_AccId")
                        .HasColumnType("int");

                    b.Property<string>("Eve_Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("Eve_DoctorId")
                        .HasColumnType("int");

                    b.Property<int>("Eve_InsertedAccId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Eve_InsertedDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("Eve_IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("Eve_ModifiedAccId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Eve_ModifiedDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("Eve_TimeFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Eve_TimeTo")
                        .HasColumnType("datetime2");

                    b.Property<int>("Eve_Type")
                        .HasColumnType("int");

                    b.Property<int>("Eve_VacId")
                        .HasColumnType("int");

                    b.HasKey("Eve_Id");

                    b.HasIndex("Eve_AccId");

                    b.HasIndex("Eve_VacId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.Notification", b =>
                {
                    b.Property<int>("Not_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Not_Id"), 1L, 1);

                    b.Property<int>("Not_EveId")
                        .HasColumnType("int");

                    b.Property<int>("Not_InsertedAccId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Not_InsertedDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("Not_IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("Not_ModifiedAccId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Not_ModifiedDate")
                        .HasColumnType("datetime");

                    b.Property<int>("Not_NtyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Not_SendTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Not_Id");

                    b.HasIndex("Not_EveId");

                    b.HasIndex("Not_NtyId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.NotificationType", b =>
                {
                    b.Property<int>("Nty_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Nty_Id"), 1L, 1);

                    b.Property<string>("Nty_Name")
                        .IsRequired()
                        .HasMaxLength(127)
                        .HasColumnType("nvarchar(127)");

                    b.Property<string>("Nty_Template")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Nty_Id");

                    b.ToTable("NotificationTypes");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.Vaccination", b =>
                {
                    b.Property<int>("Vac_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Vac_Id"), 1L, 1);

                    b.Property<int>("Vac_DaysBetweenVacs")
                        .HasColumnType("int");

                    b.Property<string>("Vac_Description")
                        .IsRequired()
                        .HasMaxLength(2047)
                        .HasColumnType("nvarchar(2047)");

                    b.Property<int>("Vac_InsertedAccId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Vac_InsertedDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("Vac_IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("Vac_ModifiedAccId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Vac_ModifiedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Vac_Name")
                        .IsRequired()
                        .HasMaxLength(127)
                        .HasColumnType("nvarchar(127)");

                    b.Property<int>("Vac_PhotoId")
                        .HasColumnType("int");

                    b.HasKey("Vac_Id");

                    b.HasIndex("Vac_PhotoId");

                    b.ToTable("Vaccinations");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.Account", b =>
                {
                    b.HasOne("PRzHealthcareAPI.Models.AccountType", "AccountType")
                        .WithMany()
                        .HasForeignKey("Acc_AtyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountType");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.Certificate", b =>
                {
                    b.HasOne("PRzHealthcareAPI.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("Cer_AccId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PRzHealthcareAPI.Models.BinData", "BinData")
                        .WithMany()
                        .HasForeignKey("Cer_BinId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("BinData");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.Event", b =>
                {
                    b.HasOne("PRzHealthcareAPI.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("Eve_AccId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PRzHealthcareAPI.Models.Vaccination", "Vaccination")
                        .WithMany()
                        .HasForeignKey("Eve_VacId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Vaccination");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.Notification", b =>
                {
                    b.HasOne("PRzHealthcareAPI.Models.Event", "Event")
                        .WithMany()
                        .HasForeignKey("Not_EveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PRzHealthcareAPI.Models.NotificationType", "NotificationType")
                        .WithMany()
                        .HasForeignKey("Not_NtyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("NotificationType");
                });

            modelBuilder.Entity("PRzHealthcareAPI.Models.Vaccination", b =>
                {
                    b.HasOne("PRzHealthcareAPI.Models.BinData", "BinData")
                        .WithMany()
                        .HasForeignKey("Vac_PhotoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BinData");
                });
#pragma warning restore 612, 618
        }
    }
}
