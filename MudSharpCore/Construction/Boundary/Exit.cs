using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.Construction.Boundary;

public class Exit : PerceivedItem, IExit
{
	private readonly List<ICell> _cells = new();

	private readonly ICellExit[] CellExits = new ICellExit[2];

	/// <summary>
	///     Constructs an Exit from information pertaining to a Non-Cardinal Exit
	/// </summary>
	/// <param name="gameworld"></param>
	/// <param name="origin"></param>
	/// <param name="destination"></param>
	/// <param name="timeMultiplier"></param>
	/// <param name="template"></param>
	/// <param name="outboundTarget"></param>
	/// <param name="inboundTarget"></param>
	/// <param name="outboundKeyword"></param>
	/// <param name="inboundKeyword"></param>
	public Exit(IFuturemud gameworld, ICell origin, ICell destination, double timeMultiplier,
		INonCardinalExitTemplate template, string outboundKeyword, string inboundKeyword, string outboundTarget,
		string inboundTarget)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbexit = new Models.Exit
			{
				CellId1 = origin.Id,
				CellId2 = destination.Id,
				Direction1 = (int)CardinalDirection.Unknown,
				Direction2 = (int)CardinalDirection.Unknown,
				TimeMultiplier = timeMultiplier,
				AcceptsDoor = false,

				InboundDescription1 = template.OriginInboundPreface,
				InboundDescription2 = template.DestinationInboundPreface,
				OutboundDescription1 = template.OriginOutboundPreface,
				OutboundDescription2 = template.DestinationOutboundPreface,

				Verb1 = template.OutboundVerb,
				Verb2 = template.InboundVerb,

				OutboundTarget1 = outboundTarget,
				InboundTarget2 = outboundTarget,
				OutboundTarget2 = inboundTarget,
				InboundTarget1 = inboundTarget,

				Keywords1 =
					GetKeywordsFromSDesc(outboundTarget)
						.Concat(new[] { outboundKeyword })
						.ToList()
						.ListToString(separator: " ", conjunction: ""),
				Keywords2 =
					GetKeywordsFromSDesc(inboundTarget)
						.Concat(new[] { inboundKeyword })
						.ToList()
						.ListToString(separator: " ", conjunction: ""),

				PrimaryKeyword1 = outboundKeyword,
				PrimaryKeyword2 = inboundKeyword,

				MaximumSizeToEnter = (int)SizeCategory.Titanic,
				MaximumSizeToEnterUpright = (int)SizeCategory.Titanic,
				BlockedLayers = BlockedLayers.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues(),
				IsClimbExit = false,
				ClimbDifficulty = (int)Difficulty.Normal
			};

			FMDB.Context.Exits.Add(dbexit);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbexit);
		}
	}

	public override object DatabaseInsert()
	{
		var dbexit = new Models.Exit
		{
			CellId1 = _cells[0].Id,
			CellId2 = _cells[1].Id
		};
		dbexit.CellId1 = CellExits[0].Origin.Id;
		dbexit.CellId2 = CellExits[1].Origin.Id;
		dbexit.Direction1 = (int)CellExits[0].OutboundDirection;
		dbexit.Direction2 = (int)CellExits[1].OutboundDirection;
		dbexit.TimeMultiplier = TimeMultiplier;
		dbexit.AcceptsDoor = AcceptsDoor;
		dbexit.DoorSize = AcceptsDoor ? (int)DoorSize : (int?)null;
		dbexit.MaximumSizeToEnter = (int)MaximumSizeToEnter;
		dbexit.MaximumSizeToEnterUpright = (int)MaximumSizeToEnterUpright;
		dbexit.ClimbDifficulty = (int)ClimbDifficulty;
		dbexit.IsClimbExit = IsClimbExit;
		dbexit.BlockedLayers = BlockedLayers.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues();
		FMDB.Context.Exits.Add(dbexit);
		if (CellExits[0] is NonCardinalCellExit nexit1)
		{
			var nexit2 = (NonCardinalCellExit)CellExits[1];
			dbexit.InboundDescription1 = nexit1.InboundDescription;
			dbexit.InboundDescription2 = nexit2.InboundDescription;
			dbexit.OutboundDescription1 = nexit1.OutboundDescription;
			dbexit.OutboundDescription2 = nexit2.OutboundDescription;

			dbexit.Verb1 = nexit1.Verb;
			dbexit.Verb2 = nexit2.Verb;

			dbexit.OutboundTarget1 = nexit1.OutboundTarget;
			dbexit.OutboundTarget2 = nexit2.OutboundTarget;
			dbexit.InboundTarget1 = nexit1.InboundTarget;
			dbexit.InboundTarget2 = nexit2.InboundTarget;

			dbexit.Keywords1 =
				GetKeywordsFromSDesc(nexit1.OutboundTarget)
					.Concat(new[] { nexit1.PrimaryKeyword })
					.ToList()
					.ListToString(separator: " ", conjunction: "");
			dbexit.Keywords2 =
				GetKeywordsFromSDesc(nexit2.OutboundTarget)
					.Concat(new[] { nexit2.PrimaryKeyword })
					.ToList()
					.ListToString(separator: " ", conjunction: "");

			dbexit.PrimaryKeyword1 = nexit1.PrimaryKeyword;
			dbexit.PrimaryKeyword2 = nexit2.PrimaryKeyword;
		}

		return dbexit;
	}

	public Exit(IFuturemud gameworld, ICell origin, ICell destination, Exit otherExit)
	{
		Gameworld = gameworld;
		_noSave = true;
		TimeMultiplier = otherExit.TimeMultiplier;

		AcceptsDoor = otherExit.AcceptsDoor;
		DoorSize = otherExit.DoorSize;
		_cells.Add(origin);
		_cells.Add(destination);

		if (otherExit.CellExits[0] is NonCardinalCellExit exit)
		{
			CellExits[0] = new NonCardinalCellExit(this, exit, origin, destination);
			CellExits[1] =
				new NonCardinalCellExit(this, (NonCardinalCellExit)otherExit.CellExits[1], destination, origin);
		}
		else
		{
			CellExits[0] = new CellExit(this, otherExit.CellExits[0], origin, destination);
			CellExits[1] = new CellExit(this, otherExit.CellExits[1], destination, origin);
		}

		MaximumSizeToEnter = otherExit.MaximumSizeToEnter;
		MaximumSizeToEnterUpright = otherExit.MaximumSizeToEnterUpright;

		_noSave = false;
	}

	/// <summary>
	///     Constructs an Exit designed to function as a Cardinal exit
	/// </summary>
	/// <param name="gameworld"></param>
	/// <param name="origin"></param>
	/// <param name="destination"></param>
	/// <param name="outboundDirection"></param>
	/// <param name="inboundDirection"></param>
	/// <param name="timeMultiplier"></param>
	public Exit(IFuturemud gameworld, ICell origin, ICell destination, CardinalDirection outboundDirection,
		CardinalDirection inboundDirection, double timeMultiplier)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbexit = new Models.Exit
			{
				CellId1 = origin.Id,
				CellId2 = destination.Id,
				Direction1 = (int)outboundDirection,
				Direction2 = (int)inboundDirection,
				TimeMultiplier = timeMultiplier,
				AcceptsDoor = false,
				MaximumSizeToEnter = (int)SizeCategory.Titanic,
				MaximumSizeToEnterUpright = (int)SizeCategory.Titanic,
				BlockedLayers = string.Empty
			};
			FMDB.Context.Exits.Add(dbexit);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbexit);
		}
	}

	public Exit(MudSharp.Models.Exit exit, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDatabase(exit);
	}

	private Exit(Exit rhs)
	{
		Gameworld = rhs.Gameworld;
		using (new FMDB())
		{
			var exit1 = rhs.CellExits[0];
			var exit2 = rhs.CellExits[1];
			var dbexit = new Models.Exit
			{
				CellId1 = exit1.Origin.Id,
				CellId2 = exit2.Origin.Id,
				Direction1 = (int)exit1.OutboundDirection,
				Direction2 = (int)exit2.OutboundDirection,
				TimeMultiplier = rhs.TimeMultiplier,
				AcceptsDoor = rhs.AcceptsDoor,
				DoorSize = rhs.AcceptsDoor ? (int)rhs.DoorSize : (int?)null,
				MaximumSizeToEnter = (int)rhs.MaximumSizeToEnter,
				MaximumSizeToEnterUpright = (int)rhs.MaximumSizeToEnterUpright,
				BlockedLayers = rhs.BlockedLayers.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues(),
				ClimbDifficulty = (int)rhs.ClimbDifficulty,
				FallCell = rhs.FallCell?.Id,
				IsClimbExit = rhs.IsClimbExit
			};

			if (exit1 is NonCardinalCellExit)
			{
				var nexit1 = exit1 as NonCardinalCellExit;
				var nexit2 = exit2 as NonCardinalCellExit;
				dbexit.InboundDescription1 = nexit1.InboundDescription;
				dbexit.InboundDescription2 = nexit2.InboundDescription;
				dbexit.OutboundDescription1 = nexit1.OutboundDescription;
				dbexit.OutboundDescription2 = nexit2.OutboundDescription;

				dbexit.Verb1 = nexit1.Verb;
				dbexit.Verb2 = nexit2.Verb;

				dbexit.OutboundTarget1 = nexit1.OutboundTarget;
				dbexit.OutboundTarget2 = nexit2.OutboundTarget;
				dbexit.InboundTarget1 = nexit1.InboundTarget;
				dbexit.InboundTarget2 = nexit2.InboundTarget;

				dbexit.Keywords1 =
					GetKeywordsFromSDesc(nexit1.OutboundTarget)
						.Concat(new[] { nexit1.PrimaryKeyword })
						.ToList()
						.ListToString(separator: " ", conjunction: "");
				dbexit.Keywords2 =
					GetKeywordsFromSDesc(nexit2.OutboundTarget)
						.Concat(new[] { nexit2.PrimaryKeyword })
						.ToList()
						.ListToString(separator: " ", conjunction: "");

				dbexit.PrimaryKeyword1 = nexit1.PrimaryKeyword;
				dbexit.PrimaryKeyword2 = nexit2.PrimaryKeyword;
			}

			FMDB.Context.Exits.Add(dbexit);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbexit);
		}
	}

	public override string ToString()
	{
		return $"Exit {Id:N0} from {_cells[0].Name} ({_cells[0].Id:N0}) to {_cells[1].Name} ({_cells[1].Id})";
	}

	public override string FrameworkItemType => "Exit";

	#region IFutureProgVariable Implementation

	public override FutureProgVariableTypes Type => FutureProgVariableTypes.Error;

	#endregion

	public IEnumerable<ICell> Cells => _cells;

	#region Overrides of PerceivedItem

	public override ICell Location => CellExits[0].Origin;

	#endregion

	public bool AcceptsDoor { get; set; }
	public SizeCategory DoorSize { get; set; }
	public IDoor Door { get; set; }
	public double TimeMultiplier { get; set; }

	public ICell FallCell { get; set; }
	public bool IsClimbExit { get; set; }
	public Difficulty ClimbDifficulty { get; set; }

	public SizeCategory MaximumSizeToEnterUpright { get; set; }

	public SizeCategory MaximumSizeToEnter { get; set; }

	public ICell Opposite(ICell cell)
	{
		return CellExitFor(cell)?.Destination;
	}

	public ICellExit CellExitFor(ICell cell)
	{
		return CellExits.FirstOrDefault(x => x.Origin == cell);
	}

	public bool IsExit(ICell cell, string verb)
	{
		return CellExitFor(cell).IsExit(verb);
	}

	public bool IsExitKeyword(ICell cell, string keyword)
	{
		return CellExitFor(cell).IsExitKeyword(keyword);
	}

	private readonly List<RoomLayer> _blockedLayers = new();
	public IEnumerable<RoomLayer> BlockedLayers => _blockedLayers;

	public void AddBlockedLayer(RoomLayer layer)
	{
		_blockedLayers.Add(layer);
		Changed = true;
	}

	public void RemoveBlockedLayer(RoomLayer layer)
	{
		_blockedLayers.Remove(layer);
		Changed = true;
	}

	public override void Register(IOutputHandler handler)
	{
		throw new NotSupportedException();
	}

	public IExit Clone()
	{
		return new Exit(this);
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				if (Door is not null)
				{
					Door.Parent.Delete();
				}

				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.Exits.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.Exits.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbexit = FMDB.Context.Exits.Find(Id);
			dbexit.AcceptsDoor = AcceptsDoor;
			dbexit.DoorSize = AcceptsDoor ? (int)DoorSize : (int?)null;
			dbexit.DoorId = Door?.Parent.Id;
			dbexit.MaximumSizeToEnter = (int)MaximumSizeToEnter;
			dbexit.MaximumSizeToEnterUpright = (int)MaximumSizeToEnterUpright;
			dbexit.IsClimbExit = IsClimbExit;
			dbexit.ClimbDifficulty = (int)ClimbDifficulty;
			dbexit.FallCell = FallCell?.Id;
			dbexit.BlockedLayers = BlockedLayers.Select(x => ((int)x).ToString("F0")).ListToCommaSeparatedValues();
			FMDB.Context.SaveChanges();
			// TODO - saving cellExit changes too?
		}

		Changed = false;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((MudSharp.Models.Exit)dbitem).Id;
	}

	public void PostLoadTasks(MudSharp.Models.Exit exit)
	{
		if (exit.DoorId.HasValue)
		{
			using (new FMDB())
			{
				var gitem = Gameworld.TryGetItem(exit.DoorId ?? 0, true);
				var door = gitem?.GetItemType<IDoor>();
				if (door != null)
				{
					Door = door;
					door.InstalledExit = this;
					gitem.FinaliseLoadTimeTasks();
				}
			}
		}
	}

	private void LoadFromDatabase(MudSharp.Models.Exit exit)
	{
		_id = exit.Id;
		IdInitialised = true;
		_noSave = true;
		TimeMultiplier = exit.TimeMultiplier;

		AcceptsDoor = exit.AcceptsDoor;
		DoorSize = (SizeCategory)(exit.DoorSize ?? 0);
		_cells.Add(Gameworld.Cells.Get(exit.CellId1));
		_cells.Add(Gameworld.Cells.Get(exit.CellId2));

		if (!string.IsNullOrEmpty(exit.Verb1))
		{
			CellExits[0] = new NonCardinalCellExit(this, exit, true);
			CellExits[1] = new NonCardinalCellExit(this, exit, false);
		}
		else
		{
			CellExits[0] = new CellExit(this, exit, true);
			CellExits[1] = new CellExit(this, exit, false);
		}

		MaximumSizeToEnter = (SizeCategory)exit.MaximumSizeToEnter;
		MaximumSizeToEnterUpright = (SizeCategory)exit.MaximumSizeToEnterUpright;
		ClimbDifficulty = (Difficulty)exit.ClimbDifficulty;
		if (exit.FallCell.HasValue)
		{
			FallCell = _cells.First(x => x.Id == exit.FallCell);
		}

		IsClimbExit = exit.IsClimbExit;
		if (!string.IsNullOrEmpty(exit.BlockedLayers))
		{
			_blockedLayers.AddRange(exit.BlockedLayers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
			                            .Select(x => (RoomLayer)int.Parse(x)));
		}

		_noSave = false;
	}
}