using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Configuration;
using osu.Game.Screens.LLin.Misc;
using osu.Game.Screens.LLin.Plugins.Config;
using osu.Game.Screens.LLin.Plugins.Types.SettingsItems;

namespace osu.Game.Screens.LLin.Plugins.Internal.DummyAudio
{
    internal partial class DummyAudioPlugin : LLinPlugin
    {
        internal DummyAudioPlugin(MConfigManager config, LLinPluginManager plmgr)
        {
            HideFromPluginManagement = true;
            this.config = config;
            this.PluginManager = plmgr;

            Name = "Audio";
            Version = LLinPluginManager.LatestPluginVersion;
        }

        private SettingsEntry[]? entries;
        private readonly MConfigManager config;

        public override SettingsEntry[] GetSettingEntries(IPluginConfigManager pluginConfigManager)
        {
            if (entries == null)
            {
                ListSettingsEntry<TypeWrapper> audioEntry;
                var audioPluginBindable = new Bindable<TypeWrapper>();

                entries = new SettingsEntry[]
                {
                    new NumberSettingsEntry<double>
                    {
                        Name = "Playback speed",
                        Bindable = config.GetBindable<double>(MSetting.MvisMusicSpeed),
                        KeyboardStep = 0.01f,
                        DisplayAsPercentage = true,
                        //TransferValueOnCommit = true
                    },
                    new BooleanSettingsEntry
                    {
                        Name = "Adjust pitch",
                        Bindable = config.GetBindable<bool>(MSetting.MvisAdjustMusicWithFreq),
                        Description = "暂不支持调整故事版的音调"
                    },
                    new BooleanSettingsEntry
                    {
                        Name = "Nightcore beat",
                        Bindable = config.GetBindable<bool>(MSetting.MvisEnableNightcoreBeat)
                    },
                    audioEntry = new ListSettingsEntry<TypeWrapper>
                    {
                        Name = "Audio controller plugin",
                        Bindable = audioPluginBindable
                    }
                };

                var plugins = PluginManager!.GetAllAudioControlPlugin();

                foreach (var pl in plugins)
                {
                    if (config.Get<string>(MSetting.MvisCurrentAudioProvider) == PluginManager.ToPath(pl))
                    {
                        audioPluginBindable.Value = pl;
                        break;
                    }
                }

                audioEntry.Values = plugins;
                audioPluginBindable.Default = PluginManager.DefaultAudioControllerType;

                audioPluginBindable.BindValueChanged(v =>
                {
                    if (v.NewValue == null)
                    {
                        config.SetValue(MSetting.MvisCurrentAudioProvider, string.Empty);
                        return;
                    }

                    var pl = v.NewValue;

                    config.SetValue(MSetting.MvisCurrentAudioProvider, PluginManager.ToPath(pl));
                });
            }

            return entries;
        }

        protected override Drawable CreateContent()
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnContentLoaded(Drawable content)
        {
            throw new System.NotImplementedException();
        }

        protected override bool PostInit()
        {
            throw new System.NotImplementedException();
        }

        public override int Version { get; }
    }
}
