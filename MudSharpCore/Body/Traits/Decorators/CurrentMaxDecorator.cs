using System.Collections.Generic;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Body.Traits.Decorators;

public class CurrentMaxDecorator : FrameworkItem, ITraitValueDecorator
{
	private static CurrentMaxDecorator _instance;

	public CurrentMaxDecorator(Models.TraitDecorator decorator)
	{
		_id = decorator.Id;
		_name = decorator.Name;
		_instance = this;
	}

	private CurrentMaxDecorator()
	{
		_name = "Auto-Generated Current Max";
		_instance = this;
	}

	public static CurrentMaxDecorator Instance => _instance ?? new CurrentMaxDecorator();

	public override string FrameworkItemType => "TraitDecorator";

	#region ITraitValueDecorator Members

	public string Decorate(ITrait trait)
	{
		var currentValue = trait.Owner?.TraitValue(trait.Definition) ?? trait.Value;
		var rawValue = trait.Owner?.TraitRawValue(trait.Definition) ?? trait.RawValue;
		var maxValue = trait.Owner?.TraitMaxValue(trait) ?? trait.MaxValue;
		if (currentValue != rawValue)
		{
			return $"{rawValue:F0}[{currentValue - rawValue:F0}] / {trait.MaxValue:F0}";
		}

		return currentValue.ToString("F0") + " / " + maxValue.ToString("F0");
	}

	public string Decorate(double value)
	{
		return $"{value:F0}";
	}

	public IEnumerable<string> OrderedDescriptors => new[] { "No Levels - Shows Actual Numbers" };

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Current/Max Decorator #{Id.ToString("N0", actor)}".ColourName());
		sb.AppendLine();
		sb.AppendLine($"* Just displays the numerical value of the trait and the maximum value, separated by a slash.");
		return sb.ToString();
	}

	#endregion
}