#nullable enable

using MudSharp.Effects;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using System.Threading;

namespace MudSharp.Construction.Boundary;

public class TransientExit : PerceivedItem, ITransientExit, IMagicPortalExit
{
	private static long _nextId;
	private readonly List<IRoom> _cells = new();
	private readonly IRoomExit[] _cellExits = new IRoomExit[2];
	private readonly List<RoomLayer> _blockedLayers = new();

	public TransientExit(IFuturemud gameworld, IRoom origin, IRoom destination, string verb, string outboundKeyword,
		string inboundKeyword, string outboundTarget, string inboundTarget, string outboundDescription,
		string inboundDescription, double timeMultiplier, ICharacter? caster = null, IMagicSpell? spell = null,
		IEffect? sourceEffect = null, string? stableKey = null)
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
		StableKey = stableKey ?? $"runtime:{Guid.NewGuid():D}";
		Verb = verb;
		OutboundKeyword = outboundKeyword;
		InboundKeyword = inboundKeyword;
		Source = origin;
		Destination = destination;

		var outboundKeywords = KeywordsFor(outboundTarget, outboundKeyword);
		var inboundKeywords = KeywordsFor(inboundTarget, inboundKeyword);
		_cellExits[0] = new NonCardinalRoomExit(this, origin, destination, verb, outboundKeyword, outboundKeywords,
			outboundDescription, outboundTarget, inboundDescription, inboundTarget);
		_cellExits[1] = new NonCardinalRoomExit(this, destination, origin, verb, inboundKeyword, inboundKeywords,
			outboundDescription, inboundTarget, inboundDescription, outboundTarget);
	}

	/// <summary>
	/// Creates an isolated copy of one existing exit between two transient cells. The copied exit deliberately
	/// has no door reference, so a combat simulation can never open, close or otherwise mutate a live door.
	/// </summary>
	internal TransientExit(
		IFuturemud gameworld,
		IRoom origin,
		IRoom destination,
		IExit sourceExit,
		IRoom sourceOrigin,
		string stableKey)
	{
		var sourceOriginExit = sourceExit.RoomExitFor(sourceOrigin) ??
		                       throw new ArgumentException("The source exit does not leave the supplied source cell.",
			                       nameof(sourceOrigin));
		var sourceDestinationExit = sourceOriginExit.Opposite ??
		                            throw new ArgumentException("The source exit has no opposite side.", nameof(sourceExit));
		Gameworld = gameworld;
		_id = Interlocked.Decrement(ref _nextId);
		IdInitialised = true;
		_name = $"transient portal {Math.Abs(_id):N0}";
		_cells.Add(origin);
		_cells.Add(destination);
		TimeMultiplier = sourceExit.TimeMultiplier;
		MaximumSizeToEnter = sourceExit.MaximumSizeToEnter;
		MaximumSizeToEnterUpright = sourceExit.MaximumSizeToEnterUpright;
		AcceptsDoor = sourceExit.AcceptsDoor;
		DoorSize = sourceExit.DoorSize;
		IsClimbExit = sourceExit.IsClimbExit;
		ClimbDifficulty = sourceExit.ClimbDifficulty;
		foreach (var blockedLayer in sourceExit.BlockedLayers)
		{
			_blockedLayers.Add(blockedLayer);
		}

		FallRoom = ReferenceEquals(sourceExit.FallRoom, sourceOrigin)
			? origin
			: ReferenceEquals(sourceExit.FallRoom, sourceOriginExit.Destination)
				? destination
				: null;
		Caster = null;
		Spell = null;
		SourceEffect = null;
		StableKey = stableKey;
		Source = origin;
		Destination = destination;
		Verb = sourceOriginExit is INonCardinalRoomExit nonCardinalOrigin
			? nonCardinalOrigin.Verb
			: sourceOriginExit.OutboundDirection.Describe().ToLowerInvariant();
		OutboundKeyword = sourceOriginExit is INonCardinalRoomExit nonCardinalOutbound
			? nonCardinalOutbound.PrimaryKeyword
			: sourceOriginExit.OutboundDirection.Describe().ToLowerInvariant();
		InboundKeyword = sourceDestinationExit is INonCardinalRoomExit nonCardinalInbound
			? nonCardinalInbound.PrimaryKeyword
			: sourceDestinationExit.OutboundDirection.Describe().ToLowerInvariant();

		if (sourceOriginExit is INonCardinalRoomExit sourceNonCardinalOrigin &&
		    sourceDestinationExit is INonCardinalRoomExit sourceNonCardinalDestination)
		{
			_cellExits[0] = new NonCardinalRoomExit(this, origin, destination,
				sourceNonCardinalOrigin.Verb, sourceNonCardinalOrigin.PrimaryKeyword,
				sourceNonCardinalOrigin.Keywords, sourceNonCardinalOrigin.OutboundDescription,
				sourceNonCardinalOrigin.OutboundTarget, sourceNonCardinalOrigin.InboundDescription,
				sourceNonCardinalOrigin.InboundTarget);
			_cellExits[1] = new NonCardinalRoomExit(this, destination, origin,
				sourceNonCardinalDestination.Verb, sourceNonCardinalDestination.PrimaryKeyword,
				sourceNonCardinalDestination.Keywords, sourceNonCardinalDestination.OutboundDescription,
				sourceNonCardinalDestination.OutboundTarget, sourceNonCardinalDestination.InboundDescription,
				sourceNonCardinalDestination.InboundTarget);
			return;
		}

		_cellExits[0] = new RoomExit(this, origin, destination, sourceOriginExit.OutboundDirection,
			sourceOriginExit.InboundDirection);
		_cellExits[1] = new RoomExit(this, destination, origin, sourceDestinationExit.OutboundDirection,
			sourceDestinationExit.InboundDirection);
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
	public override IRoom Location => _cellExits[0].Origin;
	public override ProgVariableTypes Type => ProgVariableTypes.Error;

	public bool AcceptsDoor { get; set; }
	public SizeCategory DoorSize { get; set; }
	public IDoor? Door { get; set; }
	public double TimeMultiplier { get; set; }
	public SizeCategory MaximumSizeToEnterUpright { get; set; }
	public SizeCategory MaximumSizeToEnter { get; set; }
	public IEnumerable<IRoom> Rooms => _cells;
	public IExit Exit => this;
	public IRoom Source { get; }
	public IRoom Destination { get; }
	public ICharacter? Caster { get; }
	public IMagicSpell? Spell { get; }
	public IEffect? SourceEffect { get; }
	public string StableKey { get; }
	public string Verb { get; }
	public string OutboundKeyword { get; }
	public string InboundKeyword { get; }
	public IRoom? FallRoom { get; set; }
	public bool IsClimbExit { get; set; }
	public Difficulty ClimbDifficulty { get; set; }
	public IEnumerable<RoomLayer> BlockedLayers => _blockedLayers;

	public IRoomExit? RoomExitFor(IRoom room)
	{
		return _cellExits.FirstOrDefault(x => ReferenceEquals(x.Origin, room) || x.Origin.Id == room.Id);
	}

	public IRoom? Opposite(IRoom room)
	{
		return RoomExitFor(room)?.Destination;
	}

	public bool IsExit(IRoom room, string verb)
	{
		return RoomExitFor(room)?.IsExit(verb) == true;
	}

	public bool IsExitKeyword(IRoom room, string keyword)
	{
		return RoomExitFor(room)?.IsExitKeyword(keyword) == true;
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
