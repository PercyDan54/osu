// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Game.Configuration
{
    public class MConfigManager : IniConfigManager<MSetting>
    {
        protected override string Filename => "mf.ini";
        public static MConfigManager Instance { get; private set; }

        public MConfigManager(Storage storage)
            : base(storage)
        {
            Instance = this;
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            //Other Settings
            SetDefault(MSetting.UseSayobot, true);

            //UI Settings
            SetDefault(MSetting.TrianglesEnabled, true);

            //Gameplay Settings
            SetDefault(MSetting.SamplePlaybackGain, 1f, 0f, 20f);

            //MvisSettings
            SetDefault(MSetting.MvisParticleAmount, 350, 0, 350);
            SetDefault(MSetting.MvisContentAlpha, 1f, 0f, 1f);
            SetDefault(MSetting.MvisBgBlur, 0.1f, 0f, 1f);
            SetDefault(MSetting.MvisEnableStoryboard, true);
            SetDefault(MSetting.MvisStoryboardProxy, true);
            SetDefault(MSetting.MvisUseOsuLogoVisualisation, false);
            SetDefault(MSetting.MvisIdleBgDim, 0.6f, 0f, 1f);
            SetDefault(MSetting.MvisEnableBgTriangles, true);
            SetDefault(MSetting.MvisAdjustMusicWithFreq, false);
            SetDefault(MSetting.MvisMusicSpeed, 1.0, 0.1, 2.0);
            SetDefault(MSetting.MvisEnableNightcoreBeat, false);
            SetDefault(MSetting.MvisPlayFromCollection, false);
            SetDefault(MSetting.MvisInterfaceRed, 0, 0, 255f);
            SetDefault(MSetting.MvisInterfaceGreen, 119f, 0, 255f);
            SetDefault(MSetting.MvisInterfaceBlue, 255f, 0, 255f);

            //Mvis Settings(Upstream)
            SetDefault(MSetting.MvisShowParticles, true);
            SetDefault(MSetting.MvisBarType, MvisBarType.Rounded);
            SetDefault(MSetting.MvisVisualizerAmount, 3, 1, 5);
            SetDefault(MSetting.MvisBarWidth, 3.0, 1, 20);
            SetDefault(MSetting.MvisBarsPerVisual, 120, 1, 200);
            SetDefault(MSetting.MvisRotation, 0, 0, 359);
            SetDefault(MSetting.MvisUseCustomColour, false);
            SetDefault(MSetting.MvisRed, 0, 0, 255);
            SetDefault(MSetting.MvisGreen, 0, 0, 255);
            SetDefault(MSetting.MvisBlue, 0, 0, 255);

            //Dance settings
            SetDefault(MSetting.DanceMover, OsuDanceMover.Momentum);
            SetDefault(MSetting.AngleOffset, 0.45f, 0f, 2f, 0.01f);
            SetDefault(MSetting.JumpMulti, 0.5f, 0f, 2f, 0.01f);
            SetDefault(MSetting.NextJumpMulti, 0.25f, 0f, 2f, 0.01f);
            SetDefault(MSetting.ReplayFramerate, 120f, 15f, 240f, 1f);
            SetDefault(MSetting.SpinnerRadiusStart, 235f, 10f, 350f, 1f);
            SetDefault(MSetting.SpinnerRadiusEnd, 15f, 10f, 250f, 1f);
            SetDefault(MSetting.SkipStackAngles, true);
            SetDefault(MSetting.BorderBounce, true);

            //Cursor settings
            SetDefault(MSetting.CursorTrailHueOverride, false);
            SetDefault(MSetting.CursorTrailHueShift, false);
            SetDefault(MSetting.CursorTrailHue, .5f, 0.1f, 1.0f, 0.05f);
            SetDefault(MSetting.CursorTrailHueSpeed, 10f, 5f, 50.0f, 1f);
            SetDefault(MSetting.CursorTrailFadeDuration, 300, 100, 5000, 50);
            SetDefault(MSetting.CursorTrailSize, 1f, 1f, 15.0f, 0.5f);
            SetDefault(MSetting.CursorTrailDensity, 1f, 0.5f, 10.0f, 0.5f);
        }
    }

    public enum MSetting
    {
        TrianglesEnabled,
        UseSayobot,
        MvisParticleAmount,
        MvisBgBlur,
        MvisUseOsuLogoVisualisation,
        MvisEnableStoryboard,
        MvisIdleBgDim,
        MvisContentAlpha,
        MvisEnableBgTriangles,
        MvisStoryboardProxy,
        MvisShowParticles,
        MvisVisualizerAmount,
        MvisBarWidth,
        MvisBarsPerVisual,
        MvisBarType,
        MvisRotation,
        MvisUseCustomColour,
        MvisRed,
        MvisGreen,
        MvisBlue,
        MvisMusicSpeed,
        MvisAdjustMusicWithFreq,
        MvisEnableNightcoreBeat,
        MvisPlayFromCollection,
        MvisInterfaceRed,
        MvisInterfaceGreen,
        MvisInterfaceBlue,
        SamplePlaybackGain,
        ReplayFramerate,
        SpinnerRadiusStart,
        SpinnerRadiusEnd,
        DanceMover,
        AngleOffset,
        JumpMulti,
        NextJumpMulti,
        BorderBounce,
        SkipStackAngles,
        CursorTrailDensity,
        CursorTrailHueShift,
        CursorTrailHueSpeed,
        CursorTrailFadeDuration,
        CursorTrailSize,
        CursorTrailHue,
        CursorTrailHueOverride
    }

    public enum MvisBarType
    {
        Basic,
        Rounded,
        Fall
    }

    public enum OsuDanceMover
    {
        HalfCircle,
        Flower,
        Momentum
    }
}
