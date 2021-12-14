using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name="GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.SingleOrDefault(co => co.Id == id);
            if (celestialObject == null)
                return NotFound();            
            celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(co => co.Name == name).ToList();
            if (celestialObjects == null || celestialObjects.Count == 0)
                return NotFound();

            celestialObjects.ForEach(celestialObject =>
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
            });            

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            celestialObjects.ForEach(celestialObject =>
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();
            });            

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", routeValues: new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celestialObjectFromDb = _context.CelestialObjects.SingleOrDefault(co => co.Id == id);
            if (celestialObjectFromDb == null)
                return NotFound();
            celestialObjectFromDb.Name = celestialObject.Name;
            celestialObjectFromDb.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialObjectFromDb.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(celestialObjectFromDb);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObjectFromDb = _context.CelestialObjects.SingleOrDefault(co => co.Id == id);
            if (celestialObjectFromDb == null)
                return NotFound();
            celestialObjectFromDb.Name = name;            

            _context.CelestialObjects.Update(celestialObjectFromDb);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id).ToList();
            if (celestialObjects == null || celestialObjects.Count == 0)
                return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
