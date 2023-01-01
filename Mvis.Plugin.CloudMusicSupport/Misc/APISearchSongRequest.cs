// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Online.API;

namespace Mvis.Plugin.CloudMusicSupport.Misc
{
    public class APISearchSongRequest : OsuJsonWebRequest<APISearchResponseRoot>
    {
        public string Keyword { get; }

        public APISearchSongRequest(string keyword)
        {
            Url = $"https://music.163.com/api/search/get/web?hlpretag=&hlposttag=&s={keyword}&type=1&total=true&limit=1";
            Keyword = keyword;
        }
    }
}
