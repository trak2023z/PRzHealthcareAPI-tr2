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
        }
    }
}
