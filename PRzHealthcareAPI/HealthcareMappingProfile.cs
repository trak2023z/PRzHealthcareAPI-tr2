using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Models.DTO;

namespace PRzHealthcareAPI
{
    public class HealthcareMappingProfile : Profile
    {

        public HealthcareMappingProfile()
        {
            CreateMap<RegisterUserDto, Account>()
                 .ForMember(m => m.Acc_Login, c => c.MapFrom(s => s.Login))
                 .ForMember(m => m.Acc_Firstname, c => c.MapFrom(s => s.Firstname))
                 .ForMember(m => m.Acc_Secondname, c => c.MapFrom(s => s.Secondname))
                 .ForMember(m => m.Acc_Lastname, c => c.MapFrom(s => s.Lastname))
                 .ForMember(m => m.Acc_DateOfBirth, c => c.MapFrom(s => s.DateOfBirth))
                 .ForMember(m => m.Acc_Pesel, c => c.MapFrom(s => s.Pesel))
                 .ForMember(m => m.Acc_Email, c => c.MapFrom(s => s.Email))
                 .ForMember(m => m.Acc_ContactNumber, c => c.MapFrom(s => s.ContactNumber));

            CreateMap<Account, UserDto>()
                 .ForMember(m => m.Id, c => c.MapFrom(s => s.Acc_Id))
                 .ForMember(m => m.AtyId, c => c.MapFrom(s => s.Acc_AtyId))
                 .ForMember(m => m.Login, c => c.MapFrom(s => s.Acc_Login))
                 .ForMember(m => m.Firstname, c => c.MapFrom(s => s.Acc_Firstname))
                 .ForMember(m => m.Secondname, c => c.MapFrom(s => s.Acc_Secondname))
                 .ForMember(m => m.Lastname, c => c.MapFrom(s => s.Acc_Lastname))
                 .ForMember(m => m.DateOfBirth, c => c.MapFrom(s => s.Acc_DateOfBirth))
                 .ForMember(m => m.Pesel, c => c.MapFrom(s => s.Acc_Pesel))
                 .ForMember(m => m.Email, c => c.MapFrom(s => s.Acc_Email))
                 .ForMember(m => m.IsActive, c => c.MapFrom(s => s.Acc_IsActive))
                 .ForMember(m => m.ContactNumber, c => c.MapFrom(s => s.Acc_ContactNumber))
                 .ForMember(m => m.InsertedDate, c => c.MapFrom(s => s.Acc_InsertedDate))
                 .ForMember(m => m.ModifiedDate, c => c.MapFrom(s => s.Acc_ModifiedDate));

            CreateMap<Vaccination, VaccinationDto>()
                 .ForMember(m => m.Id, c => c.MapFrom(s => s.Vac_Id))
                 .ForMember(m => m.Name, c => c.MapFrom(s => s.Vac_Name))
                 .ForMember(m => m.Description, c => c.MapFrom(s => s.Vac_Description))
                 .ForMember(m => m.PhotoId, c => c.MapFrom(s => s.Vac_PhotoId))
                 .ForMember(m => m.DaysBetweenVacs, c => c.MapFrom(s => s.Vac_DaysBetweenVacs))
                 .ForMember(m => m.IsActive, c => c.MapFrom(s => s.Vac_IsActive))
                 .ForMember(m => m.InsertedDate, c => c.MapFrom(s => s.Vac_InsertedDate))
                 .ForMember(m => m.InsertedAccId, c => c.MapFrom(s => s.Vac_InsertedAccId))
                 .ForMember(m => m.ModifiedDate, c => c.MapFrom(s => s.Vac_ModifiedDate))
                 .ForMember(m => m.ModifiedAccId, c => c.MapFrom(s => s.Vac_ModifiedAccId));
        }
    }
}
