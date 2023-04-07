using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Body.Traits.Subtypes;

public class DerivedSkill : DerivedTrait, ISkill
{
	public DerivedSkill(DerivedSkillDefinition def) : base(def)
	{
	}

	public DerivedSkill(DerivedSkillDefinition definition, Models.Trait trait, IHaveTraits owner) : base(definition,
		trait, owner)
	{
	}

	public DerivedSkill(DerivedSkillDefinition definition, IHaveTraits owner) : base(definition, owner)
	{
	}

	#region Implementation of ISkill

	public ISkillDefinition SkillDefinition => (ISkillDefinition)Definition;

	#endregion
}