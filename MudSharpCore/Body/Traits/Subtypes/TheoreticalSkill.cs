﻿using System;
using System.Collections.Generic;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Database;
using MudSharp.Logging;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Subtypes;

public class TheoreticalSkill : Trait
{
	public TheoreticalSkill(TheoreticalSkillDefinition defintion, MudSharp.Models.Trait trait, IHaveTraits owner)
		: base(trait, owner)
	{
		_definition = defintion;
		PracticalValue = trait.Value;
		TheoreticalValue = trait.AdditionalValue;
	}

	#region Overrides of Item

	protected IImprovementModel Improver => _definition.Improver;

	#endregion

	#region Overrides of Trait

	public double PracticalValue { get; private set; }

	public double TheoreticalValue { get; private set; }

	private readonly TheoreticalSkillDefinition _definition;
	public override ITraitDefinition Definition => _definition;

	public override double Value
	{
		get
		{
			_definition.ValueExpression.Parameters["theory"] = TheoreticalValue;
			_definition.ValueExpression.Parameters["practical"] = PracticalValue;
			return Convert.ToDouble(_definition.ValueExpression.Evaluate());
		}
		set
		{
			var oldValue = Value;
			PracticalValue = value;
			TheoreticalValue = value;
			TraitChanged(oldValue, Value);
			Changed = true;
		}
	}

	public override bool TraitUsed(IHaveTraits user, Outcome result, Difficulty difficulty, TraitUseType usetype, IEnumerable<Tuple<string, double>> bonuses)
	{
		Gameworld.LogManager.CustomLogEntry(LogEntryType.SkillUse, user, Definition, result, difficulty, usetype, bonuses);
		var oldValue = Value;
		switch (usetype)
		{
			case TraitUseType.Practical:
				PracticalValue += Improver.GetImprovement(user, this, difficulty, result, usetype);
				break;
			case TraitUseType.Theoretical:
				TheoreticalValue += Improver.GetImprovement(user, this, difficulty, result, usetype);
				break;
		}

		Changed = true;
		TraitChanged(oldValue, Value);
		return oldValue != Value;
	}

	public override void Save()
	{
		if (Owner == null || Definition == null)
		{
			Changed = false;
			return;
		}

		var dbtrait = FMDB.Context.Traits.Find(_owner.Id, Definition.Id);
		if (dbtrait is null)
		{
			dbtrait = new Models.Trait
			{
				BodyId = Owner.Id,
				TraitDefinitionId = Definition.Id,
				Value = 0.0,
				AdditionalValue = 0.0
			};
			FMDB.Context.Traits.Add(dbtrait);
		}
		dbtrait.Value = PracticalValue;
		dbtrait.AdditionalValue = TheoreticalValue;
		Changed = false;
	}

	#endregion
}