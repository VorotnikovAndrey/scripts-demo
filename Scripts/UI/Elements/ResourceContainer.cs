using DG.Tweening;
using TMPro;
using UnityEngine;

namespace PlayVibe
{
    public class ResourceContainer : MonoBehaviour
    {
        [SerializeField] private string _resourceType;
        [SerializeField] private AbstractNumberToStringConverterScriptableObject _converter;
        [Space]
        [SerializeField] private float _animationDuration = 1f;
        [SerializeField] private Ease _animationEase = Ease.Unset;
        [Space]
        [SerializeField] private TextMeshProUGUI _text;
    
        private Tweener _tweener;
        private int _value;
    
        public int Value
        {
            get => _value;
            private set
            {
                _value = value;
                _text.text = _converter != null ? _converter.Convert(_value) : _value.ToString();
            }
        }

        public string ResourceType => _resourceType;

        public void SetValue(int value, bool force = false)
        {
            if (force)
            {
                _tweener?.Kill();
                Value = value;
            }
            else
            {
                _tweener?.Kill();
                _tweener = DOTween.To(() => Value, x => Value = x, value, _animationDuration).SetEase(_animationEase).OnKill(() => _tweener = null);
            }
        }
    
        private void OnDisable()
        {
            _tweener?.Kill();
        }
    }
}