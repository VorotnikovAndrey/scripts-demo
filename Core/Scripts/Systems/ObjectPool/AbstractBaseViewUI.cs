using UnityEngine;

namespace Utils.ObjectPool
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class AbstractBaseViewUI : AbstractBaseView
    {
        private RectTransform _rectTransform;
        public RectTransform RectTransform 
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public override void SetParent(Transform parent)
        {
            base.SetParent(parent);

            var anchoredPos = RectTransform.anchoredPosition3D;
            anchoredPos.z = 0;
            RectTransform.anchoredPosition3D = anchoredPos;
            RectTransform.localScale = Vector3.one;
        }
    }
}