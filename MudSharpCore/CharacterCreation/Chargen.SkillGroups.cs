using MudSharp.Body.Traits;

#nullable enable
namespace MudSharp.CharacterCreation;

public partial class Chargen
{
	private bool _skillEvaluationContext;
	private bool _hydratingSkillValues;
	private bool _finalisingSkillValues;
	private string SkillClaimDescription(ITraitDefinition skill)
	{
		if (!SkillClaims.Initialised) return "";
		var sources = new List<string>();
		if (SkillClaims.Mandatory.Contains(skill.Id)) sources.Add("mandatory");
		foreach (var group in SkillClaims.Groups.Where(x => x.Value.Skills.Contains(skill.Id)))
			sources.Add(Gameworld.ChargenSkillSelectionGroups.FirstOrDefault(x => x.StableKey == group.Key)?.Name ?? group.Key);
		if (SkillClaims.Ordinary.Contains(skill.Id)) sources.Add(SkillClaims.GroupSkills.Contains(skill.Id) ? "ordinary (group covers base cost)" : "ordinary");
		return $" ({string.Join("; ", sources)})";
	}
	private bool RevalidateSkillGroups()
	{
		if (_skillEvaluationContext || State == ChargenState.Approved) return true;
		if (!SkillClaims.Initialised && !(Gameworld.ChargenSkillSelectionGroups?.Any(x => x.Enabled && !x.Retired) ?? false)) return true;
		var storyboard = Gameworld.ChargenStoryboard.StageScreenMap[ChargenStage.SelectSkills];
		var prog = FreeSkillsProgForStoryboard(storyboard);
		var complete = new SkillGroupResolver(this, prog).Complete;
		if (storyboard is Screens.SkillPickerScreenStoryboard or Screens.SkillCostPickerScreenStoryboard)
			complete &= !SkillGroupResolver.UnavailableOrdinarySkills(this).Any();
		if (storyboard is Screens.SkillPickerScreenStoryboard picker && complete)
		{
			complete = SkillClaims.ChargeableOrdinary.Count() <= Convert.ToDouble(picker.NumberOfSkillPicksProg.Execute(this));
		}
		if (!complete) _completedStages.Remove(ChargenStage.SelectSkills);
		return complete;
	}

	internal static IFutureProg? FreeSkillsProgForStoryboard(IChargenScreenStoryboard? storyboard) => storyboard switch
		{
			Screens.SkillPickerScreenStoryboard x => x.FreeSkillsProg,
			Screens.SkillCostPickerScreenStoryboard x => x.FreeSkillsProg,
			Screens.SkillSkipperScreenStoryboard x => x.FreeSkillsProg,
			Screens.SkillBoostSkipperScreenStoryboard x => x.FreeSkillsProg,
			_ => null
		};

	/// <summary>A detached evaluation context. Provisional acquisitions never enter this baseline.</summary>
	public static IChargen CreateSkillEvaluationContext(ICharacterTemplate source, IEnumerable<long> baseline)
	{
		var ids = baseline.ToHashSet();
		var result = new Chargen((source as IChargen)?.State ?? ChargenState.InProgress, null!, source.Gameworld, source.Account)
		{
			_skillEvaluationContext = true,
			Id = (source as IChargen)?.Id ?? 0,
			Stage = (source as IChargen)?.Stage ?? ChargenStage.SelectSkills,
			ApplicationType = (source as IChargen)?.ApplicationType ?? ApplicationType.Normal,
			IsSpecialApplication = (source as IChargen)?.IsSpecialApplication ?? false,
			SelectedRace = source.SelectedRace,
			SelectedCulture = source.SelectedCulture,
			SelectedEthnicity = source.SelectedEthnicity,
			SelectedName = source.SelectedName,
			SelectedBirthday = source.SelectedBirthday,
			SelectedGender = source.SelectedGender,
			SelectedHeight = source.SelectedHeight,
			SelectedWeight = source.SelectedWeight,
			Handedness = source.Handedness,
			StartingLocation = (source as IChargen)?.StartingLocation!,
			SelectedSdesc = source.SelectedSdesc,
			SelectedFullDesc = source.SelectedFullDesc,
			SelectedAttributes = [],
			SelectedSkills = source.Gameworld.Traits.Where(x => ids.Contains(x.Id)).ToList(),
			SelectedRoles = source.SelectedRoles?.ToList() ?? [],
			SelectedMerits = source.SelectedMerits?.ToList() ?? [],
			SelectedKnowledges = source.SelectedKnowledges?.ToList() ?? [],
			SelectedNotes = (source as IChargen)?.SelectedNotes.ToList() ?? [],
			SelectedAccents = source.SelectedAccents?.ToList() ?? [],
			SelectedCharacteristics = source.SelectedCharacteristics?.ToList() ?? [],
			SelectedEntityDescriptionPatterns = source.SelectedEntityDescriptionPatterns?.ToList() ?? [],
			MissingBodyparts = source.MissingBodyparts?.ToList() ?? [],
			SelectedDisfigurements = source.SelectedDisfigurements?.ToList() ?? [],
			SelectedScars = source.SelectedScars?.ToList() ?? [],
			SelectedTattoos = source.SelectedTattoos?.ToList() ?? [],
			SelectedProstheses = source.SelectedProstheses?.ToList() ?? []
		};
		result.SelectedAttributes = source.SelectedAttributes.Select(x => (ITrait)new TemporaryTrait { Definition = x.Definition, Value = x.Value, Owner = result }).ToList();
		result.SkillValues = result.SelectedSkills.Select(x => (x, source is IChargen application
			? application.SkillClaims.IndependentValues.GetValueOrDefault(x.Id)
			: source.SkillValues.Where(y => y.Item1.Id == x.Id).Select(y => y.Item2).DefaultIfEmpty(0).Max())).ToList();
		return result;
	}

	internal void RestoreSkillClaims(ChargenSkillClaims claims) { SkillClaims = claims; }

	internal void SaveSkillClaims() { if (!_skillEvaluationContext) Save(); }
}
