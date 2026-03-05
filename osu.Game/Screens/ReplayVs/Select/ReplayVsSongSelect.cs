// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Scoring;
using osu.Game.Screens.Select;

namespace osu.Game.Screens.ReplayVs.Select
{
    public partial class ReplayVsSongSelect : SongSelect, ISongSelect
    {
        private readonly ReplayVsSelectScreen.TeamContainer teamContainer;

        public ReplayVsSongSelect(ReplayVsSelectScreen.TeamContainer teamContainer)
        {
            this.teamContainer = teamContainer;
        }

        void ISongSelect.PresentScore(ScoreInfo score)
        {
            if (score.Files.Count == 0)
                return;

            teamContainer.AddScore(score);
        }

        protected override void OnStart()
        {
        }
    }
}
