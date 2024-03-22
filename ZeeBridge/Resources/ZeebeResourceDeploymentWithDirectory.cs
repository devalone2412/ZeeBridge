using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZeeBridge.Exceptions;
using ZeeBridge.Interfaces;

namespace ZeeBridge.Resources;

public class ZeebeResourceDeploymentWithDirectory : ZeebeResourceDeployment
{
    private readonly string _directoryPath;
    private readonly ILogger<ZeebeResourceDeploymentWithDirectory> _logger;
    private readonly List<string> _resources = new();
    private readonly IServiceProvider _serviceProvider;

    public ZeebeResourceDeploymentWithDirectory(IServiceProvider serviceProvider, string directoryPath) : base(
        serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _directoryPath = directoryPath;
        _logger = serviceProvider.GetRequiredService<ILogger<ZeebeResourceDeploymentWithDirectory>>();
    }

    public ZeebeResourceDeploymentWithDirectory AddResource(string resource)
    {
        if (string.IsNullOrEmpty(resource)) throw new ArgumentException("Resource must not be null or empty.");

        _resources.Add(resource);
        return this;
    }

    public async void Deploy()
    {
        if (_resources.Count == 0)
            throw new ArgumentException("Illegal call to Deploy() - please add resources first.");

        using var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var zeeBridgeClient = serviceScope.ServiceProvider.GetService<IZeeBridgeClient>();

        try
        {
            var deploymentResponse = await zeeBridgeClient!.DeployResource(_directoryPath, _resources);
            deploymentResponse.Processes.ToList()
                .ForEach(p => _logger.LogInformation("Deployed process {key}", p.BpmnProcessId));
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            throw new InvalidBpmnException(
                $"Invalid BPMN file detected. Please check your BPMN file for errors. - Error detail: {ex.Message}");
        }
    }
}