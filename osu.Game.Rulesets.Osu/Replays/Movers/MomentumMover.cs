// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Configuration;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;
using static osu.Game.Rulesets.Osu.Replays.Movers.MoverUtilExtensions;

namespace osu.Game.Rulesets.Osu.Replays.Movers
{
    public class MomentumMover : DanceMover
    {
        private readonly float jumpMult;
        private readonly float nextMult;
        private readonly float offsetMult;
        private readonly bool skipStacks;
        private float offset => MathF.PI * offsetMult;

        private Vector2 p1;
        private Vector2 p2;
        private Vector2 last;

        public MomentumMover()
        {
            var config = MConfigManager.Instance;
            jumpMult = config.Get<float>(MSetting.JumpMulti);
            nextMult = config.Get<float>(MSetting.NextJumpMulti);
            offsetMult = config.Get<float>(MSetting.AngleOffset);
            skipStacks = config.Get<bool>(MSetting.SkipStackAngles);
        }

        private bool isSame(OsuHitObject o1, OsuHitObject o2) => o1.StackedPosition == o2.StackedPosition || (skipStacks && o1.Position == o2.Position);

        private (float, bool) nextAngle()
        {
            var h = Beatmap.HitObjects;

            for (int i = ObjectIndex + 1; i < h.Count - 1; ++i)
            {
                var o = h[i];
                if (o is Slider s) return (s.GetStartAngle(), true);
                if (!isSame(o, h[i + 1])) return (o.StackedPosition.AngleRV(h[i + 1].StackedPosition), false);
            }

            return ((h[^1] as Slider)?.GetEndAngle()
                    ?? ((Start as Slider)?.GetEndAngle() ?? StartPos.AngleRV(last)) + MathF.PI, false);
        }

        public override void OnObjChange()
        {
            var distance = Vector2.Distance(StartPos, EndPos);

            var start = Start as Slider;
            var (a2, afs) = nextAngle();
            var a1 = (ObjectsDuring[ObjectIndex] ? start?.GetStartAngle() + MathF.PI : start?.GetEndAngle()) ?? (ObjectIndex == 0 ? a2 + MathF.PI : StartPos.AngleRV(last));

            p1 = V2FromRad(a1, distance * jumpMult) + StartPos;

            var a = EndPos.AngleRV(StartPos);
            if (!afs && MathF.Abs(a2 - a) < offset) a2 = a2 - a < offset ? a - offset : a + offset;
            p2 = V2FromRad(a2, distance * nextMult) + EndPos;

            if (!(End is Slider) && !isSame(Start, End)) last = p2;
        }

        public override Vector2 Update(double time)
        {
            var t = T(time);
            var r = 1 - t;

            // cubic bézier curve
            return new Vector2(
                r * r * r * StartX
                + r * r * t * p1.X * 3
                + r * t * t * p2.X * 3
                + t * t * t * EndX,
                r * r * r * StartY
                + r * r * t * p1.Y * 3
                + r * t * t * p2.Y * 3
                + t * t * t * EndY
            );
        }
    }
}
