using System.ComponentModel.DataAnnotations.Schema;

namespace PRzHealthcareAPI.Models
{
    public class Notification
    {
        public int Not_Id { get; set; }
        [ForeignKey("Event")]
        public int Not_EveId { get; set; }
        [ForeignKey("NotificationType")]
        public int Not_NtyId { get; set; }
        public DateTime Not_SendTime { get; set; }
        public bool Not_IsActive { get; set; }
        public DateTime Not_InsertedDate { get; set; }
        public int Not_InsertedAccId { get; set; }
        public DateTime Not_ModifiedDate { get; set; }
        public int Not_ModifiedAccId { get; set; }

        public virtual Event Event { get; set; }
        public virtual NotificationType NotificationType { get; set; }
    }
}
