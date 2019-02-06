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

            string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), configuration["inputFileName"]);

            Console.WriteLine($"Opening {inputFilePath}");
            if (File.Exists(inputFilePath))
            {
                FileStream file = new FileStream(inputFilePath, FileMode.Open);

                Console.WriteLine("Reading dates from input file.\n");

                List<DateTime> inputDates = FileHandler.ReadDatesFromFile(file);

                Console.WriteLine("Done.\n");

                NasaClient nasaClient = new NasaClient(configuration["nasaApi:apiKey"]);

                int pagesToRequest = ParseNumOfPagesToRequest(configuration["numOfPagesToRequest"]); //defaults to 1

                Console.WriteLine("Getting rover manifests.");

                Dictionary<string,RoverManifest> roverManifests = Task.Run(() => nasaClient.GetAllRoverManifests()).Result;

                Console.WriteLine("Done.\n");

                Dictionary<string, Dictionary<DateTime, List<RoverPhotoPage>>> roverPhotoPages = InitializeRoverPhotoPages(roverManifests, inputDates);

                Console.WriteLine("Begin fetching photos");
                foreach(var rover in roverManifests)
                {
                    Console.WriteLine($"\tFetch photos for {rover.Key}");
                    foreach(var date in inputDates)
                    {
                        Console.WriteLine($"\t\t\tat {date.ToShortDateString()}");
                        if(date >= rover.Value.PhotoManifest.LandingDate && date <= rover.Value.PhotoManifest.RecentPhotoDate)
                        {
                            for(int i = 1; i <= pagesToRequest; i++)
                            {
                                Console.WriteLine($"\t\t\t\tPage {i}");
                                RoverPhotoPage photoPage = Task.Run(() => nasaClient.GetRoverPhotoPage(rover.Key,date,i)).Result;
                                roverPhotoPages[rover.Key][date].Add(photoPage);
                            }
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("Writing to output.json");
                FileHandler.WriteRoverPhotoPagesToJson(roverPhotoPages);
                Console.WriteLine("Done.\n");
            }

            string htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "View", "index.html");

            if (File.Exists(htmlFilePath))
            {
                Console.WriteLine("Viewing output through browser....\n");
                try
                {
                    Process.Start(htmlFilePath);
                }
                catch
                {
                    Console.WriteLine($"Error opening {htmlFilePath}\n");
                }
            }

            Console.WriteLine("Enter key to exit.");
            Console.ReadLine();
        }

        public static int ParseNumOfPagesToRequest(string configValue)
        {
            try
            {
                return Int32.Parse(configValue);
            }
            catch
            {
                return 1;
            }          
        }

        public static Dictionary<string, Dictionary<DateTime, List<RoverPhotoPage>>> InitializeRoverPhotoPages(Dictionary<string, RoverManifest> roverManifests, List<DateTime> inputDates)
        {
            Dictionary<string, Dictionary<DateTime, List<RoverPhotoPage>>> roverPhotoPages = new Dictionary<string, Dictionary<DateTime, List<RoverPhotoPage>>>();

            foreach (var key in roverManifests.Keys)
            {
                roverPhotoPages.Add(key, new Dictionary<DateTime, List<RoverPhotoPage>>());

                foreach (var date in inputDates)
                {
                    roverPhotoPages[key].Add(date, new List<RoverPhotoPage>());
                }
            }

            return roverPhotoPages;
        }        
    }
}