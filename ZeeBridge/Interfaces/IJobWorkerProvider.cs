using ZeeBridge.Models;

namespace ZeeBridge.Interfaces;

internal interface IJobWorkerProvider
{
    IEnumerable<JobWorkerInfo> JobWorkerInfos { get; }
}