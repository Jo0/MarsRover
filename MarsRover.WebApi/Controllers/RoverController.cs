using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarsRover.Core.Models;
using MarsRover.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MarsRover.WebApi.Repository.EntityModels;
using Newtonsoft.Json;

namespace MarsRover.WebApi.Controllers
{
    [Route("api/v1/rovers")]
    [ApiController]
    public class RoverController : ControllerBase
    {
        private readonly MarsRoverContext _context;
        private readonly NasaClient _nasaClient;
        private readonly IConfiguration _configuration;

        public RoverController(MarsRoverContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _nasaClient = new NasaClient(_configuration.GetSection("NasaApi:ApiKey").Value);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Rover>> GetAllRoverManifest()
        {
            //TODO: pull out into service?
            foreach (var rover in _nasaClient.rovers)
            {
                var roverEntity = _context.Rover.Where(x => x.Name == rover).SingleOrDefault();

                if (roverEntity != null)
                {
                    DateTime lastUpdated = DateTime.Parse(roverEntity.LastUpdated);
                    DateTime now = DateTime.Now;

                    if (!(lastUpdated > now.AddHours(-24) && lastUpdated <= now)) //Check if rover manifest has been updated in the last 24 hours
                    {
                        RoverManifest roverManifest = Task.Run(() => _nasaClient.GetRoverManifest(rover)).Result;

                        roverEntity.RecentPhotoDate = roverManifest.PhotoManifest.RecentPhotoDate.ToString("o");
                        roverEntity.LastUpdated = now.ToString("o");
                    }
                }
                else
                {
                    RoverManifest roverManifest = Task.Run(() => _nasaClient.GetRoverManifest(rover)).Result;

                    _context.Rover.Add(new Rover
                    {
                        Name = rover,
                        LandingDate = roverManifest.PhotoManifest.LandingDate.ToString("o"),
                        RecentPhotoDate = roverManifest.PhotoManifest.RecentPhotoDate.ToString("o"),
                        LastUpdated = DateTime.Now.ToString("o")
                    });
                }
            }

            _context.SaveChanges();

            return _context.Rover.ToList();
        }

        [HttpGet("{roverId}/photos")]
        public IActionResult GetPhotosForRover(long roverId, [FromQuery]string photoDate)
        {
            var rover = _context.Rover.Find(roverId);
            var photoDateEntity = _context.PhotoDate.AsNoTracking().Where(x => x.EarthDate == photoDate).SingleOrDefault();

            if (rover != null)
            {
                if (photoDateEntity != null)
                {
                    //Look in Db first
                    var roverPhotos = _context.RoverPhoto.AsNoTracking().Where(x => x.RoverId == roverId && x.PhotoDateId == photoDateEntity.PhotoDateId).ToList();

                    if (roverPhotos.Count() > 0)
                    {
                        return Ok(roverPhotos);
                    }
                    else
                    {
                        DateTime date = DateTime.Parse(photoDate);
                        List<Repository.EntityModels.RoverPhoto> addRoverPhotos = new List<Repository.EntityModels.RoverPhoto>();

                        RoverPhotoPage roverPhotoPage = Task.Run(() => _nasaClient.GetRoverPhotos(rover.Name, date)).Result;

                        if (roverPhotoPage.Photos.Count > 0)
                        {
                            foreach (var photo in roverPhotoPage.Photos)
                            {
                                addRoverPhotos.Add(new Repository.EntityModels.RoverPhoto
                                {
                                    RoverId = rover.RoverId,
                                    RoverCameraInfo = JsonConvert.SerializeObject(photo.Camera),
                                    PhotoUrl = photo.ImageUrl,
                                    PhotoDateId = photoDateEntity.PhotoDateId
                                });
                            }

                            _context.RoverPhoto.AddRange(addRoverPhotos);
                            _context.SaveChanges();
                        }

                        return Ok(_context.RoverPhoto.AsNoTracking().Where(x => x.RoverId == roverId && x.PhotoDateId == photoDateEntity.PhotoDateId).ToList());
                    }
                }
                else
                {
                    return NotFound("Date not found, please upload date from file");
                }
            }
            else
            {
                return NotFound("Rover not found");
            }
        }
    }
}