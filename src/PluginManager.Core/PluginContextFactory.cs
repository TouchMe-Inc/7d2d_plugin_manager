using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

public class PluginContextFactory : IPluginContextFactory
{
    public IPluginContext Create(string fullPath, IPluginLoader loader, ICapabilityRegistry capabilities)
    {
        return new PluginContext(fullPath, loader, capabilities);
    }
}