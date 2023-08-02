namespace PlayVibe
{
    public class HudPopup : BasePopup
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
            
        }

        private void Unsubscribes()
        {
            
        }
    }
}