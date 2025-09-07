// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Overlays.Profile;
using osu.Game.Overlays.Profile.Sections.Ranks;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets;
using osu.Game.Screens.MapGuess;
using osu.Game.Users;

namespace osu.Game.Screens.BpGuess
{
    public partial class BpGuessGameScreen : OsuScreen
    {
        public override bool HideOverlaysOnEnter => true;

        protected override BackgroundScreen CreateBackground() => new MapGuessConfigScreen.SolidBackgroundScreen();

        [Cached]
        private OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        [Resolved]
        private RulesetStore rulesets { get; set; } = null!;

        private RulesetInfo ruleset;
        private readonly Random random = new Random();
        private readonly Bindable<UserProfileData?> user = new Bindable<UserProfileData?>();

        private OsuSpriteText text;
        private GridContainer buttonsContainer;
        private List<RoundedButton> buttons = [];
        private List<UserStatistics> users = [];
        private int index;
        private decimal answerValue;
        private int answerIndex;

        [BackgroundDependencyLoader]
        private void load(RulesetStore rulesets)
        {
            ruleset = rulesets.GetRuleset("osu");
            Padding = new MarginPadding
            {
                Horizontal = 35,
                Vertical = 40,
            };
            InternalChildren =
            [
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.9f,
                    RowDimensions =
                    [
                        new Dimension(GridSizeMode.Relative, 0.8f),
                        new Dimension(GridSizeMode.AutoSize),
                        new Dimension(GridSizeMode.AutoSize),
                    ],
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new OsuScrollContainer<ExtendedScoreContainer>
                            {
                                Masking = true,
                                RelativeSizeAxes = Axes.Both,
                                Child = new ExtendedScoreContainer(ScoreType.Best, user, UsersStrings.ShowExtraTopRanksBestTitle)
                            }
                        },
                        new Drawable[]
                        {
                            text = new OsuSpriteText
                            {
                                AlwaysPresent = true,
                                Font = FontUsage.Default.With(size: 40),
                                Height = 100,
                            }
                        },
                        new Drawable[]
                        {
                            buttonsContainer = new GridContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 50f
                            },
                        }
                    }
                }
            ];
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            getUsers();
        }

        private void getUsers()
        {
            //int n = random.Next(users.Count);

            if (true)
            {
                var req = new GetUserRankingsRequest(ruleset, page: random.Next(200));
                api.PerformAsync(req);
                req.Success += content =>
                {
                    users = content.Users;
                    index = random.Next(users.Count);
                    updateUser();
                };
                return;
            }

            //index = n;
            updateUser();
        }

        private void updateUser()
        {
            var apiUser = users[index].User;
            user.Value = new UserProfileData(apiUser, ruleset);
            var req = new GetUserScoresRequest(apiUser.Id, ScoreType.Best, new PaginationParameters(100), ruleset);
            api.PerformAsync(req);
            req.Success += updateDisplay;
        }

        private void updateDisplay(List<SoloScoreInfo> scores)
        {
            text.FadeOut(500, Easing.Out);
            answerValue = Math.Round(users[index].PP.GetValueOrDefault());

            int min = (int)(scores.Count * 0.15);
            double minPp = scores[min].PP.GetValueOrDefault();
            //var range = GetWeightedRange(minPp, scores[0].PP.GetValueOrDefault(), scores.Count);
            double range = RandomDouble(0.05, 0.1);

            answerIndex = random.Next(4);
            buttons.Clear();

            for (int i = 0; i < 4; i++)
            {
                int i1 = i;
                buttons.Add(new RoundedButton
                {
                    RelativeSizeAxes = Axes.X,
                    Text = (i == answerIndex
                        ? answerValue
                        : (int)Math.Round((double)answerValue * (1 + RandomDouble(-range, range)))).ToString(),
                    Action = () => answer(i1)
                });
            }

            buttonsContainer.Content = new Drawable[][] { buttons.Cast<Drawable>().ToArray() };
        }

        private void answer(int index)
        {
            foreach (var button in buttons)
            {
                button.Enabled.Value = false;
            }

            text.Text = $"The user's PP is {answerValue}";
            text.FadeIn(500, Easing.Out);

            if (answerIndex == index)
            {
                text.FadeColour(Colour4.Green, 500, Easing.Out);
            }
            else
            {
                text.FadeColour(Colour4.Red, 500, Easing.Out);
            }

            this.Delay(3000).Then().Schedule(getUsers);
        }

        public double RandomDouble(double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        private static (double MinSum, double MaxSum) GetWeightedRange(double min, double max, int count)
        {
            double minSum = 0;
            double maxSum = 0;

            for (int i = 0; i < count; i++)
            {
                minSum += min * Math.Pow(0.95, i);
                maxSum += max * Math.Pow(0.95, i);
            }

            return (minSum, maxSum);
        }
    }

    internal partial class ExtendedScoreContainer : PaginatedScoreContainer
    {
        protected override int InitialItemsCount => 10;

        public ExtendedScoreContainer(ScoreType type, Bindable<UserProfileData?> user, LocalisableString headerText)
            : base(type, user, headerText)
        {
        }
    }
}
