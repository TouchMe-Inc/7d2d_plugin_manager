using System;
using PluginManager.Api.Contracts;

namespace PluginManager.Core.Mappers;

public static class ChatTypeMapper
{
    public static EChatType ToGame(ChatType type) => type switch
    {
        ChatType.Global => EChatType.Global,
        ChatType.Friends => EChatType.Friends,
        ChatType.Party => EChatType.Party,
        ChatType.Whisper => EChatType.Whisper,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public static ChatType FromGame(EChatType type) => type switch
    {
        EChatType.Global => ChatType.Global,
        EChatType.Friends => ChatType.Friends,
        EChatType.Party => ChatType.Party,
        EChatType.Whisper => ChatType.Whisper,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}