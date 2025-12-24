using System;
using System.Collections.Generic;
using PluginManager.Api.Exposed.Events;
using PluginManager.Api.Hooks;

namespace PluginManager.Core.Capabilities;

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, HandlerBucket> _handlers = new();

    public void RegisterEventHandler<T>(IEventBus.GameEventHandler<T> handler, HookMode hookMode = HookMode.Post)
        where T : IGameEvent
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));
        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var bucket))
        {
            bucket = new HandlerBucket();
            _handlers[type] = bucket;
        }

        bucket.Add(handler, hookMode);
    }

    public void DeregisterEventHandler<T>(IEventBus.GameEventHandler<T> handler, HookMode hookMode = HookMode.Post)
        where T : IGameEvent
    {
        if (handler == null) return;
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var bucket))
        {
            bucket.Remove(handler, hookMode);
        }
    }

    private class HandlerBucket
    {
        private readonly Dictionary<HookMode, List<Delegate>> _handlers =
            new()
            {
                { HookMode.Pre, [] },
                { HookMode.Post, [] }
            };

        public void Add<T>(IEventBus.GameEventHandler<T> handler, HookMode mode) where T : IGameEvent
        {
            _handlers[mode].Add(handler);
        }

        public void Remove<T>(IEventBus.GameEventHandler<T> handler, HookMode mode) where T : IGameEvent
        {
            _handlers[mode].Remove(handler);
        }

        public HookResult Invoke<T>(T evt, HookMode mode) where T : IGameEvent
        {
            foreach (var d in _handlers[mode])
            {
                var result = ((IEventBus.GameEventHandler<T>)d)(evt);

                if (result is HookResult.Stop or HookResult.Handled or HookResult.Changed)
                {
                    return result;
                }
            }

            return HookResult.Continue;
        }
    }
}