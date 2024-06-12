using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PickupSchedule> PickupSchedules { get; set; }
        public DbSet<WasteLog> WasteLogs { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    // Customize the ASP.NET Identity model and override the defaults if needed.
        //    // For example, you can rename the ASP.NET Identity table names and more.
        //    // Add your customizations after calling base.OnModelCreating(builder);

        //    builder.Entity<PickupSchedule>().HasData(
        //        new PickupSchedule
        //        {
        //            Id = 1,
        //            PickupDate = DateTime.Today,
        //            PickupType = WasteType.BlueBox,
        //            UserType = UserType.Residential,
        //            Area = "M5H"
        //        },
        //        new PickupSchedule
        //        {
        //            Id = 2,
        //            PickupDate = DateTime.Today.AddDays(1),
        //            PickupType = WasteType.GreenCart,
        //            UserType = UserType.Commercial,
        //            Area = "M5V"
        //        });

        //    builder.Entity<WasteLog>().HasData(
        //        new WasteLog
        //        {
        //            Id = 1,
        //            UserId = "admin", // Use the actual user ID from Identity
        //            Date = DateTime.Today,
        //            Weight = 15.5,
        //            WasteType = WasteType.Garbage,
        //            UserType = UserType.Residential
        //        });
        //}
    }
}
