using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets;
using osu.Game.Scoring;
using osu.Game.Scoring.Legacy;
using osu.Game.Screens.Edit.Setup;
using osuTK;

namespace osu.Game.Screens.ReplayVs
{
    public class ReplayVsSelectScreen : OsuScreen
    {
        private FileChooserLabelledTextBox fileChooser1;
        private FileChooserLabelledTextBox fileChooser2;
        private OsuSpriteText errorText;
        private DatabasedLegacyScoreDecoder decoder;

        [Resolved]
        private BeatmapManager beatmapManager { get; set; }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours, RulesetStore rulesetStore)
        {
            Container container;
            FillFlowContainer flowContainer;
            decoder = new DatabasedLegacyScoreDecoder(rulesetStore, beatmapManager);

            InternalChild = container = new PopoverContainer
            {
                Masking = true,
                CornerRadius = 10,
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(0.55f, 0.6f),
                Children = new Drawable[]
                {
                    new Box
                    {
                        Colour = colours.GreySeaFoamDark,
                        RelativeSizeAxes = Axes.Both,
                    },
                    flowContainer = new FillFlowContainer
                    {
                        Padding = new MarginPadding
                        {
                            Top = 7.5f,
                        },
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(1, 0.9f),
                        Spacing = new Vector2(15),
                        Direction = FillDirection.Vertical,
                    },
                }
            };

            flowContainer.Add(fileChooser1 = new FileChooserLabelledTextBox(".osr")
            {
                Label = "Red",
                FixedLabelWidth = 160,
                Width = 0.8f,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                PlaceholderText = "Click to select a replay",
                TabbableContentContainer = container
            });
            flowContainer.Add(fileChooser2 = new FileChooserLabelledTextBox(".osr")
            {
                Label = "Blue",
                Width = 0.8f,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FixedLabelWidth = 160,
                PlaceholderText = "Click to select a replay",
                TabbableContentContainer = container
            });
            flowContainer.Add(new TriangleButton
            {
                Text = "Start",
                Action = validateReplay,
                Size = new Vector2(0.4f, 0.12f),
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
            flowContainer.Add(errorText = new OsuSpriteText
            {
                Font = OsuFont.Default.With(size: 30),
                Alpha = 0,
                Colour = colours.Red,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
        }

        private void validateReplay()
        {
            string file1 = fileChooser1.Current.Value;
            string file2 = fileChooser2.Current.Value;

            if (file1 == string.Empty || file2 == string.Empty)
            {
                showError("Select two replays");
                return;
            }

            try
            {
                var score1 = parseReplay(file1);
                var score2 = parseReplay(file2);

                if (!score1.ScoreInfo.BeatmapInfo.Equals(score2.ScoreInfo.BeatmapInfo))
                {
                    showError("Two replays must have the same beatmap");
                    return;
                }

                var beatmap = beatmapManager.GetWorkingBeatmap(score1.ScoreInfo.BeatmapInfo);
                this.Push(new ReplayVsScreen(new[] { score1, score2 }, beatmap));
            }
            catch (LegacyScoreDecoder.BeatmapNotFoundException e)
            {
                showError(e.Message);
            }
        }

        private void showError(string error)
        {
            errorText.Text = error;
            errorText.FadeIn().Then().ScaleTo(1.05f, 100, Easing.Out).Then().ScaleTo(1f, 50f);
        }

        private Score parseReplay(string path)
        {
            using var stream = new FileStream(path, FileMode.Open);
            return decoder.Parse(stream);
        }
    }
}
