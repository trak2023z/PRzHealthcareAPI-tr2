using System.ComponentModel.DataAnnotations.Schema;

namespace PRzHealthcareAPI.Models.DTO
{
    public class RegisterUserDto
    {
        public int Id { get; set; }
        public int AtyId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PhotoBinary { get; set; }
        public string Firstname { get; set; }
        public string? Secondname { get; set; }
        public string Lastname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public long Pesel { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
