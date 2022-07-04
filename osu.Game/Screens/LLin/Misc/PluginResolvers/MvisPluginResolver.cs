// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using osu.Game.Screens.LLin.Plugins;

namespace osu.Game.Screens.LLin.Misc.PluginResolvers
{
    [Obsolete("Mvis => LLin")]
    public class MvisPluginResolver : LLinPluginResolver
    {
        public MvisPluginResolver(LLinPluginManager pluginManager)
            : base(pluginManager)
        {
        }
    }
}
