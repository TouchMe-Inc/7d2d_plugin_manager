using System;
using System.Reflection;
using PluginManager.Api;

namespace PluginManager.Core;

public class AppDomainPluginLoader(string modDir) : IPluginLoader
{
    public IPlugin Load(string filePath)
    {
        var setup = new AppDomainSetup
        {
            ApplicationBase = modDir
        };

        var domain = AppDomain.CreateDomain($"PluginDomain_{Guid.NewGuid()}", null, setup);

        var bootstrap = (PluginBootstrap)domain.CreateInstanceAndUnwrap(
            typeof(PluginBootstrap).Assembly.FullName,
            typeof(PluginBootstrap).FullName,
            false, BindingFlags.Default, null, [filePath], null, null);

        return new PluginProxy(domain, bootstrap);
    }
}