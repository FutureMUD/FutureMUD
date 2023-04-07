using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Body.Traits.Decorators;

public abstract class DecoratorBase : FrameworkItem, ITraitValueDecorator
{
	public abstract string Decorate(ITrait trait);

	public abstract string Decorate(double value);

	public abstract IEnumerable<string> OrderedDescriptors { get; }

	public abstract string Show(ICharacter actor);

	public static ITraitValueDecorator GetDecorator(TraitDecorator decorator)
	{
		switch (decorator.Type)
		{
			case "SimpleNumeric":
				return new SimpleNumericDecorator(decorator);
			case "CurrentMax":
				return new CurrentMaxDecorator(decorator);
			case "Percentage":
				return new PercentageDecorator(decorator);
			case "Range":
				return new RangeDecorator(decorator);
			default:
				return null;
		}
	}
}