using Microsoft.ML.Data;

namespace DataOrderDashboard.Training
{
    public class MessageModelOutput
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; } = "";
        public string MyProperty { get; set; } = "";
    }
}
