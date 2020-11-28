// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Screens.Mvis.BottomBar
{
    public class BottomBarContainer : Container
    {
        public readonly Bindable<bool> Hovered = new Bindable<bool>();

        public BottomBarContainer()
        {
            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;
            RelativeSizeAxes = Axes.X;
            Height = 50;
        }

        protected override bool OnHover(Framework.Input.Events.HoverEvent e)
        {
            this.Hovered.Value = true;
            return base.OnHover(e);
        }

        protected override void OnHoverLost(Framework.Input.Events.HoverLostEvent e)
        {
            this.Hovered.Value = false;
            base.OnHoverLost(e);
        }
    }
}
