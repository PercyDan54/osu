using System.Collections.Generic;
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
        private void load(MConfigManager config, MvisPluginManager pluginManager, MvisScreen mvisScreen)
        {
            config.BindWith(MSetting.MvisInterfaceRed, iR);
            config.BindWith(MSetting.MvisInterfaceGreen, iG);
            config.BindWith(MSetting.MvisInterfaceBlue, iB);

            var plugins = new List<IProvideAudioControlPlugin>();
            var currentAudioControlPlugin = config.Get<string>(MSetting.MvisCurrentAudioProvider);
            IProvideAudioControlPlugin currentprovider = mvisScreen.MusicControllerWrapper;

            foreach (var pl in pluginManager.GetAllPlugins(false))
            {
                if (pl is IProvideAudioControlPlugin pacp)
                {
                    plugins.Add(pacp);

                    var type = pl.GetType();

                    if (currentAudioControlPlugin == $"{type.Namespace}+{type.Name}")
                    {
                        currentprovider = pacp;
                    }
                }
            }

            plugins.Add(mvisScreen.MusicControllerWrapper);

            Bindable<IProvideAudioControlPlugin> configBindable;
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
                    Bindable = configBindable = new Bindable<IProvideAudioControlPlugin>
                    {
                        Value = currentprovider,
                        Default = mvisScreen.MusicControllerWrapper
                    },
                    Values = plugins
                },
                new SettingsEnumPiece<TabControlPosition>
                {
                    Icon = FontAwesome.Solid.Ruler,
                    Description = "Control position",
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

            configBindable.BindValueChanged(v =>
            {
                if (v.NewValue == null)
                {
                    config.SetValue(MSetting.MvisCurrentAudioProvider, string.Empty);
                    return;
                }

                var pl = (MvisPlugin)v.NewValue;
                var type = pl.GetType();

                config.SetValue(MSetting.MvisCurrentAudioProvider, $"{type.Namespace}+{type.Name}");
            });
        }

        private class ProviderSettingsPiece<T> : SettingsListPiece<T>
            where T : IProvideAudioControlPlugin
        {
            protected override string GetValueText(T newValue)
            {
                return (newValue as MvisPlugin)?.Name ?? "???";
            }
        }
    }
}
