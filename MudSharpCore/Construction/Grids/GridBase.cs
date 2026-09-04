using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Construction.Grids;

public abstract class GridBase : LateInitialisingItem, IGrid
{
    public sealed override string FrameworkItemType => "Grid";
    public override InitialisationPhase InitialisationPhase => InitialisationPhase.First;


    private readonly List<long> _locationIds = new();
    private readonly List<ICell> _locations = new();
    public IEnumerable<ICell> Locations => _locations;

    protected GridBase(Models.Grid grid, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _id = grid.Id;
        IdInitialised = true;
        XElement root = XElement.Parse(grid.Definition);
        foreach (XElement element in root.Elements("Location"))
        {
            _locationIds.Add(long.Parse(element.Value));
        }
    }

    protected GridBase(IFuturemud gameworld, ICell initialLocation)
    {
        Gameworld = gameworld;
        Gameworld.SaveManager.AddInitialisation(this);
        if (initialLocation != null)
        {
            _locations.Add(initialLocation);
            initialLocation.CellRequestsDeletion -= Location_CellRequestsDeletion;
            initialLocation.CellRequestsDeletion += Location_CellRequestsDeletion;
        }
    }

    protected GridBase(IGrid rhs)
    {
        Gameworld = rhs.Gameworld;
        Gameworld.SaveManager.AddInitialisation(this);
        _locations.AddRange(rhs.Locations);
        foreach (ICell location in _locations)
        {
            location.CellRequestsDeletion -= Location_CellRequestsDeletion;
            location.CellRequestsDeletion += Location_CellRequestsDeletion;
        }
    }

    public void ExtendTo(ICell cell)
    {
        _locations.Add(cell);
        cell.CellRequestsDeletion -= Location_CellRequestsDeletion;
        cell.CellRequestsDeletion += Location_CellRequestsDeletion;
        Changed = true;
    }

    public virtual void WithdrawFrom(ICell cell)
    {
        _locations.Remove(cell);
        cell.CellRequestsDeletion -= Location_CellRequestsDeletion;
        Changed = true;
    }

    public virtual void LoadTimeInitialise()
    {
        _locations.AddRange(_locationIds.Select(x => Gameworld.Cells.Get(x)));
        _locationIds.Clear();
        foreach (ICell location in _locations)
        {
            location.CellRequestsDeletion -= Location_CellRequestsDeletion;
            location.CellRequestsDeletion += Location_CellRequestsDeletion;
        }
    }

    private void Location_CellRequestsDeletion(object sender, EventArgs e)
    {
        ICell cell = (ICell)sender;
        _locations.Remove(cell);
        Changed = true;
        cell.CellRequestsDeletion -= Location_CellRequestsDeletion;
    }

    public void Delete()
    {
        Gameworld.SaveManager.Abort(this);
        if (_id != 0)
        {
            using (new FMDB())
            {
                Gameworld.SaveManager.Flush();
                Grid dbitem = FMDB.Context.Grids.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.Grids.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }
            }
        }
    }

    protected virtual XElement SaveDefinition()
    {
        return new XElement("Grid",
            from location in Locations
            select new XElement("Location", location.Id)
        );
    }

    public override void Save()
    {
        Grid dbitem = FMDB.Context.Grids.Find(Id);
        dbitem.Definition = SaveDefinition().ToString();
        Changed = false;
    }

	public abstract string GridType { get; }

	public ProgVariableTypes Type => ProgVariableTypes.Grid;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		return property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(Id),
			"name" => new TextVariable(Name),
			"gridtype" => new TextVariable(GridType),
			"locations" => new CollectionVariable(Locations.ToList(), ProgVariableTypes.Location),
			_ => throw new NotSupportedException($"Unsupported grid property {property}.")
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Grid,
			new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = ProgVariableTypes.Number,
				["name"] = ProgVariableTypes.Text,
				["gridtype"] = ProgVariableTypes.Text,
				["locations"] = ProgVariableTypes.Location | ProgVariableTypes.Collection
			},
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = "The stable grid identity.",
				["name"] = "The grid name.",
				["gridtype"] = "The concrete grid implementation type.",
				["locations"] = "The cells that currently belong to this grid."
			});
	}

	public abstract string Show(ICharacter actor);

    public override object DatabaseInsert()
    {
        Grid dbitem = new()
        {
            GridType = GridType,
            Definition = SaveDefinition().ToString()
        };
        FMDB.Context.Grids.Add(dbitem);
        return dbitem;
    }

    public override void SetIDFromDatabase(object dbitem)
    {
        _id = ((Models.Grid)dbitem).Id;
    }
}
