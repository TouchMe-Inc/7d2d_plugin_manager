namespace PluginManager.Core.Adapters;

public static class ClientInfoAdapter
{
    private const string Unknown = "<unknown>";

    public static Api.Contracts.ClientInfo FromGame(ClientInfo clientInfo)
    {
        if (clientInfo == null)
        {
            return new Api.Contracts.ClientInfo(-1, Unknown, Unknown, Unknown);
        }

        var entityId = clientInfo.entityId;
        var platformId = SafeString(clientInfo.PlatformId?.CombinedString);
        var crossId = SafeString(clientInfo.CrossplatformId?.CombinedString);
        var name = SafeString(clientInfo.playerName);

        return new Api.Contracts.ClientInfo(entityId, platformId, crossId, name);
    }

    private static string SafeString(string value) =>
        string.IsNullOrWhiteSpace(value) ? Unknown : value;
}