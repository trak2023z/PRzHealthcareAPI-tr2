using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PRzHealthcareAPI.Exceptions;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Models.DTO;

namespace PRzHealthcareAPI.Services
{
    public interface IVaccinationService
    {
        List<VaccinationDto> GetAll();
        void EditVaccination(VaccinationDto dto, int accountId);
        void ArchiveVaccination(VaccinationDto dto, int accountId);
        void UnarchiveVaccination(VaccinationDto dto, int accountId);
        void AddVaccination(VaccinationDto dto, int accountId);
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

        public List<VaccinationDto> GetAll()
        {
            var vaccinationList = _dbContext.Vaccinations.ToList();
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
        public void AddVaccination(VaccinationDto dto, int accountId)
        {
            var vaccinationExists = _dbContext.Vaccinations.FirstOrDefault(x => x.Vac_Name == dto.Name);
            if (vaccinationExists != null)
            {
                throw new BadRequestException("Szczepionka o takiej nazwie już istnieje.");
            }

            Vaccination newVaccination = new()
            {
                Vac_Name = dto.Name,
                Vac_Description = dto.Description,
                Vac_DaysBetweenVacs = dto.DaysBetweenVacs,
                Vac_IsActive = true,
                Vac_ModifiedAccId = accountId,
                Vac_ModifiedDate = DateTime.Now,
                Vac_InsertedAccId = accountId,
                Vac_InsertedDate = DateTime.Now,
            };
            
            _dbContext.Vaccinations.Add(newVaccination);
            _dbContext.SaveChanges();
        }

        public void EditVaccination(VaccinationDto dto, int accountId)
        {
            var editedVaccination = _dbContext.Vaccinations.FirstOrDefault(x => x.Vac_Id == dto.Id);
            if(editedVaccination == null)
            {
                throw new NotFoundException("Szczepionka nie istnieje.");
            }

            editedVaccination.Vac_Name = dto.Name;
            editedVaccination.Vac_Description = dto.Description;
            editedVaccination.Vac_DaysBetweenVacs = dto.DaysBetweenVacs;
            editedVaccination.Vac_ModifiedAccId = accountId;
            editedVaccination.Vac_ModifiedDate = DateTime.Now;

            _dbContext.Vaccinations.Update(editedVaccination);
            _dbContext.SaveChanges();
        }
        public void ArchiveVaccination(VaccinationDto dto, int accountId)
        {
            var editedVaccination = _dbContext.Vaccinations.FirstOrDefault(x => x.Vac_Id == dto.Id);
            if (editedVaccination == null)
            {
                throw new NotFoundException("Szczepionka nie istnieje.");
            }
            var busyEventTypeId = _dbContext.EventTypes.FirstOrDefault(x => x.Ety_Name == "Zajęty").Ety_Id;
            var vaccinationInUse = _dbContext.Events.Any(x => x.Eve_VacId == dto.Id && x.Eve_Type == busyEventTypeId);
            if (vaccinationInUse)
            {
                throw new BadRequestException("Szczepionka jest wciąż w użyciu i nie może zostać archiwiziowana");
            }

            editedVaccination.Vac_IsActive = false;
            editedVaccination.Vac_ModifiedAccId = accountId;
            editedVaccination.Vac_ModifiedDate = DateTime.Now;

            _dbContext.Vaccinations.Update(editedVaccination);
            _dbContext.SaveChanges();
        }
        public void UnarchiveVaccination(VaccinationDto dto, int accountId)
        {
            var editedVaccination = _dbContext.Vaccinations.FirstOrDefault(x => x.Vac_Id == dto.Id);
            if (editedVaccination == null)
            {
                throw new NotFoundException("Szczepionka nie istnieje.");
            }

            editedVaccination.Vac_IsActive = true;
            editedVaccination.Vac_ModifiedAccId = accountId;
            editedVaccination.Vac_ModifiedDate = DateTime.Now;

            _dbContext.Vaccinations.Update(editedVaccination);
            _dbContext.SaveChanges();
        }
    }
}
