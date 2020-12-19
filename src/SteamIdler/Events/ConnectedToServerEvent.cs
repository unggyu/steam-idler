using Prism.Events;
using SteamKit2;

namespace SteamIdler.Events
{
    public class ConnectedToServerEvent : PubSubEvent<SteamClient.ConnectedCallback>
    {
    }
}
