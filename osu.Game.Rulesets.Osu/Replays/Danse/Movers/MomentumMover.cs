// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Game.Configuration;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;
using static osu.Game.Rulesets.Osu.Replays.Danse.Movers.MoverUtilExtensions;

namespace osu.Game.Rulesets.Osu.Replays.Danse.Movers
{
    public class MomentumMover : Mover
    {
        private float offset => restrictAngle * MathF.PI / 180.0f;

        private Vector2 last;
        private BezierCurveCubic curve;
        private bool first = true;
        private readonly float jumpMult;
        private readonly float offsetMult;
        private readonly bool skipStacks;
        private readonly bool streamRestrict;
        private readonly float restrictArea;
        private readonly float restrictAngle;
        private readonly float streamMult;
        private readonly bool restrictInvert;
        private readonly float durationTrigger;
        private readonly float durationMult;

        private readonly float streamArea;
        private readonly float restrictAngleAdd;
        private readonly float restrictAngleSub;
        private readonly float equalPosBounce;
        private readonly bool sliderPredict;
        private readonly bool interpolateAngles;
        private readonly bool invertAngleInterpolation;

        public MomentumMover()
        {
            var config = MConfigManager.Instance;
            jumpMult = config.Get<float>(MSetting.JumpMult);
            offsetMult = config.Get<float>(MSetting.AngleOffset);
            skipStacks = config.Get<bool>(MSetting.SkipStackAngles);
            restrictInvert = config.Get<bool>(MSetting.RestrictInvert);
            restrictAngle = config.Get<float>(MSetting.RestrictAngle);
            restrictArea = config.Get<float>(MSetting.RestrictArea);
            streamRestrict = config.Get<bool>(MSetting.StreamRestrict);
            streamMult = config.Get<float>(MSetting.StreamMult);
            durationTrigger = config.Get<float>(MSetting.DurationTrigger);
            durationMult = config.Get<float>(MSetting.DurationMult);

            streamArea = config.Get<float>(MSetting.StreamArea);
            equalPosBounce = config.Get<float>(MSetting.EqualPosBounce);
            restrictAngleAdd = config.Get<float>(MSetting.RestrictAngleAdd);
            restrictAngleSub = config.Get<float>(MSetting.RestrictAngleSub);
            sliderPredict = config.Get<bool>(MSetting.SliderPredict);
            interpolateAngles = config.Get<bool>(MSetting.InterpolateAngles);
            invertAngleInterpolation = config.Get<bool>(MSetting.InvertAngleInterpolation);
        }

        private bool isSame(DanceHitObject o1, DanceHitObject o2) => isSame(o1.BaseObject, o2.BaseObject, skipStacks);

        private bool isSame(OsuHitObject o1, OsuHitObject o2, bool skipStacks)
        {
            return o1.StackedPosition == o2.StackedPosition || (skipStacks && o1.Position == o2.Position);
        }

        public override int SetObjects(List<DanceHitObject> objects)
        {
            OsuHitObject? next = null;

            if (objects.Count > 2) next = objects[2].BaseObject;
            var start = objects[0];
            var end = objects[Math.Min(objects.Count - 1, 1)];
            var startPos = start.EndPos;
            var endPos = end.StartPos;
            StartTime = start.EndTime;
            EndTime = end.StartTime;

            bool stream = false;
            float sq1 = 0, sq2 = 0;

            if (next != null)
            {
                stream = IsStream(start.BaseObject, end.BaseObject, next) && streamRestrict;
                sq1 = Vector2.DistanceSquared(startPos, endPos);
                sq2 = Vector2.DistanceSquared(endPos, next.StackedPosition);
            }

            float area = restrictArea * MathF.PI / 180f;
            float sarea = streamArea * MathF.PI / 180f;
            float mult = jumpMult;
            float distance = Vector2.Distance(startPos, endPos);

            bool fromLong = false;
            float a, a2 = start.StartPos.AngleRV(endPos);

            for (int i = 1; i < objects.Count; i++)
            {
                var o = objects[i];

                if (o.BaseObject is Slider s)
                {
                    a2 = s.GetStartAngle();
                    fromLong = true;
                    break;
                }

                if (i == objects.Count - 1)
                {
                    a2 = last.AngleRV(startPos);
                    break;
                }
            }

            if (end.BaseObject is Slider s2 && !isSame(start, end) && sliderPredict)
            {
                var pos = startPos;
                var pos2 = endPos;
                float s2a = s2.GetStartAngle();
                float dst2 = Vector2.Distance(pos, pos2);
                pos2 = new Vector2(s2a, dst2 * mult) + pos2;
                a2 = pos.AngleRV(pos2);
            }

            float a1 = (start.BaseObject as Slider)?.GetEndAngle() ?? (first ? a2 + MathF.PI : startPos.AngleRV(last));
            float ac = a2 - endPos.AngleRV(startPos);

            if (sarea > 0 && stream && anorm(ac) < anorm(2 * MathF.PI - sarea))
            {
                a = startPos.AngleRV(endPos);
                const float sangle = MathF.PI * 0.5f;

                if (anorm(a1 - a) > MathF.PI)
                    a2 = a - sangle;
                else
                    a2 = a + sangle;

                mult = streamMult;
            }
            else if (!fromLong && area > 0 && MathF.Abs(anorm2(ac)) < area)
            {
                a = endPos.AngleRV(startPos);

                if (anorm(a2 - a) < offset != restrictInvert)
                    a2 = a + restrictAngleAdd * MathF.PI / 180f;
                else
                    a2 = a - restrictAngleSub * MathF.PI / 180f;

                mult = jumpMult;
            }
            else if (next != null && !fromLong && interpolateAngles)
            {
                float r = sq1 / (sq1 + sq2);
                a = startPos.AngleRV(endPos);

                if (invertAngleInterpolation)
                    r = sq2 / (sq1 + sq2);

                if (!isSame(start, end))
                    a2 = a + r * anorm2(a2 - a);

                mult = offsetMult;
            }

            bool bounce = !(end.BaseObject is IHasDuration) && isSame(start.BaseObject, end.BaseObject, true);

            if (equalPosBounce > 0 && bounce)
            {
                a1 = startPos.AngleRV(last);
                a2 = a1 + MathF.PI;
                distance = Vector2.Distance(last, startPos);
                mult = equalPosBounce;
            }

            float duration = (float)(EndTime - StartTime);

            if (durationTrigger > 0 && duration >= durationTrigger)
                mult *= durationMult * (duration / durationTrigger);

            var p1 = V2FromRad(a1, distance * mult) + startPos;
            var p2 = V2FromRad(a2, distance * mult) + endPos;

            if (!bounce) last = p2;

            curve = new BezierCurveCubic(startPos, endPos, p1, p2);
            first = false;

            return 3;
        }

        private float anorm(float a)
        {
            const float pi2 = 2 * MathF.PI;
            a %= pi2;

            if (a < 0)
                a += pi2;

            return a;
        }

        private float anorm2(float a)
        {
            a = anorm(a);

            if (a > MathF.PI)
                a = -(2 * MathF.PI - a);

            return a;
        }

        public override Vector2 Update(double time) => curve.CalculatePoint(ProgressAt(time));
    }
}
