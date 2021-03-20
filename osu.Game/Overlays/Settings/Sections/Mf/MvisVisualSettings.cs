// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Configuration;
using osuTK;

namespace osu.Game.Overlays.Settings.Sections.Mf
{
    public class MvisVisualSettings : SettingsSubsection
    {
        protected override string Header => "Effects";

        private SettingsCheckbox customColourCheckbox;
        private Container resizableContainer;

        [BackgroundDependencyLoader]
        private void load(MConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Show triangles when storyboard is unavaliable",
                    Current = config.GetBindable<bool>(MSetting.MvisEnableBgTriangles),
                },
                new SettingsCheckbox
                {
                    LabelText = "Show particles",
                    Current = config.GetBindable<bool>(MSetting.MvisShowParticles)
                },
                new SettingsSlider<int>
                {
                    LabelText = "Particles count",
                    TransferValueOnCommit = true,
                    Current = config.GetBindable<int>(MSetting.MvisParticleAmount),
                    KeyboardStep = 1,
                },
                new SettingsEnumDropdown<MvisBarType>
                {
                    LabelText = "Bar type",
                    Current = config.GetBindable<MvisBarType>(MSetting.MvisBarType)
                },
                new SettingsSlider<int>
                {
                    LabelText = "Bar count",
                    Current = config.GetBindable<int>(MSetting.MvisVisualizerAmount),
                    KeyboardStep = 1,
                    TransferValueOnCommit = true
                },
                new SettingsSlider<double>
                {
                    LabelText = "Bar width",
                    Current = config.GetBindable<double>(MSetting.MvisBarWidth),
                    KeyboardStep = 0.1f
                },
                new SettingsSlider<int>
                {
                    LabelText = "Bars per visual",
                    Current = config.GetBindable<int>(MSetting.MvisBarsPerVisual),
                    KeyboardStep = 1,
                    TransferValueOnCommit = true
                },
                new SettingsSlider<int>
                {
                    LabelText = "Rotation",
                    KeyboardStep = 1,
                    Current = config.GetBindable<int>(MSetting.MvisRotation)
                },
                customColourCheckbox = new SettingsCheckbox
                {
                    LabelText = "Use custom color",
                    Current = config.GetBindable<bool>(MSetting.MvisUseCustomColour)
                },
                resizableContainer = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeDuration = 200,
                    AutoSizeEasing = Easing.OutQuint,
                    Masking = true,
                    Child = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Children = new Drawable[]
                        {
                            new SettingsSlider<int>
                            {
                                LabelText = "Red",
                                KeyboardStep = 1,
                                Current = config.GetBindable<int>(MSetting.MvisRed)
                            },
                            new SettingsSlider<int>
                            {
                                LabelText = "Green",
                                KeyboardStep = 1,
                                Current = config.GetBindable<int>(MSetting.MvisGreen)
                            },
                            new SettingsSlider<int>
                            {
                                KeyboardStep = 1,
                                LabelText = "Blue",
                                Current = config.GetBindable<int>(MSetting.MvisBlue)
                            }
                        }
                    }
                },
                new SettingsCheckbox
                {
                    LabelText = "Use osu logo visualization",
                    Current = config.GetBindable<bool>(MSetting.MvisUseOsuLogoVisualisation),
                },
            };
        }

        protected override void LoadComplete()
        {
            customColourCheckbox.Current.BindValueChanged(useCustom =>
            {
                if (useCustom.NewValue)
                {
                    resizableContainer.ClearTransforms();
                    resizableContainer.AutoSizeAxes = Axes.Y;
                }
                else
                {
                    resizableContainer.AutoSizeAxes = Axes.None;
                    resizableContainer.ResizeHeightTo(0, 200, Easing.OutQuint);
                }
            }, true);

            resizableContainer.FinishTransforms();
        }
    }
}
