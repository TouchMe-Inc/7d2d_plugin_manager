using System;
using System.Linq;
using System.Reflection;
using PluginManager.Api;
using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

public class PluginBootstrap(string modulePath) : MarshalByRefObject
{
    public string ModuleName => _plugin?.ModuleName ?? "(none)";
    public string ModuleVersion => _plugin?.ModuleVersion ?? "(unknown)";
    public string ModuleAuthor => _plugin?.ModuleAuthor ?? "(unknown)";
    public string ModuleDescription => _plugin?.ModuleDescription ?? "(none)";

    public string ModulePath
    {
        get => _plugin?.ModulePath ?? modulePath;
        set => _plugin?.ModulePath = value;
    }

    public override object InitializeLifetimeService() => null;

    private IPlugin _plugin;

    public void Load(ICapabilityRegistry capabilityRegistry)
    {
        var asm = Assembly.LoadFrom(ModulePath);
        var type = asm.GetTypes().First(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
        _plugin = (IPlugin)Activator.CreateInstance(type);
        _plugin.ModulePath = ModulePath;
        _plugin.Load(capabilityRegistry);
    }

    public void Unload(ICapabilityRegistry capabilityRegistry) => _plugin?.Unload(capabilityRegistry);
}