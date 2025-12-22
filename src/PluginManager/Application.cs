namespace PluginManager;

public class Application : IModApi
{
    private IPluginManager _pluginManager;

    public void InitMod(Mod modInstance)
    {
        Log.Out($"[MODS] Init mod {modInstance.Name}, path: {modInstance.Path}");

        _pluginManager = new PluginManager(modInstance.Path);
        _pluginManager.Load();
    }
}