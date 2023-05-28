using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NLog;
using NLog.Fluent;
using PRzHealthcareAPI.Exceptions;
using PRzHealthcareAPI.Helpers;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Models.DTO;
using PublicHoliday;
using System.Globalization;

namespace PRzHealthcareAPI.Services
{
    public interface IEventService
    {
        bool SeedDates();
        EventDto GetSelectedEvent(int eventId);
        List<EventDto> GetAvailableDates(string selectedDate, string selectedDoctorId);
        List<EventDto> GetUserEvents(int accountId);
        List<EventDto> GetBusyEvents(int accountId);
        List<EventDto> GetDoctorEvents(int accountId);
        List<EventDto> GetNurseEvents();
        void TakeTerm(EventDto dto, string accountId);
        void CancelTerm(EventDto dto, string accountId);
        void FinishTerm(EventDto dto, string accountId);
        void EditTerm(EventDto dto, string accountId);
    }
    public class EventService : IEventService
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly AuthenticationSettings _authentication;
        private readonly EmailSettings _emailSettings;
        private readonly IMapper _mapper;
        private readonly ICertificateService _certificateService;

        public EventService(HealthcareDbContext dbContext, AuthenticationSettings authentication, EmailSettings emailSettings, IMapper mapper, ICertificateService certificateService)
        {
            _dbContext = dbContext;
            _authentication = authentication;
            _emailSettings = emailSettings;
            _mapper = mapper;
            _certificateService = certificateService;
        }


        /// <summary>
        /// Pobranie wolnych terminów
        /// </summary>
        /// <param name="selectedDate">Wybrana data</param>
        /// <param name="selectedDoctorId">Wybrany doktor</param>
        /// <returns>Lista wolnych terminów</returns>
        /// <exception cref="NotFoundException">Brak wolnych terminów</exception>
        public List<EventDto> GetAvailableDates(string selectedDate, string selectedDoctorId)
        {

            var availableEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Wolny").Ety_Id;
            var events = _dbContext.Events.Where(x => x.Eve_TimeFrom > Convert.ToDateTime(selectedDate) && x.Eve_TimeFrom <= Convert.ToDateTime(selectedDate).AddDays(1) && x.Eve_Type == availableEventTypeId && x.Eve_DoctorId == Convert.ToInt32(selectedDoctorId)).ToList();
            if (!events.Any())
            {
                throw new NotFoundException("Brak wolnych terminów.");
            }
            if (Convert.ToDateTime(selectedDate).Date == DateTime.Now.Date)
            {
                events = events.Where(x => x.Eve_TimeFrom > DateTime.Now.AddMinutes(20)).ToList();
            }

            List<EventDto> eventDtos = new List<EventDto>();

            foreach (var ev in events)
            {
                var eventDto = _mapper.Map<EventDto>(ev);
                eventDtos.Add(eventDto);
            }
            return eventDtos;

        }

        /// <summary>
        /// Pobranie terminów zajętych przez użytkownika
        /// </summary>
        /// <param name="accountId">Id zalogowanego użytkownika</param>
        /// <returns>Lista zajętych terminów</returns>
        public List<EventDto> GetUserEvents(int accountId)
        {
            var busyEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zajęty").Ety_Id;
            var events = _dbContext.Events.Where(x => x.Eve_AccId == accountId && x.Eve_Type == busyEventTypeId && x.Eve_TimeTo > DateTime.Now).ToList();

            List<EventDto> eventDtos = new List<EventDto>();

            foreach (var ev in events)
            {
                var eventDto = _mapper.Map<EventDto>(ev);
                eventDtos.Add(eventDto);
            }
            return eventDtos;

        }

        /// <summary>
        /// Pobranie wszystkich terminów użytkownika, które nie są wolne
        /// </summary>
        /// <param name="accountId">Id zalogowanego użytkownika</param>
        /// <returns>Lista niewolnych terminów</returns>
        public List<EventDto> GetBusyEvents(int accountId)
        {
            var freeEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Wolny").Ety_Id;
            var events = _dbContext.Events.Where(x => x.Eve_Type != freeEventTypeId && x.Eve_TimeTo > DateTime.Now).ToList();

            List<EventDto> eventDtos = new List<EventDto>();

            foreach (var ev in events)
            {
                
                var eventDto = _mapper.Map<EventDto>(ev);
                if (eventDto.AccId == accountId)
                {
                    eventDto.Description = "Twoja wizyta";
                }
                else
                {
                    eventDto.AccId = 0;
                    eventDto.Description = "Zajęty";
                }
                eventDto.DateFrom = Convert.ToDateTime(eventDto.TimeFrom);
                eventDto.DateTo = Convert.ToDateTime(eventDto.TimeTo);
                eventDto.TimeFrom = Convert.ToDateTime(eventDto.TimeFrom).ToString("MM.dd.yyyy HH:mm");
                eventDto.TimeTo = Convert.ToDateTime(eventDto.TimeTo).ToString("MM.dd.yyyy HH:mm");
                
                eventDtos.Add(eventDto);
            }
            return eventDtos;

        }

        /// <summary>
        /// Pobranie szczegółów wybranego terminu
        /// </summary>
        /// <param name="eventId">Id terminu</param>
        /// <returns>Szczegóły terminu</returns>
        /// <exception cref="NotFoundException">Brak wydarzenia</exception>
        public EventDto GetSelectedEvent(int eventId)
        {
            var selectedEvent = _dbContext.Events.FirstOrDefault(x => x.Eve_Id == eventId);
            if (selectedEvent == null)
            {
                throw new NotFoundException("Nie znaleziono odpowiedniego wydarzenia.");
            }

            var eventDto = _mapper.Map<EventDto>(selectedEvent);

            eventDto.DateFrom = Convert.ToDateTime(eventDto.TimeFrom);
            eventDto.DateTo = Convert.ToDateTime(eventDto.TimeTo);

            return eventDto;

        }

        /// <summary>
        /// Pobranie listy terminów wybranego doktora
        /// </summary>
        /// <param name="accountId">Id doktora</param>
        /// <returns>Lista terminów wybranego doktora</returns>
        public List<EventDto> GetDoctorEvents(int accountId)
        {
            var busyEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zajęty").Ety_Id;
            var events = _dbContext.Events.Where(x => x.Eve_DoctorId == accountId && x.Eve_Type == busyEventTypeId).ToList();

            List<EventDto> eventDtos = new List<EventDto>();

            foreach (var ev in events)
            {
                var eventDto = _mapper.Map<EventDto>(ev);
                eventDtos.Add(eventDto);
            }
            return eventDtos;

        }

        /// <summary>
        /// Pobranie listy terminów dla pielęgniarki
        /// </summary>
        /// <returns>Lista terminów dla pielęgniarki</returns>
        public List<EventDto> GetNurseEvents()
        {
            var busyEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zajęty").Ety_Id;
            var awayEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Nieobecność").Ety_Id;
            var finishedEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zakończony").Ety_Id;
            var events = _dbContext.Events.Where(x => (x.Eve_Type == busyEventTypeId || x.Eve_Type == awayEventTypeId || x.Eve_Type == finishedEventTypeId)).ToList();

            List<EventDto> eventDtos = new List<EventDto>();

            foreach (var ev in events)
            {
                var eventDto = _mapper.Map<EventDto>(ev);

                eventDto.DateFrom = Convert.ToDateTime(eventDto.TimeFrom);
                eventDto.DateTo = Convert.ToDateTime(eventDto.TimeTo);
                eventDto.TimeFrom = Convert.ToDateTime(eventDto.TimeFrom).ToString("MM.dd.yyyy HH:mm");
                eventDto.TimeTo = Convert.ToDateTime(eventDto.TimeTo).ToString("MM.dd.yyyy HH:mm");
                eventDtos.Add(eventDto);
            }
            return eventDtos;

        }

        /// <summary>
        /// Zajęcie terminu
        /// </summary>
        /// <param name="dto">Obiekt danego terminu</param>
        /// <param name="accountId">Id zalogowanego użytkownika</param>
        public void TakeTerm(EventDto dto, string accountId)
        {
            var busyEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zajęty").Ety_Id;
            dto.DateFrom = Convert.ToDateTime(dto.DateFrom).AddHours(2);
            TimeSpan timeFromSpan = TimeSpan.Parse(dto.TimeFrom);
            dto.DateFrom = Convert.ToDateTime(dto.DateFrom).Add(timeFromSpan);

            var changedEvent = _dbContext.Events.FirstOrDefault(x => x.Eve_TimeFrom == dto.DateFrom && x.Eve_DoctorId == dto.DoctorId);
            if (changedEvent == null)
            {
                throw new NotFoundException("Nie znaleziono odpowiedniego terminu. W razie problemów prosimy o kontakt telefoniczny.");
            }
            Account user = new Account();
            if (dto.AccId == 0)
            {
                user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Id.ToString() == accountId);
            }
            else
            {
                user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Id == dto.AccId);
            }
            if (user == null)
            {
                throw new NotFoundException("Nie znaleziono użytkownika.");
            }
            if (changedEvent.Eve_Type == busyEventTypeId)
            {
                throw new NotFoundException("Termin został zajęty.");
            }

            var selectedVaccination = _dbContext.Vaccinations.FirstOrDefault(x => x.Vac_Id == dto.VacId);
            var finishEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zakończony").Ety_Id;
            var lastUserVaccination = _dbContext.Events.Where(x => x.Eve_AccId == user.Acc_Id && x.Eve_Type == finishEventTypeId).ToList();

            if (lastUserVaccination.Any(x => x.Eve_TimeFrom.AddDays(selectedVaccination.Vac_DaysBetweenVacs) > Convert.ToDateTime(dto.TimeFrom)))
            {
                throw new BadRequestException("Prosimy odczekać odstęp czasu opisany w zaświadczeniu. W razie problemów prosimy o kontakt telefoniczny.");
            }

            var lastUserVaccinationRequest = _dbContext.Events.Where(x => x.Eve_AccId == user.Acc_Id && x.Eve_Type == busyEventTypeId).ToList();
            if (lastUserVaccinationRequest.Any())
            {
                throw new BadRequestException("Nie ma możliwości rejestracji na dwie oddzielne wizyty.");
            }

            changedEvent.Eve_AccId = user.Acc_Id;
            changedEvent.Eve_Type = busyEventTypeId;
            changedEvent.Eve_DoctorId = dto.DoctorId;
            changedEvent.Eve_VacId = dto.VacId;
            changedEvent.Eve_Description = "Szczepienie";
            changedEvent.Eve_ModifiedAccId = Convert.ToInt32(accountId);
            changedEvent.Eve_ModifiedDate = DateTime.Now;
            changedEvent.Eve_InsertedDate = DateTime.Now;
            changedEvent.Eve_InsertedAccId = Convert.ToInt32(accountId);

            _dbContext.Update(changedEvent);
            _dbContext.SaveChanges();

            Tools.SendVisitConfirmation(_emailSettings, user, changedEvent, _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Potwierdzenie wizyty w klinice PRz Healthcare"));
            Notification notif = new()
            {
                Not_EveId = changedEvent.Eve_Id,
                Not_NtyId = _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Potwierdzenie wizyty w klinice PRz Healthcare").Nty_Id,
                Not_SendTime = DateTime.Now,
                Not_IsActive = true,
                Not_InsertedDate = DateTime.Now,
                Not_InsertedAccId = user.Acc_Id,
                Not_ModifiedDate = DateTime.Now,
                Not_ModifiedAccId = user.Acc_Id,
            };
            _dbContext.Notifications.Add(notif);
            _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Zakończenie terminu
        /// </summary>
        /// <param name="dto">Obiekt terminu</param>
        /// <param name="accountId">Id zalogowanego użytkownika</param>
        public void FinishTerm(EventDto dto, string accountId)
        {
            var finishEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zakończony").Ety_Id;
            var finishedEvent = _dbContext.Events.FirstOrDefault(x => x.Eve_Id == dto.Id);
            if (finishedEvent == null)
            {
                throw new NotFoundException("Wydarzenie nie istnieje.");
            }
            var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Id == dto.AccId);
            if (user == null)
            {
                throw new NotFoundException("Nie znaleziono użytkownika.");
            }

            finishedEvent.Eve_Type = finishEventTypeId;
            finishedEvent.Eve_ModifiedAccId = Convert.ToInt32(accountId);
            finishedEvent.Eve_ModifiedDate = DateTime.Now;
            finishedEvent.Eve_InsertedDate = DateTime.Now;
            finishedEvent.Eve_InsertedAccId = Convert.ToInt32(accountId);
            finishedEvent.Eve_SerialNumber = Tools.CreateRandomToken(5);


            _dbContext.Update(finishedEvent);
            _dbContext.SaveChanges();


            MemoryStream mailAttachment = _certificateService.PrintCOVIDCertificateToMemoryStream(dto);

            Tools.SendVisitCertificate(_emailSettings, user, finishedEvent, _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Potwierdzenie odbycia wizyty w klinice PRz Healthcare"), mailAttachment);
            Notification notif = new()
            {
                Not_EveId = finishedEvent.Eve_Id,
                Not_NtyId = _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Potwierdzenie odbycia wizyty w klinice PRz Healthcare").Nty_Id,
                Not_SendTime = DateTime.Now,
                Not_IsActive = true,
                Not_InsertedDate = DateTime.Now,
                Not_InsertedAccId = Convert.ToInt32(accountId),
                Not_ModifiedDate = DateTime.Now,
                Not_ModifiedAccId = Convert.ToInt32(accountId),
            };
            _dbContext.Notifications.Add(notif);
            _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Anulowanie terminu
        /// </summary>
        /// <param name="dto">Obiekt terminu</param>
        /// <param name="accountId">Id zalogowanego użytkownika</param>
        public void CancelTerm(EventDto dto, string accountId)
        {
            var freeEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Wolny").Ety_Id;
            var canceledEvent = _dbContext.Events.FirstOrDefault(x => x.Eve_Id == dto.Id);
            if (canceledEvent == null)
            {
                throw new NotFoundException("Wydarzenie nie istnieje.");
            }
            var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Id == dto.AccId);
            if (user == null)
            {
                throw new NotFoundException("Nie znaleziono użytkownika.");
            }

            canceledEvent.Eve_AccId = null;
            canceledEvent.Eve_Description = "Dostępny";
            canceledEvent.Eve_Type = freeEventTypeId;
            canceledEvent.Eve_ModifiedAccId = Convert.ToInt32(accountId);
            canceledEvent.Eve_ModifiedDate = DateTime.Now;
            canceledEvent.Eve_InsertedDate = DateTime.Now;
            canceledEvent.Eve_InsertedAccId = Convert.ToInt32(accountId);
            canceledEvent.Eve_VacId = null;

            _dbContext.Update(canceledEvent);
            _dbContext.SaveChanges();

            Tools.SendVisitCancellation(_emailSettings, user, canceledEvent, _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Anulowanie wizyty w klinice PRz Healthcare"));
            Notification notif = new()
            {
                Not_EveId = canceledEvent.Eve_Id,
                Not_NtyId = _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Anulowanie wizyty w klinice PRz Healthcare").Nty_Id,
                Not_SendTime = DateTime.Now,
                Not_IsActive = true,
                Not_InsertedDate = DateTime.Now,
                Not_InsertedAccId = Convert.ToInt32(accountId),
                Not_ModifiedDate = DateTime.Now,
                Not_ModifiedAccId = Convert.ToInt32(accountId),
            };
            _dbContext.Notifications.Add(notif);
            _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Edycja wybranego terminu - zmiana szczepionki
        /// </summary>
        /// <param name="dto">Obiekt terminu</param>
        /// <param name="accountId">Id zalogowanego użytkownika</param>
        public void EditTerm(EventDto dto, string accountId)
        {
            var freeEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Wolny").Ety_Id;
            var editedEvent = _dbContext.Events.FirstOrDefault(x => x.Eve_Id == dto.Id);
            if (editedEvent == null)
            {
                throw new NotFoundException("Wydarzenie nie istnieje.");
            }

            editedEvent.Eve_ModifiedAccId = Convert.ToInt32(accountId);
            editedEvent.Eve_ModifiedDate = DateTime.Now;
            editedEvent.Eve_VacId = dto.VacId;

            _dbContext.Update(editedEvent);
            _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Zapełnienie wolnych terminów
        /// </summary>
        /// <returns>Poprawność wykonania funkcji</returns>
        public bool SeedDates()
        {
            var calendar = new PolandPublicHoliday();

            var doctorAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Doktor").Aty_Id;
            var administratorAccountId = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Login == "Administrator").Acc_Id;
            var availableEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Wolny").Ety_Id;

            var doctorsList = _dbContext.Accounts.Where(x => x.Acc_AtyId == doctorAccountTypeId).ToList();
            if (!doctorsList.Any())
            {
                throw new NotFoundException($@"Uzupełnij listę doktorów.");
            }
            int startHour = 8;
            int endHour = 16;

            foreach (var doctor in doctorsList)
            {
                if (doctor.Acc_Id == 3)
                {
                    continue;
                }
                for (int i = 0; i < 30; i++)
                {

                    int actualHour = startHour;
                    int actualMinute = 0;

                    while (actualHour < endHour)
                    {
                        DateTime actualDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, actualHour, actualMinute, 0);

                        if (!calendar.IsPublicHoliday(actualDate))
                        {
                            if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0).DayOfWeek != DayOfWeek.Saturday)
                            {
                                if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0).DayOfWeek != DayOfWeek.Sunday)
                                {
                                    var eventsList = _dbContext.Events.Where(x => x.Eve_DoctorId == doctor.Acc_Id && x.Eve_TimeFrom == actualDate).ToList();

                                    if (!eventsList.Any())
                                    {
                                        Event seedEvent = new Event();
                                        seedEvent = new Event()
                                        {
                                            Eve_AccId = null,
                                            Eve_TimeFrom = actualDate,
                                            Eve_TimeTo = actualDate.AddMinutes(15),
                                            Eve_Description = "Dostępny",
                                            Eve_InsertedAccId = administratorAccountId,
                                            Eve_InsertedDate = DateTime.Now,
                                            Eve_DoctorId = doctor.Acc_Id,
                                            Eve_IsActive = true,
                                            Eve_ModifiedAccId = administratorAccountId,
                                            Eve_ModifiedDate = DateTime.Now,
                                            Eve_Type = availableEventTypeId,
                                            Eve_VacId = null,
                                        };

                                        _dbContext.Events.Add(seedEvent);
                                        _dbContext.SaveChanges();
                                    }

                                    actualMinute += 15;

                                    if (actualMinute == 60)
                                    {
                                        actualMinute = 0;
                                        actualHour++;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                for (int i = 0; i < 30; i++)
                {

                    int actualHour = startHour;
                    int actualMinute = 0;

                    while (actualHour < endHour)
                    {
                        DateTime actualDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, DateTime.Now.AddDays(i).Day, actualHour, actualMinute, 0);

                        if (!calendar.IsPublicHoliday(actualDate))
                        {
                            if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0).DayOfWeek != DayOfWeek.Saturday)
                            {
                                if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0).DayOfWeek != DayOfWeek.Sunday)
                                {
                                    var eventsList = _dbContext.Events.Where(x => x.Eve_DoctorId == doctor.Acc_Id && x.Eve_TimeFrom >= DateTime.Now.AddDays(i)).ToList();

                                    if (!eventsList.Any(x => x.Eve_TimeFrom == actualDate && x.Eve_DoctorId == doctor.Acc_Id))
                                    {
                                        Event seedEvent = new Event();
                                        seedEvent = new Event()
                                        {
                                            Eve_AccId = null,
                                            Eve_TimeFrom = actualDate,
                                            Eve_TimeTo = actualDate.AddMinutes(15),
                                            Eve_Description = "Dostępny",
                                            Eve_InsertedAccId = administratorAccountId,
                                            Eve_InsertedDate = DateTime.Now,
                                            Eve_DoctorId = doctor.Acc_Id,
                                            Eve_IsActive = true,
                                            Eve_ModifiedAccId = administratorAccountId,
                                            Eve_ModifiedDate = DateTime.Now,
                                            Eve_Type = availableEventTypeId,
                                            Eve_VacId = null,
                                        };

                                        _dbContext.Events.Add(seedEvent);
                                        _dbContext.SaveChanges();
                                    }

                                    actualMinute += 15;

                                    if (actualMinute == 60)
                                    {
                                        actualMinute = 0;
                                        actualHour++;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return true;

        }
    }
}
