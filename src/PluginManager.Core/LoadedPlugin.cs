using System;
using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

internal class LoadedPlugin(AppDomain domain, PluginBootstrap bootstrap, PluginInfo info)
{
    private AppDomain Domain { get; } = domain;
    private PluginBootstrap Bootstrap { get; } = bootstrap;
    public PluginInfo Info { get; } = info;

    public void Unload(ICapabilityRegistry registry)
    {
        Bootstrap.Unload(registry);
        AppDomain.Unload(Domain);
    }
}