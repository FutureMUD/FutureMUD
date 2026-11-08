#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.NPC;
using MudSharp.NPC.Templates;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.AI;

namespace MudSharp.Magic.SpellEffects;

public sealed partial class CreateNPCEffect
{
	private XElement? _choiceDefinition;
	private NamedChoice? _boundChoice;
	private sealed record NamedChoice(string Name, long Template, long UniqueTemplate, long TokenTag, int MaterialGrade, int MinimumGrade);
	private IEnumerable<NamedChoice> NamedChoices => _choiceDefinition?.Elements("Choice").Select(x => new NamedChoice(
		(string?)x.Attribute("name") ?? "", (long?)x.Attribute("template") ?? 0, (long?)x.Attribute("uniqueTemplate") ?? 0,
		(long?)x.Attribute("tokenTag") ?? 0, (int?)x.Attribute("materialGrade") ?? 1, (int?)x.Attribute("minimumGrade") ?? 1)) ?? [];
	private string? ChoiceDefinitionError()
	{
		if (_choiceDefinition is null) return null;
		try
		{
			var choices = NamedChoices.ToArray();
			if ((int?)_choiceDefinition.Attribute("version") != 1 || LifecycleMode is null || MissionKind != SpellNpcMissionKind.None || _count != "1" ||
				choices.Length is < 1 or > 128 || choices.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != choices.Length ||
				choices.Any(x => string.IsNullOrWhiteSpace(x.Name) || x.Name.Length > 100 || x.Template <= 0 || x.UniqueTemplate <= 0 ||
					x.MinimumGrade is < 1 or > 7 || Gameworld.Tags.Get(x.TokenTag) is null || x.MaterialGrade is < 1 or > 7))
				return "Named summons require unique names, valid templates, token/material tags, grades 1-7 and a single-creature lifecycle.";
			if (choices.Any(x => NativeNpcCreationEligibility.TemplateError(Gameworld.NpcTemplates.Get(x.Template), Gameworld) is not null))
				return "A named summon template is missing or not eligible for native creation.";
			if (LifecycleMode == SpellLifecycleMode.Permanent && choices.Any(x => Gameworld.NpcTemplates.Get(x.Template)!.ArtificialIntelligences.OfType<SpellCompanionAI>().Any()))
				return "Named spell companions require an active temporary lifecycle.";
			var control = _choiceDefinition.Element("ControlBreak");
			if (control is not null && choices.Any(x => Gameworld.NpcTemplates.Get(x.Template)!.ArtificialIntelligences.OfType<SpellCompanionAI>().Count() != 1))
				return "Control-break policy requires exactly one companion controller on every named template.";
			if (control is not null && ((int?)control.Attribute("base") is not (>= 0 and <= 101) ||
				(int?)control.Attribute("perFollower") is not (>= 0 and <= 101))) return "Invalid summon control-break percentages.";
			if (_choiceDefinition.Element("Lifetime") is { } lifetime &&
				((double?)lifetime.Attribute("secondsPerGrade") is not (> 0 and <= 86400) ||
				(int?)lifetime.Attribute("minimumExtraGrades") is not (>= 0 and <= 128) ||
				(int?)lifetime.Attribute("maximumExtraGrades") is not (>= 0 and <= 128) ||
				(int)lifetime.Attribute("minimumExtraGrades")! > (int)lifetime.Attribute("maximumExtraGrades")!))
				return "Invalid named summon lifetime range.";
			return null;
		}
		catch (Exception ex) when (ex is FormatException or OverflowException) { return "Invalid named summon configuration: " + ex.Message; }
	}
	private bool BindChoice(ICharacter caster, IReadOnlyList<SpellAdditionalParameter> parameters, out string? error)
	{
		error = ChoiceDefinitionError();
		if (error is not null || _choiceDefinition is null) return error is null;
		if (parameters.Count == 0) return _boundChoice is null;
		if (parameters.Count != 1 || parameters[0].ParameterName != "npc-choice" || parameters[0].Item is not string name ||
			NamedChoices.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) is not { } choice)
		{ error = "Choose one of: " + string.Join(", ", NamedChoices.Select(x => x.Name)); return false; }
		if (_boundChoice is not null) return _boundChoice == choice && ValidateChoice(caster, out error);
		_boundChoice = choice;
		if (!ValidateChoice(caster, out error)) return false;
		var native = (MagicSpell)Spell;
		var original = native.InventoryPlanTemplate.Phases.SelectMany(x => x.Actions).Cast<InventoryPlanAction>().ToArray();
		var plan = native.InventoryPlanTemplate.SaveToXml();
		var retained = new XElement("Action", new XAttribute("state", "retained"), new XAttribute("tag", choice.TokenTag),
			new XAttribute("secondtag", 0), new XAttribute("quantity", 1), new XAttribute("carriedonly", true), new XAttribute("originalreference", "summon.token"));
		if (plan.Element("Phase") is not { } phase) { phase = new XElement("Phase"); plan.Add(phase); }
		phase.AddFirst(retained);
		var replacement = new InventoryPlanTemplate(plan, Gameworld) { Options = native.InventoryPlanTemplate.Options };
		var copied = replacement.Phases.SelectMany(x => x.Actions).Cast<InventoryPlanAction>().Where(x => !Equals(x.OriginalReference, "summon.token")).ToArray();
		for (var i = 0; i < original.Length; i++) { copied[i].PrimaryItemSelector = original[i].PrimaryItemSelector; copied[i].SecondaryItemSelector = original[i].SecondaryItemSelector; }
		foreach (var action in replacement.Phases.SelectMany(x => x.Actions).OfType<InventoryPlanActionConsume>()) action.BindSelectedGrade(Equals(action.OriginalReference, (string?)_choiceDefinition.Attribute("materialReference")) ? choice.MaterialGrade : native.InvocationGrade!.Value);
		native.InventoryPlanTemplate = replacement;
		return true;
	}
	private bool ValidateChoice(ICharacter caster, out string? error)
	{
		error = null;
		if (_boundChoice is not { } choice) return true;
		if (Spell is not MagicSpell { InvocationGrade: { } grade } || grade < choice.MinimumGrade)
		{ error = $"This summon requires grade {choice.MinimumGrade} or greater."; return false; }
		if (Gameworld.Actors.OfType<INPC>().Any(x => x.Template.Id == choice.Template || x.Template.Id == choice.UniqueTemplate))
		{ error = "That unique creature is already present in the world."; return false; }
		return true;
	}
	private void ApplySummonControl(ICharacter creature, ICharacter caster)
	{
		if (_boundChoice is null || creature.EffectsOfType<SpellNpcControlEffect>().SingleOrDefault() is not { } bond) return;
		var control = _choiceDefinition?.Element("ControlBreak");
		var followers = Gameworld.Actors.Where(x => ReferenceEquals(x.Following, caster)).Count(x => x.EffectsOfType<SpellNpcControlEffect>().Any(effect =>
			!effect.ControlLost && effect.CreatorId == CharacterInstanceIdentityComparer.IdentityId(caster) && effect.SpellId == Spell.Id));
		var threshold = control is null ? 0 : (int)control.Attribute("base")! + followers * (int)control.Attribute("perFollower")!;
		if (Random.Shared.Next(1, 101) < threshold)
		{
			if (!bond.LoseControl(caster)) throw new InvalidOperationException("Summon loss of control could not be persisted; do not replay creation.");
			caster.Send("The summoned creature breaks free of your control and turns on you!");
		}
		else if (!bond.SetOrder("follow", caster, caster)) throw new InvalidOperationException("Summon following order could not be persisted; do not replay creation.");
	}
	private bool BuildingCommandChoice(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		if (name.EqualTo("clear") && command.IsFinished)
		{
			_choiceDefinition = null; _boundChoice = null; Spell.Changed = true;
			actor.Send("Named summon choices cleared."); return true;
		}
		if (LifecycleMode is null || MissionKind != SpellNpcMissionKind.None || _count != "1" || string.IsNullOrWhiteSpace(name))
		{ actor.Send("Set a single-creature lifecycle without a mission before configuring choices."); return false; }
		var templateText = command.PopSpeech();
		if (templateText.EqualTo("none") && command.IsFinished)
		{
			_choiceDefinition?.Elements("Choice").Where(x => name.EqualTo((string?)x.Attribute("name") ?? "")).Remove();
			if (_choiceDefinition?.Elements("Choice").Any() == false) _choiceDefinition = null;
			Spell.Changed = true; actor.Send("Named summon choice removed."); return true;
		}
		var template = Gameworld.NpcTemplates.GetByIdOrName(templateText);
		var token = Gameworld.Tags.GetByIdOrName(command.PopSpeech());
		if (template is null || token is null || !int.TryParse(command.PopSpeech(), out var minimum) || minimum is < 1 or > 7 ||
			!int.TryParse(command.PopSpeech(), out var materialGrade) || materialGrade is < 1 or > 7 || !command.IsFinished ||
			NativeNpcCreationEligibility.TemplateError(template, Gameworld) is not null)
		{ actor.Send("Use choice <name> <approved NPC template> <retained token tag> <minimum grade 1-7> <material grade 1-7>, choice <name> none, or choice clear."); return false; }
		_choiceDefinition ??= new XElement("NamedChoices", new XAttribute("version", 1));
		_choiceDefinition.Elements("Choice").Where(x => name.EqualTo((string?)x.Attribute("name") ?? "")).Remove();
		_choiceDefinition.Add(new XElement("Choice", new XAttribute("name", name), new XAttribute("template", template.Id),
			new XAttribute("uniqueTemplate", template.Id), new XAttribute("tokenTag", token.Id), new XAttribute("minimumGrade", minimum), new XAttribute("materialGrade", materialGrade)));
		Spell.Changed = true; actor.Send("Named summon choice updated."); return true;
	}
	private bool BuildingCommandChoicePolicy(ICharacter actor, string mode, StringStack command)
	{
		if (_choiceDefinition is null) { actor.Send("Configure a named summon choice first."); return false; }
		var original = new XElement(_choiceDefinition);
		var value = command.PopSpeech();
		if (mode == "choicematerial")
		{
			if (!command.IsFinished || value.Length > 128 || string.IsNullOrWhiteSpace(value)) return false;
			_choiceDefinition.SetAttributeValue("materialReference", value.EqualTo("none") ? null : value);
		}
		else if (value.EqualTo("none") && command.IsFinished) _choiceDefinition.Element(mode == "choicecontrol" ? "ControlBreak" : "Lifetime")?.Remove();
		else if (mode == "choicecontrol" && int.TryParse(value, out var baseline) && int.TryParse(command.PopSpeech(), out var perFollower) && command.IsFinished)
		{
			_choiceDefinition.Element("ControlBreak")?.Remove();
			_choiceDefinition.Add(new XElement("ControlBreak", new XAttribute("base", baseline), new XAttribute("perFollower", perFollower)));
		}
		else if (mode == "choicelifetime" && double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var seconds) &&
			int.TryParse(command.PopSpeech(), out var minimum) && int.TryParse(command.PopSpeech(), out var maximum) && command.IsFinished)
		{
			_choiceDefinition.Element("Lifetime")?.Remove();
			_choiceDefinition.Add(new XElement("Lifetime", new XAttribute("secondsPerGrade", seconds), new XAttribute("minimumExtraGrades", minimum), new XAttribute("maximumExtraGrades", maximum)));
		}
		else { actor.Send("Use choicecontrol <base threshold> <threshold per follower>|none, choicelifetime <seconds per grade> <minimum extra grades> <maximum extra grades>|none, or choicematerial <material action reference>|none."); return false; }
		if (ChoiceDefinitionError() is { } error) { _choiceDefinition = original; actor.Send(error); return false; }
		Spell.Changed = true; actor.Send("Named summon policy updated."); return true;
	}
}
