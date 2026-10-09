#nullable enable

using MudSharp.Framework;

namespace MudSharp.Character;

public sealed class PhysicalReferenceTargets
{
	private readonly HashSet<(PhysicalEntityKind Kind, long Id)> _targets;

	public PhysicalReferenceTargets(IEnumerable<PhysicalEntityReference> targets)
	{
		_targets = targets.Where(x => x.Id > 0).Select(x => (x.Kind, x.Id)).ToHashSet();
	}

	public bool Includes(PhysicalEntityReference reference) => _targets.Contains((reference.Kind, reference.Id));
	public bool HasKind(PhysicalEntityKind kind) => _targets.Any(x => x.Kind == kind);
	public long[] Ids(PhysicalEntityKind kind) => _targets.Where(x => x.Kind == kind).Select(x => x.Id).ToArray();
}
