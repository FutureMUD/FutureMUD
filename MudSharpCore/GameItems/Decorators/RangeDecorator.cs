using System;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.GameItems.Decorators;

public class RangeDecorator : StackDecorator
{
	protected RankedRange<string> _quantities;

	public RangeDecorator(MudSharp.Models.StackDecorator proto)
	{
		_id = proto.Id;
		_name = proto.Name;
		Description = proto.Description;
		_quantities = new RankedRange<string>();
		var root = XElement.Parse(proto.Definition);
		foreach (var sub in root.Elements("Range"))
		{
			_quantities.Add(sub.Attribute("Item").Value, Convert.ToDouble(sub.Attribute("Min").Value),
				Convert.ToDouble(sub.Attribute("Max").Value));
		}
	}

	public override string FrameworkItemType => "RangeDecorator";

	#region IStackDecorator Members

	public override string Describe(string name, string description, double quantity)
	{
		return _quantities.Find(quantity) + description;
	}

	#endregion
}