using MudSharp.Models;

namespace MudSharp.Body.Traits.Decorators;

public abstract class DecoratorBase : FrameworkItem, ITraitValueDecorator
{
    public abstract string Decorate(ITrait trait);

    public abstract string Decorate(double value);

    public abstract IEnumerable<string> OrderedDescriptors { get; }

    public virtual IEnumerable<(double Value, string Descriptor)> OrderedDescriptorsWithThresholds => [];

	public abstract string Show(ICharacter actor);

	public static ITraitValueDecorator GetDecorator(TraitDecorator decorator, bool setAsCurrentMaxSingleton = true)
    {
        switch (decorator.Type)
        {
            case "SimpleNumeric":
                return new SimpleNumericDecorator(decorator);
            case "CurrentMax":
				return new CurrentMaxDecorator(decorator, setAsCurrentMaxSingleton);
            case "Percentage":
                return new PercentageDecorator(decorator);
            case "Range":
                return new RangeDecorator(decorator);
            default:
                return null;
        }
    }
}
