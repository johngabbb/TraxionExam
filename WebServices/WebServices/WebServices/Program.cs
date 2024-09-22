using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using App.Configuration;
using Weather.Json;
using Newtonsoft.Json;
using Weather.Services;

public class Program
{
    static async Task Main(string[] args)
    {
        try
        {

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)  // Add appsettings.json
                .Build();

            var configuration = config.GetSection("Configuration").Get<Configuration>();
            var appId = configuration.AppId;
            var baseUrl = configuration.BaseApiUrl;

            var cityName = string.Empty;

            while (string.IsNullOrEmpty(cityName))
            {
                Console.Write("Enter City Name: ");
                cityName = Console.ReadLine();
            }

            var weatherService = new WeatherService(appId, baseUrl, cityName);
            var weatherJsonResponse = await weatherService.GetWeatherJson();    // Gets Weather Json Response
            var weatherXmlResponse = await weatherService.GetWeatherXml();      // Gets Weather Xml Response

        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine("There was a problem with the HTTP request.");
            Console.WriteLine(httpEx.Message);
        }
        catch (TaskCanceledException timeoutEx)
        {
            Console.WriteLine("The request timed out.");
            Console.WriteLine(timeoutEx.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred:");
            Console.WriteLine(ex.Message);
        }

    }
}