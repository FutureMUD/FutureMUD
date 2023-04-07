using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Channel
    {
        public Channel()
        {
            ChannelCommandWords = new HashSet<ChannelCommandWord>();
            ChannelIgnorers = new HashSet<ChannelIgnorer>();
        }

        public long Id { get; set; }
        public string ChannelName { get; set; }
        public long ChannelListenerProgId { get; set; }
        public long ChannelSpeakerProgId { get; set; }
        public bool AnnounceChannelJoiners { get; set; }
        public string ChannelColour { get; set; }
        public int Mode { get; set; }
        public bool AnnounceMissedListeners { get; set; }
        public bool AddToPlayerCommandTree { get; set; }
        public bool AddToGuideCommandTree { get; set; }

        public virtual FutureProg ChannelListenerProg { get; set; }
        public virtual FutureProg ChannelSpeakerProg { get; set; }
        public virtual ICollection<ChannelCommandWord> ChannelCommandWords { get; set; }
        public virtual ICollection<ChannelIgnorer> ChannelIgnorers { get; set; }
    }
}
