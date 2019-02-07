using MarsRover.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MarsRover.Core
{
    public class NasaClient
    {
        private static HttpClient _client = new HttpClient();
        private string _apiKey;

        private string[] _rovers = { "curiosity", "opportunity", "spirit" };

        public NasaClient(string apiKey)
        {
            _apiKey = apiKey;
        }
        public async Task<RoverManifest> GetRoverManifest(string rover)
        {
            RoverManifest manifest = null;

            var response = await _client.GetAsync($"https://api.nasa.gov/mars-photos/api/v1/manifests/{rover}?api_key={_apiKey}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                manifest = JsonConvert.DeserializeObject<RoverManifest>(responseContent);
            }

            return manifest; //TODO: handle when request is not successful in a better way
        }

        public async Task<Dictionary<string, RoverManifest>> GetAllRoverManifests()
        {
            Dictionary<string, RoverManifest> roverManifests = new Dictionary<string, RoverManifest>();

            foreach (var rover in _rovers)
            {
                RoverManifest manifest = await GetRoverManifest(rover);

                if (manifest != null)
                {
                    roverManifests.Add(rover, manifest);
                }
            }

            return roverManifests;
        }

        public async Task<RoverPhotoPage> GetRoverPhotoPage(string rover, DateTime date, int page)
        {
            RoverPhotoPage photoPage = null;

            var response = await _client.GetAsync($"https://api.nasa.gov/mars-photos/api/v1/rovers/{rover}/photos?earth_date={date.ToString("yyyy-M-d")}page={page}&api_key={_apiKey}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                photoPage = JsonConvert.DeserializeObject<RoverPhotoPage>(responseContent);
            }

            return photoPage; //TODO: handle when request is not successful in a better way
        }

        public void DownloadImagesForRovers(Dictionary<string, Dictionary<DateTime, List<RoverPhotoPage>>> roverPhotoPages, string destinationPath)
        {
            foreach (var rover in roverPhotoPages.Keys)
            {
                Console.WriteLine($"\tFor {rover}");
                foreach (var date in roverPhotoPages[rover].Keys)
                {
                    Console.WriteLine($"\t\t for date {date.ToShortDateString()}");
                    foreach (var photoPage in roverPhotoPages[rover][date])
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (i > photoPage.Photos.Count - 1)
                            {
                                break;
                            }

                            RoverPhoto photo = photoPage.Photos[i];
                            string destinationDirectory = Path.Combine(destinationPath, "Photos", rover, date.ToString("MM-dd-yyyy"));

                            if (!Directory.Exists(destinationDirectory))
                            {
                                Directory.CreateDirectory(destinationDirectory);
                            }

                            var success = DownloadFile(photo.ImageUrl, destinationDirectory);

                            if (success)
                            {
                                Console.WriteLine($"\t\t\tSuccessfully downloaded {photo.ImageUrl}");
                            }
                            else
                            {
                                Console.WriteLine($"\t\tFailed to download {photo.ImageUrl}");
                            }
                        }
                    }
                }
            }
        }

        public bool DownloadFile(string sourceUrl, string destinationPath)
        {
            string fileName = sourceUrl.Substring(sourceUrl.LastIndexOf('/') + 1); //create file name from source(url) 

            string destinationFilePath = Path.Combine(destinationPath, fileName); //create path with temp tir and filename

            DownloadFileAsync(sourceUrl, destinationFilePath).Wait();

            if (File.Exists(destinationFilePath))
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        private async Task DownloadFileAsync(string source, string destinationPath)
        {

            //source being source image url
            //destination was path we made from image name from url and temp dir
            using (var request = new HttpRequestMessage(HttpMethod.Get, source))
            using (Stream contentStream = await
                (await _client.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await contentStream.CopyToAsync(stream);
            }
        }
    }
}
