// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics.UserInterfaceV2;

namespace osu.Game.Overlays.Settings.Sections.Custom
{
    public partial class DanceMomentumMoverSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "Linear Mover Settings";

        [BackgroundDependencyLoader]
        private void load(CustomConfigManager config)
        {
            Children =
            [
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Restrict angle",
                    Current = config.GetBindable<float>(CustomSetting.RestrictAngle),
                    KeyboardStep = 1,
                    LabelFormat = v => $"{v:0.##} deg"
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Restrict angle add",
                    Current = config.GetBindable<float>(CustomSetting.RestrictAngleAdd),
                    KeyboardStep = 1,
                    LabelFormat = v => $"{v:0.##} deg"
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Restrict angle sub",
                    Current = config.GetBindable<float>(CustomSetting.RestrictAngleSub),
                    KeyboardStep = 1,
                    LabelFormat = v => $"{v:0.##} deg"
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Stream multiplier",
                    Current = config.GetBindable<float>(CustomSetting.StreamMult),
                    KeyboardStep = 0.01f,
                    LabelFormat = v => $"{v:0.##}x"
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Duration multiplier",
                    Current = config.GetBindable<float>(CustomSetting.DurationMult),
                    KeyboardStep = 0.01f,
                    LabelFormat = v => $"{v:0.##}x"
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Duration multiplier trigger",
                    Current = config.GetBindable<float>(CustomSetting.DurationTrigger),
                    KeyboardStep = 50f
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Stream area",
                    Current = config.GetBindable<float>(CustomSetting.StreamArea),
                    KeyboardStep = 5f
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Minimum stream distance",
                    Current = config.GetBindable<float>(CustomSetting.StreamMinimum)
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Maximum stream distance",
                    Current = config.GetBindable<float>(CustomSetting.StreamMaximum)
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Bounce on equal pos",
                    Current = config.GetBindable<float>(CustomSetting.EqualPosBounce)
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = "Restrict invert",
                    Current = config.GetBindable<bool>(CustomSetting.RestrictInvert)
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = "Stream restrict",
                    Current = config.GetBindable<bool>(CustomSetting.StreamRestrict)
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = "Slider predict",
                    Current = config.GetBindable<bool>(CustomSetting.SliderPredict)
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = "Interpolate angles",
                    Current = config.GetBindable<bool>(CustomSetting.InterpolateAngles)
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = "Invert angle interpolation",
                    Current = config.GetBindable<bool>(CustomSetting.InvertAngleInterpolation)
                }),
            ];
        }
    }
}
