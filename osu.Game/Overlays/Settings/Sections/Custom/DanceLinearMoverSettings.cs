// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics.UserInterfaceV2;

namespace osu.Game.Overlays.Settings.Sections.Custom
{
    public partial class DanceLinearMoverSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "Linear Mover Settings";

        [BackgroundDependencyLoader]
        private void load(CustomConfigManager config)
        {
            Children =
            [
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = "Wait for preempt",
                    Current = config.GetBindable<bool>(CustomSetting.WaitForPreempt)
                }),
            ];
        }
    }
}
