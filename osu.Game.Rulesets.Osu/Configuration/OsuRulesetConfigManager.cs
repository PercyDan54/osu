// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Osu.Configuration
{
    public class OsuRulesetConfigManager : RulesetConfigManager<OsuRulesetSetting>
    {
        public static OsuRulesetConfigManager Instance { get; private set; }
        public OsuRulesetConfigManager(SettingsStore settings, RulesetInfo ruleset, int? variant = null)
            : base(settings, ruleset, variant)
        {
            Instance = this;
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();
            Set(OsuRulesetSetting.SnakingInSliders, true);
            Set(OsuRulesetSetting.SnakingOutSliders, true);
            Set(OsuRulesetSetting.DanceMover, OsuDanceMover.Momentum);
            Set(OsuRulesetSetting.AngleOffset, 8f / 18f, 0f, 2f, float.Epsilon);
            Set(OsuRulesetSetting.JumpMulti, 2f / 3f, 0f, 2f, float.Epsilon);
            Set(OsuRulesetSetting.ReplayFramerate, 60f, 15f, 240f, 1f);
            Set(OsuRulesetSetting.SpinnerRadius, 235f, 10f, 350f, 1f);
            Set(OsuRulesetSetting.SpinnerRadius2, 15f, 10f, 250f, 1f);
            Set(OsuRulesetSetting.NextJumpMulti, 2f / 3f, 0f, 2f, float.Epsilon);
            Set(OsuRulesetSetting.SkipStackAngles, true);
            Set(OsuRulesetSetting.BorderBounce, true);
            Set(OsuRulesetSetting.ShowCursorTrail, true);
            Set(OsuRulesetSetting.PlayfieldBorderStyle, PlayfieldBorderStyle.None);
        }
    }

    public enum OsuRulesetSetting
    {
        SnakingInSliders,
        SnakingOutSliders,
        ShowCursorTrail,
        ReplayFramerate,
        SpinnerRadius,
        SpinnerRadius2,
        DanceMover,
        AngleOffset,
        JumpMulti,
        NextJumpMulti,
        BorderBounce,
        SkipStackAngles,
        PlayfieldBorderStyle
    }
    public enum OsuDanceMover
    {
        HalfCircle,
        Flower,
        Momentum
    }
}
