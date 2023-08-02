using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils.Events;
using Zenject;

namespace PlayVibe
{
    public abstract class BasePopup : MonoBehaviour
    {
        [SerializeField] private string _popupType;
        [SerializeField] private bool _disableInput;
        [SerializeField] private RectTransform _body;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private List<ScreenFaderProfile> _screenFaderProfiles;

        protected CompositeDisposable CompositeDisposable { get; private set; }

        protected EventAggregator _eventAggregator;
        private List<ScreenFaderBase> _screenFaders;
        private IInput _input;

        public string PopupType => _popupType;
        public RectTransform Body => _body;
        public CanvasGroup CanvasGroup => _canvasGroup;
        public IEnumerable<ScreenFaderProfile> ScreenFaderProfiles => _screenFaderProfiles;
        public bool IsShow { get; private set; }

        [Inject]
        private void Construct(EventAggregator eventAggregator, IInput input)
        {
            _input = input;
            _eventAggregator = eventAggregator;
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void SetScreenFaders(List<ScreenFaderBase> array)
        {
            _screenFaders = array;
        }

        public async UniTask Show(object data = null)
        {
            if (IsShow)
            {
                return;
            }

            IsShow = true;

            CompositeDisposable = new CompositeDisposable();

            if (_disableInput)
            {
                _input.DisableHandler.Disable().AddTo(CompositeDisposable);
            }

            OnShow(data);

            await UniTask.WhenAll(_screenFaders.Select(fader => fader.Hide(this, true)));
            await UniTask.WhenAll(_screenFaders.Select(fader => fader.Show(this)));

            _eventAggregator.SendEvent(new PopupShowenEvent
            {
                Popup = this
            });
        }

        public async UniTask Hide()
        {
            if (!IsShow)
            {
                return;
            }

            IsShow = false;

            OnHide();

            await UniTask.WhenAll(_screenFaders.Select(fader => fader.Hide(this)));

            _eventAggregator.SendEvent(new PopupHidenEvent
            {
                Popup = this
            });

            CompositeDisposable.Dispose();

            Addressables.ReleaseInstance(gameObject);
        }

        protected abstract void OnShow(object data = null);

        protected abstract void OnHide();
    }
}