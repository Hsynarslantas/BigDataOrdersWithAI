using DataOrderDashboard.Context;
using DataOrderDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardMapComponentPartial : ViewComponent
    {
        private readonly BigDataOrderContext _context;

        public _DashboardMapComponentPartial(BigDataOrderContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"
SELECT 
    t1.CustomerCountry AS Country,
    t1.Total2024,
    t2.Total2025,
    CAST(((t2.Total2025 - t1.Total2024) * 100.0 / t1.Total2024) AS DECIMAL(5,2)) AS ChangeRate
FROM
(
    SELECT 
        c.CustomerCountry, 
        COUNT(*) AS Total2024
    FROM Orders o
    INNER JOIN Customers c ON o.CustomerId = c.CustomerId
    WHERE o.OrderDate >= '2024-01-01' AND o.OrderDate < '2025-01-01'
    GROUP BY c.CustomerCountry
) AS t1
INNER JOIN
(
    SELECT 
        c.CustomerCountry, 
        COUNT(*) AS Total2025
    FROM Orders o
    INNER JOIN Customers c ON o.CustomerId = c.CustomerId
    WHERE o.OrderDate >= '2025-01-01' AND o.OrderDate < '2026-01-01'
    GROUP BY c.CustomerCountry
) AS t2
ON t1.CustomerCountry = t2.CustomerCountry;";
                _context.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<CountryReportViewModel>();
                    while (reader.Read())
                    {
                        var countryName = reader.GetString(0);
                        result.Add(new CountryReportViewModel
                        {
                            Country = countryName,
                            Total2024 = reader.GetInt32(1),
                            Total2025 = reader.GetInt32(2),
                            ChangeRate = reader.GetDecimal(3),
                            Latitude=CountryCoordinates.GetLat(countryName),
                            Longitude=CountryCoordinates.GetLon(countryName),
                        });
                    }
                    return View(result);
                }
            }
        }
    }
}
