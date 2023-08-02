using NTC.Global.Cache;
using UnityEngine;

namespace PlayVibe
{
    public class BaseLocation : MonoCache, ILocation
    {
        [SerializeField] private CameraSettings _cameraSettings;

        public CameraSettings CameraSettings => _cameraSettings;
        
        public void Initialize()
        {
            
        }

        public void DeInitialize()
        {
            
        }
    }
}