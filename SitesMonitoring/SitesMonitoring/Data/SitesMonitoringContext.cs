
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace SitesMonitoring.Models
{    
    public class SitesMonitoringContext : IdentityDbContext<ApplicationUser>
    {
        public SitesMonitoringContext(DbContextOptions<SitesMonitoringContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<SitesMonitoring.Models.Site> Site { get; set; }

        public DbSet<SitesMonitoring.Models.Settings> Settings { get; set; }
    }
}
