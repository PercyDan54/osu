// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Configuration;

namespace osu.Game.Overlays.Settings.Sections.Mf
{
    public class MvisAudioSettings : SettingsSubsection
    {
        protected override string Header => "Audio";

        [BackgroundDependencyLoader]
        private void load(MfConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsSlider<double>
                {
                    LabelText = "Playback rate",
                    Current = config.GetBindable<double>(MfSetting.MvisMusicSpeed),
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true,
                    TransferValueOnCommit = true
                },
                new SettingsCheckbox
                {
                    LabelText = "Adjust pitch with playback rate",
                    Current = config.GetBindable<bool>(MfSetting.MvisAdjustMusicWithFreq),
                    TooltipText = ""
                },
                new SettingsCheckbox
                {
                    LabelText = "Nightcore beat",
                    Current = config.GetBindable<bool>(MfSetting.MvisEnableNightcoreBeat),
                    TooltipText = ""
                },
                new SettingsCheckbox
                {
                    LabelText = "Play music from collection",
                    Current = config.GetBindable<bool>(MfSetting.MvisPlayFromCollection),
                    TooltipText = ""
                }
            };
        }
    }
}
