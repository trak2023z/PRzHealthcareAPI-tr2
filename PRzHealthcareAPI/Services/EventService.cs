using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PRzHealthcareAPI.Exceptions;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Models.DTO;
using PublicHoliday;

namespace PRzHealthcareAPI.Services
{
    public interface IEventService
    {
        bool SeedOffDates();
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

        public bool SeedOffDates()
        {
            try
            {
                var calendar = new PolandPublicHoliday();
                for (int i = 0; i < 60; i++)
                {
                    //todo: przejście miesiąca
                    var eventsList = _dbContext.Events.Where(x => x.Eve_TimeFrom >= DateTime.Now.AddDays(-1)).ToList();
                    bool workingDay = true;
                    Event seedEvent = new Event();
                    seedEvent = new Event()
                    {
                        Eve_AccId = 1,
                        Eve_TimeFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0),
                        Eve_TimeTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 7, 59, 59),
                        Eve_Description = "Okres zamknięty",
                        Eve_InsertedAccId = 0,
                        Eve_InsertedDate = DateTime.Now,
                        Eve_DoctorId = 0,
                        Eve_IsActive = true,
                        Eve_ModifiedAccId = 0,
                        Eve_ModifiedDate = DateTime.Now,
                        Eve_Type = 3,
                        Eve_VacId = 2,
                    };
                    if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0).DayOfWeek == DayOfWeek.Saturday)
                    {
                        workingDay = false;
                        seedEvent.Eve_TimeFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0);
                        seedEvent.Eve_TimeTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 23, 59, 59);
                    }
                    if (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0).DayOfWeek == DayOfWeek.Sunday)
                    {
                        workingDay = false;
                        seedEvent.Eve_TimeFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0);
                        seedEvent.Eve_TimeTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 23, 59, 59);
                        seedEvent.Eve_Description = "Święto";
                        seedEvent.Eve_Type = 4;
                    }
                    if (calendar.IsPublicHoliday(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0)))
                    {
                        workingDay = false;
                        seedEvent.Eve_TimeFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 0, 0, 0);
                        seedEvent.Eve_TimeTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 23, 59, 59);
                        seedEvent.Eve_Description = "Święto";
                        seedEvent.Eve_Type = 4;
                    }
                    if (!eventsList.Any(x => x.Eve_TimeFrom == seedEvent.Eve_TimeFrom && x.Eve_TimeTo == seedEvent.Eve_TimeTo))
                    {
                        _dbContext.Events.Add(seedEvent);
                        _dbContext.SaveChanges();
                    }


                    if (workingDay)
                    {
                        seedEvent = new Event()
                        {
                            Eve_AccId = 1,
                            Eve_TimeFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 16, 0, 1),
                            Eve_TimeTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 23, 59, 59),
                            Eve_Description = "Okres zamknięty",
                            Eve_InsertedAccId = 0,
                            Eve_InsertedDate = DateTime.Now,
                            Eve_DoctorId = 0,
                            Eve_IsActive = true,
                            Eve_ModifiedAccId = 0,
                            Eve_ModifiedDate = DateTime.Now,
                            Eve_Type = 3,
                            Eve_VacId = 2,
                        };
                        if (!eventsList.Any(x => x.Eve_TimeFrom == seedEvent.Eve_TimeFrom && x.Eve_TimeTo == seedEvent.Eve_TimeTo))
                        {
                            _dbContext.Events.Add(seedEvent);
                            _dbContext.SaveChanges();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new BadRequestException($@"Błąd podczas uzupełniania terminów: {ex.Message}");
            }
        }
    }
}
