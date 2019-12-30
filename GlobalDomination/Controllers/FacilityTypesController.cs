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
    public class FacilityTypesController : ControllerBase
    {
        private readonly GDomContext _context;

        public FacilityTypesController(GDomContext context)
        {
            _context = context;
        }

        // GET: api/FacilityTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacilityTypes>>> GetFacilityTypes()
        {
            return await _context.FacilityTypes.ToListAsync();
        }

        // GET: api/FacilityTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FacilityTypes>> GetFacilityTypes(Guid id)
        {
            var facilityTypes = await _context.FacilityTypes.FindAsync(id);

            if (facilityTypes == null)
            {
                return NotFound();
            }

            return facilityTypes;
        }

        // PUT: api/FacilityTypes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFacilityTypes(Guid id, FacilityTypes facilityTypes)
        {
            if (id != facilityTypes.FacilityTypeId)
            {
                return BadRequest();
            }

            _context.Entry(facilityTypes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacilityTypesExists(id))
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

        // POST: api/FacilityTypes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<FacilityTypes>> PostFacilityTypes(FacilityTypes facilityTypes)
        {
            _context.FacilityTypes.Add(facilityTypes);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FacilityTypesExists(facilityTypes.FacilityTypeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFacilityTypes", new { id = facilityTypes.FacilityTypeId }, facilityTypes);
        }

        // DELETE: api/FacilityTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FacilityTypes>> DeleteFacilityTypes(Guid id)
        {
            var facilityTypes = await _context.FacilityTypes.FindAsync(id);
            if (facilityTypes == null)
            {
                return NotFound();
            }

            _context.FacilityTypes.Remove(facilityTypes);
            await _context.SaveChangesAsync();

            return facilityTypes;
        }

        private bool FacilityTypesExists(Guid id)
        {
            return _context.FacilityTypes.Any(e => e.FacilityTypeId == id);
        }
    }
}
