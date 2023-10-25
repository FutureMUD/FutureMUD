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
	}

	/// <inheritdoc />
	protected TerritorialWanderer(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
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

			if (SuitableTerritoryProg.ReturnType != FutureProgVariableTypes.Boolean)
			{
				throw new ApplicationException(
					$"TerritorialWanderer #{Id} ({Name}) specified a SuitableTerritoryProg with a return type of {SuitableTerritoryProg.ReturnType.Describe()} (expected boolean).");
			}

			if (!SuitableTerritoryProg.MatchesParameters(new[]
			    {
				    FutureProgVariableTypes.Location,
				    FutureProgVariableTypes.Character
			    }) && !SuitableTerritoryProg.MatchesParameters(new[]
			    {
				    FutureProgVariableTypes.Location,
				    FutureProgVariableTypes.Toon
			    }) &&
			    !SuitableTerritoryProg.MatchesParameters(new[]
			    {
				    FutureProgVariableTypes.Location
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

			if (DesiredTerritorySizeProg.ReturnType != FutureProgVariableTypes.Number)
			{
				throw new ApplicationException(
					$"TerritorialWanderer #{Id} ({Name}) specified a DesiredTerritorySizeProg with a return type of {DesiredTerritorySizeProg.ReturnType.Describe()} (expected number).");
			}

			if (!DesiredTerritorySizeProg.MatchesParameters(new[]
			    {
				    FutureProgVariableTypes.Character
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
			    FutureProgVariableTypes.Location
		    }) &&
		    !SuitableTerritoryProg.MatchesParameters(new[]
		    {
			    FutureProgVariableTypes.Location,
			    FutureProgVariableTypes.Toon
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
		CreatePathingEffect(character);
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
	protected override IEnumerable<ICellExit> GetPath(ICharacter ch)
	{
		var territoryEffect = ch.CombinedEffectsOfType<Territory>().FirstOrDefault();
		if (territoryEffect is null)
		{
			territoryEffect = new Territory(ch);
			ch.AddEffect(territoryEffect);
		}

		var path = ch.PathBetween(territoryEffect.Cells, 20, GetSuitabilityFunction(ch, true));
		return path;
	}
}