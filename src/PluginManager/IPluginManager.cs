using System.Collections.Generic;

namespace PluginManager;

public interface IPluginManager
{
    public void Load();
    public void LoadPlugin(string path);
    public IEnumerable<IPluginContext> GetLoadedPlugins();
}