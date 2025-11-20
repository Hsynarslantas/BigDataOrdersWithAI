namespace DataOrderDashboard.Models.ForecastModels
{
    public class NextCategoryInputModel
    {
        public float CategoryId { get; set; }
        public float Quantity { get; set; }
        public float TotalPrice { get; set; }
        public float DaysSinceLastOrder { get; set; }
    }
}
