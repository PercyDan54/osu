// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Objects.Drawables;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModAR : ModWithVisibilityAdjustment, IApplicableToBeatmap
    {
        public override string Description => @"Have fun reading the map!";

        public override string Name => "AR0";

        public override string Acronym => "AR0";

        public override double ScoreMultiplier => 1;

        private OsuBeatmap beatmap;

        public override void ApplyToBeatmap(IBeatmap map)
        {
            beatmap = (OsuBeatmap)map;
            foreach (OsuHitObject h in map.HitObjects)
            {
                h.TimePreempt = h.StartTime;
            }
        }
        protected override void ApplyIncreasedVisibilityState(DrawableHitObject drawable, ArmedState state)
        {
            if (drawable is DrawableHitCircle circle)
            {
                circle.ApproachCircle.Show();
            }
        }

        protected override void ApplyNormalVisibilityState(DrawableHitObject drawable, ArmedState state)
        {
            if (!(drawable is DrawableOsuHitObject))
                return;

            switch (drawable)
            {

                case DrawableHitCircle circle:
                    circle.ApproachCircle.Hide();
                    break;
            }
        }
    }
}
