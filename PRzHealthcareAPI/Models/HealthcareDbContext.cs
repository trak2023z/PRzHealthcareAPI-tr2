using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace PRzHealthcareAPI.Models
{
    public class HealthcareDbContext : DbContext
    {
        //KOMPUTEREK\PC
        //DESKTOP-TKR85BK
        private string _connectionString = $@"Server=KOMPUTEREK\PC;User Id=sa;Password=Mateusz1;Database=PRzHealthcare;Trusted_Connection=True;Trust Server Certificate=true";
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<BinData> BinData { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<Vaccination> Vaccinations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(x => x.Acc_Id);
            modelBuilder.Entity<Account>().Property(x => x.Acc_Login).IsRequired().HasMaxLength(63);
            modelBuilder.Entity<Account>().Property(x => x.Acc_Password).IsRequired().HasMaxLength(2047);
            modelBuilder.Entity<Account>().Property(x => x.Acc_Firstname).IsRequired().HasMaxLength(63);
            modelBuilder.Entity<Account>().Property(x => x.Acc_Lastname).IsRequired().HasMaxLength(63);
            modelBuilder.Entity<Account>().Property(x => x.Acc_DateOfBirth).IsRequired().HasColumnType("date");
            modelBuilder.Entity<Account>().Property(x => x.Acc_Pesel).IsRequired();
            modelBuilder.Entity<Account>().Property(x => x.Acc_Email).HasMaxLength(127);
            modelBuilder.Entity<Account>().Property(x => x.Acc_ContactNumber).HasMaxLength(63);
            modelBuilder.Entity<Account>().Property(x => x.Acc_AtyId).IsRequired();
            modelBuilder.Entity<Account>().Property(x => x.Acc_IsActive).IsRequired();
            modelBuilder.Entity<Account>().Property(x => x.Acc_InsertedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Account>().Property(x => x.Acc_ModifiedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Account>().Property(x => x.Acc_ReminderExpire).HasColumnType("datetime");
            modelBuilder.Entity<Account>().Property(x => x.Acc_RegistrationHash).HasMaxLength(255);
            modelBuilder.Entity<Account>().Property(x => x.Acc_ReminderHash).HasMaxLength(255);

            modelBuilder.Entity<AccountType>().HasKey(x => x.Aty_Id);
            modelBuilder.Entity<AccountType>().Property(x => x.Aty_Name).IsRequired().HasMaxLength(255);

            modelBuilder.Entity<BinData>().HasKey(x => x.Bin_Id);
            modelBuilder.Entity<BinData>().Property(x => x.Bin_Name).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<BinData>().Property(x => x.Bin_Data).IsRequired();
            modelBuilder.Entity<BinData>().Property(x => x.Bin_ModifiedAccId).IsRequired();
            modelBuilder.Entity<BinData>().Property(x => x.Bin_ModifiedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<BinData>().Property(x => x.Bin_InsertedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<BinData>().Property(x => x.Bin_InsertedAccId).IsRequired();

            modelBuilder.Entity<Certificate>().HasKey(x => x.Cer_Id);
            modelBuilder.Entity<Certificate>().Property(x => x.Cer_AccId).IsRequired();
            modelBuilder.Entity<Certificate>().Property(x => x.Cer_BinId).IsRequired();
            modelBuilder.Entity<Certificate>().Property(x => x.Cer_IsActive).IsRequired();
            modelBuilder.Entity<Certificate>().Property(x => x.Cer_InsertedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Certificate>().Property(x => x.Cer_InsertedAccId).IsRequired();
            modelBuilder.Entity<Certificate>().Property(x => x.Cer_ModifiedAccId).IsRequired();
            modelBuilder.Entity<Certificate>().Property(x => x.Cer_ModifiedDate).IsRequired().HasColumnType("datetime");

            modelBuilder.Entity<Event>().HasKey(x => x.Eve_Id);
            modelBuilder.Entity<Event>().Property(x => x.Eve_AccId).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_TimeFrom).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_TimeTo).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_Type).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_DoctorId).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_Description).HasMaxLength(255);
            modelBuilder.Entity<Event>().Property(x => x.Eve_IsActive).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_InsertedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Event>().Property(x => x.Eve_InsertedAccId).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_ModifiedAccId).IsRequired();
            modelBuilder.Entity<Event>().Property(x => x.Eve_ModifiedDate).IsRequired().HasColumnType("datetime");

            modelBuilder.Entity<Notification>().HasKey(x => x.Not_Id);
            modelBuilder.Entity<Notification>().Property(x => x.Not_EveId).IsRequired();
            modelBuilder.Entity<Notification>().Property(x => x.Not_NtyId).IsRequired();
            modelBuilder.Entity<Notification>().Property(x => x.Not_SendTime).IsRequired();
            modelBuilder.Entity<Notification>().Property(x => x.Not_IsActive).IsRequired();
            modelBuilder.Entity<Notification>().Property(x => x.Not_InsertedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Notification>().Property(x => x.Not_InsertedAccId).IsRequired();
            modelBuilder.Entity<Notification>().Property(x => x.Not_ModifiedAccId).IsRequired();
            modelBuilder.Entity<Notification>().Property(x => x.Not_ModifiedDate).IsRequired().HasColumnType("datetime");

            modelBuilder.Entity<NotificationType>().HasKey(x => x.Nty_Id);
            modelBuilder.Entity<NotificationType>().Property(x => x.Nty_Name).IsRequired().HasMaxLength(127);
            modelBuilder.Entity<NotificationType>().Property(x => x.Nty_Template).IsRequired();

            modelBuilder.Entity<Vaccination>().HasKey(x => x.Vac_Id);
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_Name).IsRequired().HasMaxLength(127);
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_Description).IsRequired().HasMaxLength(2047);
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_PhotoId).IsRequired();
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_DaysBetweenVacs).IsRequired();
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_IsActive).IsRequired();
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_InsertedDate).IsRequired().HasColumnType("datetime");
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_InsertedAccId).IsRequired();
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_ModifiedAccId).IsRequired();
            modelBuilder.Entity<Vaccination>().Property(x => x.Vac_ModifiedDate).IsRequired().HasColumnType("datetime");

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
