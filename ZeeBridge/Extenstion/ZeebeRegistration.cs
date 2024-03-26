using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Zeebe.Client;
using Zeebe.Client.Impl.Builder;
using ZeeBridge.BackgroundServices;
using ZeeBridge.BackgroundServices;
using ZeeBridge.Configs;
using ZeeBridge.Interfaces;
using ZeeBridge.Services;

namespace ZeeBridge.Extenstion;

public static class ZeebeRegistration
{
    public static IServiceCollection AddZeeBridge(this IServiceCollection services, IConfiguration configuration,
        Assembly[] assemblies)
    {
        return services
            .AddZeebeConfig(configuration)
            .AddJobWorkerInfo(assemblies)
            .AddZeebeClient()
            .AddHostedService<ZeeBridgeWorkerService>();
    }

    private static IServiceCollection AddJobWorkerInfo(this IServiceCollection services, Assembly[] assemblies)
    {
        var jobWorkerProvider = new JobWorkerProvider(assemblies);
        services.AddSingleton(typeof(IJobWorkerProvider), jobWorkerProvider);

        foreach (var jobWorkerInfo in jobWorkerProvider.JobWorkerInfos)
        {
            if (
                jobWorkerInfo.Handler.ReflectedType is null ||
                IsJobWorkerRegistered(services, jobWorkerInfo.Handler.ReflectedType)
            )
                continue;

            services.Add(new ServiceDescriptor(jobWorkerInfo.Handler.ReflectedType,
                jobWorkerInfo.Handler.ReflectedType, jobWorkerInfo.ServiceLifetime));
        }

        return services;
    }

    private static bool IsJobWorkerRegistered(IServiceCollection services, Type? reflectedType)
    {
        return services.Any(s => s.ServiceType == reflectedType && s.ImplementationType == reflectedType);
    }

    private static IServiceCollection AddZeebeConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var zeebeConfig = configuration
            .GetSection(ZeebeClientConfigOption.SectionName);
        return services
            .Configure<ZeebeClientConfigOption>(zeebeConfig);
    }

    private static IServiceCollection AddZeebeClient(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        return services.AddScoped<IZeeBridgeClient>(sp =>
        {
            var zeebeConfig = sp.GetRequiredService<IOptions<ZeebeClientConfigOption>>().Value;
            var zeebeClient = zeebeConfig.ClientConfig.ZeebeClientId switch
            {
                null or "" => ZeebeClient.Builder()
                    .UseGatewayAddress(zeebeConfig.ClientConfig.GatewayAddress)
                    .UsePlainText()
                    .Build(),
                _ => CamundaCloudClientBuilder.Builder()
                    .UseClientId(zeebeConfig.ClientConfig.ZeebeClientId)
                    .UseClientSecret(zeebeConfig.ClientConfig.ZeebeClientSecret)
                    .UseContactPoint(zeebeConfig.ClientConfig.GatewayAddress)
                    .Build()
            };
            return new ZeeBridgeClient(zeebeClient, zeebeConfig, serviceProvider);
        });
    }
}