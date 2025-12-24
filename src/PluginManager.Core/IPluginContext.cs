using PluginManager.Api;
using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

public interface IPluginContext
{
    PluginState State { get; }
    IPlugin Plugin { get; }
    ICapabilityRegistry Capabilities { get; }

    string FilePath { get; }

    void Load();
    void Unload();
}