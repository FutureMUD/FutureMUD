#nullable enable

using MudSharp.Body.Needs;
using MudSharp.CharacterCreation;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Communication.Language;
using MudSharp.Framework.Save;

namespace MudSharp.Character;

public partial class Character
{
	protected enum NativeInitialisationMode { Immediate, Deferred }

	protected void ActivateCommittedNativeCharacter(ICharacterTemplate template, object model)
	{
		if (!_deferredNativeInitialisation) throw new InvalidOperationException("Native creation was not deferred.");
		CompleteCommittedInitialisation(model);
		_deferredNativeInitialisation = false;
		SetNoSave(false);
		Body.SetNoSave(false);
		foreach (var knowledge in CharacterKnowledges.OfType<SaveableItem>()) knowledge.SetNoSave(false);
		foreach (var membership in ClanMemberships)
			if (!membership.Clan.Memberships.Contains(membership)) membership.Clan.Memberships.Add(membership);
		NeedsModel = NeedsModelFactory.LoadNeedsModel(CharacterCreation.Chargen.NeedsModelProg is { } prog
			? (string)prog.Execute(template) : "NoNeeds", this);
		Body.TotalBloodVolumeLitres = TotalBloodVolume(this);
		Body.CurrentBloodVolumeLitres = Body.TotalBloodVolumeLitres;
		InitialiseTemplateLanguageChoices(template);
		InitialiseStamina();
		CurrentStamina = MaximumStamina;
		foreach (var hook in Gameworld.DefaultHooks.Where(x => x.Applies(template, "Character")))
			if (InstallHook(hook.Hook)) HooksChanged = true;
		RefreshForcedTransformationHeartbeatRegistration();
		ReconcileCastingResourceCapacities();
		Changed = true;
		Body.Changed = true;
	}

	private void InitialiseTemplateLanguageChoices(ICharacterTemplate template)
	{
		ResolveNativeLanguage(template.SelectedNativeLanguage, template.SelectedEthnicity?.NativeLanguage, template.SelectedCulture?.NativeLanguage);
		var selectedNativeAccent = template.SelectedAccents.Where(x => x.Language == NativeLanguage && x.Role == AccentRole.Native).OrderBy(x => x.Id).FirstOrDefault();
		if (selectedNativeAccent is not null) _preferredAccents[NativeLanguage] = selectedNativeAccent;
		foreach (var language in _languages) EnsureAcquisitionAccent(language, available: language.Accents.Where(x => x.IsAvailableInChargen(template)));
		var startingAccents = _accents.ToList();
		_accents.Clear();
		foreach (var entry in startingAccents) LearnAccent(entry.Key, entry.Value);
		_currentAccent = PreferredAccent(_currentLanguage);
	}

	private void EnsureNativeCreationHasNoAdditionalForms()
	{
		if (_deferredNativeInitialisation && _merits.OfType<IAdditionalBodyFormMerit>().Any())
			throw new InvalidOperationException("Spell-owned NPC creation needs an explicit ownership adapter for merit-provided additional bodies.");
	}

	protected void ReleaseUnpublishedNativeCharacter()
	{
		_deferredNativeInitialisation = true;
		StopNeedsHeartbeat();
		ClearForcedTransformationHeartbeatRegistration();
		PauseMagicResourceGeneratorHeartbeats();
		foreach (var hook in Hooks.ToArray()) RemoveHook(hook);
		foreach (var membership in ClanMemberships) membership.Clan.Memberships.Remove(membership);
		SetNoSave(true);
		Body.SetNoSave(true);
		foreach (var trait in CharacterTraits.Concat(Body.Traits)) Gameworld.SaveManager.Abort(trait);
		foreach (var knowledge in CharacterKnowledges)
		{
			if (knowledge is SaveableItem saveable) saveable.SetNoSave(true);
			Gameworld.SaveManager.Abort(knowledge);
		}
		ReleaseEvents();
	}
}
