using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using Utils.Events;

namespace PlayVibe
{
    public class UnityServicesInitializer
    {
        private readonly EventAggregator _eventAggregator;
        private readonly List<IInitializer> _data = new();

        public UnityServicesInitializer(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            
            _data.Add(new RemoteConfigInitializer(_eventAggregator));
            
            InitializeData();
        }

        ~UnityServicesInitializer()
        {
            DeInitializeData();
        }
        
        private void Subscriptions()
        {
            AuthenticationService.Instance.SignedIn += () => 
            {
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
                Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
            };

            AuthenticationService.Instance.SignInFailed += Debug.LogError;

            AuthenticationService.Instance.SignedOut += () =>
            {
                Debug.Log("Player signed out.");
            };
 
            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log("Player session could not be refreshed and expired.");
            };
        }

        private void Unsubscribes()
        {
        }
        
        private async void InitializeData()
        {
            await UnityServices.InitializeAsync();
            
            await UniTask.WaitUntil(Utilities.CheckForInternetConnection);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            
            Subscriptions();

            foreach (var initializer in _data)
            {
                await initializer.Initialize();
            }
            
            _eventAggregator.SendEvent(new UnityServicesInitializedEvent());
        }

        private async void DeInitializeData()
        {
            Unsubscribes();
            
            foreach (var initializer in _data)
            {
                await initializer.DeInitialize();
            }
            
            _eventAggregator.SendEvent(new UnityServicesDeInitializedEvent());
        }
    }
}