namespace PluginManager.Core.Mappers;

public static class TileEntityTypeMapper
{
    public static TileEntityType ToGame(Api.Contracts.TileEntityType type) => (TileEntityType)(int)type;

    public static Api.Contracts.TileEntityType FromGame(TileEntityType type) => (Api.Contracts.TileEntityType)(int)type;
}