using Microsoft.ML.Data;

namespace DataOrderDashboard.Models.LoyaltyMachineLearningModels
{
    public class LoyaltyScoreMlDataPredictionModel
    {
        [ColumnName("Score")]
        public float LoyaltyScore { get; set; }
    }
}
