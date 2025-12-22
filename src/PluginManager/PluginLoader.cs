using System;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginManager.Api;

namespace PluginManager;

public class FileCopyPluginLoader : IPluginLoader
{
    private readonly string _cacheDir;

    public FileCopyPluginLoader(string cacheDir)
    {
        _cacheDir = cacheDir;
        Directory.CreateDirectory(_cacheDir);
    }

    public IPlugin LoadPlugin(string filePath)
    {
        var tempPath = CreateTempCopy(filePath);

        var asm = Assembly.LoadFrom(tempPath);
        var type = asm.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);

        if (type == null)
            throw new Exception($"Unable to find plugin in assembly {filePath}");

        var plugin = (IPlugin)Activator.CreateInstance(type);
        plugin.ModulePath = filePath;
        return plugin;
    }

    private string CreateTempCopy(string originalPath)
    {
        var name = Path.GetFileNameWithoutExtension(originalPath);
        var pluginDir = Path.Combine(_cacheDir, name);
        Directory.CreateDirectory(pluginDir);

        var version = Directory.GetFiles(pluginDir, "*.temp.dll").Length + 1;
        var tempPath = Path.Combine(pluginDir, $"{name}_v{version}.temp.dll");

        File.Copy(originalPath, tempPath, overwrite: true);
        return tempPath;
    }
}