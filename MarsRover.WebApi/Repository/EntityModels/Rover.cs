using System;
using System.Collections.Generic;

namespace MarsRover.WebApi.Repository.EntityModels
{
    public partial class Rover
    {
        public Rover()
        {
            RoverPhoto = new HashSet<RoverPhoto>();
        }

        public long RoverId { get; set; }
        public string Name { get; set; }
        public string LandingDate { get; set; }
        public string RecentPhotoDate { get; set; }
        public string LastUpdated { get; set; }

        public virtual ICollection<RoverPhoto> RoverPhoto { get; set; }
    }
}
