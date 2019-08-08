using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name ="GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            var result = celestialObjects.FirstOrDefault(x => x.Id == id);
            if (result == null)
                return NotFound();
            else
            {
                result.Satellites = celestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
                _context.SaveChanges();
                return Ok(result);
            }
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            var result = celestialObjects.Where(x => x.Name == name);
            if (!result.Any())
                return NotFound();
            else
            {
                foreach(var obj in result)
                    obj.Satellites = celestialObjects.Where(x => x.OrbitedObjectId == obj.Id).ToList();
                _context.SaveChanges();
                return Ok(result);
            }
        }

        [HttpGet(Name = "GetAll")]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var obj in celestialObjects)
            {
                obj.Satellites = celestialObjects.Where(x => x.OrbitedObjectId == obj.Id).ToList();
            }
            _context.SaveChanges();
            return Ok(celestialObjects);
        }
    }
}
