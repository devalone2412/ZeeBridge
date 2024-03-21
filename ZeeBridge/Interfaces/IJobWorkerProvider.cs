using ZeeBridge.Models;

namespace ZeeBridge.Interfaces;

public interface IJobWorkerProvider
{
    IEnumerable<JobWorkerInfo> JobWorkerInfos { get; }
}