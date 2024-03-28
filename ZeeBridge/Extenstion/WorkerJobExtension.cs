using Zeebe.Client.Api.Commands;
using Zeebe.Client.Api.Responses;
using Zeebe.Client.Api.Worker;

namespace ZeeBridge.Extenstion;

public static class WorkerJobExtension
{
    private static IJobClient? _jobClient;

    internal static void SetJobClient(this IJob job, IJobClient jobClient)
    {
        _jobClient = jobClient;
    }

    public static T GetVariables<T>(this IJob job)
    {
        return job.Variables.FromJson<T>() ??
               throw new InvalidOperationException($"Failed to deserialize variables for job {job.Key}");
    }

    public static bool MarkAsCompleted(this IJob job, object transferData)
    {
        var data =transferData.ToJson();
        CreateCompleteJobCommand(job, data)
            .Send()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        return true;
    }

    public static async Task MarkAsCompletedAsync(this IJob job, object transferData)
    {
        var data = transferData.ToJson();
        await CreateCompleteJobCommand(job, data)
            .Send();
    }

    public static void MarkAsFailed(this IJob job, string errorMessage)
    {
        CreateFailedJobCommand(job, errorMessage)
            .Send()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public static async Task MarkAsFailedAsync(this IJob job, string errorMessage)
    {
        await CreateFailedJobCommand(job, errorMessage)
            .Send();
    }

    private static IFailJobCommandStep2 CreateFailedJobCommand(IJob job, string errorMessage)
    {
        return _jobClient!.NewFailCommand(job.Key)
            .Retries(job.Retries - 1)
            .ErrorMessage(errorMessage);
    }

    private static ICompleteJobCommandStep1 CreateCompleteJobCommand(IJob job, string data)
    {
        return _jobClient!.NewCompleteJobCommand(job.Key)
            .Variables(data);
    }
}