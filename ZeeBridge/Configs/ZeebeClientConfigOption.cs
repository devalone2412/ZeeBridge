namespace ZeeBridge.Configs;

public class ZeebeClientConfigOption
{
    public static string SectionName = "ZeebeClientConfig";

    public required ClientConfig ClientConfig { get; init; }
    public WorkerConfig? WorkerConfig { get; init; }
}

public class WorkerConfig
{
    public int MaxJobsActive { get; init; } = 1;
    public int PollInterval { get; init; } = 1;
    public int ExecutionTimeout { get; init; } = 10;
}

public class ClientConfig
{
    public required string GatewayAddress { get; init; }
    public string? ZeebeClientId { get; init; }
    public string? ZeebeClientSecret { get; init; }
}