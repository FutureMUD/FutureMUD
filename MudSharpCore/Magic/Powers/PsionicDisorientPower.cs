#nullable enable
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public sealed class PsionicDisorientPower : PsionicTargetedPowerBase
{
	public override string PowerType => "Psionic Disorientation";
	public override string DatabaseType => "psionicdisorient";
	protected override string DefaultVerb => "disorient";
	public ITraitDefinition AffectedTrait { get; private set; }
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psionicdisorient", (row, world) => new PsionicDisorientPower(row, world));
		MagicPowerFactory.RegisterBuilderLoader("psionicdisorient", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicDisorientPower(world, school, name, trait)));
	}
	private PsionicDisorientPower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name, trait)
	{
		AffectedTrait = trait;
		Blurb = "Disrupt coordination and loosen a target's grip";
		_showHelpText = $"Use {school.SchoolVerb} disorient <target>.";
		DoDatabaseInsert();
	}
	private PsionicDisorientPower(MagicPower row, IFuturemud world) : base(row, world)
	{
		AffectedTrait = world.Traits.Get((long?)XElement.Parse(row.Definition).Element("AffectedTrait") ?? SkillCheckTrait.Id)
			?? throw new ApplicationException("Invalid disorientation attribute.");
	}
	protected override XElement SaveDefinition() => SaveTargetedDefinition(new XElement("AffectedTrait", AffectedTrait.Id));
	protected override string SubtypeHelpText => base.SubtypeHelpText + "\n\t#3affectedtrait <trait>#0 - sets the impaired attribute";
	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb) => sb.AppendLine($"Impaired trait: {AffectedTrait.Name.ColourName()}");
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PeekSpeech().EqualTo("affectedtrait")) return base.BuildingCommand(actor, command);
		command.PopSpeech();
		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null) { actor.Send("There is no such trait."); return false; }
		AffectedTrait = trait; Changed = true; actor.Send($"Disorientation now impairs {trait.Name.ColourName()}."); return true;
	}
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whom do you want to disorient?", out var target) || target is null) return;
		if (target == actor || !PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target)) { actor.Send("You cannot disorient that mind."); return; }
		var outcome = CheckPower(actor, target, CheckType.MindSayPower);
		if (outcome < MinimumSuccessThreshold) { SendFailure(actor, target); ConsumePowerCosts(actor, Verb); return; }
		target.RemoveAllEffects<PsionicDisorientation>(fireRemovalAction: true);
		target.AddEffect(new PsionicDisorientation(target, this, Math.Max(1, Math.Floor(Math.Clamp(actor.TraitValue(SkillCheckTrait), 0, 100) / 20))),
			TimeSpan.FromMinutes(10 * Random.Shared.Next(1, 3)));
		foreach (var item in target.Body.WieldedItems.ToArray())
			if (target.Body.CanDrop(item, 0)) target.Body.Drop(item);
		target.RemoveAllEffects<IGrappling>(fireRemovalAction: true);
		target.Send("Your coordination falters and your grip slackens.");
		SendSuccess(actor, target);
		PsionicActivityNotifier.Notify(actor, this, "disorientation", target);
		ConsumePowerCosts(actor, Verb);
	}
}
