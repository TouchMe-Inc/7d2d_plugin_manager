using System;
using PluginManager.Api;
using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

public class PluginProxy(AppDomain domain, PluginBootstrap bootstrap) : IPlugin
{
    public string ModuleName => bootstrap.ModuleName;
    public string ModuleVersion => bootstrap.ModuleVersion;
    public string ModuleAuthor => bootstrap.ModuleAuthor;
    public string ModuleDescription => bootstrap.ModuleDescription;

    public string ModulePath
    {
        get => bootstrap.ModulePath;
        set => bootstrap.ModulePath = value;
    }

    public void Load(ICapabilityRegistry registry)
    {
        bootstrap.Load(registry);
    }

    public void Unload(ICapabilityRegistry registry)
    {
        bootstrap.Unload(registry);
        AppDomain.Unload(domain);
    }
}