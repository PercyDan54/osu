// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using IToggleableFunctionProvider = osu.Game.Screens.LLin.Plugins.Types.IToggleableFunctionProvider;

namespace osu.Game.Screens.Mvis.Plugins.Internal.FallbackFunctionBar
{
    [Obsolete("原Mvis播放器现已移动至LLin(osu.Game.Screens.LLin)")]
    public class ToggleableBarButton : osu.Game.Screens.LLin.Plugins.Internal.FallbackFunctionBar.ToggleableBarButton
    {
        public ToggleableBarButton(IToggleableFunctionProvider provider)
            : base(provider)
        {
        }
    }
}
