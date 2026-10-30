#nullable enable

using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Health;

namespace MudSharp.Magic.SpellEffects;

public partial class RemovePoisonEffect
{
	private readonly HashSet<long> _additionalDrugIds = [];
	private readonly List<long[]> _drugGroups = [];
	private bool _firstMatchingGroup;
	private bool _removeAftereffects;
	private HashSet<long>? _selectedDrugIds;
	private sealed record OrderedPoisonSelection(ConditionBodySelection Body, long[] Drugs) : IMagicSpellEffectPreparedSelectionToken;
	private string? AdditionalDrugConfigurationError => _additionalDrugIds.Any(id => id <= 0 || Gameworld.Drugs.Get(id) is null)
		? "Poison cure contains an unresolved additional native drug." : null;
	private void LoadAdditionalDrugs(XElement root)
	{
		_additionalDrugIds.Clear();
		foreach (var element in root.Element("AdditionalDrugs")?.Elements("Drug") ?? []) _additionalDrugIds.Add((long)element);
		_drugGroups.Clear();
		foreach (var group in root.Element("DrugGroups")?.Elements("Group") ?? [])
		{
			var ids = group.Elements("Drug").Select(x => (long)x).Distinct().ToArray();
			if (ids.Length == 0) continue;
			_drugGroups.Add(ids);
			_additionalDrugIds.UnionWith(ids);
		}
		_firstMatchingGroup = (bool?)root.Element("FirstMatchingGroup") ?? false;
		_removeAftereffects = (bool?)root.Element("RemoveAftereffects") ?? false;
	}
	private void SaveAdditionalDrugs(XElement root)
	{
		if (_additionalDrugIds.Count > 0) root.Add(new XElement("AdditionalDrugs", _additionalDrugIds.Order().Select(id => new XElement("Drug", id))));
		if (_drugGroups.Count > 0) root.Add(new XElement("DrugGroups", _drugGroups.Select(group => new XElement("Group", group.Select(id => new XElement("Drug", id))))));
		root.Add(new XElement("FirstMatchingGroup", _firstMatchingGroup), new XElement("RemoveAftereffects", _removeAftereffects));
	}
	private bool BuildingCommandAdditionalDrug(ICharacter actor, StringStack command)
	{
		var drug = Gameworld.Drugs.GetByIdOrName(command.SafeRemainingArgument);
		if (drug is null) { actor.Send("Which native drug should this spell also purge?"); return false; }
		if (_additionalDrugIds.Remove(drug.Id))
		{
			for (var i = _drugGroups.Count - 1; i >= 0; i--)
			{
				_drugGroups[i] = _drugGroups[i].Where(id => id != drug.Id).ToArray();
				if (_drugGroups[i].Length == 0) _drugGroups.RemoveAt(i);
			}
			actor.Send($"This spell no longer purges all doses of {drug.Name.ColourName()}.");
		}
		else { _additionalDrugIds.Add(drug.Id); actor.Send($"This spell now also purges all active/latent doses and poison effects of {drug.Name.ColourName()}."); }
		Spell.Changed = true;
		return true;
	}
	private bool BuildingCommandCurePolicy(ICharacter actor, string option, StringStack command)
	{
		if (option == "group")
		{
			if (command.SafeRemainingArgument.EqualTo("clear")) _drugGroups.Clear();
			else
			{
				var ids = new List<long>();
				while (!command.IsFinished)
				{
					var drug = Gameworld.Drugs.GetByIdOrName(command.PopSpeech());
					if (drug is null) { actor.Send("Every group member must name a native drug."); return false; }
					ids.Add(drug.Id);
				}
				if (ids.Count == 0) return false;
				_drugGroups.Add(ids.Distinct().ToArray()); _additionalDrugIds.UnionWith(ids);
			}
		}
		else
		{
			if (!bool.TryParse(command.SafeRemainingArgument, out var enabled)) { actor.Send("Specify true or false."); return false; }
			if (option == "firstgroup") _firstMatchingGroup = enabled; else _removeAftereffects = enabled;
		}
		Spell.Changed = true;
		actor.Send("The poison cure policy was updated.");
		return true;
	}

	private bool PrimaryMatches(SpellPoisonEffect effect) => effect.Drug == Drug && effect.Vector == Vector &&
		effect.GramsFormulaText.EqualTo(FormulaText) && (OwnedFamily is null || effect.OwnedFamily == OwnedFamily);
	private bool DoseMatches(DrugDosage dose) => _additionalDrugIds.Contains(dose.Drug.Id) ||
		OwnedFamily is not null && dose.SpellOriginId.HasValue && dose.SpellOriginFamily == OwnedFamily &&
		dose.Drug == Drug && dose.OriginalVector == Vector && dose.SpellOriginFormula.EqualTo(FormulaText);
	private bool EffectMatches(SpellPoisonEffect effect) => PrimaryMatches(effect) || _additionalDrugIds.Contains(effect.Drug.Id);

	/// <summary>Pure configured selection, including latent doses and residual owned doses.</summary>
	public bool HasMatchingPoison(ICharacter target) => DefinitionError is null && target.Body is { } body &&
		(target.EffectsOfType<SpellPoisonEffect>().Any(EffectMatches) || body.ActiveDrugDosages.Concat(body.LatentDrugDosages).Any(DoseMatches) ||
		body.EffectsOfType<IDrugAftereffect>().Any(AftereffectMatches));

	private bool AftereffectMatches(IDrugAftereffect effect) => _removeAftereffects && (_additionalDrugIds.Contains(effect.SourceDrugId) ||
		Drug?.Id == effect.SourceDrugId && (OwnedFamily is null || effect.SourceFamily == OwnedFamily));
	private HashSet<long>? SelectDrugGroup(ICharacter target)
	{
		if (!_firstMatchingGroup || _drugGroups.Count == 0) return null;
		var present = target.EffectsOfType<SpellPoisonEffect>().Where(EffectMatches).Select(x => x.Drug.Id)
			.Concat(target.Body.ActiveDrugDosages.Concat(target.Body.LatentDrugDosages).Where(DoseMatches).Select(x => x.Drug.Id))
			.Concat(target.Body.EffectsOfType<IDrugAftereffect>().Where(AftereffectMatches).Select(x => x.SourceDrugId)).ToHashSet();
		return (_drugGroups.FirstOrDefault(group => group.Any(present.Contains)) ?? []).ToHashSet();
	}

	private MagicEffectOperationStatus ApplyAdditionalRemoval(ICharacter target)
	{
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		var selected = _selection ?? new ConditionBodySelection(target, target.Body);
		if (!selected.IsCurrent(target)) return MagicEffectOperationStatus.Rejected;
		var ids = _selectedDrugIds ?? SelectDrugGroup(target);
		bool Admits(long id) => ids is null || ids.Contains(id);
		var effects = target.EffectsOfType<SpellPoisonEffect>().Where(x => EffectMatches(x) && Admits(x.Drug.Id)).ToArray();
		var hasDoses = selected.Body.ActiveDrugDosages.Concat(selected.Body.LatentDrugDosages).Any(x => DoseMatches(x) && Admits(x.Drug.Id));
		var aftermath = selected.Body.EffectsOfType<IDrugAftereffect>().Where(x => AftereffectMatches(x) && Admits(x.SourceDrugId)).ToArray();
		if (effects.Length == 0 && !hasDoses && aftermath.Length == 0) return MagicEffectOperationStatus.NoChange;
		foreach (var effect in effects)
		{
			RequireSelection();
			target.RemoveEffect(effect, true);
		}
		RequireSelection();
		selected.Body.RemoveDrugDosages(x => DoseMatches(x) && Admits(x.Drug.Id));
		foreach (var effect in aftermath)
		{
			RequireSelection();
			selected.Body.RemoveEffect(effect, true);
		}
		RequireSelection();
		return MagicEffectOperationStatus.Applied;
		void RequireSelection()
		{
			if (!selected.IsCurrent(target)) throw new InvalidOperationException("Poison cure recipient changed during configured drug removal.");
		}
	}
}
