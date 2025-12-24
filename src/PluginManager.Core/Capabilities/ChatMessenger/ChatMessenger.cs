using PluginManager.Api.Capabilities.Implementations.ChatMessenger;
using PluginManager.Api.Proxy;

namespace PluginManager.Core.Capabilities.ChatMessenger;

public class ChatMessenger : ProxyObject, IChatMessenger
{
    public string Name => nameof(ChatMessenger);

    public void SendTo(int entityId, string message)
    {
        var client = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityId);
        client?.SendPackage(NetPackageManager.GetPackage<NetPackageChat>()
            .Setup(EChatType.Global, -1, message, null, EMessageSender.None,
                GeneratedTextManager.BbCodeSupportMode.Supported));
    }
}