using Newtonsoft.Json.Linq;

namespace SemestralProject.Services.ExchangeRate;

public class ExchangeRateService
{
    public async Task<decimal> GetExchangeRate(string currencyCode)
    {
        HttpClient client = new HttpClient();
        currencyCode = currencyCode.ToUpper();
        string key = "4bb67da304d41e8d5373f016";
        string url = $"https://v6.exchangerate-api.com/v6/{key}/latest/PLN";
        
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string responsee = await response.Content.ReadAsStringAsync();
        JObject json = JObject.Parse(responsee);
        
        if (json["conversion_rates"][currencyCode] == null)
        {
            throw new Exception($"Currency code: {currencyCode} does not exist");
        }

        decimal rate = (decimal)json["conversion_rates"][currencyCode];
        return rate;
    }
}