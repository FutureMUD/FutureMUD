using System;
using System.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Combat;

public class TargetLimbWeaponAttack : WeaponAttack, ITargetLimbWeaponAttack
{
	#region Overrides of WeaponAttack

	protected override void LoadFromDatabase(MudSharp.Models.WeaponAttack attack)
	{
		base.LoadFromDatabase(attack);
		TargetLimbType = (LimbType)int.Parse(attack.AdditionalInfo);
	}

	protected override void SeedInitialData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = ((int)LimbType.Arm).ToString();
	}

	#endregion

	public TargetLimbWeaponAttack(MudSharp.Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld)
	{
	}

	public TargetLimbWeaponAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
	{
		TargetLimbType = LimbType.Arm;
	}

	public LimbType TargetLimbType { get; set; }

	protected override void AddAttackSpecificCloneData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = ((int)TargetLimbType).ToString();
	}

	protected override void SaveAttackSpecificData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = ((int)TargetLimbType).ToString();
	}

	protected override string ShowBuilderInternal(ICharacter actor)
	{
		return $"Target Limb: {TargetLimbType.DescribePlural().Colour(Telnet.Green)}";
	}

	public override string HelpText => $@"{base.HelpText}
	#3limb <type>#0 - sets the limb type this attack targets";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Peek().ToLowerInvariant())
		{
			case "limb":
			case "limbtype":
			case "limb type":
				return BuildingCommandLimbType(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandLimbType(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which limb type should this attack target? Valid types are: {Enum.GetNames(typeof(LimbType)).Select(x => x.Colour(Telnet.Green)).ListToString()}.");
			return false;
		}

		if (!Enum.TryParse<LimbType>(command.PopSpeech(), true, out var result))
		{
			actor.OutputHandler.Send(
				$"That is not a valid limb type. Valid types are: {Enum.GetNames(typeof(LimbType)).Select(x => x.Colour(Telnet.Green)).ListToString()}.");
			return false;
		}

		TargetLimbType = result;
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack will now target {TargetLimbType.DescribePlural().Colour(Telnet.Green)}.");
		return true;
	}
}