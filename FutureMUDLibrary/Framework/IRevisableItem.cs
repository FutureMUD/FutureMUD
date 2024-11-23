using MudSharp.Framework.Revision;

namespace MudSharp.Framework;

public interface IRevisableItem : IFrameworkItem {
	int RevisionNumber { get; }
	RevisionStatus Status { get; }
	string IdAndRevisionFor(IPerceiver voyeur);
}