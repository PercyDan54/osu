// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using osu.Game.Overlays.Dialog;

namespace osu.Game.Screens.LLin.SideBar.PluginsPage
{
    public class PluginRemoveConfirmDialog : PopupDialog
    {
        public PluginRemoveConfirmDialog(string headerText, Action<bool> onConfirm)
        {
            HeaderText = headerText;
            BodyText = "卸载后该插件在本次osu!会话中将不再可用!";

            Buttons = new PopupDialogButton[]
            {
                new PopupDialogOkButton
                {
                    Text = "是",
                    Action = () => onConfirm(false)
                },
                new PopupDialogOkButton
                {
                    Text = "是, 并且未来不再加载该插件到播放器中",
                    Action = () => onConfirm(true)
                },
                new PopupDialogCancelButton
                {
                    Text = "否"
                }
            };
        }
    }
}
