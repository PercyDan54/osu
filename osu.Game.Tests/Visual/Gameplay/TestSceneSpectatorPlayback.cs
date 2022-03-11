// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Input.StateChanges;
using osu.Framework.Logging;
using osu.Framework.Testing;
using osu.Framework.Timing;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Spectator;
using osu.Game.Replays;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Screens.Play;
using osu.Game.Tests.Visual.Spectator;
using osu.Game.Tests.Visual.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Tests.Visual.Gameplay
{
    public class TestSceneSpectatorPlayback : OsuManualInputManagerTestScene
    {
        private TestRulesetInputManager playbackManager;
        private TestRulesetInputManager recordingManager;

        private Replay replay;

        private TestSpectatorClient spectatorClient;

        private ManualClock manualClock;

        private TestReplayRecorder recorder;

        private OsuSpriteText latencyDisplay;

        private TestFramedReplayInputHandler replayHandler;

        [SetUpSteps]
        public void SetUpSteps()
        {
            AddStep("Setup containers", () =>
            {
                replay = new Replay();
                manualClock = new ManualClock();

                Child = new DependencyProvidingContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    CachedDependencies = new[]
                    {
                        (typeof(SpectatorClient), (object)(spectatorClient = new TestSpectatorClient())),
                        (typeof(GameplayState), new GameplayState(new Beatmap(), new OsuRuleset(), Array.Empty<Mod>()))
                    },
                    Children = new Drawable[]
                    {
                        spectatorClient,
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    recordingManager = new TestRulesetInputManager(TestSceneModSettings.CreateTestRulesetInfo(), 0, SimultaneousBindingMode.Unique)
                                    {
                                        Recorder = recorder = new TestReplayRecorder
                                        {
                                            ScreenSpaceToGamefield = pos => recordingManager.ToLocalSpace(pos),
                                        },
                                        Child = new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Children = new Drawable[]
                                            {
                                                new Box
                                                {
                                                    Colour = Color4.Brown,
                                                    RelativeSizeAxes = Axes.Both,
                                                },
                                                new OsuSpriteText
                                                {
                                                    Text = "Sending",
                                                    Scale = new Vector2(3),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                },
                                                new TestInputConsumer()
                                            }
                                        },
                                    }
                                },
                                new Drawable[]
                                {
                                    playbackManager = new TestRulesetInputManager(TestSceneModSettings.CreateTestRulesetInfo(), 0, SimultaneousBindingMode.Unique)
                                    {
                                        Clock = new FramedClock(manualClock),
                                        ReplayInputHandler = replayHandler = new TestFramedReplayInputHandler(replay)
                                        {
                                            GamefieldToScreenSpace = pos => playbackManager.ToScreenSpace(pos),
                                        },
                                        Child = new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Children = new Drawable[]
                                            {
                                                new Box
                                                {
                                                    Colour = Color4.DarkBlue,
                                                    RelativeSizeAxes = Axes.Both,
                                                },
                                                new OsuSpriteText
                                                {
                                                    Text = "Receiving",
                                                    Scale = new Vector2(3),
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                },
                                                new TestInputConsumer()
                                            }
                                        },
                                    }
                                }
                            }
                        },
                        latencyDisplay = new OsuSpriteText()
                    }
                };

                spectatorClient.OnNewFrames += onNewFrames;
            });
        }

        [Test]
        public void TestBasic()
        {
            AddUntilStep("received frames", () => replay.Frames.Count > 50);
            AddStep("stop sending frames", () => recorder.Expire());
            AddUntilStep("wait for all frames received", () => replay.Frames.Count == recorder.SentFrames.Count);
        }

        [Test]
        public void TestWithSendFailure()
        {
            AddUntilStep("received frames", () => replay.Frames.Count > 50);

            int framesReceivedSoFar = 0;
            int frameSendAttemptsSoFar = 0;

            AddStep("start failing sends", () =>
            {
                spectatorClient.ShouldFailSendingFrames = true;
                framesReceivedSoFar = replay.Frames.Count;
                frameSendAttemptsSoFar = spectatorClient.FrameSendAttempts;
            });

            AddUntilStep("wait for send attempts", () => spectatorClient.FrameSendAttempts > frameSendAttemptsSoFar + 5);
            AddAssert("frames did not increase", () => framesReceivedSoFar == replay.Frames.Count);

            AddStep("stop failing sends", () => spectatorClient.ShouldFailSendingFrames = false);

            AddUntilStep("wait for next frames", () => framesReceivedSoFar < replay.Frames.Count);

            AddStep("stop sending frames", () => recorder.Expire());

            AddUntilStep("wait for all frames received", () => replay.Frames.Count == recorder.SentFrames.Count);
            AddAssert("ensure frames were received in the correct sequence", () => replay.Frames.Select(f => f.Time).SequenceEqual(recorder.SentFrames.Select(f => f.Time)));
        }

        private void onNewFrames(int userId, FrameDataBundle frames)
        {
            foreach (var legacyFrame in frames.Frames)
            {
                var frame = new TestReplayFrame();
                frame.FromLegacy(legacyFrame, null);
                replay.Frames.Add(frame);
            }

            Logger.Log($"Received {frames.Frames.Count} new frames (total {replay.Frames.Count} of {recorder.SentFrames.Count})");
        }

        private double latency = SpectatorClient.TIME_BETWEEN_SENDS;

        protected override void Update()
        {
            base.Update();

            if (latencyDisplay == null) return;

            // propagate initial time value
            if (manualClock.CurrentTime == 0)
            {
                manualClock.CurrentTime = Time.Current;
                return;
            }

            if (!replayHandler.HasFrames)
                return;

            var lastFrame = replay.Frames.LastOrDefault();

            // this isn't perfect as we basically can't be aware of the rate-of-send here (the streamer is not sending data when not being moved).
            // in gameplay playback, the case where NextFrame is null would pause gameplay and handle this correctly; it's strictly a test limitation / best effort implementation.
            if (lastFrame != null)
                latency = Math.Max(latency, Time.Current - lastFrame.Time);

            latencyDisplay.Text = $"latency: {latency:N1}";

            double proposedTime = Time.Current - latency + Time.Elapsed;

            // this will either advance by one or zero frames.
            double? time = replayHandler.SetFrameFromTime(proposedTime);

            if (time == null)
                return;

            manualClock.CurrentTime = time.Value;
        }

        public class TestFramedReplayInputHandler : FramedReplayInputHandler<TestReplayFrame>
        {
            public TestFramedReplayInputHandler(Replay replay)
                : base(replay)
            {
            }

            protected override void CollectReplayInputs(List<IInput> inputs)
            {
                inputs.Add(new MousePositionAbsoluteInput { Position = GamefieldToScreenSpace(CurrentFrame?.Position ?? Vector2.Zero) });
                inputs.Add(new ReplayState<TestAction> { PressedActions = CurrentFrame?.Actions ?? new List<TestAction>() });
            }
        }

        public class TestInputConsumer : CompositeDrawable, IKeyBindingHandler<TestAction>
        {
            public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => Parent.ReceivePositionalInputAt(screenSpacePos);

            private readonly Box box;

            public TestInputConsumer()
            {
                Size = new Vector2(30);

                Origin = Anchor.Centre;

                InternalChildren = new Drawable[]
                {
                    box = new Box
                    {
                        Colour = Color4.Black,
                        RelativeSizeAxes = Axes.Both,
                    },
                };
            }

            protected override bool OnMouseMove(MouseMoveEvent e)
            {
                Position = e.MousePosition;
                return base.OnMouseMove(e);
            }

            public bool OnPressed(KeyBindingPressEvent<TestAction> e)
            {
                if (e.Repeat)
                    return false;

                box.Colour = Color4.White;
                return true;
            }

            public void OnReleased(KeyBindingReleaseEvent<TestAction> e)
            {
                box.Colour = Color4.Black;
            }
        }

        public class TestRulesetInputManager : RulesetInputManager<TestAction>
        {
            public TestRulesetInputManager(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
                : base(ruleset, variant, unique)
            {
            }

            protected override KeyBindingContainer<TestAction> CreateKeyBindingContainer(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
                => new TestKeyBindingContainer();

            internal class TestKeyBindingContainer : KeyBindingContainer<TestAction>
            {
                public override IEnumerable<IKeyBinding> DefaultKeyBindings => new[]
                {
                    new KeyBinding(InputKey.MouseLeft, TestAction.Down),
                };
            }
        }

        public class TestReplayFrame : ReplayFrame, IConvertibleReplayFrame
        {
            public Vector2 Position;

            public List<TestAction> Actions = new List<TestAction>();

            public TestReplayFrame(double time, Vector2 position, params TestAction[] actions)
                : base(time)
            {
                Position = position;
                Actions.AddRange(actions);
            }

            public TestReplayFrame()
            {
            }

            public void FromLegacy(LegacyReplayFrame currentFrame, IBeatmap beatmap, ReplayFrame lastFrame = null)
            {
                Position = currentFrame.Position;
                Time = currentFrame.Time;
                if (currentFrame.MouseLeft)
                    Actions.Add(TestAction.Down);
            }

            public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
            {
                ReplayButtonState state = ReplayButtonState.None;

                if (Actions.Contains(TestAction.Down))
                    state |= ReplayButtonState.Left1;

                return new LegacyReplayFrame(Time, Position.X, Position.Y, state);
            }
        }

        public enum TestAction
        {
            Down,
        }

        internal class TestReplayRecorder : ReplayRecorder<TestAction>
        {
            public List<ReplayFrame> SentFrames = new List<ReplayFrame>();

            public TestReplayRecorder()
                : base(new Score
                {
                    ScoreInfo =
                    {
                        BeatmapInfo = new BeatmapInfo(),
                        Ruleset = new OsuRuleset().RulesetInfo,
                    }
                })
            {
            }

            protected override ReplayFrame HandleFrame(Vector2 mousePosition, List<TestAction> actions, ReplayFrame previousFrame)
            {
                var testReplayFrame = new TestReplayFrame(Time.Current, mousePosition, actions.ToArray());
                SentFrames.Add(testReplayFrame);
                return testReplayFrame;
            }
        }
    }
}
