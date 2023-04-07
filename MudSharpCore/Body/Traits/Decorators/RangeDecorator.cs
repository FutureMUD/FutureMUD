using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Body.Traits.Decorators;

public class RangeDecorator : FrameworkItem, IRangeTraitValueDecorator
{
	public RangeDecorator(RankedRange<string> ranges, string prefix, string suffix, bool colourWhenCapped,
		bool colourWhenBuffed, bool colourWhenDefault)
	{
		Ranges = ranges;
		Suffix = suffix;
		Prefix = prefix;
		ColourWhenCapped = colourWhenCapped;
		ColourWhenBuffed = colourWhenBuffed;
		ColourWhenDefault = colourWhenDefault;
	}

	public RangeDecorator(TraitDecorator decorator)
	{
		_id = decorator.Id;
		Ranges = new RankedRange<string>();
		var root = XElement.Parse(decorator.Contents);
		_name = root.Attribute("name").Value;
		Prefix = root.Attribute("prefix").Value;
		Suffix = root.Attribute("suffix").Value;
		ColourWhenCapped = bool.Parse(root.Attribute("colour_capped").Value);
		ColourWhenBuffed = bool.Parse(root.Attribute("colour_buffed")?.Value ?? "true");
		ColourWhenDefault = bool.Parse(root.Attribute("colour_default")?.Value ?? "true");

		foreach (var element in root.Elements("range"))
		{
			Ranges.Add(element.Attribute("text").Value, int.Parse(element.Attribute("low").Value),
				int.Parse(element.Attribute("high").Value));
		}
	}

	protected RankedRange<string> Ranges { get; set; }
	protected string Prefix { get; set; }
	protected string Suffix { get; set; }
	protected bool ColourWhenCapped { get; set; }
	protected bool ColourWhenBuffed { get; set; }
	protected bool ColourWhenDefault { get; set; }
	public override string FrameworkItemType => "TraitDecorator";

	#region ITraitValueDecorator Members

	public string Decorate(ITrait trait)
	{
		var currentValue = trait.Owner?.TraitValue(trait.Definition) ?? trait.Value;
		var maxValue = trait.Owner?.TraitMaxValue(trait) ?? trait.MaxValue;
		var rawValue = trait.Owner?.TraitRawValue(trait.Definition) ?? trait.Value;
		var capped = rawValue >= maxValue;
		var buffed = currentValue > rawValue;
		var penalised = currentValue <= rawValue && currentValue <= maxValue;
		ANSIColour colour = null;
		if (capped && buffed && ColourWhenBuffed && ColourWhenCapped)
		{
			colour = Telnet.Magenta;
		}
		else if (capped && penalised && ColourWhenBuffed && ColourWhenCapped)
		{
			colour = Telnet.BoldYellow;
		}
		else if (capped && ColourWhenCapped)
		{
			colour = Telnet.Red;
		}
		else if (buffed && ColourWhenBuffed)
		{
			colour = Telnet.BoldGreen;
		}
		else if (penalised && ColourWhenBuffed)
		{
			colour = Telnet.Yellow;
		}
		else if (ColourWhenDefault)
		{
			colour = Telnet.Green;
		}

		return
			$"{Prefix}{(colour != null ? Ranges.Find(currentValue).Colour(colour) : Ranges.Find(currentValue))}{Suffix}";
	}

	public string Decorate(double value)
	{
		return $"{Prefix}{Ranges.Find(value)}{Suffix}";
	}

	public IEnumerable<string> OrderedDescriptors => Ranges.Ranges.OrderBy(x => x.LowerBound).Select(x => x.Value);

	public double MinimumValueForDescriptor(string descriptor)
	{
		return Ranges.Ranges.FirstOrDefault(x => x.Value.EqualTo(descriptor))?.LowerBound ?? 0.0;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Range Decorator #{Id.ToString("N0", actor)} - {Name}".ColourName());
		sb.AppendLine();
		sb.AppendLine($"Prefix: {Prefix.ColourCommand()}");
		sb.AppendLine($"Suffix: {Suffix.ColourCommand()}");
		sb.AppendLine($"Colour When Capped: {ColourWhenCapped.ToColouredString()}");
		sb.AppendLine($"Colour When Buffed: {ColourWhenBuffed.ToColouredString()}");
		sb.AppendLine($"Colour By Default: {ColourWhenDefault.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine($"Descriptors:");
		foreach (var range in Ranges.Ranges.OrderBy(x => x.LowerBound))
		{
			sb.AppendLine(
				$"\t{range.LowerBound.ToString("N2", actor)} - {range.UpperBound.ToString("N2", actor)}: {range.Value.ColourValue()}");
		}

		return sb.ToString();
	}

	#endregion
}