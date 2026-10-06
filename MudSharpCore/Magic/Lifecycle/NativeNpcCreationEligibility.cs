#nullable enable

using MudSharp.CharacterCreation;
using MudSharp.Framework.Revision;
using MudSharp.NPC.Templates;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Magic.Lifecycle;

internal static class NativeNpcCreationEligibility
{
	internal static string? TemplateError(INPCTemplate? template, IFuturemud world)
	{
		if (template is not SimpleNPCTemplate)
			return "Lifecycle creation currently requires a simple native NPC template.";
		if (!ReferenceEquals(template.Gameworld, world))
			return "The native NPC template belongs to a different gameworld.";
		if (template.Status != RevisionStatus.Current)
			return "The NPC template is missing or not approved.";
		return CharacterTemplateError(template.GetCharacterTemplate());
	}

	internal static string? CharacterTemplateError(ICharacterTemplate template)
	{
		if (template.Account?.Id is not (null or 0) || template.SelectedProstheses.Any())
			return "Native spell creation requires an accountless NPC template without unadapted prosthetic creation.";
		if (template.SelectedRoles.Any(x => x.TraitAdjustments.Any()))
			return "Lifecycle creation needs an explicit deferred adapter for role trait adjustments.";
		return AdditionalFormsError(CharacterTemplateMerits.EffectiveCharacterMerits(template));
	}

	internal static string? AdditionalFormsError(IEnumerable<IMerit> effectiveCharacterMerits) =>
		effectiveCharacterMerits.OfType<IAdditionalBodyFormMerit>().Any()
			? "Spell-owned NPC creation needs an explicit ownership adapter for merit-provided additional bodies."
			: null;
}
