﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections.Mf;
using osu.Game.Rulesets.Osu.Configuration;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Osu.UI
{
    public partial class OsuSettingsSubsection : RulesetSettingsSubsection
    {
        protected override LocalisableString Header => "osu!";

        public OsuSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
        }

        [BackgroundDependencyLoader]
        private void load(MConfigManager mConfig)
        {
            var config = (OsuRulesetConfigManager)Config;

            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = RulesetSettingsStrings.SnakingInSliders,
                    Current = config.GetBindable<bool>(OsuRulesetSetting.SnakingInSliders)
                },
                new SettingsCheckbox
                {
                    ClassicDefault = false,
                    LabelText = RulesetSettingsStrings.SnakingOutSliders,
                    Current = config.GetBindable<bool>(OsuRulesetSetting.SnakingOutSliders)
                },
                new SettingsCheckbox
                {
                    LabelText = "Hide 300s",
                    Current = config.GetBindable<bool>(OsuRulesetSetting.NoDraw300)
                },
                new SettingsCheckbox
                {
                    LabelText = RulesetSettingsStrings.CursorTrail,
                    Current = config.GetBindable<bool>(OsuRulesetSetting.ShowCursorTrail)
                },
                new SettingsCheckbox
                {
                    LabelText = "Force long cursor trail",
                    Current = mConfig.GetBindable<bool>(MSetting.CursorTrailForceLong)
                },
                new SettingsCheckbox
                {
                    LabelText = "Cursor trail hue override",
                    Current = mConfig.GetBindable<bool>(MSetting.CursorTrailHueOverride)
                },
                new SettingsCheckbox
                {
                    LabelText = "Rainbow cursor trail",
                    Current = mConfig.GetBindable<bool>(MSetting.CursorTrailRainbow)
                },
                new SettingsSlider<float>
                {
                    LabelText = "Cursor trail hue",
                    Current = mConfig.GetBindable<float>(MSetting.CursorTrailHue)
                },
                new SettingsSlider<double>
                {
                    LabelText = "Cursor trail rainbow frequency",
                    Current = mConfig.GetBindable<double>(MSetting.CursorTrailRainbowFreq)
                },
                new SettingsSlider<float, DanceSettings.MultiplierSlider>
                {
                    LabelText = "Cursor trail size",
                    Current = mConfig.GetBindable<float>(MSetting.CursorTrailSize)
                },
                new SettingsSlider<float, DanceSettings.MultiplierSlider>
                {
                    LabelText = "Cursor trail density",
                    Current = mConfig.GetBindable<float>(MSetting.CursorTrailDensity)
                },
                new SettingsSlider<float>
                {
                    LabelText = "Cursor trail fade duration",
                    Current = mConfig.GetBindable<float>(MSetting.CursorTrailFadeDuration)
                },
                new SettingsEnumDropdown<PlayfieldBorderStyle>
                {
                    LabelText = RulesetSettingsStrings.PlayfieldBorderStyle,
                    Current = config.GetBindable<PlayfieldBorderStyle>(OsuRulesetSetting.PlayfieldBorderStyle),
                },
            };
        }
    }
}
