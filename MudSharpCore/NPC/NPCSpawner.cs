using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;
using Org.BouncyCastle.Asn1.X509;

namespace MudSharp.NPC;
#nullable enable
public class NPCSpawner : SaveableItem, INPCSpawner
{
	public NPCSpawner(IFuturemud game, string name)
	{
		Gameworld = game;
		_name = name;
		_minimumCount = 0;
		_targetCount = 1;
		IsActiveProg = Gameworld.FutureProgs.GetByName("AlwaysFalse");
		using (new FMDB())
		{
			var dbitem = new Models.NPCSpawner
			{
				Name = name,
				MinimumCount = _minimumCount,
				TargetCount = _targetCount,
				SpawnStrategy = (int)SpawnStrategy
			};
			FMDB.Context.NpcSpawners.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public NPCSpawner(NPCSpawner other, string newName)
	{
		Gameworld = other.Gameworld;
		_name = newName;
		_targetTemplateId = other._targetTemplateId;
		_targetCount = other._targetCount;
		_minimumCount = other._minimumCount;
		_monitoredZones.AddRange(other._monitoredZones);
		OnSpawnProg = other.OnSpawnProg;
		CountAsNPCProg = other.CountAsNPCProg;
		IsActiveProg = other.IsActiveProg;
		SpawnStrategy = other.SpawnStrategy;
		using (new FMDB())
		{
			var dbitem = new Models.NPCSpawner
			{
				Name = _name,
				MinimumCount = _minimumCount,
				TargetCount = _targetCount,
				SpawnStrategy = (int)SpawnStrategy,
				OnSpawnProgId = OnSpawnProg?.Id,
				IsActiveProgId = IsActiveProg?.Id,
				CountsAsProgId = CountAsNPCProg?.Id,
				TargetTemplateId = _targetTemplateId
			};
			FMDB.Context.NpcSpawners.Add(dbitem);
			foreach (var cell in _spawnLocations)
			{
				dbitem.Cells.Add(new NPCSpawnerCell { CellId = cell, NPCSpawner = dbitem });
			}

			foreach (var zone in _monitoredZones)
			{
				dbitem.Zones.Add(new NPCSpawnerZone() { ZoneId = zone, NPCSpawner = dbitem });
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public NPCSpawner(Models.NPCSpawner dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_minimumCount = dbitem.MinimumCount;
		_targetCount = dbitem.TargetCount;
		_targetTemplateId = dbitem.TargetTemplateId;
		CountAsNPCProg = Gameworld.FutureProgs.Get(dbitem.CountsAsProgId ?? 0L);
		IsActiveProg = Gameworld.FutureProgs.Get(dbitem.IsActiveProgId ?? 0L);
		OnSpawnProg = Gameworld.FutureProgs.Get(dbitem.OnSpawnProgId ?? 0L);
		SpawnStrategy = (SpawnStrategy)dbitem.SpawnStrategy;
		_monitoredZones.AddRange(dbitem.Zones.Select(x => x.ZoneId));
		_spawnLocations.AddRange(dbitem.Cells.Select(x => x.CellId));
	}

	#region Overrides of FrameworkItem

	/// <inheritdoc />
	public override string FrameworkItemType => "NPCSpawner";

	#endregion

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.NpcSpawners.Find(Id);
		dbitem.Name = Name;
		dbitem.CountsAsProgId = CountAsNPCProg?.Id;
		dbitem.OnSpawnProgId = OnSpawnProg?.Id;
		dbitem.IsActiveProgId = IsActiveProg?.Id;
		dbitem.SpawnStrategy = (int)SpawnStrategy;
		dbitem.TargetTemplateId = _targetTemplateId;
		dbitem.TargetCount = _targetCount;
		dbitem.MinimumCount = _minimumCount;
		FMDB.Context.NpcSpawnerCells.RemoveRange(dbitem.Cells);
		foreach (var cell in _spawnLocations)
		{
			dbitem.Cells.Add(new NPCSpawnerCell { CellId = cell, NPCSpawnerId = Id });
		}

		FMDB.Context.NpcSpawnerZones.RemoveRange(dbitem.Zones);
		foreach (var zone in _monitoredZones)
		{
			dbitem.Zones.Add(new NPCSpawnerZone() { ZoneId = zone, NPCSpawnerId = Id });
		}

		Changed = false;
	}

	#endregion

	#region Implementation of IEditableItem

	public readonly string HelpText = @$"You can use the following options when editing:

	#3name <name>#0 - sets a new name
	#3npc <template>#0 - sets an NPC Template that this spawner uses
	#3target <count>#0 - the number of NPCs this spawner tries to keep active
	#3minimum <count>#0 - the minimum count before this spawner kicks in
	#3strategy {Enum.GetValues<SpawnStrategy>().Select(x => x.DescribeEnum()).ListToCommaSeparatedValues("|")}#0 - sets a strategy
	#3active <prog>#0 - sets a prog that controls whether this is active
	#3active none#0 - clears having a prog control activity
	#3load <prog>#0 - sets a prog that executes when an NPC is loaded
	#3load none#0 - clears having an on-load prog
	#3counts <prog>#0 - sets a prog that determines which NPCs count for this spawner
	#3counts none#0 - clears having a prog determine NPC counting
	#3zone <id>#0 - toggles a zone being monitored by this spawner
	#3cell <id>#0 - toggles a cell being a spawn location for this spawner";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "activeprog":
			case "active":
			case "isactive":
				return BuildingCommandProg(actor, command);
			case "onspawn":
			case "spawn":
			case "load":
			case "onload":
			case "onloadprog":
			case "loadprog":
			case "spawnprog":
				return BuildingCommandSpawnProg(actor, command);
			case "npc":
			case "template":
			case "which":
				return BuildingCommandTemplate(actor, command);
			case "minimum":
			case "min":
				return BuildingCommandMinimum(actor, command);
			case "target":
			case "tar":
				return BuildingCommandTarget(actor, command);
			case "countas":
			case "countsas":
			case "countasprog":
			case "countsasprog":
			case "countprog":
			case "count":
			case "counts":
				return BuildingCommandCountAsProg(actor, command);
			case "strategy":
				return BuildingCommandStrategy(actor, command);
			case "zone":
				return BuildingCommandZone(actor, command);
			case "location":
			case "room":
			case "loc":
			case "cell":
				return BuildingCommandCell(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which zone do you want to toggle this spawner watching?");
			return false;
		}

		var zone = Gameworld.Zones.GetByIdOrName(command.SafeRemainingArgument);
		if (zone is null)
		{
			actor.OutputHandler.Send("There is no such zone.");
			return false;
		}

		if (_monitoredZones.Contains(zone.Id))
		{
			_monitoredZones.Remove(zone.Id);
			actor.OutputHandler.Send(
				$"The {zone.Name.ColourValue()} zone will no longer be monitored by this NPC Spawner.");
		}
		else
		{
			_monitoredZones.Add(zone.Id);
			actor.OutputHandler.Send($"The {zone.Name.ColourValue()} zone will now be monitored by this NPC Spawner.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandCell(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which location do you want to toggle as a spawn location for this NPC Spawner?");
			return false;
		}

		var cell = command.SafeRemainingArgument.EqualTo("here")
			? actor.Location
			: RoomBuilderModule.LookupCell(Gameworld, command.SafeRemainingArgument);
		if (cell is null)
		{
			actor.OutputHandler.Send("There is no such location.");
			return false;
		}

		if (_spawnLocations.Contains(cell.Id))
		{
			_spawnLocations.Remove(cell.Id);
			actor.OutputHandler.Send(
				$"This NPC Spawner will no longer use {cell.GetFriendlyReference(actor).ColourValue()} as a spawn location.");
		}
		else
		{
			_spawnLocations.Add(cell.Id);
			actor.OutputHandler.Send(
				$"This NPC Spawner will now use {cell.GetFriendlyReference(actor).ColourValue()} as a spawn location.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandCountAsProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a prog that determines whether an NPC matches this spawner, or use {"none".ColourCommand()} to clear an existing one.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "delete", "remove"))
		{
			CountAsNPCProg = null;
			Changed = true;
			actor.OutputHandler.Send(
				"This NPC Spawner will no use a prog to determine whether an NPC matches (all NPCs of the appropriate template will match instead).");
			return true;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new[]
			{
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CountAsNPCProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This NPC Spawner will use the {prog.MXPClickableFunctionName()} prog to determine whether an NPC matches this spawner.");
		return true;
	}

	private bool BuildingCommandSpawnProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a prog to be run when an NPC is spawned, or use {"none".ColourCommand()} to clear an existing one.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "delete", "remove"))
		{
			OnSpawnProg = null;
			Changed = true;
			actor.OutputHandler.Send("This NPC Spawner will no longer execute any prog when an NPC is spawned.");
			return true;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void, new[]
			{
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OnSpawnProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This NPC Spawner will now run the {prog.MXPClickableFunctionName()} prog on any NPC that it spawns.");
		return true;
	}

	private bool BuildingCommandTarget(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var amount) || amount < 1)
		{
			actor.OutputHandler.Send("You must specify a target number of NPCs to maintain that is 1 or more.");
			return false;
		}

		_targetCount = amount;
		if (_minimumCount > amount)
		{
			_minimumCount = amount;
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"This NPC Spawner will now try to maintain the number of NPCs at {_targetCount.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMinimum(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var amount) || amount < 0)
		{
			actor.OutputHandler.Send(
				"You must specify a minimum number of NPCs that triggers this spawner to spawn, and it must be 0 or greater.");
			return false;
		}

		_minimumCount = amount;
		if (_targetCount < _minimumCount)
		{
			_targetCount = _minimumCount;
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"This NPC Spawner will now kick in when the NPC count falls to {_minimumCount.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTemplate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which NPC Template should this NPC Spawner use?");
			return false;
		}

		var template = Gameworld.NpcTemplates.GetByIdOrName(command.SafeRemainingArgument);
		if (template is null)
		{
			actor.OutputHandler.Send("There is no NPC Template like that.");
			return false;
		}

		_targetTemplateId = template.Id;
		Changed = true;
		actor.OutputHandler.Send(
			$"This NPC Spawner will now spawn NPCs with the template {template.EditHeader().ColourName()}.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a prog that determines whether this spawner is active.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new FutureProgVariableTypes[]
			{
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		IsActiveProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This NPC Spawner will use the {prog.MXPClickableFunctionName()} prog to determine whether it is active.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.NPCSpawners.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already an NPC Spawner called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename NPC Spawner {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandStrategy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which spawn strategy would you like to use? The valid options are {Enum.GetValues<SpawnStrategy>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out SpawnStrategy strategy))
		{
			actor.OutputHandler.Send(
				$"That is not a valid spawn strategy. The valid options are {Enum.GetValues<SpawnStrategy>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		SpawnStrategy = strategy;
		Changed = true;
		actor.OutputHandler.Send(
			$"This NPC Spawner will now use the {SpawnStrategy.DescribeEnum().ColourName()} spawn strategy.");
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"NPC Spawner #{Id.ToString("N0", actor)} - {Name}".ColourName());
		sb.AppendLine($"Strategy: {SpawnStrategy.DescribeEnum().ColourName()}");
		sb.AppendLine($"NPC Template: {TargetTemplate?.EditHeader() ?? "None".ColourError()}");
		sb.AppendLine($"Is Active Prog: {IsActiveProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"On Spawn Prog: {OnSpawnProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Counts As Prog: {CountAsNPCProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Minimum Count For Spawn: {_minimumCount.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Target Count After Spawn: {_targetCount.ToString("N0", actor).ColourValue()}");
		if (TargetTemplate is not null)
		{
			sb.AppendLine($"Current Count: {CountCurrentNPCs(TargetTemplate).ToString("N0", actor).ColourValue()}");
		}

		sb.AppendLine();
		sb.AppendLine($"Monitored Zones:");
		foreach (var zone in MonitoredZones)
		{
			sb.AppendLine($"\t{zone.Name.ColourValue()} [{zone.Id.ToString("N0", actor)}]");
		}

		sb.AppendLine();
		sb.AppendLine($"Spawn Locations:");
		foreach (var cell in SpawnLocations)
		{
			sb.AppendLine($"\t{cell.GetFriendlyReference(actor).ColourValue()}");
		}

		return sb.ToString();
	}

	#endregion

	public IFutureProg? IsActiveProg { get; private set; }
	private long? _targetTemplateId;
	public INPCTemplate? TargetTemplate => Gameworld.NpcTemplates.Get(_targetTemplateId ?? 0);
	private int _minimumCount;
	private int _targetCount;
	private readonly List<long> _monitoredZones = new();
	public IEnumerable<IZone> MonitoredZones => _monitoredZones.SelectNotNull(x => Gameworld.Zones.Get(x));

	private readonly List<long> _spawnLocations = new();
	public IEnumerable<ICell> SpawnLocations => _spawnLocations.SelectNotNull(x => Gameworld.Cells.Get(x));
	public SpawnStrategy SpawnStrategy { get; private set; }
	public IFutureProg? OnSpawnProg { get; private set; }
	public IFutureProg? CountAsNPCProg { get; private set; }

	#region Implementation of INPCSpawner

	/// <inheritdoc />
	public bool IsActive => IsActiveProg?.Execute<bool?>() ?? false;

	private void DoSpawnNPC(INPCTemplate npcTemplate, ICharacterTemplate chTemplate, ICell location)
	{
		((SimpleCharacterTemplate)chTemplate).SelectedStartingLocation = location;
		var newCharacter = new NPC(Gameworld, chTemplate, npcTemplate);
		OnSpawnProg?.Execute(newCharacter);
		npcTemplate.OnLoadProg?.Execute(newCharacter);

		if (newCharacter.Location.IsSwimmingLayer(newCharacter.RoomLayer) && newCharacter.Race.CanSwim)
		{
			newCharacter.PositionState = PositionSwimming.Instance;
		}
		else if (newCharacter.RoomLayer.IsHigherThan(RoomLayer.GroundLevel) && newCharacter.CanFly().Truth)
		{
			newCharacter.PositionState = PositionFlying.Instance;
		}

		location.Login(newCharacter);
		newCharacter.HandleEvent(EventType.NPCOnGameLoadFinished, newCharacter);
	}

	private bool TrySpawn(INPCTemplate npcTemplate, ICharacterTemplate chTemplate)
	{
		switch (SpawnStrategy)
		{
			case SpawnStrategy.Simple:
				if (!_spawnLocations.Any())
				{
					return false;
				}

				DoSpawnNPC(npcTemplate, chTemplate, SpawnLocations.GetRandomElement());
				return true;
			case SpawnStrategy.OpenTerritory:
				var territorialAI = npcTemplate.ArtificialIntelligences
				                               .OfType<TerritorialWanderer>()
				                               .FirstOrDefault();
				if (territorialAI is null)
				{
					return false;
				}

				var potentialCells = territorialAI.PotentialFreeTerritory(chTemplate, MonitoredZones).ToList();
				if (!potentialCells.Any())
				{
					return false;
				}

				DoSpawnNPC(npcTemplate, chTemplate, potentialCells.GetRandomElement());
				break;
		}

		return false;
	}

	public int CountCurrentNPCs(INPCTemplate npcTemplate)
	{
		return MonitoredZones
		       .SelectMany(x => x.Characters)
		       .OfType<INPC>()
		       .Where(x => x.Template.Id == npcTemplate.Id)
		       .Count(x => CountAsNPCProg?.Execute<bool?>(x) ?? true);
	}

	/// <inheritdoc />
	public void CheckSpawn()
	{
		var npcTemplate = TargetTemplate;
		if (npcTemplate is null)
		{
			return;
		}

		if (!IsActive)
		{
			return;
		}

		if (!_monitoredZones.Any())
		{
			return;
		}

		var count = CountCurrentNPCs(npcTemplate);
		if (count >= _minimumCount)
		{
			return;
		}

		var chTemplate = npcTemplate.GetCharacterTemplate();
		var numberToSpawn = _targetCount - count;
		while (numberToSpawn > 0)
		{
			if (TrySpawn(npcTemplate, chTemplate))
			{
				numberToSpawn--;
				continue;
			}

			break;
		}
	}

	#endregion
}