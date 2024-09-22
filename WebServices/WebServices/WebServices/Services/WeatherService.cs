using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Weather.Json;
using Weather.Xml;

namespace Weather.Services
{
    public class WeatherService
    {
        public string _appId { get; set; }
        public string _baseApiUrl { get; set; }
        public string _city { get; set; }

        public WeatherService(string appId, string baseApiUrl, string city)
        {
            _appId = appId;
            _baseApiUrl = baseApiUrl;
            _city = city;
        }   

        public async Task<WeatherResponseJson> GetWeatherJson()
        {
            try
            {

                var payLoad = new Dictionary<string, string>
                {
                    { "q", _city },
                    { "appid", _appId }
                };

                var queryString = string.Join("&", payLoad.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
                var apiUrl = $"{_baseApiUrl}/data/2.5/weather?{queryString}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode) // Check for 200 OK status
                    {
                        var responseData = await response.Content.ReadAsStringAsync();

                        var weatherJsonData = JsonConvert.DeserializeObject<WeatherResponseJson>(responseData); // Deserialize the JSON into WeatherResponse model

                        return weatherJsonData;
                    }
                }

                return null;
            }
            catch (Exception ex) 
            {
                return null;
            }
            
        }

        public async Task<WeatherResponseXml> GetWeatherXml()
        {
            try
            {
                var payLoad = new Dictionary<string, string>
                {
                    { "q", _city },
                    { "appid", _appId },
                    { "mode", "xml" }
                };

                var queryString = string.Join("&", payLoad.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
                var apiUrl = $"{_baseApiUrl}/data/2.5/weather?{queryString}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode) 
                    {
                        var responseData = await response.Content.ReadAsStringAsync();

                        responseData = responseData.Replace("xmlns=\"\"", "");

                        var serializer = new XmlSerializer(typeof(WeatherResponseXml)); // Deserialize response data to XML response

                        using (var stringReader = new StringReader(responseData))
                        {
                            var weatherData = (WeatherResponseXml)serializer.Deserialize(stringReader);

                            return weatherData;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
    }
}
