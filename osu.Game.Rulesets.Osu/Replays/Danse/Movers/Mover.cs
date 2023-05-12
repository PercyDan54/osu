// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Mods;
using osuTK;

namespace osu.Game.Rulesets.Osu.Replays.Danse.Movers
{
    public abstract class Mover
    {
        public double StartTime { get; protected set; }
        public double EndTime { get; protected set; }
        protected double Duration => EndTime - StartTime;

        protected float ProgressAt(double time) => (float)((time - StartTime) / Duration);

        public IReadOnlyList<IApplicableToRate> TimeAffectingMods { set; protected get; } = null!;

        public virtual int SetObjects(List<DanceHitObject> objects) => 0;
        public abstract Vector2 Update(double time);
    }
}
