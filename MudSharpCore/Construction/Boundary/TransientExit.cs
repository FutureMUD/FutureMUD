#nullable enable

using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MudSharp.Construction.Boundary;

public class TransientExit : PerceivedItem, IExit, IMagicPortalExit
{
	private static long _nextId;
	private readonly List<ICell> _cells = new();
	private readonly ICellExit[] _cellExits = new ICellExit[2];
	private readonly List<RoomLayer> _blockedLayers = new();

	public TransientExit(IFuturemud gameworld, ICell origin, ICell destination, string verb, string outboundKeyword,
		string inboundKeyword, string outboundTarget, string inboundTarget, string outboundDescription,
		string inboundDescription, double timeMultiplier, ICharacter? caster = null, IMagicSpell? spell = null,
		IEffect? sourceEffect = null)
	{
		Gameworld = gameworld;
		_id = Interlocked.Decrement(ref _nextId);
		IdInitialised = true;
		_name = $"transient portal {Math.Abs(_id):N0}";
		_cells.Add(origin);
		_cells.Add(destination);
		TimeMultiplier = timeMultiplier;
		MaximumSizeToEnter = SizeCategory.Titanic;
		MaximumSizeToEnterUpright = SizeCategory.Titanic;
		AcceptsDoor = false;
		ClimbDifficulty = Difficulty.Normal;
		Caster = caster;
		Spell = spell;
		SourceEffect = sourceEffect;
		Verb = verb;
		OutboundKeyword = outboundKeyword;
		InboundKeyword = inboundKeyword;
		Source = origin;
		Destination = destination;

		var outboundKeywords = KeywordsFor(outboundTarget, outboundKeyword);
		var inboundKeywords = KeywordsFor(inboundTarget, inboundKeyword);
		_cellExits[0] = new NonCardinalCellExit(this, origin, destination, verb, outboundKeyword, outboundKeywords,
			outboundDescription, outboundTarget, inboundDescription, inboundTarget);
		_cellExits[1] = new NonCardinalCellExit(this, destination, origin, verb, inboundKeyword, inboundKeywords,
			outboundDescription, inboundTarget, inboundDescription, outboundTarget);
	}

	private static IEnumerable<string> KeywordsFor(string target, string keyword)
	{
		return target
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(x => x.Trim().ToLowerInvariant())
			.Where(x => x.Length > 1 && x != "a" && x != "an" && x != "the")
			.Concat([keyword.ToLowerInvariant()])
			.Distinct(StringComparer.InvariantCultureIgnoreCase);
	}

	public override string FrameworkItemType => "TransientExit";
	public override ICell Location => _cellExits[0].Origin;
	public override ProgVariableTypes Type => ProgVariableTypes.Error;

	public bool AcceptsDoor { get; set; }
	public SizeCategory DoorSize { get; set; }
	public IDoor? Door { get; set; }
	public double TimeMultiplier { get; set; }
	public SizeCategory MaximumSizeToEnterUpright { get; set; }
	public SizeCategory MaximumSizeToEnter { get; set; }
	public IEnumerable<ICell> Cells => _cells;
	public IExit Exit => this;
	public ICell Source { get; }
	public ICell Destination { get; }
	public ICharacter? Caster { get; }
	public IMagicSpell? Spell { get; }
	public IEffect? SourceEffect { get; }
	public string Verb { get; }
	public string OutboundKeyword { get; }
	public string InboundKeyword { get; }
	public ICell? FallCell { get; set; }
	public bool IsClimbExit { get; set; }
	public Difficulty ClimbDifficulty { get; set; }
	public IEnumerable<RoomLayer> BlockedLayers => _blockedLayers;

	public ICellExit? CellExitFor(ICell cell)
	{
		return _cellExits.FirstOrDefault(x => ReferenceEquals(x.Origin, cell) || x.Origin.Id == cell.Id);
	}

	public ICell? Opposite(ICell cell)
	{
		return CellExitFor(cell)?.Destination;
	}

	public bool IsExit(ICell cell, string verb)
	{
		return CellExitFor(cell)?.IsExit(verb) == true;
	}

	public bool IsExitKeyword(ICell cell, string keyword)
	{
		return CellExitFor(cell)?.IsExitKeyword(keyword) == true;
	}

	public IExit Clone()
	{
		throw new NotSupportedException("Transient exits cannot be cloned.");
	}

	public void PostLoadTasks(MudSharp.Models.Exit exit)
	{
	}

	public void AddBlockedLayer(RoomLayer layer)
	{
		if (!_blockedLayers.Contains(layer))
		{
			_blockedLayers.Add(layer);
		}
	}

	public void RemoveBlockedLayer(RoomLayer layer)
	{
		_blockedLayers.Remove(layer);
	}

	public void Delete()
	{
		Gameworld.ExitManager.UnregisterTransientExit(this);
	}

	public override void Register(IOutputHandler handler)
	{
	}

	public override object DatabaseInsert()
	{
		throw new NotSupportedException("Transient exits are never saved to the database.");
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		throw new NotSupportedException("Transient exits do not load database IDs.");
	}

	public override void Save()
	{
		Changed = false;
	}
}
