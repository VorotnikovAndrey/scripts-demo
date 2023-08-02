using InputSystem;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public class GameplayStage : AbstractStageBase
    {
        private readonly LocationInput _locationInput;
        private readonly CameraManager _cameraManager;
        private readonly LocationsManager _locationsManager;
        private readonly UserManager _userManager;
        private readonly DiContainer _diContainer;
        
        private BuildingController _buildingController;
        
        public override string StageType => Constants.Stages.Gameplay;
        
        public GameplayStage(
            EventAggregator eventAggregator,
            IInput input,
            CameraManager cameraManager,
            LocationsManager locationsManager,
            UserManager userManager,
            DiContainer diContainer) 
            : base(eventAggregator)
        {
            _diContainer = diContainer;
            _userManager = userManager;
            _locationsManager = locationsManager;
            _cameraManager = cameraManager;
            _locationInput = input as LocationInput;
        }

        public override async void Initialize(object data)
        {
            base.Initialize(data);

            await _locationsManager.LoadLocation(_userManager.CurrentUser.CurrentLevel);
            await _cameraManager.ChangeCamera(CameraType.LocationCamera, _locationsManager.Location.CameraSettings);

            _buildingController = _diContainer.Instantiate<BuildingController>();
            _buildingController.Initialize();
            
            _locationInput.Initialize(_cameraManager.Current);
            
            _eventAggregator.SendEvent(new ShowPopupEvent
            {
                PopupOptions = new PopupOptions
                {
                    PopupType = Constants.Popups.Hud
                }
            });
        }

        public override void DeInitialize()
        {
            _locationInput.DeInitialize();
            _buildingController.DeInitialize();
            
            base.DeInitialize();
        }
    }
}