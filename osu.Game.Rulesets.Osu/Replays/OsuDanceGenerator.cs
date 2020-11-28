// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Scoring;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Osu.Replays
{
    public class OsuDanceGenerator : OsuAutoGeneratorBase
    {
        public new OsuBeatmap Beatmap => (OsuBeatmap)base.Beatmap;

        #region Parameters

        /// <summary>
        /// If delayed movements should be used, causing the cursor to stay on each hitobject for as long as possible.
        /// Mainly for Autopilot.
        /// </summary>
        public bool DelayedMovements; // ModManager.CheckActive(Mods.Relax2);

        private float offsetX;
        private float offsetY;
        private float lastoffsetX;
        private float lastoffsetY;
        #endregion

        #region Constants

        /// <summary>
        /// The "reaction time" in ms between "seeing" a new hit object and moving to "react" to it.
        /// </summary>
        private readonly double reactionTime;

        private readonly HitWindows defaultHitWindows;

        /// <summary>
        /// What easing to use when moving between hitobjects
        /// </summary>
        private Easing preferredEasing => Easing.Out;

        #endregion

        #region Construction / Initialisation

        public OsuDanceGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
            // Already superhuman, but still somewhat realistic
            reactionTime = ApplyModsToRate(100);

            defaultHitWindows = new OsuHitWindows();
            defaultHitWindows.SetDifficulty(Beatmap.BeatmapInfo.BaseDifficulty.OverallDifficulty);
        }

        #endregion

        #region Generator

        /// <summary>
        /// Which button (left or right) to use for the current hitobject.
        /// Even means LMB will be used to click, odd means RMB will be used.
        /// This keeps track of the button previously used for alt/singletap logic.
        /// </summary>
        private int buttonIndex;

        public override Replay Generate()
        {
            if (Beatmap.HitObjects.Count == 0)
                return Replay;

            buttonIndex = 0;

            AddFrameToReplay(new OsuReplayFrame(-100000, new Vector2(256, 500)));
            AddFrameToReplay(new OsuReplayFrame(Beatmap.HitObjects[0].StartTime - 1500, new Vector2(256, 500)));

            for (int i = 0; i < Beatmap.HitObjects.Count; i++)
            {
                OsuHitObject h = Beatmap.HitObjects[i];
                OsuHitObject next = Beatmap.HitObjects[i >= Beatmap.HitObjects.Count - 1 ? i : i + 1];
                addHitObjectReplay(h, next);
            }

            return Replay;
        }

        private void addHitObjectReplay(OsuHitObject h, OsuHitObject next)
        {
            // Default values for circles/sliders
            Vector2 startPosition = h.StackedPosition;
            Easing easing = preferredEasing;
            float spinnerDirection = -1;

            // The startPosition for the slider should not be its .Position, but the point on the circle whose tangent crosses the current cursor position
            // We also modify spinnerDirection so it spins in the direction it enters the spin circle, to make a smooth transition.
            // TODO: Shouldn't the spinner always spin in the same direction?
            if (h is Spinner spinner)
            {
                // spinners with 0 spins required will auto-complete - don't bother
                if (spinner.SpinsRequired == 0)
                    return;

                calcSpinnerStartPosAndDirection(((OsuReplayFrame)Frames[^1]).Position, out startPosition, out spinnerDirection);

                Vector2 spinCentreOffset = SPINNER_CENTRE - ((OsuReplayFrame)Frames[^1]).Position;

                if (spinCentreOffset.Length > SPIN_RADIUS)
                {
                    // If moving in from the outside, don't ease out (default eases out). This means auto will "start" spinning immediately after moving into position.
                    easing = Easing.In;
                }
            }

            // Do some nice easing for cursor movements
            if (Frames.Count > 0)
            {
                moveToHitObject(h, next, startPosition, easing);
            }

            // Add frames to click the hitobject
            addHitObjectClickFrames(h, startPosition, spinnerDirection);
        }

        #endregion

        #region Helper subroutines

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

        private void moveToHitObject(OsuHitObject h, OsuHitObject next, Vector2 targetPos, Easing easing)
        {
            OsuReplayFrame lastFrame = (OsuReplayFrame)Frames[^1];

            // Wait until Auto could "see and react" to the next note.
            double waitTime = h.StartTime - Math.Max(0.0, h.TimePreempt - reactionTime);

            if (waitTime > lastFrame.Time)
            {
                lastFrame = new OsuReplayFrame(waitTime, lastFrame.Position) { Actions = lastFrame.Actions };
                AddFrameToReplay(lastFrame);
            }

            Vector2 lastPosition = lastFrame.Position;
            lastoffsetX = offsetX;
            lastoffsetY = offsetY;
            offsetX = 40 * (float)h.Scale;
            offsetY = 40 * (float)h.Scale;
            double timeDifference = ApplyModsToTime(h.StartTime - lastFrame.Time);
            double timeToNext = next.StartTime - h.GetEndTime();
            float dist = (lastPosition - targetPos).Length;
            //Snap to the edge of HitObjects
            if (h.StackedPosition.X - lastPosition.X > 15 && dist < 200)
            {
                offsetX *= -1;
            }
            if (h.StackedPosition.Y - lastPosition.Y > 15 && dist < 200)
            {
                offsetY *= -1;
            }
            //Make move distance longer when hitting jumps
            if (dist > 200)
            {
                offsetX *= -1;
                offsetY *= -1;
            }
            //Don't change offset when hitting streams
            if (buttonIndex > 1 && dist < 50 && !(h is Slider))
            {
                offsetX = lastoffsetX;
                offsetY = lastoffsetY;
            }
            //Don't set offset for short sliders
            if (h is Slider && (h as Slider).Distance < 50)
            {
                offsetX = 0;
                offsetY = 0;
            }
            Vector2 offset = new Vector2(offsetX, offsetY);
            Vector2 difference = h.StackedEndPosition - SPINNER_CENTRE;
            float radius = difference.Length;
            float angle = radius == 0 ? 0 : MathF.Atan2(difference.Y, difference.X);
            targetPos += offset;
            double t;
            for (double time = lastFrame.Time + FrameDelay; time < h.StartTime; time += FrameDelay)
            {
                Vector2 currentPosition = Interpolation.ValueAt(time, lastPosition, targetPos, lastFrame.Time, h.StartTime, easing);
                if (dist > 120 && timeToNext < 400)
                {
                    t = ApplyModsToTime(time - h.StartTime) * offsetX / (32 * h.Scale);
                    Vector2 pos2 = currentPosition + CirclePosition(t / 60 + angle, 50);
                    AddFrameToReplay(new OsuReplayFrame(time, pos2) { Actions = lastFrame.Actions });
                }
                else
                {
                    AddFrameToReplay(new OsuReplayFrame(time, currentPosition) { Actions = lastFrame.Actions });
                }

            }
            //Draw circles when two HitObject's delay is long enough, but not during long breaks
            if (timeToNext > 400)
            {
                if (timeToNext < 2000)
                {
                    for (double j = h.GetEndTime() + FrameDelay; j < next.StartTime - reactionTime; j += FrameDelay)
                    {
                        if (h is Slider)
                        {
                            targetPos = h.StackedEndPosition + offset;
                        }
                        t = ApplyModsToTime(j - h.StartTime) * h.ComboIndex % 2 == 0 ? 1 : -1;
                        Vector2 pos = Interpolation.ValueAt(j, targetPos, next.StackedPosition + offset, h.GetEndTime(), next.StartTime - reactionTime, easing);
                        Vector2 pos2 = pos + CirclePosition(t / 120 + angle, 50);
                        AddFrameToReplay(new OsuReplayFrame((int)j, new Vector2(pos2.X, pos2.Y)));
                    }

                }
                else
                {
                    AddFrameToReplay(lastFrame);
                }

            }
            // Only "snap" to hitcircles if they are far enough apart. As the time between hitcircles gets shorter the snapping threshold goes up.
            if (timeDifference > 0 && // Sanity checks
                (dist > h.Radius * (1.5 + 100.0 / timeDifference) || // Either the distance is big enough
                 timeDifference >= 266)) // ... or the beats are slow enough to tap anyway.
            {
                buttonIndex = 0;
            }
            else
            {
                buttonIndex++;
            }
        }
        // Add frames to click the hitobject
        private void addHitObjectClickFrames(OsuHitObject h, Vector2 startPosition, float spinnerDirection)
        {
            // Time to insert the first frame which clicks the object
            // Here we mainly need to determine which button to use
            var action = buttonIndex % 2 == 0 ? OsuAction.LeftButton : OsuAction.RightButton;

            OsuReplayFrame lastFrame = (OsuReplayFrame)Frames[^1];
            Vector2 lastPosition = lastFrame.Position;
            double timeDifference = ApplyModsToTime(h.StartTime - lastFrame.Time);

            var startFrame = new OsuReplayFrame(h.StartTime, new Vector2(h.StackedPosition.X + offsetX, h.StackedPosition.Y + offsetY), action);

            // TODO: Why do we delay 1 ms if the object is a spinner? There already is KEY_UP_DELAY from hEndTime.
            double hEndTime = h.GetEndTime() + 15;
            int endDelay = h is Spinner ? 1 : 0;
            var endFrame = new OsuReplayFrame(hEndTime + endDelay, new Vector2(h.StackedEndPosition.X + offsetX, h.StackedEndPosition.Y + offsetY));

            // Decrement because we want the previous frame, not the next one
            int index = FindInsertionIndex(startFrame) - 1;

            // If the previous frame has a button pressed, force alternation.
            // If there are frames ahead, modify those to use the new button press.
            // Do we have a previous frame? No need to check for < replay.Count since we decremented!
            if (index >= 0)
            {
                var previousFrame = (OsuReplayFrame)Frames[index];
                var previousActions = previousFrame.Actions;

                // If a button is already held, then we simply alternate
                if (previousActions.Any())
                {
                    // Force alternation if we have the same button. Otherwise we can just keep the naturally to us assigned button.
                    if (previousActions.Contains(action))
                    {
                        action = action == OsuAction.LeftButton ? OsuAction.RightButton : OsuAction.LeftButton;
                        startFrame.Actions.Clear();
                        startFrame.Actions.Add(action);
                    }

                    // We always follow the most recent slider / spinner, so remove any other frames that occur while it exists.
                    int endIndex = FindInsertionIndex(endFrame);

                    if (index < Frames.Count - 1)
                        Frames.RemoveRange(index + 1, Math.Max(0, endIndex - (index + 1)));

                    // After alternating we need to keep holding the other button in the future rather than the previous one.
                    for (int j = index + 1; j < Frames.Count; ++j)
                    {
                        var frame = (OsuReplayFrame)Frames[j];

                        // Don't affect frames which stop pressing a button!
                        if (j < Frames.Count - 1 || frame.Actions.SequenceEqual(previousActions))
                        {
                            frame.Actions.Clear();
                            frame.Actions.Add(action);
                        }
                    }
                }
            }

            AddFrameToReplay(startFrame);

            switch (h)
            {
                // We add intermediate frames for spinning / following a slider here.
                case Spinner spinner:
                    Vector2 difference = startPosition - SPINNER_CENTRE;

                    float radius = difference.Length;
                    float angle = radius == 0 ? 0 : MathF.Atan2(difference.Y, difference.X);

                    double t;
                    double r = spinner.SpinsRequired > 3 ? 235 : SPIN_RADIUS;
                    double r1;
                    double rEndTime = h.StartTime + (spinner.Duration * 0.6);
                    for (double j = h.StartTime + FrameDelay; j < spinner.EndTime; j += FrameDelay)
                    {
                        t = ApplyModsToTime(j - h.StartTime) * spinnerDirection;
                        r1 = j > rEndTime ? 15 : Interpolation.ValueAt(j, r, 15, spinner.StartTime, rEndTime, Easing.In);
                        Vector2 pos = SPINNER_CENTRE + CirclePosition(t / 20 + angle, r1);
                        AddFrameToReplay(new OsuReplayFrame((int)j, new Vector2(pos.X, pos.Y), action));
                    }
                    break;

                case Slider slider:
                {
                    if (slider.Distance / slider.RepeatCount < 40 && slider.RepeatCount >= 1)
                    {
                        AddFrameToReplay(new OsuReplayFrame((int)h.StartTime, h.StackedPosition, action));
                    }
                    else
                    {
                        if (slider.Duration > 300)
                        {
                            double t1;
                            double speed = slider.Distance / slider.Duration;
                            for (double j = FrameDelay; j < slider.Duration; j += FrameDelay)
                            {

                                Vector2 difference1 = startPosition - SPINNER_CENTRE;
                                float radius1 = difference1.Length;
                                float angle1 = radius1 == 0 ? 0 : MathF.Atan2(difference1.Y, difference1.X);
                                t1 = ApplyModsToTime(j) * offsetX / 32;
                                Vector2 pos = slider.StackedPositionAt(j / slider.Duration);
                                Vector2 pos2 = pos + CirclePosition(t1 / 12 * speed + angle1, 15);
                                AddFrameToReplay(new OsuReplayFrame((int)h.StartTime + j, new Vector2(pos2.X + offsetX, pos2.Y + offsetY), action));
                            }
                        }
                        else
                        {
                            for (double j = FrameDelay; j < slider.Duration; j += FrameDelay)
                            {
                                Vector2 pos = slider.StackedPositionAt(j / slider.Duration);
                                AddFrameToReplay(new OsuReplayFrame(h.StartTime + j, new Vector2(pos.X + offsetX + 10 * offsetX / 32, pos.Y + offsetY + 10 * offsetY / 32), action));
                            }
                        }
                    }
                    AddFrameToReplay(new OsuReplayFrame(slider.EndTime, new Vector2(slider.StackedEndPosition.X + offsetX + 10 * offsetX / 32, slider.StackedEndPosition.Y + offsetY + 10 * offsetY / 32), action));
                    break;
                }
            }
            // We only want to let go of our button if we are at the end of the current replay. Otherwise something is still going on after us so we need to keep the button pressed!
            if (Frames[^1].Time <= endFrame.Time)
                AddFrameToReplay(endFrame);
        }
        #endregion
    }
}
