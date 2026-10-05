using MudSharp.Magic;

#nullable enable
namespace MudSharp.GameItems;

public enum MagicDeviceRole { Charged, Focus, Dual }
public enum MagicDeviceEligibility { Anyone, Caster, MagicType, AcquiredSpell }
public enum MagicDeviceKind { Wand, Staff }

/// <summary>Paid instance charges; a focus invocation always uses ordinary caster authority.</summary>
public interface IChargedMagicDevice : IGameItemComponent
{
	int Charges { get; }
	int Capacity { get; }
	long? SpellId { get; }
	int? Grade { get; }
	MagicDeviceRole Role { get; }
	string? DataError { get; }
}
