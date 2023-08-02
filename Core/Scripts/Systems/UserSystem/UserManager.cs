using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.CloudSave;
using Unity.Services.RemoteConfig;
using Utils.Events;

namespace PlayVibe
{
    public sealed class UserManager
    {
        private readonly EventAggregator _eventAggregator;

        public UserModel CurrentUser { get; private set; }

        public UserManager(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            
            Subscriptions();
        }

        ~UserManager()
        {
            Unsubscribes();
        }

        private void Subscriptions()
        {
            _eventAggregator.Add<SaveEvent>(OnSaveEvent);
            _eventAggregator.Add<RemoteConfigUpdatedEvent>(OnRemoteConfigUpdatedEvent);
        }

        private void Unsubscribes()
        {
            _eventAggregator.Remove<SaveEvent>(OnSaveEvent);
            _eventAggregator.Remove<RemoteConfigUpdatedEvent>(OnRemoteConfigUpdatedEvent);
        }
        
        private void OnSaveEvent(SaveEvent sender)
        {
            Save().Forget();
        }

        private void OnRemoteConfigUpdatedEvent(RemoteConfigUpdatedEvent sender)
        {
            Load();
        }

        private UserModel CreateUserModel()
        {
            var userModel = new UserModel();
            
            userModel.CurrentLevel = RemoteConfigService.Instance.appConfig.GetString(Constants.RemoteCongif.LocationStart);
            userModel.Resources.Add(Constants.Resources.Soft, RemoteConfigService.Instance.appConfig.GetInt(Constants.RemoteCongif.ResourcesSoftStart));
            userModel.Resources.Add(Constants.Resources.Hard, RemoteConfigService.Instance.appConfig.GetInt(Constants.RemoteCongif.ResourcesHardStart));

            return userModel;
        }

        private async void Load()
        {
            var data = await CloudSaveService.Instance.Data.LoadAllAsync();

            data.TryGetValue(Constants.User.UserModel, out var model);
            
            if (!string.IsNullOrEmpty(model))
            {
                CurrentUser = JsonConvert.DeserializeObject<UserModel>(model);
            }
            else
            {
                CurrentUser = CreateUserModel();
                await Save();
            }
            
            _eventAggregator.SendEvent(new UserLoadedEvent
            {
                UserModel = CurrentUser
            });
        }

        private async UniTask Save()
        {
            var json = JsonConvert.SerializeObject(CurrentUser);

            if (string.IsNullOrEmpty(json))
            {
                return;
            }
            
            var data = new Dictionary<string, object>
            {
                {Constants.User.UserModel, json}
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        }
    }
}