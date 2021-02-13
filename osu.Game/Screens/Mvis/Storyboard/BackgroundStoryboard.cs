using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Skinning;
using osu.Game.Storyboards.Drawables;

namespace osu.Game.Screens.Mvis.Storyboard
{
    [LongRunningLoad]
    public class BackgroundStoryboard : BeatmapSkinProvidingContainer
    {
        public StoryboardClock RunningClock;
        private DrawableStoryboard drawableStoryboard;

        private readonly WorkingBeatmap working;

        public BackgroundStoryboard(WorkingBeatmap beatmap)
            : base(beatmap.Skin)
        {
            working = beatmap;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            drawableStoryboard = working.Storyboard.CreateDrawable();
            drawableStoryboard.Clock = RunningClock;

            LoadComponent(drawableStoryboard);
            Add(drawableStoryboard);
        }

        public Drawable StoryboardProxy() => drawableStoryboard.OverlayLayer.CreateProxy();

        protected override void Dispose(bool isDisposing)
        {
            drawableStoryboard?.Dispose();
            base.Dispose(isDisposing);
        }
    }
}
