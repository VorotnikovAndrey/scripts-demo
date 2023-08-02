using InputSyatem;
using NTC.Global.Cache;
using UnityEngine;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public class BuildingPoint : MonoCache, IClickableView
    {
        [SerializeField] private string _id;
        [SerializeField] private Vector2 _cameraOffset = new Vector2(0.5f, 0.5f);
        
        [Inject] protected EventAggregator _eventAggregator;

        public string Id => _id;
        public Vector2 Offset => _cameraOffset;

        protected override void OnEnabled()
        {
            Subscriptions();
        }
        
        protected override void OnDisabled()
        {
            Unsubscribes();
        }
        
        protected virtual void Subscriptions()
        {
           
        }

        protected virtual void Unsubscribes()
        {
            
        }

        public void ProcessClick()
        {
            _eventAggregator.SendEvent(new BuildingPointClickEvent
            {
                Point = this,
            });
        }
    }
}