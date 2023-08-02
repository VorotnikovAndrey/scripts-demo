using System;
using Zenject;

namespace PlayVibe
{
    public class ScreenFaderFactory : PlaceholderFactory<ScreenFaderBase>
    {
        public ScreenFaderBase GetFader(ScreenFaderProfile profile)
        {
            return DetermineFadeType(profile);
        }

        protected ScreenFaderBase DetermineFadeType(ScreenFaderProfile profile)
        {
            switch (profile.ScreenFaderType)
            {
                case ScreenFaderType.FadeIn:
                    return new ScreenFaderFadeIn(profile);
                case ScreenFaderType.ScaleIn:
                    return new ScreenFaderScaleIn(profile);
                case ScreenFaderType.MoveIn:
                    return new ScreenFaderMoveIn(profile);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}