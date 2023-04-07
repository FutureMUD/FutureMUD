using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Body.Traits.Subtypes;

public class DerivedAttribute : DerivedTrait, IAttribute
{
	private readonly DerivedAttributeDefinition _attributeDefinition;
	public IAttributeDefinition AttributeDefinition => _attributeDefinition;

	public DerivedAttribute(DerivedAttributeDefinition definition, Models.Trait trait, IHaveTraits owner) : base(
		definition, trait, owner)
	{
		_attributeDefinition = definition;
	}

	public DerivedAttribute(DerivedAttributeDefinition definition, IHaveTraits owner) : base(definition, owner)
	{
		_attributeDefinition = definition;
	}
}