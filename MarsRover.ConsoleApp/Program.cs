using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MarsRover.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            Console.WriteLine(configuration["nasaApi:apiKey"]);

            NasaClient nasaClient = new NasaClient(configuration["nasaApi:apiKey"]);

            nasaClient.GetRoverManifests();

            Console.ReadLine();
        }
    }
}