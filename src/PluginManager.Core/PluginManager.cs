using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

public class PluginManager : IPluginManager
{
    private readonly string _rootDirectory;
    private readonly string _pluginsDirectory;
    private readonly ICapabilityRegistry _capabilities;

    private readonly Dictionary<string, LoadedPlugin> _plugins = new();

    public PluginManager(string rootDirectory, ICapabilityRegistry capabilities)
    {
        _rootDirectory = Path.GetFullPath(rootDirectory);
        _pluginsDirectory = Path.Combine(_rootDirectory, "Plugins");
        _capabilities = capabilities;
    }

    public void LoadPlugin(string path)
    {
        var fullPath = Path.Combine(_pluginsDirectory, path);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(fullPath);
        }

        if (_plugins.ContainsKey(fullPath))
            throw new InvalidOperationException("Plugin already loaded");

        var setup = new AppDomainSetup
        {
            ApplicationBase = _pluginsDirectory,
            PrivateBinPath = _rootDirectory
        };

        var domain = AppDomain.CreateDomain($"PluginDomain_{Guid.NewGuid()}", null, setup);

        var bootstrap = (PluginBootstrap)domain.CreateInstanceAndUnwrap(
            typeof(PluginBootstrap).Assembly.FullName,
            typeof(PluginBootstrap).FullName!,
            false, BindingFlags.Default, null, [fullPath],
            null, null
        );

        bootstrap.Load(_capabilities);

        var info = new PluginInfo
        {
            Name = bootstrap.ModuleName,
            Version = bootstrap.ModuleVersion,
            Author = bootstrap.ModuleAuthor,
            Description = bootstrap.ModuleDescription,
            Path = fullPath
        };

        _plugins[fullPath] = new LoadedPlugin(domain, bootstrap, info);
    }

    public void UnloadPlugin(string path)
    {
        var fullPath = Path.Combine(_pluginsDirectory, path);

        if (!_plugins.TryGetValue(fullPath, out var plugin))
            return;

        plugin.Unload(_capabilities);
        _plugins.Remove(fullPath);
    }

    public IEnumerable<PluginInfo> GetLoadedPlugins()
    {
        return _plugins.Values.Select(p => p.Info).ToList();
    }
}