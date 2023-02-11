using System.ComponentModel.DataAnnotations.Schema;

namespace PRzHealthcareAPI.Models.DTO
{
    public class VaccinationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PhotoId { get; set; }
        public string PhotoBinary { get; set; }
        public int DaysBetweenVacs { get; set; }
        public bool IsActive { get; set; }
        public DateTime InsertedDate { get; set; }
        public int InsertedAccId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedAccId { get; set; }
    }
}
