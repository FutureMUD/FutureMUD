using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Communication {

	public enum ChannelSpeakerNameMode
	{
		AccountName = 0,
		CharacterName,
		CharacterFullName,
		AnonymousToPlayers
	}

	public interface IChannel : IFrameworkItem, IHaveFuturemud, IEditableItem {
        IEnumerable<string> CommandWords { get; }
        void Send(ICharacter source, string message);
        bool Ignore(ICharacter character);
        bool Acknowledge(ICharacter character);

        bool AnnounceChannelJoiners { get; }
        bool AnnounceMissedListeners { get; }
        bool AddToPlayerCommandTree { get; }
        bool AddToGuideCommandTree { get; }
        ChannelSpeakerNameMode Mode { get; }
        IFutureProg ChannelListenerProg { get; }
        IFutureProg ChannelSpeakerProg { get; }
        string ChannelColour { get; }
    }
}