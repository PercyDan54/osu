// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Replays;
using osu.Game.Scoring;
using osu.Game.Screens.Play;

namespace osu.Game.Rulesets.Mods
{
    public abstract class ModAutoplay : Mod, IApplicableFailOverride, ICreateReplay, IApplicableToHUD, IApplicableToPlayer
    {
        public override string Name => "Autoplay";
        public override string Acronym => "AT";
        public override IconUsage? Icon => OsuIcon.ModAuto;
        public override ModType Type => ModType.Automation;
        public override string Description => "Watch a perfect automated play through the song.";
        public override double ScoreMultiplier => 1;

        [SettingSource("Save score")]
        public Bindable<bool> SaveScore { get; } = new BindableBool();

        [SettingSource("Hide replay interface")]
        public Bindable<bool> HideInterface { get; } = new BindableBool();

        public bool PerformFail() => false;

        public bool RestartOnFail => false;

        public override bool UserPlayable => false;

        public override Type[] IncompatibleMods => new[] { typeof(ModRelax), typeof(ModFailCondition), typeof(ModNoFail) };

        public override bool HasImplementation => GetType().GenericTypeArguments.Length == 0;

        public virtual Score CreateReplayScore(IBeatmap beatmap, IReadOnlyList<Mod> mods) => new Score { Replay = new Replay() };

        public virtual void ApplyToHUD(HUDOverlay overlay)
        {
            if (HideInterface.Value)
                overlay.PlayerSettingsOverlay.Children.ForEach(child => child.Hide());
        }

        public virtual void ApplyToPlayer(Player player) => ((ReplayPlayer)player).SaveScore = SaveScore.Value;
    }
}
