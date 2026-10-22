#nullable enable

using MudSharp.Models;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public sealed class SpellBackedPower : MagicPowerBase
{
	public override string PowerType => "Spell Backed";
	public override string DatabaseType => "spellbacked";
	public string Verb { get; private set; } = "invoke";
	private long _spellId;
	public int? InvocationGrade { get; private set; }
	public IMagicSpell? Spell => Gameworld.MagicSpells.Get(_spellId);
	public override IEnumerable<string> Verbs => [Verb];

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("spellbacked", (power, world) => new SpellBackedPower(power, world));
		MagicPowerFactory.RegisterBuilderLoader("spellbacked", (world, school, name, actor, command) =>
		{
			var spell = world.MagicSpells.GetByIdOrName(command.SafeRemainingArgument);
			if (spell?.Trigger is not ICastMagicTrigger || spell.School != school)
			{
				actor.OutputHandler.Send("Specify a cast-trigger spell belonging to this school.");
				return null;
			}
			return new SpellBackedPower(world, school, name, spell);
		});
	}

	private SpellBackedPower(MagicPower power, IFuturemud world) : base(power, world)
	{
		var root = XElement.Parse(power.Definition);
		Verb = root.Element("Verb")?.Value ?? "invoke";
		_spellId = long.Parse(root.Element("Spell")?.Value ?? "0");
		InvocationGrade = (int?)root.Element("InvocationGrade");
		if (InvocationGrade is < 1 or > 7) throw new FormatException("Spell-backed invocation grade must be 1-7.");
	}

	private SpellBackedPower(IFuturemud world, IMagicSchool school, string name, IMagicSpell spell) : base(world, school, name)
	{
		_spellId = spell.Id;
		IsPsionic = true;
		EnablePsionicTraceDefaults();
		Blurb = "Invoke a configured spell through a power verb";
		_showHelpText = "Use this power's verb followed by the spell power level and normal spell arguments.";
		DoDatabaseInsert();
	}

	protected override XElement SaveDefinition()
	{
		var root = new XElement("Definition", new XElement("Verb", Verb), new XElement("Spell", _spellId));
		if (InvocationGrade is { } grade) root.Add(new XElement("InvocationGrade", grade));
		AddBaseDefinition(root);
		return root;
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (Spell is null || !Spell.ReadyForGame || Spell.School != School || Spell.Trigger is not ICastMagicTrigger trigger ||
		    !Spell.CharacterKnowsSpell(actor) || CanInvokePower(actor) == false)
		{
			actor.OutputHandler.Send("You cannot invoke the spell associated with this power.");
			return;
		}
		var invokedSpell = Spell;
		if (InvocationGrade is { } grade)
		{
			if (Spell is not MagicSpell { GradeProfile: { } profile } native || profile.Grades.SingleOrDefault(x => x.Grade == grade) is not { } selected)
			{
				actor.OutputHandler.Send("The configured spell has no matching invocation grade.");
				return;
			}
			var resolution = SpellTargetCapture.Resolve(actor, native, selected.Power, command, true);
			if (resolution is null) return;
			invokedSpell = native.CastingCopy(actor, native.CastingTrait, grade, selected.Power, native.CastingDifficulty, grade);
			using var selectedInvocation = new SpellPowerInvocation(actor, invokedSpell, this);
			invokedSpell.CastSpell(actor, resolution.Target, selected.Power, resolution.Parameters);
			if (selectedInvocation.Result.Status != MagicInvocationStatus.Refused)
			{
				ConsumePowerCosts(actor, verb);
				PsionicActivityNotifier.Notify(actor, this, $"the invocation of {Name}");
			}
			return;
		}
		using var invocation = new SpellPowerInvocation(actor, invokedSpell, this);
		trigger.DoTriggerCast(actor, command);
		if (invocation.Result.Status != MagicInvocationStatus.Refused)
		{
			ConsumePowerCosts(actor, verb);
			PsionicActivityNotifier.Notify(actor, this, $"the invocation of {Name}");
		}
	}

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Verb: {Verb.ColourCommand()}");
		sb.AppendLine($"Spell: {(Spell?.Name ?? "Missing spell").ColourName()}");
		sb.AppendLine($"Invocation grade: {InvocationGrade?.ToString() ?? "Legacy power argument"}");
		sb.AppendLine("Casting costs and cooldowns are supplied by the spell.");
	}

	protected override string SubtypeHelpText => @"	#3verb <word>#0 - sets the invocation verb
	#3spell <spell>#0 - selects a cast-trigger spell in the same school
	#3grade <1-7|none>#0 - uses a selected-grade spell copy, or the legacy power argument";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "grade":
				var text = command.PopSpeech();
				if (text.EqualTo("none")) InvocationGrade = null;
				else if (int.TryParse(text, out var selectedGrade) && selectedGrade is >= 1 and <= 7) InvocationGrade = selectedGrade;
				else return false;
				Changed = true;
				actor.Send("Spell-backed invocation grade updated.");
				return true;
			case "cost":
				actor.OutputHandler.Send("Configure casting costs on the underlying spell.");
				return false;
			case "verb":
				var verb = command.PopSpeech().ToLowerInvariant();
				if (string.IsNullOrWhiteSpace(verb) || verb.Any(char.IsWhiteSpace) || !command.IsFinished) return false;
				Verb = verb;
				Changed = true;
				actor.OutputHandler.Send($"The invocation verb is now {Verb.ColourCommand()}.");
				return true;
			case "spell":
				var spell = Gameworld.MagicSpells.GetByIdOrName(command.SafeRemainingArgument);
				if (spell?.Trigger is not ICastMagicTrigger || spell.School != School) return false;
				_spellId = spell.Id;
				Changed = true;
				actor.OutputHandler.Send($"The power now invokes {spell.Name.ColourName()}.");
				return true;
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}
}
