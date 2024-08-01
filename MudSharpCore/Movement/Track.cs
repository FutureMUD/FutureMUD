using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
		BodyProtoType = Gameworld.BodyPrototypes.Get(track.BodyPrototypeId)!;
		Cell = Gameworld.Cells.Get(track.CellId)!;
		RoomLayer = (RoomLayer)track.RoomLayer;
		FromExit = Gameworld.ExitManager.GetExitByID(track.FromDirectionExitId ?? 0);
		ToExit = Gameworld.ExitManager.GetExitByID(track.ToDirectionExitId ?? 0);
		FromSpeed = Gameworld.MoveSpeeds.Get(track.FromMoveSpeedId ?? 0);
		ToSpeed = Gameworld.MoveSpeeds.Get(track.ToMoveSpeedId ?? 0);
		TrackCircumstances = (TrackCircumstances)track.TrackCircumstances;
		ExertionLevel = (ExertionLevel)track.ExertionLevel;
		TrackIntensityVisual = track.TrackIntensityVisual;
		TrackIntensityOlfactory = track.TrackIntensityOlfactory;
		TurnedAround = track.TurnedAround;
		_characterId = track.CharacterId;
		MudDateTime = new MudDateTime(track.MudDateTime, Gameworld);
	}

	public Track(IFuturemud gameworld, ICharacter who, ICellExit exit, TrackCircumstances circumstances, bool isLeaving)
	{
		Gameworld = gameworld;
		_character = who;
		_characterId = who.Id;
		BodyProtoType = who.Body.Prototype;
		ExertionLevel = who.Body.CurrentExertion;
		TrackCircumstances = circumstances;
		MudDateTime = who.Location.DateTime();
		RoomLayer = who.RoomLayer;
		if (who.Location.IsSwimmingLayer(RoomLayer) || RoomLayer.In(RoomLayer.InAir, RoomLayer.HighInAir))
		{
			TrackIntensityVisual = 0.0;
		}
		else
		{
			TrackIntensityVisual = 1.0 * who.Location.Terrain(who).TrackIntensityMultiplierVisual * who.Race.TrackIntensityVisual;
		}

		TrackIntensityOlfactory = 1.0 * who.Location.Terrain(who).TrackIntensityMultiplierOlfactory * who.Race.TrackIntensityOlfactory;
		Cell = exit.Origin;
		if (isLeaving)
		{
			ToExit = exit.Exit;
			ToSpeed = who.CurrentSpeed;
		}
		else
		{
			FromExit = exit.Exit;
			FromSpeed = who.CurrentSpeed;
		}

		gameworld.SaveManager.AddInitialisation(this);
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.Tracks.Find(Id);
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

	private readonly long _characterId;
	private ICharacter? _character;

	/// <inheritdoc />
	public ICharacter Character => _character ??= Gameworld.TryGetCharacter(_characterId, true);

	/// <inheritdoc />
	public IBodyPrototype BodyProtoType { get; set; }

	/// <inheritdoc />
	public ICell Cell { get; set; }

	/// <inheritdoc />
	public RoomLayer RoomLayer { get; set; }

	/// <inheritdoc />
	public IExit? FromExit { get; set; }

	public ICellExit? FromCellExit => FromExit?.CellExitFor(Cell);

	/// <inheritdoc />
	public IExit? ToExit { get; set; }

	public ICellExit? ToCellExit => ToExit?.CellExitFor(Cell);

	/// <inheritdoc />
	public IMoveSpeed? FromSpeed { get; set; }

	/// <inheritdoc />
	public IMoveSpeed? ToSpeed { get; set; }

	/// <inheritdoc />
	public TrackCircumstances TrackCircumstances { get; set; }

	/// <inheritdoc />
	public ExertionLevel ExertionLevel { get; set; }

	/// <inheritdoc />
	public MudDateTime MudDateTime { get; set; }

	/// <inheritdoc />
	public double TrackIntensityVisual { get; set; }
	public double TrackIntensityOlfactory { get; set; }
	public bool TurnedAround { get; set; }

	public string DescribeForTracksCommand(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append(BodyProtoType.Name.A_An_RespectPlurals(true).ColourCharacter());
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
