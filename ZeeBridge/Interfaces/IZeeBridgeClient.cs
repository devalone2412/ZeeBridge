using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;
using ZeeBridge.Models;

namespace ZeeBridge.Interfaces;

public interface IZeeBridgeClient
{
    Task<IDeployResourceResponse> DeployResource(string directoryPath, List<string> resources);
    internal Task<IJobWorker> CreateWorker(JobWorkerInfo jobWorkerInfo, CancellationToken cancellationToken);
    void StartMessageEvent(string messageName, object? data = null);
    Task StartEvent(string processId, object? data = null);
    Task StartEvent(string processId, int version, object? data = null);
    Task<T?> StartEventWithResult<T>(string processId, object? data = null);
    Task<T?> StartEventWithResult<T>(string processId, int version, object? data = null);
}