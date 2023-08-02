using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace PlayVibe
{
    [CreateAssetMenu(fileName = "New ScreenFaderProfile", menuName = "ScreenFaderProfile", order = 51)]
    public class ScreenFaderProfile : ScriptableObject
    {
        [SerializeField] protected ScreenFaderType _screenFaderType;
        [Space]
        [SerializeField] protected Ease _openEasing;
        [SerializeField] protected float _openFadeDuration = 1;
        [SerializeField] protected bool _useOpen = true;
        [Space]
        [SerializeField] protected Ease _closeEasing = default;
        [SerializeField] protected float _closeFadeDuration = 1;
        [SerializeField] protected bool _useClose = true;
        [Space]
        [ShowIf("IsScale")] 
        [SerializeField] protected ScaleModel _scaleModel;
        [ShowIf("IsMove")] 
        [SerializeField] protected MoveModel _moveModel;

        public bool IsFade => _screenFaderType == ScreenFaderType.FadeIn;
        public bool IsScale => _screenFaderType == ScreenFaderType.ScaleIn;
        public bool IsMove => _screenFaderType == ScreenFaderType.MoveIn;
        
        public ScaleModel Scale => _scaleModel;
        public MoveModel Move => _moveModel;
        
        public ScreenFaderType ScreenFaderType => _screenFaderType;
        public Ease EasingOpen => _openEasing;
        public Ease EasingClose => _closeEasing;
        public float FadeDurationOpen => _openFadeDuration;
        public float FadeDurationClose => _closeFadeDuration;
        public bool UseOpen => _useOpen;
        public bool UseClose => _useClose;

        [Serializable]
        public class ScaleModel
        {
            public Vector3 ShowScaleA = Vector3.zero;
            public Vector3 ShowScaleB = Vector3.one;
            public Vector3 HideScaleA = Vector3.one;
            public Vector3 HideScaleB = Vector3.zero;
        }
        
        [Serializable]
        public class MoveModel
        {
            public MoveInSide OpenDirection;
            public MoveInSide CloseDirection;
        }
    }
}
