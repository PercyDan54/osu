// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Scoring;
using osu.Game.Screens.OnlinePlay.Multiplayer.Spectate;
using osu.Game.Screens.Play;
using osu.Game.Screens.Play.HUD;

namespace osu.Game.Screens.ReplayVs
{
    public class ReplayVsScreen : OsuScreen
    {
        // Isolates beatmap/ruleset to this screen.
        public override bool DisallowExternalBeatmapRulesetChanges => true;

        public override bool HideOverlaysOnEnter => true;

        // We are managing our own adjustments. For now, this happens inside the Player instances themselves.
        public override bool? AllowTrackAdjustments => false;
        public override bool AllowBackButton => false;

        /// <summary>
        /// Whether all spectating players have finished loading.
        /// </summary>
        public bool AllPlayersLoaded => instances.All(p => p?.PlayerLoaded == true);

        [Resolved]
        private OsuColour colours { get; set; }

        private readonly WorkingBeatmap beatmap;
        private readonly PlayerArea[] instances;
        private MasterGameplayClockContainer masterClockContainer;
        private ISyncManager syncManager;
        private PlayerGrid grid;
        private PlayerArea currentAudioSource;
        private bool canStartMasterClock;
        private readonly int replayCount;
        private readonly Score[] teamRedScores;
        private readonly Score[] teamBlueScores;
        private readonly BindableInt teamRedScore = new BindableInt();
        private readonly BindableInt teamBlueScore = new BindableInt();

        public ReplayVsScreen(Score[] teamRedScores, Score[] teamBlueScores, WorkingBeatmap beatmap)
        {
            replayCount = teamRedScores.Length + teamBlueScores.Length;
            instances = new PlayerArea[replayCount];
            this.teamRedScores = teamRedScores;
            this.teamBlueScores = teamBlueScores;
            this.beatmap = beatmap;
        }

        protected override void LoadComplete()
        {
            Container scoreDisplayContainer;
            Beatmap.Value = beatmap;
            masterClockContainer = new MasterGameplayClockContainer(Beatmap.Value, 0);

            InternalChildren = new[]
            {
                (Drawable)(syncManager = new CatchUpSyncManager(masterClockContainer)),
                masterClockContainer.WithChild(new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RowDimensions = new[] { new Dimension(GridSizeMode.AutoSize) },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            scoreDisplayContainer = new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y
                            },
                        },
                        new Drawable[]
                        {
                            grid = new PlayerGrid { RelativeSizeAxes = Axes.Both }
                        },
                    }
                }),
                new HoldForMenuButton
                {
                    Action = this.Exit,
                    Padding = new MarginPadding
                    {
                        Bottom = 20
                    },
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                }
            };

            for (int i = 0; i < teamRedScores.Length; i++)
            {
                grid.Add(instances[i] = new PlayerArea(-1, masterClockContainer.GameplayClock, true, colours.Red));
                syncManager.AddPlayerClock(instances[i].GameplayClock);
            }

            for (int i = teamRedScores.Length; i < replayCount; i++)
            {
                grid.Add(instances[i] = new PlayerArea(-1, masterClockContainer.GameplayClock, true, colours.Blue));
                syncManager.AddPlayerClock(instances[i].GameplayClock);
            }

            LoadComponentAsync(new MatchScoreDisplay
            {
                Team1Score = { BindTarget = teamRedScore },
                Team2Score = { BindTarget = teamBlueScore },
            }, scoreDisplayContainer.Add);

            base.LoadComplete();

            masterClockContainer.Reset();
            masterClockContainer.Stop();

            syncManager.ReadyToStart += onReadyToStart;
            syncManager.MasterState.BindValueChanged(onMasterStateChanged, true);

            for (int i = 0; i < teamRedScores.Length; i++)
            {
                instances[i].LoadScore(teamRedScores[i]);
            }

            for (int i = 0; i < teamBlueScores.Length; i++)
            {
                instances[i + teamRedScores.Length].LoadScore(teamBlueScores[i]);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!isCandidateAudioSource(currentAudioSource?.GameplayClock))
            {
                currentAudioSource = instances.Where(i => isCandidateAudioSource(i.GameplayClock))
                                              .OrderBy(i => Math.Abs(i.GameplayClock.CurrentTime - syncManager.MasterClock.CurrentTime))
                                              .FirstOrDefault();

                foreach (var instance in instances)
                    instance.Mute = instance != currentAudioSource;
            }

            if (!AllPlayersLoaded) return;

            int team1Score = 0;
            int team2Score = 0;

            for (int i = 0; i < teamRedScores.Length; i++)
            {
                if (instances[i].Player.ScoreProcessor != null)
                {
                    team1Score += Convert.ToInt32(instances[i].Player.ScoreProcessor.TotalScore.Value);
                }
            }

            for (int i = teamRedScores.Length; i < replayCount; i++)
            {
                if (instances[i].Player.ScoreProcessor != null)
                {
                    team2Score += Convert.ToInt32(instances[i].Player.ScoreProcessor.TotalScore.Value);
                }
            }

            teamRedScore.Value = team1Score;
            teamBlueScore.Value = team2Score;
        }

        private bool isCandidateAudioSource([CanBeNull] ISpectatorPlayerClock clock)
            => clock?.IsRunning == true && !clock.IsCatchingUp && !clock.WaitingOnFrames.Value;

        private void onReadyToStart()
        {
            // Seek the master clock to the gameplay time.
            // This is chosen as the first available frame in the players' replays, which matches the seek by each individual SpectatorPlayer.
            double startTime = instances.Where(i => i.Score != null)
                                        .SelectMany(i => i.Score.Replay.Frames)
                                        .Select(f => f.Time)
                                        .DefaultIfEmpty(0)
                                        .Min();

            masterClockContainer.Seek(startTime);
            masterClockContainer.Start();

            // Although the clock has been started, this flag is set to allow for later synchronisation state changes to also be able to start it.
            canStartMasterClock = true;
        }

        private void onMasterStateChanged(ValueChangedEvent<MasterClockState> state)
        {
            switch (state.NewValue)
            {
                case MasterClockState.Synchronised:
                    if (canStartMasterClock)
                        masterClockContainer.Start();

                    break;

                case MasterClockState.TooFarAhead:
                    masterClockContainer.Stop();
                    break;
            }
        }
    }
}
