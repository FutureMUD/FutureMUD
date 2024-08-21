using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Functions.DateTime;
using MudSharp.TimeAndDate;

#nullable enable
namespace MudSharp.Movement;
public class Track : LateInitialisingItem, ITrack
{
	/// <inheritdoc />
	public override string FrameworkItemType => "Track";

	public Track(Models.Track track, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = track.Id;
		IdInitialised = true;
		_bodyProtoTypeId = track.BodyPrototypeId;
		_cellId = track.CellId;
		RoomLayer = (RoomLayer)track.RoomLayer;
		_fromExitId = track.FromDirectionExitId;
		_toExitId = track.ToDirectionExitId;
		_fromSpeedId = track.FromMoveSpeedId;
		_toSpeedId = track.ToMoveSpeedId;
		TrackCircumstances = (TrackCircumstances)track.TrackCircumstances;
		ExertionLevel = (ExertionLevel)track.ExertionLevel;
		TrackIntensityVisual = track.TrackIntensityVisual;
		TrackIntensityOlfactory = track.TrackIntensityOlfactory;
		TurnedAround = track.TurnedAround;
		_characterId = track.CharacterId;
		_mudDateTimeText = track.MudDateTime;
	}

	public Track(IFuturemud gameworld, ICharacter who, ICellExit exit, TrackCircumstances circumstances, bool isLeaving, double visual, double olfactory)
	{
		Gameworld = gameworld;
		_character = who;
		_characterId = who.Id;
		_bodyProtoType = who.Body.Prototype;
		ExertionLevel = who.Body.CurrentExertion;
		TrackCircumstances = circumstances;
		_mudDateTime = who.Location.DateTime();
		RoomLayer = who.RoomLayer;
		TrackIntensityOlfactory = olfactory;
		TrackIntensityVisual = visual;
		_cell = exit.Origin;
		if (isLeaving)
		{
			ToExit = exit.Exit;
			_toSpeed = who.CurrentSpeed;
		}
		else
		{
			FromExit = exit.Exit;
			_fromSpeed = who.CurrentSpeed;
		}

		gameworld.SaveManager.AddInitialisation(this);
	}

	/// <inheritdoc />
	public override void Save()
	{
		if (Deleted)
		{
			return;
		}

		var dbitem = FMDB.Context.Tracks.Find(Id);
		if (dbitem is null)
		{
			return;
		}
		dbitem.ExertionLevel = (int)ExertionLevel;
		dbitem.TrackIntensityVisual = TrackIntensityVisual;
		dbitem.TrackIntensityOlfactory = TrackIntensityOlfactory;
		dbitem.RoomLayer = (int)RoomLayer;
		dbitem.TrackCircumstances = (int)TrackCircumstances;
		dbitem.CellId = Cell.Id;
		dbitem.CharacterId = _characterId;
		dbitem.MudDateTime = MudDateTime.GetDateTimeString();
		dbitem.FromMoveSpeedId = FromSpeed?.Id;
		dbitem.ToMoveSpeedId = ToSpeed?.Id;
		dbitem.FromDirectionExitId = FromExit?.Id;
		dbitem.ToDirectionExitId = ToExit?.Id;
		dbitem.BodyPrototypeId = BodyProtoType.Id;
		Changed = false;
	}

	/// <inheritdoc />
	public override object DatabaseInsert()
	{
		var dbitem = new Models.Track
		{
			ExertionLevel = (int)ExertionLevel,
			TrackIntensityVisual = TrackIntensityVisual,
			TrackIntensityOlfactory = TrackIntensityOlfactory,
			TurnedAround = TurnedAround,
			RoomLayer = (int)RoomLayer,
			TrackCircumstances = (int)TrackCircumstances,
			CellId = Cell.Id,
			CharacterId = _characterId,
			MudDateTime = MudDateTime.GetDateTimeString(),
			FromMoveSpeedId = FromSpeed?.Id,
			ToMoveSpeedId = ToSpeed?.Id,
			FromDirectionExitId = FromExit?.Id,
			ToDirectionExitId = ToExit?.Id,
			BodyPrototypeId = BodyProtoType.Id
		};
		FMDB.Context.Tracks.Add(dbitem);
		return dbitem;
	}

	/// <inheritdoc />
	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((MudSharp.Models.Track)dbitem).Id;
	}

	public static void CreateGlobalHeartbeatEvent()
	{
		var gameworld = Futuremud.Games.First();
		gameworld.HeartbeatManager.FuzzyHourHeartbeat += () =>
		{
			var delete = gameworld.Tracks.Count - gameworld.GetStaticInt("MaximumTrackCount");
			if (delete > 0)
			{
				var toDeleteTracks = gameworld.Tracks
				                              .OrderBy(x => x.MudDateTime)
				                              .Take(delete)
				                              .ToArray();
				var toDelete = toDeleteTracks
				                        .Select(x => x.Id)
				                        .ToHashSet();
				using (new FMDB())
				{
					FMDB.Context.Tracks
					    .Where(x => toDelete.Contains(x.Id))
					    .ExecuteDelete();
				}
				
				foreach (var track in toDeleteTracks)
				{
					track.Deleted = true;
					track.Cell.RemoveTrack(track);
					gameworld.Destroy(track);
					gameworld.SaveManager.Abort(track);
				}

				gameworld.DebugMessage($"Destroyed {delete.ToString("N0").ColourValue()} tracks to maintain target maximum count.");
			}
		};
	}

	private readonly long _characterId;
	private ICharacter? _character;

	/// <inheritdoc />
	public ICharacter Character => _character ??= Gameworld.TryGetCharacter(_characterId, true);

	private readonly long _bodyProtoTypeId;
	private IBodyPrototype? _bodyProtoType;

	/// <inheritdoc />
	public IBodyPrototype BodyProtoType => _bodyProtoType ??= Gameworld.BodyPrototypes.Get(_bodyProtoTypeId)!;

	private long _cellId;
	private ICell? _cell;

	/// <inheritdoc />
	public ICell Cell => (_cell ??= Gameworld.Cells.Get(_cellId))!;

	/// <inheritdoc />
	public RoomLayer RoomLayer { get; set; }

	private long? _fromExitId;
	private IExit? _fromExit;

	/// <inheritdoc />
	public IExit? FromExit 
	{
		get
		{
			if (_fromExit is null && _fromExitId is not null)
			{
				_fromExit = Gameworld.ExitManager.GetExitByID(_fromExitId.Value);
			}

			return _fromExit;
		}
		private init
		{
			_fromExit = value;
			_fromExitId = value?.Id;
		}
	}

	public ICellExit? FromCellExit => FromExit?.CellExitFor(Cell);

	private long? _toExitId;
	private IExit? _toExit;

	/// <inheritdoc />
	public IExit? ToExit
	{
		get
		{
			if (_toExit is null && _toExitId is not null)
			{
				_toExit = Gameworld.ExitManager.GetExitByID(_toExitId.Value);
			}

			return _toExit;
		}
		private init
		{
			_toExit = value;
			_toExitId = value?.Id;
		}
	}

	public ICellExit? ToCellExit => ToExit?.CellExitFor(Cell);

	private long? _fromSpeedId;
	private IMoveSpeed? _fromSpeed;

	/// <inheritdoc />
	public IMoveSpeed? FromSpeed => _fromSpeed ??= Gameworld.MoveSpeeds.Get(_fromSpeedId ?? 0);

	private long? _toSpeedId;
	private IMoveSpeed? _toSpeed;
	/// <inheritdoc />
	public IMoveSpeed? ToSpeed => _toSpeed ??= Gameworld.MoveSpeeds.Get(_toSpeedId ?? 0);

	/// <inheritdoc />
	public TrackCircumstances TrackCircumstances { get; set; }

	/// <inheritdoc />
	public ExertionLevel ExertionLevel { get; set; }

	private string _mudDateTimeText;
	private MudDateTime? _mudDateTime;

	/// <inheritdoc />
	public MudDateTime MudDateTime => _mudDateTime ??= new MudDateTime(_mudDateTimeText, Gameworld);

	/// <inheritdoc />
	public double TrackIntensityVisual { get; set; }
	public double TrackIntensityOlfactory { get; set; }
	public bool TurnedAround { get; set; }

	public bool Deleted { get; set; }

	public string DescribeForTracksCommand(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append(BodyProtoType.NameForTracking.A_An_RespectPlurals(true).ColourCharacter());
		var speed = FromSpeed ?? ToSpeed;
		sb.Append(" ");
		if (TrackCircumstances.HasFlag(TrackCircumstances.Dragged))
		{
			sb.Append("dragged ");
		}
		else
		{
			sb.Append(speed!.PresentParticiple);
			sb.Append(" ");
		}
		
		sb.Append((FromCellExit?.InboundMovementSuffix ?? ToCellExit?.OutboundMovementSuffix));
		if (TurnedAround)
		{
			sb.Append(" and looped back");
		}
		// TODO _ how to display intensity
		var since = Cell.DateTime() - MudDateTime;
		sb.Append(" ");
		sb.Append(Telnet.Green.Colour);
		sb.Append(since.Describe(actor));
		sb.Append(" ago");
		sb.Append(Telnet.RESETALL);
		return sb.ToString();
	}
}
