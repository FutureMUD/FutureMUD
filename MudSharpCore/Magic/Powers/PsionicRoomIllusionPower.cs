#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

/// <summary>Projects authored sensory narration to the current room without creating physical objects.</summary>
public sealed class PsionicRoomIllusionPower : MagicPowerBase
{
	public override string PowerType => "Room Illusion";
	public override string DatabaseType => "psionicillusion";
	public override IEnumerable<string> Verbs => ["illusion"];
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psionicillusion", (power, world) => new PsionicRoomIllusionPower(power, world));
		MagicPowerFactory.RegisterBuilderLoader("psionicillusion", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicRoomIllusionPower(world, school, name, trait)));
	}
	private ITraitDefinition SkillCheckTrait { get; set; }
	private Difficulty SkillCheckDifficulty { get; set; }
	private Outcome MinimumSuccessThreshold { get; set; }
	private PsionicRoomIllusionPower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name)
	{
		IsPsionic = true; EnablePsionicTraceDefaults();
		Blurb = "Project a sensory illusion to everyone in the room";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} ILLUSION <description> to project a brief sensory illusion into everyone's perception in your room.";
		SkillCheckTrait = trait; SkillCheckDifficulty = Difficulty.Normal; MinimumSuccessThreshold = Outcome.MinorPass;
		DoDatabaseInsert();
	}
	private PsionicRoomIllusionPower(MagicPower model, IFuturemud world) : base(model, world)
	{
		var root = XElement.Parse(model.Definition);
		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(root.Element("SkillCheckTrait")?.Value ?? "0")) ?? throw new ApplicationException("Invalid skill trait for room illusion.");
		SkillCheckDifficulty = (Difficulty)int.Parse(root.Element("SkillCheckDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		MinimumSuccessThreshold = (Outcome)int.Parse(root.Element("MinimumSuccessThreshold")?.Value ?? ((int)Outcome.MinorPass).ToString());
	}
	protected override XElement SaveDefinition()
	{
		var root = new XElement("Definition", new XElement("SkillCheckTrait", SkillCheckTrait.Id), new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty), new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold));
		AddBaseDefinition(root); return root;
	}
	protected override string SubtypeHelpText => @"	#3skill <which>#0 - sets the skill used for projection
	#3difficulty <difficulty>#0 - sets projection difficulty
	#3threshold <outcome>#0 - sets the minimum success outcome";
	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Skill: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Threshold: {MinimumSuccessThreshold.DescribeColour()}");
	}
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "skill": case "trait":
				var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
				if (trait is null) { actor.OutputHandler.Send("That is not a valid skill or attribute."); return false; }
				SkillCheckTrait = trait; Changed = true; actor.OutputHandler.Send($"The illusion now uses {trait.Name.ColourName()}."); return true;
			case "difficulty":
				if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty difficulty)) { actor.OutputHandler.Send("That is not a valid difficulty."); return false; }
				SkillCheckDifficulty = difficulty; Changed = true; actor.OutputHandler.Send($"The illusion check is now {difficulty.DescribeColoured()}."); return true;
			case "threshold":
				if (!command.SafeRemainingArgument.TryParseEnum(out Outcome outcome)) { actor.OutputHandler.Send("That is not a valid outcome."); return false; }
				MinimumSuccessThreshold = outcome; Changed = true; actor.OutputHandler.Send($"The illusion now requires {outcome.DescribeColour()}."); return true;
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (command.IsFinished) { actor.OutputHandler.Send("What illusion do you want to project?"); return; }
		if (actor.Location is null) { actor.OutputHandler.Send("There is no room here to receive the illusion."); return; }
		var illusion = command.SafeRemainingArgument.Sanitise().NormaliseSpacing().ProperSentences().Fullstop();
		if (string.IsNullOrWhiteSpace(illusion)) { actor.OutputHandler.Send("The illusion needs readable text."); return; }
		if (!HandleGeneralUseRestrictions(actor)) return;
		var cost = CanAffordToInvokePower(actor, verb);
		if (!cost.Truth) { actor.OutputHandler.Send($"You lack sufficient {cost.missing.Name.Colour(Telnet.BoldMagenta)}."); return; }
		if (!CanInvokePower(actor)) { actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor)?.ToString() ?? "You cannot use that power right now."); return; }
		var outcome = Gameworld.GetCheck(CheckType.MagicTelepathyCheck).Check(actor, SkillCheckDifficulty, SkillCheckTrait);
		if (outcome < MinimumSuccessThreshold) { actor.OutputHandler.Send("You fail to shape the illusion clearly enough to project it."); ConsumePowerCosts(actor, verb); return; }
		foreach (var recipient in actor.Location.Characters.Where(x => x != actor && !x.AffectedBy<IIgnoreForceEffect>() &&
			!x.EffectsOfType<MindBarrierEffect>().Any(barrier => barrier.Applies(actor))).ToList())
			recipient.OutputHandler.Send($"A sensory illusion washes over your perceptions: {illusion}");
		actor.OutputHandler.Send("You project the illusion into the room.");
		PsionicActivityNotifier.Notify(actor, this, "a room-wide sensory illusion");
		ConsumePowerCosts(actor, verb);
	}
}
