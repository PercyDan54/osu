using Mvis.Plugin.RulesetPanel.Config;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Overlays.Settings;
using osu.Game.Screens.Mvis.Plugins;
using osu.Game.Screens.Mvis.Plugins.Config;
using osuTK;

namespace Mvis.Plugin.RulesetPanel.UI
{
    public class RulesetPanelSettings : PluginSettingsSubSection
    {
        private Container resizableContainer;
        private SettingsCheckbox customColourCheckbox;

        public RulesetPanelSettings(MvisPlugin plugin)
            : base(plugin)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (RulesetPanelConfigManager)ConfigManager;

            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Enable Mvis Panel",
                    Current = config.GetBindable<bool>(RulesetPanelSetting.EnableRulesetPanel)
                },
                new SettingsSlider<float>
                {
                    LabelText = "Opacity",
                    Current = config.GetBindable<float>(RulesetPanelSetting.IdleAlpha),
                    DisplayAsPercentage = true,
                    KeyboardStep = 0.01f,
                },
                new SettingsCheckbox
                {
                    LabelText = "Show particles",
                    Current = config.GetBindable<bool>(RulesetPanelSetting.ShowParticles)
                },
                new SettingsSlider<int>
                {
                    LabelText = "Particle amount",
                    TransferValueOnCommit = true,
                    Current = config.GetBindable<int>(RulesetPanelSetting.ParticleAmount),
                    KeyboardStep = 1,
                },
                new SettingsEnumDropdown<MvisBarType>
                {
                    LabelText = "Bar type",
                    Current = config.GetBindable<MvisBarType>(RulesetPanelSetting.BarType)
                },
                new SettingsSlider<int>
                {
                    LabelText = "Bar count",
                    Current = config.GetBindable<int>(RulesetPanelSetting.VisualizerAmount),
                    KeyboardStep = 1,
                    TransferValueOnCommit = true
                },
                new SettingsSlider<double>
                {
                    LabelText = "Bar width",
                    Current = config.GetBindable<double>(RulesetPanelSetting.BarWidth),
                    KeyboardStep = 0.1f
                },
                new SettingsSlider<int>
                {
                    LabelText = "Bar density",
                    Current = config.GetBindable<int>(RulesetPanelSetting.BarsPerVisual),
                    KeyboardStep = 1,
                    TransferValueOnCommit = true
                },
                new SettingsSlider<int>
                {
                    LabelText = "Rotation",
                    KeyboardStep = 1,
                    Current = config.GetBindable<int>(RulesetPanelSetting.Rotation)
                },
                customColourCheckbox = new SettingsCheckbox
                {
                    LabelText = "Use custom color",
                    Current = config.GetBindable<bool>(RulesetPanelSetting.UseCustomColour)
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
                                Current = config.GetBindable<int>(RulesetPanelSetting.Red)
                            },
                            new SettingsSlider<int>
                            {
                                LabelText = "Green",
                                KeyboardStep = 1,
                                Current = config.GetBindable<int>(RulesetPanelSetting.Green)
                            },
                            new SettingsSlider<int>
                            {
                                KeyboardStep = 1,
                                LabelText = "Blue",
                                Current = config.GetBindable<int>(RulesetPanelSetting.Blue)
                            }
                        }
                    }
                },
                new SettingsCheckbox
                {
                    LabelText = "Use osu! logo visualization",
                    Current = config.GetBindable<bool>(RulesetPanelSetting.UseOsuLogoVisualisation),
                }
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
