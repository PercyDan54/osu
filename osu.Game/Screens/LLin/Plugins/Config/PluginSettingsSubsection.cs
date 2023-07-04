using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;

namespace osu.Game.Screens.LLin.Plugins.Config
{
    public partial class PluginSettingsSubsection : SettingsSubsection
    {
        private readonly LLinPlugin plugin;

        public PluginSettingsSubsection(LLinPlugin plugin)
        {
            this.plugin = plugin;
            Name = $"{plugin}的subsection";

            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
        }

        protected override LocalisableString Header => plugin.Name;

        [BackgroundDependencyLoader]
        private void load(LLinPluginManager pluginManager)
        {
            var entries = pluginManager.GetSettingsFor(plugin);

            if (entries != null)
            {
                foreach (var se in entries)
                {
                    Add(se.ToSettingsItem());
                }
            }
        }
    }
}
