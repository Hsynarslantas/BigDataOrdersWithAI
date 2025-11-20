namespace DataOrderDashboard.Models.LoyaltyModels
{
    public class LoyaltyScoreModel
    {
        public string CustomerFullName { get; set; }
        public int TotalOrders { get; set; }
        public double TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public double LoyaltyScore { get; set; }
    }
}
