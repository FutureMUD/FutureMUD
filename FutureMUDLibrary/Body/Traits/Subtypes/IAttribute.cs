using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Body.Traits.Subtypes
{
    public interface IAttribute : ITrait
    {
        IAttributeDefinition AttributeDefinition { get; }
    }
}
