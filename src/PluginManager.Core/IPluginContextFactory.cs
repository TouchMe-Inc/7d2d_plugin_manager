using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

public interface IPluginContextFactory
{
    IPluginContext Create(string fullPath, IPluginLoader loader, ICapabilityRegistry capabilities);
}