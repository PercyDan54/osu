// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Input.StateChanges;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Replays;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModAutopilotDance : OsuModAutopilot
    {
        public override string Name => "Autopilot (With Dance)";
        public override Type[] IncompatibleMods => base.IncompatibleMods.Append(typeof(OsuModAutopilot)).ToArray();

        private OsuInputManager inputManager;

        private IFrameStableClock gameplayClock;

        private List<OsuReplayFrame> replayFrames;

        private int currentFrame;

        public override void Update(Playfield playfield)
        {
            if (currentFrame == replayFrames.Count - 1) return;

            double time = gameplayClock.CurrentTime;

            // Very naive implementation of autopilot based on proximity to replay frames.
            // TODO: this needs to be based on user interactions to better match stable (pausing until judgement is registered).
            if (Math.Abs(replayFrames[currentFrame + 1].Time - time) <= Math.Abs(replayFrames[currentFrame].Time - time))
            {
                currentFrame++;
                new MousePositionAbsoluteInput { Position = playfield.ToScreenSpace(replayFrames[currentFrame].Position) }.Apply(inputManager.CurrentState, inputManager);
            }

            // TODO: Implement the functionality to automatically spin spinners
        }

        public override void ApplyToDrawableRuleset(DrawableRuleset<OsuHitObject> drawableRuleset)
        {
            gameplayClock = drawableRuleset.FrameStableClock;

            // Grab the input manager to disable the user's cursor, and for future use
            inputManager = (OsuInputManager)drawableRuleset.KeyBindingInputManager;
            inputManager.AllowUserCursorMovement = false;

            // Generate the replay frames the cursor should follow
            replayFrames = new OsuDanceGenerator(drawableRuleset.Beatmap, drawableRuleset.Mods).Generate().Frames.Cast<OsuReplayFrame>().ToList();
        }
    }
}
