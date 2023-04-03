namespace PRzHealthcareAPI.Models.DTO
{
    public class ChangeUserPasswordDto
    {
        public string? Login { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? NewPasswordRepeat { get; set; }
    }

}
