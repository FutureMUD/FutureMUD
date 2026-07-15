#nullable enable

namespace FutureMUD.Web.Publishing;

public sealed class ReleaseCleanupService : BackgroundService
{
	private readonly ReleaseStore _store;
	private readonly ILogger<ReleaseCleanupService> _logger;

	public ReleaseCleanupService(ReleaseStore store, ILogger<ReleaseCleanupService> logger)
	{
		_store = store;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
		do
		{
			try
			{
				_store.Cleanup(DateTimeOffset.UtcNow);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Release storage cleanup failed.");
			}
		}
		while (await timer.WaitForNextTickAsync(stoppingToken));
	}
}
