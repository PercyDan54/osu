// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Replays.Movers;
using osu.Game.Rulesets.Osu.UI;
using osuTK;
using static osu.Game.Configuration.OsuDanceMover;
using static osu.Game.Rulesets.Osu.Replays.Movers.MoverUtilExtensions;

namespace osu.Game.Rulesets.Osu.Replays
{
    public class OsuDanceGenerator : OsuAutoGeneratorBase
    {
        public static Mover GetMover(OsuDanceMover mover) =>
            mover switch
            {
                AxisAligned => new AxisAlignedMover(),
                Aggresive => new AggressiveMover(),
                Bezier => new BezierMover(),
                HalfCircle => new HalfCircleMover(),
                Flower => new FlowerMover(),
                Pippi => new PippiMover(),
                Linear => new LinearMover(),
                _ => new MomentumMover()
            };

        public new OsuBeatmap Beatmap => (OsuBeatmap)base.Beatmap;
        private readonly Mover mover;
        private readonly float spinRadiusStart;
        private readonly float spinRadiusEnd;
        private readonly bool sliderDance;
        private readonly bool pippiSpinner;
        private readonly bool pippiStream;
        private readonly bool skipShortSliders;
        private bool isStream;
        private readonly MConfigManager config;
        private readonly double frameDelay;
        private int buttonIndex;
        private List<OsuHitObject> hitObjects = new List<OsuHitObject>();
        private readonly bool isPippi;

        public OsuDanceGenerator(IBeatmap beatmap, IReadOnlyList<Mod> mods)
            : base(beatmap, mods)
        {
            config = MConfigManager.Instance;
            mover = GetMover(config.Get<OsuDanceMover>(MSetting.DanceMover));
            isPippi = mover is PippiMover;
            frameDelay = 1000.0 / config.Get<float>(MSetting.ReplayFramerate);
            spinRadiusStart = config.Get<float>(MSetting.SpinnerRadiusStart);
            spinRadiusEnd = config.Get<float>(MSetting.SpinnerRadiusEnd);
            sliderDance = config.Get<bool>(MSetting.SliderDance);
            pippiSpinner = config.Get<bool>(MSetting.PippiSpinner) || isPippi;
            pippiStream = config.Get<bool>(MSetting.PippiStream);
            skipShortSliders = config.Get<bool>(MSetting.SkipShortSlider);
            mover.TimeAffectingMods = mods.OfType<IApplicableToRate>().ToList();
            preProcessObjects();
        }

        private void preProcessObjects()
        {
            for (int i = 0; i < Beatmap.HitObjects.Count; ++i)
            {
                var h = Beatmap.HitObjects[i];

                if (h is Spinner { SpinsRequired: 0 })
                    continue;

                hitObjects.Add(h);
            }

            hitObjects = hitObjects.OrderBy(h => h.StartTime).ToList();
            mover.HitObjects = hitObjects;
        }

        private OsuAction getAction(OsuHitObject h, OsuHitObject last)
        {
            double timeDifference = ApplyModsToTimeDelta(last.StartTime, h.StartTime);

            if (timeDifference > 0 && timeDifference < 266)
                buttonIndex++;
            else
                buttonIndex = 0;

            return buttonIndex % 2 == 0 ? OsuAction.LeftButton : OsuAction.RightButton;
        }

        private void addHitObjectClickFrames(OsuHitObject h, int index)
        {
            OsuHitObject last = hitObjects[Math.Max(0, index - 1)];
            OsuAction action = getAction(h, last);
            Vector2 startPosition = h.StackedPosition;
            Vector2 difference = startPosition - SPINNER_CENTRE;
            float radius = difference.Length;
            float angle = radius == 0 ? 0 : MathF.Atan2(difference.Y, difference.X);
            Vector2 pos;

            switch (h)
            {
                case Slider slider:
                    AddFrameToReplay(new OsuReplayFrame(slider.StartTime, slider.StackedPosition, action));

                    var points = slider.NestedHitObjects.SkipWhile(p => p is SliderRepeat).Cast<OsuHitObject>()
                                       .OrderBy(p => p.StartTime)
                                       .ToList();

                    var mid = slider.StackedEndPosition;

                    if (skipShortSliders && slider.RepeatCount == 1 || Math.Abs(Vector2.Distance(startPosition, mid)) <= h.Radius * 1.6)
                    {
                        mid = slider.Path.PositionAt(0.5);

                        if (Vector2.Distance(startPosition, mid) <= h.Radius * 1.6)
                        {
                            AddFrameToReplay(new OsuReplayFrame(slider.EndTime, mid, action));
                            return;
                        }
                    }

                    if (sliderDance && points.Count > 2)
                    {
                        for (int i = 0; i < points.Count - 1; i++)
                        {
                            var point = points[i];
                            var next = points[i + 1];
                            double duration = next.StartTime - point.StartTime;

                            if (i == points.Count - 2)
                                duration += 36;

                            for (double j = GetFrameDelay(point.StartTime); j < duration; j += GetFrameDelay(point.StartTime + j))
                            {
                                double scaleFactor = j / duration;
                                pos = point.StackedPosition + (next.StackedPosition - point.StackedPosition) * (float)scaleFactor;

                                addPippiFrame(new OsuReplayFrame(point.StartTime + j, pos, action), isPippi ? -1 : 0);
                            }
                        }
                    }
                    else
                    {
                        for (double j = GetFrameDelay(slider.StartTime); j < slider.Duration; j += GetFrameDelay(slider.StartTime + j))
                        {
                            pos = slider.StackedPositionAt(j / slider.Duration);
                            addPippiFrame(new OsuReplayFrame(h.StartTime + j, pos, action), isPippi ? -1 : 0);
                        }
                    }

                    break;

                case Spinner spinner:
                    double radiusStart = spinner.SpinsRequired > 3 ? spinRadiusStart : spinRadiusEnd;
                    double rEndTime = spinner.StartTime + spinner.Duration * 0.7;
                    double previousFrame = h.StartTime;

                    for (double nextFrame = h.StartTime + GetFrameDelay(h.StartTime); nextFrame < spinner.EndTime; nextFrame += ApplyModsToRate(nextFrame, frameDelay))
                    {
                        double t = ApplyModsToTimeDelta(previousFrame, nextFrame) * -1;
                        angle += (float)t / 20;
                        double r = nextFrame > rEndTime ? spinRadiusEnd : Interpolation.ValueAt(nextFrame, radiusStart, spinRadiusEnd, spinner.StartTime, rEndTime, Easing.In);
                        pos = SPINNER_CENTRE + CirclePosition(angle, r);
                        addPippiFrame(new OsuReplayFrame((int)nextFrame, pos, action), pippiSpinner ? (float)r : 0);

                        previousFrame = nextFrame;
                    }

                    break;

                default:
                    addPippiFrame(new OsuReplayFrame(h.StartTime, mover.Update(h.StartTime), action), isStream ? -1 : 0);
                    break;
            }
        }

        public override Replay Generate()
        {
            OsuHitObject h = hitObjects[0];

            AddFrameToReplay(new OsuReplayFrame(-10000, h.StackedPosition));

            Vector2 baseSize = OsuPlayfield.BASE_SIZE;

            float xf = baseSize.X / 0.8f * (4f / 3f);
            float x0 = (baseSize.X - xf) / 2f;
            float x1 = xf + x0;

            float yf = baseSize.Y / 0.8f;
            float y0 = (baseSize.Y - yf) / 2f;
            float y1 = yf + y0;
            mover.OnObjChange();

            for (int i = 0; i < hitObjects.Count; i++)
            {
                h = hitObjects[i];
                mover.StartPos = h.StackedEndPosition;
                var next = hitObjects[Math.Min(i + 1, hitObjects.Count - 1)];
                isStream = pippiStream && IsStream(h, next, hitObjects[Math.Min(hitObjects.Count - 1, i + 2)]) && !(mover is PippiMover);
                mover.EndPos = next.StackedPosition;
                mover.ObjectIndex = Math.Min(hitObjects.Count - 2, i);
                mover.OnObjChange();
                addHitObjectClickFrames(h, i);

                for (double time = h.GetEndTime() + frameDelay; time < mover.End.StartTime; time += frameDelay)
                {
                    var currentPosition = ApplyPippiOffset(mover.Update(time), time, isStream ? -1 : 0);

                    if (config.Get<bool>(MSetting.BorderBounce))
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

            addHitObjectClickFrames(hitObjects[^1], hitObjects.Count - 1);
            var lastFrame = (OsuReplayFrame)Frames[^1];
            lastFrame.Actions.Clear();
            AddFrameToReplay(lastFrame);

            return Replay;
        }

        protected override HitObject GetNextObject(int currentIndex)
        {
            var next = hitObjects[Math.Min(Beatmap.HitObjects.Count - 1, currentIndex + 1)];

            return next;
        }

        private void addPippiFrame(OsuReplayFrame frame, float radius)
        {
            frame.Position = ApplyPippiOffset(frame.Position, frame.Time, radius);
            AddFrameToReplay(frame);
        }
    }
}
