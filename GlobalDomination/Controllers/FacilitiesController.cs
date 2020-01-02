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
        private readonly GDomContext _readContext;
        private readonly GDomContext _writeContext;
        private readonly IAsyncPolicy<Facilities> _itemCachePolicy;
        private readonly IAsyncPolicy<List<Facilities>> _collectionCachePolicy;
        private readonly IDistributedCache _cache;
        private const string _facilityKey = "FacilityCacheKey";

        public FacilitiesController((GDomContext readContext, GDomContext writeContext) context, IDistributedCache cache, IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            _readContext = context.readContext;
            _writeContext = context.writeContext;
            _cache = cache;
            _itemCachePolicy = policyRegistry.Get<IAsyncPolicy<Facilities>>(CachePolicyHelper.FacilitiesEntityCachePolicyName);
            _collectionCachePolicy = policyRegistry.Get<IAsyncPolicy<List<Facilities>>>(CachePolicyHelper.FacilitiesEntityCollectionCachePolicyName);
        }

        // GET: api/Facilities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Facilities>>> GetFacilities()
        {
            //var facilities = await _collectionCachePolicy.ExecuteAsync(context => _readContext.Facilities.ToListAsync<Facilities>(), new Context(_facilityKey));
            var facilities = await _collectionCachePolicy.ExecuteAsync(context => _readContext.Facilities.Where(f => true).AsNoTracking().ToListAsync<Facilities>(), new Context(_facilityKey));

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
            //var facility = await _itemCachePolicy.ExecuteAsync(context => _readContext.Facilities.FindAsync(id).AsTask(), new Context(_facilityKey + id));
            var facility = await _itemCachePolicy.ExecuteAsync(context => _readContext.Facilities.Where(f => f.FacilityId == id).AsNoTracking().FirstAsync(), new Context(_facilityKey + id));

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
            var facilities = await _collectionCachePolicy.ExecuteAsync(context => _readContext.Facilities.Where(f => f.PostalCode.ToLower() == zip.ToLower()).AsNoTracking().ToListAsync(), new Context(_facilityKey + zip.ToLower()));

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
            var facilities = await _collectionCachePolicy.ExecuteAsync(context => _readContext.Facilities.Where(f => f.State.ToLower() == state.ToLower()).AsNoTracking().ToListAsync(), new Context(_facilityKey + state.ToLower()));

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
            await _cache.RemoveAsync(_facilityKey + facilities.PostalCode.ToLower());
            await _cache.RemoveAsync(_facilityKey + facilities.State.ToLower());
            await _cache.RemoveAsync(_facilityKey + id);

            _writeContext.Entry(facilities).State = EntityState.Modified;

            try
            {
                await _writeContext.SaveChangesAsync();
                _writeContext.Entry(facilities).State = EntityState.Detached;
                _readContext.Entry(facilities).State = EntityState.Detached;
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
            _writeContext.Facilities.Add(facilities);
            await _writeContext.SaveChangesAsync();

            return CreatedAtAction("GetFacilities", new { id = facilities.FacilityId }, facilities);
        }

        // DELETE: api/Facilities/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Facilities>> DeleteFacilities(Guid id)
        {
            var facilities = await _writeContext.Facilities.FindAsync(id);
            if (facilities == null)
            {
                return NotFound();
            }
            await _cache.RemoveAsync(_facilityKey);
            await _cache.RemoveAsync(_facilityKey + facilities.PostalCode.ToLower());
            await _cache.RemoveAsync(_facilityKey + facilities.State.ToLower());
            await _cache.RemoveAsync(_facilityKey + id);

            _writeContext.Facilities.Remove(facilities);
            await _writeContext.SaveChangesAsync();

            return facilities;
        }

        private bool FacilitiesExists(Guid id)
        {
            return _readContext.Facilities.Any(e => e.FacilityId == id);
        }
    }
}
