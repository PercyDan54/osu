// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using osu.Framework.Extensions;

namespace osu.Game.Screens.LLin.SideBar.Settings.Items
{
    public class SettingsEnumPiece<T> : SettingsListPiece<T>
        where T : struct, Enum
    {
        public SettingsEnumPiece()
        {
            var array = (T[])Enum.GetValues(typeof(T));
            Values = array.ToList();
        }

        protected override string GetValueText(T newValue) => newValue.GetDescription();
    }
}
