// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osuTK.Graphics;

namespace osu.Game.Screens.Select
{
    public partial class FooterButtonOpenInMvis : FooterButton
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            SelectedColour = new Color4(0, 86, 73, 255);
            DeselectedColour = SelectedColour.Opacity(0.5f);
            Text = @"Open in Mvis";
        }
    }
}
