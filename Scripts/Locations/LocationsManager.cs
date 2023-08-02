using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace PlayVibe
{
    public class LocationsManager
    {
        private DiContainer _diContainer;
        
        public ILocation Location { get; private set; }

        public LocationsManager(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        public async UniTask LoadLocation(string id)
        {
            var operation = await Addressables.LoadAssetAsync<GameObject>(id).Task;
            
            var instance = _diContainer.InstantiatePrefab(operation.gameObject);
            if (instance == null)
            {
                Debug.LogError("Instance is not found!".AddColorTag(Color.red));
                return;
            }

            if (Location != null)
            {
                Location?.DeInitialize();
                Addressables.Release(Location);
            }
          
            Location = instance.GetComponent<ILocation>();
            Location.Initialize();
        }
    }
}