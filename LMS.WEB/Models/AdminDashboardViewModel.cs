namespace LMS.WEB.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalPendingRequests { get; set; }
        public int TotalApprovedRequests { get; set; }
        public int TotalRejectedRequests { get; set; }

        public List<AdminLeaveItemViewModel> RecentRequests { get; set; } = new();
    }
}
