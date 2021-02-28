// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Game.Configuration
{
    public class MfConfigManager : IniConfigManager<MfSetting>
    {
        protected override string Filename => "mf.ini";
        public static MfConfigManager Instance { get; private set; }

        public MfConfigManager(Storage storage)
            : base(storage)
        {
            Instance = this;
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            //Other Settings
            Set(MfSetting.UseSayobot, true);

            //UI Settings
            Set(MfSetting.TrianglesEnabled, true);

            //Gameplay Settings
            Set(MfSetting.SamplePlaybackGain, 1f, 0f, 20f);

            //MvisSettings
            Set(MfSetting.MvisParticleAmount, 350, 0, 350);
            Set(MfSetting.MvisContentAlpha, 1f, 0f, 1f);
            Set(MfSetting.MvisBgBlur, 0.1f, 0f, 1f);
            Set(MfSetting.MvisEnableStoryboard, true);
            Set(MfSetting.MvisStoryboardProxy, true);
            Set(MfSetting.MvisUseOsuLogoVisualisation, false);
            Set(MfSetting.MvisIdleBgDim, 0.6f, 0f, 1f);
            Set(MfSetting.MvisEnableBgTriangles, true);
            Set(MfSetting.MvisAdjustMusicWithFreq, false);
            Set(MfSetting.MvisMusicSpeed, 1.0, 0.1, 2.0);
            Set(MfSetting.MvisEnableNightcoreBeat, false);
            Set(MfSetting.MvisPlayFromCollection, false);
            Set(MfSetting.MvisInterfaceRed, 0, 0, 255f);
            Set(MfSetting.MvisInterfaceGreen, 119f, 0, 255f);
            Set(MfSetting.MvisInterfaceBlue, 255f, 0, 255f);

            //Mvis Settings(Upstream)
            Set(MfSetting.MvisShowParticles, true);
            Set(MfSetting.MvisBarType, MvisBarType.Rounded);
            Set(MfSetting.MvisVisualizerAmount, 3, 1, 5);
            Set(MfSetting.MvisBarWidth, 3.0, 1, 20);
            Set(MfSetting.MvisBarsPerVisual, 120, 1, 200);
            Set(MfSetting.MvisRotation, 0, 0, 359);
            Set(MfSetting.MvisUseCustomColour, false);
            Set(MfSetting.MvisRed, 0, 0, 255);
            Set(MfSetting.MvisGreen, 0, 0, 255);
            Set(MfSetting.MvisBlue, 0, 0, 255);

            //Dance settings
            Set(MfSetting.DanceMover, OsuDanceMover.Momentum);
            Set(MfSetting.AngleOffset, 8f / 18f, 0f, 2f, 0.01f);
            Set(MfSetting.JumpMulti, 2f / 3f, 0f, 2f, 0.01f);
            Set(MfSetting.ReplayFramerate, 120f, 15f, 240f, 1f);
            Set(MfSetting.SpinnerRadiusStart, 235f, 10f, 350f, 1f);
            Set(MfSetting.SpinnerRadiusEnd, 15f, 10f, 250f, 1f);
            Set(MfSetting.NextJumpMulti, 2f / 3f, 0f, 2f, 0.01f);
            Set(MfSetting.SliderDance, false);
            Set(MfSetting.SkipStackAngles, true);
            Set(MfSetting.BorderBounce, true);
        }
    }

    public enum MfSetting
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
        SliderDance
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
