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
        private void load(MConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsSlider<double>
                {
                    LabelText = "Playback rate",
                    Current = config.GetBindable<double>(MSetting.MvisMusicSpeed),
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true,
                    TransferValueOnCommit = true
                },
                new SettingsCheckbox
                {
                    LabelText = "Adjust pitch with playback rate",
                    Current = config.GetBindable<bool>(MSetting.MvisAdjustMusicWithFreq),
                    TooltipText = ""
                },
                new SettingsCheckbox
                {
                    LabelText = "Nightcore beat",
                    Current = config.GetBindable<bool>(MSetting.MvisEnableNightcoreBeat),
                    TooltipText = ""
                },
                new SettingsCheckbox
                {
                    LabelText = "Play music from collection",
                    Current = config.GetBindable<bool>(MSetting.MvisPlayFromCollection),
                    TooltipText = ""
                }
            };
        }
    }
}
