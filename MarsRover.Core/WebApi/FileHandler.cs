using MarsRover.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MarsRover.Core.WebApi
{
    public class FileHandler
    {
        public static List<string> ReadDatesFromFile(Stream stream)
        {
            List<string> parsedDates = new List<string>();

            using (StreamReader reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();

                    try
                    {
                        DateTime parsedDate = DateTime.Parse(line);

                        //_logger.Info($"Parse {line} to {parsedDate}");

                        parsedDates.Add(parsedDate.ToString("o")); //SQLite expects ISO8601 as date string format when storing as TEXT field 
                    }
                    catch
                    {
                        //_logger.Info($"\tInvalid date format for {line}");
                    }
                }
            }

            return parsedDates;
        }

        public static void WriteRoverPhotoPagesToJson(Dictionary<string, Dictionary<DateTime, List<RoverPhotoPage>>> roverPhotoPage)
        {
            string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "output.json");

            Task.Run(() => File.WriteAllTextAsync(outputFilePath, JsonConvert.SerializeObject(roverPhotoPage)));
        }
    }
}