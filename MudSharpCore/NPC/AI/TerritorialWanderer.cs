using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.NPC.AI;

public class TerritorialWanderer : PathingAIBase
{
	public IFutureProg SuitableTerritoryProg { get; protected set; }
	public string WanderEmote { get; protected set; }
	public double WanderChancePerMinute { get; protected set; }
	public IFutureProg DesiredTerritorySizeProg { get; protected set; }
	public bool WillShareTerritory { get; protected set; }
	public bool WillShareTerritoryWithOtherRaces { get; protected set; }

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Suitable Territory Prog: {SuitableTerritoryProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Territory Size Prog: {DesiredTerritorySizeProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Wander Emote: {WanderEmote?.ColourCommand() ?? ""}");
		sb.AppendLine($"Wander Chance Per Minute: {WanderChancePerMinute.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Will Share Territory: {WillShareTerritory.ToColouredString()}");
		sb.AppendLine($"Will Share Territory (Other Races): {WillShareTerritoryWithOtherRaces.ToColouredString()}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3territory <prog>#0 - sets the prog that controls if a room is suitable for territory
	#3size <prog>#0 - sets the prog that controls how much territory each individual wants
	#3wander <emote>#0 - sets the travel emote
	#3wander clear#0 - clears the travel emote
	#3chance <%>#0 - sets the percentage chance per minute of wandering
	#3share#0 - toggles sharing territory with others of its species
	#3shareother#0 - toggles sharing territory with other races";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "territory":
			case "territoryprog":
				return BuildingCommandTerritoryProg(actor, command);
			case "size":
			case "sizeprog":
				return BuildingCommandSizeProg(actor, command);
			case "wander":
				return BuildingCommandWander(actor, command);
			case "chance":
				return BuildingCommandChance(actor, command);
			case "share":
				return BuildingCommandShare(actor);
			case "shareother":
			case "shareothers":
				return BuildingCommandShareOthers(actor);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandShareOthers(ICharacter actor)
	{
		WillShareTerritoryWithOtherRaces = !WillShareTerritoryWithOtherRaces;
		Changed = true;
		actor.OutputHandler.Send(
			$"This AI will {WillShareTerritoryWithOtherRaces.NowNoLonger()} share territory with other races.");
		return true;
	}

	private bool BuildingCommandShare(ICharacter actor)
	{
		WillShareTerritory = !WillShareTerritory;
		Changed = true;
		actor.OutputHandler.Send(
			$"This AI will {WillShareTerritory.NowNoLonger()} share territory with others of its species.");
		return true;
	}

	private bool BuildingCommandChance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.SafeRemainingArgument.TryParsePercentage(out var value) || value < 0.0 || value > 1.0)
		{
			actor.OutputHandler.Send(
				$"You must supply a valid percentage between {0.0.ToString("P0", actor).ColourValue()} and {1.0.ToString("P0", actor).ColourValue()}");
			return false;
		}

		WanderChancePerMinute = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This AI will now have a {value.ToString("P2", actor).ColourValue()} chance of wandering every minute.");
		return true;
	}

	private bool BuildingCommandWander(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a travel emote or use #3clear#0 to clear it."
				.SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "delete", "remove"))
		{
			WanderEmote = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer have any travel emote.");
			return true;
		}

		WanderEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This AI will now do the following travel string when wandering: {WanderEmote.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandSizeProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, 
			ProgVariableTypes.Number, 
			new[] { ProgVariableTypes.Character }
			).LookupProg();
		if (prog is null)
		{
			return false;
		}

		DesiredTerritorySizeProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to determine how many rooms it should have in its territory.");
		return true;
	}

	private bool BuildingCommandTerritoryProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new []
			{
				new[] { ProgVariableTypes.Location },
				new[] { ProgVariableTypes.Location, ProgVariableTypes.Character },
			}
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		SuitableTerritoryProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to determine whether a room is suitable for its territory.");
		return true;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SuitableTerritoryProg", SuitableTerritoryProg?.Id ?? 0),
			new XElement("DesiredTerritorySizeProg", DesiredTerritorySizeProg?.Id ?? 0),
			new XElement("WanderChancePerMinute", WanderChancePerMinute),
			new XElement("WanderEmote", new XCData(WanderEmote)),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay),
			new XElement("WillShareTerritory", WillShareTerritory),
			new XElement("WillShareTerritoryWithOtherRaces", WillShareTerritoryWithOtherRaces)
		).ToString();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("TerritorialWanderer", (ai, gameworld) => new TerritorialWanderer(ai, gameworld));
		RegisterAIBuilderInformation("territorialwanderer", (game, name) => new TerritorialWanderer(game, name), new TerritorialWanderer().HelpText);
	}

	/// <inheritdoc />
	protected TerritorialWanderer(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private TerritorialWanderer()
	{

	}

	private TerritorialWanderer(IFuturemud gameworld, string name) : base(gameworld, name, "TerritorialWanderer")
	{
		SuitableTerritoryProg = Gameworld.AlwaysTrueProg;
		DesiredTerritorySizeProg = Gameworld.AlwaysOneProg;
		WanderEmote = string.Empty;
		WanderChancePerMinute = 0.33;
		WillShareTerritory = false;
		WillShareTerritoryWithOtherRaces = true;
		DatabaseInitialise();
	}

	#region Overrides of PathingAIBase

	/// <inheritdoc />
	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		var element = root.Element("SuitableTerritoryProg");
		if (element != null)
		{
			SuitableTerritoryProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (SuitableTerritoryProg == null)
			{
				throw new ApplicationException(
					$"TerritorialWanderer #{Id} ({Name}) specified a null SuitableTerritoryProg.");
			}

			if (SuitableTerritoryProg.ReturnType != ProgVariableTypes.Boolean)
			{
				throw new ApplicationException(
					$"TerritorialWanderer #{Id} ({Name}) specified a SuitableTerritoryProg with a return type of {SuitableTerritoryProg.ReturnType.Describe()} (expected boolean).");
			}

			if (!SuitableTerritoryProg.MatchesParameters(new[]
				{
					ProgVariableTypes.Location,
					ProgVariableTypes.Character
				}) && !SuitableTerritoryProg.MatchesParameters(new[]
				{
					ProgVariableTypes.Location,
					ProgVariableTypes.Toon
				}) &&
				!SuitableTerritoryProg.MatchesParameters(new[]
				{
					ProgVariableTypes.Location
				})
			   )
			{
				throw new ApplicationException(
					$"TerritorialWanderer #{Id} ({Name}) specified a SuitableTerritoryProg that was not compatible with the expected parameter inputs of location,character or location,toon or location.");
			}
		}
		else
		{
			throw new ApplicationException(
				$"TerritorialWanderer #{Id} ({Name}) did not supply a SuitableTerritoryProg element.");
		}

		element = root.Element("DesiredTerritorySizeProg");
		if (element != null)
		{
			DesiredTerritorySizeProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
			if (DesiredTerritorySizeProg == null)
			{
				throw new ApplicationException(
					$"TerritorialWanderer #{Id} ({Name}) specified a null DesiredTerritorySizeProg.");
			}

			if (DesiredTerritorySizeProg.ReturnType != ProgVariableTypes.Number)
			{
				throw new ApplicationException(
					$"TerritorialWanderer #{Id} ({Name}) specified a DesiredTerritorySizeProg with a return type of {DesiredTerritorySizeProg.ReturnType.Describe()} (expected number).");
			}

			if (!DesiredTerritorySizeProg.MatchesParameters(new[]
				{
					ProgVariableTypes.Character
				})
			   )
			{
				throw new ApplicationException(
					$"TerritorialWanderer #{Id} ({Name}) specified a DesiredTerritorySizeProg that was not compatible with the expected parameter inputs of character.");
			}
		}
		else
		{
			throw new ApplicationException(
				$"TerritorialWanderer #{Id} ({Name}) did not supply a DesiredTerritorySizeProg element.");
		}

		element = root.Element("WanderChancePerMinute");
		if (element != null)
		{
			if (!double.TryParse(element.Value, out var value))
			{
				throw new ApplicationException(
					$"TerritorialWanderer #{Id} ({Name}) supplied a WanderChancePerMinute that was not a number.");
			}

			WanderChancePerMinute = value;
		}
		else
		{
			throw new ApplicationException(
				$"TerritorialWanderer #{Id} ({Name}) did not supply a WanderChancePerMinute element.");
		}

		WanderEmote = root.Element("WanderEmote")?.Value ??
					  throw new ApplicationException(
						  $"TerritorialWanderer #{Id} ({Name}) did not supply a WanderEmote element.");

		OpenDoors = bool.Parse(root.Element("OpenDoors")?.Value ?? "false");
		UseKeys = bool.Parse(root.Element("UseKeys")?.Value ?? "false");
		SmashLockedDoors = bool.Parse(root.Element("SmashLockedDoors")?.Value ?? "false");
		CloseDoorsBehind = bool.Parse(root.Element("CloseDoorsBehind")?.Value ?? "false");
		UseDoorguards = bool.Parse(root.Element("UseDoorguards")?.Value ?? "false");
		MoveEvenIfObstructionInWay = bool.Parse(root.Element("MoveEvenIfObstructionInWay")?.Value ?? "false");
		WillShareTerritory = bool.Parse(root.Element("WillShareTerritory")?.Value ?? "false");
		WillShareTerritoryWithOtherRaces =
			bool.Parse(root.Element("WillShareTerritoryWithOtherRaces")?.Value ?? "false");
	}

	#endregion

	public IEnumerable<ICell> PotentialFreeTerritory(ICharacterTemplate template, IEnumerable<IZone> zones)
	{
		if (!SuitableTerritoryProg.MatchesParameters(new[]
			{
				ProgVariableTypes.Location
			}) &&
			!SuitableTerritoryProg.MatchesParameters(new[]
			{
				ProgVariableTypes.Location,
				ProgVariableTypes.Toon
			}))
		{
			return Enumerable.Empty<ICell>();
		}

		ICollection<ICell> claimedTerritory;
		if (WillShareTerritory)
		{
			claimedTerritory = new List<ICell>();
		}
		else if (WillShareTerritoryWithOtherRaces)
		{
			claimedTerritory = template.Gameworld.NPCs
									   .Where(x => !x.Race.SameRace(template.SelectedRace))
									   .SelectNotNull(x => x.CombinedEffectsOfType<Territory>().FirstOrDefault())
									   .SelectMany(x => x.Cells)
									   .Distinct()
									   .ToList();
		}
		else
		{
			claimedTerritory = template.Gameworld.NPCs
									   .SelectNotNull(x => x.CombinedEffectsOfType<Territory>().FirstOrDefault())
									   .SelectMany(x => x.Cells)
									   .Distinct()
									   .ToList();
		}

		return zones
			   .SelectMany(x => x.Cells)
			   .Except(claimedTerritory)
			   .Where(x => SuitableTerritoryProg?.Execute<bool?>(x, template) ?? false)
			;
	}

	private void EvaluateWander(ICharacter character)
	{
		if (character.State.HasFlag(CharacterState.Dead) || !character.State.IsAble())
		{
			return;
		}

		if (character.Movement != null)
		{
			return;
		}

		if (character.EffectsOfType<FollowingPath>().Any())
		{
			return;
		}

		if (character.Combat is not null || character.Effects.Any(x => x.IsBlockingEffect("movement")))
		{
			return;
		}

		var territoryEffect = character.CombinedEffectsOfType<Territory>().First();
		if (!territoryEffect.Cells.Any())
		{
			return;
		}

		if (territoryEffect.Cells.Contains(character.Location))
		{
			// Wandering in territory
			if (RandomUtilities.DoubleRandom(0.0, 1.0) > WanderChancePerMinute)
			{
				return;
			}

			var suitability = GetSuitabilityFunction(character);
			var options = character.Location.ExitsFor(character)
								   .Where(x => territoryEffect.Cells.Contains(x.Destination))
								   .Where(x => suitability(x))
								   .ToList();
			if (options.Any())
			{
				character.Move(options.GetWeightedRandom(x => !character.AffectedBy<AdjacentToExit>(x) ? 5.0 : 1.0),
					new PlayerEmote(WanderEmote, character), true);
			}

			return;
		}

		// Outside of territory, try to get back
		CreatePathingEffectIfPathExists(character);
	}

	protected void EvaluateTerritory(ICharacter character)
	{
		var territoryEffect = character.CombinedEffectsOfType<Territory>().FirstOrDefault();
		if (territoryEffect is null)
		{
			territoryEffect = new Territory(character);
			character.AddEffect(territoryEffect);
		}

		var cells = territoryEffect.Cells.ToList();
		if (cells.Count < DesiredTerritorySizeProg.ExecuteInt(0, character))
		{
			ICollection<ICell> claimedTerritory;
			if (WillShareTerritory)
			{
				claimedTerritory = new List<ICell>();
			}
			else if (WillShareTerritoryWithOtherRaces)
			{
				claimedTerritory = character.Gameworld.NPCs
											.Where(x => !x.Race.SameRace(character.Race))
											.SelectNotNull(x => x.CombinedEffectsOfType<Territory>().FirstOrDefault())
											.SelectMany(x => x.Cells)
											.Distinct()
											.ToList();
			}
			else
			{
				claimedTerritory = character.Gameworld.NPCs
											.SelectNotNull(x => x.CombinedEffectsOfType<Territory>().FirstOrDefault())
											.SelectMany(x => x.Cells)
											.Distinct()
											.ToList();
			}

			if (cells.Count == 0)
			{
				if (SuitableTerritoryProg.Execute<bool?>(character.Location, character) == true &&
					!claimedTerritory.Contains(character.Location))
				{
					territoryEffect.AddCell(character.Location);
					return;
				}

				var (target, _) = character.AcquireTargetAndPath(
					loc => SuitableTerritoryProg.Execute<bool?>(loc, character) == true &&
						   !claimedTerritory.Contains(loc),
					20, GetSuitabilityFunction(character));
				if (target is not ICell cell)
				{
					return;
				}

				territoryEffect.AddCell(cell);
				return;
			}

			foreach (var cell in territoryEffect.Cells)
			{
				var expand = cell
							 .ExitsFor(character, true)
							 .Where(x => SuitableTerritoryProg.Execute<bool?>(x.Destination, character) == true &&
										 !claimedTerritory.Contains(x.Destination))
							 .Select(x => x.Destination)
							 .GetRandomElement();
				if (expand is not null && !territoryEffect.Cells.Contains(expand))
				{
					territoryEffect.AddCell(expand);
					return;
				}
			}
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = arguments[0] as ICharacter;
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.MinuteTick:
				EvaluateTerritory(arguments[0]);
				EvaluateWander(arguments[0]);
				break;
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.MinuteTick:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	/// <inheritdoc />
	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		var territoryEffect = ch.CombinedEffectsOfType<Territory>().FirstOrDefault();
		if (territoryEffect is null)
		{
			territoryEffect = new Territory(ch);
			ch.AddEffect(territoryEffect);
		}

		var path = ch.PathBetween(territoryEffect.Cells, 20, GetSuitabilityFunction(ch, true)).ToList();
		return (path.Last().Destination, path);
	}
}