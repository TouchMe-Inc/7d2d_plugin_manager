using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PluginManager.Core;

public class Application : IModApi
{
    public static Application Instance { get; private set; }

    public IPluginManager PluginManager { get; private set; }

    public void InitMod(Mod modInstance)
    {
        Instance = this;

        var pluginsDirectory = Path.Combine(modInstance.Path, "Plugins");
        var loader = new AppDomainPluginLoader(modInstance.Path);
        var contextFactory = new PluginContextFactory();
        var capabilities = new CapabilityRegistry();

        PluginManager = new PluginManager(pluginsDirectory, loader, contextFactory, capabilities);

        var pluginListFile = Path.Combine(modInstance.Path, "Configs", "plugins.txt");
        foreach (var name in GetPluginNamesFromFile(pluginListFile))
        {
            LoadPluginSafe(name);
        }
    }

    private IEnumerable<string> GetPluginNamesFromFile(string filepath)
    {
        if (!File.Exists(filepath))
        {
            Log.Error($"Plugins list file not found: {filepath}");
            return Enumerable.Empty<string>();
        }

        return File.ReadAllLines(filepath)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line));
    }

    private void LoadPluginSafe(string name)
    {
        try
        {
            PluginManager.LoadPlugin(name);
            Log.Out($"Loaded plugin: {name}");
        }
        catch (FileNotFoundException)
        {
            Log.Error($"Plugin file not found: {name}");
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to load plugin {name}: {ex}");
        }
    }
}