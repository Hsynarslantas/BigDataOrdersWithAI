using DataOrderDashboard.Context;
using DataOrderDashboard.Entities;
using DataOrderDashboard.Models.LoyaltyMachineLearningModels;
using DataOrderDashboard.Models.LoyaltyModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;

namespace DataOrderDashboard.Controllers
{
    public class CustomerLoyaltyController : Controller
    {
        private readonly BigDataOrderContext _context;
        private readonly string _modelPath = "wwwroot/mlModels/LoyaltyScoreModel.zip";

        public CustomerLoyaltyController(BigDataOrderContext context)
        {
            _context = context;
        }

        public IActionResult EnglandLoyaltyScore()
        {
            var loyaltyScore = _context.Customers.Include(c => c.Orders).ThenInclude(o => o.Product).Where(
                c => c.CustomerCountry == "İngiltere" && c.CustomerCity == "Londra").Select(c => new
                {
                    CustomerFullName = c.CustomerName + " " + c.CustomerSurname,
                    TotalOrders = c.Orders.Count(),
                    TotalSpent = c.Orders.Sum(o => o.Quantity * o.Product.UnitPrice),
                    LastOrderDate = c.Orders.Max(o => (DateTime?)o.OrderDate)
                }).AsEnumerable().Select(x =>
                {
                    var daySinceLastOrder = (x.LastOrderDate.HasValue)
                    ? (DateTime.Now - x.LastOrderDate.Value).TotalDays
                    : double.MaxValue;
                    double recencyScore = daySinceLastOrder switch
                    {
                        <= 30 => 100,
                        <= 90 => 75,
                        <= 180 => 50,
                        <= 365 => 25,
                        _ => 10
                    };
                    double frequency = x.TotalOrders switch
                    {
                        >= 20 => 100,
                        >= 10 => 80,
                        >= 5 => 60,
                        >= 2 => 40,
                        1 => 20,
                        _ => 10,

                    };
                    double monetaryScore = x.TotalSpent switch
                    {
                        >= 5000 => 100,
                        >= 3000 => 80,
                        >= 1000 => 60,
                        >= 500 => 40,
                        >= 100 => 20,
                        _ => 10,
                    };
                    double loyaltyScore = (recencyScore * 0.4) + (frequency * 0.3) + (monetaryScore * 0.3);
                    return new LoyaltyScoreModel
                    {
                        CustomerFullName = x.CustomerFullName,
                        TotalOrders = x.TotalOrders,
                        TotalSpent = Math.Round(x.TotalSpent, 2),
                        LastOrderDate = x.LastOrderDate,
                        LoyaltyScore = Math.Round(loyaltyScore, 2),
                    };
                }).OrderByDescending(x => x.LoyaltyScore).ToList();
            return View(loyaltyScore);
        }

        public IActionResult EnglandLoyaltyScoreWithML()
        {
            //Tarih Atama 
            var firstDate = _context.Orders.Max(x => x.OrderDate);

            var data = _context.Customers.Include(c => c.Orders).ThenInclude(o => o.Product).Where(
               c => c.CustomerCountry == "İngiltere" && c.CustomerCity == "Londra").AsEnumerable().Select(c =>
               {
                   //Son Sipariş Tarihini Bulma
                   var lastOrderDate = c.Orders.Max(o => (DateTime?)o.OrderDate);
                   //Son Sipariş Tarihinin Üzerinden Kaç Gün Geçtiğini Hesaplama
                    var daySince = lastOrderDate.HasValue
                    ? (firstDate - lastOrderDate.Value).TotalDays
                    : double.MaxValue;
                   //RFM metrikleri
                   double recency = daySince;
                   double frequency = c.Orders.Count();
                   double monetary = c.Orders.Sum(o => o.Quantity * o.Product.UnitPrice);

                   //Loyalty Skor ağırlıklı ortalama bulma
                   double loyalty = (RecencyScore(recency) * 0.4) +
                   (FrequencyScore(frequency) * 0.3) +
                   (MonetaryScore(monetary) * 0.3);

                   //ML.net gidecek Veri Listesi
                   return new LoyaltyScoreMlDataModel
                   {
                       CustomerName = c.CustomerName + " " + c.CustomerSurname,
                       Recency = (float)recency,
                       Frequency = (float)frequency,
                       Monetary = (float)monetary,
                       LoyaltyScore = (float)loyalty,
                   };

               }).ToList();
            //ML İşlemleri 
            var mlContext = new MLContext();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);

            //Pipeline
            var pipeline = mlContext.Transforms
                .Concatenate("Features", "Recency", "Frequency", "Monetary")
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.Regression.Trainers.Sdca(
                    labelColumnName: "LoyaltyScore",
                    maximumNumberOfIterations: 100
                    ));

            //Model Eğitme 
            var model = pipeline.Fit(dataView);
            //Model Kaydetme 
            mlContext.Model.Save(model, dataView.Schema, _modelPath);
            //Tahmin Methodu
            var predictionEngine = mlContext.Model.
                CreatePredictionEngine<LoyaltyScoreMlDataModel, LoyaltyScoreMlDataPredictionModel>(model);
            //Her Müşteri İçin ML .Net Tahmini
            var results = data.Select(x =>
            {
                var prediction = predictionEngine.Predict(new LoyaltyScoreMlDataModel
                {
                    Recency = x.Recency,
                    Frequency = x.Frequency,
                    Monetary = x.Monetary,
                });
                return new ResultLoyaltyScoreMlDataModel
                {
                    CustomerName = x.CustomerName,
                    Recency = x.Recency,
                    Frequency = x.Frequency,
                    Monetary = x.Monetary,
                    ActualLoyaltyScore = Math.Round(x.LoyaltyScore, 2),
                    PredictedLoyaltyScore = Math.Round(Math.Max(0, Math.Min(100, prediction.LoyaltyScore)), 2)
                };
            }).OrderByDescending(x => x.PredictedLoyaltyScore).ToList();
            return View(results);
        }

        //Yardımcı Skor methodları hazırlama 
        private static double RecencyScore(double days) => days switch
        {
            <= 30 => 100,
            <= 90 => 75,
            <= 180 => 50,
            <= 365 => 25,
            _ => 10
        };
        private static double FrequencyScore(double orders) => orders switch
        {
            >= 20 => 100,
            >= 10 => 80,
            >= 5 => 60,
            >= 2 => 40,
            1 => 20,
            _ => 10
        };
        private static double MonetaryScore(double spent) => spent switch
        {
            >= 5000 => 100,
            >= 3000 => 80,
            >= 1000 => 60,
            >= 500 => 40,
            >= 100 => 20,
            _ => 10
        };
    }
}
