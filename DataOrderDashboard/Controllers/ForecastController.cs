using DataOrderDashboard.Context;
using DataOrderDashboard.Models.ForecastModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace DataOrderDashboard.Controllers
{
    public class ForecastController : Controller
    {
        private readonly   BigDataOrderContext _context;
        private readonly MLContext _mlContext;
        public ForecastController(BigDataOrderContext context, MLContext mlContext)
        {
            _context = context;
            _mlContext = mlContext;
        }
        //Ödeme Yöntemi Tahminleri
        public IActionResult PaymentMethodForecast()
        {
            var startDate = new DateTime(2025,01,01);
            var endDate = new DateTime(2025,12,31);

            var montlyPaymentData=_context.Orders.Where(o=>o.OrderDate>startDate &&  o.OrderDate < endDate).AsEnumerable().GroupBy(o => new
            {
                Month=new DateTime(o.OrderDate.Year,o.OrderDate.Month,1),
                o.PaymentMethod
            })
                .Select(g => new
                {
                    Month=g.Key.Month,
                    PaymentMethod=g.Key.PaymentMethod,
                    OrderCount=g.Count()
                })
                .OrderBy(x=>x.Month) .ToList();
            // Tahmin Yapılan Listeyi Tutan Kısım
            var forecasts = new List<Object>();
            // Her Ödeme Yöntemi İçin Ayrı Ayrı Model Oluşturan Kısım
            foreach(var method in montlyPaymentData.Select(x => x.PaymentMethod).Distinct())
            {
                var methodData=montlyPaymentData 
                    .Where(x=>x.PaymentMethod==method)
                    .Select((x,index)=>new PaymentMethodForecastModel
                    {
                        PaymentMethod=method,
                        MonthIndex=index+1,
                        OrderCount=x.OrderCount
                    }).ToList();
                var dataView=_mlContext.Data.LoadFromEnumerable(methodData);

                //Forecast Modeli


                var pipeline = _mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedValues",
                    inputColumnName: nameof(PaymentMethodForecastModel.OrderCount),
                    windowSize: 4,
                    seriesLength: methodData.Count,
                    trainSize: methodData.Count,
                    horizon: 3,
                    confidenceLevel: 0.95f
                    );

                var model=pipeline.Fit(dataView);
                var engine = model.CreateTimeSeriesEngine<PaymentMethodForecastModel, PaymentMethodForecastPredictionModel>(_mlContext);
                var prediction = engine.Predict();

                //2026 Ocak-Şubat-Mart Tahmnileme

                for(int i=0; i<prediction.ForecastedValues.Length; i++)
                {
                    forecasts.Add(new
                    {
                        PaymentMethod = method,
                        Month = new DateTime(2026, i + 1, 1).ToString("yyyy MMM"),
                        ForecastedCount = (int)prediction.ForecastedValues[i]
                    });
                }
            } 
            return View(forecasts);
        }
        //Almanya Şehir Satış Tahminleri
        public IActionResult GermanyOrderSalesForecast()
        {
            var startDate=new DateTime(2023,1,1);
            var EndDate = new DateTime(2025, 12, 31);

            var cityOrderData=_context.Orders.Include(x=>x.Customer).Where(o=>o.OrderDate>=startDate && o.OrderDate<EndDate && o.Customer.CustomerCountry=="Almanya")
                .AsEnumerable()
                .GroupBy(o => new 
                {
                    o.Customer.CustomerCity,
                    Year=o.OrderDate.Year,
                    Month=o.OrderDate.Month,

                })
                .Select(k => new
                {
                    City=k.Key.CustomerCity,
                    Year=k.Key.Year,
                    DateKey=$"{k.Key.Year}-{k.Key.Month:D2}",
                    OrderCount=k.Count()
                }).OrderBy(x => x.City).ThenBy(x=>x.DateKey).ToList();

            var forecasts = new List<object>();
            foreach (var city in cityOrderData.Select(x => x.City).Distinct())
            {
                var citydata=cityOrderData.Where(x=>x.City==city).Select((x,index)=>new GermanyCitiesForecastDataModel
                {
                    City=x.City,
                    MonthIndex=index+1,
                    OrderCount=x.OrderCount
                }).ToList();

                if (citydata.Count < 4)
                    continue;
                    
                var dataView=_mlContext.Data.LoadFromEnumerable(citydata);

                var pipeline = _mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedValues",
                    inputColumnName: nameof(GermanyCitiesForecastDataModel.OrderCount),
                    windowSize: 12,
                    seriesLength: citydata.Count,
                    trainSize: citydata.Count,
                    horizon: 12,
                    confidenceLevel: 0.95f
                    );
                var model=pipeline.Fit(dataView);
                var engine = model.CreateTimeSeriesEngine<GermanyCitiesForecastDataModel, GermanyCitiesForecastPredictionModel>(_mlContext);
                var prediction = engine.Predict();

                var yearlyForecast = (int)prediction.ForecastedValues.Sum();

                var year2024Count=cityOrderData
                    .Where(x=>x.City==city && x.Year==2024)
                    .Sum(x=>x.OrderCount);
                var year2025Count = cityOrderData
                   .Where(x => x.City == city && x.Year == 2025)
                   .Sum(x => x.OrderCount);

                var diff = yearlyForecast - year2025Count;
                double? growthDate = year2025Count > 0
                    ? (diff / (double)year2025Count) *100.0
                    : (double?)null;

                forecasts.Add(new
                {
                    City = city,
                    Year2024=year2024Count,
                    Year2025=year2025Count,
                    Year = "2026",
                    ForecastedCount = yearlyForecast,
                    GrowthRate=growthDate
                });

            }
            return View(forecasts);
        }

        public IActionResult MostSalesProduct2026Forecast()
        {
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2025, 12, 31);

           
            var topProducts = _context.Orders
                .Include(o => o.Product)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => new { o.ProductId, o.Product.ProductName })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalSales = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(5)
                .ToList();

            var forecasts = new List<object>();

            foreach (var product in topProducts)
            {
            
                var productSalesData = _context.Orders
                    .Where(o => o.ProductId == product.ProductId && o.OrderDate >= startDate && o.OrderDate <= endDate)
                    .AsEnumerable()
                    .GroupBy(o => new { Year = o.OrderDate.Year, Month = o.OrderDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        OrderCount = g.Sum(x => x.Quantity)
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToList();

            
                if (productSalesData.Count < 6)
                    continue;

               
                var modelData = productSalesData.Select((x, index) => new MostSalesProductsModel
                {
                    MonthIndex = index + 1,
                    OrderCount = x.OrderCount
                }).ToList();
                var dataView = _mlContext.Data.LoadFromEnumerable(modelData);
                var pipeline = _mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedValues",
                    inputColumnName: nameof(MostSalesProductsModel.OrderCount),
                    windowSize: 6,
                    seriesLength: modelData.Count,
                    trainSize: modelData.Count,
                    horizon: 12,
                    confidenceLevel: 0.95f
                );
                var model = pipeline.Fit(dataView);
                var engine = model.CreateTimeSeriesEngine<MostSalesProductsModel, MostSalesProductsPredictionModel>(_mlContext);
                var prediction = engine.Predict();

                var year2024Count = productSalesData.Where(x => x.Year == 2024).Sum(x => x.OrderCount);
                var year2025Count = productSalesData.Where(x => x.Year == 2025).Sum(x => x.OrderCount);
              
                var forecasted2026 = (int)prediction.ForecastedValues.Sum();
                forecasts.Add(new
                {
                    Product = product.ProductName,
                    Year2024 = year2024Count,
                    Year2025 = year2025Count,
                    ForecastedValues = forecasted2026
                });
            }

            return View(forecasts);
        }


    }
}
