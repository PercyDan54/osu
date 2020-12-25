// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections.Mf;

namespace osu.Game.Overlays
{
    public class MfSettingsPanel : SettingsSubPanel
    {
        protected override Drawable CreateHeader() => new SettingsHeader("Custom osu! settings", "some extra cusom settings");

        [BackgroundDependencyLoader(permitNulls: true)]
        private void load()
        {
            AddSection(new MfMainSection());
            AddSection(new MfMvisSection());
        }
    }
}
