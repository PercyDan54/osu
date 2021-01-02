﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Osu.Replays;
using osu.Game.Scoring;
using osu.Game.Users;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModDance : OsuModAutoplay
    {
        public override Type[] IncompatibleMods => base.IncompatibleMods.Append(typeof(OsuModAutopilot)).Append(typeof(OsuModSpunOut)).ToArray();
        public override string Name => "Cursor dance";
        public override string Description => "Watch a perfect automated cursor dance through the song.";

        public override Score CreateReplayScore(IBeatmap beatmap) => new Score
        {
            ScoreInfo = new ScoreInfo { User = new User { Username = "danser" } },
            Replay = new OsuDanceGenerator(beatmap).Generate()
        };
    }
}
