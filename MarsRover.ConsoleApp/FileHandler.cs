﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MarsRover.ConsoleApp.Models;
using Newtonsoft.Json;

namespace MarsRover.ConsoleApp
{
    public class FileHandler
    {
        public static List<DateTime> ReadDatesFromFile(Stream stream)
        {
            List<DateTime> parsedDates = new List<DateTime>();

            using (StreamReader reader = new StreamReader(stream))
            {
                while(reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();

                    try
                    {
                        DateTime parsedDate = DateTime.Parse(line);

                        //Console.WriteLine($"Parse {line} to {parsedDate}");

                        parsedDates.Add(parsedDate);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Invalid date format for {line}");
                    }
                }
            }

            return parsedDates;
        }

        public static void WriteRoverPhotoPagesToJson(Dictionary<string, Dictionary<DateTime,List<RoverPhotoPage>>> roverPhotoPage)
        {
            string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "output.json");

            Task.Run(() => File.WriteAllTextAsync(outputFilePath,JsonConvert.SerializeObject(roverPhotoPage)));
        }        
    }
}
