using PluginManager.Api.Capabilities.Implementations.Utils;
using PluginManager.Api.Contracts;
using PluginManager.Api.Proxy;
using PluginManager.Core.Adapters;

namespace PluginManager.Core.Capabilities.Utils;

public class PlayerUtil: ProxyObject, IPlayerUtil
{
    public string Name => nameof(PlayerUtil);

    public void Kick(int entityId)
    {
        GetClientInfoFromEntityId(entityId)?
        .SendPackage(
            NetPackageManager.GetPackage<NetPackagePlayerDenied>().Setup(
                new GameUtils.KickPlayerData(GameUtils.EKickReason.ManualKick)
                )
            );
    }

    public void Teleport(int entityId, Vector3 position)
    {
        GetClientInfoFromEntityId(entityId)?
            .SendPackage(
                NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(
                    Vector3Adapter.ToGame(position)
                    )
                );
    }
    
    public Vector3 GetPlayerPosition(int entityId)
    {
        var entityPlayer = GetEntityPlayerFromEntityId(entityId);
        return entityPlayer == null ? null : Vector3Adapter.FromGame(entityPlayer.position);
    }
    
    public LandClaimOwner GetClaimOwner(int entityId, Vector3Int position)
    {
        if (GameManager.Instance.persistentPlayers == null) return LandClaimOwner.None;

        var playerData = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(entityId);
        if (playerData == null) return LandClaimOwner.None;

        var checkPos = Vector3i.FromVector3Rounded(Vector3IntAdapter.ToGame(position));
        var claimSize = GameStats.GetInt(EnumGameStats.LandClaimSize);
        var halfSize = (claimSize - 1) / 2;

        var minX = checkPos.x - halfSize;
        var maxX = checkPos.x + halfSize;
        var minZ = checkPos.z - halfSize;
        var maxZ = checkPos.z + halfSize;

        var chunkRadiusX = claimSize / 16 + 1;
        var chunkRadiusZ = claimSize / 16 + 1;

        for (var i = -chunkRadiusX; i <= chunkRadiusX; i++)
        {
            var x = minX + i * 16;
            for (var j = -chunkRadiusZ; j <= chunkRadiusZ; j++)
            {
                var z = minZ + j * 16;
                var chunk = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(new Vector3i(x, checkPos.y, z));
                if (!chunk.IndexedBlocks.TryGetValue("lpblock", out var lpBlocks)) continue;

                var worldPos = chunk.GetWorldPos();
                foreach (var localPos in lpBlocks)
                {
                    var blockPos = localPos + worldPos;
                    
                    if (blockPos.x < minX || blockPos.x > maxX || blockPos.z < minZ || blockPos.z > maxZ) continue;

                    if (!BlockLandClaim.IsPrimary(chunk.GetBlock(localPos))) continue;

                    var owner = GameManager.Instance.persistentPlayers.GetLandProtectionBlockOwner(blockPos);
                    if (owner == null || !GameManager.Instance.World.IsLandProtectionValidForPlayer(owner)) continue;

                    if (playerData == owner) 
                        return LandClaimOwner.Self;
                    
                    return owner.ACL?.Contains(playerData.PrimaryId) == true ? LandClaimOwner.Ally : LandClaimOwner.Other;
                }
            }
        }

        return LandClaimOwner.None;
    }

    private EntityPlayer GetEntityPlayerFromEntityId(int entityId)
    {
        GameManager.Instance.World.Players.dict.TryGetValue(entityId, out var entityPlayer);
        return entityPlayer;
    }

    private ClientInfo GetClientInfoFromEntityId(int entityId)
    {
        var clientInfo = ConnectionManager.Instance.Clients.ForEntityId(entityId);
        
        return clientInfo is { loginDone: true } ? clientInfo : null;
    }
}