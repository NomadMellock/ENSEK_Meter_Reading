using ENSEK_Meter_Reading.Models;
using Microsoft.EntityFrameworkCore;

namespace ENSEK_Meter_Reading.Data
{
    /// <summary>
    /// Database Context
    /// </summary>
    public class MeterReadingContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        /// <summary>
        /// MeterReading DB Context
        /// </summary>
        /// <param name="configuration"></param>
        public MeterReadingContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        /// <summary>
        /// Override OnConfiguring for Default Connection Settings
        /// </summary>
        /// <param name="options"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("Default"));
        }

        /// <summary>
        /// FileDetails DbSet
        /// </summary>
        public DbSet<FileDetails> FileDetails { get; set; }
    }
}
