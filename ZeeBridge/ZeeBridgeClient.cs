﻿using System.Reflection;
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
    private readonly IServiceProvider _serviceProvider;

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

    Task<IJobWorker> IZeeBridgeClient.CreateWorker(JobWorkerInfo jobWorkerInfo, CancellationToken cancellationToken)
    {
        var workerConfigs = _zeebeClientConfigOption.WorkerConfig;

        var maxJobActive = jobWorkerInfo.MaxJobsActive ?? workerConfigs?.MaxJobsActive ?? 1;

        var defaultPollInterval = workerConfigs?.PollInterval ?? 1;
        var pollInterval = jobWorkerInfo.PollInterval ?? TimeSpan.FromSeconds(defaultPollInterval);

        var defaultExecutionTimeout = workerConfigs?.ExecutionTimeout ?? 10;
        var executionTimeout = jobWorkerInfo.ExecutionTimeout ?? TimeSpan.FromSeconds(defaultExecutionTimeout);

        return Task.FromResult(_zeebeClient.NewWorker()
            .JobType(jobWorkerInfo.JobType)
            .Handler((client, job) => Handler(client, job, jobWorkerInfo.Handler, cancellationToken))
            .MaxJobsActive(maxJobActive)
            .Name(jobWorkerInfo.WorkerName ?? Environment.MachineName)
            .AutoCompletion()
            .PollingTimeout(pollInterval)
            .Timeout(executionTimeout)
            .Open());
    }

    public void StartMessageEvent(string messageName, object? data = null)
    {
        throw new NotImplementedException();
    }

    public Task StartEvent(string processId, object? data = null, TimeSpan? requestTimeout = null)
    {
        var startEventCommand = _zeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId(processId)
            .LatestVersion();

        if (data is not null)
            startEventCommand
                .Variables(data.ToJson());

        return startEventCommand.Send(requestTimeout);
    }

    public Task StartEvent(string processId, int version, object? data = null, TimeSpan? requestTimeout = null)
    {
        var startEventCommand = _zeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId(processId)
            .Version(version);

        if (data is not null)
            startEventCommand
                .Variables(data.ToJson());

        
        return startEventCommand.Send(requestTimeout);
    }

    public async Task<T?> StartEventWithResult<T>(string processId, object? data = null, TimeSpan? requestTimeout = null)
    {
        var startEventCommand = _zeebeClient.NewCreateProcessInstanceCommand()
            .BpmnProcessId(processId)
            .LatestVersion();

        if (data is not null)
            startEventCommand
                .Variables(data.ToJson());

        var result = await startEventCommand
            .WithResult()
            .Send(requestTimeout);

        return result.Variables.FromJson<T>();
    }

    public async Task<T?> StartEventWithResult<T>(string processId, int version, object? data = null, TimeSpan? requestTimeout = null)
    {
        var startEventCommand = _zeebeClient
            .NewCreateProcessInstanceCommand()
            .BpmnProcessId(processId)
            .Version(version);

        if (data is not null)
            startEventCommand.Variables(data.ToJson());

        var result = await startEventCommand
            .WithResult()
            .Send(requestTimeout);

        return result.Variables.FromJson<T>();
    }

    private async Task Handler(
        IJobClient client,
        IJob job,
        MethodBase handler,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var handlerInstance = _serviceProvider.GetService(handler.ReflectedType);
        if (handlerInstance is null)
            throw new InvalidOperationException($"There is no service registered for {handler.ReflectedType}");

        job.SetJobClient(client);
        
        var result = handler.Invoke(handlerInstance, new object[] { job, cancellationToken });
        if (result is Task task)
        {
            await task;
        }
    }
}