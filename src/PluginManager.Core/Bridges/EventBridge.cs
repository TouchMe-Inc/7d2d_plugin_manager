using PluginManager.Api.Capabilities.Implementations.Events;
using PluginManager.Api.Hooks;

namespace PluginManager.Core.Bridges;

public abstract class EventBridge<TData, TEvent>(IEventRunner eventBus) where TEvent : IGameEvent
{
    protected ModEvents.EModEventResult Handle(ref TData data)
    {
        var evt = Convert(ref data);

        Log.Out($"Event {evt.GetType().Name}");
        var result = eventBus.Publish(evt, HookMode.Pre);

        switch (result)
        {
            case HookResult.Handled:
                eventBus.Publish(evt, HookMode.Post);
                return ModEvents.EModEventResult.StopHandlersAndVanilla;

            case HookResult.Stop:
                return ModEvents.EModEventResult.StopHandlersAndVanilla;

            case HookResult.Changed:
                ApplyChanges(evt);
                eventBus.Publish(evt, HookMode.Post);
                return ModEvents.EModEventResult.StopHandlersAndVanilla;
        }

        eventBus.Publish(evt, HookMode.Post);
        return ModEvents.EModEventResult.Continue;
    }

    protected abstract TEvent Convert(ref TData data);

    protected virtual void ApplyChanges(TEvent evt)
    {
    }
}