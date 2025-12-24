using PluginManager.Api;

namespace PluginManager.Core;

public interface IPluginLoader
{
    IPlugin Load(string filePath);
}