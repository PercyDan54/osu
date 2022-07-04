// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Bindables;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace osu.Game.Screens.LLin.SideBar.Settings
{
    public interface ISettingsItem<T> : IHasTooltip
    {
        public LocalisableString Description { get; set; }
        public IconUsage Icon { get; set; }
        public Bindable<T> Bindable { get; }
    }
}
