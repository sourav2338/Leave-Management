using LMS.Domain;

namespace LMS.WEB.Models
{
    public class LeaveViewModel
    {
        public int Id { get; set; }

        public DateTime FromDate { get; set; } = DateTime.UtcNow;

        public DateTime ToDate { get; set; } = DateTime.UtcNow.AddDays(1);

        public string? Reason { get; set; }
       
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminRemarks { get; set; }
        public int? ReviewedByAdminId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public int UpdatedBy { get; set; }
        public DateTime UpdatedON { get; set; } = DateTime.UtcNow;
    }
}
