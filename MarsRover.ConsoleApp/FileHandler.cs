using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarsRover.ConsoleApp
{
    public class FileHandler
    {
        public FileHandler()
        {

        }

        public List<DateTime> ReadDatesFromFile(Stream stream)
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

                        Console.WriteLine($"Parse {line} to {parsedDate}");

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
    }
}
