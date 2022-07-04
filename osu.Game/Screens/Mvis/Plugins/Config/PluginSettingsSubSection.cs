// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using osu.Game.Screens.LLin.Plugins;

namespace osu.Game.Screens.Mvis.Plugins.Config
{
    [Obsolete("原Mvis播放器现已移动至LLin(osu.Game.Screens.LLin)")]
    public abstract class PluginSettingsSubSection : osu.Game.Screens.LLin.Plugins.Config.PluginSettingsSubSection
    {
        protected PluginSettingsSubSection(LLinPlugin plugin)
            : base(plugin)
        {
        }
    }
}
