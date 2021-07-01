// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Overlays.Settings.Sections.Mf
{
    public class DanceSettings : SettingsSubsection
    {
        protected override string Header => "Osu cursor dance settings";

        [BackgroundDependencyLoader]
        private void load(MConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsEnumDropdown<OsuDanceMover>
                {
                    LabelText = "Dance mover",
                    Current = config.GetBindable<OsuDanceMover>(MSetting.DanceMover)
                },
                new SettingsSlider<float, FramerateSlider>
                {
                    LabelText = "Autoplay framerate",
                    Current = config.GetBindable<float>(MSetting.ReplayFramerate),
                    KeyboardStep = 30f
                },
                new SettingsSlider<float>
                {
                    LabelText = "Spinner start radius",
                    Current = config.GetBindable<float>(MSetting.SpinnerRadiusStart),
                    KeyboardStep = 5f,
                },
                new SettingsSlider<float>
                {
                    LabelText = "Spinner end radius",
                    Current = config.GetBindable<float>(MSetting.SpinnerRadiusEnd),
                    KeyboardStep = 5f,
                },
                new SettingsSlider<float, MultiplierSlider>
                {
                    LabelText = "Slider dance multiplier",
                    Current = config.GetBindable<float>(MSetting.SliderDanceMult),
                    KeyboardStep = 0.1f,
                },
                new SettingsSlider<float, AngleSlider>
                {
                    LabelText = "Angle offset",
                    Current = config.GetBindable<float>(MSetting.AngleOffset),
                    KeyboardStep = 1f / 18f
                },
                new SettingsSlider<float, MultiplierSlider>
                {
                    LabelText = "Jump multiplier",
                    Current = config.GetBindable<float>(MSetting.JumpMulti),
                    KeyboardStep = 0.01f
                },
                new SettingsSlider<float, MultiplierSlider>
                {
                    LabelText = "Next jump multiplier",
                    Current = config.GetBindable<float>(MSetting.NextJumpMulti),
                    KeyboardStep = 0.01f
                },
                new SettingsCheckbox
                {
                    LabelText = "Slider dance",
                    Current = config.GetBindable<bool>(MSetting.SliderDance)
                },
                new SettingsCheckbox
                {
                    LabelText = "Skip stack angles",
                    Current = config.GetBindable<bool>(MSetting.SkipStackAngles)
                },
                new SettingsCheckbox
                {
                    LabelText = "Bounce on edges",
                    Current = config.GetBindable<bool>(MSetting.BorderBounce)
                },
                new SettingsCheckbox
                {
                    LabelText = "Force pippi mover for spinners",
                    Current = config.GetBindable<bool>(MSetting.PippiSpinner)
                },
                new SettingsCheckbox
                {
                    LabelText = "Force pippi mover for streams",
                    Current = config.GetBindable<bool>(MSetting.PippiStream)
                }
            };
        }

        public class MultiplierSlider : OsuSliderBar<float>
        {
            public override LocalisableString TooltipText => Current.Value.ToString("g2") + "x";
        }

        private class AngleSlider : OsuSliderBar<float>
        {
            public override LocalisableString TooltipText => (Current.Value * 180).ToString("g2") + "deg";
        }

        private class FramerateSlider : OsuSliderBar<float>
        {
            public override LocalisableString TooltipText => Current.Value.ToString("g0") + "fps";
        }
    }
}
