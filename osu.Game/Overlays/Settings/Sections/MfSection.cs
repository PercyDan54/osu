﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Overlays.Settings.Sections.Input;

namespace osu.Game.Overlays.Settings.Sections
{
    public class MfSection : SettingsSection
    {
        public override string Header => "Custom";
        public override Drawable CreateIcon() => new SpriteIcon
        {
            Icon = FontAwesome.Solid.Bars
        };

        public MfSection(MfSettingsPanel mfpanel)
        {
            Children = new Drawable[]
            {
                new MfSettingsEnteranceButton(mfpanel),
            };
        }
    }
}
