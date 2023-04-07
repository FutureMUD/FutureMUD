using System.Collections.Generic;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Body.Traits.Decorators;

public class SimpleNumericDecorator : FrameworkItem, ITraitValueDecorator
{
	public SimpleNumericDecorator(Models.TraitDecorator decorator)
	{
		_id = decorator.Id;
		_name = decorator.Name;
	}

	public override string FrameworkItemType => "TraitDecorator";

	public string Decorate(ITrait trait)
	{
		return (trait.Owner?.TraitValue(trait.Definition) ?? trait.Value).ToString("F0");
	}

	public string Decorate(double value)
	{
		return value.ToString("F0");
	}

	public IEnumerable<string> OrderedDescriptors => new[] { "No Levels - Shows Actual Numbers" };

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Simple Numeric Decorator #{Id.ToString("N0", actor)}".ColourName());
		sb.AppendLine();
		sb.AppendLine($"* Just displays the numerical value of the trait.");
		return sb.ToString();
	}
}