using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using WasteReductionPlatform.Data;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.Services
{
    public class NotificationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //public async Task DetectHighWasteProductionAsync()
        
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        //        var users = await userManager.Users.ToListAsync();

        //        foreach (var user in users)
        //        {
        //            var totalWaste = await context.WasteLogs
        //                .Where(w => w.UserId == user.Id && w.Date > DateTime.Today.AddDays(-30))
        //                .SumAsync(w => w.Weight);

        //            if (totalWaste > 100) // Example threshold for high waste production
        //            {
        //                var alert = new Notification
        //                {
        //                    UserId = user.Id,
        //                    Date = DateTime.Now,
        //                    Message = "Your waste production has exceeded 100kg in the past 30 days.",
        //                    IsRead = false,
        //                    Type = NotificationType.HighWasteProduction
        //                };

        //                context.Notifications.Add(alert);
        //                await context.SaveChangesAsync();
        //            }
        //        }
        //    }
        //}
        public async Task DetectHighWasteProductionAsync(string userId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var totalWaste = await context.WasteLogs
                    .Where(w => w.UserId == userId && w.Date > DateTime.Today.AddDays(-30))
                    .SumAsync(w => w.Weight);

                if (totalWaste > 100) // Example threshold for high waste production
                {
                    var alert = new Notification
                    {
                        UserId = userId,
                        Date = DateTime.Now,
                        Message = "Alert: Your waste production has exceeded 100kg in the past 30 days. Please consider reducing your waste to contribute to a cleaner environment.",
                        IsRead = false,
                        Type = NotificationType.HighWasteProduction
                    };

                    context.Notifications.Add(alert);
                    await context.SaveChangesAsync();
                }
            }
        }


        public async Task DetectMissedPickupsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var missedPickups = await context.UserPickupRequests
                    .Include(r => r.PickupSchedule)
                    .Where(r => r.PickupSchedule.PickupDate < DateTime.Now && !r.PickupSchedule.IsConfirmed)
                    .ToListAsync();

                foreach (var request in missedPickups)
                {
                    var notification = new Notification
                    {
                        UserId = request.UserId,
                        ScheduledPickupDate = request.PickupSchedule.PickupDate,
                        Date = DateTime.Now,
                        Message = $"Your scheduled pickup for {request.PickupSchedule.PickupDate.ToShortDateString()} of type {request.PickupSchedule.PickupType} in area {request.PickupSchedule.Area} was missed.",
                        IsRead = false,
                        Type = NotificationType.MissedPickup
                    };

                    context.Notifications.Add(notification);
                    context.PickupSchedules.Remove(request.PickupSchedule);
                }

                await context.SaveChangesAsync();
            }
        }

    }
}
