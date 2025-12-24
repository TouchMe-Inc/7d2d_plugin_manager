using System;
using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

internal class LoadedPlugin
{
    public AppDomain Domain { get; }
    public PluginBootstrap Bootstrap { get; }
    public PluginInfo Info { get; }

    public LoadedPlugin(AppDomain domain, PluginBootstrap bootstrap, PluginInfo info)
    {
        Domain = domain;
        Bootstrap = bootstrap;
        Info = info;
    }

    public void Unload(ICapabilityRegistry registry)
    {
        Bootstrap.Unload(registry);
        AppDomain.Unload(Domain);
    }
}