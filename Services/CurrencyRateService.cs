using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Schimb_Valutar_API.Services.Interfaces;

namespace Schimb_Valutar_API.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly HttpClient _httpClient;

        public CurrencyRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, decimal>> GetRatesAsync()
        {
            var rates = new Dictionary<string, decimal>();

            try
            {
                var response = await _httpClient.GetStringAsync("https://www.bnr.ro/nbrfxrates.xml");

                response = CleanXmlResponse(response);
                XDocument doc = XDocument.Parse(response);

                XNamespace ns = "http://www.bnr.ro/xsd";

                var cubeElement = doc.Descendants(ns + "Cube").FirstOrDefault();
                if (cubeElement != null)
                {
                    foreach (var rateElement in cubeElement.Elements(ns + "Rate"))
                    {
                        var currencyCode = rateElement.Attribute("currency")?.Value;
                        var rateString = rateElement.Value;
                        var multiplierAttribute = rateElement.Attribute("multiplier");

                        int multiplier = 1;
                        if (multiplierAttribute != null && int.TryParse(multiplierAttribute.Value, out var parsedMultiplier))
                        {
                            multiplier = parsedMultiplier;
                        }

                        if (!string.IsNullOrEmpty(currencyCode) && decimal.TryParse(rateString, out var rate))
                        {
                            rates[currencyCode] = rate / multiplier;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid rate data: CurrencyCode={currencyCode}, RateString={rateString}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Cube element not found in XML.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP request error: {httpEx.Message}");
                throw new ApplicationException("Error fetching XML response from BNR", httpEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                throw new ApplicationException("Error parsing XML response from BNR", ex);
            }

            return rates;
        }

        private string CleanXmlResponse(string xml)
        {
            xml = xml.Replace("&lt;", "<").Replace("&gt;", ">");
            return xml;
        }
    }
}
