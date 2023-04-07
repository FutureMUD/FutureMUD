using System;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.GameItems.Decorators;

public class PileDecorator : StackDecorator
{
	protected RankedRange<string> _quantities;

	public PileDecorator(MudSharp.Models.StackDecorator proto)
	{
		_id = proto.Id;
		_name = proto.Name;
		Description = proto.Description;
		_quantities = new RankedRange<string>();
		var root = XElement.Parse(proto.Definition);
		foreach (var sub in root.Elements("Range"))
		{
			_quantities.Add(sub.Attribute("Item").Value, Convert.ToInt32(sub.Attribute("Min").Value),
				Convert.ToInt32(sub.Attribute("Max").Value));
		}
	}

	public override string FrameworkItemType => "PileDecorator";

	#region IStackDecorator Members

	public override string Describe(string name, string description, double quantity)
	{
		var iquantity = (int)quantity;
		if (!string.IsNullOrEmpty(name) && iquantity != 1)
		{
			description = description.Replace(name, name.Pluralise());
		}

		return iquantity == 1 ? description.A_An() : _quantities.Find(iquantity) + description;
	}

	#endregion
}