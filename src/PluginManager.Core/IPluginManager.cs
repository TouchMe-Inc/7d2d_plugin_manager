using System.Collections.Generic;

namespace PluginManager.Core;

public interface IPluginManager
{
    public void LoadPlugin(string name);
    public void UnloadPlugin(string name);
    public IEnumerable<PluginInfo> GetLoadedPlugins();
}