namespace LMS.WEB.Models
{
    public class EmployeeDashboardViewModel
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;

        public int TotalLeavesCount { get; set; }
        public int PendingLeavesCount { get; set; }
        public int ApprovedLeavesCount { get; set; }
        public int RejectedLeavesCount { get; set; }

        public List<EmployeeLeaveHistoryItemViewModel> MyLeaves { get; set; } = new();
    }
}
