// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using osuTK.Input;

namespace osu.Game.Screens.Mvis.Plugins
{
    [Obsolete("原Mvis播放器现已移动至LLin(osu.Game.Screens.LLin)")]
    public class PluginKeybind : osu.Game.Screens.LLin.Plugins.PluginKeybind
    {
        public PluginKeybind(Key key, Action action, string name = "???")
            : base(key, action, name)
        {
        }
    }
}
