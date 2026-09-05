#nullable enable

using MudSharp.Models;

namespace MudSharp.Magic.Powers;

public sealed class SpellBackedPower : MagicPowerBase
{
	public override string PowerType => "Spell Backed";
	public override string DatabaseType => "spellbacked";
	public string Verb { get; private set; } = "invoke";
	private long _spellId;
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
		AddBaseDefinition(root);
		return root;
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (Spell is null || !Spell.ReadyForGame || Spell.School != School || Spell.Trigger is not ICastMagicTrigger trigger ||
		    !Spell.CharacterKnowsSpell(actor) || CanInvokePowerProg.ExecuteBool(actor) == false)
		{
			actor.OutputHandler.Send("You cannot invoke the spell associated with this power.");
			return;
		}
		using var invocation = new SpellPowerInvocation(actor, Spell, this);
		trigger.DoTriggerCast(actor, command);
		if (invocation.Result.Status != MagicInvocationStatus.Refused)
		{
			PsionicActivityNotifier.Notify(actor, this, $"the invocation of {Name}");
		}
	}

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Verb: {Verb.ColourCommand()}");
		sb.AppendLine($"Spell: {(Spell?.Name ?? "Missing spell").ColourName()}");
		sb.AppendLine("Casting costs and cooldowns are supplied by the spell.");
	}

	protected override string SubtypeHelpText => @"	#3verb <word>#0 - sets the invocation verb
	#3spell <spell>#0 - selects a cast-trigger spell in the same school";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
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
