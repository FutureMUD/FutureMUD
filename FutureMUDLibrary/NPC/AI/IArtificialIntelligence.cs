using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.NPC.AI {
	public interface IArtificialIntelligence : IFrameworkItem, IHandleEvents, IHaveFuturemud, IEditableItem
	{
		string AIType { get; }
		bool CountsAsAggressive { get; }
		string RawXmlDefinition { get; }
	}
}