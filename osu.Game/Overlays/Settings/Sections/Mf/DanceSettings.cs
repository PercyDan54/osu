// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Configuration;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Overlays.Settings.Sections.Mf
{
    public class DanceSettings : SettingsSubsection
    {
        protected override string Header => "Osu cursor dance settings";

        [BackgroundDependencyLoader]
        private void load(MfConfigManager config)
        {
            Children = new Drawable[]
            {
               new SettingsSlider<float, FramerateSlider>
                {
                    LabelText = "Autoplay framerate",
                    Current = config.GetBindable<float>(MfSetting.ReplayFramerate),
                    KeyboardStep = 30f
                },
                new SettingsSlider<float>
                {
                    LabelText = "Spinner start radius",
                    Current = config.GetBindable<float>(MfSetting.SpinnerRadiusStart),
                    KeyboardStep = 5f,
                },
                new SettingsSlider<float>
                {
                    LabelText = "Spinner end radius",
                    Current = config.GetBindable<float>(MfSetting.SpinnerRadiusEnd),
                    KeyboardStep = 5f,
                },
                new SettingsEnumDropdown<OsuDanceMover>
                {
                    LabelText = "Dance mover",
                    Current = config.GetBindable<OsuDanceMover>(MfSetting.DanceMover)
                },
                new SettingsSlider<float, AngleSlider>
                {
                    LabelText = "Angle offset",
                    Current = config.GetBindable<float>(MfSetting.AngleOffset),
                    KeyboardStep = 1f / 18f
                },
                new SettingsSlider<float, MultiplierSlider>
                {
                    LabelText = "Jump multiplier",
                    Current = config.GetBindable<float>(MfSetting.JumpMulti),
                    KeyboardStep = 1f / 12f
                },
                new SettingsSlider<float, MultiplierSlider>
                {
                    LabelText = "Next jump multiplier",
                    Current = config.GetBindable<float>(MfSetting.NextJumpMulti),
                    KeyboardStep = 1f / 12f
                },
                new SettingsCheckbox
                {
                    LabelText = "Slider dance",
                    Current = config.GetBindable<bool>(MfSetting.SliderDance)
                },
                new SettingsCheckbox
                {
                    LabelText = "Skip stack angles",
                    Current = config.GetBindable<bool>(MfSetting.SkipStackAngles)
                },
                new SettingsCheckbox
                {
                    LabelText = "Bounce on edges",
                    Current = config.GetBindable<bool>(MfSetting.BorderBounce)
                },
            };
        }
        private class MultiplierSlider : OsuSliderBar<float>
        {
            public override string TooltipText => Current.Value.ToString("N3") + "x";
        }

        private class AngleSlider : OsuSliderBar<float>
        {
            public override string TooltipText => (Current.Value * 180).ToString("N2") + "deg";
        }

        private class FramerateSlider : OsuSliderBar<float>
        {
            public override string TooltipText => Current.Value.ToString("N0") + "fps";
        }
    }
}
