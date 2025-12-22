using System;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginManager.Api;

namespace PluginManager;

public class PluginContext : IPluginContext
{
    public PluginState State { get; private set; } = PluginState.Unregistered;
    public IPlugin Plugin { get; private set; }
    public int PluginId { get; }
    public string FilePath { get; }

    private readonly IPluginLoader _loader;

    public PluginContext(string path, int id, IPluginLoader loader)
    {
        FilePath = path;
        PluginId = id;
        _loader = loader;

        var fileWatcher = new FileSystemWatcher
        {
            Path = Path.GetDirectoryName(path),
            Filter = "*.dll"
        };

        fileWatcher.Deleted += (s, e) =>
        {
            Log.Out($"Plugin {Plugin.ModuleName} has been deleted, unloading...");
            Unload(hotReload: true);
        };

        fileWatcher.Changed += (s, e) =>
        {
            Log.Out($"Reloading plugin {Plugin.ModuleName}");
            Unload(hotReload: true);
            Load(hotReload: true);
        };

        fileWatcher.EnableRaisingEvents = true;
    }

    public void Load(bool hotReload = false)
    {
        if (State == PluginState.Loaded) return;

        State = PluginState.Loading;

        try
        {
            Plugin = _loader.LoadPlugin(FilePath);
            Plugin.Load(hotReload);
            State = PluginState.Loaded;
        }
        catch
        {
            Log.Error($"Failed to load plugin {Plugin.ModuleName}");
            Unload(hotReload);
        }
    }

    public void Unload(bool hotReload = false)
    {
        if (State == PluginState.Unloaded) return;

        State = PluginState.Unloaded;

        Log.Out($"Unloading plugin {Plugin.ModuleName}");

        try
        {
            Plugin.Unload(hotReload);
        }
        catch
        {
            Log.Error($"Failed to unload {Plugin.ModuleName} during error recovery, forcing cleanup");
            return;
        }

        Log.Out($"Finished unloading plugin {Plugin.ModuleName}");
    }
}