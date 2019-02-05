using MarsRover.ConsoleApp.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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

            Stopwatch stopwatch = new Stopwatch();

            string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), configuration["inputFileName"]);

            if (File.Exists(inputFilePath))
            {
                FileStream file = new FileStream(inputFilePath, FileMode.Open);

                List<DateTime> inputDates = FileHandler.ReadDatesFromFile(file);

                NasaClient nasaClient = new NasaClient(configuration["nasaApi:apiKey"]);
                
                int pagesToRequest;
                if(!Int32.TryParse(configuration["numOfPagesToRequest"], out pagesToRequest)) //if false, output == 0
                {
                    pagesToRequest = 1;
                }

                stopwatch.Start();
                Dictionary<string,RoverManifest> roverManifests = Task.Run(() => nasaClient.GetAllRoverManifests()).Result;
                stopwatch.Stop();
                Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds}ms to retrieve rover manifests.");
                stopwatch.Reset();

                Dictionary<string, Dictionary<DateTime,List<RoverPhotoPage>>> roverPhotoPages = new Dictionary<string, Dictionary<DateTime,List<RoverPhotoPage>>>();

                foreach(var key in roverManifests.Keys)
                {
                    roverPhotoPages.Add(key,new Dictionary<DateTime,List<RoverPhotoPage>>());

                    foreach(var date in inputDates)
                    {
                        roverPhotoPages[key].Add(date,new List<RoverPhotoPage>());
                    }
                }

                foreach(var rover in roverManifests)
                {
                    foreach(var date in inputDates)
                    {
                        stopwatch.Start();
                        if(date >= rover.Value.PhotoManifest.LandingDate && date <= rover.Value.PhotoManifest.RecentPhotoDate)
                        {
                            for(int i = 1; i <= pagesToRequest; i++)
                            {
                                RoverPhotoPage photoPage = Task.Run(() => nasaClient.GetRoverPhotoPage(rover.Key,date,i)).Result; // Readability
                                roverPhotoPages[rover.Key][date].Add(photoPage);
                            }
                        }
                        stopwatch.Stop();
                        Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds}ms to retrieve {pagesToRequest} photo page(s) for {rover.Key} taken at {date.ToShortDateString()}.");
                        stopwatch.Reset();
                    }                    
                }

                FileHandler.WriteRoverPhotoPagesToJson(roverPhotoPages);
            }

            Console.ReadLine();
        }
    }
}