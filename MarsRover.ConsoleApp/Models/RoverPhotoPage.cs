using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarsRover.ConsoleApp.Models
{
    public class RoverPhotoPage
    {
        [JsonProperty("photos")]
        public List<RoverPhoto> Photos { get; set; }
    }

    public class RoverPhoto
    {
        [JsonProperty("camera")]
        public RoverCameraInfo Camera { get; set; }

        [JsonProperty("img_src")]
        public string ImageUrl { get; set; }

        [JsonProperty("earth_date")]
        public DateTime EarthDate { get; set; }
    }

    public class RoverCameraInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rover_id")]
        public int RoverId { get; set; }

        [JsonProperty("full_name")]
        public string FullCameraName { get; set; }
    }
}
