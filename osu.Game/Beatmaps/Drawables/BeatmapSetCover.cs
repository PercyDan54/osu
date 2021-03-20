﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Configuration;

namespace osu.Game.Beatmaps.Drawables
{
    [LongRunningLoad]
    public class BeatmapSetCover : Sprite
    {
        private readonly BeatmapSetInfo set;
        private readonly BeatmapSetCoverType type;

        public BeatmapSetCover(BeatmapSetInfo set, BeatmapSetCoverType type = BeatmapSetCoverType.Cover)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            this.set = set;
            this.type = type;
        }

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures, MConfigManager mfconfig)
        {
            string resource = null;

            switch (mfconfig.Get<bool>(MSetting.UseSayobot))
            {
                case true:
                    switch (type)
                    {
                        case BeatmapSetCoverType.Cover:
                            resource = $"https://a.sayobot.cn/beatmaps/{set?.OnlineBeatmapSetID}/covers/cover.jpg";
                            break;

                        case BeatmapSetCoverType.Card:
                            resource = $"https://a.sayobot.cn/beatmaps/{set?.OnlineBeatmapSetID}/covers/cover.jpg";
                            break;

                        case BeatmapSetCoverType.List:
                            resource = $"https://a.sayobot.cn/beatmaps/{set?.OnlineBeatmapSetID}/covers/cover.jpg";
                            break;
                    }
                    break;

                case false:
                default:
                    switch (type)
                    {
                        case BeatmapSetCoverType.Cover:
                            resource = set.OnlineInfo.Covers.Cover;
                            break;

                        case BeatmapSetCoverType.Card:
                            resource = set.OnlineInfo.Covers.Card;
                            break;

                        case BeatmapSetCoverType.List:
                            resource = set.OnlineInfo.Covers.List;
                            break;
                    }
                    break;
            }

            if (resource != null)
                Texture = textures.Get(resource);
        }
    }

    public enum BeatmapSetCoverType
    {
        Cover,
        Card,
        List,
    }
}
