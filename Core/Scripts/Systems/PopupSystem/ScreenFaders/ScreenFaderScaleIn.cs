using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace PlayVibe
{
    public class ScreenFaderScaleIn : ScreenFaderBase
    {
        public ScreenFaderScaleIn(ScreenFaderProfile profile) : base(profile)
        {

        }

        public override async UniTask Show(BasePopup popup, bool immediate = false)
        {
            if (!_profile.UseOpen)
            {
                return;
            }
            
            _tweener?.Kill();

            if (!immediate)
            {
                popup.Body.localScale = _profile.Scale.ShowScaleA;
                _tweener = popup.Body.DOScale(_profile.Scale.ShowScaleB, _profile.FadeDurationOpen).SetEase(_profile.EasingOpen);
                await _tweener.AsyncWaitForCompletion().AddTo(CompositeDisposable);
            }
            else
            {
                popup.Body.localScale = _profile.Scale.ShowScaleB;
            }
        }

        public override async UniTask Hide(BasePopup popup, bool immediate = false)
        {
            if (!_profile.UseClose)
            {
                return;
            }
            
            _tweener?.Kill();

            if (!immediate)
            {
                popup.Body.localScale = _profile.Scale.HideScaleA;
                _tweener = popup.Body.DOScale(_profile.Scale.HideScaleB, _profile.FadeDurationClose).SetEase(_profile.EasingClose);
                await _tweener.AsyncWaitForCompletion().AddTo(CompositeDisposable);
            }
            else
            {
                popup.Body.localScale = Vector3.zero;
            }
        }
    }
}
