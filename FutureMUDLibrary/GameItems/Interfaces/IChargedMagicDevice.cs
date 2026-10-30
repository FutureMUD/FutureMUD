using MudSharp.Magic;
using System.Collections.Generic;

#nullable enable
namespace MudSharp.GameItems;

public enum MagicDeviceRole { Charged, Focus, Dual }
public enum MagicDeviceEligibility { Anyone, Caster, MagicType, AcquiredSpell }
public enum MagicDeviceKind { Wand, Staff }
public enum MagicDeviceMode { StoredCharges, EnergyReservoir }
public enum MagicDeviceDelivery { OtherCharacters, RoomCorpses, NativeArea }

/// <summary>Paid instance charges; a focus invocation always uses ordinary caster authority.</summary>
public interface IChargedMagicDevice : IGameItemComponent
{
	int Charges { get; }
	int Capacity { get; }
	long? SpellId { get; }
	int? Grade { get; }
	MagicDeviceRole Role { get; }
	MagicDeviceMode Mode { get; }
	int Energy { get; }
	int MaximumEnergy { get; }
	long? ResourceId { get; }
	long? ReservoirSpellId { get; }
	MagicDeviceDelivery Delivery { get; }
	IReadOnlyDictionary<int, int> GradeCosts { get; }
	bool AllowsWornUse { get; }
	bool RequiresOpenSky { get; }
	string? DataError { get; }
}
