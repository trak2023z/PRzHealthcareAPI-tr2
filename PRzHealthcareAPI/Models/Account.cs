using System.ComponentModel.DataAnnotations.Schema;

namespace PRzHealthcareAPI.Models
{
    public class Account
    {
        public int Acc_Id { get; set; }

        [ForeignKey("AccountType")]
        public int Acc_AtyId { get; set; }
        public string Acc_Login { get; set; }
        public string Acc_Password { get; set;}
        public int Acc_PhotoId { get; set; }
        public string Acc_Firstname { get; set; }
        public string? Acc_Secondname { get; set; }
        public string Acc_Lastname { get; set; }
        public DateTime Acc_DateOfBirth { get; set; }
        public long Acc_Pesel { get; set; }
        public string Acc_Email { get; set; }
        public string Acc_ContactNumber { get; set; }
        public bool Acc_IsActive { get; set; }
        public DateTime Acc_InsertedDate { get; set; }
        public DateTime Acc_ModifiedDate { get; set; }

        public virtual AccountType AccountType { get; set; }
    }
}
