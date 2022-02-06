// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Scoring;
using osu.Game.Screens.OnlinePlay.Multiplayer.Spectate;
using osu.Game.Screens.Play;

namespace osu.Game.Screens.ReplayVs
{
    public class ReplayVsPlayer : Player
    {
        private readonly Bindable<bool> waitingOnFrames = new Bindable<bool>();
        private readonly ISpectatorPlayerClock spectatorPlayerClock;
        private readonly Score score;
        private readonly ColourInfo teamColor;

        protected override bool CheckModsAllowFailure() => false;

        public ReplayVsPlayer([NotNull] Score score, [NotNull] ISpectatorPlayerClock spectatorPlayerClock, ColourInfo teamColor)
            : base(new PlayerConfiguration { AllowUserInteraction = false })
        {
            this.spectatorPlayerClock = spectatorPlayerClock;
            this.score = score;
            this.teamColor = teamColor;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            spectatorPlayerClock.WaitingOnFrames.BindTo(waitingOnFrames);

            HUDOverlay.PlayerSettingsOverlay.Expire();
            HUDOverlay.HoldToQuit.Expire();

            AddInternal(new OsuSpriteText
            {
                Text = score.ScoreInfo.User.Username,
                Font = OsuFont.Default.With(size: 50),
                Colour = teamColor,
                Y = 100,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
            });
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            waitingOnFrames.Value = DrawableRuleset.FrameStableClock.WaitingOnFrames.Value;
        }

        protected override void PrepareReplay()
        {
            DrawableRuleset?.SetReplayScore(score);
        }

        protected override Score CreateScore(IBeatmap beatmap) => score;

        protected override GameplayClockContainer CreateGameplayClockContainer(WorkingBeatmap beatmap, double gameplayStart)
            => new MultiSpectatorPlayer.SpectatorGameplayClockContainer(spectatorPlayerClock);
    }
}
