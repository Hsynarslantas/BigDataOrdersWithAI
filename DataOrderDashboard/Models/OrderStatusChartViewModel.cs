namespace DataOrderDashboard.Models
{
    public class OrderStatusChartViewModel
    {
        public int Percantage { get; set; }
        public bool IsPositive { get; set; }
        public string Title { get; set; }
        public string ChangeText { get; set; }
        public string Color { get; set; }
    }
}
