using Microsoft.Extensions.DependencyInjection;

namespace ZeeBridge.Attributes;

public class ServiceLifetimeAttribute : Attribute
{
    public ServiceLifetimeAttribute(ServiceLifetime serviceLifetime)
    {
        ServiceLifetime = serviceLifetime;
    }

    internal ServiceLifetime ServiceLifetime { get; }
}