// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.IO;
using System.Reflection;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Skinning;

namespace osu.Game.Screens.MapGuess
{
    public class MapGuessWorkingBeatmap : WorkingBeatmap
    {
        private static readonly MethodInfo getBeatmap = typeof(WorkingBeatmap).GetMethod(nameof(GetBeatmap), BindingFlags.Instance | BindingFlags.NonPublic)!;
        private static readonly MethodInfo getBeatmapTrack = typeof(WorkingBeatmap).GetMethod(nameof(GetBeatmapTrack), BindingFlags.Instance | BindingFlags.NonPublic)!;
        private readonly WorkingBeatmap beatmap;

        public MapGuessWorkingBeatmap(WorkingBeatmap beatmap, AudioManager audioManager)
            : base(beatmap.BeatmapInfo, audioManager)
        {
            this.beatmap = beatmap;
        }

        protected override IBeatmap GetBeatmap() => (IBeatmap)getBeatmap.Invoke(beatmap, null)!;

        public override Texture GetBackground() => beatmap.GetBackground();

        public override Stream GetStream(string storagePath) => beatmap.GetStream(storagePath);

        protected override Track GetBeatmapTrack() => (Track)getBeatmapTrack.Invoke(beatmap, null)!;

        // Disable beatmap skins/hitsounds
        protected internal override ISkin GetSkin() => null!;
    }
}
