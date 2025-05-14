using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;

namespace MudSharp.Construction;

public class CellOverlayPackage : Framework.Revision.EditableItem, ICellOverlayPackage
{
	public CellOverlayPackage(MudSharp.Models.CellOverlayPackage package, IFuturemud gameworld)
		: base(package.EditableItem)
	{
		Gameworld = gameworld;
		LoadFromDatabase(package);
	}

	public CellOverlayPackage(IFuturemud gameworld, IAccount originator, string name)
		: base(originator)
	{
		Gameworld = gameworld;
		_name = name;
		using (new FMDB())
		{
			try
			{
				var dbpack = new Models.CellOverlayPackage();
				FMDB.Context.CellOverlayPackages.Add(dbpack);
				dbpack.Id = gameworld.CellOverlayPackages.NextID();
				dbpack.RevisionNumber = RevisionNumber;
				dbpack.Name = name;
				dbpack.EditableItem = new Models.EditableItem
				{
					RevisionNumber = RevisionNumber,
					BuilderAccountId = BuilderAccountID,
					BuilderDate = BuilderDate,
					RevisionStatus = (int)Status
				};
				FMDB.Context.EditableItems.Add(dbpack.EditableItem);
				FMDB.Context.SaveChanges();
				LoadFromDatabase(dbpack);
			}
			catch (DbUpdateException e)
			{
				Console.WriteLine(e.Message);
				throw;
			}
		}
	}

	public override string FrameworkItemType => "CellOverlayPackage";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new NotSupportedException();
	}

	public override string Show(ICharacter actor)
	{
		using (new FMDB())
		{
			var sb = new StringBuilder();
			var splitterLine = new string('=', actor.Account.LineFormatLength).Colour(Telnet.Green).NoWrap();
			sb.AppendLine(splitterLine);
			sb.AppendLine($"{EditHeader(),-60}{"Status: " + Status.Describe(),-60}");
			sb.AppendLine(splitterLine);
			sb.AppendLine(
				$"{"Built On: " + BuilderDate.GetLocalDateString(actor),-60}{"Built By: " + (FMDB.Context.Accounts.Find(BuilderAccountID)?.Name.Proper() ?? "Nobody"),-60}");
			sb.AppendLine(splitterLine);
			if (Status == RevisionStatus.Current || Status == RevisionStatus.Revised ||
			    Status == RevisionStatus.Obsolete)
			{
				sb.AppendLine(
					$"{"Approved On: " + (ReviewerDate?.GetLocalDateString(actor) ?? "Never"),-60}{"Approved By: " + (FMDB.Context.Accounts.Find(ReviewerAccountID)?.Name.Proper() ?? "Nobody"),-60}");
				sb.AppendLine(splitterLine);
			}

			if (Status == RevisionStatus.Rejected)
			{
				sb.AppendLine(
					$"{"Rejected On: " + (ReviewerDate?.GetLocalDateString(actor) ?? "Never"),-60}{"Rejected By: " + (FMDB.Context.Accounts.Find(ReviewerAccountID)?.Name.Proper() ?? "Nobody"),-60}");
				sb.AppendLine(splitterLine);
			}

			if (Status == RevisionStatus.Obsolete)
			{
				sb.AppendLine($"{"Obsolete On: " + (ObsoleteDate?.GetLocalDateString(actor) ?? "Never"),-60}");
				sb.AppendLine(splitterLine);
			}

			sb.AppendLine();
			var overlays = Gameworld.Cells.SelectMany(x => x.Overlays).Where(x => x.Package == this).ToList();
			sb.AppendLine(
				$"Overlays ({overlays.Count(x => x.Cell.CurrentOverlay == x).ToString("N0", actor).ColourValue()} of {overlays.Count.ToString("N0", actor).ColourValue()} live):");
			sb.AppendLine();

			foreach (var overlay in overlays)
			{
				sb.AppendLine(
					$"Cell #{overlay.Cell.Id.ToString("N0", actor)} ({overlay.Cell.Zone.Name.ColourName()}): {overlay.CellName.ColourName()}");
			}

			return sb.ToString();
		}
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var dbnew = new Models.CellOverlayPackage
			{
				Id = Id,
				RevisionNumber =
					FMDB.Context.CellOverlayPackages.Where(x => x.Id == Id)
					    .Select(x => x.RevisionNumber)
					    .AsEnumerable()
					    .DefaultIfEmpty(0)
					    .Max() + 1,
				Name = base.Name.Proper()
			};
			dbnew.EditableItem = new Models.EditableItem
			{
				BuilderDate = DateTime.UtcNow,
				RevisionNumber = dbnew.RevisionNumber,
				BuilderAccountId = initiator.Account.Id,
				RevisionStatus = (int)RevisionStatus.UnderDesign
			};
			var dbold = FMDB.Context.CellOverlayPackages
			                .Include(x => x.CellOverlays)
			                .ThenInclude(x => x.CellOverlaysExits)
			                .ThenInclude(cellOverlayExit => cellOverlayExit.Exit)
			                .Include(cellOverlayPackage => cellOverlayPackage.CellOverlays)
			                .ThenInclude(cellOverlay => cellOverlay.Cell)
			                .Include(cellOverlayPackage => cellOverlayPackage.CellOverlays)
			                .ThenInclude(cellOverlay => cellOverlay.HearingProfile)
			                .Include(cellOverlayPackage => cellOverlayPackage.CellOverlays)
			                .ThenInclude(cellOverlay => cellOverlay.Terrain)
			                .First(x => x.Id == Id && x.RevisionNumber == RevisionNumber);
			foreach (var overlay in dbold.CellOverlays)
			{
				var newOverlay = new Models.CellOverlay
				{
					AddedLight = overlay.AddedLight,
					AmbientLightFactor = overlay.AmbientLightFactor,
					AtmosphereId = overlay.AtmosphereId,
					AtmosphereType = overlay.AtmosphereType,
					Cell = overlay.Cell,
					CellDescription = overlay.CellDescription,
					CellName = overlay.CellName,
					CellOverlayPackage = dbnew,
					HearingProfile = overlay.HearingProfile,
					Name = overlay.Name,
					OutdoorsType = overlay.OutdoorsType,
					SafeQuit = overlay.SafeQuit,
					Terrain = overlay.Terrain
				};
				foreach (var exit in overlay.CellOverlaysExits)
				{
					newOverlay.CellOverlaysExits.Add(new CellOverlayExit
					{
						CellOverlay = newOverlay,
						Exit = exit.Exit
					});
				}

				dbnew.CellOverlays.Add(newOverlay);
				FMDB.Context.CellOverlays.Add(newOverlay);
			}

			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			FMDB.Context.CellOverlayPackages.Add(dbnew);
			FMDB.Context.SaveChanges();
			var package = new CellOverlayPackage(dbnew, Gameworld);
			Gameworld.Add(package);
			foreach (var overlay in dbnew.CellOverlays)
			{
				var cell = Gameworld.Cells.Get(overlay.CellId);
				if (cell is null)
				{
					continue;
				}
				cell.AddOverlay(new CellOverlay(overlay, cell, Gameworld));
			}

			return package;
		}
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.CellOverlayPackages.GetAll(Id);
	}

	public override string EditHeader()
	{
		return $"Cell Overlay Package: \"{Name}\" (ID #{Id} Rev {RevisionNumber})";
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbpack = FMDB.Context.CellOverlayPackages.Find(Id, RevisionNumber);
			dbpack.Name = Name;

			if (_statusChanged)
			{
				base.Save(dbpack.EditableItem);
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public void LoadFromDatabase(MudSharp.Models.CellOverlayPackage package)
	{
		_name = package.Name;
		_id = package.Id;
	}

	public void SetName(string newName)
	{
		_name = newName;
		Changed = true;
	}

	#region Implementation of IFutureProgVariable

	/// <summary>
	///     The FutureProgVariableType that represents this IFutureProgVariable
	/// </summary>
	public ProgVariableTypes Type => ProgVariableTypes.OverlayPackage;

	/// <summary>
	///     Returns an object representing the underlying variable wrapped in this IFutureProgVariable
	/// </summary>
	public object GetObject => this;

	/// <summary>
	///     Requests an IFutureProgVariable representing the property referenced by the given string.
	/// </summary>
	/// <param name="property">A string representing the property to be retrieved</param>
	/// <returns>An IFutureProgVariable representing the desired property</returns>
	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "revnum":
			case "revision":
				return new NumberVariable(RevisionNumber);
			case "status":
				return new TextVariable(Status.Describe());
		}

		throw new NotSupportedException($"Unsupported property type {property} in {FrameworkItemType}.GetProperty");
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "revnum", ProgVariableTypes.Number },
			{ "revision", ProgVariableTypes.Number },
			{ "status", ProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "revnum", "An alias for the revision property" },
			{ "revision", "" },
			{ "status", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.OverlayPackage,
			DotReferenceHandler(), DotReferenceHelp());
	}

	#endregion
}