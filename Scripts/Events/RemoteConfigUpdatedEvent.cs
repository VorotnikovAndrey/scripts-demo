using Utils.Events;

namespace PlayVibe
{
    public class RemoteConfigUpdatedEvent : BaseEvent
    {
        public Unity.Services.RemoteConfig.ConfigResponse Config;
    }
}
