﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;

namespace osu.Game.Overlays.Settings.Sections.Input
{
    public class MfSettingsEnteranceButton : SettingsSubsection
    {
        protected override string Header => "Custom osu settings";

        public MfSettingsEnteranceButton(MfSettingsPanel mfpanel)
        {
            Children = new Drawable[]
            {
                new SettingsButton
                {
                    Text = "Open settings menu",
                    TooltipText = "Settings here is not provided by official",
                    Action = mfpanel.ToggleVisibility
                },
            };
        }
    }
}
