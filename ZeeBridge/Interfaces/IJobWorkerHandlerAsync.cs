using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;

namespace ZeeBridge.Interfaces;

public interface IJobWorkerHandlerAsync
{
    Task HandlerAsync(IJobClient client, IJob activatedJob, CancellationToken cancellationToken);
}