// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Utils;
using osu.Game.Configuration;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Replays.Danse.Objects;
using osuTK;

namespace osu.Game.Rulesets.Osu.Replays.Danse.Movers
{
    public static class MoverUtilExtensions
    {
        public static float AngleRV(this Vector2 v1, Vector2 v2) => MathF.Atan2(v1.Y - v2.Y, v1.X - v2.X);
        public static Vector2 V2FromRad(float rad, float radius) => new Vector2(MathF.Cos(rad) * radius, MathF.Sin(rad) * radius);

        public static float GetEndAngle(this DanceHitObject s) => s.GetAngle();
        public static float GetStartAngle(this DanceHitObject s) => s.GetAngle(true);

        public static float GetAngle(this DanceHitObject s, bool start = false)
        {
            if (s.BaseObject is not Slider)
                ThrowHelper.ThrowInvalidOperationException($"Only call {nameof(GetAngle)} on Sliders");

            return (start ? s.StartPos : s.EndPos).AngleRV(s.PositionAt(
                start ? 1 / s.Duration : (s.Duration - 1) / s.Duration
            ));
        }

        public static bool IsRetarded(this Slider s) => s.Distance == 0 || Precision.AlmostEquals(s.StartTime, s.EndTime);

        public static float AngleBetween(Vector2 centre, Vector2 v1, Vector2 v2)
        {
            float a = Vector2.Distance(centre, v1);
            float b = Vector2.Distance(centre, v2);
            float c = Vector2.Distance(v1, v2);
            return MathF.Acos((a * a + b * b - c * c) / (2 * a * b));
        }

        public static bool IsStream(params DanceHitObject[] hitObjects)
        {
            var config = MConfigManager.Instance;
            float max = config.Get<float>(MSetting.StreamMaximum);
            float min = config.Get<float>(MSetting.StreamMinimum);

            var h = hitObjects[0];
            bool isStream = false;

            for (int i = 0; i < hitObjects.Length - 1; i++)
            {
                var next = hitObjects[i + 1];
                float distanceSquared = Vector2.DistanceSquared(next.StartPos, h.EndPos);
                double timeDifference = next.StartTime - h.EndTime;

                isStream = distanceSquared >= min && distanceSquared <= max && timeDifference < 200 && h.BaseObject is HitCircle && next.BaseObject is HitCircle;
                h = next;
            }

            return isStream;
        }

        public static Vector2 ApplyPippiOffset(Vector2 pos, double time, float radius)
        {
            if (radius < 0f)
            {
                radius = OsuHitObject.OBJECT_RADIUS / 2 * 0.98f;
            }

            return pos + V2FromRad((float)time / 100f, radius);
        }
    }
}
