using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Context;
using Database.Entity;
using Microsoft.Extensions.Caching.Distributed;
using Polly.Registry;
using Polly;
using GlobalDomination.Utilities;

namespace GlobalDomination.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilityTypesController : ControllerBase
    {
        private readonly GDomContext _readContext;
        private readonly GDomContext _writeContext;
        private readonly IAsyncPolicy<FacilityTypes> _itemCachePolicy;
        private readonly IAsyncPolicy<List<FacilityTypes>> _collectionCachePolicy;
        private readonly IDistributedCache _cache;
        private const string _facilityTypesKey = "FacilityTypesCacheKey";

        public FacilityTypesController((GDomContext readContext, GDomContext writeContext) context, IDistributedCache cache, IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            _readContext = context.readContext;
            _writeContext = context.writeContext;
            _cache = cache;
            _itemCachePolicy = policyRegistry.Get<IAsyncPolicy<FacilityTypes>>(CachePolicyHelper.FacilityTypesEntityCachePolicyName);
            _collectionCachePolicy = policyRegistry.Get<IAsyncPolicy<List<FacilityTypes>>>(CachePolicyHelper.FacilityTypesEntityCollectionCachePolicyName);
        }

        // GET: api/FacilityTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacilityTypes>>> GetFacilityTypes()
        {

            //var facilityTypes = await _collectionCachePolicy.ExecuteAsync(context => _readContext.FacilityTypes.ToListAsync<FacilityTypes>(), new Context(_facilityTypesKey));
            var facilityTypes = await _collectionCachePolicy.ExecuteAsync(context => _readContext.FacilityTypes.Where(f => true).AsNoTracking().ToListAsync<FacilityTypes>(), new Context(_facilityTypesKey));

            if (facilityTypes == null)
            {
                return NotFound();
            }

            return facilityTypes;
        }

        // GET: api/FacilityTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FacilityTypes>> GetFacilityTypes(Guid id)
        {
            //var facilityTypes = await _itemCachePolicy.ExecuteAsync(context => _readContext.FacilityTypes.FindAsync(id).AsTask(), new Context(_facilityTypesKey + id));
            var facilityTypes = await _itemCachePolicy.ExecuteAsync(context => _readContext.FacilityTypes.Where(f => f.FacilityTypeId == id).AsNoTracking().FirstAsync(), new Context(_facilityTypesKey + id));

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
            await _cache.RemoveAsync(_facilityTypesKey);
            await _cache.RemoveAsync(_facilityTypesKey + id);

            _writeContext.Entry(facilityTypes).State = EntityState.Modified;

            try
            {
                await _writeContext.SaveChangesAsync();
                _writeContext.Entry(facilityTypes).State = EntityState.Detached;
                _readContext.Entry(facilityTypes).State = EntityState.Detached;
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
            await _cache.RemoveAsync(_facilityTypesKey);
            _writeContext.FacilityTypes.Add(facilityTypes);
            await _writeContext.SaveChangesAsync();           

            return CreatedAtAction("GetFacilityTypes", new { id = facilityTypes.FacilityTypeId }, facilityTypes);
        }

        // DELETE: api/FacilityTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FacilityTypes>> DeleteFacilityTypes(Guid id)
        {
            var facilityTypes = await _writeContext.FacilityTypes.FindAsync(id);
            if (facilityTypes == null)
            {
                return NotFound();
            }

            await _cache.RemoveAsync(_facilityTypesKey);
            await _cache.RemoveAsync(_facilityTypesKey + id);
            _writeContext.FacilityTypes.Remove(facilityTypes);
            await _writeContext.SaveChangesAsync();

            return facilityTypes;
        }

        private bool FacilityTypesExists(Guid id)
        {
            return _readContext.FacilityTypes.Any(e => e.FacilityTypeId == id);
        }
    }
}
