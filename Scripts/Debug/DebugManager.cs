using UniRx;
using UnityEngine;

namespace PlayVibe
{
    public class DebugManager
    {
        private readonly EventAggregator _eventAggregator;
        private readonly CompositeDisposable _compositeDisposable = new();
        
        private const int ResourcesUpper = 1000;
        private const int ResourcesMult = 100;

        public DebugManager(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Setup();
            
            Subscriptions();
        }

        ~DebugManager()
        {
            Unsubscribes();
            
            _compositeDisposable.Dispose();
        }
        
        private void Subscriptions()
        {
           
        }

        private void Unsubscribes()
        {
            
        }

        private void Setup()
        {
            Observable.EveryUpdate().Subscribe(_ => EveryUpdate()).AddTo(_compositeDisposable);
        }

        private void EveryUpdate()
        {
            UpdateResourceBar();
        }

        private void UpdateResourceBar()
        {
            bool pressed = Input.GetKey(KeyCode.LeftShift);

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _eventAggregator.SendEvent(new AddResourceEvent
                {
                    Value = pressed ? ResourcesUpper * ResourcesMult : ResourcesUpper,
                    ResourceType = Constants.Resources.Soft
                });
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _eventAggregator.SendEvent(new AddResourceEvent
                {
                    Value = pressed ? -(ResourcesUpper * ResourcesMult) : -ResourcesUpper,
                    ResourceType = Constants.Resources.Soft
                });
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _eventAggregator.SendEvent(new AddResourceEvent
                {
                    Value = pressed ? -(ResourcesUpper * ResourcesMult) : -ResourcesUpper,
                    ResourceType = Constants.Resources.Hard
                });
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _eventAggregator.SendEvent(new AddResourceEvent
                {
                    Value = pressed ? ResourcesUpper * ResourcesMult : ResourcesUpper,
                    ResourceType = Constants.Resources.Hard
                });
            }
        }
    }
}