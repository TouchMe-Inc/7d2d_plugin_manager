using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

public class PluginManager(
    string pluginsDirectory,
    IPluginLoader loader,
    IPluginContextFactory contextFactory,
    ICapabilityRegistry capabilities)
    : IPluginManager
{
    private readonly HashSet<IPluginContext> _loadedPluginContexts = [];

    public void LoadPlugin(string path)
    {
        var fullPath = Path.Combine(pluginsDirectory, path);
        if (!File.Exists(fullPath)) throw new FileNotFoundException($"Plugin file not found: {fullPath}");

        var ctx = contextFactory.Create(fullPath, loader, capabilities);

        _loadedPluginContexts.Add(ctx);
        ctx.Load();
    }

    public void UnloadPlugin(string path)
    {
        var fullPath = Path.Combine(pluginsDirectory, path);

        var ctx = _loadedPluginContexts.FirstOrDefault(p =>
            string.Equals(p.FilePath, fullPath, StringComparison.OrdinalIgnoreCase));

        if (ctx == null) return;
        ctx.Unload();
        _loadedPluginContexts.Remove(ctx);
    }

    public IEnumerable<IPluginContext> GetLoadedPlugins() => _loadedPluginContexts;
}