// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.IO.Network;
using osu.Game.Beatmaps;
using osu.Game.Configuration;

namespace osu.Game.Online.API.Requests
{
    public class DownloadBeatmapSetRequest : ArchiveDownloadRequest<IBeatmapSetInfo>
    {
        private readonly bool noVideo;
        private readonly bool useSayobot;

        public DownloadBeatmapSetRequest(IBeatmapSetInfo set, bool noVideo)
            : base(set)
        {
            this.noVideo = noVideo;
            useSayobot = MConfigManager.Instance.Get<bool>(MSetting.UseSayobot);
        }

        protected override WebRequest CreateWebRequest()
        {
            var req = base.CreateWebRequest();
            req.Timeout = 60000;
            return req;
        }

        protected override string FileExtension => ".osz";

        protected override string Uri
        {
            get
            {
                if (useSayobot)
                    return $@"https://txy1.sayobot.cn/beatmaps/download/{Target}";

                return $@"{API.APIEndpointUrl}/api/v2/{Target}";
            }
        }

        protected override string Target
        {
            get
            {
                if (useSayobot)
                {
                    string idFull = Model.OnlineID.ToString();

                    string target = $@"{(noVideo ? "novideo" : "full")}/{idFull}";
                    return target;
                }

                return $@"beatmapsets/{Model.OnlineID}/download{(noVideo ? "?noVideo=1" : "")}";
            }
        }
    }
}
