using System.Collections.Generic;
using Mvis.Plugin.Sandbox.Config;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Screens.Mvis.Plugins;
using osu.Game.Screens.Mvis.Plugins.Config;
using osu.Game.Screens.Mvis.SideBar.Settings.Items;

namespace Mvis.Plugin.Sandbox.UI
{
    public class RulesetPanelSidebarSection : PluginSidebarSettingsSection
    {
        public RulesetPanelSidebarSection(MvisPlugin plugin)
            : base(plugin)
        {
            Title = "Sandbox";
        }

        public override int Columns => 5;

        private readonly Bindable<VisualizerLayout> layoutType = new Bindable<VisualizerLayout>();

        private readonly List<Drawable> typeAItems = new List<Drawable>();
        private readonly List<Drawable> typeBItems = new List<Drawable>();

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (SandboxConfigManager)ConfigManager;

            config.BindWith(SandboxSetting.VisualizerLayout, layoutType);

            AddRange(new Drawable[]
            {
                new SettingsTogglePiece
                {
                    Description = "Enable Mvis panel",
                    Bindable = config.GetBindable<bool>(SandboxSetting.EnableRulesetPanel)
                },
                new SettingsSliderPiece<float>
                {
                    Description = "Alpha when idle",
                    Bindable = config.GetBindable<float>(SandboxSetting.IdleAlpha),
                    DisplayAsPercentage = true
                },
                new SettingsTogglePiece
                {
                    Description = "Show particles",
                    Bindable = config.GetBindable<bool>(SandboxSetting.ShowParticles)
                },
                new SettingsSliderPiece<int>
                {
                    Description = "Particle amount",
                    TransferValueOnCommit = true,
                    Bindable = config.GetBindable<int>(SandboxSetting.ParticleCount)
                },
                new SettingsEnumPiece<VisualizerLayout>
                {
                    Description = "Layout type",
                    Bindable = layoutType
                },
                new SettingsTogglePiece
                {
                    Description = "Show beatmap info",
                    Bindable = config.GetBindable<bool>(SandboxSetting.ShowBeatmapInfo)
                },
            });

            //workaround: PluginSidebarSettingsSection的grid

            typeAItems.AddRange(new SettingsPieceBasePanel[]
            {
                new SettingsSliderPiece<int>
                {
                    Description = "Radius",
                    Bindable = config.GetBindable<int>(SandboxSetting.Radius)
                },
                new SettingsEnumPiece<CircularBarType>
                {
                    Description = "Bar type",
                    Bindable = config.GetBindable<CircularBarType>(SandboxSetting.CircularBarType)
                },
                new SettingsSliderPiece<int>
                {
                    Description = "Rotation",
                    Bindable = config.GetBindable<int>(SandboxSetting.Rotation)
                },
                new SettingsSliderPiece<int>
                {
                    Description = "Decay",
                    Bindable = config.GetBindable<int>(SandboxSetting.DecayA)
                },
                new SettingsSliderPiece<int>
                {
                    Description = "Height multiplier",
                    Bindable = config.GetBindable<int>(SandboxSetting.MultiplierA)
                },
                new SettingsTogglePiece
                {
                    Description = "Symmetry",
                    Bindable = config.GetBindable<bool>(SandboxSetting.Symmetry)
                },
                new SettingsSliderPiece<int>
                {
                    Description = "Smoothness",
                    Bindable = config.GetBindable<int>(SandboxSetting.SmoothnessA)
                },
                new SettingsSliderPiece<double>
                {
                    Description = "Bar width",
                    Bindable = config.GetBindable<double>(SandboxSetting.BarWidthA)
                },
                new SettingsSliderPiece<int>
                {
                    Description = "Visualizer amount",
                    Bindable = config.GetBindable<int>(SandboxSetting.VisualizerAmount),
                    TransferValueOnCommit = true
                },
                new SettingsSliderPiece<int>
                {
                    Description = "Bars per visual",
                    Bindable = config.GetBindable<int>(SandboxSetting.BarsPerVisual),
                    TransferValueOnCommit = true
                }
            });

            typeBItems.AddRange(new SettingsPieceBasePanel[]
            {
                new SettingsSliderPiece<int>
                {
                    Description = "Decay",
                    Bindable = config.GetBindable<int>(SandboxSetting.DecayB),
                },
                new SettingsSliderPiece<int>
                {
                    Description = "Height multiplier",
                    Bindable = config.GetBindable<int>(SandboxSetting.MultiplierB),
                },
                new SettingsSliderPiece<int>
                {
                    Description = "Smoothness",
                    Bindable = config.GetBindable<int>(SandboxSetting.SmoothnessB),
                },
                new SettingsSliderPiece<double>
                {
                    Description = "Bar width",
                    Bindable = config.GetBindable<double>(SandboxSetting.BarWidthB),
                },
                new SettingsSliderPiece<int>
                {
                    Description = "Bar count",
                    Bindable = config.GetBindable<int>(SandboxSetting.BarCountB),
                },
                new SettingsEnumPiece<LinearBarType>
                {
                    Description = "Bar type",
                    Bindable = config.GetBindable<LinearBarType>(SandboxSetting.LinearBarType)
                },
            });

            AddRange(typeAItems.ToArray());
            AddRange(typeBItems.ToArray());
        }

        protected override void LoadComplete()
        {
            layoutType.BindValueChanged(v =>
            {
                foreach (var p in typeAItems)
                    p.FadeOut();

                foreach (var p in typeBItems)
                    p.FadeOut();

                switch (v.NewValue)
                {
                    case VisualizerLayout.Empty:
                        break;

                    case VisualizerLayout.TypeA:
                        foreach (var p in typeAItems)
                            p.FadeIn();
                        break;

                    case VisualizerLayout.TypeB:
                        foreach (var p in typeBItems)
                            p.FadeIn();

                        break;
                }
            }, true);
            base.LoadComplete();
        }
    }
}
