using System.Collections.Generic;

namespace PluginManager.Core;

public interface IPluginManager
{
    public void LoadPlugin(string path);
    public void UnloadPlugin(string path);
    public IEnumerable<IPluginContext> GetLoadedPlugins();
}