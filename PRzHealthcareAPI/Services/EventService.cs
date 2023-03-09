using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NLog;
using NLog.Fluent;
using PRzHealthcareAPI.Exceptions;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Models.DTO;
using PublicHoliday;

namespace PRzHealthcareAPI.Services
{
    public interface IEventService
    {
        bool SeedDates();
        List<EventDto> GetAvailableDates(string selectedDate, string selectedDoctorId);
        List<EventDto> GetUserEvents(int accountId);
    }
    public class EventService : IEventService
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly AuthenticationSettings _authentication;
        private readonly IMapper _mapper;

        public EventService(HealthcareDbContext dbContext, AuthenticationSettings authentication, IMapper mapper)
        {
            _dbContext = dbContext;
            _authentication = authentication;
            _mapper = mapper;
        }

        public List<EventDto> GetAvailableDates(string selectedDate, string selectedDoctorId)
        {
            var events = _dbContext.Events.Where(x => x.Eve_TimeFrom > Convert.ToDateTime(selectedDate) && x.Eve_TimeFrom <= Convert.ToDateTime(selectedDate).AddDays(1) && x.Eve_Type == 1 && x.Eve_DoctorId == Convert.ToInt32(selectedDoctorId)).ToList();
            if (!events.Any())
            {
                throw new NotFoundException("Brak wolnych terminów.");
            }

            List<EventDto> eventDtos = new List<EventDto>();

            foreach (var ev in events)
            {
                var eventDto = _mapper.Map<EventDto>(ev);
                eventDtos.Add(eventDto);
            }
            return eventDtos;

        }

        public List<EventDto> GetUserEvents(int accountId)
        {
            var events = _dbContext.Events.Where(x => x.Eve_AccId == accountId && x.Eve_Type == 2).ToList();

            List<EventDto> eventDtos = new List<EventDto>();

            foreach (var ev in events)
            {
                var eventDto = _mapper.Map<EventDto>(ev);
                eventDtos.Add(eventDto);
            }
            return eventDtos;

        }
        public bool SeedDates()
        {
            var calendar = new PolandPublicHoliday();

            var doctorsList = _dbContext.Accounts.Where(x => x.Acc_AtyId == 3).ToList();
            if (!doctorsList.Any())
            {
                throw new NotFoundException($@"Uzupełnij listę doktorów.");
            }
            int startHour = 8;
            int endHour = 16;

            foreach (var doctor in doctorsList)
            {
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
                                    var eventsList = _dbContext.Events.Where(x => x.Eve_DoctorId == doctor.Acc_Id && x.Eve_TimeFrom == actualDate && x.Eve_TimeTo == actualDate.AddMinutes(15)).ToList();

                                    if (!eventsList.Any())
                                    {
                                        Event seedEvent = new Event();
                                        seedEvent = new Event()
                                        {
                                            Eve_AccId = 1,
                                            Eve_TimeFrom = actualDate,
                                            Eve_TimeTo = actualDate.AddMinutes(15),
                                            Eve_Description = "Dostępny",
                                            Eve_InsertedAccId = 1,
                                            Eve_InsertedDate = DateTime.Now,
                                            Eve_DoctorId = doctor.Acc_Id,
                                            Eve_IsActive = true,
                                            Eve_ModifiedAccId = 1,
                                            Eve_ModifiedDate = DateTime.Now,
                                            Eve_Type = 1,
                                            Eve_VacId = 2,
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
                                    var eventsList = _dbContext.Events.Where(x => x.Eve_TimeFrom >= DateTime.Now.AddDays(i)).ToList();

                                    if (!eventsList.Any(x => x.Eve_TimeFrom == actualDate && x.Eve_TimeTo == actualDate.AddMinutes(15)))
                                    {
                                        Event seedEvent = new Event();
                                        seedEvent = new Event()
                                        {
                                            Eve_AccId = 1,
                                            Eve_TimeFrom = actualDate,
                                            Eve_TimeTo = actualDate.AddMinutes(15),
                                            Eve_Description = "Dostępny",
                                            Eve_InsertedAccId = 1,
                                            Eve_InsertedDate = DateTime.Now,
                                            Eve_DoctorId = doctor.Acc_Id,
                                            Eve_IsActive = true,
                                            Eve_ModifiedAccId = 1,
                                            Eve_ModifiedDate = DateTime.Now,
                                            Eve_Type = 1,
                                            Eve_VacId = 2,
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
