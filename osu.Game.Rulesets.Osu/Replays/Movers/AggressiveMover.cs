// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;
using static osu.Game.Rulesets.Osu.Replays.Movers.MoverUtilExtensions;

namespace osu.Game.Rulesets.Osu.Replays.Movers
{
    public class AggressiveMover : DanceMover
    {
        private SliderPath path;
        private float lastAngle;

        public override void OnObjChange()
        {
            var scaledDistance = (float)(EndTime - StartTime);
            var newAngle = lastAngle + MathF.PI;

            if (Start is Slider start)
                newAngle = start.GetEndAngle();

            var points = new List<PathControlPoint>
            {
                new PathControlPoint(StartPos, PathType.Bezier),
                new PathControlPoint(V2FromRad(newAngle, scaledDistance) + StartPos),
            };

            if (scaledDistance > 1)
                lastAngle = points[1].Position.Value.AngleRV(EndPos);

            if (End is Slider end)
            {
                points.Add(new PathControlPoint(V2FromRad(end.GetStartAngle(), scaledDistance) + EndPos));
            }

            points.Add(new PathControlPoint(EndPos));

            path = new SliderPath(points.ToArray());
        }

        public override Vector2 Update(double time) => path.PositionAt(T(time));
    }
}
