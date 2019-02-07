using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarsRover.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MarsRover.WebApi.Controllers
{
    [Route("api/v1/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        //Get Date Input file
        [HttpPost]
        public async Task<IActionResult> UploadDateInputFile(IFormFile file)
        {
            //TODO: validate file
            if(file.Length > 0)
            {
                using(MemoryStream stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    List<DateTime> dates = FileHandler.ReadDatesFromFile(stream);

                    return Ok(dates);
                }
            }   

            return Ok();
        }
    }
}
