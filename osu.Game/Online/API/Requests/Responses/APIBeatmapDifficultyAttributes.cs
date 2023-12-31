// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Newtonsoft.Json;

namespace osu.Game.Online.API.Requests.Responses
{
    public class APIBeatmapDifficultyAttributesResponse
    {
        [JsonProperty("attributes")]
        public APIBeatmapDifficultyAttributes Attributes { get; set; } = null!;
    }

    public class APIBeatmapDifficultyAttributes
    {
        [JsonProperty("max_combo")]
        public int MaxCombo { get; set; }

        [JsonProperty("star_rating")]
        public double StarRating { get; set; }
    }
}
