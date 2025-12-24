using PluginManager.Api;
using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

public class PluginContext(string path, IPluginLoader loader, ICapabilityRegistry capabilities) : IPluginContext
{
    public PluginState State { get; private set; } = PluginState.Unregistered;
    public IPlugin Plugin { get; private set; }
    public ICapabilityRegistry Capabilities { get; } = capabilities;
    public string FilePath { get; } = path;

    public void Load()
    {
        if (State == PluginState.Loaded) return;

        State = PluginState.Loading;

        try
        {
            Plugin = loader.Load(FilePath);
            Plugin.Load(Capabilities);
            State = PluginState.Loaded;
        }
        catch
        {
            Log.Error($"Failed to load plugin {Plugin.ModuleName}");
            Unload();
            throw;
        }
    }

    public void Unload()
    {
        if (State == PluginState.Unloaded) return;

        State = PluginState.Unloaded;
        var cachedName = Plugin.ModuleName;

        Log.Out($"Unloading plugin {cachedName}");

        try
        {
            Plugin.Unload(Capabilities);
        }
        catch
        {
            Log.Error($"Failed to unload {cachedName} during error recovery, forcing cleanup");
            throw;
        }

        Log.Out($"Finished unloading plugin {cachedName}");
    }
}