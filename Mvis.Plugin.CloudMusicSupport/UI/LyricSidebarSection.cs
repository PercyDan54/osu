using Mvis.Plugin.CloudMusicSupport.Config;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Screens.Mvis.Plugins;
using osu.Game.Screens.Mvis.Plugins.Config;
using osu.Game.Screens.Mvis.SideBar.Settings.Items;

namespace Mvis.Plugin.CloudMusicSupport.UI
{
    public class LyricSidebarSection : PluginSidebarSettingsSection
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (LyricConfigManager)ConfigManager;

            AddRange(new Drawable[]
            {
                new SettingsTogglePiece
                {
                    Description = "Enable lyrics",
                    Bindable = config.GetBindable<bool>(LyricSettings.EnablePlugin)
                },
                new SettingsTogglePiece
                {
                    Icon = FontAwesome.Solid.SwimmingPool,
                    Description = "Use DrawablePool",
                    Bindable = config.GetBindable<bool>(LyricSettings.UseDrawablePool)
                },
                new SettingsTogglePiece
                {
                    Icon = FontAwesome.Solid.Save,
                    Description = "Save lyrics",
                    Bindable = config.GetBindable<bool>(LyricSettings.SaveLrcWhenFetchFinish),
                    TooltipText = "Lyrics will be saved at \"custom/lyrics/beatmap-{ID}.json\""
                },
                new SettingsTogglePiece
                {
                    Icon = FontAwesome.Solid.FillDrip,
                    Description = "Disable shadow",
                    Bindable = config.GetBindable<bool>(LyricSettings.NoExtraShadow),
                },
                new SettingsSliderPiece<double>
                {
                    Description = "Global lyric offset",
                    Bindable = config.GetBindable<double>(LyricSettings.LyricOffset),
                    TooltipText = "Try change this when the lyric desyncs"
                },
                new SettingsSliderPiece<float>
                {
                    Description = "Fade in duration",
                    Bindable = config.GetBindable<float>(LyricSettings.LyricFadeInDuration)
                },
                new SettingsSliderPiece<float>
                {
                    Description = "Fade out duration",
                    Bindable = config.GetBindable<float>(LyricSettings.LyricFadeOutDuration)
                }
            });
        }

        public LyricSidebarSection(MvisPlugin plugin)
            : base(plugin)
        {
        }
    }
}
