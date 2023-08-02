using UnityEngine;

namespace PlayVibe
{
    public sealed class Wallet
    {
        private readonly EventAggregator _eventAggregator;
        private readonly UserManager _userManager;

        public Wallet(
            EventAggregator eventAggregator,
            UserManager userManager)
        {
            _userManager = userManager;
            _eventAggregator = eventAggregator;
            
            Subscriptions();
        }

        ~Wallet()
        {
            Unsubscribes();
        }

        private void Subscriptions()
        {
            _eventAggregator.Add<AddResourceEvent>(OnAddResourceEvent);
        }

        private void Unsubscribes()
        {
            _eventAggregator.Remove<AddResourceEvent>(OnAddResourceEvent);
        }
        
        private void OnAddResourceEvent(AddResourceEvent sender)
        {
            var result = Mathf.Clamp(_userManager.CurrentUser.Resources[sender.ResourceType] + sender.Value, 0, int.MaxValue);
            
            _userManager.CurrentUser.Resources[sender.ResourceType] = result;
            
            _eventAggregator.SendEvent(new SaveEvent());
        }
    }
}