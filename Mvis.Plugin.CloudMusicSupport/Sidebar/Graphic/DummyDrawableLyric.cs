using System;
using Mvis.Plugin.CloudMusicSupport.Misc;

namespace Mvis.Plugin.CloudMusicSupport.Sidebar.Graphic
{
    public class DummyDrawableLyric : DrawableLyric
    {
        public override int FinalHeight()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateValue(Lyric lyric)
        {
            throw new NotImplementedException();
        }
    }
}
