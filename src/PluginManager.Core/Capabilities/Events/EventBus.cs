using System;
using System.Collections.Generic;
using PluginManager.Api.Capabilities.Implementations.Events;
using PluginManager.Api.Hooks;
using PluginManager.Api.Proxy;

namespace PluginManager.Core.Capabilities.Events;

public sealed class EventBus : ProxyObject, IEventHandlers, IEventRunner
{
    public string Name => nameof(EventBus);
    
    private readonly Dictionary<Type, HandlerBucket> _handlers = new();

    public void RegisterHandler<T>(DelegateProxy proxy, HookMode mode) where T : IGameEvent
    {
        if (!_handlers.TryGetValue(typeof(T), out var bucket))
            _handlers[typeof(T)] = bucket = new HandlerBucket();

        bucket.Add(proxy, mode);
    }

    public void DeregisterHandler<T>(DelegateProxy proxy, HookMode mode) where T : IGameEvent
    {
        if (_handlers.TryGetValue(typeof(T), out var bucket))
            bucket.Remove(proxy, mode);
    }

    public HookResult Publish<T>(T evt, HookMode mode) where T : IGameEvent
    {
        return _handlers.TryGetValue(typeof(T), out var bucket)
            ? bucket.Invoke(evt, mode)
            : HookResult.Continue;
    }

    private class HandlerBucket
    {
        private readonly Dictionary<HookMode, List<DelegateProxy>> _handlers = new()
        {
            { HookMode.Pre, [] },
            { HookMode.Post, [] }
        };

        public void Add(DelegateProxy proxy, HookMode mode) => _handlers[mode].Add(proxy);
        public void Remove(DelegateProxy proxy, HookMode mode) => _handlers[mode].RemoveAll(p => p.Equals(proxy));

        public HookResult Invoke<T>(T evt, HookMode mode) where T : IGameEvent
        {
            var final = HookResult.Continue;
            var handlers = _handlers[mode];

            for (int i = 0; i < handlers.Count; i++)
            {
                var result = (HookResult)handlers[i].Invoke(evt);

                switch (result)
                {
                    case HookResult.Continue: break;
                    case HookResult.Changed:
                    {
                        if (final == HookResult.Continue)
                        {
                            final = HookResult.Changed;
                        }

                        break;
                    }
                    case HookResult.Handled:
                    {
                        final = HookResult.Handled;
                        break;
                    }

                    case HookResult.Stop: return HookResult.Stop;
                }
            }

            return final;
        }
    }
}