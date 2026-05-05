#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public sealed class SensitivityPower : PsionicSustainedSelfPowerBase
{
	public override string PowerType => "Sensitivity";
	public override string DatabaseType => "sensitivity";
	protected override string DefaultBeginVerb => "sensitivity";
	protected override string DefaultEndVerb => "endsensitivity";
	protected override CheckType ActivationCheckType => CheckType.SensitivityPower;

	public string ScanVerb { get; private set; }
	public MagicPowerDistance ScanDistance { get; private set; }
	public PerceptionTypes GrantedPerceptions { get; private set; }
	public IReadOnlyCollection<PsionicActivityKind> ActivityKinds => _activityKinds;
	private readonly List<PsionicActivityKind> _activityKinds = new();
	public uint ActivityRange { get; private set; }
	public Difficulty ActivityDifficulty { get; private set; }
	public Difficulty CapabilityDifficulty { get; private set; }
	public bool PermitCapabilityRead { get; private set; }
	public bool NotifySelf { get; private set; }
	public string ActivityEcho { get; private set; }

	public override IEnumerable<string> Verbs => [BeginVerb, EndVerb, ScanVerb];

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("sensitivity", (power, gameworld) => new SensitivityPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("sensitivity", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new SensitivityPower(gameworld, school, name, trait));
	}

	private SensitivityPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) :
		base(gameworld, school, name, trait)
	{
		Blurb = "Sense magical and psychic activity";
		_showHelpText =
			$"Use {school.SchoolVerb.ToUpperInvariant()} SENSITIVITY to sustain magical and psychic sensitivity, and {school.SchoolVerb.ToUpperInvariant()} SENSCAN <target> to actively read auras.";
		ScanVerb = "senscan";
		ScanDistance = MagicPowerDistance.SameLocationOnly;
		GrantedPerceptions = PerceptionTypes.SenseMagical | PerceptionTypes.SensePsychic;
		_activityKinds.AddRange([PsionicActivityKind.Magical, PsionicActivityKind.Psychic]);
		ActivityRange = 2;
		ActivityDifficulty = Difficulty.Normal;
		CapabilityDifficulty = Difficulty.ExtremelyHard;
		PermitCapabilityRead = true;
		NotifySelf = false;
		ConcentrationPointsToSustain = 0.0;
		SustainPenalty = 0.0;
		BeginEmote = "Your awareness opens to unseen currents.";
		EndEmote = "Your awareness closes to unseen currents.";
		FailEmote = "You reach for unseen currents, but sense nothing.";
		ActivityEcho = "A ripple of {kind} activity touches your sensitivity: {description}.";
		DoDatabaseInsert();
	}

	private SensitivityPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		ScanVerb = root.Element("ScanVerb")?.Value ?? "senscan";
		ScanDistance = Enum.Parse<MagicPowerDistance>(root.Element("ScanDistance")?.Value ??
		                                               nameof(MagicPowerDistance.SameLocationOnly), true);
		GrantedPerceptions = Enum.Parse<PerceptionTypes>(root.Element("GrantedPerceptions")?.Value ??
		                                                  (PerceptionTypes.SenseMagical | PerceptionTypes.SensePsychic)
		                                                  .ToString(), true);
		_activityKinds.AddRange((root.Element("ActivityKinds")?.Value ?? "Magical,Psychic")
		                         .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		                         .Select(x => Enum.Parse<PsionicActivityKind>(x, true))
		                         .Distinct());
		ActivityRange = uint.Parse(root.Element("ActivityRange")?.Value ?? "2");
		ActivityDifficulty = (Difficulty)int.Parse(root.Element("ActivityDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		CapabilityDifficulty = (Difficulty)int.Parse(root.Element("CapabilityDifficulty")?.Value ?? ((int)Difficulty.ExtremelyHard).ToString());
		PermitCapabilityRead = bool.Parse(root.Element("PermitCapabilityRead")?.Value ?? "true");
		NotifySelf = bool.Parse(root.Element("NotifySelf")?.Value ?? "false");
		ActivityEcho = root.Element("ActivityEcho")?.Value ??
		               "A ripple of {kind} activity touches your sensitivity: {description}.";
	}

	protected override XElement SaveDefinition()
	{
		return SaveSustainedSelfDefinition(
			new XElement("ScanVerb", ScanVerb),
			new XElement("ScanDistance", ScanDistance),
			new XElement("GrantedPerceptions", GrantedPerceptions),
			new XElement("ActivityKinds", _activityKinds.Select(x => x.ToString()).ListToCommaSeparatedValues()),
			new XElement("ActivityRange", ActivityRange),
			new XElement("ActivityDifficulty", (int)ActivityDifficulty),
			new XElement("CapabilityDifficulty", (int)CapabilityDifficulty),
			new XElement("PermitCapabilityRead", PermitCapabilityRead),
			new XElement("NotifySelf", NotifySelf),
			new XElement("ActivityEcho", new XCData(ActivityEcho))
		);
	}

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (verb.EqualTo(ScanVerb))
		{
			UseScan(actor, command);
			return;
		}

		base.UseCommand(actor, verb, command);
	}

	protected override IEffect CreateEffect(ICharacter actor)
	{
		return new PsionicSensitivityEffect(actor, this);
	}

	protected override IEnumerable<IEffect> ActiveEffects(ICharacter actor)
	{
		return actor.EffectsOfType<PsionicSensitivityEffect>().Where(x => x.Power == this);
	}

	public string FormatActivityEcho(ICharacter observer, PsionicActivity activity)
	{
		var source = PsionicTrafficHelper.SourceDescription(activity.Source, observer, School);
		return ActivityEcho
		       .Replace("{kind}", activity.Kind.DescribeEnum().ToLowerInvariant())
		       .Replace("{description}", activity.Description)
		       .Replace("{source}", source)
		       .SubstituteANSIColour()
		       .ProperSentences();
	}

	private void UseScan(ICharacter actor, StringStack command)
	{
		if (!ActiveEffects(actor).Any())
		{
			actor.OutputHandler.Send("You must be sustaining sensitivity before you can actively scan.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Whom do you want to scan?");
			return;
		}

		var targetText = command.PopSpeech();
		var target = targetText.EqualToAny("me", "self")
			? actor
			: AcquireTarget(actor, targetText, ScanDistance);
		if (target is null)
		{
			actor.OutputHandler.Send("You cannot find any eligible mind by that description.");
			return;
		}

		var outcome = Gameworld.GetCheck(CheckType.SensitivityPower)
		                       .Check(actor, SkillCheckDifficulty, SkillCheckTrait, target);
		if (outcome < MinimumSuccessThreshold)
		{
			actor.OutputHandler.Send("You cannot read any clear aura from that target.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Sensitivity scan of {target.HowSeen(actor, true).ColourName()}:");
		var auras = target.Effects.Concat(target.Body.Effects)
		                  .Where(x => x.Applies())
		                  .Where(x => x is IMagicEffect || x is IMagicSpellEffect)
		                  .Select(x => x.Describe(actor))
		                  .Where(x => !string.IsNullOrWhiteSpace(x))
		                  .Distinct()
		                  .ToList();
		if (auras.Any())
		{
			foreach (var aura in auras)
			{
				sb.AppendLine($"\t{aura}");
			}
		}
		else
		{
			sb.AppendLine("\tYou sense no active magical or psychic auras.");
		}

		if (PermitCapabilityRead)
		{
			var capabilityOutcome = Gameworld.GetCheck(CheckType.SensitivityCapabilityRead)
			                                 .Check(actor, CapabilityDifficulty, SkillCheckTrait, target);
			if (capabilityOutcome >= MinimumSuccessThreshold)
			{
				var capabilities = target.Capabilities.ToList();
				if (capabilities.Any())
				{
					sb.AppendLine("Capabilities:");
					foreach (var capability in capabilities)
					{
						sb.AppendLine(
							$"\t{capability.Name.ColourName()} ({capability.School.Name.Colour(capability.School.PowerListColour)}, power level {capability.PowerLevel.ToString("N0", actor).ColourValue()})");
					}
				}
				else
				{
					sb.AppendLine("Capabilities: none sensed.");
				}
			}
			else
			{
				sb.AppendLine("Capabilities: too faint to read.");
			}
		}

		PsionicActivityNotifier.Notify(actor, this, "an active sensitivity scan");
		ConsumePowerCosts(actor, ScanVerb);
		actor.OutputHandler.Send(sb.ToString());
	}

	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Scan Verb: {ScanVerb.ColourCommand()}");
		sb.AppendLine($"Scan Distance: {ScanDistance.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Granted Perceptions: {GrantedPerceptions.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Activity Kinds: {_activityKinds.Select(x => x.DescribeEnum()).ListToString().ColourValue()}");
		sb.AppendLine($"Activity Range: {ActivityRange.ToString("N0", actor).ColourValue()} rooms");
		sb.AppendLine($"Activity Difficulty: {ActivityDifficulty.DescribeColoured()}");
		sb.AppendLine($"Capability Difficulty: {CapabilityDifficulty.DescribeColoured()}");
		sb.AppendLine($"Capability Read: {PermitCapabilityRead.ToColouredString()}");
		sb.AppendLine($"Notify Self: {NotifySelf.ToColouredString()}");
		sb.AppendLine($"Activity Echo: {ActivityEcho.ColourCommand()}");
	}

	protected override string SubtypeHelpText => $@"{base.SubtypeHelpText}
	#3scanverb <verb>#0 - sets the active scan verb
	#3scandistance <distance>#0 - sets active scan target distance
	#3perceptions <flags>#0 - sets granted perception flags
	#3activity <magic|psychic|both>#0 - sets which activity pings are noticed
	#3range <rooms>#0 - sets passive activity range
	#3activitydifficulty <difficulty>#0 - sets passive ping difficulty
	#3capabilitydifficulty <difficulty>#0 - sets capability-read difficulty
	#3capability#0 - toggles capability reads
	#3notifyself#0 - toggles noticing your own activity
	#3activityecho <text>#0 - sets passive ping text, with {{kind}}, {{description}}, {{source}}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "scanverb":
				return BuildingCommandScanVerb(actor, command);
			case "scandistance":
			case "distance":
				return BuildingCommandScanDistance(actor, command);
			case "perception":
			case "perceptions":
				return BuildingCommandPerceptions(actor, command);
			case "activity":
			case "activitytypes":
				return BuildingCommandActivity(actor, command);
			case "range":
				return BuildingCommandRange(actor, command);
			case "activitydifficulty":
			case "activitydiff":
				return BuildingCommandDifficulty(actor, command, true);
			case "capabilitydifficulty":
			case "capabilitydiff":
				return BuildingCommandDifficulty(actor, command, false);
			case "capability":
				PermitCapabilityRead = !PermitCapabilityRead;
				Changed = true;
				actor.OutputHandler.Send($"Sensitivity will {PermitCapabilityRead.NowNoLonger()} attempt capability reads.");
				return true;
			case "notifyself":
				NotifySelf = !NotifySelf;
				Changed = true;
				actor.OutputHandler.Send($"Sensitivity will {NotifySelf.NowNoLonger()} notify the user about their own activity.");
				return true;
			case "activityecho":
				return BuildingCommandActivityEcho(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandScanVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should activate the scan?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();
		var costs = InvocationCosts[ScanVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(ScanVerb);
		ScanVerb = verb;
		Changed = true;
		actor.OutputHandler.Send($"Sensitivity scan now uses {ScanVerb.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandScanDistance(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out MagicPowerDistance value))
		{
			actor.OutputHandler.Send($"Valid distances are {Enum.GetValues<MagicPowerDistance>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		ScanDistance = value;
		Changed = true;
		actor.OutputHandler.Send($"Sensitivity scan can now target {value.LongDescription().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPerceptions(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out PerceptionTypes value))
		{
			actor.OutputHandler.Send("That is not a valid perception flag set.");
			return false;
		}

		GrantedPerceptions = value;
		Changed = true;
		actor.OutputHandler.Send($"Sensitivity now grants {GrantedPerceptions.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandActivity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Choose magic, psychic, or both.");
			return false;
		}

		_activityKinds.Clear();
		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "magic":
			case "magical":
				_activityKinds.Add(PsionicActivityKind.Magical);
				break;
			case "psychic":
			case "psi":
				_activityKinds.Add(PsionicActivityKind.Psychic);
				break;
			case "both":
			case "all":
				_activityKinds.AddRange([PsionicActivityKind.Magical, PsionicActivityKind.Psychic]);
				break;
			default:
				actor.OutputHandler.Send("Choose magic, psychic, or both.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"Sensitivity now notices {_activityKinds.Select(x => x.DescribeEnum()).ListToString().ColourValue()} activity.");
		return true;
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack command)
	{
		if (!uint.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a whole number of rooms.");
			return false;
		}

		ActivityRange = value;
		Changed = true;
		actor.OutputHandler.Send($"Sensitivity now notices activity {ActivityRange.ToString("N0", actor).ColourValue()} rooms away.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command, bool activity)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"Valid difficulties are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		if (activity)
		{
			ActivityDifficulty = value;
		}
		else
		{
			CapabilityDifficulty = value;
		}

		Changed = true;
		actor.OutputHandler.Send($"{(activity ? "Activity" : "Capability")} reads now use {value.DescribeColoured()} difficulty.");
		return true;
	}

	private bool BuildingCommandActivityEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What text should be shown for activity pings?");
			return false;
		}

		ActivityEcho = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The activity echo is now {ActivityEcho.ColourCommand()}.");
		return true;
	}
}
