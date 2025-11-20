namespace DataOrderDashboard.Models.ForecastModels
{
    public class NextCategoryPredictionModel
    {
        public float PredictedCategoryId { get; set; }
        public float[] Score { get; set; }
    }
}
