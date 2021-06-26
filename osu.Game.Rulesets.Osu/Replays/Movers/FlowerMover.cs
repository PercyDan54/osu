// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Configuration;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;
using static osu.Game.Rulesets.Osu.Replays.Movers.MoverUtilExtensions;

namespace osu.Game.Rulesets.Osu.Replays.Movers
{
    public class FlowerMover : DanceMover
    {
        private readonly float mult;
        private readonly float nextMult;
        private readonly float offsetMult;
        private float offset => MathF.PI * offsetMult;

        private Vector2 p1;
        private Vector2 p2;

        private float invert = 1;
        private float lastAngle;
        private Vector2 lastPoint;

        public FlowerMover()
        {
            var config = MConfigManager.Instance;
            mult = config.Get<float>(MSetting.JumpMulti);
            nextMult = config.Get<float>(MSetting.JumpMulti);
            offsetMult = config.Get<float>(MSetting.AngleOffset);
        }

        public override void OnObjChange()
        {
            float dist = Vector2.Distance(StartPos, EndPos);
            float scaled = mult * dist;
            float next = nextMult * dist;

            Slider start = Start as Slider;
            Slider end = End as Slider;

            float newAngle = offset * invert;

            if (start != null && end != null)
            {
                invert *= -1;
                p1 = V2FromRad(start.GetEndAngle(), scaled) + StartPos;
                p2 = V2FromRad(end.GetStartAngle(), next) + EndPos;
            }
            else if (start != null)
            {
                invert *= -1;
                lastAngle = StartPos.AngleRV(EndPos) - newAngle;

                p1 = V2FromRad(start.GetEndAngle(), scaled) + StartPos;
                p2 = V2FromRad(lastAngle, next) + EndPos;
            }
            else if (end != null)
            {
                lastAngle += MathF.PI;
                p1 = V2FromRad(lastAngle, scaled) + StartPos;
                p2 = V2FromRad(end.GetStartAngle(), next) + EndPos;
            }
            else
            {
                if (AngleBetween(StartPos, lastPoint, EndPos) >= offset)
                    invert *= -1;

                newAngle = StartPos.AngleRV(EndPos) - newAngle;

                p1 = V2FromRad(lastAngle + MathF.PI, scaled) + StartPos;
                p2 = V2FromRad(newAngle, next) + EndPos;
                if (scaled / mult > 2) lastAngle = newAngle;
            }

            lastPoint = StartPos;
        }

        public override Vector2 Update(double time)
        {
            var t = T(time);
            var r = 1 - t;

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
