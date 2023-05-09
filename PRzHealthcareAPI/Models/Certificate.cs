using System.ComponentModel.DataAnnotations.Schema;

namespace PRzHealthcareAPI.Models
{
    public class Certificate
    {
        public int Cer_Id { get; set; }
        [ForeignKey("Account")]
        public int Cer_AccId { get; set; }
        public string? Cer_Name { get; set; }
        [ForeignKey("BinData")]
        public int Cer_BinId { get; set; }
        public bool Cer_IsActive { get; set; }
        public DateTime Cer_InsertedDate { get; set; }
        public int Cer_InsertedAccId { get; set; }
        public DateTime Cer_ModifiedDate { get; set; }
        public int Cer_ModifiedAccId { get; set; }

        public virtual Account Account { get; set; }
        public virtual BinData BinData { get; set; }
    }
}
