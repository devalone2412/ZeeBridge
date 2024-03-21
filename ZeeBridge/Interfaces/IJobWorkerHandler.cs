using Zeebe.Client.Api.Responses;

namespace ZeeBridge.Interfaces;

public interface IJobWorkerHandler
{
    void Handler(IJob activatedJob, CancellationToken cancellationToken);
}