using Utils.Events;

namespace PlayVibe
{
    public class PreloadStage : AbstractStageBase
    {
        private readonly UserManager _userManager;
        
        public override string StageType => Constants.Stages.Preload;
        
        public PreloadStage(EventAggregator eventAggregator, UserManager userManager) : base(eventAggregator)
        {
            _userManager = userManager;
        }

        public override void Initialize(object data)
        {
            base.Initialize(data);

            _eventAggregator.SendEvent(new ShowPopupEvent
            {
                PopupOptions = new PopupOptions
                {
                    PopupType = Constants.Popups.Preload
                }
            });
        }
    }
}