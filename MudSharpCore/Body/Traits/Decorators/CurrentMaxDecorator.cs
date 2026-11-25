
namespace MudSharp.Body.Traits.Decorators;

public class CurrentMaxDecorator : FrameworkItem, ITraitValueDecorator
{
    private static CurrentMaxDecorator _instance;

	public CurrentMaxDecorator(Models.TraitDecorator decorator, bool setAsSingleton = true)
    {
        _id = decorator.Id;
        _name = decorator.Name;
		if (setAsSingleton)
		{
			_instance = this;
		}
    }

    private CurrentMaxDecorator()
    {
        _name = "Auto-Generated Current Max";
        _instance = this;
    }

    public static CurrentMaxDecorator Instance => _instance ?? new CurrentMaxDecorator();

    public override string FrameworkItemType => "TraitDecorator";

    #region ITraitValueDecorator Members

    public string Decorate(ITrait trait) => Decorate(trait, trait.Owner);

    public string Decorate(ITrait trait, IHaveTraits subject)
    {
        double currentValue = subject?.TraitValue(trait.Definition) ?? trait.Value;
        double rawValue = subject?.TraitRawValue(trait.Definition) ?? trait.RawValue;
        double maxValue = subject?.TraitMaxValue(trait) ?? trait.MaxValue;
        if (currentValue != rawValue)
        {
            return $"{rawValue:F0}[{currentValue - rawValue:F0}] / {maxValue:F0}";
        }

        return currentValue.ToString("F0") + " / " + maxValue.ToString("F0");
    }

    public string Decorate(double value)
    {
        return $"{value:F0}";
    }

    public IEnumerable<string> OrderedDescriptors => new[] { "No Levels - Shows Actual Numbers" };

	public IEnumerable<(double Value, string Descriptor)> OrderedDescriptorsWithThresholds => [];

	public string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Current/Max Decorator #{Id.ToString("N0", actor)}".ColourName());
        sb.AppendLine();
        sb.AppendLine($"* Just displays the numerical value of the trait and the maximum value, separated by a slash.");
        return sb.ToString();
    }

    #endregion
}
