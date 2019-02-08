using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarsRover.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MarsRover.WebApi.Repository.EntityModels;

namespace MarsRover.WebApi.Controllers
{
    [Route("api/v1/rover")]
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
            //Get latest rover manifests            
            return _context.Rover.ToList();
        }

        [HttpGet("{roverId}/photos")]
        public ActionResult<IEnumerable<RoverPhoto>> GetPhotosForRover(int roverId)
        {
            return _context.RoverPhoto.Where(x => x.RoverId == roverId).ToList();
        }
    }
}