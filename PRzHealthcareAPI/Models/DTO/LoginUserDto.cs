namespace PRzHealthcareAPI.Models.DTO
{
    public class LoginUserDto
    {
        public int AccId { get; set; }
        public string? Login { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
        public int AtyId { get; set; }
        public string? Token { get; set; }
    }
}
