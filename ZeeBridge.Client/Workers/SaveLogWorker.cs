using Zeebe.Client.Api.Responses;
using ZeeBridge.Attributes;
using ZeeBridge.Client.Models;
using ZeeBridge.Extenstion;
using ZeeBridge.Interfaces;

namespace ZeeBridge.Client.Workers;

[JobType("save-log")]
public class SaveLogWorker : IJobWorkerHandlerAsync
{
    public async Task HandlerAsync(IJob activatedJob, CancellationToken cancellationToken)
    {
        Console.WriteLine("Save log worker activated");
        var data = new WeatherRequest
        {
            City = "Ho Chi Minh"
        };
        
        await activatedJob.MarkAsCompletedAsync(data);
    }
}