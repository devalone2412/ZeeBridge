using Zeebe.Client.Api.Responses;

namespace ZeeBridge.Interfaces;

public interface IJobWorkerHandlerAsync
{
    Task HandlerAsync(IJob activatedJob, CancellationToken cancellationToken);
}