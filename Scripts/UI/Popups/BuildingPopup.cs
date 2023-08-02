using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PlayVibe
{
    public class BuildingPopup : BasePopup
    {
        [SerializeField] private List<Button> _closeButtons;
        
        private LocationCamera _locationCamera;
        private CameraManager _cameraManager;

        [Inject]
        private void Construct(CameraManager cameraManager)
        {
            _cameraManager = cameraManager;
            _locationCamera = _cameraManager.Current as LocationCamera;
        }
        
        protected override void OnShow(object data = null)
        {
            _closeButtons.ForEach(x =>x.OnClickAsObservable().Subscribe(_ => Hide().Forget()).AddTo(CompositeDisposable));
        }

        protected override void OnHide()
        {
            _locationCamera.SwitchToDefaultState();
        }
    }
}