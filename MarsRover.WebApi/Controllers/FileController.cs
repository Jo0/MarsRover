using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarsRover.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MarsRover.WebApi.Repository.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace MarsRover.WebApi.Controllers
{
    [Route("api/v1/dates")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly MarsRoverContext _context;

        public FileController(MarsRoverContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PhotoDate>> GetDates()
        {
            return _context.PhotoDate.ToList();
        }

        [HttpPost]
        public IActionResult UploadDateInputFile(IFormFile file)
        {
            try
            {
                int insertedRecords = 0;
                List<PhotoDate> addDates = new List<PhotoDate>();
                
                Dictionary<string, List<string>> resultSet = new Dictionary<string, List<string>>();
                resultSet.Add("Read", new List<string>());
                resultSet.Add("Invalid", new List<string>());
                resultSet.Add("Exists", new List<string>());
                resultSet.Add("Added", new List<string>());

                //TODO: validate file
                if (file.Length > 0)
                {
                    Dictionary<string, List<string>> dates = FileHandler.ReadDatesFromFile(file.OpenReadStream());

                    resultSet["Read"] = dates["Read"];
                    resultSet["Invalid"] = dates["Invalid"];

                    foreach (var date in dates["Parsed"])
                    {
                        if (!_context.PhotoDate.AsNoTracking().Where(x => x.EarthDate == date).Any())
                        {   
                            resultSet["Added"].Add(date);
                            addDates.Add(new PhotoDate
                            {
                                EarthDate = date
                            });
                        }
                        else
                        {
                            resultSet["Exists"].Add(date);
                        }
                    }

                    if (addDates.Count() > 0)
                    {
                        _context.PhotoDate.AddRange(addDates);
                        insertedRecords = _context.SaveChanges();
                    }
                }

                return Ok(resultSet);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}