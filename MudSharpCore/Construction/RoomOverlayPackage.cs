using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;

namespace MudSharp.Construction;

public class RoomOverlayPackage : Framework.Revision.EditableItem, IRoomOverlayPackage
{
	public const int MaximumNameLength = 4000;

    public RoomOverlayPackage(MudSharp.Models.RoomOverlayPackage package, IFuturemud gameworld)
        : base(package.EditableItem)
    {
        Gameworld = gameworld;
        LoadFromDatabase(package);
    }

    public RoomOverlayPackage(IFuturemud gameworld, IAccount originator, string name)
        : base(originator)
    {
        Gameworld = gameworld;
        _name = name;
        using (new FMDB())
        {
            try
            {
                Models.RoomOverlayPackage dbpack = new();
                FMDB.Context.RoomOverlayPackages.Add(dbpack);
                dbpack.Id = gameworld.RoomOverlayPackages.NextID();
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

    public override string FrameworkItemType => "RoomOverlayPackage";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        throw new NotSupportedException();
    }

    public override string Show(ICharacter actor)
    {
        using (new FMDB())
        {
            StringBuilder sb = new();
            string splitterLine = new string('=', actor.Account.LineFormatLength).Colour(Telnet.Green).NoWrap();
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
            List<IRoomOverlay> overlays = Gameworld.Rooms.SelectMany(x => x.Overlays).Where(x => x.Package == this).ToList();
            sb.AppendLine(
                $"Overlays ({overlays.Count(x => x.Room.CurrentOverlay == x).ToString("N0", actor).ColourValue()} of {overlays.Count.ToString("N0", actor).ColourValue()} live):");
            sb.AppendLine();

            foreach (IRoomOverlay overlay in overlays)
            {
                sb.AppendLine(
                    $"Room #{overlay.Room.Id.ToString("N0", actor)} ({overlay.Room.Zone.Name.ColourName()}): {overlay.RoomName.ColourName()}");
            }

            return sb.ToString();
        }
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        using (new FMDB())
        {
            Models.RoomOverlayPackage dbnew = new()
            {
                Id = Id,
                RevisionNumber =
                    FMDB.Context.RoomOverlayPackages.Where(x => x.Id == Id)
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
            Models.RoomOverlayPackage dbold = FMDB.Context.RoomOverlayPackages
                            .Include(x => x.RoomOverlays)
                            .ThenInclude(x => x.RoomOverlaysExits)
                            .ThenInclude(cellOverlayExit => cellOverlayExit.Exit)
                            .Include(cellOverlayPackage => cellOverlayPackage.RoomOverlays)
                            .ThenInclude(cellOverlay => cellOverlay.Room)
                            .Include(cellOverlayPackage => cellOverlayPackage.RoomOverlays)
                            .ThenInclude(cellOverlay => cellOverlay.HearingProfile)
                            .Include(cellOverlayPackage => cellOverlayPackage.RoomOverlays)
                            .ThenInclude(cellOverlay => cellOverlay.Terrain)
                            .First(x => x.Id == Id && x.RevisionNumber == RevisionNumber);
            foreach (Models.RoomOverlay overlay in dbold.RoomOverlays)
            {
                Models.RoomOverlay newOverlay = new()
                {
                    AddedLight = overlay.AddedLight,
                    AmbientLightFactor = overlay.AmbientLightFactor,
                    AtmosphereId = overlay.AtmosphereId,
                    AtmosphereType = overlay.AtmosphereType,
                    Room = overlay.Room,
                    RoomDescription = overlay.RoomDescription,
                    RoomName = overlay.RoomName,
                    RoomOverlayPackage = dbnew,
                    HearingProfile = overlay.HearingProfile,
                    Name = overlay.Name,
                    OutdoorsType = overlay.OutdoorsType,
                    SafeQuit = overlay.SafeQuit,
                    Terrain = overlay.Terrain
                };
                foreach (RoomOverlayExit exit in overlay.RoomOverlaysExits)
                {
                    newOverlay.RoomOverlaysExits.Add(new RoomOverlayExit
                    {
                        RoomOverlay = newOverlay,
                        Exit = exit.Exit
                    });
                }

                dbnew.RoomOverlays.Add(newOverlay);
                FMDB.Context.RoomOverlays.Add(newOverlay);
            }

            FMDB.Context.EditableItems.Add(dbnew.EditableItem);
            FMDB.Context.RoomOverlayPackages.Add(dbnew);
            FMDB.Context.SaveChanges();
            RoomOverlayPackage package = new(dbnew, Gameworld);
            Gameworld.Add(package);
            foreach (Models.RoomOverlay overlay in dbnew.RoomOverlays)
            {
                IRoom room = Gameworld.Rooms.Get(overlay.RoomId);
                if (room is null)
                {
                    continue;
                }
                room.AddOverlay(new RoomOverlay(overlay, room, Gameworld));
            }

            return package;
        }
    }

    protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
    {
        return Gameworld.RoomOverlayPackages.GetAll(Id);
    }

    public override string EditHeader()
    {
        return $"Room Overlay Package: \"{Name}\" (ID #{Id} Rev {RevisionNumber})";
    }

    public override void Save()
    {
        using (new FMDB())
        {
            Models.RoomOverlayPackage dbpack = FMDB.Context.RoomOverlayPackages.Find(Id, RevisionNumber);
            dbpack.Name = Name;

            if (_statusChanged)
            {
                base.Save(dbpack.EditableItem);
            }

            FMDB.Context.SaveChanges();
        }

        Changed = false;
    }

    public void LoadFromDatabase(MudSharp.Models.RoomOverlayPackage package)
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
