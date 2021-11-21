// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;

namespace osu.Game.Rulesets.Osu.Replays.Movers
{
    public abstract class Mover
    {
        protected double StartTime => Start.GetEndTime();
        protected double EndTime => End.StartTime;
        protected double Duration => EndTime - StartTime;

        public Vector2 StartPos;
        public Vector2 EndPos;
        protected float StartX => StartPos.X;
        protected float StartY => StartPos.Y;
        protected float EndX => EndPos.X;
        protected float EndY => EndPos.Y;

        protected float T(double time) => (float)((time - StartTime) / Duration);

        public int ObjectIndex { set; protected get; }
        public List<OsuHitObject> HitObjects;
        public IReadOnlyList<IApplicableToRate> TimeAffectingMods { set; protected get; }

        public OsuHitObject Start => HitObjects[ObjectIndex];

        public OsuHitObject End => HitObjects[ObjectIndex + 1];

        public virtual void OnObjChange() { }
        public abstract Vector2 Update(double time);
    }
}
