using Zeebe.Client.Api.Responses;
using ZeeBridge.Attributes;
using ZeeBridge.Client.Models;
using ZeeBridge.Extenstion;
using ZeeBridge.Interfaces;

namespace ZeeBridge.Client.Workers;

[JobType("save-aa-task")]
public class SaveAaTaskWorker : IJobWorkerHandlerAsync
{
    public Task HandlerAsync(IJob activatedJob, CancellationToken cancellationToken)
    {
        Console.WriteLine("Save AA task worker activated");
        var data = new WeatherRequest
        {
            City = "Ho Chi Minh"
        };
        
        activatedJob.MarkAsCompleted(data);
        return Task.CompletedTask;
    }
}