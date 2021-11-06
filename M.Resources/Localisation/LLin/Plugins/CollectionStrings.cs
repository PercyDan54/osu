using Humanizer;
using osu.Framework.Localisation;

namespace M.Resources.Localisation.LLin.Plugins
{
    public static class CollectionStrings
    {
        private const string prefix = @"M.Resources.Localisation.LLin.Plugins.YaspStrings";

        public static LocalisableString NoCollectionSelected => new TranslatableString(getKey(@"no_collection_selected"), @"No collection selected");

        public static LocalisableString AudioControlRequest => new TranslatableString(getKey(@"audio_control_request"), "Activate to make this plugin work\nThis notification won't appear again in this session");

        public static LocalisableString SelectOneFirst => new TranslatableString(getKey(@"select_one_first"), @"Select one first"!);

        public static LocalisableString EntryTooltip => new TranslatableString(getKey(@"entry_tooltip"), "Browse collections");

        public static LocalisableString SongCount(int count) => new TranslatableString(getKey(@"song_count"), @"song".ToQuantity(count), count);

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
