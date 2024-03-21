using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;

namespace ZeeBridge.Interfaces;

public interface IJobWorkerHandler
{
    void Handler(IJobClient client, IJob activatedJob, CancellationToken cancellationToken);
}