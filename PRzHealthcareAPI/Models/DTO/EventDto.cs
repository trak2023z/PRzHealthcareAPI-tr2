using System.ComponentModel.DataAnnotations.Schema;

namespace PRzHealthcareAPI.Models.DTO
{
    public class EventDto
    {
        public int Id { get; set; }
        public int AccId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? TimeFrom { get; set; }
        public string? TimeTo { get; set; }
        public int Type { get; set; }
        public int DoctorId { get; set; }
        public int VacId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime InsertedDate { get; set; }
        public int InsertedAccId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedAccId { get; set; }
    }
}
