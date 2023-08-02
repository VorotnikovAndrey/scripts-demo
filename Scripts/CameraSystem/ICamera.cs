using UnityEngine;
using CameraType = UnityEngine.CameraType;

namespace PlayVibe
{
    public interface ICamera
    {
        CameraType CameraType { get; }
        Camera Camera { get; }
        GameObject GameObject { get; }

        void Initialize(CameraSettings cameraSettings);
        void DeInitialize();
    }
}
