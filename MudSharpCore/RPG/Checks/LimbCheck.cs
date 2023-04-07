using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.RPG.Checks;

public class LimbCheck : StandardCheck
{
	private readonly HashSet<LimbRequirement> _limbRequirements = new();

	public LimbCheck(Check check, IFuturemud game) : base(check, game)
	{
		LoadFromXml(XElement.Parse(check.CheckTemplate.Definition));
	}

	private void LoadFromXml(XElement element)
	{
		foreach (var item in element.Elements("Limbs"))
		{
			_limbRequirements.Add(new LimbRequirement(item, Gameworld));
		}
	}

	public class LimbRequirement
	{
		public LimbRequirement(XElement element, IFuturemud gameworld)
		{
			Limb = (LimbType)int.Parse(element.Element("Limb")?.Value ?? "0");
			Minimum = int.Parse(element.Element("Minimum")?.Value ?? "0");
		}

		public LimbType Limb { get; }

		public int Minimum { get; }

		#region Overrides of Object

		/// <summary>
		///     Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <returns>
		///     true if the specified object  is equal to the current object; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			if (obj is LimbRequirement objLimbReq)
			{
				return objLimbReq.Limb.Equals(Limb) && objLimbReq.Minimum.Equals(Minimum);
			}

			return false;
		}

		#region Overrides of Object

		/// <summary>
		///     Serves as the default hash function.
		/// </summary>
		/// <returns>
		///     A hash code for the current object.
		/// </returns>
		public override int GetHashCode()
		{
			return Limb.GetHashCode() + Minimum.GetHashCode();
		}

		#endregion

		#endregion
	}

	#region Overrides of StandardCheck

	public override Tuple<CheckOutcome, CheckOutcome> MultiDifficultyCheck(IPerceivableHaveTraits checkee,
		Difficulty difficulty1, Difficulty difficulty2, IPerceivable target = null, ITraitDefinition trait = null,
		double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		if (checkee is not IHaveABody checkeeHaveBody)
		{
			return Tuple.Create(
				new CheckOutcome
				{
					Outcome = Outcome.MajorFail,
					AcquiredTraits = Enumerable.Empty<ITraitDefinition>()
				},
				new CheckOutcome
				{
					Outcome = Outcome.MajorFail,
					AcquiredTraits = Enumerable.Empty<ITraitDefinition>()
				}
			);
		}

		if (_limbRequirements.Any(requirement => requirement.Minimum > checkeeHaveBody.Body.Limbs
			    .Where(x => x.LimbType == requirement.Limb)
			    .Select(x => checkeeHaveBody.Body.CanUseLimb(x))
			    .Count(x => x == CanUseLimbResult.CanUse)))
		{
			return Tuple.Create(
				new CheckOutcome
				{
					Outcome = Outcome.MajorFail,
					AcquiredTraits = Enumerable.Empty<ITraitDefinition>()
				},
				new CheckOutcome
				{
					Outcome = Outcome.MajorFail,
					AcquiredTraits = Enumerable.Empty<ITraitDefinition>()
				}
			);
		}

		return base.MultiDifficultyCheck(checkee, difficulty1, difficulty2, target, trait, externalBonus, traitUseType,
			customParameters);
	}

	public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
		IPerceivable target = null,
		IUseTrait tool = null, double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		if (checkee is not IHaveABody checkeeHaveBody)
		{
			return new CheckOutcome
			{
				Outcome = Outcome.MajorFail,
				AcquiredTraits = Enumerable.Empty<ITraitDefinition>()
			};
		}

		if (_limbRequirements.Any(requirement => requirement.Minimum > checkeeHaveBody.Body.Limbs
			    .Where(x => x.LimbType == requirement.Limb)
			    .Select(x => checkeeHaveBody.Body.CanUseLimb(x))
			    .Count(x => x == CanUseLimbResult.CanUse)))
		{
			return new CheckOutcome
			{
				Outcome = Outcome.MajorFail,
				AcquiredTraits = Enumerable.Empty<ITraitDefinition>()
			};
		}

		return base.Check(checkee, difficulty, target, tool, externalBonus, traitUseType, customParameters);
	}

	public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
		ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		if (checkee is not IHaveABody checkeeHaveBody)
		{
			return new CheckOutcome
			{
				Outcome = Outcome.MajorFail,
				AcquiredTraits = Enumerable.Empty<ITraitDefinition>()
			};
		}

		if (_limbRequirements.Any(requirement => requirement.Minimum > checkeeHaveBody.Body.Limbs
			    .Where(x => x.LimbType == requirement.Limb)
			    .Select(x => checkeeHaveBody.Body.CanUseLimb(x))
			    .Count(x => x == CanUseLimbResult.CanUse)))
		{
			return new CheckOutcome
			{
				Outcome = Outcome.MajorFail,
				AcquiredTraits = Enumerable.Empty<ITraitDefinition>()
			};
		}

		return base.Check(checkee, difficulty, trait, target, externalBonus, traitUseType, customParameters);
	}

	#endregion
}