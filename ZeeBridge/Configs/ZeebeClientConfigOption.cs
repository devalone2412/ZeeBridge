namespace ZeeBridge.Configs;

internal class ZeebeClientConfigOption
{
    public static string SectionName = "ZeebeClientConfig";

    public ClientConfig ClientConfig { get; init; }
    public WorkerConfig? WorkerConfig { get; init; }
}

internal class WorkerConfig
{
    public int? MaxJobsActive { get; init; }
    public int? PollInterval { get; init; }
    public int? ExecutionTimeout { get; init; }
}

internal class ClientConfig
{
    public string GatewayAddress { get; init; }
    public string? ZeebeClientId { get; init; }
    public string? ZeebeClientSecret { get; init; }
}