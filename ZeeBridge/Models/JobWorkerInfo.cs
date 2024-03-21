using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ZeeBridge.Models;

internal class JobWorkerInfo
{
    public MethodInfo Handler { get; init; }
    public ServiceLifetime ServiceLifetime { get; init; }
    public string JobType { get; init; }
    public int? MaxJobsActive { get; init; }
    public string? WorkerName { get; init; }
    public TimeSpan? PollInterval { get; init; }
    public TimeSpan? ExecutionTimeout { get; init; }
}