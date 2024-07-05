using WasteReductionPlatform.Services;

public class NotificationBackgroundService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;

	public NotificationBackgroundService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
				await notificationService.DetectMissedPickupsAsync();
				await notificationService.DetectHighWasteProductionAsync();
			}
			await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Adjust the time interval as needed
		}
	}
}
