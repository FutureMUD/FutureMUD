using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

public enum FirearmAttachmentSlotType
{
	Optic,
	Stock,
	Grip,
	Muzzle,
	Barrel,
	Underbarrel,
	Side,
	Bayonet,
	Other
}

public enum FirearmFireModeType
{
	Single,
	Burst,
	Automatic
}

public enum FirearmCycleType
{
	Manual,
	SelfLoading
}

public sealed record FirearmAttachmentSlot(string Name, FirearmAttachmentSlotType Type, string FormFactor);

public sealed record FirearmFireMode(
	FirearmFireModeType Type,
	int RoundsPerTrigger,
	double RecoilPenalty,
	double ExtraStaminaPerRound,
	double ExtraDelayPerRound)
{
	public const int MaximumRoundsPerTrigger = 10;
}

public sealed record FirearmAttachmentModifiers(
	double AccuracyBonus = 0.0,
	double AimBonus = 0.0,
	double DamageMultiplier = 1.0,
	double RangeMultiplier = 1.0,
	double RecoilMultiplier = 1.0,
	double StaminaMultiplier = 1.0,
	double DelayMultiplier = 1.0,
	double AimLossMultiplier = 1.0,
	int LoudnessOffset = 0);

public interface IFirearm : IRangedWeapon, IFirearmAttachmentHost
{
	IReadOnlyCollection<FirearmFireMode> FireModes { get; }
	FirearmFireMode CurrentFireMode { get; }
	FirearmCycleType CycleType { get; }
	double EffectiveAccuracyBonus { get; }
	double EffectiveStaminaToFire { get; }
	double EffectiveFireDelay { get; }
	double EffectiveAimLoss { get; }
	int EffectiveRangeInRooms { get; }
	bool SetFireMode(FirearmFireModeType mode);
}

public static class FirearmItemExtensions
{
	public static IEnumerable<IGameItem> IncludingFirearmAttachments(this IEnumerable<IGameItem> items)
	{
		return items.SelectMany(x => new[] { x }.Concat(
			x.GetItemType<IFirearmAttachmentHost>()?.InstalledAttachments.Values.Select(y => y.Parent) ?? []));
	}
}

public static class FirearmMath
{
	public static FirearmAttachmentModifiers CombineModifiers(IEnumerable<FirearmAttachmentModifiers> modifiers)
	{
		var values = modifiers.ToList();
		return new FirearmAttachmentModifiers(
			values.Sum(x => x.AccuracyBonus),
			values.Sum(x => x.AimBonus),
			values.Aggregate(1.0, (value, x) => value * x.DamageMultiplier),
			values.Aggregate(1.0, (value, x) => value * x.RangeMultiplier),
			values.Aggregate(1.0, (value, x) => value * x.RecoilMultiplier),
			values.Aggregate(1.0, (value, x) => value * x.StaminaMultiplier),
			values.Aggregate(1.0, (value, x) => value * x.DelayMultiplier),
			values.Aggregate(1.0, (value, x) => value * x.AimLossMultiplier),
			values.Sum(x => x.LoudnessOffset));
	}

	public static Outcome ProjectileOutcome(Outcome baseOutcome, FirearmFireMode mode, int roundIndex,
		int projectileIndex, double projectileSpreadPenalty, double recoilMultiplier)
	{
		var recoilSteps = (int)Math.Floor(
			(mode.RecoilPenalty * Math.Max(0, roundIndex) +
			 projectileSpreadPenalty * Math.Max(0, projectileIndex)) *
			Math.Max(0.0, recoilMultiplier));
		return baseOutcome.StageDown(Math.Max(0, recoilSteps));
	}
}

public interface IFirearmAttachmentHost : IGameItemComponent
{
	IReadOnlyCollection<FirearmAttachmentSlot> AttachmentSlots { get; }
	IReadOnlyDictionary<string, IFirearmAttachment> InstalledAttachments { get; }
	FirearmAttachmentModifiers CombinedAttachmentModifiers { get; }
	bool CanAttach(IFirearmAttachment attachment, string? slotName, out string whyNot);
	bool Attach(IFirearmAttachment attachment, string? slotName, out string whyNot);
	bool CanDetach(IFirearmAttachment attachment, out string whyNot);
	bool Detach(IFirearmAttachment attachment, out string whyNot);
}

public interface IFirearmAttachment : IGameItemComponent
{
	FirearmAttachmentSlotType SlotType { get; }
	IReadOnlyCollection<string> FormFactors { get; }
	FirearmAttachmentModifiers Modifiers { get; }
	string? FireEmote { get; }
	IFirearmAttachmentHost? InstalledIn { get; set; }

	bool Fits(FirearmAttachmentSlot slot)
	{
		return SlotType == slot.Type &&
		       FormFactors.Any(x => x.EqualTo(slot.FormFactor));
	}
}
