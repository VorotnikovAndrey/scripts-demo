using Utils.Events;

namespace PlayVibe
{
    public class ChangeCameraEvent : BaseEvent
    {
        public CameraType CameraType;
        public CameraSettings CameraSettings;
    }
}