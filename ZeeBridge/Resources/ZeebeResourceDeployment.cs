using Google.Apis.Logging;
using ILogger = Google.Apis.Logging.ILogger;

namespace ZeeBridge.Resources;

public class ZeebeResourceDeployment
{
    private readonly IServiceProvider _serviceProvider;

    public ZeebeResourceDeployment(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ZeebeResourceDeploymentWithDirectory UsingDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new ArgumentException("Directory does not exist.");
        }
        
        if (string.IsNullOrEmpty(directoryPath))
        {
            throw new ArgumentException("Directory must not be null or empty.");
        }
        
        return new ZeebeResourceDeploymentWithDirectory(_serviceProvider, directoryPath);
    }
}