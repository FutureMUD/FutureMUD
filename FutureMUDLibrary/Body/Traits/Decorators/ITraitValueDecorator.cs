using MudSharp.Character;
using MudSharp.Framework;
using System.Collections.Generic;

namespace MudSharp.Body.Traits.Decorators
{
    public interface ITraitValueDecorator : IFrameworkItem
    {
        string Decorate(ITrait trait);
        string Decorate(double value);
        IEnumerable<string> OrderedDescriptors { get; }
        IEnumerable<(double Value, string Descriptor)> OrderedDescriptorsWithThresholds { get; }
        string Show(ICharacter actor);
    }

    public interface IRangeTraitValueDecorator : ITraitValueDecorator
    {
        double MinimumValueForDescriptor(string descriptor);
    }
}