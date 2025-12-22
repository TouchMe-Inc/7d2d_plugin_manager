using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace PluginManager;

public class PluginManager : IPluginManager
{
    private readonly HashSet<IPluginContext> _loadedPluginContexts = [];
    private readonly string _pluginsDir;
    private readonly string _pluginsListFile;
    private readonly IPluginLoader _loader;

    public PluginManager(string modDir)
    {
        _pluginsDir = Path.Combine(modDir, "Plugins");
        _pluginsListFile = Path.Combine(_pluginsDir, "plugins.txt");
        _loader = new FileCopyPluginLoader(Path.Combine(modDir, "PluginCache"));
    }

    public void Load()
    {
        if (!File.Exists(_pluginsListFile))
        {
            Log.Out($"Plugins list file not found: {_pluginsListFile}");
            return;
        }

        var pluginNames = File.ReadAllLines(_pluginsListFile).Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line.Trim());

        foreach (var name in pluginNames)
        {
            var path = Path.Combine(_pluginsDir, name);
            if (!File.Exists(path))
            {
                Log.Error($"Plugin file not found: {path}");
                continue;
            }

            LoadPlugin(path);
        }
    }

    public void LoadPlugin(string path)
    {
        var plugin = new PluginContext(path, _loadedPluginContexts.Select(x => x.PluginId).DefaultIfEmpty(0).Max() + 1, _loader);
        _loadedPluginContexts.Add(plugin);
        plugin.Load();
    }

    public IEnumerable<IPluginContext> GetLoadedPlugins()
    {
        return _loadedPluginContexts;
    }
}