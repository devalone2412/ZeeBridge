using System.Reflection;
using System.Text.Json;
using Zeebe.Client;
using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;
using ZeeBridge.Configs;
using ZeeBridge.Extenstion;
using ZeeBridge.Interfaces;
using ZeeBridge.Models;

namespace ZeeBridge;

public class ZeeBridgeClient : IZeeBridgeClient
{
    private readonly IZeebeClient _zeebeClient;
    private readonly ZeebeClientConfigOption _zeebeClientConfigOption;
    private IServiceProvider _serviceProvider;

    internal ZeeBridgeClient(IZeebeClient zeebeClient, ZeebeClientConfigOption zeebeClientConfigOption,
        IServiceProvider serviceProvider)
    {
        _zeebeClient = zeebeClient;
        _zeebeClientConfigOption = zeebeClientConfigOption;
        _serviceProvider = serviceProvider;
    }

    public Task<IDeployResourceResponse> DeployResource(string directoryPath, List<string> resources)
    {
        var deploymentCommand = _zeebeClient.NewDeployCommand()
            .AddResourceFile(Path.Combine(directoryPath, resources.First()));

        resources.RemoveAt(0);
        resources.ForEach(resource =>
            deploymentCommand.AddResourceFile(Path.Combine(directoryPath, resource)));
        return deploymentCommand.Send();
    }

    public Task<IJobWorker> CreateWorker(JobWorkerInfo jobWorkerInfo, CancellationToken cancellationToken)
    {
        var workerConfigs = _zeebeClientConfigOption.WorkerConfig;

        return Task.FromResult(_zeebeClient.NewWorker()
            .JobType(jobWorkerInfo.JobType)
            .Handler((client, job) => Handler(client, job, jobWorkerInfo.Handler, cancellationToken))
            .MaxJobsActive(jobWorkerInfo.MaxJobsActive ?? workerConfigs.MaxJobsActive)
            .Name(jobWorkerInfo.WorkerName ?? Environment.MachineName)
            .AutoCompletion()
            .PollingTimeout(jobWorkerInfo.PollInterval ?? TimeSpan.FromSeconds(workerConfigs.PollInterval))
            .Timeout(jobWorkerInfo.ExecutionTimeout ?? TimeSpan.FromSeconds(workerConfigs.ExecutionTimeout))
            .Open());
    }

    private Task Handler(IJobClient client, IJob job, MethodInfo handler, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var handlerInstance = _serviceProvider.GetService(handler.ReflectedType);
        if (handlerInstance is null)
        {
            throw new InvalidOperationException($"Ther is no service registered for {handler.ReflectedType}");
        }

        handler.Invoke(handlerInstance, new object[] { client, job, cancellationToken });
        return Task.CompletedTask;
    }

    public void StartMessageEvent(string messageName, object? data = null)
    {
        throw new NotImplementedException();
    }

    public Task StartEvent(string processId, object? data = null)
    {
        var startEventCommand = _zeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId(processId)
            .LatestVersion();

        if (data is not null)
        {
            startEventCommand
                .Variables(data.ToJson());
        }

        return startEventCommand.Send();
    }

    public Task StartEvent(string processId, int version, object? data = null)
    {
        var startEventCommand = _zeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId(processId)
            .Version(version);

        if (data is not null)
        {
            startEventCommand
                .Variables(data.ToJson());
        }

        return startEventCommand.Send();
    }

    public async Task<T?> StartEventWithResult<T>(string processId, object? data = null)
    {
        var startEventCommand = _zeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId(processId)
            .LatestVersion();

        if (data is not null)
        {
            startEventCommand
                .Variables(data.ToJson());
        }

        var result = await startEventCommand
            .WithResult()
            .Send();

        return JsonSerializer.Deserialize<T>(result.Variables);
    }

    public async Task<T?> StartEventWithResult<T>(string processId, int version, object? data = null)
    {
        var startEventCommand = _zeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId(processId)
            .Version(version);

        if (data is not null)
        {
            startEventCommand
                .Variables(data.ToJson());
        }

        var result = await startEventCommand
            .WithResult()
            .Send();

        return JsonSerializer.Deserialize<T>(result.Variables);
    }
}