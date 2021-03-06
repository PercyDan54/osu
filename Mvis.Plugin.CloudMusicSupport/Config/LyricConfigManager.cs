using osu.Framework.Configuration;
using osu.Framework.Platform;
using osu.Game.Screens.Mvis.Plugins.Config;

namespace Mvis.Plugin.CloudMusicSupport.Config
{
    public class LyricConfigManager : PluginConfigManager<LyricSettings>
    {
        public LyricConfigManager(Storage storage)
            : base(storage)
        {
        }

        /// <summary>
        /// 在这里初始化默认值, 更多用法请见 <see cref="ConfigManager"/>
        /// </summary>
        protected override void InitialiseDefaults()
        {
            SetDefault(LyricSettings.EnablePlugin, true);
            SetDefault(LyricSettings.LyricOffset, 0, -50000d, 50000d, 10d);
            SetDefault(LyricSettings.LyricFadeInDuration, 200f, 0, 1000);
            SetDefault(LyricSettings.LyricFadeOutDuration, 200f, 0, 1000);
            SetDefault(LyricSettings.SaveLrcWhenFetchFinish, true);
            SetDefault(LyricSettings.NoExtraShadow, true);
            SetDefault(LyricSettings.UseDrawablePool, false);
            base.InitialiseDefaults();
        }

        //配置文件名，已更改的值将在"plugin-{ConfigName}.ini"中保存
        protected override string ConfigName => "lyric";
    }

    public enum LyricSettings
    {
        EnablePlugin,
        LyricOffset,
        LyricFadeInDuration,
        LyricFadeOutDuration,
        SaveLrcWhenFetchFinish,
        NoExtraShadow,
        UseDrawablePool
    }
}
