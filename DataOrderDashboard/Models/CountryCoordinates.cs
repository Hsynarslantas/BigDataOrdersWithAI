namespace DataOrderDashboard.Models
{
    public class CountryCoordinates
    {
        private static readonly Dictionary<string, (double Lat, double Lon)>
    _coords = new()
    {
    { "Türkiye", (39.9208, 32.8541) },
    { "Fransa", (48.8566, 2.3522) },
    { "Almanya", (52.5200, 13.4050) },
    { "İspanya", (40.4168, -3.7038) },
    { "İtalya", (41.9028, 12.4964) },
    { "Hollanda", (52.3676, 4.9041) },
    { "Belçika", (50.8503, 4.3517) },
    { "Avusturya", (48.2100, 16.3700) },
    { "İskoçya", (55.9533, -3.1883) },
    { "Portekiz", (38.7169, -9.1390) }, 
    { "İngiltere", (51.5074, -0.1278) },
    };

        public static double GetLat(string country)
        => _coords.ContainsKey(country) ? _coords[country].Lat : 0;

        public static double GetLon(string country)
        => _coords.ContainsKey(country) ? _coords[country].Lon : 0;
    }
}

