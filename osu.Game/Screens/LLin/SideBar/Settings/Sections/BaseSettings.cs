using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Configuration;
using osu.Game.Screens.LLin.Plugins;
using osu.Game.Screens.LLin.Plugins.Types;
using osu.Game.Screens.LLin.SideBar.Settings.Items;
using osu.Game.Screens.LLin.SideBar.Tabs;

namespace osu.Game.Screens.LLin.SideBar.Settings.Sections
{
    public class BaseSettings : Section
    {
        private readonly BindableFloat iR = new BindableFloat();
        private readonly BindableFloat iG = new BindableFloat();
        private readonly BindableFloat iB = new BindableFloat();

        public BaseSettings()
        {
            Title = "Basic settings";
        }

        [BackgroundDependencyLoader]
        private void load(MConfigManager config, LLinPluginManager pluginManager, IImplementLLin mvisScreen)
        {
            config.BindWith(MSetting.MvisInterfaceRed, iR);
            config.BindWith(MSetting.MvisInterfaceGreen, iG);
            config.BindWith(MSetting.MvisInterfaceBlue, iB);

            var functionBarProviders = pluginManager.GetAllFunctionBarProviders();
            functionBarProviders.Insert(0, pluginManager.DummyFunctionBar);

            string currentAudioControlPlugin = config.Get<string>(MSetting.MvisCurrentAudioProvider);
            string currentFunctionbar = config.Get<string>(MSetting.MvisCurrentFunctionBar);

            Bindable<IProvideAudioControlPlugin> audioConfigBindable;
            Bindable<IFunctionBarProvider> functionBarConfigBindable;

            AddRange(new Drawable[]
            {
                new SettingsSliderPiece<float>
                {
                    Description = "Red",
                    Bindable = iR
                },
                new SettingsSliderPiece<float>
                {
                    Description = "Green",
                    Bindable = iG
                },
                new SettingsSliderPiece<float>
                {
                    Description = "Blue",
                    Bindable = iB
                },
                new ProviderSettingsPiece<IProvideAudioControlPlugin>
                {
                    Icon = FontAwesome.Solid.Bullseye,
                    Description = "Music control plugin",
                    Bindable = audioConfigBindable = new Bindable<IProvideAudioControlPlugin>
                    {
                        Value = pluginManager.GetAudioControlByPath(currentAudioControlPlugin),
                        Default = pluginManager.DefaultAudioController
                    },
                    Values = pluginManager.GetAllAudioControlPlugin()
                },
                new ProviderSettingsPiece<IFunctionBarProvider>
                {
                    Icon = FontAwesome.Solid.Bullseye,
                    Description = "底栏插件",
                    Bindable = functionBarConfigBindable = new Bindable<IFunctionBarProvider>
                    {
                        Value = pluginManager.GetFunctionBarProviderByPath(currentFunctionbar),
                        Default = pluginManager.DummyFunctionBar
                    },
                    Values = functionBarProviders
                },
                new SettingsEnumPiece<TabControlPosition>
                {
                    Icon = FontAwesome.Solid.Ruler,
                    Description = "TabControl Position",
                    Bindable = config.GetBindable<TabControlPosition>(MSetting.MvisTabControlPosition)
                },
                new SettingsSliderPiece<float>
                {
                    Icon = FontAwesome.Solid.SolarPanel,
                    Description = "Background blur",
                    Bindable = config.GetBindable<float>(MSetting.MvisBgBlur),
                    DisplayAsPercentage = true
                },
                new SettingsSliderPiece<float>
                {
                    Icon = FontAwesome.Regular.Sun,
                    Description = "Bg dim when idle",
                    Bindable = config.GetBindable<float>(MSetting.MvisIdleBgDim),
                    DisplayAsPercentage = true
                },
                new SettingsTogglePiece
                {
                    Icon = FontAwesome.Regular.ArrowAltCircleUp,
                    Description = "Top Proxy",
                    Bindable = config.GetBindable<bool>(MSetting.MvisStoryboardProxy),
                },
                new SettingsTogglePiece
                {
                    Icon = FontAwesome.Solid.Clock,
                    Description = "Enable animation",
                    Bindable = config.GetBindable<bool>(MSetting.MvisEnableBgTriangles),
                    TooltipText = "Show background animation when possible"
                },
            });

            audioConfigBindable.BindValueChanged(v =>
            {
                if (v.NewValue == null)
                {
                    config.SetValue(MSetting.MvisCurrentAudioProvider, string.Empty);
                    return;
                }

                config.SetValue(MSetting.MvisCurrentAudioProvider, pluginManager.ToPath(v.NewValue));
            });

            functionBarConfigBindable.BindValueChanged(v =>
            {
                if (v.NewValue == null)
                {
                    config.SetValue(MSetting.MvisCurrentFunctionBar, string.Empty);
                    return;
                }

                config.SetValue(MSetting.MvisCurrentFunctionBar, pluginManager.ToPath(v.NewValue));
            });
        }

        private class ProviderSettingsPiece<T> : SettingsListPiece<T>
        {
            protected override string GetValueText(T newValue)
            {
                return (newValue as LLinPlugin)?.Name ?? "???";
            }
        }
    }
}
