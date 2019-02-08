using System;
using System.Collections.Generic;

namespace MarsRover.WebApi.Repository.EntityModels
{
    public partial class PhotoDate
    {
        public PhotoDate()
        {
            RoverPhoto = new HashSet<RoverPhoto>();
        }

        public long PhotoDateId { get; set; }
        public string EarthDate { get; set; }

        public virtual ICollection<RoverPhoto> RoverPhoto { get; set; }
    }
}
