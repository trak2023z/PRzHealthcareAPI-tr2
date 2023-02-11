using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Models.DTO;

namespace PRzHealthcareAPI.Services
{
    public interface IVaccinationService
    {
        List<VaccinationDto> GetAllActive();
    }

    public class VaccinationService : IVaccinationService
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly AuthenticationSettings _authentication;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly IMapper _mapper;

        public VaccinationService(HealthcareDbContext dbContext, AuthenticationSettings authentication, IPasswordHasher<Account> passwordHasher, IMapper mapper)
        {
            _dbContext = dbContext;
            _authentication = authentication;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public List<VaccinationDto> GetAllActive()
        {
            var vaccinationList = _dbContext.Vaccinations.Where(x => x.Vac_IsActive).ToList();
            if (vaccinationList is null)
            {
                return new List<VaccinationDto>();
            }

            List<VaccinationDto> vaccinationListDto = new List<VaccinationDto>();

            foreach (var vaccination in vaccinationList)
            {
                vaccinationListDto.Add(_mapper.Map<VaccinationDto>(vaccination));
            }

            return vaccinationListDto;
        }
    }
}
