// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Game.Screens.LLin.Plugins.Config
{
    public abstract class PluginConfigManager<TLookup> : IniConfigManager<TLookup>, IPluginConfigManager
        where TLookup : struct, Enum
    {
        protected abstract string ConfigName { get; }
        protected override string Filename => $"custom/plugin-{ConfigName}.ini";

        protected PluginConfigManager(Storage storage)
            : base(storage)
        {
        }
    }
}
