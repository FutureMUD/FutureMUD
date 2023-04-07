using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Body.Traits.Decorators {
    public interface ITraitValueDecorator : IFrameworkItem {
        string Decorate(ITrait trait);
        string Decorate(double value);
        IEnumerable<string> OrderedDescriptors { get; }
        string Show(ICharacter actor);
    }

    public interface IRangeTraitValueDecorator : ITraitValueDecorator
    {
        double MinimumValueForDescriptor(string descriptor);
    }
}