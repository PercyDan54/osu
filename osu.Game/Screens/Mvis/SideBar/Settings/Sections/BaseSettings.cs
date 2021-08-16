using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Configuration;
using osu.Game.Screens.Mvis.Plugins;
using osu.Game.Screens.Mvis.Plugins.Types;
using osu.Game.Screens.Mvis.SideBar.Settings.Items;
using osu.Game.Screens.Mvis.SideBar.Tabs;

namespace osu.Game.Screens.Mvis.SideBar.Settings.Sections
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
        private void load(MConfigManager config, MvisPluginManager pluginManager)
        {
            config.BindWith(MSetting.MvisInterfaceRed, iR);
            config.BindWith(MSetting.MvisInterfaceGreen, iG);
            config.BindWith(MSetting.MvisInterfaceBlue, iB);

            var functionBarProviders = pluginManager.GetAllFunctionBarProviders();
            functionBarProviders.Insert(0, pluginManager.DummyFunctionBar);

            var currentAudioControlPlugin = config.Get<string>(MSetting.MvisCurrentAudioProvider);
            var currentFunctionbar = config.Get<string>(MSetting.MvisCurrentFunctionBar);

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
                    Description = "Music controller",
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
                    Description = "Function bar",
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
                    Description = "TabControl positon",
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
                    Description = "Idle bg dim",
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
                    Description = "Enable triangles",
                    Bindable = config.GetBindable<bool>(MSetting.MvisEnableBgTriangles),
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
                return (newValue as MvisPlugin)?.Name ?? "???";
            }
        }
    }
}
