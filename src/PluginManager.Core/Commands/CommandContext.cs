using System.Collections.Generic;
using PluginManager.Api.Capabilities.Implementations.Commands;
using PluginManager.Api.Proxy;

namespace PluginManager.Core.Commands;

public class CommandContext(IReadOnlyList<string> args, int entityId)
    : ProxyObject, ICommandContext
{
    public IReadOnlyList<string> Args { get; } = args;
    public int EntityId { get; } = entityId;
}