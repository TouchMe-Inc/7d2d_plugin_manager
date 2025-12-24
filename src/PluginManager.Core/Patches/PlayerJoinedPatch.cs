using HarmonyLib;
using PluginManager.Api.Capabilities.Implementations.Events.GameEvents;
using PluginManager.Api.Hooks;

namespace PluginManager.Core.Patches;

[HarmonyPatch(typeof(GameManager), nameof(GameManager.RequestToEnterGame))]
public class PlayerJoinedPatch
{
    static void Prefix(ClientInfo _cInfo)
    {
        var tileEntityAccessAttemptEvent = new PlayerJoinedGameEvent(_cInfo.entityId);
        ModContext.EventRunner.Publish(tileEntityAccessAttemptEvent, HookMode.Pre);
    }
    
    static void Postfix(ClientInfo _cInfo)
    {
        var tileEntityAccessAttemptEvent = new PlayerJoinedGameEvent(_cInfo.entityId);
        ModContext.EventRunner.Publish(tileEntityAccessAttemptEvent, HookMode.Post);
    }
}