using NTC.Global.Cache;
using UnityEngine;

namespace PlayVibe
{
    [RequireComponent(typeof(Camera))]
    public class PreloadCamera : MonoCache, ICamera
    {
        public CameraType CameraType => CameraType.PreloaderCamera;
        
        [HideInInspector] [SerializeField] private Camera _cam;

        public Camera Camera => _cam;
        public GameObject GameObject => gameObject;

        private void OnValidate()
        {
            if (_cam == null)
            {
                _cam = GetComponent<Camera>();
            }
        }
        
        public void Initialize(CameraSettings cameraSettings)
        {
            
        }

        public void DeInitialize()
        {
            
        }
    }
}