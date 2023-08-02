using PopupSystem;

namespace PlayVibe
{
    public class PopupOptions
    {
        public object Data { get; set; } = null;
        public string PopupType { get; set; } = string.Empty;
        public PopupGroup PopupGroup { get; set; } = PopupGroup.Gameplay;
    }
}
