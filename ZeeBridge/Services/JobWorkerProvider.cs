using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ZeeBridge.Attributes;
using ZeeBridge.Interfaces;
using ZeeBridge.Models;

namespace ZeeBridge.Services;

public class JobWorkerProvider : IJobWorkerProvider
{
    private readonly Assembly[] _assemblies;
    private List<JobWorkerInfo>? _jobWorkerInfos;

    public JobWorkerProvider(Assembly[] assemblies)
    {
        _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
    }

    public IEnumerable<JobWorkerInfo> JobWorkerInfos
    {
        get
        {
            if (_jobWorkerInfos != null)
            {
                return _jobWorkerInfos;
            }

            _jobWorkerInfos = GetJobWorkerInfos(_assemblies).ToList();
            return _jobWorkerInfos;
        }
    }

    private static IEnumerable<JobWorkerInfo> GetJobWorkerInfos(Assembly[] assemblies)
    {
        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(IsZeebeWorker)
            .SelectMany(CreateJobWorkerInfo);
    }

    private static IEnumerable<JobWorkerInfo> CreateJobWorkerInfo(Type jobHandlerType)
    {
        return GetJobHandlerMethods(jobHandlerType)
            .Select(m => CreateJobWorkerInfo(jobHandlerType, m));
    }

    private static JobWorkerInfo CreateJobWorkerInfo(Type jobHandlerType, MethodInfo methodInfo)
    {
        return new JobWorkerInfo
        {
            Handler = methodInfo,
            ServiceLifetime = GetServiceLifetime(methodInfo),
            JobType = GetJobType(jobHandlerType),
            MaxJobsActive = GetMaxJobsActive(jobHandlerType),
            WorkerName = GetWorkerName(jobHandlerType),
            PollInterval = GetPollInterval(jobHandlerType),
            ExecutionTimeout = GetExecutionTimeout(jobHandlerType),
        };
    }

    private static ServiceLifetime GetServiceLifetime(MethodInfo handlerMethod)
    {
        var handler = handlerMethod.ReflectedType;
        var attr = handler?.GetCustomAttribute<ServiceLifetimeAttribute>();
        return attr?.ServiceLifetime ?? ServiceLifetime.Transient;
    }

    private static TimeSpan? GetExecutionTimeout(Type jobType)
    {
        var attr = jobType.GetCustomAttribute<ExecutionTimeoutAttribute>();
        return attr?.ExecutionTimeout;
    }

    private static TimeSpan? GetPollInterval(Type jobType)
    {
        var attr = jobType.GetCustomAttribute<PollIntervalAttribute>();
        return attr?.PollInterval;
    }

    private static string? GetWorkerName(Type jobType)
    {
        var attr = jobType.GetCustomAttribute<WorkerNameAttribute>();
        return attr?.WorkerName;
    }

    private static int? GetMaxJobsActive(Type jobType)
    {
        var attr = jobType.GetCustomAttribute<MaxJobActiveAttribute>();
        return attr?.MaxJobActive;
    }

    private static string GetJobType(Type jobType)
    {
        var attr = jobType.GetCustomAttribute<JobTypeAttribute>();
        if (attr is null)
        {
            throw new InvalidOperationException($"The job handler {jobType.Name} must have a JobTypeAttribute");
        }

        var name = attr.JobType;
        if (string.IsNullOrEmpty(name))
        {
            throw new InvalidOperationException(
                $"The job handler {jobType.Name} must have a non-empty JobTypeAttribute");
        }

        return name;
    }

    private static IEnumerable<MethodInfo> GetJobHandlerMethods(Type jobHandlerType)
    {
        var jobHandlerMethods = jobHandlerType.GetInterfaces()
            .Where(IsZeebeJobWorkerHandler)
            .SelectMany(i => i.GetMethods()).ToList();

        if (jobHandlerMethods.Count() > 1)
        {
            throw new InvalidOperationException(
                $"{jobHandlerType.Name} must not have more than one 'HandleJob' method");
        }

        return jobHandlerType.GetMethods()
            .Where(m => IsValidJobHandlerMethod(m, jobHandlerMethods));
    }

    private static bool IsValidJobHandlerMethod(MethodInfo methodInfo, IEnumerable<MethodInfo> jobHandlerMethods)
    {
        var methodParameters = methodInfo.GetParameters()
            .Select(p => p.ParameterType)
            .ToList();

        return jobHandlerMethods.Any(h =>
            h.Name.Equals(methodInfo.Name) &&
            h.GetParameters().Select(p => p.ParameterType).SequenceEqual(methodParameters) &&
            h.ReturnParameter.ParameterType == methodInfo.ReturnParameter.ParameterType
        );
    }

    private static bool IsZeebeJobWorkerHandler(Type type)
    {
        return type == typeof(IJobWorkerHandler) || type == typeof(IJobWorkerHandlerAsync);
    }

    private static bool IsZeebeWorker(Type type)
    {
        var interfaces = type.GetInterfaces();
        return interfaces.Contains(typeof(IJobWorkerHandler)) ||
               interfaces.Contains(typeof(IJobWorkerHandlerAsync));
    }
}