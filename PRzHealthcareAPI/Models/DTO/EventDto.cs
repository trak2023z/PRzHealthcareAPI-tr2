using System.ComponentModel.DataAnnotations.Schema;

namespace PRzHealthcareAPI.Models.DTO
{
    public class EventDto
    {
        public int Id { get; set; }
        public int AccId { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public int Type { get; set; }
        public int DoctorId { get; set; }
        public int VacId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime InsertedDate { get; set; }
        public int InsertedAccId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedAccId { get; set; }
    }
}
