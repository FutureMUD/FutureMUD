using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class RescuerAI : ArtificialIntelligenceBase
{
	protected RescuerAI(Models.ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	private RescuerAI()
	{

	}

	private RescuerAI(IFuturemud gameworld, string name) : base(gameworld, name, "Rescuer")
	{
		IsFriendProg = Gameworld.AlwaysFalseProg;
		DatabaseInitialise();
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("IsFriendProg", IsFriendProg?.Id ?? 0L)
		).ToString();
	}

	public IFutureProg IsFriendProg { get; set; }

	public static void RegisterLoader()
	{
		RegisterAIType("Rescuer", (ai, gameworld) => new RescuerAI(ai, gameworld));
		RegisterAIBuilderInformation("rescuer", (gameworld, name) => new RescuerAI(gameworld, name), new RescuerAI().HelpText);
	}

	private void LoadFromXml(XElement root)
	{
		IsFriendProg =
			long.TryParse(
				root.Element("IsFriendProg")?.Value ??
				throw new ApplicationException($"The RescuerAI with ID {Id} did not have an IsFriendProg attribute"),
				out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(root.Element("IsFriendProg").Value);
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = (ICharacter)arguments[0];
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.TenSecondTick:
				return HandleTenSecondTick((ICharacter)arguments[0]);
		}

		return false;
	}

	private bool HandleTenSecondTick(ICharacter character)
	{
		if (!IsGenerallyAble(character))
		{
			return false;
		}

		if (character.AffectedBy<IRescueEffect>())
		{
			return false;
		}

		if (IsFriendProg is null)
		{
			return false;
		}

		ICharacter friendlyCombatant;
		if (IsFriendProg.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character }))
		{
			friendlyCombatant = character.Location
										 .LayerCharacters(character.RoomLayer)
										 .Where(x => x != character && x.Combat != null &&
													 IsFriendProg.Execute<bool?>(character, x) == true)
										 .GetWeightedRandom(x => x.MeleeRange ? 100.0 : 1.0);
		}
		else
		{
			friendlyCombatant = character.Location
										 .LayerCharacters(character.RoomLayer)
										 .Where(x => x != character && x.Combat != null &&
													 IsFriendProg.Execute<bool?>(x) == true)
										 .GetWeightedRandom(x => x.MeleeRange ? 100.0 : 1.0);
		}

		if (friendlyCombatant == null)
		{
			return false;
		}

		if (character.Combat != friendlyCombatant.Combat)
		{
			var opponent = friendlyCombatant.Combat.Combatants
			                                .Where(x =>
				                                x.CombatTarget == friendlyCombatant &&
				                                x.ColocatedWith(character) &&
				                                character.CanEngage(x)
			                                )
			                                .GetRandomElement();
			character.Engage(opponent);
		}

		character.AddEffect(new Rescue(character, friendlyCombatant));
		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.TenSecondTick:
					return true;
			}
		}

		return false;
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Is Friend Prog: {IsFriendProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => @"	#3friend <prog>#0 - sets the prog that determines whether someone is a friend and will be rescued";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "friend":
			case "isfriend":
			case "friendly":
			case "isfriendly":
			case "friendprog":
			case "isfriendprog":
			case "friendlyprog":
			case "isfriendlyprog":
				return BuildingCommandIsFriendlyProg(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandIsFriendlyProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to control whether this NPC will rescue a target?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		IsFriendProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This NPC will now use the {prog.MXPClickableFunctionName()} prog to determine whether to rescue a target.");
		return true;
	}
}