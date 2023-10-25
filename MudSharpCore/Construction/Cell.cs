using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Community.Boards;
using MudSharp.Construction.Boundary;
using MudSharp.Construction.Grids;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using ExpressionEngine;
using System.Xml.Linq;
using MudSharp.Models;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character.Name;
using MudSharp.Framework.Revision;
using MudSharp.Work.Foraging;
using MudSharp.Work.Projects;

namespace MudSharp.Construction;

public partial class Cell : Location, IDisposable, ICell
{
	private readonly List<IRangedCover> _localCover = new();

	private bool _contentsChanged;

	protected List<IEditableCellOverlay> _overlays = new();

	private bool _yieldsChanged;

	public Cell(ICellOverlayPackage package, IRoom room, bool temporary = false) : base(room.Gameworld)
	{
		Room = room;
		using (new FMDB())
		{
			var dbCell = new Models.Cell
			{
				RoomId = room.Id,
				Temporary = temporary,
				EffectData = SaveEffects().ToString()
			};
			FMDB.Context.Cells.Add(dbCell);
			FMDB.Context.SaveChanges();
			_id = dbCell.Id;
			var newOverlay = new CellOverlay(this, package);
			dbCell.CurrentOverlayId = newOverlay.Id;
			FMDB.Context.SaveChanges();
			SetupCell(dbCell);

			var hooks = Gameworld.DefaultHooks.Where(x => x.Applies(this, "Cell")).Select(x => x.Hook).ToList();
			if (hooks.Any())
			{
				FMDB.Connection.Execute(
					$"INSERT INTO Hooks_Perceivables (PerceivableId, PerceivableType, HookId) VALUES {hooks.Select(x => $"({Id:N0}, 'Cell', {x.Id:N0})").ListToString(conjunction: "", twoItemJoiner: ", ")}");

				foreach (var hook in hooks)
				{
					InstallHook(hook);
				}
			}
		}

		Gameworld.Add(this);
	}

	public Cell(ICellOverlayPackage package, IRoom room, ICell templateCell, bool temporary = false) : base(
		room.Gameworld)
	{
		Room = room;
		using (new FMDB())
		{
			var dbCell = new Models.Cell
			{
				RoomId = room.Id,
				Temporary = temporary,
				EffectData = SaveEffects().ToString()
			};
			FMDB.Context.Cells.Add(dbCell);
			FMDB.Context.SaveChanges();
			_id = dbCell.Id;
			var newOverlay = new CellOverlay(this, package, templateCell);
			dbCell.CurrentOverlayId = newOverlay.Id;
			FMDB.Context.SaveChanges();
			SetupCell(dbCell);

			var hooks = Gameworld.DefaultHooks.Where(x => x.Applies(this, "Cell")).Select(x => x.Hook).ToList();
			if (hooks.Any())
			{
				FMDB.Connection.Execute(
					$"INSERT INTO Hooks_Perceivables (PerceivableId, PerceivableType, HookId) VALUES {hooks.Select(x => $"({Id:N0}, 'Cell', {x.Id:N0})").ListToString(conjunction: "", twoItemJoiner: ", ")}");

				foreach (var hook in hooks)
				{
					InstallHook(hook);
				}
			}
		}

		Gameworld.Add(this);
	}

	public Cell(MudSharp.Models.Cell cell, IRoom room) : base(room.Gameworld)
	{
		Room = room;
		SetupCell(cell);
	}

	public bool ContentsChanged
	{
		get => _contentsChanged;
		set
		{
			if (_noSave && value)
			{
				return;
			}

			if (value)
			{
				Changed = true;
			}

			_contentsChanged = value;
		}
	}

	public bool YieldsChanged
	{
		get => _yieldsChanged;
		set
		{
			if (_noSave && value)
			{
				return;
			}

			if (value)
			{
				Changed = true;
			}

			_yieldsChanged = value;
		}
	}

	public int? X => Room?.X;

	public int? Y => Room?.Y;

	public int? Z => Room?.Z;

	/// <summary>
	/// If a cell is temporary, it may disappear at any time.
	/// </summary>
	public bool Temporary { get; set; }

	protected List<IMovement> Movements { get; set; }
	public override string FrameworkItemType => "Cell";

	public override ICell Location
	{
		get => this;

		protected set { }
	}

	public bool IsSwimmingLayer(RoomLayer referenceLayer = RoomLayer.GroundLevel)
	{
		// TODO - wading pools
		// TODO - room flooding effects
		return !referenceLayer.IsHigherThan(RoomLayer.GroundLevel) &&
		       Terrain(null).TerrainLayers.Any(x => x.IsUnderwater());
	}

	public bool IsUnderwaterLayer(RoomLayer referenceLayer)
	{
		// TODO - room flooding effects
		return referenceLayer.IsLowerThan(RoomLayer.GroundLevel) &&
		       Terrain(null).TerrainLayers.Any(x => x.IsUnderwater());
	}

	public override void Insert(IGameItem thing, bool newStack)
	{
		if (thing == null || _gameItems.Contains(thing))
		{
#if DEBUG
			if (_gameItems.Contains(thing))
			{
				throw new ApplicationException("Item duplication in Cell.");
			}
#endif
			return;
		}

		var newLayer = HandleEnterLayers(thing);

		if (!newStack)
		{
			var mergeTarget = LayerGameItems(newLayer).FirstOrDefault(thing.CanMerge);
			if (mergeTarget != null)
			{
				mergeTarget.Merge(thing);
				thing.Delete();
				return;
			}
		}


		thing.ContainedIn = null;
		thing.MoveTo(this, newLayer);
		base.Insert(thing, newStack);
		Room.Insert(thing, newStack);
		_gameItems = _gameItems.OrderBy(x => !x.HighPriority).ToList();
		if (IsSwimmingLayer(newLayer))
		{
			thing.PositionState = PositionFloatingInWater.Instance;
		}

		ContentsChanged = true;
		CheckFallExitStatus();
	}

	private RoomLayer HandleEnterLayers(IGameItem thing)
	{
		var localLayers = Terrain(thing).TerrainLayers.ToList();
		if (localLayers.Contains(thing.RoomLayer))
		{
			return thing.RoomLayer;
		}

		var highest = localLayers.HighestLayer();
		var lowest = localLayers.LowestLayer();
		if (thing.RoomLayer.IsHigherThan(highest))
		{
			return highest;
		}

		return lowest;
	}

	public IEnumerable<ICell> Surrounds
	{
		get { return Gameworld.ExitManager.GetExitsFor(this).Select(x => x.Destination); }
	}

	public override string Name => CurrentOverlay.CellName;

	public override void Extract(IGameItem thing)
	{
		if (thing == null || !_gameItems.Contains(thing))
		{
			return;
		}

		base.Extract(thing);
		Room.Extract(thing);
		ContentsChanged = true;
		CheckFallExitStatus();
	}

	public IRoom Room { get; }

	public IZone Zone => Room.Zone;
	public IShard Shard => Room.Zone.Shard;

	public IEnumerable<IArea> Areas => Room.Areas;

	public IHearingProfile HearingProfile(IPerceiver voyeur)
	{
		return GetOverlayFor(voyeur).HearingProfile;
	}

	public int LoadItems(IEnumerable<Models.GameItem> items)
	{
		var stagingTable = new List<Tuple<Models.GameItem, IGameItem>>();
		foreach (var item in items)
		{
			var gitem = Gameworld.TryGetItem(item, true);
			if (gitem == null)
			{
				continue;
			}

			if (gitem.InInventoryOf != null || gitem.ContainedIn != null || gitem.Location != null)
			{
				Changed = true;
				Gameworld.SystemMessage(
					$"Duplicated Item: {gitem.HowSeen(gitem, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {gitem.Id.ToString("N0")}",
					true);
				continue;
			}

			gitem.Drop(this);
			stagingTable.Add(new Tuple<Models.GameItem, IGameItem>(item, gitem));
			_gameItems.Add(gitem);
			Room.Insert(gitem);
		}

		foreach (var item in stagingTable)
		{
			item.Item2.LoadPosition(item.Item1);
			item.Item2.FinaliseLoadTimeTasks();
		}

		_gameItems = _gameItems.OrderBy(x => !x.HighPriority).ToList();
		return _gameItems.Count;
	}

	public IEnumerable<ICellExit> ExitsFor(IPerceiver voyeur, bool ignoreLayers = false)
	{
		return Gameworld.ExitManager.GetExitsFor(this, GetOverlayFor(voyeur),
			ignoreLayers ? default : voyeur?.RoomLayer);
	}

	public ICellExit GetExit(CardinalDirection direction, IPerceiver voyeur)
	{
		return Gameworld.ExitManager.GetExit(this, direction, voyeur);
	}

	public ICellExit GetExit(string direction, string target, IPerceiver voyeur)
	{
		return Gameworld.ExitManager.GetExit(this, direction, target, voyeur, GetOverlayFor(voyeur));
	}

	public ICellExit GetExitKeyword(string direction, IPerceiver voyeur)
	{
		return Gameworld.ExitManager.GetExitKeyword(this, direction, voyeur, GetOverlayFor(voyeur));
	}

	public ICellExit GetExitTo(ICell otherCell, IPerceiver voyeur, bool ignoreLayers = false)
	{
		return
			Gameworld.ExitManager.GetExitsFor(this, GetOverlayFor(voyeur), ignoreLayers ? default : voyeur?.RoomLayer)
			         .FirstOrDefault(x => x.Destination == otherCell);
	}

	public ITerrain Terrain(IPerceiver voyeur)
	{
		return GetOverlayFor(voyeur).Terrain;
	}

	public CellOutdoorsType OutdoorsType(IPerceiver voyeur)
	{
		return GetOverlayFor(voyeur).OutdoorsType;
	}

	public PrecipitationLevel HighestRecentPrecipitationLevel(IPerceiver voyeur)
	{
		var overlay = GetOverlayFor(voyeur);
		if (overlay.Terrain.OverrideWeatherController != null)
		{
			return overlay.Terrain.OverrideWeatherController.HighestRecentPrecipitationLevel;
		}

		return Areas.FirstOrDefault(x => x.Weather != null)?.Weather.HighestRecentPrecipitationLevel ??
		       Zone.Weather?.HighestRecentPrecipitationLevel ??
		       PrecipitationLevel.Parched;
	}

	public IWeatherController WeatherController =>
		CurrentOverlay.Terrain.OverrideWeatherController ??
		Areas.FirstOrDefault(x => x.Weather != null)?.Weather ??
		Zone.Weather;

	public IWeatherEvent CurrentWeather(IPerceiver voyeur)
	{
		var overlay = GetOverlayFor(voyeur);
		if (overlay.Terrain.OverrideWeatherController != null)
		{
			return overlay.Terrain.OverrideWeatherController.CurrentWeatherEvent;
		}

		return Areas.FirstOrDefault(x => x.Weather != null)?.Weather.CurrentWeatherEvent ??
		       Zone.Weather?.CurrentWeatherEvent;
	}

	public ISeason CurrentSeason(IPerceiver voyeur)
	{
		var overlay = GetOverlayFor(voyeur);
		if (overlay.Terrain.OverrideWeatherController != null)
		{
			return overlay.Terrain.OverrideWeatherController.CurrentSeason;
		}

		return Areas.FirstOrDefault(x => x.Weather != null)?.Weather.CurrentSeason ??
		       Zone.Weather?.CurrentSeason;
	}

	public double CurrentTemperature(IPerceiver voyeur)
	{
		var baseTemperature = 0.0;
		var overlay = GetOverlayFor(voyeur);
		if (overlay.Terrain.OverrideWeatherController != null)
		{
			baseTemperature = overlay.Terrain.OverrideWeatherController.CurrentTemperature;
		}
		else
		{
			var weather = Areas.FirstOrDefault(x => x.Weather != null)?.Weather ??
			              Zone.Weather;

			if (weather == null)
			{
				baseTemperature = Gameworld.GetStaticDouble("DefaultCellTemperature");
			}
			else
			{
				switch (overlay.OutdoorsType)
				{
					case CellOutdoorsType.Indoors:
					case CellOutdoorsType.IndoorsWithWindows:
					case CellOutdoorsType.IndoorsNoLight:
						// Indoors is sheltered from rain and wind unless there is an open exit to an outdoors room
						if (Gameworld.ExitManager.GetExitsFor(this, overlay, voyeur?.RoomLayer).Any(x =>
							    x.Exit.Door?.IsOpen != false && x.Destination.OutdoorsType(voyeur)
							                                     .In(CellOutdoorsType.IndoorsClimateExposed,
								                                     CellOutdoorsType.Outdoors)))
						{
							baseTemperature = weather.CurrentTemperature +
							                  weather.CurrentWeatherEvent.PrecipitationTemperatureEffect;
							break;
						}

						baseTemperature = weather.CurrentTemperature +
						                  weather.CurrentWeatherEvent.PrecipitationTemperatureEffect +
						                  weather.CurrentWeatherEvent.WindTemperatureEffect;
						break;
					case CellOutdoorsType.IndoorsClimateExposed:
						// This kind of location only protects from the rain, not the wind
						baseTemperature = weather.CurrentTemperature +
						                  weather.CurrentWeatherEvent.PrecipitationTemperatureEffect;
						break;
					default:
						baseTemperature = weather.CurrentTemperature;
						break;
				}
			}
		}

		var effectTemperature = EffectsOfType<IAffectEnvironmentalTemperature>()
		                        .Concat(Zone.EffectsOfType<IAffectEnvironmentalTemperature>())
		                        .Where(x => x.Applies())
		                        .Select(x => x.TemperatureDelta)
		                        .DefaultIfEmpty(0)
		                        .Sum();

		return baseTemperature + effectTemperature;
	}

	public double CurrentIllumination(IPerceiver voyeur)
	{
		if (voyeur is null)
		{
			voyeur = new DummyPerceiver();
		}

		var overlay = GetOverlayFor(voyeur);
		var environmentalLight = (Room.Zone.CurrentLightLevel * overlay.AmbientLightFactor + overlay.AddedLight) *
		                         (CurrentWeather(voyeur)?.LightLevelMultiplier ?? 1.0);
		var ambientLight = environmentalLight +
		                   LayerCharacters(voyeur.RoomLayer).Sum(x => x.IlluminationProvided) +
		                   LayerGameItems(voyeur.RoomLayer).Sum(x => x.IlluminationProvided) +
		                   EffectsOfType<IAreaLightEffect>(x => x.Applies()).Sum(x => x.AddedLight);
		return ambientLight;
	}

	public Difficulty SpotDifficulty(IPerceiver spotter)
	{
		var spotDifficultyWeather =
			CurrentWeather(spotter)?.Precipitation.MinimumSightDifficulty() ?? Difficulty.Automatic;
		var spotDifficultyLight = Gameworld.LightModel.GetSightDifficulty(CurrentIllumination(spotter));
		var spotDifficultyTerrain = Terrain(spotter).SpotDifficulty;
		return spotDifficultyLight.Highest(spotDifficultyWeather, spotDifficultyTerrain);
	}

	public ICellOverlay CurrentOverlay { get; protected set; }

	public bool SetCurrentOverlay(ICellOverlayPackage package)
	{
		var overlay = Overlays.FirstOrDefault(x => x.Package == package);
		if (overlay == null)
		{
			return false;
		}

		CurrentOverlay = overlay;
		Changed = true;
		return true;
	}

	public IEditableCellOverlay GetOrCreateOverlay(ICellOverlayPackage package)
	{
		var overlay = _overlays.FirstOrDefault(x => x.Package == package);
		if (overlay == null)
		{
			overlay = CurrentOverlay.CreateClone(package);
			_overlays.Add(overlay);
			Changed = true;
		}

		return overlay;
	}

	public void AddOverlay(IEditableCellOverlay overlay)
	{
		_overlays.Add(overlay);
	}

	public void RemoveOverlay(long id)
	{
		var overlay = _overlays.Find(x => x.Id == id);
		if (overlay is null)
		{
			return;
		}

		_overlays.Remove(overlay);
		if (CurrentOverlay == overlay)
		{
			CurrentOverlay = _overlays.FirstOrDefault(x => x.Package.Status == RevisionStatus.Current);
			Changed = true;
		}
	}

	public ICellOverlay GetOverlay(ICellOverlayPackage package)
	{
		return _overlays.FirstOrDefault(x => x.Package == package);
	}

	public IEnumerable<ICellOverlay> Overlays => _overlays;

	public void Login(ICharacter loginCharacter)
	{
		loginCharacter.State &= ~CharacterState.Stasis;
		loginCharacter.LastMinutesUpdate = System.DateTime.UtcNow;
		loginCharacter.LoginDateTime = System.DateTime.UtcNow;
		loginCharacter.OutputHandler?.Register(loginCharacter);
		Enter(loginCharacter, noSave: true, roomLayer: loginCharacter.RoomLayer);
		if (loginCharacter is NPC.NPC npc)
		{
			npc.SetupEventSubscriptions();
		}
		else
		{
			if (loginCharacter.IsGuest)
			{
				Gameworld.SystemMessage($"Account {loginCharacter.Account.Name} has entered the guest lounge.", true);
			}
			else
			{
				if (loginCharacter.PreviousLoginDateTime != null)
				{
					loginCharacter.Body.DoOfflineHealing(loginCharacter.LoginDateTime -
					                                     (loginCharacter.LastLogoutDateTime ??
					                                      loginCharacter.LoginDateTime));
				}

				if (loginCharacter.IsAdministrator() && Gameworld.Boards.Any(x => x.DisplayOnLogin))
				{
					var counts = new Dictionary<IBoard, int>();
					foreach (var board in Gameworld.Boards.Where(x => x.DisplayOnLogin))
					{
						counts[board] =
							board.Posts.Count(x =>
								x.PostTime > (loginCharacter.PreviousLoginDateTime ?? System.DateTime.MinValue));
					}

					if (counts.Any(x => x.Value > 0))
					{
						var sb = new StringBuilder();
						sb.AppendLine();
						sb.AppendLine("The following boards have new posts since your last login:");
						foreach (var count in counts.Where(x => x.Value > 0))
						{
							sb.AppendLine(
								$"\t{count.Key.Name.Colour(Telnet.Green)} - {count.Value:N0} new post{(count.Value == 1 ? "" : "s")}.");
						}

						sb.AppendLine();
						loginCharacter.Send(sb.ToString());
					}
				}

				Gameworld.SystemMessage(
					new EmoteOutput(
						new Emote(
							$"@ ({loginCharacter.PersonalName.GetName(NameStyle.FullName)}) has logged in.",
							loginCharacter), flags: OutputFlags.SuppressSource), true);
			}
		}

		loginCharacter.LoginCharacter();
	}

	public override void Enter(ICharacter movingCharacter, ICellExit exit = null, bool noSave = false,
		RoomLayer roomLayer = RoomLayer.GroundLevel)
	{
		base.Enter(movingCharacter, exit);
		movingCharacter.MoveTo(this, roomLayer, exit, noSave);
		Room.Enter(movingCharacter, exit);
		DoEnterEvent(movingCharacter);

		if (exit != null && exit.InboundDirection != CardinalDirection.Unknown)
		{
			movingCharacter.AddEffect(new AdjacentToExit(movingCharacter, exit.Exit.CellExitFor(this)),
				AdjacentToExit.DefaultEffectTimeSpan);
		}

		movingCharacter.HandleEvent(EventType.CharacterEnterCell, movingCharacter, this,
			movingCharacter.Movement?.Exit);
		foreach (var witness in EventHandlers.Except(movingCharacter))
		{
			witness.HandleEvent(EventType.CharacterEnterCellWitness, movingCharacter, this,
				movingCharacter.Movement?.Exit, witness);
		}

		foreach (var witness in movingCharacter.Body.ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterEnterCellWitness, movingCharacter, this,
				movingCharacter.Movement?.Exit, witness);
		}

		CheckFallExitStatus();
	}

	private RoomLayer HandleEnterLayers(ICharacter movingCharacter)
	{
		var localLayers = Terrain(movingCharacter).TerrainLayers.ToList();
		if (localLayers.Contains(movingCharacter.RoomLayer))
		{
			return movingCharacter.RoomLayer;
		}

		var highest = localLayers.HighestLayer();
		var lowest = localLayers.LowestLayer();
		if (movingCharacter.RoomLayer.IsHigherThan(highest))
		{
			return highest;
		}

		return lowest;
	}

	public override void Leave(ICharacter movingCharacter)
	{
		base.Leave(movingCharacter);
		Room.Leave(movingCharacter);
		DoLeaveEvent(movingCharacter);
		movingCharacter.HandleEvent(EventType.CharacterLeaveCell, movingCharacter, this,
			movingCharacter.Movement?.Exit);
		foreach (var witness in EventHandlers.Except(movingCharacter))
		{
			witness.HandleEvent(EventType.CharacterLeaveCellWitness, movingCharacter, this,
				movingCharacter.Movement?.Exit, witness);
		}

		foreach (var witness in movingCharacter.Body.ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterLeaveCellWitness, movingCharacter, this,
				movingCharacter.Movement?.Exit, witness);
		}

		CheckFallExitStatus();
	}

	public IFluid Atmosphere => CurrentOverlay.Atmosphere;

	public override IEnumerable<ICalendar> Calendars => Room.Calendars;

	public override IEnumerable<IClock> Clocks => Room.Clocks;

	public IPermanentShop Shop { get; set; }

	public override IMudTimeZone TimeZone(IClock whichClock)
	{
		return Room.TimeZone(whichClock);
	}

	public override IEnumerable<ICelestialObject> Celestials => Room.Celestials;
	public IEnumerable<IRangedCover> LocalCover => _localCover;

	public IEnumerable<IRangedCover> GetCoverFor(IPerceiver voyeur)
	{
		return Terrain(voyeur).TerrainCovers.Concat(LocalCover).Distinct().ToList();
	}

	public bool IsExitVisible(IPerceiver voyeur, ICellExit exit, PerceptionTypes type,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return
			EffectHandler.EffectsOfType<IExitHiddenEffect>()
			             .Where(x => x.Exit == exit.Exit && x.Applies(voyeur))
			             .Select(x => x.HiddenTypes)
			             .DefaultIfEmpty(PerceptionTypes.None)
			             .Aggregate(type, (prev, effect) => prev & ~effect) != PerceptionTypes.None;
	}

	public override void Register(IOutputHandler handler)
	{
		throw new NotImplementedException();
	}

	public override CelestialInformation GetInfo(ICelestialObject celestial)
	{
		return Room.GetInfo(celestial);
	}

	public double EstimatedDirectDistanceTo(ICell otherCell)
	{
		return Math.Sqrt(Math.Pow(Room.X - otherCell.Room.X, 2) + Math.Pow(Room.Y - otherCell.Room.Y, 2) +
		                 Math.Pow(Room.Z - otherCell.Room.Z, 2));
	}

	public TimeOfDay CurrentTimeOfDay => Zone.CurrentTimeOfDay;

	public override MudDate Date(ICalendar whichCalendar)
	{
		return Room.Date(whichCalendar);
	}

	public override MudTime Time(IClock whichClock)
	{
		return Room.Time(whichClock);
	}

	public void RegisterMovement(IMovement move)
	{
		Movements.Add(move);
		if (Equals(move.Exit.Destination, this))
		{
			foreach (var member in move.CharacterMovers)
			{
				member.HandleEvent(EventType.CharacterEnterCell, member, this, move.Exit);
				foreach (var witness in EventHandlers.Except(member))
				{
					witness.HandleEvent(EventType.CharacterEnterCellWitness, member, this, move.Exit, witness);
				}

				foreach (var witness in member.Body.ExternalItems)
				{
					witness.HandleEvent(EventType.CharacterEnterCellWitness, member, this, move.Exit, witness);
				}
			}
		}
		else if (Equals(move.Exit.Origin, this))
		{
			foreach (var member in move.CharacterMovers)
			{
				member.HandleEvent(EventType.CharacterBeginMovement, member, this, move.Exit);
				foreach (var witness in EventHandlers.Except(member))
				{
					witness.HandleEvent(EventType.CharacterBeginMovementWitness, member, this, move.Exit, witness);
				}

				foreach (var witness in member.Body.ExternalItems)
				{
					witness.HandleEvent(EventType.CharacterBeginMovementWitness, member, this, move.Exit, witness);
				}
			}
		}
	}

	public void ResolveMovement(IMovement move)
	{
		Movements.Remove(move);
		// Cancelled movements need to handle their own events
		if (!move.Cancelled)
		{
			if (Equals(move.Exit.Destination, this))
			{
				foreach (var member in move.CharacterMovers)
				{
					member.HandleEvent(EventType.CharacterEnterCellFinish, member, this, move.Exit);
					foreach (var witness in EventHandlers.Except(member))
					{
						witness.HandleEvent(EventType.CharacterEnterCellFinishWitness, member, this, move.Exit,
							witness);
					}

					foreach (var witness in member.Body.ExternalItems)
					{
						witness.HandleEvent(EventType.CharacterEnterCellFinishWitness, member, this, move.Exit,
							witness);
					}
				}
			}
			else if (Equals(move.Exit.Origin, this))
			{
				foreach (var member in move.CharacterMovers)
				{
					member.HandleEvent(EventType.CharacterEnterCell, member, this, move.Exit);
					foreach (var witness in EventHandlers.Except(member))
					{
						witness.HandleEvent(EventType.CharacterEnterCellWitness, member, this, move.Exit, witness);
					}

					foreach (var witness in member.Body.ExternalItems)
					{
						witness.HandleEvent(EventType.CharacterEnterCellWitness, member, this, move.Exit, witness);
					}
				}
			}
		}
	}

	public override Difficulty LocalAudioDifficulty(IPerceiver perceiver, AudioVolume volume, Proximity proximity)
	{
		var overlay = GetOverlayFor(perceiver);
		return overlay.HearingProfile?.AudioDifficulty(this, volume, proximity) ??
		       base.LocalAudioDifficulty(perceiver, volume, proximity);
	}

	public override void Save()
	{
#if DEBUG
		Console.WriteLine(
			$"Saving Cell {Id:N0} - Changed {Changed} - Contents: {ContentsChanged} - Yields {YieldsChanged}");
#else
#endif
		var dbcell = FMDB.Context.Cells.Find(Id);
		dbcell.CurrentOverlayId = CurrentOverlay.Id;
		dbcell.RoomId = Room.Id;
		dbcell.ForagableProfileId = ForagableProfile?.Id;
		SaveEffects();
		if (ContentsChanged)
		{
			FMDB.Context.CellsGameItems.RemoveRange(dbcell.CellsGameItems);
			foreach (var item in GameItems)
			{
				dbcell.CellsGameItems.Add(new CellsGameItems { Cell = dbcell, GameItemId = item.Id });
			}

			_contentsChanged = false;
		}

		if (ResourcesChanged)
		{
			SaveMagic(dbcell);
		}

		if (YieldsChanged)
		{
			FMDB.Context.CellsForagableYields.RemoveRange(dbcell.CellsForagableYields);
			foreach (var item in _foragableYields)
			{
				var dbyield = new CellsForagableYield
				{
					Cell = dbcell,
					ForagableType = item.Key,
					Yield = item.Value
				};
				dbcell.CellsForagableYields.Add(dbyield);
			}

			_yieldsChanged = false;
		}

		if (TagsChanged)
		{
			SaveTags(dbcell);
		}

		if (EffectsChanged)
		{
			dbcell.EffectData = SaveEffects().ToString();
			EffectsChanged = false;
		}

		if (HooksChanged)
		{
			FMDB.Context.HooksPerceivables.RemoveRange(dbcell.HooksPerceivables);
			foreach (var hook in _installedHooks)
			{
				dbcell.HooksPerceivables.Add(new HooksPerceivable
				{
					Cell = dbcell,
					HookId = hook.Id
				});
			}

			HooksChanged = false;
		}

		Changed = false;
		try
		{
			FMDB.Context.SaveChanges();
		}
		catch (DbUpdateException e)
		{
			Console.WriteLine($"DbUpdateException encountered when saving cell {Id}: {e}");
			Gameworld.SystemMessage($"DbUpdateException encountered when saving cell {Id}: {e}", true);
		}
	}

	public void Dispose()
	{
		Gameworld.Destroy(this);
		GC.SuppressFinalize(this);
	}

	public static void RegisterPerceivableType(IFuturemud gameworld)
	{
		gameworld.RegisterPerceivableType("Cell", id => gameworld.Cells.Get(id));
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return (obj as ICell)?.Id == Id || this == obj;
	}

	public string GetFriendlyReference(IPerceiver voyeur)
	{
		return
			$"{HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLayers)} [#{Id.ToString("N0", voyeur)}]";
	}

	public void SetupCell(MudSharp.Models.Cell cell)
	{
		_noSave = true;
		_id = cell.Id;
		Room.Register(this);
		foreach (var overlay in cell.CellOverlays)
		{
			_overlays.Add(new CellOverlay(overlay, this, Gameworld));
		}

		CurrentOverlay = _overlays.First(x => x.Id == cell.CurrentOverlayId);
		_foragableProfile = Gameworld.ForagableProfiles.Get(cell.ForagableProfileId ?? 0);
		Movements = new List<IMovement>();
		LoadHooks(cell.HooksPerceivables, "Cell");
		LoadEffects(XElement.Parse(cell.EffectData.IfNullOrWhiteSpace("<Effects/>")));
		foreach (var cover in cell.CellsRangedCovers)
		{
			_localCover.Add(Gameworld.RangedCovers.Get(cover.RangedCoverId));
		}

		Temporary = cell.Temporary;
		foreach (var controller in _overlays.SelectNotNull(x => x.Terrain.OverrideWeatherController).Distinct())
		{
			_subscribedWeatherControllers.Add(controller);
			controller.WeatherEcho += TerrainEchoController;
			controller.WeatherChanged += TerrainChangedController;
			controller.WeatherRoomTick += ControllerOnWeatherRoomTick;
		}

		foreach (var area in Areas)
		{
			if (area.Weather != null && !_subscribedWeatherControllers.Contains(area.Weather))
			{
				area.Weather.WeatherEcho += WeatherEchoController;
				area.Weather.WeatherChanged += WeatherChangedController;
				area.Weather.WeatherRoomTick += ControllerOnWeatherRoomTick;
				_subscribedWeatherControllers.Add(area.Weather);
			}
		}

		if (Zone.Weather != null && !_subscribedWeatherControllers.Contains(Zone.Weather))
		{
			Zone.Weather.WeatherEcho += WeatherEchoController;
			Zone.Weather.WeatherChanged += WeatherChangedController;
			Zone.Weather.WeatherRoomTick += ControllerOnWeatherRoomTick;
			_subscribedWeatherControllers.Add(Zone.Weather);
		}

		LoadTags(cell);
		ScheduleCachedEffects();
		LoadMagic(cell);
		_noSave = false;
	}

	private void ControllerOnWeatherRoomTick(Action<ICell> visitor)
	{
		visitor(this);
	}

	private void WeatherChangedController(IWeatherController sender, IWeatherEvent oldWeather, IWeatherEvent newWeather)
	{
		foreach (var handler in EventHandlers)
		{
			if (handler is IPerceiver p && Terrain(p).OverrideWeatherController != sender &&
			    Terrain(p).OverrideWeatherController != null)
			{
				continue;
			}

			HandleEvent(EventType.WeatherChanged, handler, oldWeather, newWeather);
		}
	}

	private void WeatherEchoController(IWeatherController sender, string echo)
	{
		foreach (var actor in Characters.Where(x =>
			         Terrain(x).OverrideWeatherController == sender || Terrain(x).OverrideWeatherController == null))
		{
			if (actor.RoomLayer.IsUnderwater())
			{
				continue;
			}

			switch (actor.Location.OutdoorsType(actor))
			{
				case CellOutdoorsType.Outdoors:
					actor.OutputHandler.Send(echo);
					break;
				case CellOutdoorsType.IndoorsClimateExposed:
				case CellOutdoorsType.IndoorsWithWindows:
					actor.OutputHandler.Send($"{"[Outside]".ColourValue()} {echo}");
					break;
			}
		}
	}

	private readonly List<IWeatherController> _subscribedWeatherControllers = new();

	private void TerrainChangedController(IWeatherController sender, IWeatherEvent oldWeather, IWeatherEvent newWeather)
	{
		foreach (var handler in EventHandlers)
		{
			if (handler is IPerceiver p && Terrain(p).OverrideWeatherController != sender)
			{
				continue;
			}

			HandleEvent(EventType.WeatherChanged, handler, oldWeather, newWeather);
		}
	}

	private void TerrainEchoController(IWeatherController sender, string echo)
	{
		foreach (var actor in Characters)
		{
			if (actor.RoomLayer.IsUnderwater())
			{
				continue;
			}

			var terrain = Terrain(actor);
			if (terrain.OverrideWeatherController != sender)
			{
				continue;
			}


			actor.OutputHandler.Send(echo);
		}
	}

	public void AreaAdded(IArea area)
	{
		if (area.Weather != null && !_subscribedWeatherControllers.Contains(area.Weather))
		{
			area.Weather.WeatherEcho += WeatherEchoController;
			area.Weather.WeatherChanged += WeatherChangedController;
			_subscribedWeatherControllers.Add(area.Weather);
		}
	}

	public void AreaRemoved(IArea area)
	{
		if (area.Weather != null && _subscribedWeatherControllers.Contains(area.Weather))
		{
			area.Weather.WeatherEcho -= WeatherEchoController;
			area.Weather.WeatherChanged -= WeatherChangedController;
			_subscribedWeatherControllers.Remove(area.Weather);
		}
	}

	/// <summary>
	///     If the voyeur is specifying an overlay package they wish to see, and this cell has an overlay from that package,
	///     display that, otherwise display the current one
	/// </summary>
	/// <param name="voyeur">The person for whom the overlay is being displayed</param>
	/// <returns>The appropriate ICellOverlay</returns>
	public ICellOverlay GetOverlayFor(IPerceiver voyeur)
	{
		return voyeur?.CurrentOverlayPackage != null
			? Overlays.FirstOrDefault(x => x.Package == voyeur.CurrentOverlayPackage) ?? CurrentOverlay
			: CurrentOverlay;
	}

	public bool CanSee(ILocateable target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return true;
	}

	public override string ToString()
	{
		return $"Cell ID {Id} Name {CurrentOverlay.CellName}";
	}

	public void OnExitsInitialised()
	{
		CheckFallExitStatus();
	}

	private bool _roomFallActive;
	private bool _treeFallActive;
	private bool _sinkUnderwaterActive;

	private void CheckFallExitStatus()
	{
		if (_roomFallActive)
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= RoomFallTick;
			_roomFallActive = false;
		}

		if (_treeFallActive)
		{
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= TreeFallTick;
			_treeFallActive = false;
		}

		if (_sinkUnderwaterActive)
		{
			Gameworld.HeartbeatManager.FuzzyThirtySecondHeartbeat -= SinkUnderwaterTick;
			_sinkUnderwaterActive = false;
		}

		if (!_characters.Any() && !_gameItems.Any())
		{
			return;
		}

		_fallExit = ExitsFor(null).FirstOrDefault(x => x.IsFallExit && x.OutboundDirection == CardinalDirection.Down);

		if (_fallExit != null || Terrain(null).TerrainLayers.Any(x => x.In(RoomLayer.InAir, RoomLayer.HighInAir)))
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += RoomFallTick;
			_roomFallActive = true;
		}

		if (Terrain(null).TerrainLayers.Any(x => x.In(RoomLayer.HighInTrees, RoomLayer.InTrees, RoomLayer.OnRooftops)))
		{
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += TreeFallTick;
			_treeFallActive = true;
		}

		if (Terrain(null).TerrainLayers.Any(x => x.IsUnderwater()))
		{
			Gameworld.HeartbeatManager.FuzzyThirtySecondHeartbeat += SinkUnderwaterTick;
			_sinkUnderwaterActive = true;
		}
	}

	private ICellExit _fallExit;

	private static Expression _itemWeightPerWindLevelExpression;

	public static Expression ItemWeightPerWindLevelExpression
	{
		get
		{
			if (_itemWeightPerWindLevelExpression == null)
			{
				_itemWeightPerWindLevelExpression = new Expression(Futuremud.Games.First()
				                                                            .GetStaticConfiguration(
					                                                            "ItemWeightPerWindLevelTreeFall"));
			}

			return _itemWeightPerWindLevelExpression;
		}
	}

	private static Dictionary<WindLevel, double> WindLevels = new();

	private double WeightForWind(WindLevel level)
	{
		if (!WindLevels.ContainsKey(level))
		{
			ItemWeightPerWindLevelExpression.Parameters["wind"] = (int)level;
			WindLevels[level] = Convert.ToDouble(ItemWeightPerWindLevelExpression.Evaluate());
		}

		return WindLevels[level];
	}

	private static Dictionary<WindLevel, Difficulty> _treeFallDifficultyPerWind;

	private static Dictionary<WindLevel, Difficulty> TreeFallDifficultyPerWind
	{
		get
		{
			if (_treeFallDifficultyPerWind == null)
			{
				_treeFallDifficultyPerWind = new Dictionary<WindLevel, Difficulty>();
				foreach (var wind in Enum.GetValues(typeof(WindLevel)).OfType<WindLevel>())
				{
					_treeFallDifficultyPerWind[wind] = (Difficulty)Futuremud.Games.First()
					                                                        .GetStaticInt(
						                                                        $"TreeFallDifficulty{wind.DescribeEnum()}");
				}
			}

			return _treeFallDifficultyPerWind;
		}
	}

	private void SinkUnderwaterTick()
	{
		var items = GameItems.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
		var characters = Characters.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
		var lowestLayer = Terrain(null).TerrainLayers.FirstMin(x => x.LayerHeight());
		var liquid = (ILiquid)Terrain(null).WaterFluid;
		var mixture = new LiquidMixture(liquid, double.MaxValue, Gameworld);
		var underwaterLayers = Terrain(null).TerrainLayers.Where(x => IsSwimmingLayer(x)).ToList();
		foreach (var layer in underwaterLayers.OrderBy(x => x.LayerHeight()))
		{
			foreach (var item in items[layer])
			{
				item.ExposeToLiquid(mixture, null, LiquidExposureDirection.Irrelevant);
				if (!item.IsItemType<IHoldable>() || !item.GetItemType<IHoldable>().IsHoldable)
				{
					continue;
				}

				if (layer == lowestLayer && item.PositionState != PositionFloatingInWater.Instance)
				{
					continue;
				}

				if (item.Buoyancy(liquid.Density) >= 0.0)
				{
					continue;
				}

				if (lowestLayer == layer)
				{
					item.OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("ItemSinkHitBottomEmote"), item),
						style: OutputStyle.NoNewLine));
					item.PositionState = PositionUndefined.Instance;
					continue;
				}

				if (IsUnderwaterLayer(layer))
				{
					item.OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("ItemSinkEmote"), item), style: OutputStyle.NoNewLine));
				}
				else
				{
					item.OutputHandler.Handle(new EmoteOutput(
						new Emote(Gameworld.GetStaticString("ItemSinkBelowSurfaceEmote"), item),
						style: OutputStyle.NoNewLine));
				}

				item.RoomLayer = underwaterLayers.Where(x => x.IsLowerThan(layer)).FirstMax(x => x.LayerHeight());
				item.OutputHandler.Handle(new EmoteOutput(
					new Emote(Gameworld.GetStaticString("ItemSinkTargetEmote"), item), style: OutputStyle.NoNewLine,
					flags: OutputFlags.SuppressSource));
			}

			foreach (var character in characters[layer])
			{
				if (character.PositionState == PositionFlying.Instance)
				{
					continue;
				}

				foreach (var item in character.Body.ExposedItems)
				{
					item.ExposeToLiquid(mixture, null, LiquidExposureDirection.FromOnTop);
				}
			}
		}
	}

	private void TreeFallTick()
	{
		var weather = CurrentWeather(null);
		if (weather == null)
		{
			return;
		}

		var itemWeightLimit = WeightForWind(weather.Wind);
		var items = GameItems.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
		foreach (var item in items[RoomLayer.HighInTrees])
		{
			if (!item.IsItemType<IHoldable>() || !item.GetItemType<IHoldable>().IsHoldable)
			{
				continue;
			}

			if (item.Weight > itemWeightLimit)
			{
				continue;
			}

			item.FallToGround();
		}

		foreach (var item in items[RoomLayer.InTrees])
		{
			if (!item.IsItemType<IHoldable>() || !item.GetItemType<IHoldable>().IsHoldable)
			{
				continue;
			}

			if (item.Weight > itemWeightLimit)
			{
				continue;
			}

			item.FallToGround();
		}

		foreach (var item in items[RoomLayer.OnRooftops])
		{
			if (!item.IsItemType<IHoldable>() || !item.GetItemType<IHoldable>().IsHoldable)
			{
				continue;
			}

			if (item.Weight > itemWeightLimit)
			{
				continue;
			}

			item.FallToGround();
		}


		var characters = Characters.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
		var check = Gameworld.GetCheck(CheckType.AvoidFallDueToWind);
		var difficulty = TreeFallDifficultyPerWind[weather.Wind];
		foreach (var ch in characters[RoomLayer.HighInTrees])
		{
			if (check.Check(ch, difficulty).FailureDegrees() <= 1)
			{
				continue;
			}

			ch.FallToGround();
		}

		foreach (var ch in characters[RoomLayer.InTrees])
		{
			if (check.Check(ch, difficulty).FailureDegrees() <= 1)
			{
				continue;
			}

			ch.FallToGround();
		}

		foreach (var ch in characters[RoomLayer.OnRooftops])
		{
			if (check.Check(ch, difficulty).FailureDegrees() <= 1)
			{
				continue;
			}

			ch.FallToGround();
		}
	}

	private void RoomFallTick()
	{
		var items = GameItems.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
		foreach (var item in items[RoomLayer.HighInAir])
		{
			if (item.ShouldFall())
			{
				item.FallToGround();
			}
		}

		var characters = Characters.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
		foreach (var ch in characters[RoomLayer.HighInAir])
		{
			if (ch.ShouldFall())
			{
				ch.FallToGround();
			}
		}

		foreach (var item in items[RoomLayer.InAir])
		{
			if (item.ShouldFall())
			{
				item.FallToGround();
			}
		}

		foreach (var ch in characters[RoomLayer.InAir])
		{
			if (ch.ShouldFall())
			{
				ch.FallToGround();
			}
		}

		if (_fallExit?.Exit.Door?.IsOpen == false)
		{
			return;
		}

		var lowest = Terrain(null).TerrainLayers.LowestLayer();
		if (lowest.IsUnderwater())
		{
			return;
		}

		foreach (var item in items[lowest])
		{
			if (item.ShouldFall())
			{
				item.FallToGround();
			}
		}

		foreach (var ch in characters[lowest])
		{
			if (ch.ShouldFall())
			{
				ch.FallToGround();
			}
		}
	}

	#region ICell Members

	public (bool Truth, IEnumerable<string> Errors) ProposeDelete()
	{
		var response = new ProposalRejectionResponse();
		CellProposedForDeletion?.Invoke(this, response);
		return (!response.IsRejected, response.Reasons);
	}

	public event CellProposedForDeletionDelegate CellProposedForDeletion;
	public event EventHandler CellRequestsDeletion;

	public void Destroy(ICell fallbackCell)
	{
		var action = DestroyWithDatabaseAction(fallbackCell);
		Gameworld.SaveManager.Flush();
		Gameworld.LogManager.FlushLog();
		using (new FMDB())
		{
			action?.Invoke();
			Gameworld.SaveManager.Abort(this);
			if (_id != 0)
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.Cells.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.Cells.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public Action DestroyWithDatabaseAction(ICell fallbackCell)
	{
		_noSave = true;
		CellRequestsDeletion?.Invoke(this, EventArgs.Empty);
		foreach (var item in _gameItems.ToList())
		{
			Extract(item);
			fallbackCell.Insert(item, true);
		}

		foreach (var ch in _characters.ToList())
		{
			ch.Combat?.EndCombat(true);
			ch.Movement?.Cancel();
			ch.Movement = null;
			Leave(ch);
			fallbackCell.Enter(ch);
		}

		Gameworld.SaveManager.Abort(this);
		Gameworld.Destroy(this);
		Gameworld.EffectScheduler.Destroy(this);
		Gameworld.ExitManager.DeleteCell(this);
		return () =>
		{
			EffectHandler.RemoveAllEffects();
			FMDB.Connection.Query($"UPDATE characters SET location = {fallbackCell.Id} WHERE location = {Id}");
		};
	}

	public bool CanGet(IGameItem item, ICharacter getter)
	{
		if (!CanGetAccess(item, getter))
		{
			return false;
		}

		if (Characters.Any(x => x.Body.EffectsOfType<RestraintEffect>().Any(y => y.TargetItem == item)))
		{
			return false;
		}

		return true;
	}

	public string WhyCannotGet(IGameItem item, ICharacter getter)
	{
		if (!CanGetAccess(item, getter))
		{
			return WhyCannotGetAccess(item, getter);
		}

		if (Characters.Any(x => x.Body.EffectsOfType<RestraintEffect>().Any(y => y.TargetItem == item)))
		{
			return
				$"Unfortunately {Characters.Where(x => x.Body.EffectsOfType<RestraintEffect>().Any(y => y.TargetItem == item)).Select(x => x.HowSeen(getter)).ListToString()} are secured to {item.HowSeen(getter)} and so you must first release them from their restraints before you can get that.";
		}

		throw new ApplicationException("");
	}

	public bool CanGetAccess(IGameItem item, ICharacter getter)
	{
		var vicinity = LayerGameItems(getter.RoomLayer).Except(item).Where(x => x.InVicinity(item)).ToList();
		return !LayerCharacters(getter.RoomLayer).Except(getter)
		                                         .Any(
			                                         x =>
				                                         x.EffectsOfType<IGuardItemEffect>()
				                                          .Any(y => (y.Applies() && y.TargetItem == item) ||
				                                                    vicinity.Contains(y.TargetItem)));
	}

	public string WhyCannotGetAccess(IGameItem item, ICharacter getter)
	{
		var vicinity = LayerGameItems(getter.RoomLayer).Except(item).Where(x => x.InVicinity(item)).ToList();
		var guarders =
			LayerCharacters(getter.RoomLayer).Except(getter)
			                                 .Where(
				                                 x =>
					                                 x.EffectsOfType<IGuardItemEffect>()
					                                  .Any(y => (y.Applies() && y.TargetItem == item) ||
					                                            vicinity.Contains(y.TargetItem)))
			                                 .ToList();
		if (guarders.Any())
		{
			return
				$"{guarders.Select(x => x.HowSeen(getter)).ListToString()} {(guarders.Count == 1 ? "is" : "are")} guarding {item.HowSeen(getter)} and blocking all access to it.";
		}

		throw new NotImplementedException();
	}

	private long _foragableProfileId;
	private IForagableProfile _foragableProfile;

	public IForagableProfile ForagableProfile
	{
		get
		{
			if (_foragableProfileId != 0)
			{
				_foragableProfile = Gameworld.ForagableProfiles.Get(_foragableProfileId);
				_foragableProfileId = 0;
			}

			return _foragableProfile ?? Room.Zone.ForagableProfile ?? CurrentOverlay.Terrain.ForagableProfile;
		}
		set
		{
			_foragableProfile = value;
			Changed = true;
		}
	}

	private readonly Dictionary<string, double> _foragableYields = new();

	public double GetForagableYield(string foragableType)
	{
		return _foragableYields.ContainsKey(foragableType) ? _foragableYields[foragableType] : 0.0;
	}

	public void ConsumeYieldFor(IForagable foragable)
	{
		foreach (var type in foragable.ForagableTypes.Where(type => _foragableYields.ContainsKey(type)))
		{
			_foragableYields[type] -= 1.0;
		}

		YieldsChanged = true;
		Gameworld.HeartbeatManager.HourHeartbeat -= YieldTick;
		Gameworld.HeartbeatManager.HourHeartbeat += YieldTick;
	}

	public void ConsumeYield(string foragableType, double yield)
	{
		_foragableYields[foragableType] -= yield;
		YieldsChanged = true;
		Gameworld.HeartbeatManager.HourHeartbeat -= YieldTick;
		Gameworld.HeartbeatManager.HourHeartbeat += YieldTick;
	}

	private double GetMaxYield(string type)
	{
		if (!(ForagableProfile?.MaximumYieldPoints.ContainsKey(type) ?? false))
		{
			return 0.0;
		}

		return ForagableProfile.MaximumYieldPoints[type];
	}

	private double GetHourlyYield(string type)
	{
		if (!ForagableProfile?.HourlyYieldPoints.ContainsKey(type) ?? false)
		{
			return 0.0;
		}

		return ForagableProfile.HourlyYieldPoints[type];
	}

	private void YieldTick()
	{
		if (ForagableProfile == null)
		{
			Gameworld.HeartbeatManager.HourHeartbeat -= YieldTick;
			return;
		}

		foreach (var item in _foragableYields.ToList())
		{
			_foragableYields[item.Key] += GetHourlyYield(item.Key);
			_foragableYields[item.Key] = Math.Min(_foragableYields[item.Key],
				GetMaxYield(item.Key));
		}

		YieldsChanged = true;
		if (_foragableYields.All(x => GetMaxYield(x.Key) <= x.Value))
		{
			Gameworld.HeartbeatManager.HourHeartbeat -= YieldTick;
		}
	}

	public IEnumerable<string> ForagableTypes => _foragableYields.Select(x => x.Key);

	public void PostLoadTasks(MudSharp.Models.Cell cell)
	{
		if (ForagableProfile == null)
		{
			return;
		}

		foreach (var foragable in cell.CellsForagableYields.ToList())
		{
			_foragableYields[foragable.ForagableType] = foragable.Yield;
		}

		foreach (
			var yield in
			ForagableProfile.MaximumYieldPoints.Where(x => !_foragableYields.ContainsKey(x.Key)).ToList())
		{
			_foragableYields[yield.Key] = yield.Value;
		}

		if (!_foragableYields.All(x => GetMaxYield(x.Key) <= x.Value))
		{
			Gameworld.HeartbeatManager.HourHeartbeat += YieldTick;
		}
	}

	private readonly List<ILocalProject> _localProjects = new();
	public IEnumerable<ILocalProject> LocalProjects => _localProjects;

	public void AddProject(ILocalProject project)
	{
		_localProjects.Add(project);
	}

	public void RemoveProject(ILocalProject project)
	{
		_localProjects.Remove(project);
	}

	#endregion

	#region Tags

	private readonly List<ITag> _tags = new();
	public IEnumerable<ITag> Tags => _tags;

	private bool _tagsChanged;

	public bool TagsChanged
	{
		get => _tagsChanged;
		set
		{
			if (value && !_tagsChanged)
			{
				Changed = true;
			}

			_tagsChanged = value;
		}
	}

	public bool AddTag(ITag tag)
	{
		if (!_tags.Contains(tag))
		{
			_tags.Add(tag);
			TagsChanged = true;
			return true;
		}

		return false;
	}

	public bool RemoveTag(ITag tag)
	{
		if (_tags.Contains(tag))
		{
			_tags.Remove(tag);
			TagsChanged = true;
			return true;
		}

		return false;
	}

	public bool IsA(ITag tag)
	{
		return _tags.Any(x => x.IsA(tag));
	}

	private void SaveTags(MudSharp.Models.Cell cell)
	{
		FMDB.Context.CellsTags.RemoveRange(cell.CellsTags);
		foreach (var tag in Tags)
		{
			cell.CellsTags.Add(new CellsTags { Cell = cell, TagId = tag.Id });
		}

		_tagsChanged = false;
	}

	private void LoadTags(MudSharp.Models.Cell cell)
	{
		foreach (var tag in cell.CellsTags)
		{
			var gtag = Gameworld.Tags.Get(tag.TagId);
			if (gtag != null)
			{
				_tags.Add(gtag);
			}
		}
	}

	#endregion
}