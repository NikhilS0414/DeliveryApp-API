using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TruckAPI.Data;
using TruckAPI.Models;


namespace TruckAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TruckController : Controller
    {
        private readonly TruckDbContext _truckDbContext;
        private readonly IMemoryCache _memoryCache;
        public TruckController(TruckDbContext truckDbContext, IMemoryCache memoryCache)
        {
            _truckDbContext = truckDbContext;
            _memoryCache = memoryCache;
        }
        [HttpGet]
        public async Task<IActionResult> GetTruckDetails()
        {
            var cachekey = "Truck";
            if (!_memoryCache.TryGetValue(cachekey, out var data))      //Checking cache memory 
            {
                data = await _truckDbContext.truckDetails.ToListAsync();

                //setting up cache options
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };

                //setting caching entries
                _memoryCache.Set(cachekey, data);
            }
            return Ok(data);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetTruckDetail([FromRoute] int id)
        {
            var truckDetail = await _truckDbContext.truckDetails.FindAsync(id);
            if (truckDetail == null)
            {
                return NotFound();
            }
            return Ok(truckDetail);

        }

        [HttpPost]
        public async Task<IActionResult> AddTruckDetails(AddTruckRequest addTruckRequest)
        {
            try
            {
                var countval = _truckDbContext.truckDetails.Where(a => a.TruckName == addTruckRequest.TruckName)
                                                       .Where(a => a.Source == addTruckRequest.Source)
                                                       .Where(a => a.Destination == addTruckRequest.Destination)
                                                       .Where(a => a.StartDate == addTruckRequest.StartDate)
                                                       .Where(a => a.EndDate == addTruckRequest.EndDate).Count();
                if (countval == 0)
                {
                    var truckDetails = new TruckDetails()
                    {
                        TruckID = Guid.NewGuid(),
                        TruckName = addTruckRequest.TruckName,
                        Source = addTruckRequest.Source,
                        Destination = addTruckRequest.Destination,
                        StartDate = addTruckRequest.StartDate,
                        EndDate = addTruckRequest.EndDate
                    };
                    await _truckDbContext.truckDetails.AddAsync(truckDetails);
                    await _truckDbContext.SaveChangesAsync();

                    return Ok(truckDetails);
                }
                return Ok("This Truck already scheduled, can you please try with other truck");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTruckDetails([FromRoute] int id, UpdateTruckRequest updateTruckRequest)
        {
            var truckDetails = await _truckDbContext.truckDetails.FindAsync(id);

            if (truckDetails != null)
            {
                truckDetails.TruckName = updateTruckRequest.TruckName;
                truckDetails.Source = updateTruckRequest.Source;
                truckDetails.Destination = updateTruckRequest.Destination;
                truckDetails.StartDate = updateTruckRequest.StartDate;
                truckDetails.EndDate = updateTruckRequest.EndDate;

                await _truckDbContext.SaveChangesAsync();

                return Ok(truckDetails);

            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTruckDetail([FromRoute] int id)
        {
            var truckdetail = await _truckDbContext.truckDetails.FindAsync(id);
            if (truckdetail != null)
            {
                _truckDbContext.truckDetails.Remove(truckdetail);
                await _truckDbContext.SaveChangesAsync();
                return Ok(truckdetail);
            }
            return NotFound();
        }

    }
}
