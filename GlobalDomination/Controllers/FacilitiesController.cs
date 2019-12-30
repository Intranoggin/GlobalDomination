using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Context;
using Database.Entity;

namespace GlobalDomination.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilitiesController : ControllerBase
    {
        private readonly GDomContext _context;

        public FacilitiesController(GDomContext context)
        {
            _context = context;
        }

        // GET: api/Facilities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Facilities>>> GetFacilities()
        {
            return await _context.Facilities.ToListAsync();
        }

        // GET: api/Facilities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Facilities>> GetFacilities(Guid id)
        {
            var facilities = await _context.Facilities.FindAsync(id);

            if (facilities == null)
            {
                return NotFound();
            }

            return facilities;
        }

        // PUT: api/Facilities/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFacilities(Guid id, Facilities facilities)
        {
            if (id != facilities.FacilityId)
            {
                return BadRequest();
            }

            _context.Entry(facilities).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacilitiesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Facilities
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Facilities>> PostFacilities(Facilities facilities)
        {
            _context.Facilities.Add(facilities);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFacilities", new { id = facilities.FacilityId }, facilities);
        }

        // DELETE: api/Facilities/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Facilities>> DeleteFacilities(Guid id)
        {
            var facilities = await _context.Facilities.FindAsync(id);
            if (facilities == null)
            {
                return NotFound();
            }

            _context.Facilities.Remove(facilities);
            await _context.SaveChangesAsync();

            return facilities;
        }

        private bool FacilitiesExists(Guid id)
        {
            return _context.Facilities.Any(e => e.FacilityId == id);
        }
    }
}
