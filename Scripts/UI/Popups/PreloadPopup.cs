using Cysharp.Threading.Tasks;
using Utils.Events;

namespace PlayVibe
{
    public class PreloadPopup : BasePopup
    {
        protected override void OnShow(object data = null)
        {
            Subscriptions();
        }

        protected override void OnHide()
        {
            Unsubscribes();
        }
        
        private void Subscriptions()
        {
            _eventAggregator.Add<UserLoadedEvent>(OnUserLoadedEvent);
        }

        private void Unsubscribes()
        {
            _eventAggregator.Remove<UserLoadedEvent>(OnUserLoadedEvent);
        }

        private void OnUserLoadedEvent(UserLoadedEvent sender)
        {
            _eventAggregator.SendEvent(new ChangeStageEvent
            {
                Stage = Constants.Stages.Gameplay
            });
            
            Hide().Forget();
        }
    }
}