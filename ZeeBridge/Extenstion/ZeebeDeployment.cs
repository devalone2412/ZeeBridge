using Microsoft.Extensions.Hosting;
using ZeeBridge.Resources;

namespace ZeeBridge.Extenstion;

public static class ZeebeDeployment
{
    public static ZeebeResourceDeployment CreateDeployment(this IServiceProvider serviceProvider)
    {
        return new ZeebeResourceDeployment(serviceProvider);
    }

    public static ZeebeResourceDeployment CreateDeployment(this IHost host)
    {
        return new ZeebeResourceDeployment(host.Services);
    }
}