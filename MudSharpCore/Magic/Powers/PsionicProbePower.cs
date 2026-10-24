#nullable enable
using MudSharp.Body.Traits;
using MudSharp.Character.Name;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Framework.Units;

namespace MudSharp.Magic.Powers;

/// <summary>Reads progressively deeper information from a contacted mind.</summary>
public sealed class PsionicProbePower : PsionicTargetedPowerBase
{
	public override string PowerType => "Psionic Probe";
	public override string DatabaseType => "psionicprobe";
	protected override string DefaultVerb => "probe";
	public IFutureProg? InformationProg { get; private set; }
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psionicprobe", (row, world) => new PsionicProbePower(row, world));
		MagicPowerFactory.RegisterBuilderLoader("psionicprobe", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicProbePower(world, school, name, trait)));
	}
	private PsionicProbePower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name, trait)
	{
		PowerDistance = MagicPowerDistance.AnyConnectedMind;
		Blurb = "Read the person behind a contacted mind";
		_showHelpText = $"Use {school.SchoolVerb} probe <contacted target>. Skill reveals physical details (10), attributes (30), identity, plans, affiliations and contacts (50), then any additional memories configured by your world (60+). The target must be of the same race.";
		DoDatabaseInsert();
	}
	private PsionicProbePower(MagicPower row, IFuturemud world) : base(row, world)
	{
		var id = (long?)XElement.Parse(row.Definition).Element("InformationProg") ?? 0;
		InformationProg = id == 0 ? null : world.FutureProgs.Get(id) ?? throw new ApplicationException("Invalid Probe information prog.");
	}
	protected override XElement SaveDefinition() => SaveTargetedDefinition(new XElement("InformationProg", InformationProg?.Id ?? 0));
	protected override string SubtypeHelpText => base.SubtypeHelpText + "\n\t#3information <prog|none>#0 - text prog (actor, target, skill) for additional memories";
	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb) => sb.AppendLine($"Additional information prog: {InformationProg?.MXPClickableFunctionName() ?? "None"}");
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PeekSpeech().EqualTo("information")) return base.BuildingCommand(actor, command);
		command.PopSpeech();
		if (command.SafeRemainingArgument.EqualTo("none")) InformationProg = null;
		else
		{
			var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Text,
				[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Number]).LookupProg();
			if (prog is null) return false;
			InformationProg = prog;
		}
		Changed = true;
		actor.Send("The additional Probe information prog has been updated.");
		return true;
	}
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (!TryPrepareTarget(actor, command, "Whose mind do you want to probe?", out var target) || target is null) return;
		if (!actor.Race.SameRace(target.Race) || !PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target))
		{
			actor.Send("You cannot make sense of that mind."); return;
		}
		var outcome = CheckPower(actor, target, CheckType.MindLookPower);
		if (outcome < MinimumSuccessThreshold) { SendFailure(actor, target); ConsumePowerCosts(actor, Verb); return; }
		var skill = Math.Clamp(actor.TraitValue(SkillCheckTrait), 0, 100);
		var sb = new StringBuilder("You probe the mind and learn:\n");
		if (skill >= 10)
		{
			sb.AppendLine($"Age: {target.AgeInYears.ToString("N0", actor)} years");
			sb.AppendLine($"Height: {Gameworld.UnitManager.DescribeExact(target.Height, UnitType.Length, actor)}");
			sb.AppendLine($"Weight: {Gameworld.UnitManager.DescribeExact(target.Weight, UnitType.Mass, actor)}");
		}
		if (skill >= 30)
			foreach (var attribute in target.Race.Attributes)
				sb.AppendLine($"{attribute.Name}: {target.TraitValue(attribute).ToString("N0", actor)}");
		if (skill >= 50)
		{
			if (target.IsPlayerCharacter) sb.AppendLine($"True name: {target.PersonalName.GetName(NameStyle.FullName)}");
			sb.AppendLine($"Immediate aims: {target.ShortTermPlan}");
			sb.AppendLine($"Long-term aims: {target.LongTermPlan}");
			foreach (var membership in target.ClanMemberships.Where(x => !x.IsArchivedMembership))
				sb.AppendLine($"Affiliation: {membership.Clan.FullName}, {membership.Rank.Name}; {membership.Appointments.Select(x => x.Name).ListToString()}");
			foreach (var contact in target.CombinedEffectsOfType<ConnectMindEffect>())
				sb.AppendLine($"Contacting: {contact.TargetCharacter.HowSeen(actor)}");
		}
		if (skill >= 60 && InformationProg is not null) sb.AppendLine(InformationProg.Execute<string>(actor, target, skill));
		actor.Send(sb.ToString());
		PsionicActivityNotifier.Notify(actor, this, "probing a mind", target);
		ConsumePowerCosts(actor, Verb);
	}
}
