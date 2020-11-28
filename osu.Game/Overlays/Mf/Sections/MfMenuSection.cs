using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays.Profile;
using osuTK;
using static osu.Game.Overlays.Profile.ProfileSection;

namespace osu.Game.Overlays.Mf.Sections
{
    public abstract class MfMenuSection : Container
    {
        public abstract string Title { get; }
        public abstract string SectionId { get; }
        public Drawable ChildDrawable { get; set; }

        private FillFlowContainer contentFillFlow;
        private SectionTriangles bgTriangle;

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Masking = true;
            Anchor = Anchor.TopCentre;
            Origin = Anchor.TopCentre;
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background5,
                },
                bgTriangle = new SectionTriangles
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre
                },
                contentFillFlow = new FillFlowContainer
                {
                    Padding = new MarginPadding { Top = 20, Bottom = 20, Left = 50, Right = 50 },
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(20),
                    LayoutEasing = Easing.OutQuint,
                    LayoutDuration = 500,
                    Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Text = Title,
                            Font = OsuFont.GetFont(size: 30, weight: FontWeight.SemiBold),
                        }
                    }
                }
            };
        }

        protected override void UpdateAfterChildren()
        {
            bgTriangle.Height = DrawHeight * 0.3f;
            base.UpdateAfterChildren();
        }

        protected override void LoadComplete()
        {
            if (ChildDrawable != null)
                contentFillFlow.Add(ChildDrawable);
        }

        public override void Show() =>
            this.FadeIn(300, Easing.OutQuint);

        public override void Hide() =>
            this.FadeOut(300, Easing.OutQuint);
    }
}
