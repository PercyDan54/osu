using System;
using Mvis.Plugin.CloudMusicSupport.Config;
using Mvis.Plugin.CloudMusicSupport.Misc;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Mvis;
using osuTK.Graphics;

namespace Mvis.Plugin.CloudMusicSupport.Sidebar
{
    public class LyricInfoPiece : CompositeDrawable, IHasTooltip, IHasContextMenu
    {
        public readonly Lyric Value;

        public Action<Lyric> Action;
        public LocalisableString TooltipText { get; private set; }

        private Box hoverBox;

        [Resolved]
        private LyricConfigManager config { get; set; }

        [Resolved]
        private MvisScreen mvisScreen { get; set; }

        public LyricInfoPiece(Lyric lrc)
        {
            CornerRadius = 5f;
            Masking = true;

            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Value = lrc;

            Colour = string.IsNullOrEmpty(lrc.Content)
                ? Color4Extensions.FromHex(@"555")
                : Color4.White;
        }

        [BackgroundDependencyLoader]
        private void load(CustomColourProvider colourProvider)
        {
            var timeSpan = TimeSpan.FromMilliseconds(Value.Time);

            Box bgBox;
            Box fgBox;
            InternalChildren = new Drawable[]
            {
                bgBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background4
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Masking = true,
                            CornerRadius = 5,
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Margin = new MarginPadding(5),
                            Children = new Drawable[]
                            {
                                fgBox = new Box
                                {
                                    Colour = colourProvider.Highlight1,
                                    RelativeSizeAxes = Axes.Both,
                                },
                                new OsuSpriteText
                                {
                                    Font = OsuFont.GetFont(size: 17, weight: FontWeight.Bold),
                                    Text = $"{timeSpan:mm\\:ss\\.fff}",
                                    Colour = Color4.Black,
                                    Margin = new MarginPadding { Horizontal = 5, Vertical = 3 }
                                },
                            }
                        },
                        new OsuSpriteText
                        {
                            Text = Value.Content,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            RelativeSizeAxes = Axes.X,
                            Margin = new MarginPadding { Left = 5, Bottom = 5 }
                        },
                        new OsuSpriteText
                        {
                            Text = Value.TranslatedString,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            RelativeSizeAxes = Axes.X,
                            Margin = new MarginPadding { Left = 5, Bottom = 5 }
                        },
                    }
                },
                hoverBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.White.Opacity(0.2f),
                    Alpha = 0
                },
                new HoverClickSounds()
            };

            TooltipText = $"Go to {timeSpan:mm\\:ss\\.fff}";

            colourProvider.HueColour.BindValueChanged(_ =>
            {
                bgBox.Colour = colourProvider.Background4;
                fgBox.Colour = colourProvider.Highlight1;
            }, true);
        }

        protected override bool OnClick(ClickEvent e)
        {
            Action?.Invoke(Value);
            return base.OnClick(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            hoverBox.FadeIn(300);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hoverBox.FadeOut(300);
            base.OnHoverLost(e);
        }

        public MenuItem[] ContextMenuItems => new MenuItem[] { new OsuMenuItem("Set as current offset", MenuItemType.Standard, () => config.SetValue(LyricSettings.LyricOffset, Value.Time - mvisScreen.CurrentTrack.CurrentTime)) };
    }
}
