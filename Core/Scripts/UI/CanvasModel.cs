using System;
using PopupSystem;
using UnityEngine;

namespace PlayVibe
{
    [Serializable]
    public class CanvasModel
    {
        [SerializeField] private PopupGroup _group;
        [SerializeField] private RectTransform _rectTransform;

        public PopupGroup PopupGroup => _group;
        public RectTransform RectTransform => _rectTransform;
    }
}