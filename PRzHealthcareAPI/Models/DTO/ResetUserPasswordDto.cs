namespace PRzHealthcareAPI.Models.DTO
{
    public class ResetUserPasswordDto
    {
        public string? HashCode { get; set; }
        public string? NewPassword { get; set; }
        public string? NewPasswordRepeat { get; set; }
    }

}
