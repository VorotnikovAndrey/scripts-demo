using System.Collections.Generic;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public class BuildingController
    {
        private EventAggregator _eventAggregator;

        private readonly Dictionary<BuildingModel, BuildingView> _elements = new();

        [Inject]
        private void Construct(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            Subscriptions();
        }

        public void DeInitialize()
        {
            Unsubscribes();
        }
        
        private void Subscriptions()
        {
            _eventAggregator.Add<BuildingPointClickEvent>(OnBuildingPointClickEvent);
        }

        private void Unsubscribes()
        {
            _eventAggregator.Remove<BuildingPointClickEvent>(OnBuildingPointClickEvent);
        }
        
        private void OnBuildingPointClickEvent(BuildingPointClickEvent sender)
        {
            _eventAggregator.SendEvent(new ShowPopupEvent
            {
                PopupOptions = new PopupOptions
                {
                    PopupType = Constants.Popups.Building
                }
            });
        }
    }
}