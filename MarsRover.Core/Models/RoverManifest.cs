using Newtonsoft.Json;
using System;

namespace MarsRover.Core.Models
{
    public class RoverManifest
    {
      [JsonProperty("photo_manifest")]
      public PhotoManifest PhotoManifest { get; set; }
    }

    public class PhotoManifest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("launch_date")]
        public DateTime LandingDate { get; set; }

        [JsonProperty("max_date")]
        public DateTime RecentPhotoDate { get; set; }
    }
}
