using System.Collections.Generic;
using System.Linq;
using PopupSystem;
using UnityEngine;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public sealed class MainCanvas : MonoBehaviour
    {
        [SerializeField] private List<CanvasModel> _models;
        
        private EventAggregator _eventAggregator;

        [Inject]
        private void Constuct(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Add<PopupHidenEvent>(OnPopupHidenEvent);
        }

        private void OnPopupHidenEvent(PopupHidenEvent sender)
        {
            if (sender.Popup.PopupType != Constants.Popups.Preload)
            {
                return;
            }
            
            _eventAggregator.Remove<PopupHidenEvent>(OnPopupHidenEvent);
        }

        public RectTransform Get(PopupGroup group)
        {
            return _models.FirstOrDefault(x => x.PopupGroup == group)?.RectTransform;
        }
    }
}