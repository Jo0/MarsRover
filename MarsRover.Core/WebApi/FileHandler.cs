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
        public static Dictionary<string, List<string>> ReadDatesFromFile(Stream stream)
        {
            Dictionary<string, List<string>> dates = new Dictionary<string, List<string>>();
            dates.Add("Read", new List<string>());
            dates.Add("Parsed", new List<string>());
            dates.Add("Invalid", new List<string>());

            using (StreamReader reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    dates["Read"].Add(line);

                    try
                    {
                        DateTime parsedDate = DateTime.Parse(line);

                        //_logger.Info($"Parse {line} to {parsedDate}");

                        dates["Parsed"].Add(parsedDate.ToString("o")); //SQLite expects ISO8601 as date string format when storing as TEXT field 
                    }
                    catch
                    {
                        //_logger.Info($"\tInvalid date format for {line}");
                        dates["Invalid"].Add(line);
                    }
                }
            }

            return dates;
        }

        public static void WriteRoverPhotoPagesToJson(Dictionary<string, Dictionary<DateTime, List<RoverPhotoPage>>> roverPhotoPage)
        {
            string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "output.json");

            Task.Run(() => File.WriteAllTextAsync(outputFilePath, JsonConvert.SerializeObject(roverPhotoPage)));
        }
    }
}