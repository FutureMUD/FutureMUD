#nullable enable

using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

/// <summary>Common bounded configuration and resolution for the new psychic techniques.</summary>
public abstract class PsychicTechniquePower : PsionicTargetedPowerBase
{
	protected PsychicTechniquePower(Models.MagicPower model, IFuturemud world) : base(model, world)
	{
		var root = XElement.Parse(model.Definition);
		foreach (var setting in new[] { "Duration", "Amount", "Loss" })
		{
			if (root.Element(setting) is { } element && !double.IsFinite((double)element))
				throw new ApplicationException($"Psychic power {model.Id} has a non-finite {setting}.");
		}
		Duration = TimeSpan.FromSeconds(Math.Clamp((double?)root.Element("Duration") ?? 300, 1, 604800));
		Amount = Math.Clamp((double?)root.Element("Amount") ?? 10, 0, 1000);
		Loss = Math.Clamp((double?)root.Element("Loss") ?? 0.1, 0, 0.99);
		ResourceId = (long?)root.Element("Resource") ?? 0;
		Permanent = (bool?)root.Element("Permanent") ?? false;
		FeedbackMode = (string?)root.Element("FeedbackMode") ?? "resource";
		VirtualResistance = (Outcome)((int?)root.Element("VirtualResistance") ?? (int)Outcome.MinorPass);
		VirtualSourceLimit = Math.Clamp((int?)root.Element("VirtualSourceLimit") ?? 8, 1, 64);
		CircleMemberLimit = Math.Clamp((int?)root.Element("CircleMemberLimit") ?? 8, 2, 64);
		if (FeedbackMode is not ("warning" or "resource" or "stun") || VirtualResistance < Outcome.MajorFail || VirtualResistance > Outcome.MajorPass)
			throw new ApplicationException($"Psychic power {model.Id} has an invalid feedback mode or virtual resistance.");
	}
	protected PsychicTechniquePower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name, trait) { }
	public TimeSpan Duration { get; private set; } = TimeSpan.FromMinutes(5);
	public double Amount { get; private set; } = 10;
	public double Loss { get; private set; } = 0.1;
	public long ResourceId { get; private set; }
	public bool Permanent { get; private set; }
	public string FeedbackMode { get; private set; } = "resource";
	public Outcome VirtualResistance { get; private set; } = Outcome.MinorPass;
	public int VirtualSourceLimit { get; private set; } = 8;
	public int CircleMemberLimit { get; private set; } = 8;
	public override string DatabaseType => DefaultVerb;
	public override string PowerType => DefaultVerb.Proper();
	protected override XElement SaveDefinition() => SaveTargetedDefinition(new XElement("Duration", Duration.TotalSeconds),
		new XElement("Amount", Amount), new XElement("Loss", Loss), new XElement("Resource", ResourceId), new XElement("Permanent", Permanent), new XElement("FeedbackMode", FeedbackMode), new XElement("VirtualResistance", (int)VirtualResistance), new XElement("VirtualSourceLimit", VirtualSourceLimit), new XElement("CircleMemberLimit", CircleMemberLimit));
	protected bool CanMaintain(ICharacter actor)
	{
		var capacity = actor.Capabilities.Where(x => x.School == School).Select(x => x.ConcentrationAbility(actor)).DefaultIfEmpty(0).Max();
		if (capacity >= actor.Effects.OfType<IConcentrationConsumingEffect>().Sum(x => x.ConcentrationPointsConsumed) + 1) return true;
		actor.Send("You lack the concentration capacity to maintain another effect.");
		return false;
	}
	protected void Initialise(string help)
	{
		Blurb = help;
		_showHelpText = $"Use {School.SchoolVerb} {Verb} {help}";
		DoDatabaseInsert();
	}
	protected bool Resolve(ICharacter actor, ICharacter target, MentalActionKind kind, bool hostile, out MagicInvocationResult result)
	{
		var context = new MentalActionContext(actor, target, this, kind, hostile);
		if (!MentalActionService.CanAttempt(context) || !CanAffordToInvokePower(actor, Verb).Truth)
		{
			result = new(MagicInvocationStatus.Refused);
			SendFailure(actor, target);
			return false;
		}
		ConsumePowerCosts(actor, Verb);
		result = MentalActionService.Resolve(context, SkillCheckTrait, SkillCheckDifficulty, MinimumSuccessThreshold);
		if (result.Succeeded) return true;
		if (result.Status == MagicInvocationStatus.Failed) Complete(actor, target, "a resisted mental action");
		SendFailure(actor, target);
		return false;
	}
	protected void Complete(ICharacter actor, ICharacter? target, string activity)
	{
		PsionicActivityNotifier.Notify(actor, this, activity, target);
	}
	protected static void Register(string token, Func<Models.MagicPower, IFuturemud, IMagicPower> load,
		Func<IFuturemud, IMagicSchool, string, ITraitDefinition, IMagicPower> create)
	{
		MagicPowerFactory.RegisterLoader(token, load);
		MagicPowerFactory.RegisterBuilderLoader(token, (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command, trait => create(world, school, name, trait)));
	}
	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Duration: {Duration.Describe(actor)}; Amount: {Amount.ToString("N2", actor)}; Loss: {Loss.ToString("P0", actor)}");
		sb.AppendLine($"Resource: {(Gameworld.MagicResources.Get(ResourceId)?.Name ?? "Not set").ColourName()}; Permanent forgetting: {Permanent.ToColouredString()}");
	}
	protected override string SubtypeHelpText => base.SubtypeHelpText + @"
	#3duration <seconds>#0 - sets duration, from 1 second to 7 days
	#3amount <number>#0 - sets magnitude, from 0 to 1000
	#3loss <fraction>#0 - sets resource-transfer loss, from 0 to 0.99
	#3resource <resource>#0 - selects the affected resource
	#3permanent#0 - toggles permanent witness forgetting
	#3virtualresistance <outcome>#0 - sets the fixed opposing outcome for virtual witnesses
	#3virtuallimit <1-64>#0 - limits virtual sources checked per invocation
	#3circlelimit <2-64>#0 - sets the circle member limit, including the leader
	#3feedbackmode warning|resource|stun#0 - configures the reactive defence";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		if (option == "circlelimit")
		{
			if (!int.TryParse(command.PopSpeech(), out var limit) || limit < 2 || limit > 64) return false;
			CircleMemberLimit = limit;
		}
		else if (option == "virtualresistance")
		{
			if (!command.SafeRemainingArgument.TryParseEnum<Outcome>(out var outcome) || outcome < Outcome.MajorFail || outcome > Outcome.MajorPass) return false;
			VirtualResistance = outcome;
		}
		else if (option == "virtuallimit")
		{
			if (!int.TryParse(command.PopSpeech(), out var limit) || limit < 1 || limit > 64) return false;
			VirtualSourceLimit = limit;
		}
		else if (option == "feedbackmode")
		{
			var mode = command.PopForSwitch();
			if (mode is not ("warning" or "resource" or "stun")) { actor.Send("Choose warning, resource, or stun."); return false; }
			FeedbackMode = mode;
		}
		else if (option == "resource")
		{
			var resource = Gameworld.MagicResources.GetByIdOrName(command.SafeRemainingArgument);
			if (resource is null) { actor.OutputHandler.Send("There is no such magic resource."); return false; }
			ResourceId = resource.Id;
		}
		else if (option == "permanent") Permanent = !Permanent;
		else if (option is "duration" or "amount" or "loss")
		{
			if (!double.TryParse(command.SafeRemainingArgument, out var value) || !double.IsFinite(value) || value < (option == "duration" ? 1 : 0) ||
			    value > (option == "duration" ? 604800 : option == "loss" ? 0.99 : 1000))
			{ actor.OutputHandler.Send("Specify a finite value within the bounds shown in help."); return false; }
			switch (option) { case "duration": Duration = TimeSpan.FromSeconds(value); break; case "amount": Amount = value; break; case "loss": Loss = value; break; }
		}
		else return base.BuildingCommand(actor, command.GetUndo());
		Changed = true;
		actor.OutputHandler.Send("The power configuration has been updated.");
		return true;
	}
}
