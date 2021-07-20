// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Configuration;

namespace osu.Game.Overlays.Settings.Sections.Mf
{
    public class MvisAudioSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "Audio";

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
                },
                new SettingsCheckbox
                {
                    LabelText = "Nightcore beat",
                    Current = config.GetBindable<bool>(MSetting.MvisEnableNightcoreBeat),
                }
            };
        }
    }
}
