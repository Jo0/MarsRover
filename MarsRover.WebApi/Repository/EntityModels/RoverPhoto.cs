using System;
using System.Collections.Generic;

namespace MarsRover.WebApi.Repository.EntityModels
{
    public partial class RoverPhoto
    {
        public long RoverPhotoId { get; set; }
        public long? RoverId { get; set; }
        public string RoverCameraInfo { get; set; }
        public string PhotoUrl { get; set; }
        public long? PhotoDateId { get; set; }

        public virtual PhotoDate PhotoDate { get; set; }
        public virtual Rover Rover { get; set; }
    }
}
