using Mvis.Plugin.Yasp.Config;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Screens.Mvis.Plugins;
using osu.Game.Screens.Mvis.Plugins.Config;
using osu.Game.Screens.Mvis.SideBar.Settings.Items;

namespace Mvis.Plugin.Yasp.UI
{
    public class YaspSidebarSection : PluginSidebarSettingsSection
    {
        public YaspSidebarSection(MvisPlugin plugin)
            : base(plugin)
        {
        }

        public override int Columns => 2;

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (YaspConfigManager)ConfigManager;

            AddRange(new Drawable[]
            {
                new SettingsTogglePiece
                {
                    Description = "Enable",
                    Bindable = config.GetBindable<bool>(YaspSettings.EnablePlugin)
                },
                new SettingsSliderPiece<float>
                {
                    Icon = FontAwesome.Solid.ExpandArrowsAlt,
                    Description = "Scale",
                    Bindable = config.GetBindable<float>(YaspSettings.Scale),
                    DisplayAsPercentage = true
                }
            });
        }
    }
}
