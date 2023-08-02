using Cysharp.Threading.Tasks;
using Unity.Services.RemoteConfig;
using UnityEngine;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public class RemoteConfigInitializer : IInitializer
    {
        private readonly EventAggregator _eventAggregator;

        public RemoteConfigInitializer(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        
        public struct userAttributes
        {
        }

        public struct appAttributes
        {
        }
        
        public UniTask Initialize()
        {
            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
            RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
            
            return UniTask.CompletedTask;
        }

        public UniTask DeInitialize()
        {
            RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteSettings;
            
            return UniTask.CompletedTask;
        }

        private void ApplyRemoteSettings(ConfigResponse configResponse)
        {
            switch (configResponse.requestOrigin)
            {
                case ConfigOrigin.Default:
                    Debug.Log("Nothing were loaded, used default settings");
                    break;
                case ConfigOrigin.Cached:
                    Debug.Log("Nothing were loaded, used cashed values from previous session");
                    break;
                case ConfigOrigin.Remote:
                    Debug.Log("New settings were loaded");
                    break;
            }
            
            _eventAggregator.SendEvent(new RemoteConfigUpdatedEvent
            {
                Config = configResponse
            });
        }
    }
}