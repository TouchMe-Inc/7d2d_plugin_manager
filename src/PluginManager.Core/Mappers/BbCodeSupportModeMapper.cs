using System;
using PluginManager.Api.Contracts;

namespace PluginManager.Core.Mappers;

public static class BbCodeSupportModeMapper
{
    public static GeneratedTextManager.BbCodeSupportMode ToGame(BbCodeSupportMode type) => type switch
    {
        BbCodeSupportMode.NotSupported => GeneratedTextManager.BbCodeSupportMode.NotSupported,
        BbCodeSupportMode.Supported => GeneratedTextManager.BbCodeSupportMode.Supported,
        BbCodeSupportMode.SupportedAndAddEscapes => GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public static BbCodeSupportMode FromGame(GeneratedTextManager.BbCodeSupportMode type) => type switch
    {
        GeneratedTextManager.BbCodeSupportMode.NotSupported => BbCodeSupportMode.NotSupported,
        GeneratedTextManager.BbCodeSupportMode.Supported => BbCodeSupportMode.Supported,
        GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes => BbCodeSupportMode.SupportedAndAddEscapes,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}