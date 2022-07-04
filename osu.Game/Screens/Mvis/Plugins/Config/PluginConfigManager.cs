// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using osu.Framework.Platform;

namespace osu.Game.Screens.Mvis.Plugins.Config
{
    [Obsolete("原Mvis播放器现已移动至LLin(osu.Game.Screens.LLin)")]
    public abstract class PluginConfigManager<TLookup> : osu.Game.Screens.LLin.Plugins.Config.PluginConfigManager<TLookup>
        where TLookup : struct, Enum
    {
        protected PluginConfigManager(Storage storage)
            : base(storage)
        {
        }
    }
}
