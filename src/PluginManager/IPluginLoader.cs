using PluginManager.Api;

namespace PluginManager;

public interface IPluginLoader
{
    IPlugin LoadPlugin(string filePath);
}