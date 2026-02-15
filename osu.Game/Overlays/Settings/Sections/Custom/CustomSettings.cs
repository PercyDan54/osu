// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Framework.Screens;
using osu.Game.Screens;
using osu.Game.Screens.BpGuess;
using osu.Game.Screens.MapGuess;

namespace osu.Game.Overlays.Settings.Sections.Custom
{
    public partial class CustomSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "Super Secret Settings";

        [Resolved]
        private IPerformFromScreenRunner runner { get; set; } = null!;

        public CustomSettings(CustomSettingsPanel panel)
        {
            Children = new Drawable[]
            {
                new SettingsButtonV2
                {
                    Text = "Open settings menu",
                    TooltipText = "Settings here is not provided by official",
                    Action = panel.ToggleVisibility
                },
                new SettingsButtonV2
                {
                    Text = "Open map guess",
                    Action = () => runner.PerformFromScreen(s => s.Push(new MapGuessConfigScreen()))
                },
                new SettingsButtonV2
                {
                    Text = "Open BP guess",
                    Action = () => runner.PerformFromScreen(s => s.Push(new BpGuessGameScreen()))
                },
            };
        }
    }
}
