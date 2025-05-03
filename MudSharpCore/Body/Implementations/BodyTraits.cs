using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.GameItems.Interfaces;
using MudSharp.Work.Projects.Impacts;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	#region IHaveTraits Members

	protected readonly List<ITrait> _traits = new();

	public double TraitValue(ITraitDefinition definition, TraitBonusContext context = TraitBonusContext.None)
	{
		var trait = _traits.FirstOrDefault(x => x.Definition == definition);
		var baseValue = trait?.Value ?? 0.0;
		baseValue +=
			Merits.OfType<ITraitBonusMerit>().Where(x => x.Applies(Actor))
			      .Sum(x => x.BonusForTrait(definition, context));
		baseValue +=
			Actor.Merits.OfType<ITraitBonusMerit>()
			     .Where(x => x.Applies(Actor))
			     .Sum(x => x.BonusForTrait(definition, context));
		baseValue +=
			EffectHandler.EffectsOfType<ITraitBonusEffect>()
			             .Where(x => x.Applies(Actor))
						 .Where(x => x.AppliesToTrait(trait))
			             .Sum(x => x.GetBonus(trait));
		baseValue +=
			Actor
				.EffectsOfType<ITraitBonusEffect>()
				.Where(x => x.Applies(Actor))
				.Where(x => x.AppliesToTrait(trait))
				.Sum(x => x.GetBonus(trait));
		baseValue += Implants.OfType<IImplantTraitChange>().Sum(x => x.BonusForTrait(trait, context));
		baseValue += ExternalItems.SelectNotNull(x => x.GetItemType<IChangeTraitsInInventory>())
		                          .Sum(x => x.BonusForTrait(definition, context));
		baseValue += Actor.CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(Actor))
		                  .OfType<ILabourImpactTraits>().Sum(x => x.EffectOnTrait(trait, context)) ?? 0.0;
		return baseValue;
	}

	public bool AddTrait(ITraitDefinition trait, double value)
	{
		if (_traits.Any(x => x.Definition == trait))
		{
			return false;
		}

		_traits.Add(trait.NewTrait(this, value));
		Changed = true;
		foreach (var language in Gameworld.Languages.Where(x => x.LinkedTrait == trait))
		{
			Actor.LearnLanguage(language);
			Actor.LearnAccent(language.DefaultLearnerAccent);
		}

		return true;
	}

	public bool RemoveTrait(ITraitDefinition trait)
	{
		if (_traits.All(x => x.Definition != trait))
		{
			return false;
		}

		_traits.RemoveAll(x => x.Definition == trait);
		Changed = true;
		foreach (var language in Languages.Where(x => x.LinkedTrait == trait).ToList())
		{
			ForgetLanguage(language);
		}

		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbtrait = FMDB.Context.Traits.Find(Id, trait.Id);
			if (dbtrait != null)
			{
				FMDB.Context.Traits.Remove(dbtrait);
				FMDB.Context.SaveChanges();
			}
		}

		return true;
	}

	public bool SetTraitValue(ITraitDefinition trait, double value)
	{
		var bodyTrait = _traits.FirstOrDefault(x => x.Definition == trait);
		if (bodyTrait == null)
		{
			AddTrait(trait, value);
			return true;
		}

		bodyTrait.Value = value;
		Changed = true;
		return true;
	}

	public double TraitRawValue(ITraitDefinition trait)
	{
		return _traits.FirstOrDefault(x => x.Definition == trait)?.Value ?? 0.0;
	}

	public double TraitMaxValue(ITraitDefinition trait)
	{
		return TraitMaxValue(_traits.FirstOrDefault(x => x.Definition == trait));
	}

	public double TraitMaxValue(ITrait trait)
	{
		if (trait is null)
		{
			return 0.0;
		}

		var baseValue = trait.MaxValue;
		baseValue += Actor.CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(Actor))
		                  .OfType<ILabourImpactTraitCaps>().Sum(x => x.EffectOnTrait(trait.Definition)) ?? 0.0;

		return baseValue;
	}

	public bool HasTrait(ITraitDefinition trait)
	{
		return _traits.Any(x => x.Definition == trait);
	}

	public ITrait GetTrait(ITraitDefinition definition)
	{
		return _traits.FirstOrDefault(x => x.Definition == definition);
	}

	public string GetTraitDecorated(ITraitDefinition trait)
	{
		return trait.Decorator.Decorate(GetTrait(trait));
	}

	public IEnumerable<ITrait> Traits => _traits;

	public IEnumerable<ITrait> TraitsOfType(TraitType type)
	{
		return _traits.Where(x => x.Definition.TraitType == type);
	}

	#endregion

	#region IHaveLanguage

	public void SetPreferredAccent(IAccent accent)
	{
		Actor.SetPreferredAccent(accent);
	}

	public IAccent PreferredAccent(ILanguage language)
	{
		return Actor.PreferredAccent(language);
	}

	public Difficulty AccentDifficulty(IAccent accent, bool canImprove = true)
	{
		return Actor.AccentDifficulty(accent, canImprove);
	}

	public IEnumerable<ILanguage> Languages => Actor.Languages;

	public ILanguage CurrentLanguage
	{
		get => Actor.CurrentLanguage;
		set => Actor.CurrentLanguage = value;
	}

	public ILanguage CurrentWritingLanguage
	{
		get => Actor.CurrentWritingLanguage;
		set => Actor.CurrentWritingLanguage = value;
	}

	public IEnumerable<IAccent> Accents => Actor.Accents;

	public IAccent CurrentAccent
	{
		get => Actor.CurrentAccent;
		set => Actor.CurrentAccent = value;
	}

	public IEnumerable<IScript> Scripts => Actor.Scripts;

	public IScript CurrentScript
	{
		get => Actor.CurrentScript;
		set => Actor.CurrentScript = value;
	}

	public WritingStyleDescriptors WritingStyle
	{
		get => Actor.WritingStyle;
		set => Actor.WritingStyle = value;
	}

	public void LearnLanguage(ILanguage language)
	{
		Actor.LearnLanguage(language);
	}

	public void LearnAccent(IAccent accent, Difficulty difficulty)
	{
		Actor.LearnAccent(accent, difficulty);
	}

	public void LearnScript(IScript script)
	{
		Actor.LearnScript(script);
	}

	public void ForgetLanguage(ILanguage language)
	{
		Actor.ForgetLanguage(language);
	}

	public void ForgetAccent(IAccent accent)
	{
		Actor.ForgetAccent(accent);
	}

	public void ForgetScript(IScript script)
	{
		Actor.ForgetScript(script);
	}

	#endregion
}