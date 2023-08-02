using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;

namespace PlayVibe
{
    public class ScreenFaderFadeIn : ScreenFaderBase
    {
        public ScreenFaderFadeIn(ScreenFaderProfile profile) : base(profile)
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
                _tweener = popup.CanvasGroup.DOFade(1f, _profile.FadeDurationOpen).SetEase(_profile.EasingOpen);
                await _tweener.AsyncWaitForCompletion().AddTo(CompositeDisposable);
            }
            else
            {
                popup.CanvasGroup.alpha = 1f;
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
                _tweener = popup.CanvasGroup.DOFade(0f, _profile.FadeDurationClose).SetEase(_profile.EasingClose);
                await _tweener.AsyncWaitForCompletion().AddTo(CompositeDisposable);
            }
            else
            {
                popup.CanvasGroup.alpha = 0f;
            }
        }
    }
}