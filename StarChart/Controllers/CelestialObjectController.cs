using System.Linq;
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
            var result = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (result == null)
                return NotFound();
            else
            {
                result.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
                _context.SaveChanges();
                return Ok(result);
            }
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var result = _context.CelestialObjects.Where(x => x.Name == name);
            if (!result.Any())
                return NotFound();
            else
            {
                foreach(var obj in result)
                    obj.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == obj.Id).ToList();
                _context.SaveChanges();
                return Ok(result);
            }
        }

        [HttpGet(Name = "GetAll")]
        public IActionResult GetAll()
        {
            foreach (var obj in _context.CelestialObjects)
            {
                obj.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == obj.Id).ToList();
            }
            _context.SaveChanges();
            return Ok(_context.CelestialObjects.ToList());
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (obj == null)
                return NotFound();
            obj.Name = celestialObject.Name;
            obj.OrbitalPeriod = celestialObject.OrbitalPeriod;
            obj.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var obj = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (obj == null)
                return NotFound();
            obj.Name = name;
            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _context.CelestialObjects.Where(x => x.Id == id);
            if (!result.Any())
                return NotFound();
            else
            {
                _context.CelestialObjects.RemoveRange(result);
                _context.SaveChanges();
                return NoContent();
            }
        }
    }
}
