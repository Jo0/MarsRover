using MarsRover.Core;
using MarsRover.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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

                Console.WriteLine("Reading dates from input file.");

                List<DateTime> inputDates = FileHandler.ReadDatesFromFile(file);

                Console.WriteLine("Done.\n");

                NasaClient nasaClient = new NasaClient(configuration["nasaApi:apiKey"]);

                int pagesToRequest = ParseNumOfPagesToRequest(configuration["numOfPagesToRequest"]); //defaults to 1

                Console.WriteLine("Getting rover manifests.");

                Dictionary<string, RoverManifest> roverManifests = Task.Run(() => nasaClient.GetAllRoverManifests()).Result;

                Console.WriteLine("Done.\n");

                Dictionary<string, Dictionary<DateTime, List<RoverPhotoPage>>> roverPhotoPages = InitializeRoverPhotoPages(roverManifests, inputDates);

                Console.WriteLine("Begin fetching photos");
                foreach (var rover in roverManifests)
                {
                    Console.WriteLine($"\tFetch photos for {rover.Key}");
                    foreach (var date in inputDates)
                    {
                        Console.WriteLine($"\t\t\tat {date.ToShortDateString()}");
                        if (date >= rover.Value.PhotoManifest.LandingDate && date <= rover.Value.PhotoManifest.RecentPhotoDate)
                        {
                            for (int i = 1; i <= pagesToRequest; i++)
                            {
                                Console.WriteLine($"\t\t\t\tPage {i}");
                                RoverPhotoPage photoPage = Task.Run(() => nasaClient.GetRoverPhotoPage(rover.Key, date, i)).Result;
                                roverPhotoPages[rover.Key][date].Add(photoPage);
                            }
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("Download photos");
                nasaClient.DownloadImagesForRovers(roverPhotoPages, Directory.GetCurrentDirectory());
                Console.WriteLine("Done.");

                Console.WriteLine("Writing Rover Photo Pages to output.json");
                FileHandler.WriteRoverPhotoPagesToJson(roverPhotoPages);
                Console.WriteLine("Done.\n");
            }

            Console.WriteLine("Enter key to exit.");
            Console.ReadLine();
        }

        public static int ParseNumOfPagesToRequest(string configValue)
        {
            try
            {
                int parsedInt = Int32.Parse(configValue);
                
                if (parsedInt > 0)
                {
                    return parsedInt;
                }
                else
                {
                    return 1;
                }
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