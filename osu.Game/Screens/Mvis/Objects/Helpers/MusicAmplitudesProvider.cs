﻿using osu.Framework.Bindables;

namespace osu.Game.Screens.Mvis.Objects.Helpers
{
    public abstract class MusicAmplitudesProvider : CurrentBeatmapProvider
    {
        public readonly BindableBool IsKiai = new BindableBool();

        protected override void Update()
        {
            base.Update();

            var track = Beatmap.Value?.Track;
            OnAmplitudesUpdate(track?.CurrentAmplitudes.FrequencyAmplitudes.Span.ToArray() ?? new float[256]);
            IsKiai.Value = Beatmap.Value?.Beatmap.ControlPointInfo.EffectPointAt(track?.CurrentTime ?? 0).KiaiMode ?? false;
        }

        protected abstract void OnAmplitudesUpdate(float[] amplitudes);
    }
}
