using Microsoft.AspNetCore.Identity;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Services;

namespace PRzHealthcareAPI
{
    public class HealthcareSeeder
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly IEventService _eventService;

        public HealthcareSeeder(HealthcareDbContext dbContext, IPasswordHasher<Account> passwordHasher, IEventService eventService)
        {
            this._dbContext = dbContext;
            this._passwordHasher = passwordHasher;
            this._eventService = eventService;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.AccountTypes.Any())
                {
                    var accountTypes = GetAccountTypes();
                    _dbContext.AccountTypes.AddRange(accountTypes);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Accounts.Any())
                {
                    var accounts = GetAccounts();
                    _dbContext.Accounts.AddRange(accounts);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.EventTypes.Any())
                {
                    var eventTypes = GetEventTypes();
                    _dbContext.EventTypes.AddRange(eventTypes);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.NotificationTypes.Any())
                {
                    var notificationTypes = GetNotificationTypes();
                    _dbContext.NotificationTypes.AddRange(notificationTypes);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Vaccinations.Any())
                {
                    var vaccinations = GetVaccinations();
                    _dbContext.Vaccinations.AddRange(vaccinations);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Events.Any())
                {
                    _eventService.SeedDates();
                }

            }
        }

        private IEnumerable<EventType> GetEventTypes()
        {
            var eventTypes = new List<EventType>()
            {
                new EventType()
                {
                    Ety_Name = "Wolny",
                },
                new EventType()
                {
                    Ety_Name = "Zajęty",
                },
                new EventType()
                {
                    Ety_Name = "Nieobecność",
                },
            };
            return eventTypes;
        }
        private IEnumerable<Account> GetAccounts()
        {
            var accounts = new List<Account>();

            var adminAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Administrator").Aty_Id;
            var doctorAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Doktor").Aty_Id;
            var nurseAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Pielęgniarz").Aty_Id;
            var patientAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Pacjent").Aty_Id;

            var adminAccount = new Account()
            {
                Acc_AtyId = adminAccountTypeId,
                Acc_ContactNumber = "123-456-789",
                Acc_DateOfBirth = Convert.ToDateTime("14.06.1998"),
                Acc_Email = @"165083@stud.prz.edu.pl",
                Acc_Firstname = "Mateusz",
                Acc_Lastname = "Kubis",
                Acc_Pesel = 98061400000,
                Acc_Secondname = "Dariusz",
                Acc_PhotoId = null,
                Acc_Login = "Administrator",
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
                Acc_RegistrationHash = null,
                Acc_ReminderExpire = null,
                Acc_ReminderHash = null,
            };
            adminAccount.Acc_Password = _passwordHasher.HashPassword(adminAccount, "Leopard12");

            var doctor1Account = new Account()
            {
                Acc_AtyId = doctorAccountTypeId,
                Acc_ContactNumber = "123-456-789",
                Acc_DateOfBirth = Convert.ToDateTime("01.01.1988"),
                Acc_Email = @"165083@stud.prz.edu.pl",
                Acc_Firstname = "Jan",
                Acc_Lastname = "Tomczyk",
                Acc_Pesel = 88010125698,
                Acc_Secondname = "",
                Acc_PhotoId = null,
                Acc_Login = "JTomczyk",
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
                Acc_RegistrationHash = null,
                Acc_ReminderExpire = null,
                Acc_ReminderHash = null,
            };
            doctor1Account.Acc_Password = _passwordHasher.HashPassword(doctor1Account, "Leopard12");

            var doctor2Account = new Account()
            {
                Acc_AtyId = doctorAccountTypeId,
                Acc_ContactNumber = "966-455-123",
                Acc_DateOfBirth = Convert.ToDateTime("23.05.1968"),
                Acc_Email = @"165083@stud.prz.edu.pl",
                Acc_Firstname = "Andrzej",
                Acc_Lastname = "Rodzinny",
                Acc_Pesel = 68052369584,
                Acc_Secondname = "Tomasz",
                Acc_PhotoId = null,
                Acc_Login = "ARodzinny",
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
                Acc_RegistrationHash = null,
                Acc_ReminderExpire = null,
                Acc_ReminderHash = null,
            };
            doctor2Account.Acc_Password = _passwordHasher.HashPassword(doctor2Account, "Leopard12");

            var nurseAccount = new Account()
            {
                Acc_AtyId = nurseAccountTypeId,
                Acc_ContactNumber = "123-456-789",
                Acc_DateOfBirth = Convert.ToDateTime("01.01.1988"),
                Acc_Email = @"165083@stud.prz.edu.pl",
                Acc_Firstname = "Marta",
                Acc_Lastname = "Nowak",
                Acc_Pesel = 96021456325,
                Acc_Secondname = "",
                Acc_PhotoId = null,
                Acc_Login = "MNowak",
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
                Acc_RegistrationHash = null,
                Acc_ReminderExpire = null,
                Acc_ReminderHash = null,
            };
            nurseAccount.Acc_Password = _passwordHasher.HashPassword(nurseAccount, "Leopard12");

            var patientAccount = new Account()
            {
                Acc_AtyId = patientAccountTypeId,
                Acc_ContactNumber = "987-456-132",
                Acc_DateOfBirth = Convert.ToDateTime("01.06.1996"),
                Acc_Email = @"165083@stud.prz.edu.pl",
                Acc_Firstname = "Mariola",
                Acc_Lastname = "Łoskot",
                Acc_Pesel = 96060136548,
                Acc_Secondname = "",
                Acc_PhotoId = null,
                Acc_Login = "MLoskot",
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
                Acc_RegistrationHash = null,
                Acc_ReminderExpire = null,
                Acc_ReminderHash = null,
            };
            patientAccount.Acc_Password = _passwordHasher.HashPassword(patientAccount, "Leopard12");

            accounts.Add(patientAccount);
            accounts.Add(nurseAccount);
            accounts.Add(doctor1Account);
            accounts.Add(doctor2Account);
            accounts.Add(adminAccount);

            return accounts;
        }
        private IEnumerable<Vaccination> GetVaccinations()
        {
            var administratorId = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Login == "Administrator").Acc_Id;
            var vaccinations = new List<Vaccination>()
            {
                new Vaccination()
                {
                   Vac_Name = "Comirnaty",
                   Vac_Description = @"Szczepionka została dopuszczona do obrotu w całej Unii Europejskiej w procedurze warunkowego dopuszczenia do obrotu (decyzja Komisji Europejskiej z 21.12.2020). Szczepionka chroni przed objawami COVID-19 wywołanymi przez koronawirusa SARS-CoV-2.  Może być podawana osobom w wieku >6 miesięcy. Szczegółowe zalecenia dotyczące stosowania szczepionki przedstawione są w Charakterystyce Produktu Leczniczego oraz ulotce dla pacjenta dołączonej do opakowania szczepionki.",
                   Vac_PhotoId = null,
                   Vac_DaysBetweenVacs = 12,
                   Vac_IsActive = true,
                   Vac_InsertedDate = DateTime.Now,
                   Vac_InsertedAccId = administratorId,
                   Vac_ModifiedDate = DateTime.Now,
                   Vac_ModifiedAccId = administratorId
                },
                new Vaccination()
                {
                   Vac_Name = "Spikevax",
                   Vac_Description = @"Szczepionka została dopuszczona do obrotu w całej Unii Europejskiej w procedurze warunkowego dopuszczenia do obrotu (decyzja Komisji Europejskiej z 06.01.2021). Szczepionka chroni przed objawami COVID-19 wywołanymi przez koronawirusa SARS-CoV-2. Może być podawana osobom w wieku ≥6 lat. Szczegółowe zalecenia dotyczące stosowania szczepionki przedstawione są w Charakterystyce Produktu Leczniczego oraz Ulotce dla pacjenta.",
                   Vac_PhotoId = null,
                   Vac_DaysBetweenVacs = 12,
                   Vac_IsActive = true,
                   Vac_InsertedDate = DateTime.Now,
                   Vac_InsertedAccId = administratorId,
                   Vac_ModifiedDate = DateTime.Now,
                   Vac_ModifiedAccId = administratorId
                },
                new Vaccination()
                {
                   Vac_Name = "Vaxzevria",
                   Vac_Description = @"Szczepionka została dopuszczona do obrotu w całej Unii Europejskiej w procedurze warunkowego dopuszczenia do obrotu (decyzja Komisji Europejskiej z 29.01.2021). Szczepionka chroni przed objawami COVID-19 wywołanymi przez koronawirusa SARS-CoV-2. Może być podawana osobom w wieku ≥18 lat, w postaci dwóch dawek. Szczegółowe zalecenia dotyczące stosowania szczepionki przedstawione są w Charakterystyce Produktu Leczniczego oraz Ulotce dla pacjenta (szczepionka obecnie w Polsce niedostępna).",
                   Vac_PhotoId = null,
                   Vac_DaysBetweenVacs = 12,
                   Vac_IsActive = true,
                   Vac_InsertedDate = DateTime.Now,
                   Vac_InsertedAccId = administratorId,
                   Vac_ModifiedDate = DateTime.Now,
                   Vac_ModifiedAccId = administratorId
                },
            };
            return vaccinations;
        }

        private IEnumerable<NotificationType> GetNotificationTypes()
        {
            var notificationTypes = new List<NotificationType>()
            {
                new NotificationType()
                {
                    Nty_Name = "Rejestracja użytkownika w systemie PRz Healthcare",
                    Nty_Template = @"<p>Witaj, @@NAZWA</p>
<p>Kliknij w ten link, aby dokończyć proces rejestracji: <a href=""@@LINK"" target=""_blank"">Potwierdź email</a></p>
<p><br></p>
<p>Jeśli nie rejestrowałeś się w naszym serwisie, zignoruj tę wiadomość.</p>
<p>Wiadomość wygenerowana automatycznie, prosimy na nią nie odpowiadać.</p>
<p>PRz Healthcare<br>Rzesz&oacute;w, ul. Wincentego Pola 2</p>"
                },
            };
            return notificationTypes;
        }

        public IEnumerable<AccountType> GetAccountTypes()
        {
            var accountTypes = new List<AccountType>()
            {
                new AccountType()
                {
                    Aty_Name = "Pacjent"
                },
                new AccountType()
                {
                    Aty_Name = "Pielęgniarz"
                },
                new AccountType()
                {
                    Aty_Name = "Doktor"
                },
                new AccountType()
                {
                    Aty_Name = "Administrator"
                },
                new AccountType()
                {
                    Aty_Name = "Niepotwierdzony"
                }
            };

            return accountTypes;
        }
    }
}
