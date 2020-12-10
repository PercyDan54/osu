// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Replays;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Replays.Movers;
using osu.Game.Rulesets.Osu.UI;
using osuTK;
using static osu.Game.Configuration.OsuDanceMover;

namespace osu.Game.Rulesets.Osu.Replays
{
    public class OsuDanceGenerator : OsuAutoGeneratorBase
    {
        public static BaseDanceMover GetMover(OsuDanceMover mover) =>
            mover switch
            {
                HalfCircle => new HalfCircleMover(),
                Flower => new FlowerMover(),
                Momentum => new MomentumMover(),
                _ => new TestMover()
            };

        public new OsuBeatmap Beatmap => (OsuBeatmap)base.Beatmap;
        private readonly bool[] objectsDuring;
        private readonly BaseDanceMover mover;
        private readonly float spinRadiusStart;
        private readonly float spinRadiusEnd;
        private readonly MfConfigManager config;

        private int buttonIndex;
        private readonly double frameTime;

        private OsuAction getAction(OsuHitObject h, OsuHitObject last)
        {
            double timeDifference = ApplyModsToTime(h.StartTime - last.StartTime);
            if (timeDifference > 0 && // Sanity checks
               ((last.StackedPosition - h.StackedPosition).Length > h.Radius * (1.5 + 100.0 / timeDifference) || // Either the distance is big enough
                timeDifference >= 266)) // ... or the beats are slow enough to tap anyway.
            {
                buttonIndex = 0;
            }
            else
            {
                buttonIndex++;
            }
            return buttonIndex % 2 == 0 ? OsuAction.LeftButton : OsuAction.RightButton;
        }
        private static void calcSpinnerStartPosAndDirection(Vector2 prevPos, out Vector2 startPosition, out float spinnerDirection)
        {
            Vector2 spinCentreOffset = SPINNER_CENTRE - prevPos;
            float distFromCentre = spinCentreOffset.Length;
            float distToTangentPoint = MathF.Sqrt(distFromCentre * distFromCentre - SPIN_RADIUS * SPIN_RADIUS);

            if (distFromCentre > SPIN_RADIUS)
            {
                // Previous cursor position was outside spin circle, set startPosition to the tangent point.

                // Angle between centre offset and tangent point offset.
                float angle = MathF.Asin(SPIN_RADIUS / distFromCentre);

                if (angle > 0)
                {
                    spinnerDirection = -1;
                }
                else
                {
                    spinnerDirection = 1;
                }

                // Rotate by angle so it's parallel to tangent line
                spinCentreOffset.X = spinCentreOffset.X * MathF.Cos(angle) - spinCentreOffset.Y * MathF.Sin(angle);
                spinCentreOffset.Y = spinCentreOffset.X * MathF.Sin(angle) + spinCentreOffset.Y * MathF.Cos(angle);

                // Set length to distToTangentPoint
                spinCentreOffset.Normalize();
                spinCentreOffset *= distToTangentPoint;

                // Move along the tangent line, now startPosition is at the tangent point.
                startPosition = prevPos + spinCentreOffset;
            }
            else if (spinCentreOffset.Length > 0)
            {
                // Previous cursor position was inside spin circle, set startPosition to the nearest point on spin circle.
                startPosition = SPINNER_CENTRE - spinCentreOffset * (SPIN_RADIUS / spinCentreOffset.Length);
                spinnerDirection = 1;
            }
            else
            {
                // Degenerate case where cursor position is exactly at the centre of the spin circle.
                startPosition = SPINNER_CENTRE + new Vector2(0, -SPIN_RADIUS);
                spinnerDirection = 1;
            }
        }

        public OsuDanceGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
            config = MfConfigManager.Instance;
            frameTime = 1000.0 / config.Get<float>(MfSetting.ReplayFramerate);
            spinRadiusStart = config.Get<float>(MfSetting.SpinnerRadius);
            spinRadiusEnd = config.Get<float>(MfSetting.SpinnerRadius2);
            mover = GetMover(config.Get<OsuDanceMover>(MfSetting.DanceMover));

            mover.Beatmap = Beatmap;

            var objectsDuring = new bool[Beatmap.HitObjects.Count];

            for (int i = 0; i < Beatmap.HitObjects.Count - 1; ++i)
            {
                var e = Beatmap.HitObjects[i].GetEndTime();
                objectsDuring[i] = false;

                for (int j = i + 1; j < Beatmap.HitObjects.Count; ++j)
                {
                    if (Beatmap.HitObjects[j].StartTime + 1 > e) continue;

                    objectsDuring[i] = true;
                    break;
                }
            }

            this.objectsDuring = objectsDuring;
            mover.ObjectsDuring = objectsDuring;
        }

        private void objectGenerate(OsuHitObject h, int idx)
        {
            float sDirection = -1;
            OsuAction action = getAction(h, Beatmap.HitObjects[idx == 0 ? idx : idx - 1]);
            Vector2 startPosition = h.StackedPosition;
            switch (h)
            {
                case Slider slider:
                    AddFrameToReplay(new OsuReplayFrame(slider.StartTime, slider.StackedPosition, action));

                    if (objectsDuring[idx]) break;

                    for (double t = slider.StartTime + frameTime; t < slider.EndTime; t += frameTime)
                        if (slider.Distance / slider.RepeatCount <= 34 && slider.RepeatCount >= 1)
                        {
                            AddFrameToReplay(new OsuReplayFrame((int)h.StartTime, h.StackedPosition, action));
                        }
                        else
                        {
                            if (slider.Duration > 300)
                            {
                                double speed = slider.Distance / slider.Duration;
                                for (double j = FrameDelay; j < slider.Duration; j += FrameDelay)
                                {

                                    Vector2 difference1 = startPosition - SPINNER_CENTRE;
                                    float radius1 = difference1.Length;
                                    float angle1 = radius1 == 0 ? 0 : MathF.Atan2(difference1.Y, difference1.X);
                                    Vector2 pos = slider.StackedPositionAt(j / slider.Duration);
                                    Vector2 pos2 = pos + CirclePosition(ApplyModsToTime(j) / 12 * speed + angle1, 15);
                                    AddFrameToReplay(new OsuReplayFrame((int)h.StartTime + j, pos2, action));
                                }
                            }
                            else
                            {
                                for (double j = FrameDelay; j < slider.Duration; j += FrameDelay)
                                {
                                    Vector2 pos = slider.StackedPositionAt(j / slider.Duration);
                                    AddFrameToReplay(new OsuReplayFrame(h.StartTime + j, pos, action));
                                }
                            }
                        }

                    break;

                case Spinner s:
                    calcSpinnerStartPosAndDirection(((OsuReplayFrame)Frames[^1]).Position, out startPosition, out sDirection);
                    Vector2 difference = s.StackedPosition - SPINNER_CENTRE;

                    float radius = difference.Length;
                    float angle = radius == 0 ? 0 : MathF.Atan2(difference.Y, difference.X);

                    double t1;
                    double r = s.SpinsRequired > 3 ? spinRadiusStart : spinRadiusEnd;
                    double r1;
                    double rEndTime = s.StartTime + (s.Duration * 0.6);
                    for (double j = s.StartTime + FrameDelay; j < s.EndTime; j += FrameDelay)
                    {
                        t1 = ApplyModsToTime(j - s.StartTime) * sDirection;
                        r1 = j > rEndTime ? spinRadiusEnd : Interpolation.ValueAt(j, r, spinRadiusEnd, s.StartTime, rEndTime, Easing.In);
                        Vector2 pos = SPINNER_CENTRE + CirclePosition(t1 / 20 + angle, r1);
                        AddFrameToReplay(new OsuReplayFrame((int)j, new Vector2(pos.X, pos.Y), action));
                    }
                    break;

                case HitCircle c:
                    AddFrameToReplay(new OsuReplayFrame(c.StartTime, c.StackedPosition, action));
                    break;

                default: return;
            }
        }

        public override Replay Generate()
        {
            var o = Beatmap.HitObjects[0];
            AddFrameToReplay(new OsuReplayFrame(-100000, new Vector2(256, 500)));
            AddFrameToReplay(new OsuReplayFrame(Beatmap.HitObjects[0].StartTime - 1500, new Vector2(256, 500)));

            var bs = OsuPlayfield.BASE_SIZE;

            var xf = bs.X / 0.8f * (4f / 3f);
            var x0 = (bs.X - xf) / 2f;
            var x1 = xf + x0;

            var yf = bs.Y / 0.8f;
            var y0 = (bs.Y - yf) / 2f;
            var y1 = yf + y0;

            for (int i = 0; i < Beatmap.HitObjects.Count - 1; i++)
            {
                o = Beatmap.HitObjects[i];
                objectGenerate(o, i);

                mover.ObjectIndex = i;
                mover.OnObjChange();

                for (double t = (objectsDuring[i] ? o.StartTime : o.GetEndTime()) + frameTime; t < mover.End.StartTime; t += frameTime)
                {
                    var v = mover.Update(t);

                    if (config.Get<bool>(MfSetting.BorderBounce))
                    {
                        if (v.X < x0) v.X = x0 - (v.X - x0);
                        if (v.Y < y0) v.Y = y0 - (v.Y - y0);

                        if (v.X > x1)
                        {
                            var x = v.X - x0;
                            var m = (int)(x / xf);
                            x %= xf;
                            x = m % 2 == 0 ? x : xf - x;
                            v.X = x + x0;
                        }

                        if (v.Y > y1)
                        {
                            var y = v.Y - y0;
                            var m = (int)(y / yf);
                            y %= yf;
                            y = m % 2 == 0 ? y : yf - y;
                            v.Y = y + y0;
                        }
                    }

                    AddFrameToReplay(new OsuReplayFrame(t, v));
                }
            }

            objectGenerate(Beatmap.HitObjects[^1], Beatmap.HitObjects.Count - 1);

            return Replay;
        }
    }

    public abstract class BaseDanceMover
    {
        protected double StartTime => Start.GetEndTime();
        protected double EndTime => End.StartTime;
        protected double Duration => EndTime - StartTime;
        protected float DurationF => (float)Duration;

        protected Vector2 StartPos => ObjectsDuring[ObjectIndex] ? Start.StackedPosition : Start.StackedEndPosition;
        protected Vector2 EndPos => End.StackedPosition;
        protected float StartX => StartPos.X;
        protected float StartY => StartPos.Y;
        protected float EndX => EndPos.X;
        protected float EndY => EndPos.Y;

        protected float T(double time) => (float)((time - StartTime) / Duration);

        public bool[] ObjectsDuring { set; protected get; }

        public int ObjectIndex { set; protected get; }
        public OsuBeatmap Beatmap { set; protected get; }
        public OsuHitObject Start => Beatmap.HitObjects[ObjectIndex];
        public OsuHitObject End => Beatmap.HitObjects[ObjectIndex + 1];
        public virtual void OnObjChange() { }
        public abstract Vector2 Update(double time);
    }

    public abstract class BaseDanceObjectMover<TObject>
        where TObject : OsuHitObject, IHasDuration
    {
        protected double StartTime => Object.StartTime;
        protected double Duration => Object.Duration;

        protected float T(double time) => (float)((time - StartTime) / Duration);

        public TObject Object { set; protected get; }
        public virtual void OnObjChange() { }
        public abstract Vector2 Update(double time);
    }
}
