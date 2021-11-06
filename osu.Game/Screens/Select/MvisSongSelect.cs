using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osu.Game.Overlays;

namespace osu.Game.Screens.Select
{
    public class MvisSongSelect : SongSelect
    {
        public override bool HideOverlaysOnEnter => true;

        [Resolved]
        private MusicController musicController { get; set; }

        public Action ExitAction;

        [BackgroundDependencyLoader]
        private void load()
        {
            musicController.CurrentTrack.Looping = true;
        }

        protected override BeatmapDetailArea CreateBeatmapDetailArea() => new MvisBeatmapDetailArea
        {
            SelectCurrentAction = () => OnStart(),
        };

        public override bool AllowEditing => false;

        protected override bool OnStart()
        {
            SampleConfirm?.Play();

            if (BeatmapSetChanged)
                musicController.SeekTo(0);

            ExitAction?.Invoke();

            this.Exit();

            return true;
        }

        public override bool OnExiting(IScreen next)
        {
            ExitAction?.Invoke();
            return base.OnExiting(next);
        }
    }
}
