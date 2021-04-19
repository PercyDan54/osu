// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osuTK;

namespace osu.Game.Screens.Edit.Timing
{
    public class RowAttribute : CompositeDrawable
    {
        private readonly ControlPoint point;

        protected FillFlowContainer Content { get; private set; }

        public RowAttribute(ControlPoint point)
        {
            this.point = point;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours, OverlayColourProvider overlayColours)
        {
            AutoSizeAxes = Axes.X;

            Height = 20;

            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;

            Masking = true;
            CornerRadius = 5;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    Colour = overlayColours.Background4,
                    RelativeSizeAxes = Axes.Both,
                },
                Content = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Direction = FillDirection.Horizontal,
                    Margin = new MarginPadding { Right = 5 },
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Y,
                            AutoSizeAxes = Axes.X,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    Colour = point.GetRepresentingColour(colours),
                                    RelativeSizeAxes = Axes.Both,
                                },
                                new OsuSpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Padding = new MarginPadding(3),
                                    Font = OsuFont.Default.With(weight: FontWeight.SemiBold, size: 12),
                                    Text = point.GetType().Name.Replace("ControlPoint", string.Empty).ToLowerInvariant(),
                                    Colour = colours.Gray0
                                },
                            },
                        },
                    },
                }
            };
        }
    }
}
