// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Rulesets.Osu.Configuration;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Replays.Movers;
using osu.Game.Rulesets.Osu.Replays.Movers.Sliders;
using osu.Game.Rulesets.Osu.UI;
using osuTK;
using static osu.Game.Rulesets.Osu.Configuration.OsuDanceMover;

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
        private readonly BaseDanceObjectMover<Slider> sliderMover;
        private readonly OsuRulesetConfigManager config;

        private bool tapRight;
        private readonly double frameTime;

        // yes, this is intended.
        // ReSharper disable once AssignmentInConditionalExpression
        private OsuAction action() => (tapRight = !tapRight) ? OsuAction.LeftButton : OsuAction.RightButton;

        public OsuDanceGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
            config = OsuRulesetConfigManager.Instance;
            frameTime = 1000.0 / config.Get<float>(OsuRulesetSetting.ReplayFramerate);
            spinRadiusStart = config.Get<float>(OsuRulesetSetting.SpinnerRadius);
            spinRadiusEnd = config.Get<float>(OsuRulesetSetting.SpinnerRadius2);
            mover = GetMover(config.Get<OsuDanceMover>(OsuRulesetSetting.DanceMover));
            sliderMover = new SimpleSliderMover();

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

        private void objectGenerate(OsuHitObject o, int idx)
        {
            float sDirection = -1;
            var tap = action();
            switch (o)
            {
                case Slider s:
                    sliderMover.Object = s;
                    sliderMover.OnObjChange();
                    AddFrameToReplay(new OsuReplayFrame(s.StartTime, s.StackedPosition, tap));

                    if (objectsDuring[idx]) break;

                    for (double t = s.StartTime + frameTime; t < s.EndTime; t += frameTime)
                        AddFrameToReplay(new OsuReplayFrame(t, sliderMover.Update(t), tap));

                    break;

                case Spinner s:
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
                        AddFrameToReplay(new OsuReplayFrame((int)j, new Vector2(pos.X, pos.Y), tap));
                    }
                    break;

                case HitCircle c:
                    AddFrameToReplay(new OsuReplayFrame(c.StartTime, c.StackedPosition, tap));
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

                    if (config.Get<bool>(OsuRulesetSetting.BorderBounce))
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
