using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Framework;

namespace MudSharp.Body.Traits;

public static class TraitFactory
{
	public static IAttribute LoadAttribute(IAttributeDefinition definition, IHaveTraits owner, double value)
	{
		switch (definition)
		{
			case AttributeDefinition ad:
				return new Subtypes.Attribute(ad, value, owner);
			case DerivedAttributeDefinition dad:
				return new DerivedAttribute(dad, owner);
		}

		throw new ArgumentOutOfRangeException(nameof(definition), definition.TraitType.DescribeEnum(),
			"Non-attribute type of trait asked to load");
	}

	public static ISkill LoadSkill(ISkillDefinition definition, IHaveTraits owner, double value)
	{
		switch (definition)
		{
			case SkillDefinition skill:
				return new Skill(skill, value, owner);
			case DerivedSkillDefinition dskill:
				return new DerivedSkill(dskill, owner);
		}

		throw new ArgumentOutOfRangeException(nameof(definition), definition.TraitType.DescribeEnum(),
			"Non-skill type of trait asked to load");
	}
}