

using Microsoft.EntityFrameworkCore;
using TruckAPI.Models;

namespace TruckAPI.Data
{
    public class TruckDbContext : DbContext
    {
        public TruckDbContext(DbContextOptions options) : base(options)
        {
        }
         
        public DbSet<TruckDetails> truckDetails { get; set; }
    }
}
