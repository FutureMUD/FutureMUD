using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp_Unit_Tests;

public class FrameworkItemStub : IFrameworkItem
{
	public string Name { get; set; } = string.Empty;
	public long Id { get; set; }
	public string FrameworkItemType => "Stub";
}

public class RevisableItemStub : FrameworkItemStub, IRevisableItem
{
	public int RevisionNumber { get; set; }
	public RevisionStatus Status { get; set; }
	public string IdAndRevisionFor(IPerceiver voyeur) => $"{Id}:{RevisionNumber}";
}
