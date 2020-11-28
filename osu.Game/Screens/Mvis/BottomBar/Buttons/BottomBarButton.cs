// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Screens.Mvis.Modules;
using osu.Game.Configuration;
using osu.Framework.Graphics.Effects;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Input.Events;
using osu.Game.Skinning;

namespace osu.Game.Screens.Mvis.BottomBar.Buttons
{
    public class BottomBarButton : OsuClickableContainer
    {
        [Resolved]
        private CustomColourProvider colourProvider { get; set; }

        protected CustomColourProvider ColourProvider => colourProvider;
        protected FillFlowContainer ContentFillFlow;
        protected virtual string BackgroundTextureName => "MButtonSquare-background";

        protected virtual Drawable CreateBackgroundTexture => new SkinnableSprite(BackgroundTextureName, confineMode: ConfineMode.ScaleToFit)
        {
            RelativeSizeAxes = Axes.Both,
            CentreComponent = false
        };

        protected Box BgBox;
        private Box flashBox;
        private Container content;

        public IconUsage ButtonIcon
        {
            get => SpriteIcon.Icon;
            set => SpriteIcon.Icon = value;
        }

        public string Text
        {
            get => spriteText.Text;
            set => spriteText.Text = value;
        }

        private readonly OsuSpriteText spriteText = new OsuSpriteText
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
        };

        protected SpriteIcon SpriteIcon = new SpriteIcon
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(13),
        };

        public bool NoIcon;

        public BottomBarButton()
        {
            Size = new Vector2(30, 30);
        }

        [BackgroundDependencyLoader]
        private void load(MfConfigManager config)
        {
            Children = new Drawable[]
            {
                content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    CornerRadius = 5,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Radius = 1.5f,
                        Colour = Color4.Black.Opacity(0.6f),
                        Offset = new Vector2(0, 1.5f)
                    },
                    Children = new[]
                    {
                        BgBox = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = ColourProvider.Background3
                        },
                        CreateBackgroundTexture,
                        ContentFillFlow = new FillFlowContainer
                        {
                            Margin = new MarginPadding { Left = 15, Right = 15 },
                            AutoSizeAxes = Axes.X,
                            RelativeSizeAxes = Axes.Y,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Spacing = new Vector2(5)
                        },
                        flashBox = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Color4.White,
                            Alpha = 0,
                        }
                    }
                }
            };

            if (!NoIcon)
                ContentFillFlow.Add(SpriteIcon);

            if (Text != null)
                ContentFillFlow.Add(spriteText);

            // From OsuAnimatedButton
            if (AutoSizeAxes != Axes.None)
            {
                content.RelativeSizeAxes = (Axes.Both & ~AutoSizeAxes);
                content.AutoSizeAxes = AutoSizeAxes;
            }

            colourProvider.HueColour.BindValueChanged(_ =>
            {
                BgBox.Colour = ColourProvider.Background3;
            });
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            content.ScaleTo(0.9f, 2000, Easing.OutQuint);
            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            content.ScaleTo(1f, 1000, Easing.OutElastic);
            flashBox.FadeOut(1000, Easing.OutQuint);
            base.OnMouseUp(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            OnClickAnimation();
            return base.OnClick(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            flashBox.FadeTo(0.2f, 300);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);
            flashBox.FadeTo(0f, 300);
        }

        protected virtual void OnClickAnimation()
        {
            flashBox.FadeTo(0.3f).Then().FadeTo(IsHovered ? 0.2f : 0f, 300);
        }
    }
}
