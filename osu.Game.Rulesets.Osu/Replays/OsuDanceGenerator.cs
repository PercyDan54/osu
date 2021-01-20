// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Replays;
using osu.Game.Rulesets.Objects;
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
        public static DanceMover GetMover(OsuDanceMover mover) =>
            mover switch
            {
                HalfCircle => new HalfCircleMover(),
                Flower => new FlowerMover(),
                Momentum => new MomentumMover(),
                _ => new MomentumMover()
            };

        public new OsuBeatmap Beatmap => (OsuBeatmap)base.Beatmap;
        private readonly bool sliderDance;
        private readonly bool[] objectsDuring;
        private readonly DanceMover mover;
        private readonly float spinRadiusStart;
        private readonly float spinRadiusEnd;
        private readonly MfConfigManager config;
        private readonly double frameDelay;
        private int buttonIndex;

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
            frameDelay = 1000.0 / config.Get<float>(MfSetting.ReplayFramerate);
            spinRadiusStart = config.Get<float>(MfSetting.SpinnerRadiusStart);
            sliderDance = config.Get<bool>(MfSetting.SliderDance);
            spinRadiusEnd = config.Get<float>(MfSetting.SpinnerRadiusEnd);
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

        private void moveToHitObject(OsuHitObject h, int idx, bool sliderDance)
        {
            OsuHitObject last = Beatmap.HitObjects[idx == 0 ? idx : idx - 1];
            OsuAction action = getAction(h, last);
            Vector2 startPosition = h.StackedPosition;
            Vector2 difference = startPosition - SPINNER_CENTRE;
            float radius = difference.Length;
            float angle = radius == 0 ? 0 : MathF.Atan2(difference.Y, difference.X);
            double t;

            switch (h)
            {
                case Slider slider:
                    AddFrameToReplay(new OsuReplayFrame(slider.StartTime, slider.StackedPosition, action));

                    if (objectsDuring[idx]) break;

                    if (slider.Distance / slider.RepeatCount <= 38 && slider.RepeatCount >= 1)
                    {
                        AddFrameToReplay(new OsuReplayFrame(h.StartTime, h.StackedPosition, action));
                        AddFrameToReplay(new OsuReplayFrame(h.StartTime + slider.Duration * 0.6, h.StackedPosition, action));
                    }
                    else
                    {
                        double speed = slider.Distance / slider.Duration;

                        for (double j = FrameDelay; j < slider.Duration; j += FrameDelay)
                        {
                            bool canDance = sliderDance && slider.Duration > 350;
                            Vector2 pos = slider.StackedPositionAt(j / slider.Duration);
                            Vector2 pos2 = pos + CirclePosition(ApplyModsToTime(j) / 12 * speed + angle, 20);
                            AddFrameToReplay(new OsuReplayFrame((int)h.StartTime + j, canDance ? pos2 : pos, action));
                        }
                    }

                    break;

                case Spinner spinner:
                    if(spinner.SpinsRequired == 0) return;
                    double r = spinner.SpinsRequired > 3 ? spinRadiusStart : spinRadiusEnd;
                    double r1;
                    double rEndTime = spinner.StartTime + spinner.Duration * 0.6;
                    float spinDirection;
                    calcSpinnerStartPosAndDirection(((OsuReplayFrame)Frames[^1]).Position, out startPosition, out spinDirection);

                    for (double j = spinner.StartTime + FrameDelay; j < spinner.EndTime; j += FrameDelay)
                    {
                        t = ApplyModsToTime(j - h.StartTime) * spinDirection;
                        r1 = j > rEndTime ? spinRadiusEnd : Interpolation.ValueAt(j, r, spinRadiusEnd, spinner.StartTime, rEndTime, Easing.In);
                        Vector2 pos = SPINNER_CENTRE + CirclePosition(t / 20 + angle, r1);
                        AddFrameToReplay(new OsuReplayFrame((int)j, new Vector2(pos.X, pos.Y), action));
                    }

                    break;

                case HitCircle circle:
                    AddFrameToReplay(new OsuReplayFrame(circle.StartTime, circle.StackedPosition, action));
                    break;

                default: return;
            }
        }

        public Replay GenerateReplay(bool sliderDance)
        {
            OsuHitObject hitObject = Beatmap.HitObjects[0];
            AddFrameToReplay(new OsuReplayFrame(-10000, hitObject.StackedPosition));
            AddFrameToReplay(new OsuReplayFrame(Beatmap.HitObjects[0].StartTime, hitObject.StackedPosition));

            Vector2 base_size = OsuPlayfield.BASE_SIZE;

            float xf = base_size.X / 0.8f * (4f / 3f);
            float x0 = (base_size.X - xf) / 2f;
            float x1 = xf + x0;

            float yf = base_size.Y / 0.8f;
            float y0 = (base_size.Y - yf) / 2f;
            float y1 = yf + y0;

            for (int i = 0; i < Beatmap.HitObjects.Count - 1; i++)
            {
                OsuReplayFrame lastFrame = (OsuReplayFrame)Frames[^1];
                hitObject = Beatmap.HitObjects[i];
                moveToHitObject(hitObject, i, sliderDance);

                mover.ObjectIndex = i;
                mover.OnObjChange();

                for (double time = (objectsDuring[i] ? hitObject.StartTime : hitObject.GetEndTime()) + frameDelay; time < mover.End.StartTime; time += frameDelay)
                {
                    Start:
                    var timeToNext = mover.End.StartTime - time;

                    if (timeToNext > 3000 && lastFrame.Time < mover.End.StartTime - timeToNext * 0.6)
                    {
                        AddFrameToReplay(new OsuReplayFrame(hitObject.GetEndTime(), lastFrame.Position));
                        AddFrameToReplay(new OsuReplayFrame(mover.End.StartTime - timeToNext * 0.8, lastFrame.Position));
                        time = mover.End.StartTime - timeToNext * 0.6;
                        goto Start;
                    }

                    Vector2 currentPosition = mover.Update(time);

                    if (config.Get<bool>(MfSetting.BorderBounce))
                    {
                        if (currentPosition.X < x0) currentPosition.X = x0 - (currentPosition.X - x0);
                        if (currentPosition.Y < y0) currentPosition.Y = y0 - (currentPosition.Y - y0);

                        if (currentPosition.X > x1)
                        {
                            float x = currentPosition.X - x0;
                            int m = (int)(x / xf);
                            x %= xf;
                            x = m % 2 == 0 ? x : xf - x;
                            currentPosition.X = x + x0;
                        }

                        if (currentPosition.Y > y1)
                        {
                            float y = currentPosition.Y - y0;
                            float m = (int)(y / yf);
                            y %= yf;
                            y = m % 2 == 0 ? y : yf - y;
                            currentPosition.Y = y + y0;
                        }
                    }

                    AddFrameToReplay(new OsuReplayFrame(time, currentPosition));
                }
            }

            moveToHitObject(Beatmap.HitObjects[^1], Beatmap.HitObjects.Count - 1, sliderDance);

            return Replay;
        }

        public override Replay Generate()
        {
            return GenerateReplay(sliderDance);
        }
    }

    public abstract class DanceMover
    {
        protected double StartTime => Start.GetEndTime();
        protected double EndTime => End.StartTime;
        protected double Duration => EndTime - StartTime;

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

        public OsuHitObject Start
        {
            get
            {
                if (Beatmap.HitObjects[ObjectIndex] is Spinner spinner && spinner.SpinsRequired == 0) return Beatmap.HitObjects[ObjectIndex - 1];

                return Beatmap.HitObjects[ObjectIndex];
            }
        }

        public OsuHitObject End
        {
            get
            {
                if (Beatmap.HitObjects[ObjectIndex + 1] is Spinner spinner && spinner.SpinsRequired == 0) return Beatmap.HitObjects[ObjectIndex];

                return Beatmap.HitObjects[ObjectIndex + 1];
            }
        }

        public virtual void OnObjChange() { }
        public abstract Vector2 Update(double time);
    }
}
