using System;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg.Functions.GameItem;
using MudSharp.GameItems;

namespace MudSharp.Combat;

public class FixedBodypartWeaponAttack : WeaponAttack, IFixedBodypartWeaponAttack
{
	#region Overrides of WeaponAttack

	protected override void LoadFromDatabase(MudSharp.Models.WeaponAttack attack)
	{
		base.LoadFromDatabase(attack);
		Bodypart = Gameworld.BodypartShapes.Get(long.Parse(attack.AdditionalInfo));
	}

	protected override void SeedInitialData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = Gameworld.BodypartShapes.First().Id.ToString();
	}

	#endregion

	public FixedBodypartWeaponAttack(MudSharp.Models.WeaponAttack attack, IFuturemud gameworld) : base(attack,
		gameworld)
	{
	}

	public FixedBodypartWeaponAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
	{
	}

	#region Implementation of IFixedBodypartWeaponAttack

	public IBodypartShape Bodypart { get; set; }

	#endregion

	#region Overrides of WeaponAttack

	public override bool UsableAttack(IPerceiver attacker, IGameItem weapon, IPerceiver target,
		AttackHandednessOptions handedness,
		bool ignorePosition,
		params BuiltInCombatMoveType[] type)
	{
		return base.UsableAttack(attacker, weapon, target, handedness, ignorePosition, type) &&
		       ((target as IHaveABody)?.Body.Bodyparts.Any(x => x.Shape == Bodypart) ?? true);
	}

	#endregion

	protected override void AddAttackSpecificCloneData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = Bodypart.Id.ToString();
	}

	protected override void SaveAttackSpecificData(MudSharp.Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = Bodypart.Id.ToString();
	}

	protected override string ShowBuilderInternal(ICharacter actor)
	{
		return $"Target Bodypart: {Bodypart?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}";
	}

	#region Overrides of WeaponAttack

	protected override void DescribeForAttacksCommandInternal(StringBuilder sb, ICharacter actor)
	{
		sb.Append(" Hits: ");
		sb.Append(Bodypart.Name.ColourName());
	}

	#endregion

	public override string HelpText => $@"{base.HelpText}
	#3target <shape>#0 - sets the bodypart shape this attack targets";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PeekSpeech().ToLowerInvariant())
		{
			case "target":
			case "targetpart":
			case "targetbodypart":
			case "target bodypart":
			case "target part":
				return BuildingCommandBodypart(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandBodypart(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a bodypart shape for this weapon attack to target.");
			return false;
		}

		var shape = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.BodypartShapes.Get(value)
			: Gameworld.BodypartShapes.GetByName(command.Last);
		if (shape == null)
		{
			actor.OutputHandler.Send("That is not a valid bodypart shape.");
			return false;
		}

		Bodypart = shape;
		Changed = true;
		actor.OutputHandler.Send(
			$"This weapon attack will now target bodyparts of shape {shape.Name.Colour(Telnet.Cyan)}.");
		return true;
	}
}