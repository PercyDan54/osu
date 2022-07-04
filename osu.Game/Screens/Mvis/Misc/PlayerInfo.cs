// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;

namespace osu.Game.Screens.Mvis.Misc
{
    [Obsolete("原Mvis播放器现已移动至LLin(osu.Game.Screens.LLin)")]
    public class PlayerInfo : osu.Game.Screens.LLin.Misc.PlayerInfo
    {
    }

    [Flags]
    [Obsolete("原Mvis播放器现已移动至LLin(osu.Game.Screens.LLin)")]
    public enum PlayerFlags
    {
        None = 1, //bug: None = 0 时 HasFlagFast(PlayerFlags.None)始终为True
        OverlayProxy = 1 << 1,
        SidebarSupport = 1 << 2,

        All = OverlayProxy | SidebarSupport
    }
}
