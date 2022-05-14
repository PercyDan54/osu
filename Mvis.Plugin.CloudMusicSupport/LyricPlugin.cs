using System.Collections.Generic;
using JetBrains.Annotations;
using M.DBus.Tray;
using Mvis.Plugin.CloudMusicSupport.Config;
using Mvis.Plugin.CloudMusicSupport.DBus;
using Mvis.Plugin.CloudMusicSupport.Helper;
using Mvis.Plugin.CloudMusicSupport.Misc;
using Mvis.Plugin.CloudMusicSupport.Sidebar;
using Mvis.Plugin.CloudMusicSupport.UI;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Screens.LLin.Plugins;
using osu.Game.Screens.LLin.Plugins.Config;
using osu.Game.Screens.LLin.Plugins.Types;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;

namespace Mvis.Plugin.CloudMusicSupport
{
    public class LyricPlugin : BindableControlledPlugin
    {
        /// <summary>
        /// 请参阅 <see cref="LLinPlugin.TargetLayer"/>
        /// </summary>
        public override TargetLayer Target => TargetLayer.Foreground;

        public override IPluginConfigManager CreateConfigManager(Storage storage)
            => new LyricConfigManager(storage);

        public override PluginSettingsSubSection CreateSettingsSubSection()
            => new LyricSettingsSubSection(this);

        public override PluginSidebarPage CreateSidebarPage()
            => new LyricSidebarSectionContainer(this);

        public override PluginSidebarSettingsSection CreateSidebarSettingsSection()
            => new LyricSidebarSection(this);

        public override int Version => 9;

        private WorkingBeatmap currentWorkingBeatmap;
        private LyricLineHandler lrcLine;

        /// <summary>
        /// 请参阅 <see cref="LLinPlugin.CreateContent()"/>
        /// </summary>
        protected override Drawable CreateContent() => lrcLine = new LyricLineHandler();

        private readonly LyricProcessor processor = new LyricProcessor();

        [CanBeNull]
        private List<Lyric> cachedLyrics;

        public readonly List<Lyric> EmptyLyricList = new List<Lyric>();

        [CanBeNull]
        private APILyricResponseRoot currentResponseRoot;

        [NotNull]
        public List<Lyric> Lyrics
        {
            get => cachedLyrics ?? EmptyLyricList;
            private set => cachedLyrics = value;
        }

        public void ReplaceLyricWith(List<Lyric> newList, bool saveToDisk)
        {
            CurrentStatus.Value = Status.Working;

            Lyrics = newList;

            if (saveToDisk)
                WriteLyricToDisk();

            CurrentStatus.Value = Status.Finish;
        }

        public void GetLyricFor(int id)
        {
            CurrentStatus.Value = Status.Working;
            processor.StartFetchById(id, onLyricRequestFinished, onLyricRequestFail);
        }

        private Track track;

        public readonly BindableDouble Offset = new BindableDouble
        {
            MaxValue = 3000,
            MinValue = -3000
        };

        private Bindable<bool> autoSave;

        public readonly Bindable<Status> CurrentStatus = new Bindable<Status>();

        public LyricPlugin()
        {
            Name = "Lyrics";
            Description = "Get lyrics from Netease music";
            Author = "MATRIX-夜翎";
            Depth = -1;

            Flags.AddRange(new[]
            {
                PluginFlags.CanDisable,
                PluginFlags.CanUnload
            });

            RelativeSizeAxes = Axes.Both;
            Anchor = Origin = Anchor.BottomCentre;
        }

        /// <summary>
        /// 请参阅 <see cref="LLinPlugin.OnContentLoaded(Drawable)"/>
        /// </summary>
        protected override bool OnContentLoaded(Drawable content) => true;

        private readonly SimpleEntry lyricEntry = new SimpleEntry
        {
            Enabled = false
        };

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (LyricConfigManager)Dependencies.Get<LLinPluginManager>().GetConfigManager(this);

            config.BindWith(LyricSettings.EnablePlugin, Value);
            autoSave = config.GetBindable<bool>(LyricSettings.SaveLrcWhenFetchFinish);

            AddInternal(processor);

            PluginManager.RegisterDBusObject(dbusObject = new LyricDBusObject());

            if (LLin != null)
                LLin.Exiting += onMvisExiting;

            Offset.BindValueChanged(v =>
            {
                if (currentResponseRoot != null)
                    currentResponseRoot.LocalOffset = v.NewValue;
            });
        }

        private void onMvisExiting()
        {
            resetDBusMessage();
            PluginManager.UnRegisterDBusObject(new LyricDBusObject());

            if (!Disabled.Value)
                PluginManager.RemoveDBusMenuEntry(lyricEntry);
        }

        public void WriteLyricToDisk(WorkingBeatmap currentBeatmap = null)
        {
            currentBeatmap ??= currentWorkingBeatmap;
            processor.WriteLrcToFile(currentResponseRoot, currentBeatmap);
        }

        public void RefreshLyric(bool noLocalFile = false)
        {
            CurrentStatus.Value = Status.Working;

            if (lrcLine != null)
            {
                lrcLine.Text = string.Empty;
                lrcLine.TranslatedText = string.Empty;
            }

            Lyrics.Clear();
            currentResponseRoot = null;
            CurrentLine = null;
            processor.StartFetchByBeatmap(currentWorkingBeatmap, noLocalFile, onLyricRequestFinished, onLyricRequestFail);
        }

        private double targetTime => track.CurrentTime + Offset.Value;

        private void onBeatmapChanged(WorkingBeatmap working)
        {
            if (Disabled.Value) return;

            if (currentWorkingBeatmap != null) WriteLyricToDisk(currentWorkingBeatmap);

            currentWorkingBeatmap = working;
            track = working.Track;

            CurrentStatus.Value = Status.Working;

            RefreshLyric();
        }

        private void onLyricRequestFail(string msg)
        {
            //onLyricRequestFail会在非Update上执行，因此添加Schedule确保不会发生InvalidThreadForMutationException
            Schedule(() =>
            {
                Lyrics.Clear();
                CurrentStatus.Value = Status.Failed;
            });
        }

        private void onLyricRequestFinished(APILyricResponseRoot responseRoot)
        {
            Schedule(() =>
            {
                Offset.Value = responseRoot.LocalOffset;
                currentResponseRoot = responseRoot;

                Lyrics = responseRoot.ToLyricList();

                if (autoSave.Value)
                    WriteLyricToDisk();

                CurrentStatus.Value = Status.Finish;
            });
        }

        public override bool Disable()
        {
            this.MoveToX(-10, 300, Easing.OutQuint).FadeOut(300, Easing.OutQuint);

            resetDBusMessage();
            PluginManager.RemoveDBusMenuEntry(lyricEntry);

            return base.Disable();
        }

        public override bool Enable()
        {
            bool result = base.Enable();

            this.MoveToX(0, 300, Easing.OutQuint).FadeIn(300, Easing.OutQuint);

            LLin?.OnBeatmapChanged(onBeatmapChanged, this, true);

            if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
            {
                dbusObject.RawLyric = currentLine?.Content;
                dbusObject.TranslatedLyric = currentLine?.TranslatedString;

                PluginManager.AddDBusMenuEntry(lyricEntry);
            }

            return result;
        }

        private void resetDBusMessage()
        {
            if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
            {
                dbusObject.RawLyric = string.Empty;
                dbusObject.TranslatedLyric = string.Empty;
            }
        }

        protected override bool PostInit() => true;

        private Lyric currentLine;
        private readonly Lyric emptyLine = new Lyric();

        public Lyric CurrentLine
        {
            get => currentLine;
            set
            {
                value ??= emptyLine;

                currentLine = value;

                if (RuntimeInfo.OS == RuntimeInfo.Platform.Linux)
                {
                    dbusObject.RawLyric = value.Content;
                    dbusObject.TranslatedLyric = value.TranslatedString;

                    lyricEntry.Label = value.Content + "\n" + value.TranslatedString;
                }
            }
        }

        private readonly Lyric defaultLrc = new Lyric();
        private LyricDBusObject dbusObject;

        protected override void Update()
        {
            base.Update();

            Padding = new MarginPadding { Bottom = (LLin?.BottomBarHeight ?? 0) + 20 };

            if (ContentLoaded)
            {
                var lrc = Lyrics.FindLast(l => targetTime >= l.Time) ?? defaultLrc;

                if (!lrc.Equals(CurrentLine))
                {
                    lrcLine.Text = lrc.Content;
                    lrcLine.TranslatedText = lrc.TranslatedString;

                    CurrentLine = lrc.GetCopy();
                }
            }
        }

        public enum Status
        {
            Working,
            Failed,
            Finish
        }
    }
}