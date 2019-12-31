using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Context;
using Database.Entity;
using Polly;
using Microsoft.Extensions.Caching.Distributed;
using Polly.Registry;
using GlobalDomination.Utilities;

namespace GlobalDomination.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilitiesController : ControllerBase
    {
        private readonly GDomContext _context;
        private readonly IAsyncPolicy<Facilities> _itemCachePolicy;
        private readonly IAsyncPolicy<List<Facilities>> _collectionCachePolicy;
        private readonly IDistributedCache _cache;
        private const string _facilityKey = "FacilityCacheKey";

        public FacilitiesController(GDomContext context, IDistributedCache cache, IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            _context = context;
            _cache = cache;
            _itemCachePolicy = policyRegistry.Get<IAsyncPolicy<Facilities>>(CachePolicyHelper.FacilitiesEntityCachePolicyName);
            _collectionCachePolicy = policyRegistry.Get<IAsyncPolicy<List<Facilities>>>(CachePolicyHelper.FacilitiesEntityCollectionCachePolicyName);
        }

        // GET: api/Facilities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Facilities>>> GetFacilities()
        {
            var facilities = await _collectionCachePolicy.ExecuteAsync(context => _context.Facilities.ToListAsync<Facilities>(), new Context(_facilityKey));

            if (facilities == null)
            {
                return NotFound();
            }

            return facilities;
        }

        // GET: api/Facilities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Facilities>> GetFacilities(Guid id)
        {
            var facility = await _itemCachePolicy.ExecuteAsync(context => _context.Facilities.FindAsync(id).AsTask(), new Context(_facilityKey + id));

            if (facility == null)
            {
                return NotFound();
            }

            return facility;
        }
        
        // GET: api/Facilities/Zip/43210
        [HttpGet("zip/{zip}")]
        public async Task<ActionResult<IEnumerable<Facilities>>> GetFacilitiesByZip(string zip)
        {
            var facilities = await _collectionCachePolicy.ExecuteAsync(context => _context.Facilities.Where(f => f.PostalCode.ToLower() == zip.ToLower()).ToListAsync(), new Context(_facilityKey + zip));

            if (facilities == null)
            {
                return NotFound();
            }

            return facilities;
        }

        // GET: api/Facilities/State/OH
        [HttpGet("state/{state}")]
        public async Task<ActionResult<IEnumerable<Facilities>>> GetFacilitiesByState(string state)
        {
            var facilities = await _collectionCachePolicy.ExecuteAsync(context => _context.Facilities.Where(f => f.State.ToLower() == state.ToLower()).ToListAsync(), new Context(_facilityKey + state));

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
            await _cache.RemoveAsync(_facilityKey);
            await _cache.RemoveAsync(_facilityKey + facilities.PostalCode);
            await _cache.RemoveAsync(_facilityKey + facilities.State);
            await _cache.RemoveAsync(_facilityKey + id);

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
            await _cache.RemoveAsync(_facilityKey);
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
            await _cache.RemoveAsync(_facilityKey);
            await _cache.RemoveAsync(_facilityKey + facilities.PostalCode);
            await _cache.RemoveAsync(_facilityKey + facilities.State);
            await _cache.RemoveAsync(_facilityKey + id);

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
