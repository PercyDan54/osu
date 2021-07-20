// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections.Mf;
using osu.Game.Rulesets.Osu.Configuration;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Osu.UI
{
    public class OsuSettingsSubsection : RulesetSettingsSubsection
    {
        protected override LocalisableString Header => "osu!";

        public OsuSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (OsuRulesetConfigManager)Config;
            var mconfig = MConfigManager.Instance;

            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Snaking in sliders",
                    Current = config.GetBindable<bool>(OsuRulesetSetting.SnakingInSliders)
                },
                new SettingsCheckbox
                {
                    LabelText = "Snaking out sliders",
                    Current = config.GetBindable<bool>(OsuRulesetSetting.SnakingOutSliders)
                },
                new SettingsCheckbox
                {
                    LabelText = "Cursor trail",
                    Current = config.GetBindable<bool>(OsuRulesetSetting.ShowCursorTrail)
                },
                new SettingsCheckbox
                {
                    LabelText = "Cursor trail hue override",
                    Current = mconfig.GetBindable<bool>(MSetting.CursorTrailHueOverride)
                },
                new SettingsCheckbox
                {
                    LabelText = "Cursor trail hue shifting",
                    Current = mconfig.GetBindable<bool>(MSetting.CursorTrailHueShift)
                },
                new SettingsSlider<float>
                {
                    LabelText = "Cursor trail hue",
                    Current = mconfig.GetBindable<float>(MSetting.CursorTrailHue)
                },
                new SettingsSlider<float>
                {
                    LabelText = "Cursor trail hue speed",
                    Current = mconfig.GetBindable<float>(MSetting.CursorTrailHueSpeed)
                },
                new SettingsSlider<float, DanceSettings.MultiplierSlider>
                {
                    LabelText = "Cursor trail size",
                    Current = mconfig.GetBindable<float>(MSetting.CursorTrailSize)
                },
                new SettingsSlider<float, DanceSettings.MultiplierSlider>
                {
                    LabelText = "Cursor trail density",
                    Current = mconfig.GetBindable<float>(MSetting.CursorTrailDensity)
                },
                new SettingsSlider<float>
                {
                    LabelText = "Cursor trail fade duration",
                    Current = mconfig.GetBindable<float>(MSetting.CursorTrailFadeDuration)
                },
                new SettingsEnumDropdown<PlayfieldBorderStyle>
                {
                    LabelText = "Playfield border style",
                    Current = config.GetBindable<PlayfieldBorderStyle>(OsuRulesetSetting.PlayfieldBorderStyle),
                }
            };
        }
    }
}
