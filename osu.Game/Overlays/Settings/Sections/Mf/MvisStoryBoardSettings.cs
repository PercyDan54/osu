// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Configuration;

namespace osu.Game.Overlays.Settings.Sections.General
{
    public class MvisStoryBoardSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "Storyboard";

        [BackgroundDependencyLoader]
        private void load(MConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Enable storyboard overlay",
                    Current = config.GetBindable<bool>(MSetting.MvisStoryboardProxy)
                }
            };
        }
    }
}
