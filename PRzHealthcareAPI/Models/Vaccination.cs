using System.ComponentModel.DataAnnotations.Schema;

namespace PRzHealthcareAPI.Models
{
    public class Vaccination
    {
        public int Vac_Id { get; set; }
        public string Vac_Name { get; set; }
        public string Vac_Description { get; set; }
        [ForeignKey("BinData")]
        public int? Vac_PhotoId { get; set; }
        public int Vac_DaysBetweenVacs { get; set; }
        public bool Vac_IsActive { get; set; }
        public DateTime Vac_InsertedDate { get; set; }
        public int Vac_InsertedAccId { get; set; }
        public DateTime Vac_ModifiedDate { get; set; }
        public int Vac_ModifiedAccId { get; set; }

        public virtual BinData BinData { get; set; }
    }
}
