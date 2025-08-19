// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Scoring;
using osu.Game.Screens.Backgrounds;
using osu.Game.Screens.Play;
using osu.Game.Users;

namespace osu.Game.Screens.MapGuess
{
    public partial class MapGuessPlayer : ReplayPlayer
    {
        private readonly MapGuessConfig config;
        private readonly double startTime;
        private double playLength;

        public BindableBool Paused { get; } = new BindableBool(true);

        protected override UserActivity InitialActivity => null!;

        public MapGuessPlayer(Score score, double startTime, MapGuessConfig config)
            : base(score, new PlayerConfiguration
            {
                AllowUserInteraction = false,
                ShowResults = false
            })
        {
            this.startTime = startTime;
            this.config = config;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            HUDOverlay.ShowHud.Value = false;
            HUDOverlay.ShowHud.Disabled = true;
            HUDOverlay.PlayfieldSkinLayer.Hide();
            BreakOverlay.Hide();
            DrawableRuleset.Overlays.Hide();
            DrawableRuleset.Playfield.DisplayJudgements.Value = false;
            playLength = config.PreviewLength.Value;

            if (!config.Music.Value)
                Beatmap.Value.Track.Volume.Value = 0;

            if (!config.ShowHitobjects.Value)
                DrawableRuleset.Hide();

            Reset(false);
            Schedule(() =>
            {
                bool showBackground = config.ShowBackground.Value;
                ApplyToBackground(b =>
                {
                    b.IgnoreUserSettings.Value = true;
                    b.StoryboardReplacesBackground.Value = false;
                    b.DimWhenUserSettingsIgnored.Value = showBackground ? (config.ShowHitobjects.Value ? 0.7f : 0) : 1;
                });
                ToggleBackground(showBackground);
                SetBackgroundBlur(config.BackgroundBlur.Value);
            });
        }

        protected override void Update()
        {
            base.Update();

            if (GameplayClockContainer.CurrentTime >= startTime + playLength && !Paused.Value)
            {
                GameplayClockContainer.Stop();
                Paused.Value = true;
                this.FadeOut(500, Easing.OutQuart);
            }
        }

        protected override void PerformFail()
        {
            // base logic intentionally suppressed - failing in multiplayer only marks the score with F rank
            ScoreProcessor.FailScore(Score.ScoreInfo);
        }

        public void Reset(bool answer)
        {
            Paused.Value = false;
            GameplayClockContainer.Stop();
            SetGameplayStartTime(startTime);

            if (answer)
            {
                playLength = config.ShowAnswerLength.Value;
                Beatmap.Value.Track.Volume.Value = 1;
                ApplyToBackground(b => b.DimWhenUserSettingsIgnored.Value = 0.7f);
                ToggleBackground(true);
                SetBackgroundBlur(0);
                DrawableRuleset.Show();
            }

            GameplayClockContainer.Start();
            Paused.Value = false;
            this.FadeIn(200, Easing.In);
        }

        public void ToggleBackground(bool show)
        {
            ApplyToBackground(b =>
            {
                if (show)
                    b.Show();
                else
                    b.Hide();
            });
        }

        public void SetBackgroundBlur(float blur)
        {
            ApplyToBackground(b =>
            {
                b.BlurAmount.Value = blur * BackgroundScreenBeatmap.USER_BLUR_FACTOR;
            });
        }
    }
}
