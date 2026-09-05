#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;

namespace MudSharp.Magic.Powers;

public sealed class SelectiveForgettingPower : PsychicTechniquePower
{
	protected override string DefaultVerb => "forgetting";
	public static void RegisterLoader() => Register("forgetting", (m,w) => new SelectiveForgettingPower(m,w), (w,s,n,t) => new SelectiveForgettingPower(w,s,n,t));
	private SelectiveForgettingPower(Models.MagicPower m, IFuturemud w) : base(m,w) { }
	private SelectiveForgettingPower(IFuturemud w, IMagicSchool s, string n, ITraitDefinition t) : base(w,s,n,t) =>
		Initialise("incidents; virtual <incident>; or <target> <witness|skill|knowledge|recognition> <incident or subject>.");

	public static List<Crime> KnownIncidents(ICharacter actor)
	{
		var id = CharacterInstanceIdentityComparer.IdentityId(actor);
		return actor.Gameworld.LegalAuthorities.SelectMany(x => x.KnownCrimes.Concat(x.UnknownCrimes).Concat(x.StaleCrimes))
			.OfType<Crime>().Where(x => x.CriminalId == id || x.VictimId == id || x.CanWitnessRecall(id))
			.Distinct().OrderByDescending(x => x.RealTimeOfCrime).ThenBy(x => x.Id).ToList();
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var first = command.PeekSpeech();
		if (first.EqualTo("incidents"))
		{
			var incidents = KnownIncidents(actor);
			actor.OutputHandler.Send(incidents.Count == 0 ? "You recall no eligible incidents." :
				string.Join("\n", incidents.Select((x, i) => $"{(i + 1).ToString("N0", actor)}: {x.DescribeCrime(actor)}")));
			return;
		}
		if (first.EqualTo("virtual")) { command.PopSpeech(); ForgetVirtual(actor, command); return; }
		if (!TryPrepareTarget(actor, command, "Whose memory do you want to suppress?", out var target) || target is null) return;
		var mode = command.PopForSwitch();
		if (mode == "witness")
		{
			var crime = SelectIncident(actor, command.PopSpeech());
			if (crime is null) { actor.OutputHandler.Send("Select an incident from your incidents list."); return; }
			if (!Resolve(actor, target, MentalActionKind.WitnessForgetting, true, out _)) return;
			foreach (var memory in crime.WitnessMemories.Where(x => x.Kind == CrimeWitnessSourceKind.Character &&
			         x.SourceId == CharacterInstanceIdentityComparer.IdentityId(target)))
			{
				crime.ForgetWitness(memory, actor, Duration, Permanent);
				if (!Permanent) target.AddEffect(new WitnessRecallSuppressionEffect(target, this, crime, memory), Duration);
			}
			actor.OutputHandler.Send("You press the incident out of that mind's reach.");
			Complete(actor, target, "witness forgetting");
			return;
		}
		long id;
		switch (mode)
		{
			case "skill":
				var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
				if (trait is null || trait.TraitType is not (TraitType.Skill or TraitType.TheoreticalSkill or TraitType.DerivedSkill)) { actor.OutputHandler.Send("Specify a skill definition."); return; }
				id = trait.Id;
				break;
			case "knowledge":
				var knowledge = Gameworld.Knowledges.GetByIdOrName(command.SafeRemainingArgument);
				if (knowledge is null) { actor.OutputHandler.Send("Specify a knowledge definition."); return; }
				id = knowledge.Id;
				break;
			case "recognition":
				var person = actor.TargetActor(command.SafeRemainingArgument);
				if (person is null) { actor.OutputHandler.Send("Specify a person you can identify here."); return; }
				id = CharacterInstanceIdentityComparer.IdentityId(person);
				break;
			default: actor.OutputHandler.Send("Choose witness, skill, knowledge, or recognition."); return;
		}
		if (!Resolve(actor, target, MentalActionKind.Influence, true, out _)) return;
		var suppression = mode == "skill" ? new PsychicSkillSuppressionEffect(target, id) : new PsychicSuppressionEffect(target, mode, id);
		suppression.OriginPowerId = Id;
		target.AddEffect(suppression, Duration);
		actor.OutputHandler.Send("You place a veil over that part of the mind.");
		Complete(actor, target, "selective forgetting");
	}

	private static Crime? SelectIncident(ICharacter actor, string text) => int.TryParse(text, out var index) && index > 0
		? KnownIncidents(actor).ElementAtOrDefault(index - 1) : null;

	private void ForgetVirtual(ICharacter actor, StringStack command)
	{
		if (!HandleGeneralUseRestrictions(actor) || !CanAffordToInvokePower(actor, Verb).Truth) return;
		var crime = SelectIncident(actor, command.PopSpeech());
		if (crime is null || crime.CrimeLocation != actor.Location || RuntimeClock.UtcNow - crime.RealTimeOfCrime > TimeSpan.FromHours(24))
		{ actor.OutputHandler.Send("You must be at the scene of a known incident from the last day."); return; }
		if (CanInvokePowerProg.ExecuteBool(actor, actor) == false ||
		    MagicInterdictionHelper.GetInterdiction(actor, actor.Location, School, false) is not null) return;
		foreach (var memory in crime.WitnessMemories.Where(x => x.Kind == CrimeWitnessSourceKind.Virtual && !x.PermanentlyForgotten).Take(VirtualSourceLimit))
		{
			if (!CanAffordToInvokePower(actor, Verb).Truth) break;
			ConsumePowerCosts(actor, Verb);
			var outcome = CheckPower(actor, actor, CheckType.MagicTelepathyCheck);
			if (outcome.Outcome >= MinimumSuccessThreshold && outcome.Outcome > VirtualResistance) crime.ForgetWitness(memory, actor, Duration, Permanent);
			Complete(actor, null, "virtual witness forgetting");
		}
		actor.OutputHandler.Send("You reach into the remembered impressions of the incident's bystanders.");
	}
}
