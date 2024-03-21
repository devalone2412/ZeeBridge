using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;
using ZeeBridge.Attributes;
using ZeeBridge.Interfaces;

namespace ZeeBridge.Client.Workers;

[JobType("get-weather-api-information")]
[MaxJobActive(1)]
[WorkerName("Laptop Acer Nitro 5")]
[PollInterval(5)]
[ExecutionTimeout(10)]
[ServiceLifetime(ServiceLifetime.Transient)]
public class GetApiInformationWorker : IJobWorkerHandlerAsync
{
    public Task HandlerAsync(IJobClient client, IJob activatedJob, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Console.WriteLine($"Job {nameof(GetApiInformationWorker)} completed!");
        return Task.CompletedTask;
    }
}