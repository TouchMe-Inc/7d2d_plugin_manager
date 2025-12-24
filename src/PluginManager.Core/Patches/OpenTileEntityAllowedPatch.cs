using HarmonyLib;
using PluginManager.Api.Capabilities.Implementations.Events.GameEvents;
using PluginManager.Api.Hooks;

namespace PluginManager.Core.Patches;

[HarmonyPatch(typeof(GameManager), nameof(GameManager.OpenTileEntityAllowed))]
public static class OpenTileEntityAllowedPatch
{
    static bool Prefix(GameManager __instance, ref bool __result, int _entityIdThatOpenedIt, TileEntity _te,
        string _customUi)
    {
        var tileEntityAccessAttemptEvent = new TileEntityAccessAttemptEvent(_entityIdThatOpenedIt, _te);
        var result = ModContext.EventRunner.Publish(tileEntityAccessAttemptEvent, HookMode.Pre);

        if (result == HookResult.Continue)
        {
            return true;
        }

        __result = false;
        return false;
    }
}