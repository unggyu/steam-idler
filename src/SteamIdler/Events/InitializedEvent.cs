using Prism.Events;
using SteamKit2;

namespace SteamIdler.Events
{
    public class InitializedEvent : PubSubEvent<SteamClient.ConnectedCallback>
    {
    }
}
