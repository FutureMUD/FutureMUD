#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Planes;

namespace MudSharp.Magic.Powers;

/// <summary>Maintains a psionic clan channel and permits same-plane communion with its online members.</summary>
public sealed class PsionicCathexisPower : MagicPowerBase
{
	public override string PowerType => "Cathexis";
	public override string DatabaseType => "psioniccathexis";
	public Difficulty DetectableWithDetectMagic { get; private set; } = Difficulty.Normal;
	public bool MaintainCathexis(ICharacter actor) => CanInvokePower(actor) && PayStunUpkeep(actor, 1.0);
	public override IEnumerable<string> Verbs => ["cathexis", "endcathexis", "commune"];
	private ITraitDefinition SkillCheckTrait { get; set; }
	private Difficulty SkillCheckDifficulty { get; set; }
	private Outcome MinimumSuccessThreshold { get; set; }

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psioniccathexis", (power, world) => new PsionicCathexisPower(power, world));
		MagicPowerFactory.RegisterBuilderLoader("psioniccathexis", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicCathexisPower(world, school, name, trait)));
	}

	private PsionicCathexisPower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name)
	{
		IsPsionic = true; EnablePsionicTraceDefaults();
		Blurb = "Open a mental communion channel with a clan";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} CATHEXIS [clan] to open a clan channel, COMMUNE <message> to address eligible members on the same plane, or ENDCATHEXIS to close it.";
		SkillCheckTrait = trait; SkillCheckDifficulty = Difficulty.Normal; MinimumSuccessThreshold = Outcome.MinorPass;
		DoDatabaseInsert();
	}
	private PsionicCathexisPower(MagicPower model, IFuturemud world) : base(model, world)
	{
		var root = XElement.Parse(model.Definition);
		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(root.Element("SkillCheckTrait")?.Value ?? "0")) ?? throw new ApplicationException("Invalid cathexis skill trait.");
		SkillCheckDifficulty = (Difficulty)int.Parse(root.Element("SkillCheckDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		MinimumSuccessThreshold = (Outcome)int.Parse(root.Element("MinimumSuccessThreshold")?.Value ?? ((int)Outcome.MinorPass).ToString());
	}
	protected override XElement SaveDefinition()
	{
		var root = new XElement("Definition", new XElement("SkillCheckTrait", SkillCheckTrait.Id), new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty), new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold));
		AddBaseDefinition(root); return root;
	}
	protected override string SubtypeHelpText => @"	#3skill <which>#0 - sets the skill used to establish cathexis
	#3difficulty <difficulty>#0 - sets cathexis difficulty
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
				SkillCheckTrait = trait; Changed = true; actor.OutputHandler.Send($"Cathexis now uses {trait.Name.ColourName()}."); return true;
			case "difficulty":
				if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty difficulty)) { actor.OutputHandler.Send("That is not a valid difficulty."); return false; }
				SkillCheckDifficulty = difficulty; Changed = true; actor.OutputHandler.Send($"Cathexis now checks at {difficulty.DescribeColoured()}."); return true;
			case "threshold":
				if (!command.SafeRemainingArgument.TryParseEnum(out Outcome outcome)) { actor.OutputHandler.Send("That is not a valid outcome."); return false; }
				MinimumSuccessThreshold = outcome; Changed = true; actor.OutputHandler.Send($"Cathexis now requires {outcome.DescribeColour()}."); return true;
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (verb.EqualTo("endcathexis"))
		{
			var active = actor.EffectsOfType<PsionicCathexisEffect>().FirstOrDefault(x => x.Power == this);
			if (active is null) { actor.OutputHandler.Send("You are not in a cathexic state."); return; }
		actor.RemoveEffect(active, true); actor.OutputHandler.Send("You release the clan communion channel."); ConsumePowerCosts(actor, "endcathexis"); return;
		}
		if (verb.EqualTo("commune")) { Commune(actor, command); return; }
		Begin(actor, command);
	}

	private void Begin(ICharacter actor, StringStack command)
	{
		if (actor.EffectsOfType<PsionicCathexisEffect>().Any(x => x.Power == this))
		{
			actor.OutputHandler.Send("You are already in a cathexic state."); return;
		}
		var clans = actor.ClanMemberships.Select(x => x.Clan).Distinct().ToList();
		if (clans.Count == 0) { actor.OutputHandler.Send("You must belong to a clan to establish cathexis."); return; }
		var clanText = command.SafeRemainingArgument;
		var clan = command.IsFinished ? clans.Count == 1 ? clans[0] : null : clans.FirstOrDefault(x => x.Name.EqualTo(clanText) || x.Id.ToString() == clanText);
		if (clan is null) { actor.OutputHandler.Send(clans.Count > 1 ? $"Which clan do you mean: {clans.Select(x => x.Name).ListToString()}?" : "You do not belong to that clan."); return; }
		if (!HandleGeneralUseRestrictions(actor)) return;
		var cost = CanAffordToInvokePower(actor, "cathexis");
		if (!cost.Truth) { actor.OutputHandler.Send($"You lack sufficient {cost.missing.Name.Colour(Telnet.BoldMagenta)}."); return; }
		if (!CanInvokePower(actor)) { actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor)?.ToString() ?? "You cannot use that power right now."); return; }
		var outcome = Gameworld.GetCheck(CheckType.MagicTelepathyCheck).Check(actor, SkillCheckDifficulty, SkillCheckTrait);
		if (outcome < MinimumSuccessThreshold) { actor.OutputHandler.Send("You cannot settle your mind into a cathexic state."); ConsumePowerCosts(actor, "cathexis"); return; }
		actor.AddEffect(new PsionicCathexisEffect(actor, this, clan.Id));
		actor.OutputHandler.Send($"Your mind opens into a cathexic communion with {clan.Name.ColourName()}.");
		PsionicActivityNotifier.Notify(actor, this, "cathexis");
		ConsumePowerCosts(actor, "cathexis");
	}

	private void Commune(ICharacter actor, StringStack command)
	{
		var active = actor.EffectsOfType<PsionicCathexisEffect>().FirstOrDefault(x => x.Power == this);
		if (active is null) { actor.OutputHandler.Send("You must first establish cathexis with a clan."); return; }
		if (command.IsFinished) { actor.OutputHandler.Send("What do you wish to commune?"); return; }
		var message = command.SafeRemainingArgument.Sanitise().NormaliseSpacing().ProperSentences().Fullstop();
		if (string.IsNullOrWhiteSpace(message)) { actor.OutputHandler.Send("Your message has no readable content."); return; }
		var clan = Gameworld.Clans.Get(active.ClanId);
		if (clan is null || !actor.ClanMemberships.Any(x => x.Clan.Id == active.ClanId))
		{
			actor.RemoveEffect(active, true); actor.OutputHandler.Send("Your clan communion has ended."); return;
		}
		if (!HandleGeneralUseRestrictions(actor) || !CanInvokePower(actor)) return;
		var cost = CanAffordToInvokePower(actor, "commune");
		if (!cost.Truth)
		{
			actor.OutputHandler.Send($"You lack sufficient {cost.missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}
		foreach (var recipient in clan.Memberships.Select(x => Gameworld.Characters.Get(x.MemberId)).OfType<ICharacter>()
			.Where(x => x != actor && x.Location is not null && actor.SharesPlaneWith(x) &&
			            !x.State.IsAsleep() &&
			            PsionicTrafficHelper.CanSendPsionicTraffic(actor, x)).Distinct())
			recipient.OutputHandler.Send($"A thought from {PsionicTrafficHelper.SourceDescription(actor, recipient, School)} reaches you through the clan communion: {message}");
		actor.OutputHandler.Send($"You commune with the eligible members of {clan.Name.ColourName()}: {message}");
		PsionicActivityNotifier.Notify(actor, this, "clan communion");
		ConsumePowerCosts(actor, verb: "commune");
	}
}
