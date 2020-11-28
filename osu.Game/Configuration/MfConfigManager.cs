// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.ComponentModel;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Game.Configuration
{
    public class MfConfigManager : IniConfigManager<MfSetting>
    {
        protected override string Filename => "mf.ini";

        public MfConfigManager(Storage storage)
            : base(storage)
        {
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            //Other Settings
            Set(MfSetting.UseSayobot, true);

            //UI Settings
            Set(MfSetting.OptUI, true);
            Set(MfSetting.TrianglesEnabled, true);
            Set(MfSetting.SongSelectBgBlur, 0.2f, 0f, 1f);

            //Intro Settings
            Set(MfSetting.IntroLoadDirectToSongSelect, false);

            //Gameplay Settings
            Set(MfSetting.SamplePlaybackGain, 1f, 0f, 20f);

            //MvisSettings
            Set(MfSetting.MvisParticleAmount, 350, 0, 350);
            Set(MfSetting.MvisContentAlpha, 1f, 0f, 1f);
            Set(MfSetting.MvisBgBlur, 0.1f, 0f, 1f);
            Set(MfSetting.MvisEnableStoryboard, true);
            Set(MfSetting.MvisUseOsuLogoVisualisation, false);
            Set(MfSetting.MvisIdleBgDim, 0.6f, 0f, 1f);
            Set(MfSetting.MvisEnableBgTriangles, true);
            Set(MfSetting.MvisEnableSBOverlayProxy, true);
            Set(MfSetting.MvisAdjustMusicWithFreq, true);
            Set(MfSetting.MvisMusicSpeed, 1.0, 0.1, 2.0);
            Set(MfSetting.MvisEnableNightcoreBeat, false);
            Set(MfSetting.MvisPlayFromCollection, false);
            Set(MfSetting.MvisInterfaceRed, value: 0, 0, 255f);
            Set(MfSetting.MvisInterfaceGreen, value: 119f, 0, 255f);
            Set(MfSetting.MvisInterfaceBlue, value: 255f, 0, 255f);

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

            //File Select Filter
            Set(MfSetting.FileFilter, FileFilterType.All);
        }
    }

    public enum MfSetting
    {
        OptUI,
        TrianglesEnabled,
        UseSayobot,
        MvisParticleAmount,
        MvisBgBlur,
        MvisUseOsuLogoVisualisation,
        MvisEnableStoryboard,
        MvisIdleBgDim,
        MvisContentAlpha,
        MvisEnableBgTriangles,
        MvisEnableSBOverlayProxy,
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
        SongSelectBgBlur,
        IntroLoadDirectToSongSelect,
        FileFilter,
    }

    public enum MvisBarType
    {
        Basic,
        Rounded,
        Fall
    }

    public enum FileFilterType
    {
        [Description("皮肤文件")]
        Skin,
        [Description("谱面文件")]
        Beatmap,
        [Description("回放文件")]
        Replay,
        [Description("全部")]
        All
    }
}
