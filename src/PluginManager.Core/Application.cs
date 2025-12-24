using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using PluginManager.Api.Capabilities.Implementations.Commands;
using PluginManager.Api.Capabilities.Implementations.Events;
using PluginManager.Api.Capabilities.Implementations.Logger;
using PluginManager.Api.ChatMessenger;
using PluginManager.Core.Capabilities.ChatMessenger;
using PluginManager.Core.Capabilities.Events;
using PluginManager.Core.Capabilities.Logger;
using PluginManager.Core.Commands;

namespace PluginManager.Core;

public class Application : IModApi
{
    public void InitMod(Mod modInstance)
    {
        var capabilities = new CapabilityRegistry();
        var eventBus = new EventBus();
        var commandManager = new CommandManager();

        capabilities.Register<IEventHandlers>(eventBus);
        capabilities.Register<ILogger>(new Logger());
        capabilities.Register<IChatMessenger>(new ChatMessenger());
        capabilities.Register<ICommandManager>(commandManager);

        var pluginManager = new PluginManager(modInstance.Path, capabilities);

        ModContext.Config = new Config();
        ModContext.PluginManager = pluginManager;
        ModContext.Capabilities = capabilities;
        ModContext.EventRunner = eventBus;
        ModContext.CommandRegistry = commandManager;

        var pluginListFile = Path.Combine(modInstance.Path, "Config", "plugins.txt");
        foreach (var name in GetPluginNamesFromFile(pluginListFile))
        {
            try
            {
                pluginManager.LoadPlugin(name);
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

        var harmony = new Harmony("pluginmanager");
        harmony.PatchAll();
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
}