namespace PlayVibe
{
    public interface ILocation
    {
        public CameraSettings CameraSettings { get; }
        
        public void Initialize();
        public void DeInitialize();
    }
}