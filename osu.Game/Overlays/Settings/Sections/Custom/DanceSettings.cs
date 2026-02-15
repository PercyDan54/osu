// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics.UserInterfaceV2;

namespace osu.Game.Overlays.Settings.Sections.Custom
{
    public partial class DanceSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "osu! Cursor Dance Settings";

        [BackgroundDependencyLoader]
        private void load(CustomConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsItemV2(new FormEnumDropdown<OsuDanceMover>
                {
                    Caption = "Dance Mover",
                    Current = config.GetBindable<OsuDanceMover>(CustomSetting.DanceMover)
                }),
                new SettingsItemV2(new FormEnumDropdown<OsuDanceSpinnerMover>
                {
                    Caption = "Dance Spinner Mover",
                    Current = config.GetBindable<OsuDanceSpinnerMover>(CustomSetting.DanceSpinnerMover)
                }),
                new SettingsItemV2(new FormSliderBar<double>
                {
                    Caption = "Replay framerate",
                    Current = config.GetBindable<double>(CustomSetting.ReplayFramerate),
                    KeyboardStep = 10,
                    LabelFormat = v => $"{v:G0} fps"
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = "Change replay framerate for spinners",
                    HintText = "Makes spinner movements smoother, but may not be played back on Stable",
                    Current = config.GetBindable<bool>(CustomSetting.SpinnerChangeFramerate)
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Spinner start radius",
                    Current = config.GetBindable<float>(CustomSetting.SpinnerRadiusStart),
                    KeyboardStep = 5f,
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Spinner end radius",
                    Current = config.GetBindable<float>(CustomSetting.SpinnerRadiusEnd),
                    KeyboardStep = 5f,
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Angle offset",
                    Current = config.GetBindable<float>(CustomSetting.AngleOffset),
                    KeyboardStep = 1f / 18f,
                    LabelFormat = v => $"{v * 180:0.##} deg"
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = "Jump multiplier",
                    Current = config.GetBindable<float>(CustomSetting.JumpMult),
                    KeyboardStep = 0.01f,
                    LabelFormat = v => $"{v:0.##}x"
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = "Slider dance",
                    Current = config.GetBindable<bool>(CustomSetting.SliderDance)
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = "Skip stack angles",
                    Current = config.GetBindable<bool>(CustomSetting.SkipStackAngles)
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = "Bounce on edges",
                    Current = config.GetBindable<bool>(CustomSetting.BorderBounce)
                }),
            };
        }
    }
}
