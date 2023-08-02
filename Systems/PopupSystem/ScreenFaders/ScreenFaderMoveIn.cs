using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace PlayVibe
{
    public class ScreenFaderMoveIn : ScreenFaderBase
    {
        public ScreenFaderMoveIn(ScreenFaderProfile profile) : base(profile)
        {

        }

        protected enum OpenOrClose
        {
            Open = 0,
            Close = 1
        };
        
        public override async UniTask Show(BasePopup popup, bool immediate = false)
        {
            if (!_profile.UseOpen)
            {
                return;
            }
            
            popup.Body.localPosition = DetermineInitialPosition(OpenOrClose.Open);

            _tweener?.Kill();

            if (!immediate)
            {
                _tweener = popup.Body.DOLocalMove(Vector3.zero, _profile.FadeDurationOpen).SetEase(_profile.EasingOpen);
                await _tweener.AsyncWaitForCompletion().AddTo(CompositeDisposable);
            }
            else
            {
                popup.Body.localPosition = Vector3.zero;
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
                _tweener = popup.Body.DOLocalMove(DetermineInitialPosition(OpenOrClose.Close), _profile.FadeDurationClose).SetEase(_profile.EasingClose);
                await _tweener.AsyncWaitForCompletion().AddTo(CompositeDisposable);
            }
            else
            {
                popup.Body.localPosition = DetermineInitialPosition(OpenOrClose.Close);
            }
        }

        private Vector2 DetermineInitialPosition(OpenOrClose state)
        {
            switch (state == OpenOrClose.Open ? _profile.Move.OpenDirection : _profile.Move.CloseDirection)
            {
                case MoveInSide.Top:
                    return new Vector2(0f, Screen.height * 2);
                case MoveInSide.Down:
                    return new Vector2(0f, -(Screen.height * 2));
                case MoveInSide.Left:
                    return new Vector2(-(Screen.width * 2), 0f);
                case MoveInSide.Right:
                    return new Vector2(Screen.width * 2, 0f);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
