// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using osuTK.Input;

namespace osu.Game.Screens.LLin.Plugins
{
    public class PluginKeybind
    {
        /// <summary>
        /// 目标按键
        /// </summary>
        public readonly Key Key;

        /// <summary>
        /// 触发后要执行的动作
        /// </summary>
        public readonly Action Action;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        internal int Id;

        public PluginKeybind(Key key, Action action, string name = "???")
        {
            Key = key;
            Action = action;
            Name = name;
        }

        public override string ToString() => "按键 " + Key + $" 上的键位绑定(Id: {Id}, {Action})";
    }
}
