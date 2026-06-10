using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Character;

public sealed class MountGearProfile
{
	private readonly List<string> _warnings = new();

	public MountGearProfile(IEnumerable<IGameItem> gearItems, RidingGearRole roles, double controlBonus,
		double stabilityBonus)
	{
		GearItems = gearItems.Distinct().ToList();
		Roles = roles;
		ControlBonus = controlBonus;
		StabilityBonus = stabilityBonus;

		HasSaddle = Roles.HasFlag(RidingGearRole.Saddle) || Roles.HasFlag(RidingGearRole.PackSaddle);
		HasSaddlePad = Roles.HasFlag(RidingGearRole.SaddlePad);
		HasStirrups = Roles.HasFlag(RidingGearRole.Stirrups);
		HasBridle = Roles.HasFlag(RidingGearRole.Bridle) || Roles.HasFlag(RidingGearRole.BitlessControl);
		HasReins = Roles.HasFlag(RidingGearRole.Reins) || Roles.HasFlag(RidingGearRole.BitlessControl);
		HasBit = Roles.HasFlag(RidingGearRole.Bit) || Roles.HasFlag(RidingGearRole.BitlessControl);
		UsesBitlessControl = Roles.HasFlag(RidingGearRole.BitlessControl);

		if (!HasSaddle)
		{
			ControlBonus -= 5.0;
			StabilityBonus -= 10.0;
			_warnings.Add("no saddle or pack saddle");
		}
		else if (!HasSaddlePad)
		{
			StabilityBonus -= 2.0;
			_warnings.Add("no saddle pad");
		}

		if (!HasStirrups && HasSaddle)
		{
			StabilityBonus -= 5.0;
			_warnings.Add("no stirrups");
		}

		if (!HasBridle)
		{
			ControlBonus -= 10.0;
			_warnings.Add("no bridle or equivalent control headgear");
		}

		if (!HasReins)
		{
			ControlBonus -= 10.0;
			_warnings.Add("no reins or equivalent control aid");
		}

		if (!HasBit && HasBridle && HasReins)
		{
			ControlBonus -= 5.0;
			_warnings.Add("no bit or explicit bitless-control gear");
		}
	}

	public IReadOnlyList<IGameItem> GearItems { get; }
	public RidingGearRole Roles { get; }
	public double ControlBonus { get; private set; }
	public double StabilityBonus { get; private set; }
	public bool HasSaddle { get; }
	public bool HasSaddlePad { get; }
	public bool HasStirrups { get; }
	public bool HasBridle { get; }
	public bool HasReins { get; }
	public bool HasBit { get; }
	public bool UsesBitlessControl { get; }
	public IReadOnlyList<string> Warnings => _warnings;

	public Difficulty AdjustControlDifficulty(Difficulty difficulty)
	{
		return difficulty.ApplyBonus(ControlBonus);
	}

	public Difficulty AdjustStabilityDifficulty(Difficulty difficulty)
	{
		return difficulty.ApplyBonus(StabilityBonus);
	}
}
