// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics.UserInterfaceV2;

namespace osu.Game.Overlays.Settings.Sections.Custom
{
    public partial class DanceBezierMoverSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "Bezier Mover Settings";

        [BackgroundDependencyLoader]
        private void load(CustomConfigManager config)
        {
            Children =
            [
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Aggressiveness",
                    Current = config.GetBindable<float>(CustomSetting.BezierAggressiveness),
                    KeyboardStep = 0.1f
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Slider aggressiveness",
                    Current = config.GetBindable<float>(CustomSetting.BezierSliderAggressiveness),
                    KeyboardStep = 0.5f
                }),
            ];
        }
    }
}
