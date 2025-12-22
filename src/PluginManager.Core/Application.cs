using HarmonyLib;
using PluginManager.Api.Capabilities.Implementations.ChatMessenger;
using PluginManager.Api.Capabilities.Implementations.Commands;
using PluginManager.Api.Capabilities.Implementations.Events;
using PluginManager.Api.Capabilities.Implementations.Logger;
using PluginManager.Api.Capabilities.Implementations.Utils;
using PluginManager.Core.Capabilities.ChatMessenger;
using PluginManager.Core.Capabilities.Events;
using PluginManager.Core.Capabilities.Logger;
using PluginManager.Core.Capabilities.Utils;
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
        capabilities.Register<IPlayerUtil>(new PlayerUtil());

        var pluginManager = new PluginManager(modInstance.Path, capabilities);

        ModContext.Config = new Config();
        ModContext.PluginManager = pluginManager;
        ModContext.Capabilities = capabilities;
        ModContext.EventRunner = eventBus;
        ModContext.CommandRegistry = commandManager;

        pluginManager.Load();

        var harmony = new Harmony("pluginmanager");
        harmony.PatchAll();
    }
}