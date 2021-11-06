// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Screens.LLin;

namespace osu.Game.Overlays.Settings.Sections.Mf
{
    public class MvisUISettings : SettingsSubsection
    {
        protected override LocalisableString Header => "UI";
        private readonly BindableFloat iR = new BindableFloat();
        private readonly BindableFloat iG = new BindableFloat();
        private readonly BindableFloat iB = new BindableFloat();
        private ColourPreviewer preview;

        [BackgroundDependencyLoader]
        private void load(MConfigManager config)
        {
            config.BindWith(MSetting.MvisInterfaceRed, iR);
            config.BindWith(MSetting.MvisInterfaceGreen, iG);
            config.BindWith(MSetting.MvisInterfaceBlue, iB);

            Children = new Drawable[]
            {
                new SettingsSlider<float>
                {
                    LabelText = "Background blur",
                    Current = config.GetBindable<float>(MSetting.MvisBgBlur),
                    DisplayAsPercentage = true,
                    KeyboardStep = 0.01f,
                },
                new SettingsSlider<float>
                {
                    LabelText = "Idle background Dim",
                    Current = config.GetBindable<float>(MSetting.MvisIdleBgDim),
                    DisplayAsPercentage = true,
                    KeyboardStep = 0.01f,
                },
                new SettingsSlider<float>
                {
                    LabelText = "Opacity",
                    Current = config.GetBindable<float>(MSetting.MvisContentAlpha),
                    DisplayAsPercentage = true,
                    KeyboardStep = 0.01f,
                },
                new SettingsSlider<float>
                {
                    LabelText = "Red",
                    Current = iR,
                    KeyboardStep = 1,
                    TransferValueOnCommit = false
                },
                new SettingsSlider<float>
                {
                    LabelText = "Green",
                    Current = iG,
                    KeyboardStep = 1,
                    TransferValueOnCommit = false
                },
                new SettingsSlider<float>
                {
                    LabelText = "Blue",
                    Current = iB,
                    KeyboardStep = 1,
                    TransferValueOnCommit = false
                },
                preview = new ColourPreviewer()
            };
        }

        protected override void LoadComplete()
        {
            iR.BindValueChanged(_ => updateColor());
            iG.BindValueChanged(_ => updateColor());
            iB.BindValueChanged(_ => updateColor(), true);
        }

        private void updateColor() => preview.UpdateColor(iR.Value, iG.Value, iB.Value);

        private class ColourPreviewer : Container, IHasTooltip
        {
            private readonly CustomColourProvider provider = new CustomColourProvider();
            private Box bg6;
            private Box bg5;
            private Box bg4;
            private Box bg3;
            private Box bg2;
            private Box bg1;
            private Box hl;
            private Box l4;
            private Box l3;
            private Box c2;

            public LocalisableString TooltipText { get; private set; }

            [BackgroundDependencyLoader]
            private void load()
            {
                Height = 50;
                RelativeSizeAxes = Axes.X;
                Padding = new MarginPadding { Horizontal = 15 };
                InternalChildren = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Height = 0.5f,
                        Children = new Drawable[]
                        {
                            bg5 = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.2f
                            },
                            bg4 = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.2f
                            },
                            bg3 = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.2f
                            },
                            bg2 = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.2f
                            },
                            bg1 = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.2f
                            },
                        }
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Height = 0.5f,
                        Margin = new MarginPadding { Top = 25 },
                        Children = new Drawable[]
                        {
                            bg6 = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.2f
                            },
                            hl = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.2f
                            },
                            l4 = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.2f
                            },
                            l3 = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.2f
                            },
                            c2 = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0.2f
                            },
                        }
                    }
                };
            }

            public void UpdateColor(float r, float g, float b)
            {
                provider.UpdateHueColor(r, g, b);

                bg5.Colour = provider.Background5;
                bg4.Colour = provider.Background4;
                bg3.Colour = provider.Background3;
                bg2.Colour = provider.Background2;
                bg1.Colour = provider.Background1;

                bg6.Colour = provider.Background6;
                hl.Colour = provider.Highlight1;
                l4.Colour = provider.Light4;
                l3.Colour = provider.Light3;
                c2.Colour = provider.Content2;

                TooltipText = $"Hue: {(provider.HueColour.Value * 360):#0.00}";
            }
        }
    }
}
