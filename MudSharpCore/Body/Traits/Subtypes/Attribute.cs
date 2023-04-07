namespace MudSharp.Body.Traits.Subtypes;

public class Attribute : Trait, IAttribute
{
	protected AttributeDefinition _definition;
	public IAttributeDefinition AttributeDefinition => _definition;

	public Attribute(AttributeDefinition def, double value, bool nonSaving = false)
	{
		_definition = def;
		_value = value;
		_nonSaving = nonSaving;
	}

	public Attribute(AttributeDefinition definition, MudSharp.Models.Trait trait, IHaveTraits owner)
		: base(trait, owner)
	{
		_definition = definition;
	}

	public Attribute(AttributeDefinition definition, double value, IHaveTraits owner)
	{
		_owner = owner;
		_value = value;
		_definition = definition;
	}

	public override ITraitDefinition Definition => _definition;
}