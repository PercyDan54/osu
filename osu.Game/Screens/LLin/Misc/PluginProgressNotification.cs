// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using osu.Game.Overlays.Notifications;

namespace osu.Game.Screens.LLin.Misc
{
    public class PluginProgressNotification : ProgressNotification
    {
        public Action OnComplete { get; set; }

        protected override void Completed()
        {
            OnComplete?.Invoke();
            base.Completed();
        }
    }
}
