using System;
using System.Collections.Generic;
using PluginManager.Api.Capabilities;

namespace PluginManager.Core;

public sealed class CapabilityRegistry : ICapabilityRegistry
{
    private readonly Dictionary<(Type, string), Lazy<object>> _instances = new();

    public void Register<T>(IPluginCapability<T> capability, Func<T> supplier)
    {
        var key = (typeof(T), capability.Name);

        if (_instances.ContainsKey(key))
            throw new InvalidOperationException(
                $"Capability '{capability.Name}' for {typeof(T).Name} is already registered");
        
        _instances[key] = new Lazy<object>(() => supplier()!);
    }

    public void Deregister<T>(IPluginCapability<T> capability)
    {
        var key = (typeof(T), capability.Name);
        _instances.Remove(key);
    }

    public T Resolve<T>(IPluginCapability<T> capability)
    {
        var key = (typeof(T), capability.Name);

        if (_instances.TryGetValue(key, out var lazy))
            return (T)lazy.Value;

        throw new InvalidOperationException(
            $"Capability '{capability.Name}' for {typeof(T).Name} was not found");
    }
}
