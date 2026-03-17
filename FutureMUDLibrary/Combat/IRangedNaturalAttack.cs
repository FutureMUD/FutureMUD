namespace MudSharp.Combat;

public interface IRangedNaturalAttack : IWeaponAttack
{
	int RangeInRooms { get; }
	RangedScatterType ScatterType { get; }
}
