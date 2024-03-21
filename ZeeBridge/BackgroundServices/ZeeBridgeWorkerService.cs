using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zeebe.Client.Api.Worker;
using ZeeBridge.Interfaces;

namespace ZeeBridge.BackgroundServices;

internal class ZeeBridgeWorkerService : BackgroundService
{
    private readonly IJobWorkerProvider _jobWorkerProvider;
    private readonly List<IJobWorker> _jobWorkers = new();
    private readonly ILogger<ZeeBridgeWorkerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private CancellationTokenSource _cancellationTokenSource;

    public ZeeBridgeWorkerService(IServiceProvider serviceProvider, IJobWorkerProvider jobWorkerProvider,
        ILogger<ZeeBridgeWorkerService> logger)
    {
        _serviceProvider = serviceProvider;
        _jobWorkerProvider = jobWorkerProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var zeeBridgeClient = scope.ServiceProvider.GetRequiredService<IZeeBridgeClient>();

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        foreach (var jobWorkerInfo in _jobWorkerProvider.JobWorkerInfos)
        {
            var jobWorker = await zeeBridgeClient.CreateWorker(jobWorkerInfo, _cancellationTokenSource.Token);
            _logger.LogInformation("Created job worker for type '{0}'", jobWorkerInfo.JobType);

            _jobWorkers.Add(jobWorker);
        }

        _logger.LogInformation("Created {0} job workers", _jobWorkers.Count);
    }
}