using MudSharp.Combat;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

public interface IMusketCartridge : IAmmo
{
	double BulletBore { get; }
	IGameItemProto BulletProto { get; }
	double? PowderMass { get; }
	bool IncludesWad { get; }
}
