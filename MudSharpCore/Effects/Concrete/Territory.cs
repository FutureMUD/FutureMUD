using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class Territory : Effect, IEffectSubtype
{
	private readonly List<(ICell Cell, HashSet<string> Flags)> _cells = new();
	public IEnumerable<ICell> Cells => _cells.Select(x => x.Cell);

	public void AddCell(ICell cell)
	{
		if (!_cells.Any(x => x.Cell == cell))
		{
			_cells.Add((cell, new HashSet<string>()));
			Changed = true;
		}
	}

	public void RemoveCell(ICell cell)
	{
		_cells.RemoveAll(x => x.Cell == cell);
		Changed = true;
	}

	public void TagCell(ICell cell, string tag)
	{
		if (!_cells.Any(x => x.Cell == cell))
		{
			return;
		}

		_cells.First(x => x.Cell == cell).Flags.Add(tag.ToLowerInvariant());
		Changed = true;
	}

	public void UntagCell(ICell cell, string tag)
	{
		if (!_cells.Any(x => x.Cell == cell))
		{
			return;
		}

		_cells.First(x => x.Cell == cell).Flags.Remove(tag.ToLowerInvariant());
		Changed = true;
	}

	public bool HasFlag(ICell cell, string flag)
	{
		flag = flag.ToLowerInvariant();
		return _cells.Any(x => x.Cell == cell && x.Flags.Contains(flag));
	}

	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("Territory", (effect, owner) => new Territory(effect, owner));
	}

	#endregion

	#region Constructors

	public Territory(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	protected Territory(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXML(effect.Element("Effect"));
	}

	#endregion

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect",
				from cell in _cells
				select new XElement("Cell", new XAttribute("id", cell.Cell.Id),
					from flag in cell.Flags
					select new XElement("Flag", new XCData(flag)))
			);
	}

	protected void LoadFromXML(XElement root)
	{
		foreach (var item in root.Elements("Cell"))
		{
			var cell = Gameworld.Cells.Get(long.Parse(item.Attribute("id").Value));
			if (cell == null)
			{
				Changed = true;
				continue;
			}

			var tuple = (cell, flags: new HashSet<string>());
			foreach (var flag in item.Elements("Flag"))
			{
				tuple.flags.Add(flag.Value);
			}
		}
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "Territory";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Has territory [{_cells.Select(x => x.Cell.Id.ToString("F0")).ListToCommaSeparatedValues(" ")}]";
	}

	public override bool SavingEffect => true;

	#endregion
}