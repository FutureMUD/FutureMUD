using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Communication.Language;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Character;

public partial class Character
{
	private readonly List<ICharacterForm> _forms = new();
	private readonly List<CharacterFormSourceMapping> _formSources = new();
	private bool _isSwitchingBodies;
	private Alignment _handedness;

	private sealed class CharacterFormSourceMapping
	{
		public CharacterFormSourceMapping(CharacterFormSourceType sourceType, long sourceId, string sourceKey, IBody body)
		{
			SourceType = sourceType;
			SourceId = sourceId;
			SourceKey = sourceKey ?? string.Empty;
			Body = body;
		}

		public CharacterFormSourceType SourceType { get; init; }
		public long SourceId { get; init; }
		public string SourceKey { get; init; }
		public IBody Body { get; set; }

		public bool Matches(ICharacterFormSource source)
		{
			return SourceType == source.SourceType &&
			       SourceId == source.SourceId &&
			       SourceKey.EqualTo(source.SourceKey ?? string.Empty);
		}

		public bool Matches(MudSharp.Models.CharacterBodySource source)
		{
			return SourceType == (CharacterFormSourceType)source.SourceType &&
			       SourceId == source.SourceId &&
			       SourceKey.EqualTo(source.SourceKey ?? string.Empty);
		}
	}

	public IEnumerable<ICharacterForm> Forms => _forms.OrderBy(x => x.SortOrder).ThenBy(x => x.Alias);
	public IEnumerable<IBody> Bodies => Forms.Select(x => x.Body).Distinct();
	public IEnumerable<ITrait> CharacterTraits => _characterTraits;
	public IBody CurrentBody => Body;
	public event CurrentBodyChangedEvent CurrentBodyChanged;

	private SimpleCharacterTemplate GetCharacterTemplateForBody(IBody body)
	{
		List<IAccent> accents = _accents.Where(x => x.Value <= Difficulty.Trivial).Select(x => x.Key).Distinct().ToList();
		foreach (var language in Languages)
		{
			if (accents.All(x => x.Language != language))
			{
				accents.Add(_accents.Where(x => x.Key.Language == language).FirstMin(x => x.Value).Key ??
				            language.DefaultLearnerAccent);
			}
		}

		return new SimpleCharacterTemplate
		{
			Handedness = body.Handedness,
			MissingBodyparts = body.SeveredRoots.ToList(),
			SelectedAccents = accents,
			SelectedKnowledges = Knowledges.ToList(),
			SelectedCharacteristics = body.CharacteristicDefinitions
			                             .Select(x => (x, body.GetCharacteristic(x, this))).ToList(),
			SelectedMerits = Merits.OfType<ICharacterMerit>().ToList(),
			SelectedRoles = Roles.ToList(),
			SelectedWeight = body.Weight,
			SelectedHeight = body.Height,
			SelectedName = PersonalName,
			SelectedCulture = Culture,
			SelectedEthnicity = body.Ethnicity,
			SelectedRace = body.Race,
			SelectedBirthday = Birthday,
			SelectedEntityDescriptionPatterns = body.EntityDescriptionPatterns.ToList(),
			SelectedFullDesc = body.GetRawDescriptions.FullDescription,
			SelectedSdesc = body.GetRawDescriptions.ShortDescription,
			SelectedGender = body.Gender.Enum,
			SkillValues = (from skill in _characterTraits.OfType<ISkill>() select (skill.Definition, skill.RawValue))
				.ToList(),
			SelectedAttributes = (from attribute in body.Traits.OfType<IAttribute>()
				select TraitFactory.LoadAttribute(attribute.AttributeDefinition, body, attribute.RawValue))
				.ToList<ITrait>(),
			Gameworld = Gameworld
		};
	}

	private IEntityDescriptionPattern GetRandomValidDescriptionPattern(ICharacterTemplate template,
		EntityDescriptionType type)
	{
		return Gameworld.EntityDescriptionPatterns
		                .Where(x => x.Type == type)
		                .Where(x => x.IsValidSelection(template))
		                .GetWeightedRandom(x => x.RelativeWeight);
	}

	private static string DefaultDescriptionFallback(IRace race, EntityDescriptionType type)
	{
		return type switch
		{
			EntityDescriptionType.ShortDescription => $"a {race.Name.ToLowerInvariant()}",
			EntityDescriptionType.FullDescription => "You cannot tell anything special or unique about it.",
			_ => string.Empty
		};
	}

	private bool TryApplyDescriptionPatternsToTemplate(SimpleCharacterTemplate template,
		ICharacterFormSpecification specification, out string whyNot)
	{
		var shortPattern = specification.ShortDescriptionPattern;
		if (shortPattern is not null &&
		    (shortPattern.Type != EntityDescriptionType.ShortDescription || !shortPattern.IsValidSelection(template)))
		{
			whyNot = "That short description pattern is not valid for the selected form.";
			return false;
		}

		var fullPattern = specification.FullDescriptionPattern;
		if (fullPattern is not null &&
		    (fullPattern.Type != EntityDescriptionType.FullDescription || !fullPattern.IsValidSelection(template)))
		{
			whyNot = "That full description pattern is not valid for the selected form.";
			return false;
		}

		shortPattern ??= GetRandomValidDescriptionPattern(template, EntityDescriptionType.ShortDescription);
		fullPattern ??= GetRandomValidDescriptionPattern(template, EntityDescriptionType.FullDescription);

		template.SelectedEntityDescriptionPatterns.RemoveAll(x => x.Type == EntityDescriptionType.ShortDescription);
		template.SelectedEntityDescriptionPatterns.RemoveAll(x => x.Type == EntityDescriptionType.FullDescription);
		if (shortPattern is not null)
		{
			template.SelectedEntityDescriptionPatterns.Add(shortPattern);
			template.SelectedSdesc = shortPattern.Pattern;
		}
		else
		{
			template.SelectedSdesc = DefaultDescriptionFallback(template.SelectedRace, EntityDescriptionType.ShortDescription);
		}

		if (fullPattern is not null)
		{
			template.SelectedEntityDescriptionPatterns.Add(fullPattern);
			template.SelectedFullDesc = fullPattern.Pattern;
		}
		else
		{
			template.SelectedFullDesc = DefaultDescriptionFallback(template.SelectedRace, EntityDescriptionType.FullDescription);
		}

		whyNot = string.Empty;
		return true;
	}

	private string DescribeTransformationEcho(ICharacterForm form)
	{
		return form.TransformationEcho switch
		{
			null => $"Default ({Gameworld.GetStaticString("DefaultFormTransformationEcho").ColourCommand()})",
			"" => "Suppressed".ColourError(),
			_ => form.TransformationEcho.ColourCommand()
		};
	}

	private void EmitTransformationEcho(ICharacterForm form, IBody oldBody, IBody newBody)
	{
		var echo = form.TransformationEcho ?? Gameworld.GetStaticString("DefaultFormTransformationEcho");
		if (string.IsNullOrWhiteSpace(echo))
		{
			return;
		}

		OutputHandler.Handle(new EmoteOutput(new Emote(echo, this, oldBody, newBody)));
	}

	private CharacterForm DefaultFormFor(IBody body)
	{
		return new CharacterForm(body, body.Prototype.Name)
		{
			TraumaMode = BodySwitchTraumaMode.Automatic,
			AllowVoluntarySwitch = true
		};
	}

	private void InitialiseDefaultForm(IBody body)
	{
		_forms.Clear();
		_formSources.Clear();
		_forms.Add(DefaultFormFor(body));
	}

	private void InitialiseCharacterTraitsFromTemplate(ICharacterTemplate template)
	{
		_characterTraits.Clear();
		foreach (var skill in template.SkillValues.GroupBy(x => x.Item1).Select(x => x.Last()))
		{
			var trait = skill.Item1.NewTrait(this, skill.Item2);
			trait.Initialise(this);
			_characterTraits.Add(trait);
		}
	}

	private void LoadCharacterTraits(MudSharp.Models.Character character)
	{
		_characterTraits.Clear();
		foreach (var trait in character.CharacterTraits)
		{
			var tempTrait = new MudSharp.Models.Trait
			{
				BodyId = character.Id,
				TraitDefinitionId = trait.TraitDefinitionId,
				Value = trait.Value,
				AdditionalValue = trait.AdditionalValue
			};
			var loadedTrait = MudSharp.Body.Traits.TraitDefinition.LoadTrait(tempTrait, Gameworld, this);
			loadedTrait?.Initialise(this);
			if (loadedTrait != null)
			{
				_characterTraits.Add(loadedTrait);
			}
		}
	}

	private void LoadForms(MudSharp.Models.Character character)
	{
		_forms.Clear();
		_formSources.Clear();
		Body = new Body.Implementations.Body(character.Body, Gameworld, this);
		(Body as Body.Implementations.Body)?.SanitizeIncompatibleHealthState();
		var bodies = new Dictionary<long, IBody>
		{
			{ Body.Id, Body }
		};

		foreach (var form in character.CharacterBodies.OrderBy(x => x.SortOrder).ThenBy(x => x.Alias))
		{
			if (!bodies.TryGetValue(form.BodyId, out var body))
			{
				body = new Body.Implementations.Body(form.Body, Gameworld, this);
				(body as Body.Implementations.Body)?.SanitizeIncompatibleHealthState();
				bodies[body.Id] = body;
			}

			_forms.Add(new CharacterForm(form, body, Gameworld));
		}

		foreach (var source in character.CharacterBodySources)
		{
			if (!bodies.TryGetValue(source.BodyId, out var body))
			{
				body = new Body.Implementations.Body(source.Body, Gameworld, this);
				(body as Body.Implementations.Body)?.SanitizeIncompatibleHealthState();
				bodies[body.Id] = body;
			}

			if (_forms.All(x => x.Body != body))
			{
				_forms.Add(DefaultFormFor(body));
			}

			_formSources.Add(new CharacterFormSourceMapping(
				(CharacterFormSourceType)source.SourceType,
				source.SourceId,
				source.SourceKey,
				body
			));
		}

		if (_forms.All(x => x.Body != Body))
		{
			_forms.Add(DefaultFormFor(Body));
		}

		if (!_forms.Any())
		{
			InitialiseDefaultForm(Body);
		}

		_handedness = Body.Handedness;
	}

	private void SaveForms(MudSharp.Models.Character dbchar)
	{
		dbchar.BodyId = Body.Id;

		var removedForms = dbchar.CharacterBodies
			.Where(x => _forms.All(y => y.Body.Id != x.BodyId))
			.ToList();
		if (removedForms.Any())
		{
			FMDB.Context.RemoveRange(removedForms);
		}

		var removedSources = dbchar.CharacterBodySources
			.Where(x => _formSources.All(y => !y.Matches(x) || y.Body.Id != x.BodyId))
			.ToList();
		if (removedSources.Any())
		{
			FMDB.Context.RemoveRange(removedSources);
		}

		foreach (var form in _forms)
		{
			var dbform = dbchar.CharacterBodies.FirstOrDefault(x => x.BodyId == form.Body.Id);
			if (dbform == null)
			{
				dbform = new MudSharp.Models.CharacterBody
				{
					Character = dbchar,
					BodyId = form.Body.Id
				};
				dbchar.CharacterBodies.Add(dbform);
			}

			dbform.Alias = form.Alias;
			dbform.SortOrder = form.SortOrder;
			dbform.TraumaMode = (int)form.TraumaMode;
			dbform.TransformationEcho = form.TransformationEcho;
			dbform.AllowVoluntarySwitch = form.AllowVoluntarySwitch;
			dbform.CanVoluntarilySwitchProgId = form.CanVoluntarilySwitchProg?.Id;
			dbform.WhyCannotVoluntarilySwitchProgId = form.WhyCannotVoluntarilySwitchProg?.Id;
			dbform.CanSeeFormProgId = form.CanSeeFormProg?.Id;
		}

		foreach (var source in _formSources)
		{
			var dbsource = dbchar.CharacterBodySources.FirstOrDefault(x => source.Matches(x));
			if (dbsource == null)
			{
				dbsource = new MudSharp.Models.CharacterBodySource
				{
					Character = dbchar,
					SourceType = (int)source.SourceType,
					SourceId = source.SourceId,
					SourceKey = source.SourceKey
				};
				dbchar.CharacterBodySources.Add(dbsource);
			}

			dbsource.BodyId = source.Body.Id;
		}
	}

	private void InsertInitialForms(MudSharp.Models.Character dbitem)
	{
		if (!_forms.Any())
		{
			InitialiseDefaultForm(Body);
		}

		foreach (var form in _forms)
		{
			dbitem.CharacterBodies.Add(new MudSharp.Models.CharacterBody
			{
				Character = dbitem,
				Body = form.Body == Body ? dbitem.Body : FMDB.Context.Bodies.Find(form.Body.Id),
				Alias = form.Alias,
				SortOrder = form.SortOrder,
				TraumaMode = (int)form.TraumaMode,
				TransformationEcho = form.TransformationEcho,
				AllowVoluntarySwitch = form.AllowVoluntarySwitch,
				CanVoluntarilySwitchProgId = form.CanVoluntarilySwitchProg?.Id,
				WhyCannotVoluntarilySwitchProgId = form.WhyCannotVoluntarilySwitchProg?.Id,
				CanSeeFormProgId = form.CanSeeFormProg?.Id
			});
		}

		foreach (var source in _formSources)
		{
			dbitem.CharacterBodySources.Add(new MudSharp.Models.CharacterBodySource
			{
				Character = dbitem,
				Body = source.Body == Body ? dbitem.Body : FMDB.Context.Bodies.Find(source.Body.Id),
				SourceType = (int)source.SourceType,
				SourceId = source.SourceId,
				SourceKey = source.SourceKey
			});
		}
	}

	private void InsertInitialCharacterTraits(MudSharp.Models.Character dbitem)
	{
		foreach (var trait in _characterTraits)
		{
			dbitem.CharacterTraits.Add(new MudSharp.Models.CharacterTrait
			{
				Character = dbitem,
				TraitDefinitionId = trait.Definition.Id,
				Value = trait.RawValue,
				AdditionalValue = trait is Body.Traits.Subtypes.TheoreticalSkill theoretical
					? theoretical.TheoreticalValue
					: 0.0
			});
		}
	}

	private ICharacterForm GetForm(IBody target)
	{
		return _forms.FirstOrDefault(x => x.Body == target);
	}

	private static ICharacterForm ResolveForm(IEnumerable<ICharacterForm> forms, string text)
	{
		if (long.TryParse(text, out var value))
		{
			return forms.FirstOrDefault(x => x.Body.Id == value);
		}

		return forms.FirstOrDefault(x => x.Alias.EqualTo(text));
	}

	internal ICharacterForm ResolveForm(string text)
	{
		return ResolveForm(_forms, text);
	}

	internal IEnumerable<ICharacterForm> VisibleFormsFor(ICharacter viewer)
	{
		return _forms.Where(x => x.Body == CurrentBody || x.CanSee(viewer))
		             .OrderBy(x => x.SortOrder)
		             .ThenBy(x => x.Alias);
	}

	internal ICharacterForm ResolveVisibleForm(ICharacter viewer, string text)
	{
		return ResolveForm(VisibleFormsFor(viewer), text);
	}

	private bool AliasInUse(string alias, ICharacterForm except = null)
	{
		return _forms.Any(x => !ReferenceEquals(x, except) && x.Alias.EqualTo(alias));
	}

	private string GetNextAvailableAlias(string desiredAlias)
	{
		if (!AliasInUse(desiredAlias))
		{
			return desiredAlias;
		}

		for (var i = 2; true; i++)
		{
			var alias = $"{desiredAlias} {i}";
			if (!AliasInUse(alias))
			{
				return alias;
			}
		}
	}

	internal bool TrySetFormAlias(ICharacterForm form, string alias, out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(alias))
		{
			whyNot = "You must specify a non-blank alias.";
			return false;
		}

		if (AliasInUse(alias, form))
		{
			whyNot = "You already have another form with that alias.";
			return false;
		}

		form.Alias = alias;
		Changed = true;
		whyNot = string.Empty;
		return true;
	}

	internal bool TrySetFormSortOrder(ICharacterForm form, int sortOrder, out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		form.SortOrder = sortOrder;
		Changed = true;
		whyNot = string.Empty;
		return true;
	}

	internal bool TrySetFormTraumaMode(ICharacterForm form, BodySwitchTraumaMode traumaMode, out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		form.TraumaMode = traumaMode;
		Changed = true;
		whyNot = string.Empty;
		return true;
	}

	internal bool TrySetFormTransformationEcho(ICharacterForm form, string? transformationEcho, out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		form.TransformationEcho = transformationEcho;
		Changed = true;
		whyNot = string.Empty;
		return true;
	}

	internal bool TrySetFormAllowVoluntary(ICharacterForm form, bool allowVoluntary, out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		form.AllowVoluntarySwitch = allowVoluntary;
		Changed = true;
		whyNot = string.Empty;
		return true;
	}

	internal bool TrySetFormVisibilityProg(ICharacterForm form, IFutureProg prog, out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		form.CanSeeFormProg = prog;
		Changed = true;
		whyNot = string.Empty;
		return true;
	}

	internal bool TrySetFormCanSwitchProg(ICharacterForm form, IFutureProg prog, out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		form.CanVoluntarilySwitchProg = prog;
		Changed = true;
		whyNot = string.Empty;
		return true;
	}

	internal bool TrySetFormWhyCantProg(ICharacterForm form, IFutureProg prog, out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		form.WhyCannotVoluntarilySwitchProg = prog;
		Changed = true;
		whyNot = string.Empty;
		return true;
	}

	internal bool TryClearFormVisibilityProg(ICharacterForm form, out string whyNot)
	{
		return TrySetFormVisibilityProg(form, null, out whyNot);
	}

	internal bool TryClearFormCanSwitchProg(ICharacterForm form, out string whyNot)
	{
		return TrySetFormCanSwitchProg(form, null, out whyNot);
	}

	internal bool TryClearFormWhyCantProg(ICharacterForm form, out string whyNot)
	{
		return TrySetFormWhyCantProg(form, null, out whyNot);
	}

	internal bool TrySetFormDescriptionPattern(ICharacterForm form, IEntityDescriptionPattern pattern,
		out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		if (pattern is null)
		{
			whyNot = "There is no such entity description pattern.";
			return false;
		}

		var template = GetCharacterTemplateForBody(form.Body);
		if (!pattern.IsValidSelection(template))
		{
			whyNot = $"That {pattern.Type.Describe().ToLowerInvariant()} pattern is not valid for that form.";
			return false;
		}

		switch (pattern.Type)
		{
			case EntityDescriptionType.ShortDescription:
				form.Body.SetShortDescriptionPattern(pattern);
				break;
			case EntityDescriptionType.FullDescription:
				form.Body.SetFullDescriptionPattern(pattern);
				break;
			default:
				whyNot = "That pattern type is not supported.";
				return false;
		}

		Changed = true;
		whyNot = string.Empty;
		return true;
	}

	internal bool TryRandomiseFormDescriptionPattern(ICharacterForm form, EntityDescriptionType type, out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		var template = GetCharacterTemplateForBody(form.Body);
		var pattern = GetRandomValidDescriptionPattern(template, type);
		if (pattern is null)
		{
			whyNot = $"There are no valid {type.Describe().ToLowerInvariant()} patterns for that form.";
			return false;
		}

		return TrySetFormDescriptionPattern(form, pattern, out whyNot);
	}

	internal bool TryClearFormDescriptionPattern(ICharacterForm form, EntityDescriptionType type, out string whyNot)
	{
		if (form is null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		switch (type)
		{
			case EntityDescriptionType.ShortDescription:
				form.Body.ClearShortDescriptionPattern();
				break;
			case EntityDescriptionType.FullDescription:
				form.Body.ClearFullDescriptionPattern();
				break;
			default:
				whyNot = "That pattern type is not supported.";
				return false;
		}

		Changed = true;
		whyNot = string.Empty;
		return true;
	}

	private CharacterFormSourceMapping GetFormSource(ICharacterFormSource source)
	{
		return _formSources.FirstOrDefault(x => x.Matches(source));
	}

	private void SetFormSource(IBody body, ICharacterFormSource source)
	{
		var existing = GetFormSource(source);
		if (existing == null)
		{
			_formSources.Add(new CharacterFormSourceMapping(source.SourceType, source.SourceId, source.SourceKey, body));
		}
		else
		{
			existing.Body = body;
		}

		Changed = true;
	}

	private bool TryNormaliseFormSpecification(ICharacterFormSpecification specification, out IRace race,
		out IEthnicity ethnicity, out Gender selectedGender, out string desiredAlias, out int desiredSortOrder,
		out string whyNot)
	{
		race = specification?.Race;
		ethnicity = null;
		selectedGender = default;
		desiredAlias = string.Empty;
		desiredSortOrder = 0;
		if (race is null)
		{
			whyNot = "There is no such race.";
			return false;
		}

		selectedGender = specification.Gender ?? Gender.Enum;
		if (!race.AllowedGenders.Contains(selectedGender))
		{
			selectedGender = race.AllowedGenders.FirstOrDefault();
			if (!race.AllowedGenders.Contains(selectedGender))
			{
				whyNot = "That race does not permit any valid genders.";
				return false;
			}
		}

		var targetRace = race;
		ethnicity = specification.Ethnicity ?? (targetRace.SameRace(Ethnicity?.ParentRace)
			? Ethnicity
			: Gameworld.Ethnicities.FirstOrDefault(x => targetRace.SameRace(x.ParentRace)));
		if (ethnicity is null)
		{
			whyNot = "That race does not have any compatible ethnicity to use for an alternate form.";
			return false;
		}

		if (!race.SameRace(ethnicity.ParentRace))
		{
			whyNot = "That ethnicity is not compatible with the selected race.";
			return false;
		}

		desiredAlias = specification.Alias;
		if (string.IsNullOrWhiteSpace(desiredAlias))
		{
			desiredAlias = race.Name;
		}

		desiredSortOrder = specification.SortOrder ??
		                   (_forms.Select(x => x.SortOrder).DefaultIfEmpty(-1).Max() + 1);
		whyNot = string.Empty;
		return true;
	}

	private ICharacterForm TryFindMatchingExistingForm(IRace race, IEthnicity ethnicity, Gender gender, string alias)
	{
		return _forms.Where(x => x.Alias.EqualTo(alias))
		             .Where(x => x.Body.Race.SameRace(race))
		             .Where(x => x.Body.Ethnicity == ethnicity)
		             .Where(x => x.Body.Gender.Enum == gender)
		             .Take(2)
		             .ToList() switch
		             {
			             [var form] => form,
			             _ => null
		             };
	}

	private bool TryCreateForm(ICharacterFormSpecification specification, IRace race, IEthnicity ethnicity,
		Gender gender, string desiredAlias, int desiredSortOrder, out ICharacterForm form, out string whyNot)
	{
		var baseTemplate = GetCharacterTemplate();
		var template = baseTemplate as SimpleCharacterTemplate ?? new SimpleCharacterTemplate(baseTemplate.SaveToXml(), Gameworld);
		var handedness = race.HandednessOptions.Contains(template.Handedness) ? template.Handedness : race.DefaultHandedness;
		var selectedCharacteristics = race.Characteristics(gender)
		                                 .Select(x => (
			                                 x,
			                                 template.SelectedCharacteristics.FirstOrDefault(y => y.Item1 == x).Item2 ??
			                                 x.DefaultValue))
		                                 .Where(x => x.Item2 is not null)
		                                 .ToList();
		var selectedAttributes = template.SelectedAttributes
		                                 .Where(x => x.Definition is IAttributeDefinition definition &&
		                                             race.Attributes.Contains(definition))
		                                 .ToList();
		var newTemplate = template with
		{
			SelectedRace = race,
			SelectedEthnicity = ethnicity,
			SelectedGender = gender,
			SelectedEntityDescriptionPatterns = new List<IEntityDescriptionPattern>(template.SelectedEntityDescriptionPatterns),
			SelectedCharacteristics = selectedCharacteristics,
			SelectedAttributes = selectedAttributes,
			Handedness = handedness,
			MissingBodyparts = [],
			SelectedDisfigurements = [],
			SelectedScars = [],
			SelectedTattoos = [],
			SelectedProstheses = [],
			HealthStrategy = race.DefaultHealthStrategy
		};

		if (!TryApplyDescriptionPatternsToTemplate(newTemplate, specification, out whyNot))
		{
			form = null;
			return false;
		}

		var newBody = new Body.Implementations.Body(Gameworld, this, newTemplate);
		newBody.SuspendForCharacter();
		Gameworld.SaveManager.AddInitialisation(newBody);
		Gameworld.SaveManager.Flush();
		if (newBody.Changed)
		{
			Gameworld.SaveManager.Flush();
		}

		var newForm = new CharacterForm(newBody, GetNextAvailableAlias(desiredAlias), desiredSortOrder)
		{
			TraumaMode = specification.TraumaMode,
			TransformationEcho = specification.TransformationEcho,
			AllowVoluntarySwitch = specification.AllowVoluntarySwitch,
			CanVoluntarilySwitchProg = specification.CanVoluntarilySwitchProg,
			WhyCannotVoluntarilySwitchProg = specification.WhyCannotVoluntarilySwitchProg,
			CanSeeFormProg = specification.CanSeeFormProg
		};
		_forms.Add(newForm);
		Changed = true;
		form = newForm;
		whyNot = string.Empty;
		return true;
	}

	internal bool TryAddForm(IRace race, IEthnicity ethnicity, Gender? gender, out ICharacterForm form,
		out string whyNot)
	{
		var specification = new CharacterFormSpecification
		{
			Race = race,
			Ethnicity = ethnicity,
			Gender = gender,
			Alias = race?.Name,
			SortOrder = _forms.Select(x => x.SortOrder).DefaultIfEmpty(-1).Max() + 1,
			TraumaMode = BodySwitchTraumaMode.Automatic,
			AllowVoluntarySwitch = false
		};
		if (!TryNormaliseFormSpecification(specification, out race, out ethnicity, out var selectedGender,
			    out var desiredAlias, out var desiredSortOrder, out whyNot))
		{
			form = null;
			return false;
		}

		return TryCreateForm(specification, race, ethnicity, selectedGender, desiredAlias, desiredSortOrder, out form,
			out whyNot);
	}

	public bool EnsureForm(ICharacterFormSpecification specification, ICharacterFormSource source, out ICharacterForm form,
		out string whyNot)
	{
		form = null;
		if (!TryNormaliseFormSpecification(specification, out var race, out var ethnicity, out var selectedGender,
			    out var desiredAlias, out var desiredSortOrder, out whyNot))
		{
			return false;
		}

		var sourced = GetFormSource(source);
		if (sourced != null)
		{
			form = GetForm(sourced.Body);
			if (form == null)
			{
				form = DefaultFormFor(sourced.Body);
				_forms.Add(form);
				Changed = true;
			}

			whyNot = string.Empty;
			return true;
		}

		form = TryFindMatchingExistingForm(race, ethnicity, selectedGender, desiredAlias);
		if (form != null)
		{
			SetFormSource(form.Body, source);
			whyNot = string.Empty;
			return true;
		}

		if (!TryCreateForm(specification, race, ethnicity, selectedGender, desiredAlias, desiredSortOrder, out form,
			    out whyNot))
		{
			return false;
		}

		SetFormSource(form.Body, source);
		return true;
	}

	private void EnsureProvisionedFormFromMerit(IAdditionalBodyFormMerit merit)
	{
		if (EnsureForm(merit.FormSpecification,
			    new CharacterFormSource(CharacterFormSourceType.Merit, merit.Id),
			    out _,
			    out _))
		{
			return;
		}

		Gameworld.SystemMessage(
			$"Character #{Id.ToString("N0")} could not ensure merit-provided form from merit #{merit.Id.ToString("N0")}.",
			true
		);
	}

	private void EnsureProvisionedFormsFromMerits()
	{
		foreach (var merit in _merits.OfType<IAdditionalBodyFormMerit>())
		{
			EnsureProvisionedFormFromMerit(merit);
		}
	}

	private BodySwitchTraumaMode GetEffectiveTraumaMode(ICharacterForm form, IBody target)
	{
		if (form.TraumaMode != BodySwitchTraumaMode.Automatic)
		{
			return form.TraumaMode;
		}

		if (Body?.HealthStrategy?.CanTransferBodyStateTo(target?.HealthStrategy) == true &&
		    target?.HealthStrategy?.CanTransferBodyStateTo(Body.HealthStrategy) == true)
		{
			return BodySwitchTraumaMode.Transfer;
		}

		return BodySwitchTraumaMode.Stash;
	}

	public bool CanSwitchBody(IBody target, BodySwitchIntent intent, out string whyNot)
	{
		if (target == null)
		{
			whyNot = "There is no such form.";
			return false;
		}

		if (_isSwitchingBodies)
		{
			whyNot = "You are already switching forms.";
			return false;
		}

		if (State.HasFlag(CharacterState.Dead))
		{
			whyNot = "You cannot switch forms while dead.";
			return false;
		}

		if (State.HasFlag(CharacterState.Stasis))
		{
			whyNot = "You cannot switch forms while in stasis.";
			return false;
		}

		if (target == Body)
		{
			whyNot = "You are already using that form.";
			return false;
		}

		var form = GetForm(target);
		if (form == null)
		{
			whyNot = "That body is not one of your forms.";
			return false;
		}

		if (intent == BodySwitchIntent.Voluntary && !form.CanSwitchVoluntarily(this, out whyNot))
		{
			return false;
		}

		var currentBody = Body as MudSharp.Body.Implementations.Body;
		var targetBody = target as MudSharp.Body.Implementations.Body;
		var traumaMode = GetEffectiveTraumaMode(form, target);
		var switchReason = string.Empty;
		if (currentBody is null || targetBody is null ||
		    !targetBody.TryPrepareSwitchFrom(currentBody, traumaMode, out _, out switchReason))
		{
			whyNot = switchReason ?? "That form cannot be used for switching right now.";
			return false;
		}

		whyNot = string.Empty;
		return true;
	}

	public bool SwitchToBody(IBody target, BodySwitchIntent intent)
	{
		if (!CanSwitchBody(target, intent, out _) ||
		    Body is not MudSharp.Body.Implementations.Body oldBody ||
		    target is not MudSharp.Body.Implementations.Body newBody ||
		    GetForm(target) is not { } form ||
		    !newBody.TryPrepareSwitchFrom(oldBody, GetEffectiveTraumaMode(form, target), out var switchPlan, out _))
		{
			return false;
		}

		_isSwitchingBodies = true;
		try
		{
			PrepareForBodySwitch();
			Body = newBody;
			newBody.ActivateForCharacter();
			newBody.ApplySwitchPlan(switchPlan);
			_handedness = Body.Handedness;
			_gender = Body.Gender;
			PostProcessBodySwitch();
			newBody.FinaliseSwitchActivation();
			EmitTransformationEcho(form, oldBody, Body);
			CurrentBodyChanged?.Invoke(this, oldBody, Body);
			Changed = true;
			return true;
		}
		finally
		{
			_isSwitchingBodies = false;
		}
	}

	private void PrepareForBodySwitch()
	{
		Movement?.CancelForMoverOnly(this);
		Stop(true);
		QueuedMoveCommands.Clear();
		Aim?.ReleaseEvents();
		Aim = null;
		RemoveAllEffects(x => x.IsEffectType<IActionEffect>(), true);
		RemoveAllEffects<IRemoveOnMovementEffect>(fireRemovalAction: true);
		RemoveAllEffects(x => x.IsEffectType<Dragging>(), true);
		RemoveAllEffects<Dragging.DragTarget>(fireRemovalAction: true);
		Body.RemoveAllEffects(x => x.IsEffectType<IActionEffect>(), true);

		if (RidingMount is not null)
		{
			RidingMount.RemoveRider(this);
			RidingMount = null;
		}

		foreach (var rider in Riders.ToList())
		{
			RemoveRider(rider);
			rider.RidingMount = null;
		}
	}

	private void PostProcessBodySwitch()
	{
		TargettedBodypart = null;
		if (Combat is not null)
		{
			foreach (var combatant in Combat.Combatants.OfType<ICharacter>().Where(x => x.CombatTarget == this).ToList())
			{
				if (combatant.TargettedBodypart is not null &&
				    Body.Prototype.AllBodypartsBonesAndOrgans.All(x =>
					    x != combatant.TargettedBodypart &&
					    !x.CountsAs(combatant.TargettedBodypart) &&
					    !combatant.TargettedBodypart.CountsAs(x)))
				{
					combatant.TargettedBodypart = null;
				}
			}
		}

		if (!CanMovePosition(PositionState, true))
		{
			PositionModifier = PositionModifier.None;
			PositionTarget = null;
			PositionState = MostUprightMobilePosition(true) ?? PositionSprawled.Instance;
		}

		Body.Look(true);
	}
}
