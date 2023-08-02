using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;

namespace PlayVibe
{
    [Serializable]
    public class ScreenFaderBase
    {
        protected ScreenFaderProfile _profile;
        protected Tweener _tweener;
        
        public CompositeDisposable CompositeDisposable { get; } = new();

        protected ScreenFaderBase(ScreenFaderProfile profile)
        {
            _profile = profile;
        }

        public virtual UniTask Show(BasePopup popup, bool immediate = false)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask Hide(BasePopup popup, bool immediate = false)
        {
            return UniTask.CompletedTask;
        }

        private void OnDestroy()
        {
            _tweener?.Kill();
            CompositeDisposable.Dispose();
        }
    }
}
