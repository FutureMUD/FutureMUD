using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Functions.DateTime;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate;
using MudSharp.Framework.Units;
using MudSharp.Vehicles;
using System.IO;
using System.Threading;

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
		_vehicleId = track.VehicleId;
        _mudDateTimeText = track.MudDateTime;
		RoutePositionMetres = track.RoutePosition.HasValue ? (double)track.RoutePosition.Value : null;
		RouteDirection = track.RouteDirection.HasValue
			? (RouteCellDirection)track.RouteDirection.Value
			: null;
		ValidateLoadedSpatialState(track);
    }

	private void ValidateLoadedSpatialState(Models.Track track)
	{
		var cell = Gameworld.Cells.Get(track.CellId) ??
			throw new InvalidDataException($"Track #{track.Id:N0} references missing Cell #{track.CellId:N0}.");
		var route = cell.RouteDefinition;
		if (route is null)
		{
			if (RoutePositionMetres.HasValue || RouteDirection.HasValue)
			{
				throw new InvalidDataException(
					$"Track #{track.Id:N0} has RouteCell position or direction data but Cell #{track.CellId:N0} is ordinary.");
			}

			return;
		}

		if (!RoutePositionMetres.HasValue || !double.IsFinite(RoutePositionMetres.Value) ||
			RoutePositionMetres.Value < 0.0 || RoutePositionMetres.Value > route.LengthMetres)
		{
			throw new InvalidDataException(
				$"Track #{track.Id:N0} has an invalid or missing coordinate in RouteCell #{track.CellId:N0}; valid coordinates are 0-{route.LengthMetres:N3}m.");
		}

		if (RouteDirection is not (RouteCellDirection.Negative or RouteCellDirection.Positive))
		{
			throw new InvalidDataException(
				$"Track #{track.Id:N0} has an invalid or missing longitudinal direction in RouteCell #{track.CellId:N0}.");
		}
	}

    public Track(IFuturemud gameworld, ICharacter who, ICellExit exit, TrackCircumstances circumstances, bool isLeaving, double visual, double olfactory)
    {
        Gameworld = gameworld;
        _character = who;
        _characterId = who.Id;
        _bodyProtoType = who.Body.Prototype;
		_bodyProtoTypeId = who.Body.Prototype.Id;
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

	public Track(
		IFuturemud gameworld,
		ICharacter who,
		double routePositionMetres,
		RouteCellDirection routeDirection,
		TrackCircumstances circumstances,
		double visual,
		double olfactory)
	{
		Gameworld = gameworld;
		_character = who;
		_characterId = who.Id;
		_bodyProtoType = who.Body.Prototype;
		_bodyProtoTypeId = who.Body.Prototype.Id;
		ExertionLevel = who.Body.CurrentExertion;
		TrackCircumstances = circumstances;
		_mudDateTime = who.Location.DateTime();
		RoomLayer = who.RoomLayer;
		TrackIntensityOlfactory = olfactory;
		TrackIntensityVisual = visual;
		_cell = who.Location;
		RoutePositionMetres = routePositionMetres;
		RouteDirection = routeDirection;
		_toSpeed = who.CurrentSpeed;

		gameworld.SaveManager.AddInitialisation(this);
	}

	public Track(
		IFuturemud gameworld,
		IVehicle vehicle,
		double routePositionMetres,
		RouteCellDirection routeDirection,
		double visual,
		double olfactory)
	{
		Gameworld = gameworld;
		_vehicle = vehicle;
		_vehicleId = vehicle.Id;
		ExertionLevel = ExertionLevel.Normal;
		TrackCircumstances = TrackCircumstances.None;
		_mudDateTime = vehicle.Location.DateTime();
		RoomLayer = vehicle.RoomLayer;
		TrackIntensityOlfactory = olfactory;
		TrackIntensityVisual = visual;
		_cell = vehicle.Location;
		RoutePositionMetres = routePositionMetres;
		RouteDirection = routeDirection;

		gameworld.SaveManager.AddInitialisation(this);
	}

    /// <inheritdoc />
    public override void Save()
    {
        Changed = false;
        if (Deleted)
        {
            return;
        }

        Models.Track? dbitem = FMDB.Context.Tracks.Find(Id);
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
		dbitem.VehicleId = _vehicleId;
        dbitem.MudDateTime = MudDateTime.GetDateTimeString();
        dbitem.FromMoveSpeedId = FromSpeed?.Id;
        dbitem.ToMoveSpeedId = ToSpeed?.Id;
        dbitem.FromDirectionExitId = FromExit?.Id;
        dbitem.ToDirectionExitId = ToExit?.Id;
		dbitem.BodyPrototypeId = _bodyProtoTypeId ?? BodyProtoType?.Id;
		dbitem.RoutePosition = RoutePositionMetres.HasValue
			? Math.Round((decimal)RoutePositionMetres.Value, 3, MidpointRounding.AwayFromZero)
			: null;
		dbitem.RouteDirection = RouteDirection.HasValue ? (int)RouteDirection.Value : null;
    }

    /// <inheritdoc />
    public override object DatabaseInsert()
    {
        Models.Track dbitem = new()
        {
            ExertionLevel = (int)ExertionLevel,
            TrackIntensityVisual = TrackIntensityVisual,
            TrackIntensityOlfactory = TrackIntensityOlfactory,
            TurnedAround = TurnedAround,
            RoomLayer = (int)RoomLayer,
            TrackCircumstances = (int)TrackCircumstances,
            CellId = Cell.Id,
            CharacterId = _characterId,
			VehicleId = _vehicleId,
            MudDateTime = MudDateTime.GetDateTimeString(),
            FromMoveSpeedId = FromSpeed?.Id,
            ToMoveSpeedId = ToSpeed?.Id,
            FromDirectionExitId = FromExit?.Id,
            ToDirectionExitId = ToExit?.Id,
			BodyPrototypeId = _bodyProtoTypeId ?? BodyProtoType?.Id,
			RoutePosition = RoutePositionMetres.HasValue
				? Math.Round((decimal)RoutePositionMetres.Value, 3, MidpointRounding.AwayFromZero)
				: null,
			RouteDirection = RouteDirection.HasValue ? (int)RouteDirection.Value : null
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
        IFuturemud gameworld = Futuremud.Games.First();
        gameworld.HeartbeatManager.FuzzyHourHeartbeat += () =>
        {
            int delete = gameworld.Tracks.Count - gameworld.GetStaticInt("MaximumTrackCount");
            if (delete > 0)
            {
                ITrack[] toDeleteTracks = gameworld.Tracks
                                              .OrderBy(x => x.MudDateTime)
                                              .Take(delete)
                                              .ToArray();
                HashSet<long> toDelete = toDeleteTracks
                                        .Select(x => x.Id)
                                        .ToHashSet();
                using (new FMDB())
                {
                    FMDB.Context.Tracks
                        .Where(x => toDelete.Contains(x.Id))
                        .ExecuteDelete();
                }

                foreach (ITrack? track in toDeleteTracks)
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

	private readonly long? _characterId;
    private ICharacter? _character;

    /// <inheritdoc />
	public ICharacter? Character => _character ??= _characterId.HasValue
		? Gameworld.TryGetCharacter(_characterId.Value, true)
		: null;

	private readonly long? _bodyProtoTypeId;
    private IBodyPrototype? _bodyProtoType;

    /// <inheritdoc />
	public IBodyPrototype? BodyProtoType => _bodyProtoType ??= _bodyProtoTypeId.HasValue
		? Gameworld.BodyPrototypes.Get(_bodyProtoTypeId.Value)
		: null;

	private readonly long? _vehicleId;
	private IVehicle? _vehicle;

	/// <inheritdoc />
	public IVehicle? Vehicle => _vehicle ??= _vehicleId.HasValue
		? Gameworld.Vehicles.Get(_vehicleId.Value)
		: null;

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

    private string _mudDateTimeText = null!;
    private MudDateTime? _mudDateTime;

    /// <inheritdoc />
    public MudDateTime MudDateTime => _mudDateTime ??= MudDateTime.FromStoredStringOrFallback(_mudDateTimeText,
        Gameworld, StoredMudDateTimeFallback.CurrentDateTime, "Track", Id, Name, "MudDateTime");

    /// <inheritdoc />
    public double TrackIntensityVisual { get; set; }
    public double TrackIntensityOlfactory { get; set; }
    public bool TurnedAround { get; set; }
	public double? RoutePositionMetres { get; private set; }
	public RouteCellDirection? RouteDirection { get; private set; }

    public Difficulty VisualTrackDifficulty(ICharacter actor)
    {
        double ability = actor.TrackingAbilityVisual;
        if (ability <= 0.0)
        {
            return Difficulty.Impossible;
        }

        return (ability / TrackIntensityVisual) switch
        {
            < 0.1 => Difficulty.Insane,
            < 0.3 => Difficulty.ExtremelyHard,
            < 0.5 => Difficulty.VeryHard,
            < 0.8 => Difficulty.Hard,
            < 1.2 => Difficulty.Normal,
            < 1.5 => Difficulty.Easy,
            < 2.0 => Difficulty.VeryEasy,
            < 3.0 => Difficulty.ExtremelyEasy,
            < 5.0 => Difficulty.Trivial,
            _ => Difficulty.Automatic
        };
    }

    public Difficulty OlfactoryTrackDifficulty(ICharacter actor)
    {
        double ability = actor.TrackingAbilityOlfactory;
        if (ability <= 0.0)
        {
            return Difficulty.Impossible;
        }

        return (ability / TrackIntensityOlfactory) switch
        {
            < 0.1 => Difficulty.Insane,
            < 0.3 => Difficulty.ExtremelyHard,
            < 0.5 => Difficulty.VeryHard,
            < 0.8 => Difficulty.Hard,
            < 1.2 => Difficulty.Normal,
            < 1.5 => Difficulty.Easy,
            < 2.0 => Difficulty.VeryEasy,
            < 3.0 => Difficulty.ExtremelyEasy,
            < 5.0 => Difficulty.Trivial,
            _ => Difficulty.Automatic
        };
    }

    public bool Deleted { get; set; }

    public string DescribeForTracksCommand(ICharacter actor)
    {
        StringBuilder sb = new();
		var vehicle = Vehicle;
		if (vehicle is not null)
		{
			sb.Append($"vehicle tracks from {vehicle.Name.ColourName()}");
		}
		else if (BodyProtoType is not null)
		{
			sb.Append(BodyProtoType.NameForTracking.A_An_RespectPlurals(true).ColourCharacter());
		}
		else
		{
			sb.Append("unidentifiable tracks".ColourCharacter());
		}
        IMoveSpeed? speed = FromSpeed ?? ToSpeed;
        sb.Append(" ");
        if (TrackCircumstances.HasFlag(TrackCircumstances.Dragged))
        {
            sb.Append("dragged ");
        }
        else if (speed is not null)
        {
            sb.Append(speed.PresentParticiple);
            sb.Append(" ");
        }
		else if (vehicle is not null)
		{
			sb.Append("travelled ");
		}

		if (RoutePositionMetres.HasValue && RouteDirection.HasValue)
		{
			var route = Cell.RouteDefinition;
			var directionName = RouteDirection == RouteCellDirection.Positive
				? route?.PositiveDirectionName ?? "forward"
				: route?.NegativeDirectionName ?? "backward";
			var distance = actor.Gameworld.UnitManager.DescribeMostSignificantExact(
				RoutePositionMetres.Value / actor.Gameworld.UnitManager.BaseHeightToMetres,
				UnitType.Length,
				actor);
			sb.Append($"{directionName} past {distance}");
		}
		else
		{
			sb.Append(FromCellExit?.InboundMovementSuffix ?? ToCellExit?.OutboundMovementSuffix);
		}
        if (TurnedAround)
        {
            sb.Append(" and looped back");
        }
        TimeSpan since = Cell.DateTime() - MudDateTime;
        sb.Append(" ");
        sb.Append(Telnet.Green.Colour);
        sb.Append(since.Describe(actor));
        sb.Append(" ago");
        sb.Append(Telnet.RESETALL);
        sb.Append(" (");
        sb.Append(TrackIntensityVisual.ToStringP2Colour(actor));
        sb.Append(" V / ");
        sb.Append(TrackIntensityOlfactory.ToStringP2Colour(actor));
        sb.Append(" O)");
        if (TrackCircumstances.HasFlag(TrackCircumstances.MagicallyMarked))
        {
            sb.Append(" ");
            sb.Append("[magically marked]".Colour(Telnet.BoldCyan));
        }
        return sb.ToString();
    }
}
