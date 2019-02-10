using MarsRover.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MarsRover.Core.WebApi
{
    public class NasaClient
    {
        private static HttpClient _client = new HttpClient();
        private string _apiKey;

        public string[] rovers = { "curiosity", "opportunity", "spirit" };

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

            foreach (var rover in rovers)
            {
                RoverManifest manifest = await GetRoverManifest(rover);

                if (manifest != null)
                {
                    roverManifests.Add(rover, manifest);
                }
            }

            return roverManifests;
        }

         public async Task<RoverPhotoPage> GetRoverPhotos(string rover, DateTime date)
        {
            RoverPhotoPage photoPage = null;

            var response = await _client.GetAsync($"https://api.nasa.gov/mars-photos/api/v1/rovers/{rover}/photos?earth_date={date.ToString("yyyy-M-d")}&api_key={_apiKey}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                photoPage = JsonConvert.DeserializeObject<RoverPhotoPage>(responseContent);
            }

            return photoPage; //TODO: handle when request is not successful in a better way
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
    }
}
