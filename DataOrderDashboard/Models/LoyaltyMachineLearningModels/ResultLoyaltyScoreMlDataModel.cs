namespace DataOrderDashboard.Models.LoyaltyMachineLearningModels
{
    public class ResultLoyaltyScoreMlDataModel
    {
        public string CustomerName { get; set; }
        public double Recency { get; set; }
        public double Frequency { get; set; }
        public double Monetary { get; set; }
        public double ActualLoyaltyScore { get; set; }
        public double PredictedLoyaltyScore { get; set; }
      
    }
}
