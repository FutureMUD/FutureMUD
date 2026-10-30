#nullable enable

using MudSharp.Body;
using MudSharp.Effects.Concrete.SpellEffects;

namespace MudSharp.Health;

/// <summary>Explicit drug/origin selection; never infers that a drug is a poison.</summary>
internal static class DrugPurgeRules
{
	internal static bool HasMatching(ICharacter actor, IDrug drug, string? family, bool includeAftereffects = false) =>
		actor.Body.PartInfections.OfType<IDrugSymptomInfection>().Any(x => Matches(x, drug, family)) ||
		actor.EffectsOfType<SpellPoisonEffect>().Any(x => Matches(x, drug, family)) ||
		actor.Body.ActiveDrugDosages.Concat(actor.Body.LatentDrugDosages).Any(x => Matches(x, drug, family)) ||
		includeAftereffects && actor.Body.EffectsOfType<IDrugAftereffect>().Any(x => Matches(x, drug, family));

	internal static bool Purge(ICharacter actor, IDrug drug, string? family, bool includeAftereffects = false)
	{
		var body = actor.Body;
		var infections = body.PartInfections.OfType<IDrugSymptomInfection>().Where(x => Matches(x, drug, family)).ToArray();
		var effects = actor.EffectsOfType<SpellPoisonEffect>().Where(x => Matches(x, drug, family)).ToArray();
		var changed = effects.Length > 0 || body.ActiveDrugDosages.Concat(body.LatentDrugDosages).Any(x => Matches(x, drug, family));
		var aftermath = includeAftereffects ? body.EffectsOfType<IDrugAftereffect>().Where(x => Matches(x, drug, family)).ToArray() : [];
		changed |= aftermath.Length > 0 || infections.Length > 0;
		foreach (var infection in infections)
		{
			RequireBody(actor, body);
			body.RemoveInfection(infection);
			infection.Delete();
		}
		if (infections.Length > 0) body.RefreshDrugEffects();
		foreach (var effect in effects)
		{
			RequireBody(actor, body);
			actor.RemoveEffect(effect, true);
		}
		RequireBody(actor, body);
		body.RemoveDrugDosages(x => Matches(x, drug, family));
		RequireBody(actor, body);
		foreach (var effect in aftermath)
		{
			RequireBody(actor, body);
			body.RemoveEffect(effect, true);
		}
		RequireBody(actor, body);
		return changed;
	}

	private static bool Matches(IDrugSymptomInfection infection, IDrug drug, string? family) =>
		infection.CureWithDrug && infection.DefinitionDrugId == drug.Id && (family is null || infection.SourceFamily == family);
	private static bool Matches(SpellPoisonEffect effect, IDrug drug, string? family) =>
		effect.Drug.Id == drug.Id && (family is null || effect.OwnedFamily == family);
	private static bool Matches(DrugDosage dose, IDrug drug, string? family) =>
		dose.Drug.Id == drug.Id && (family is null || dose.SpellOriginId.HasValue && dose.SpellOriginFamily == family);
	private static bool Matches(IDrugAftereffect effect, IDrug drug, string? family) =>
		effect.SourceDrugId == drug.Id && (family is null || effect.SourceFamily == family);
	private static void RequireBody(ICharacter actor, IBody body)
	{
		if (!ReferenceEquals(actor.Body, body)) throw new InvalidOperationException("The drug-purge recipient's body changed during treatment.");
	}
}
