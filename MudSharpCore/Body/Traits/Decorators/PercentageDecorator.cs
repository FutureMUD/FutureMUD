using MudSharp.Character;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Body.Traits.Decorators;

public class PercentageDecorator : FrameworkItem, ITraitValueDecorator
{
    public PercentageDecorator(Models.TraitDecorator decorator)
    {
        _id = decorator.Id;
        _name = decorator.Name;
    }

    public override string FrameworkItemType => "TraitDecorator";

    #region ITraitValueDecorator Members

    public string Decorate(ITrait trait)
    {
        double value = trait.Owner?.TraitValue(trait.Definition, TraitBonusContext.None) ?? trait.Value;
        double max = trait.Owner?.TraitMaxValue(trait) ?? trait.MaxValue;
        if (max == 0.0)
        {
            max = 1.0;
        }

        return (value / max).ToString("P0");
    }

    public string Decorate(double value)
    {
        return $"{value:F0}";
    }

    public IEnumerable<string> OrderedDescriptors => new[] { "No Levels - Shows Percentages" };

    public string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Percentage Decorator #{Id.ToString("N0", actor)}".ColourName());
        sb.AppendLine();
        sb.AppendLine($"* Just displays the percentage value of the trait compared to its maximum.");
        return sb.ToString();
    }

    #endregion
}