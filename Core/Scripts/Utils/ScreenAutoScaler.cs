using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class ScreenAutoScaler : MonoBehaviour
    {
        [HideInInspector] [SerializeField] private CanvasScaler canvasScaler = default;

        [SerializeField] private Vector2Int defaultResolution = new Vector2Int(1080, 1920);
        [SerializeField] private bool enable = true;

        #region RefreshDPI

        private static readonly int dpi = 350;

        private static float Width
        {
            get
            {
                _width ??= Screen.width;

                return _width.Value;
            }
        }

        private static float? _width;
        private static float? _height;
        private static Vector2Int _screenSize;

        private static float Height
        {
            get
            {
                _height ??= Screen.height;

                return _height.Value;
            }
        }

        #endregion

       private void OnValidate()
        {
            canvasScaler = GetComponent<CanvasScaler>();
        }

        private void Start()
        {
            if (!enable) return;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            UpdateResolution();
        }

        private void UpdateResolution()
        {
            RefreshDPI();

            float factorX = (float) _screenSize.x / (float) defaultResolution.x;
            float factorY = (float) _screenSize.y / (float) defaultResolution.y;
            float coef = Mathf.Clamp(factorY - factorX, 0, float.MaxValue);

            canvasScaler.scaleFactor = factorY - coef;
        }
        
        private static void RefreshDPI()
        {
            if (Screen.dpi > dpi)
            {
                float value = Screen.dpi / dpi;
                _screenSize = new Vector2Int((int) (Width / value), (int) (Height / value));
                Screen.SetResolution(_screenSize.x, _screenSize.y, true);
            }
            else
            {
                _screenSize = new Vector2Int((int) (Width), (int) (Height));
            }
        }

#if UNITY_EDITOR
        public void UpdateResolutionForScreenShot()
        {
            var screen = GetMainGameViewSize();
            Screen.SetResolution((int)screen.x, (int)screen.y, true);

            float factorX = (float)screen.x / (float)defaultResolution.x;
            float factorY = (float)screen.y / (float)defaultResolution.y;
            float coef = Mathf.Clamp(factorY - factorX, 0, float.MaxValue);

            canvasScaler.scaleFactor = factorY - coef;
        }

        public static Vector2 GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }
#endif
    }
}