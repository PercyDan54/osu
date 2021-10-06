using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;
using osu.Game.Screens.Mvis.Plugins;
using osu.Game.Screens.Mvis.Plugins.Types;

namespace osu.Game.Screens.Mvis.Misc.PluginResolvers
{
    public class MvisPluginResolver
    {
        private readonly MvisPluginManager pluginManager;

        public MvisPluginResolver(MvisPluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
        }

        public string ToPath(object target)
        {
            return target.GetType().Name + "@" + target.GetType().Namespace;
        }

        internal void UpdatePluginDictionary(List<MvisPlugin> newPluginList)
        {
            functionBarDictionary.Clear();
            audioPluginDictionary.Clear();

            foreach (var plugin in newPluginList)
            {
                var pluginPath =
                    plugin.GetType().Name
                    + "@"
                    + plugin.GetType().Namespace;

                if (plugin is IFunctionBarProvider functionBarProvider)
                    functionBarDictionary[pluginPath] = functionBarProvider;

                if (plugin is IProvideAudioControlPlugin audioControlPlugin)
                    audioPluginDictionary[pluginPath] = audioControlPlugin;
            }

            var defaultAudioControlPath = pluginManager.DefaultAudioController.GetType().Name
                                          + "@"
                                          + pluginManager.DefaultAudioController.GetType().Namespace;

            audioPluginDictionary[defaultAudioControlPath] = pluginManager.DefaultAudioController;
        }

        internal bool RemoveFunctionBarProvider(IFunctionBarProvider functionBarProvider)
            => functionBarDictionary.Remove(ToPath(functionBarProvider), out _);

        internal bool RemoveAudioControlProvider(IProvideAudioControlPlugin provideAudioControlPlugin)
            => audioPluginDictionary.Remove(ToPath(provideAudioControlPlugin), out _);

        private readonly ConcurrentDictionary<string, IProvideAudioControlPlugin> audioPluginDictionary = new ConcurrentDictionary<string, IProvideAudioControlPlugin>();
        private readonly ConcurrentDictionary<string, IFunctionBarProvider> functionBarDictionary = new ConcurrentDictionary<string, IFunctionBarProvider>();

        [CanBeNull]
        internal IProvideAudioControlPlugin GetAudioControlPluginByPath(string path)
        {
            if (audioPluginDictionary.TryGetValue(path, out var result))
                return result;

            return null;
        }

        [CanBeNull]
        internal IFunctionBarProvider GetFunctionBarProviderByPath(string path)
        {
            if (functionBarDictionary.TryGetValue(path, out var result))
                return result;

            return null;
        }

        private List<IProvideAudioControlPlugin> cachedAudioControlPluginList;

        internal List<IProvideAudioControlPlugin> GetAllAudioControlPlugin()
        {
            var list = new List<IProvideAudioControlPlugin>();

            foreach (var keyPair in audioPluginDictionary)
            {
                list.Add(keyPair.Value);
            }

            if (cachedAudioControlPluginList == null || cachedAudioControlPluginList != list)
                cachedAudioControlPluginList = list;

            return list;
        }

        private List<IFunctionBarProvider> cachedFunctionBarPluginList;

        internal List<IFunctionBarProvider> GetAllFunctionBarProviders()
        {
            var list = new List<IFunctionBarProvider>();

            foreach (var keyPair in functionBarDictionary)
            {
                list.Add(keyPair.Value);
            }

            if (cachedFunctionBarPluginList == null || cachedFunctionBarPluginList != list)
                cachedFunctionBarPluginList = list;

            return list;
        }
    }
}
