
using System.Text;
using System.Text.Json;
using DataOrderDashboard.Context;
using DataOrderDashboard.Models.ForecastModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using static System.Net.Mime.MediaTypeNames;

namespace DataOrderDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailAIByLastOrdersComponentPartial : ViewComponent
    {
        private readonly BigDataOrderContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MLContext _ml;
        public _CustomerDetailAIByLastOrdersComponentPartial(BigDataOrderContext context, IHttpClientFactory httpClientFactory, MLContext ml)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _ml = ml;
        }
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            // ---------------------------------------------
            // 1) Kullanıcının son 10 siparişi
            // ---------------------------------------------
            var orders = await _context.Orders
                .Where(o => o.CustomerId == id)
                .Include(o => o.Product)
                    .ThenInclude(p => p.Category)
                .OrderBy(o => o.OrderDate)
                .Take(10)
                .ToListAsync();

            // Hiç sipariş yoksa doğrudan fallback ViewModel
            if (!orders.Any())
            {
                var emptyVm = new CustomerDetailForecastViewModel
                {
                    LastOrderDate = "Yok",
                    LastProduct = "Yok",
                    PredictedCategory = "Bilinmiyor",
                    PredictedDate = DateTime.Now.AddDays(30).ToString("dd MMM yyyy")
                };

                return View(emptyVm);
            }

            // Son sipariş bilgileri
            var lastOrder = orders.Last();
            string lastOrderDate = lastOrder.OrderDate.ToString("dd MMM yyyy");
            string lastProduct = lastOrder.Product.ProductName;

            // ---------------------------------------------
            // 2) Sonraki alışveriş tarihini tahmin et 
            // ---------------------------------------------
            var dayGaps = new List<int>();
            for (int i = 1; i < orders.Count; i++)
                dayGaps.Add((orders[i].OrderDate - orders[i - 1].OrderDate).Days);

            int avgGap = dayGaps.Count > 0 ? (int)dayGaps.Average() : 30;
            var predictedDate = lastOrder.OrderDate.AddDays(avgGap).ToString("dd MMM yyyy");

            // ---------------------------------------------
            // 3) ML ile kategori tahmini 
            // ---------------------------------------------
            var grouped = orders
                .GroupBy(o => o.Product.Category.CategoryName)
                .ToList();

            string predictedCategory = "Bilinmiyor";
            double topScore = -1;

            foreach (var group in grouped)
            {
                var monthData = group
                    .Select((x, index) => new CustomerCategoryForecastModel
                    {
                        MonthIndex = index + 1,
                        OrderCount = x.Quantity
                    })
                    .ToList();

                int seriesLength = monthData.Count;

                // ---------------------------------------------------------
                // A) ML.NET güvenli kullanım: SSA 
                // ---------------------------------------------------------
                if (seriesLength < 4)
                { 
                    float fallbackScore = monthData.Sum(m => m.OrderCount);

                    if (fallbackScore > topScore)
                    {
                        topScore = fallbackScore;
                        predictedCategory = group.Key;
                    }

                    continue; // ML kısmına girme
                }

                // ---------------------------------------------------------
                // B) SSA Parametreleri ()
                // ---------------------------------------------------------
                int windowSize = Math.Max(2, seriesLength / 2);
                int horizon = 1;
                int trainSize = seriesLength;

              
                if (windowSize >= seriesLength)
                    windowSize = seriesLength - 1;

                var dataView = _ml.Data.LoadFromEnumerable(monthData);

                var pipeline = _ml.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedValues",
                    inputColumnName: nameof(CustomerCategoryForecastModel.OrderCount),
                    windowSize: windowSize,
                    seriesLength: seriesLength,
                    trainSize: trainSize,
                    horizon: horizon
                );

                var model = pipeline.Fit(dataView);
                var engine = model.CreateTimeSeriesEngine<CustomerCategoryForecastModel,
                    CustomerCategoryForecastPredictionModel>(_ml);

                var prediction = engine.Predict();
                float forecastValue = prediction.ForecastedValues[0];

                if (forecastValue > topScore)
                {
                    topScore = forecastValue;
                    predictedCategory = group.Key;
                }
            }

            var vm = new CustomerDetailForecastViewModel
            {
                LastOrderDate = lastOrderDate,
                LastProduct = lastProduct,
                PredictedCategory = predictedCategory,
                PredictedDate = predictedDate
            };

            return View(vm);
        }

    }
}



