using Mvis.Plugin.Sandbox.Config;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Overlays.Settings;
using osu.Game.Screens.Mvis.Plugins;
using osu.Game.Screens.Mvis.Plugins.Config;
using osuTK;

namespace Mvis.Plugin.Sandbox.UI
{
    public class SandboxSettings : PluginSettingsSubSection
    {
        private FillFlowContainer typeASettings;
        private FillFlowContainer typeBSettings;

        private readonly Bindable<VisualizerLayout> layoutType = new Bindable<VisualizerLayout>();

        public SandboxSettings(MvisPlugin plugin)
            : base(plugin)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (SandboxConfigManager)ConfigManager;

            config.BindWith(SandboxSetting.VisualizerLayout, layoutType);

            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Enable Mvis panel",
                    Current = config.GetBindable<bool>(SandboxSetting.EnableRulesetPanel)
                },
                new SettingsSlider<float>
                {
                    LabelText = "Alpha when idle",
                    Current = config.GetBindable<float>(SandboxSetting.IdleAlpha),
                    DisplayAsPercentage = true,
                    KeyboardStep = 0.01f,
                },
                new SettingsCheckbox
                {
                    LabelText = "Show particles",
                    Current = config.GetBindable<bool>(SandboxSetting.ShowParticles)
                },
                new SettingsSlider<int>
                {
                    LabelText = "Particle amount",
                    TransferValueOnCommit = true,
                    Current = config.GetBindable<int>(SandboxSetting.ParticleCount),
                    KeyboardStep = 1,
                },
                new SettingsEnumDropdown<VisualizerLayout>
                {
                    LabelText = "Layout type",
                    Current = layoutType
                },
                typeASettings = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Masking = true,
                    Spacing = new Vector2(0, 8),
                    Children = new Drawable[]
                    {
                        new SettingsSlider<int>
                        {
                            LabelText = "Radius",
                            KeyboardStep = 1,
                            Current = config.GetBindable<int>(SandboxSetting.Radius)
                        },
                        new SettingsEnumDropdown<CircularBarType>
                        {
                            LabelText = "Bar Type",
                            Current = config.GetBindable<CircularBarType>(SandboxSetting.CircularBarType)
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Rotation",
                            KeyboardStep = 1,
                            Current = config.GetBindable<int>(SandboxSetting.Rotation)
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Decay",
                            Current = config.GetBindable<int>(SandboxSetting.DecayA),
                            KeyboardStep = 1
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Height multiplier",
                            Current = config.GetBindable<int>(SandboxSetting.MultiplierA),
                            KeyboardStep = 1
                        },
                        new SettingsCheckbox
                        {
                            LabelText = "Symmetry",
                            Current = config.GetBindable<bool>(SandboxSetting.Symmetry)
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Smoothness",
                            Current = config.GetBindable<int>(SandboxSetting.SmoothnessA),
                            KeyboardStep = 1
                        },
                        new SettingsSlider<double>
                        {
                            LabelText = "Bar width",
                            Current = config.GetBindable<double>(SandboxSetting.BarWidthA),
                            KeyboardStep = 0.1f
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Visualizer amount",
                            Current = config.GetBindable<int>(SandboxSetting.VisualizerAmount),
                            KeyboardStep = 1,
                            TransferValueOnCommit = true
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Bars per visual",
                            Current = config.GetBindable<int>(SandboxSetting.BarsPerVisual),
                            KeyboardStep = 1,
                            TransferValueOnCommit = true
                        }
                    }
                },
                typeBSettings = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(0, 8),
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new SettingsSlider<int>
                        {
                            LabelText = "Decay",
                            Current = config.GetBindable<int>(SandboxSetting.DecayB),
                            KeyboardStep = 1
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Height multiplier",
                            Current = config.GetBindable<int>(SandboxSetting.MultiplierB),
                            KeyboardStep = 1
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Smoothness",
                            Current = config.GetBindable<int>(SandboxSetting.SmoothnessB),
                            KeyboardStep = 1
                        },
                        new SettingsSlider<double>
                        {
                            LabelText = "Bar width",
                            Current = config.GetBindable<double>(SandboxSetting.BarWidthB),
                            KeyboardStep = 0.1f
                        },
                        new SettingsSlider<int>
                        {
                            LabelText = "Bar count",
                            Current = config.GetBindable<int>(SandboxSetting.BarCountB),
                            KeyboardStep = 0.1f
                        },
                        new SettingsEnumDropdown<LinearBarType>
                        {
                            LabelText = "Bar type",
                            Current = config.GetBindable<LinearBarType>(SandboxSetting.LinearBarType)
                        },
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            layoutType.BindValueChanged(v =>
            {
                switch (v.NewValue)
                {
                    case VisualizerLayout.Empty:
                        typeASettings.FadeOut();
                        typeBSettings.FadeOut();
                        break;

                    case VisualizerLayout.TypeA:
                        typeASettings.FadeIn();
                        typeBSettings.FadeOut();
                        break;

                    case VisualizerLayout.TypeB:
                        typeASettings.FadeOut();
                        typeBSettings.FadeIn();
                        break;
                }
            }, true);
            base.LoadComplete();
        }
    }
}
