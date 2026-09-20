namespace LMS.WEB.Models
{
    public class EmployeeLeaveHistoryItemViewModel
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalDays => (ToDate.Date - FromDate.Date).Days + 1;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? AdminRemarks { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
