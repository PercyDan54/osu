// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Configuration;

namespace osu.Game.Overlays.Settings.Sections.Mf
{
    public class MvisVisualSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "Effects";

        [BackgroundDependencyLoader]
        private void load(MConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Show triangles",
                    Current = config.GetBindable<bool>(MSetting.MvisEnableBgTriangles),
                },
            };
        }
    }
}
