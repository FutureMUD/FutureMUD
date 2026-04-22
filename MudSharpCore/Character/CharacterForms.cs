using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.NPC.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Character;

public partial class Character
{
	private readonly List<ICharacterForm> _forms = new();
	private bool _isSwitchingBodies;
	private Alignment _handedness;

	public IEnumerable<ICharacterForm> Forms => _forms.OrderBy(x => x.SortOrder).ThenBy(x => x.Alias);
	public IEnumerable<IBody> Bodies => Forms.Select(x => x.Body).Distinct();
	public IEnumerable<ITrait> CharacterTraits => _characterTraits;
	public IBody CurrentBody => Body;
	public event CurrentBodyChangedEvent CurrentBodyChanged;

	private CharacterForm DefaultFormFor(IBody body)
	{
		return new CharacterForm(body, body.Prototype.Name)
		{
			AllowVoluntarySwitch = true
		};
	}

	private void InitialiseDefaultForm(IBody body)
	{
		_forms.Clear();
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
		Body = new Body.Implementations.Body(character.Body, Gameworld, this);
		var bodies = new Dictionary<long, IBody>
		{
			{ Body.Id, Body }
		};

		foreach (var form in character.CharacterBodies.OrderBy(x => x.SortOrder).ThenBy(x => x.Alias))
		{
			if (!bodies.TryGetValue(form.BodyId, out var body))
			{
				body = new Body.Implementations.Body(form.Body, Gameworld, this);
				bodies[body.Id] = body;
			}

			_forms.Add(new CharacterForm(form, body, Gameworld));
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
			dbform.AllowVoluntarySwitch = form.AllowVoluntarySwitch;
			dbform.CanVoluntarilySwitchProgId = form.CanVoluntarilySwitchProg?.Id;
			dbform.WhyCannotVoluntarilySwitchProgId = form.WhyCannotVoluntarilySwitchProg?.Id;
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
				AllowVoluntarySwitch = form.AllowVoluntarySwitch,
				CanVoluntarilySwitchProgId = form.CanVoluntarilySwitchProg?.Id,
				WhyCannotVoluntarilySwitchProgId = form.WhyCannotVoluntarilySwitchProg?.Id
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

	internal ICharacterForm ResolveForm(string text)
	{
		if (long.TryParse(text, out var value))
		{
			return _forms.FirstOrDefault(x => x.Body.Id == value);
		}

		return _forms.FirstOrDefault(x => x.Alias.EqualTo(text));
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

	internal bool TryAddForm(IRace race, IEthnicity ethnicity, Gender? gender, out ICharacterForm form,
		out string whyNot)
	{
		form = null;
		if (race is null)
		{
			whyNot = "There is no such race.";
			return false;
		}

		var selectedGender = gender ?? Gender.Enum;
		if (!race.AllowedGenders.Contains(selectedGender))
		{
			selectedGender = race.AllowedGenders.FirstOrDefault();
			if (!race.AllowedGenders.Contains(selectedGender))
			{
				whyNot = "That race does not permit any valid genders.";
				return false;
			}
		}

		ethnicity ??= race.SameRace(Ethnicity?.ParentRace)
			? Ethnicity
			: Gameworld.Ethnicities.FirstOrDefault(x => race.SameRace(x.ParentRace));
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

		var baseTemplate = GetCharacterTemplate();
		var template = baseTemplate as SimpleCharacterTemplate ?? new SimpleCharacterTemplate(baseTemplate.SaveToXml(), Gameworld);
		var handedness = race.HandednessOptions.Contains(template.Handedness) ? template.Handedness : race.DefaultHandedness;
		var selectedCharacteristics = race.Characteristics(selectedGender)
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
			SelectedGender = selectedGender,
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

		var newBody = new Body.Implementations.Body(Gameworld, this, newTemplate);
		newBody.SuspendForCharacter();
		Gameworld.SaveManager.AddInitialisation(newBody);
		Gameworld.SaveManager.Flush();
		if (newBody.Changed)
		{
			Gameworld.SaveManager.Flush();
		}

		var newForm = new CharacterForm(newBody, GetNextAvailableAlias(race.Name), _forms.Select(x => x.SortOrder).DefaultIfEmpty().Max() + 1)
		{
			AllowVoluntarySwitch = false
		};
		_forms.Add(newForm);
		Changed = true;
		form = newForm;
		whyNot = string.Empty;
		return true;
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
		var switchReason = string.Empty;
		if (currentBody is null || targetBody is null || !targetBody.TryPrepareSwitchFrom(currentBody, out _, out switchReason))
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
		    !newBody.TryPrepareSwitchFrom(oldBody, out var switchPlan, out _))
		{
			return false;
		}

		_isSwitchingBodies = true;
		try
		{
			PrepareForBodySwitch();
			oldBody.SuspendForCharacter();
			Body = newBody;
			newBody.ActivateForCharacter();
			newBody.ApplySwitchPlan(switchPlan);
			_handedness = Body.Handedness;
			_gender = Body.Gender;
			PostProcessBodySwitch();
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
