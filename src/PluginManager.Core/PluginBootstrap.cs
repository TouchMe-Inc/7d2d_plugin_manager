using System;
using System.Linq;
using System.Reflection;
using PluginManager.Api;
using PluginManager.Api.Capabilities;
using PluginManager.Api.Proxy;

namespace PluginManager.Core;

public class PluginBootstrap(string modulePath) : ProxyObject
{
    public string ModuleName => _plugin?.ModuleName ?? "(none)";
    public string ModuleVersion => _plugin?.ModuleVersion ?? "(unknown)";
    public string ModuleAuthor => _plugin?.ModuleAuthor ?? "(unknown)";
    public string ModuleDescription => _plugin?.ModuleDescription ?? "(none)";
    
    private IPlugin _plugin;

    public void Load(ICapabilityRegistry capabilityRegistry)
    {
        var asm = Assembly.LoadFrom(modulePath);
        var type = asm.GetTypes().First(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
        _plugin = (IPlugin)Activator.CreateInstance(type);
        _plugin.Load(capabilityRegistry);
    }

    public void Unload(ICapabilityRegistry capabilityRegistry)
    {
        _plugin?.Unload(capabilityRegistry);
        _plugin = null;
    }
}