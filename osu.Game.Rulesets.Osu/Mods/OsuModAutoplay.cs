// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Replays;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModAutoplay : ModAutoplay
    {
        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[] { typeof(OsuModAimAssist), typeof(OsuModAutopilot), typeof(OsuModSpunOut) }).ToArray();

        [SettingSource("Cursor Dance", "Enable cursor dance")]
        public Bindable<bool> CursorDance { get; } = new BindableBool();

        public override Score CreateReplayScore(IBeatmap beatmap, IReadOnlyList<Mod> mods)
        {
            return new Score
            {
                ScoreInfo = new ScoreInfo
                {
                    User = new APIUser { Username = CursorDance.Value ? "danser" : "Autoplay" },
                    BeatmapInfo = beatmap.BeatmapInfo,
                    OnlineID = beatmap.BeatmapInfo.OnlineID,
                    Date = DateTime.UtcNow,
                    Mods = mods.ToArray()
                },
                Replay = CursorDance.Value ? new OsuDanceGenerator(beatmap, mods).Generate() : new OsuAutoGenerator(beatmap, mods).Generate()
            };
        }
    }
}
