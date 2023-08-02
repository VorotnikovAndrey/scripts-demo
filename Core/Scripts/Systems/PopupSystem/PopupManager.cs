using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public sealed class PopupManager
    {
        private readonly List<BasePopup> _popups = new();
        private readonly EventAggregator _eventAggregator;
        private readonly ScreenFaderFactory _screenFaderFactory;
        private readonly MainCanvas _mainCanvas;
        private readonly DiContainer _diContainer;

        public PopupManager(EventAggregator eventAggregator, ScreenFaderFactory screenFaderFactory, MainCanvas mainCanvas, DiContainer diContainer)
        {
            _diContainer = diContainer;
            _mainCanvas = mainCanvas;
            _screenFaderFactory = screenFaderFactory;
            _eventAggregator = eventAggregator;

            Subscriptions();
        }

        ~PopupManager()
        {
            Unsubscribes();
        }

        private void Subscriptions()
        {
            _eventAggregator.Add<ShowPopupEvent>(OnShowPopupEvent);
            _eventAggregator.Add<HidePopupEvent>(OnHidePopupEvent);
            _eventAggregator.Add<HideAllPopupEvent>(OnHideAllPopupEvent);
            _eventAggregator.Add<PopupHidenEvent>(OnPopupHidenEvent);
        }

        private void Unsubscribes()
        {
            _eventAggregator.Remove<ShowPopupEvent>(OnShowPopupEvent);
            _eventAggregator.Remove<HidePopupEvent>(OnHidePopupEvent);
            _eventAggregator.Remove<HideAllPopupEvent>(OnHideAllPopupEvent);
            _eventAggregator.Remove<PopupHidenEvent>(OnPopupHidenEvent);
        }

        private async void OnShowPopupEvent(ShowPopupEvent sender)
        {
            var popup = await ShowPopup(sender.PopupOptions);

            if (popup == null)
            {
                return;
            }

            if (popup.IsShow)
            {
                return;
            }

            popup.Show(sender.PopupOptions.Data).Forget();
        }

        private void OnHidePopupEvent(HidePopupEvent sender)
        {
            sender.Popup.Hide().Forget();
        }

        private void OnHideAllPopupEvent(HideAllPopupEvent sender)
        {
            foreach (var popup in _popups)
            {
                if (!popup.IsShow)
                {
                    continue;
                }

                popup.Hide().Forget();
            }
        }

        private async UniTask<BasePopup> ShowPopup(PopupOptions options)
        {
            var handle = Addressables.InstantiateAsync(options.PopupType, _mainCanvas.Get(options.PopupGroup));
    
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var instance = handle.Result;
                
                _diContainer.InjectGameObject(instance);
                
                var popup = instance.GetComponent<BasePopup>();
                popup.SetScreenFaders(popup.ScreenFaderProfiles.Select(profile => _screenFaderFactory.GetFader(profile)).ToList());

                instance.SetActive(true);

                _popups.Add(popup);
                
                return popup;
            }

            Debug.LogError("Failed to instantiate object: " + handle.OperationException.Message);

            return null;
        }

        
        private void OnPopupHidenEvent(PopupHidenEvent sender)
        {
            _popups.Remove(sender.Popup);
        }
    }
}
