using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;

namespace MudSharp.Construction;

public class CellOverlay : SaveableItem, IEditableCellOverlay
{
	private double _addedLight;

	private double _ambientLightFactor;


	private string _cellDescription;

	private string _cellName;

	protected List<long> _exitIDs;

	private IHearingProfile _hearingProfile;

	private CellOutdoorsType _outdoorsType;

	private ITerrain _terrain;

	private IFluid _atmosphere;

	public IFluid Atmosphere
	{
		get => _atmosphere;
		set
		{
			_atmosphere = value;
			Changed = true;
		}
	}

	private bool _safeQuit;

	public bool SafeQuit
	{
		get => _safeQuit;
		set
		{
			_safeQuit = value;
			Changed = true;
		}
	}

	public CellOverlay(MudSharp.Models.CellOverlay overlay, ICell cell, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Cell = cell;
		LoadFromDatabase(overlay);
	}

	protected CellOverlay(ICellOverlay rhs, ICellOverlayPackage package)
	{
		Package = package;
		Cell = rhs.Cell;
		Gameworld = rhs.Gameworld;
		using (new FMDB())
		{
			var dboverlay = new Models.CellOverlay
			{
				Name = rhs.Name,
				CellId = rhs.Cell.Id,
				CellDescription = rhs.CellDescription,
				CellName = rhs.CellName,
				CellOverlayPackageId = package.Id,
				CellOverlayPackageRevisionNumber = package.RevisionNumber,
				TerrainId = rhs.Terrain.Id,
				OutdoorsType = (int)rhs.OutdoorsType,
				HearingProfileId = rhs.HearingProfile?.Id,
				AmbientLightFactor = rhs.AmbientLightFactor,
				AddedLight = rhs.AddedLight,
				AtmosphereId = rhs.Atmosphere?.Id,
				AtmosphereType = rhs.Atmosphere is ILiquid ? "liquid" : "gas",
				SafeQuit = rhs.SafeQuit
			};
			foreach (var exit in rhs.ExitIDs)
			{
				dboverlay.CellOverlaysExits.Add(new CellOverlayExit { CellOverlay = dboverlay, ExitId = exit });
			}

			FMDB.Context.CellOverlays.Add(dboverlay);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dboverlay);
		}
	}

	public CellOverlay(ICell cell, ICellOverlayPackage package)
	{
		Gameworld = cell.Gameworld;
		Package = package;
		Cell = cell;
		var terrain = FMDB.Context.Terrains.First(x => x.DefaultTerrain);
		using (new FMDB())
		{
			var dboverlay = new Models.CellOverlay
			{
				Name = package.Name,
				CellId = Cell.Id,
				CellDescription =
					"This is a newly built location that has not yet been described. It should not be approved for use in game.",
				CellName = "An Unnamed Location",
				CellOverlayPackageId = package.Id,
				CellOverlayPackageRevisionNumber = package.RevisionNumber,
				Terrain = terrain,
				OutdoorsType = terrain.DefaultCellOutdoorsType,
				AddedLight = 0,
				AmbientLightFactor = (CellOutdoorsType)terrain.DefaultCellOutdoorsType switch
				{
					CellOutdoorsType.Outdoors => 1.0,
					CellOutdoorsType.Indoors => 0.25,
					CellOutdoorsType.IndoorsClimateExposed => 0.9,
					CellOutdoorsType.IndoorsWithWindows => 0.35,
					CellOutdoorsType.IndoorsNoLight => 0.0,
					_ => 1.0
				},
				AtmosphereId = Gameworld.GetStaticLong("DefaultAtmosphereId"),
				AtmosphereType = Gameworld.GetStaticConfiguration("DefaultAtmosphereType"),
				SafeQuit = Gameworld.GetStaticBool("RoomsSafeQuitByDefault")
			};
			FMDB.Context.CellOverlays.Add(dboverlay);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dboverlay);
		}
	}

	public CellOverlay(ICell cell, ICellOverlayPackage package, ICell templateCell)
	{
		Gameworld = cell.Gameworld;
		Package = package;
		Cell = cell;
		using (new FMDB())
		{
			var dboverlay = new Models.CellOverlay
			{
				Name = package.Name,
				CellId = Cell.Id,
				CellDescription = templateCell?.CurrentOverlay.CellDescription ??
				                  "This is a newly built location that has not yet been described. It should not be approved for use in game.",
				CellName = templateCell?.CurrentOverlay.CellName ?? "An Unnamed Location",
				CellOverlayPackageId = package.Id,
				CellOverlayPackageRevisionNumber = package.RevisionNumber
			};
			var terrainId = templateCell?.CurrentOverlay.Terrain.Id ?? 0;
			dboverlay.Terrain = FMDB.Context.Terrains.FirstOrDefault(x => x.Id == terrainId) ??
			                    FMDB.Context.Terrains.First(x => x.DefaultTerrain);
			dboverlay.OutdoorsType = (int)(templateCell?.CurrentOverlay.OutdoorsType ?? CellOutdoorsType.Outdoors);
			dboverlay.AddedLight = templateCell?.CurrentOverlay.AddedLight ?? 0;
			dboverlay.AmbientLightFactor = templateCell?.CurrentOverlay.AmbientLightFactor ?? 1.0;
			dboverlay.SafeQuit = templateCell?.SafeQuit ?? Gameworld.GetStaticBool("RoomsSafeQuitByDefault");
			if (templateCell != null)
			{
				dboverlay.AtmosphereId = templateCell.CurrentOverlay.Atmosphere?.Id;
				dboverlay.AtmosphereType = templateCell.CurrentOverlay.Atmosphere is ILiquid ? "liquid" : "gas";
			}
			else
			{
				dboverlay.AtmosphereId = Gameworld.GetStaticLong("DefaultAtmosphereId");
				dboverlay.AtmosphereType = Gameworld.GetStaticConfiguration("DefaultAtmosphereType");
			}

			FMDB.Context.CellOverlays.Add(dboverlay);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dboverlay);
		}
	}

	public override string FrameworkItemType => "CellOverlay";

	public override string Name => Package.Name;

	public ICellOverlayPackage Package { get; protected set; }

	/// <summary>
	///     When this CellOverlay is in place, this is the Name of the Cell
	/// </summary>
	public string CellName
	{
		get => _cellName;
		set
		{
			_cellName = value;
			Changed = true;
		}
	}

	/// <summary>
	///     When this CellOverlay is in place, this is the Description of the Cell
	/// </summary>
	public string CellDescription
	{
		get => _cellDescription;
		set
		{
			_cellDescription = value;
			Changed = true;
		}
	}

	public ITerrain Terrain
	{
		get => _terrain;
		set
		{
			_terrain = value;
			_atmosphere = _terrain?.Atmosphere;
			Changed = true;
		}
	}

	public CellOutdoorsType OutdoorsType
	{
		get => _outdoorsType;
		set
		{
			_outdoorsType = value;
			Changed = true;
		}
	}

	public IHearingProfile HearingProfile
	{
		get => _hearingProfile;
		set
		{
			_hearingProfile = value;
			Changed = true;
		}
	}

	public double AmbientLightFactor
	{
		get => _ambientLightFactor;
		set
		{
			_ambientLightFactor = value;
			Changed = true;
		}
	}

	public double AddedLight
	{
		get => _addedLight;
		set
		{
			_addedLight = value;
			Changed = true;
		}
	}

	/// <summary>
	///     The ID numbers of the Exits which are in place when this CellOverlay is selected
	/// </summary>
	public IEnumerable<long> ExitIDs => _exitIDs;

	public ICell Cell { get; protected set; }

	public IEditableCellOverlay CreateClone(ICellOverlayPackage package)
	{
		return new CellOverlay(this, package);
	}


	public void AddExit(IExit exit)
	{
		_exitIDs.Add(exit.Id);
		Changed = true;
		Gameworld.ExitManager.UpdateCellOverlayExits(Cell, this);
	}

	public void RemoveExit(IExit exit)
	{
		_exitIDs.Remove(exit.Id);
		Changed = true;
		Gameworld.ExitManager.UpdateCellOverlayExits(Cell, this);
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dboverlay = FMDB.Context.CellOverlays.Find(Id);
			dboverlay.Name = Name;
			dboverlay.CellName = CellName;
			dboverlay.CellDescription = CellDescription;
			dboverlay.TerrainId = Terrain.Id;
			dboverlay.OutdoorsType = (int)OutdoorsType;
			dboverlay.HearingProfileId = HearingProfile?.Id;
			dboverlay.AddedLight = AddedLight;
			dboverlay.AmbientLightFactor = AmbientLightFactor;
			dboverlay.AtmosphereId = Atmosphere?.Id;
			dboverlay.AtmosphereType = Atmosphere is ILiquid ? "liquid" : "gas";
			dboverlay.SafeQuit = SafeQuit;
			FMDB.Context.CellOverlaysExits.RemoveRange(dboverlay.CellOverlaysExits);
			foreach (var exit in ExitIDs)
			{
				dboverlay.CellOverlaysExits.Add(new CellOverlayExit { CellOverlay = dboverlay, ExitId = exit });
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	private void LoadFromDatabase(Models.CellOverlay overlay)
	{
		_noSave = true;
		Package = Gameworld.CellOverlayPackages.Get(overlay.CellOverlayPackageId,
			overlay.CellOverlayPackageRevisionNumber);
		_id = overlay.Id;
		CellName = overlay.CellName;
		CellDescription = overlay.CellDescription;
		_exitIDs = overlay.CellOverlaysExits.Select(x => x.ExitId).ToList();
		_terrain = Gameworld.Terrains.Get(overlay.TerrainId);
		HearingProfile = Gameworld.HearingProfiles.Get(overlay.HearingProfileId ?? 0);
		OutdoorsType = (CellOutdoorsType)overlay.OutdoorsType;
		AmbientLightFactor = overlay.AmbientLightFactor;
		AddedLight = overlay.AddedLight;
		_safeQuit = overlay.SafeQuit;
		if (overlay.AtmosphereId != null)
		{
			Atmosphere = overlay.AtmosphereType.Equals("gas", StringComparison.InvariantCultureIgnoreCase)
				? (IFluid)Gameworld.Gases.Get(overlay.AtmosphereId.Value)
				: Gameworld.Liquids.Get(overlay.AtmosphereId.Value);
		}

		_noSave = false;
	}
}