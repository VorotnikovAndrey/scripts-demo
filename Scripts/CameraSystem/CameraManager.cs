using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public class CameraManager
    {
        private readonly EventAggregator _eventAggregator;
        private readonly DiContainer _diContainer;

        public ICamera Current { get; private set; }

        public CameraManager(
            EventAggregator eventAggregator,
            [Inject(Id = CameraType.PreloaderCamera)] ICamera preloadCamera,
            DiContainer diContainer)
        {
            _diContainer = diContainer;
            _eventAggregator = eventAggregator;

            Current = preloadCamera;

            Subscriptions();
        }

        ~CameraManager()
        {
            Unsubscribes();
        }
        
        private void Subscriptions()
        {
            _eventAggregator.Add<ChangeCameraEvent>(OnChangeCameraEvent);
        }

        private void Unsubscribes()
        {
            _eventAggregator.Remove<ChangeCameraEvent>(OnChangeCameraEvent);
        }

        private void OnChangeCameraEvent(ChangeCameraEvent sender)
        {
            ApplyChangeCamera(sender.CameraType, sender.CameraSettings).Forget();
        }

        public async UniTask ChangeCamera(CameraType type, CameraSettings cameraSettings)
        {
            await ApplyChangeCamera(type, cameraSettings);
        }

        private async UniTask ApplyChangeCamera(CameraType type, CameraSettings cameraSettings)
        {
            var operation = await Addressables.LoadAssetAsync<GameObject>($"{type}").Task;
            
            var instance = _diContainer.InstantiatePrefab(operation.gameObject);
            if (instance == null)
            {
                Debug.LogError("Instance is not found!".AddColorTag(Color.red));
                return;
            }

            if (Current != null)
            {
                Current.DeInitialize();
                GameObject.DestroyImmediate(Current.GameObject);
            }
            
            Current = instance.GetComponent<ICamera>();
            Current.Initialize(cameraSettings);
        }
    }
}