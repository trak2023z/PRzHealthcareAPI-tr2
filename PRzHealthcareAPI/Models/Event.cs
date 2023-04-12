using System.ComponentModel.DataAnnotations.Schema;

namespace PRzHealthcareAPI.Models
{
    public class Event
    {
        public int Eve_Id { get; set; }
        [ForeignKey("Account")]
        public int? Eve_AccId { get; set; }  
        public DateTime Eve_TimeFrom { get; set; }
        public DateTime Eve_TimeTo { get; set; }
        [ForeignKey("EventType")]
        public int Eve_Type { get; set; }
        public int? Eve_DoctorId { get; set; }
        [ForeignKey("Vaccination")]
        public int? Eve_VacId { get; set; }
        public string Eve_Description { get; set; }
        public bool Eve_IsActive { get; set; }  
        public DateTime Eve_InsertedDate { get; set; }
        public int Eve_InsertedAccId { get; set; }
        public DateTime Eve_ModifiedDate { get; set; }
        public int Eve_ModifiedAccId { get; set; }
        public string? Eve_SerialNumber { get; set; }

        public virtual Account Account { get; set; }
        public virtual Vaccination Vaccination { get; set; }
    }
}
