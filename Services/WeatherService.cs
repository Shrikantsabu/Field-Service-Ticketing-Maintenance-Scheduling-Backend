namespace Field_Service_Ticketing___Maintenance_Scheduling_Backend.Services
{
    public class WeatherService : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherService> _logger;
        public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<string> GetWeatherRiskFlagAsync(string siteLocation)
        {
            // For simplicity assume siteLocation contains latitude,longitude
            try
            {
                var parts = siteLocation.Split(',');
                if (parts.Length != 2)
                    return "UNKNOWN";
                var lat = parts[0].Trim();
                var lon = parts[1].Trim();
                var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Weather API returned status {response.StatusCode}");
                    return "UNKNOWN";
                }
                var weatherData = await response.Content.ReadFromJsonAsync<OpenMeteoResponse>();
                if (weatherData?.CurrentWeather == null)
                    return "UNKNOWN";
                // Example logic for weather risk: high wind -> severe, temp extremes -> caution
                var windSpeed = weatherData.CurrentWeather.Windspeed;
                var temperature = weatherData.CurrentWeather.Temperature;
                if (windSpeed > 20) return "SEVERE";
                if (temperature < 0 || temperature > 35) return "CAUTION";
                return "NONE";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Weather API call failed");
                return "UNKNOWN";
            }
        }
    }
    public class OpenMeteoResponse
    {
        public CurrentWeather? CurrentWeather { get; set; }
    }
    public class CurrentWeather
    {
        public double Temperature { get; set; }
        public double Windspeed { get; set; }
    }

}

