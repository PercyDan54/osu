// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace osu.Game.Overlays.Settings.Sections
{
    public partial class MfSection : SettingsSection
    {
        public override LocalisableString Header => "Custom";

        public override Drawable CreateIcon() => new SpriteIcon
        {
            Icon = FontAwesome.Solid.Bars
        };

        public MfSection(MfSettingsPanel mfpanel)
        {
            Children = new Drawable[]
            {
                new MfSettingsEntranceButton(mfpanel),
            };
        }
    }
}
