using Mvis.Plugin.CloudMusicSupport.Config;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Overlays.Settings;
using osu.Game.Screens.Mvis.Plugins;
using osu.Game.Screens.Mvis.Plugins.Config;

namespace Mvis.Plugin.CloudMusicSupport.UI
{
    public class LyricSettingsSubSection : PluginSettingsSubSection
    {
        public LyricSettingsSubSection(MvisPlugin plugin)
            : base(plugin)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (LyricConfigManager)ConfigManager;

            SettingsCheckbox useDrawablePoolCheckBox;
            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Enable lyrics",
                    Current = config.GetBindable<bool>(LyricSettings.EnablePlugin)
                },
                useDrawablePoolCheckBox = new SettingsCheckbox
                {
                    LabelText = "Use DrawablePool",
                    Current = config.GetBindable<bool>(LyricSettings.UseDrawablePool)
                },
                new SettingsCheckbox
                {
                    LabelText = "Save lyrics",
                    Current = config.GetBindable<bool>(LyricSettings.SaveLrcWhenFetchFinish),
                    TooltipText = "Lyrics will be save in\"custom/lyrics/beatmap-{ID}.json\""
                },
                new SettingsCheckbox
                {
                    LabelText = "Disable shadow",
                    Current = config.GetBindable<bool>(LyricSettings.NoExtraShadow),
                    TooltipText = "Don't add shaders to texts"
                },
                new SettingsSlider<double>
                {
                    LabelText = "Global lyric offset",
                    Current = config.GetBindable<double>(LyricSettings.LyricOffset),
                    TooltipText = "Try change this when the lyric desyncs"
                },
                new SettingsSlider<float>
                {
                    LabelText = "Fade in duration",
                    Current = config.GetBindable<float>(LyricSettings.LyricFadeInDuration)
                },
                new SettingsSlider<float>
                {
                    LabelText = "Fade out duration",
                    Current = config.GetBindable<float>(LyricSettings.LyricFadeOutDuration)
                },
            };
            useDrawablePoolCheckBox.WarningText = "Experimental function!";
        }
    }
}
