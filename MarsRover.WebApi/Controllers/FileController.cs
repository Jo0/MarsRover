using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarsRover.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MarsRover.WebApi.Repository.EntityModels;

namespace MarsRover.WebApi.Controllers
{
    [Route("api/v1/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly MarsRoverContext _context;
        public FileController(MarsRoverContext context)
        {
            _context = context;
        }

        //Get Date Input file
        [HttpPost]
        public IActionResult UploadDateInputFile(IFormFile file)
        {
            //TODO: validate file
            if (file.Length > 0)
            {
                List<string> dates = FileHandler.ReadDatesFromFile(file.OpenReadStream());

                foreach (var date in dates)
                {
                    if(!_context.PhotoDate.Where(x => x.EarthDate == date).Any())
                    {
                        _context.PhotoDate.Add(new PhotoDate{
                            EarthDate = date
                        });
                    }
                }
            }

            return Ok();
        }
    }
}