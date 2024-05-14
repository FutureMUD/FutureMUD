using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;

namespace MudSharp.Magic.Powers;

public abstract class MagicPowerBase : SaveableItem, IMagicPower
{

	protected MagicPowerBase(Models.MagicPower power, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = power.Id;
		_name = power.Name;
		Blurb = power.Blurb;
		School = Gameworld.MagicSchools.Get(power.MagicSchoolId);
		_showHelpText = power.ShowHelp;
		var root = XElement.Parse(power.Definition);
		var element = root.Element("CanInvokePowerProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no CanInvokePowerProg in the definition XML for power {Id} ({Name}).");
		}

		CanInvokePowerProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("WhyCantInvokePowerProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no WhyCantInvokePowerProg in the definition XML for power {Id} ({Name}).");
		}

		WhyCantInvokePowerProg = long.TryParse(element.Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("InvocationCosts");
		if (element != null)
		{
			foreach (var verbElement in element.Element("Verbs")?.Elements("Verb") ?? Enumerable.Empty<XElement>())
			{
				var verb = verbElement.Attribute("verb")?.Value ?? throw new ApplicationException(
					$"There was no verb attribute in the InvocationCosts of a Verb element in the definition XML for power {Id} ({Name}).");
				foreach (var sub in verbElement.Elements())
				{
					var which = sub.Attribute("resource")?.Value;
					if (string.IsNullOrWhiteSpace(which))
					{
						throw new ApplicationException(
							$"There was no resource attribute in the InvocationCosts in the definition XML for power {Id} ({Name}).");
					}

					var resource = long.TryParse(which, out value)
						? gameworld.MagicResources.Get(value)
						: gameworld.MagicResources.GetByName(which);
					if (resource == null)
					{
						throw new ApplicationException(
							$"Could not load the resource referred to by '{which}' in the InvocationCosts in the definition XML for power {Id} ({Name}).");
					}

					if (!double.TryParse(sub.Value, out var dvalue))
					{
						throw new ApplicationException(
							$"Could not convert the amount in the InvocationCosts in the definition XML for power {Id} ({Name}).");
					}

					InvocationCosts.Add(verb, (resource, dvalue));
				}
			}
		}
	}

	protected MagicPowerBase(IFuturemud gameworld, IMagicSchool school, string name)
	{
		Gameworld = gameworld;
		School = school;
		_name = name;
        CanInvokePowerProg = Gameworld.AlwaysTrueProg;
		WhyCantInvokePowerProg = Gameworld.UniversalErrorTextProg;
    }

	protected void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.MagicPower
			{
				Name = Name,
				Blurb = Blurb,
				ShowHelp = _showHelpText,
				MagicSchoolId = School.Id,
				PowerModel = PowerType,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.MagicPowers.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

    public sealed override string FrameworkItemType => "MagicPower";

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		return $"Magic Power {Id} - {Name}";
	}

	public ICharacter AcquireTarget(ICharacter owner, string targetText, MagicPowerDistance distance)
	{
		switch (distance)
		{
			case MagicPowerDistance.AnyConnectedMind:
				return owner.CombinedEffectsOfType<ConnectMindEffect>()
							.Where(x => x.School == School)
							.Select(x => x.TargetCharacter)
							.Where(x => TargetFilterFunction(owner, x))
							.GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.AnyConnectedMindOrConnectedTo:
				return owner.CombinedEffectsOfType<ConnectMindEffect>().Where(x => x.School == School)
							.Select(x => x.TargetCharacter)
							.Concat(owner.CombinedEffectsOfType<MindConnectedToEffect>().Where(x => x.School == School)
										 .Select(x => x.OriginatorCharacter))
							.Where(x => TargetFilterFunction(owner, x))
							.GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SameLocationOnly:
				return owner.Location.Characters
							.Where(x => TargetFilterFunction(owner, x))
							.GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.AdjacentLocationsOnly:
				return owner.Location.Characters
							.Except(owner)
							.Concat(owner.Location.ExitsFor(owner).Select(x => x.Destination)
										 .SelectMany(x => x.Characters))
							.Where(x => TargetFilterFunction(owner, x))
							.GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SameAreaOnly:
				return owner.Location.Areas.Any()
					? owner.Location.Areas
						   .SelectMany(x => x.Cells.SelectMany(y => y.Characters))
						   .Except(owner)
						   .Where(x => TargetFilterFunction(owner, x))
						   .GetFromItemListByKeyword(targetText, owner)
					: owner.Location.Characters
						   .Where(x => TargetFilterFunction(owner, x))
						   .GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SameZoneOnly:
				return owner.Location.Zone.Cells
							.SelectMany(x => x.Characters)
							.Except(owner)
							.Where(x => TargetFilterFunction(owner, x))
							.GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SameShardOnly:
				return owner.Location.Shard.Cells
							.SelectMany(x => x.Characters)
							.Except(owner)
							.Where(x => TargetFilterFunction(owner, x))
							.GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SamePlaneOnly:
				// TODO
				return Gameworld.Shards
								.SelectMany(x => x.Cells.SelectMany(y => y.Characters))
								.Except(owner)
								.Where(x => TargetFilterFunction(owner, x))
								.GetFromItemListByKeyword(targetText, owner);
			case MagicPowerDistance.SeenTargetOnly:
				return owner.SeenTargets
							.OfType<ICharacter>()
							.Concat(owner.Location.Characters)
							.Where(x => TargetFilterFunction(owner, x)).GetFromItemListByKeyword(targetText, owner);
		}

		throw new ApplicationException("Unknown MagicPowerDistance in MagicPowerBase.AcquireTarget");
	}

	protected virtual bool TargetFilterFunction(ICharacter owner, ICharacter target)
	{
		return true;
	}

	public bool TargetIsInRange(ICharacter owner, ICharacter target, MagicPowerDistance distance)
	{
		switch (distance)
		{
			case MagicPowerDistance.AnyConnectedMind:
				return owner.CombinedEffectsOfType<ConnectMindEffect>().Any(x => x.TargetCharacter == target);
			case MagicPowerDistance.AnyConnectedMindOrConnectedTo:
				return owner.CombinedEffectsOfType<ConnectMindEffect>().Any(x => x.TargetCharacter == target) ||
					   owner.CombinedEffectsOfType<MindConnectedToEffect>().Any(x => x.OriginatorCharacter == target)
					;
			case MagicPowerDistance.SameLocationOnly:
				return owner.Location == target.Location;
			case MagicPowerDistance.AdjacentLocationsOnly:
				return owner.Location.CellsInVicinity(1, x => true, x => true)
							.Any(x => x == target.Location);
			case MagicPowerDistance.SameAreaOnly:
				return owner.Location.Areas.Any(x => target.Location.Areas.Contains(x)) ||
					   owner.Location == target.Location;
			case MagicPowerDistance.SameZoneOnly:
				return owner.Location.Zone == target.Location.Zone;
			case MagicPowerDistance.SameShardOnly:
				return owner.Location.Shard == target.Location.Shard;
			case MagicPowerDistance.SamePlaneOnly:
				return true; // TODO
			case MagicPowerDistance.SeenTargetOnly:
				return owner.SeenTargets.Concat(owner.Location.Characters.Except(owner).Where(x => owner.CanSee(x)))
							.Contains(target);
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	#region Implementation of IMagicPower

	public IMagicSchool School { get; protected set; }

	public abstract string PowerType { get; }

	protected string _showHelpText;

	public virtual string ShowHelp(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine(Name.GetLineWithTitle(voyeur.InnerLineFormatLength, voyeur.Account.UseUnicode, Telnet.BoldMagenta,
			Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"School: {School.Name.Colour(School.PowerListColour)}");
		sb.AppendLine($"Blurb: {Blurb.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine(_showHelpText.SubstituteANSIColour().Wrap(voyeur.InnerLineFormatLength));
		if (InvocationCosts.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Costs to Use:");
			foreach (var verb in InvocationCosts)
			{
				if (verb.Value.Any())
				{
					sb.AppendLine(
						$"\t{verb.Key.Proper().Colour(School.PowerListColour)}: {verb.Value.Select(x => $"{x.Cost.ToString("N2", voyeur)} {x.Resource.ShortName}".ColourValue()).ListToString()}");
				}
				else
				{
					sb.AppendLine($"\t{verb.Key.Proper().Colour(School.PowerListColour)}: {"None".ColourValue()}");
				}
			}
		}

		return sb.ToString();
	}

	public abstract void UseCommand(ICharacter actor, string verb, StringStack command);

	protected bool HandleGeneralUseRestrictions(ICharacter actor)
	{
		if (actor.Movement != null)
		{
			actor.OutputHandler.Send("You cannot use that power while you are moving.");
			return false;
		}

		if (!actor.State.IsAble())
		{
			actor.OutputHandler.Send($"You cannot use that power while you are {actor.State.Describe()}.");
			return false;
		}

		if (actor.IsEngagedInMelee)
		{
			actor.OutputHandler.Send("You cannot use that power while you are in melee combat!");
			return false;
		}

		var (truth, blocks) = actor.IsBlocked("general");
		if (truth)
		{
			actor.OutputHandler.Send($"You cannot use that power because you are {blocks}.");
			return false;
		}

		return true;
	}

	public (bool Truth, IMagicResource missing) CanAffordToInvokePower(ICharacter actor, string verb)
	{
		var costs = InvocationCosts[verb];
		foreach (var (resource, cost) in costs)
		{
			if (actor.MagicResourceAmounts.FirstOrDefault(x => x.Key == resource).Value <= cost)
			{
				return (false, resource);
			}
		}

		return (true, null);
	}

	public void ConsumePowerCosts(ICharacter actor, string verb)
	{
		foreach (var (resource, cost) in InvocationCosts[verb])
		{
			actor.UseResource(resource, cost);
		}
	}

	public string Blurb { get; protected set; }
	public abstract IEnumerable<string> Verbs { get; }
	public IFutureProg CanInvokePowerProg { get; private set; }
	public IFutureProg WhyCantInvokePowerProg { get; private set; }

	public CollectionDictionary<string, (IMagicResource Resource, double Cost)> InvocationCosts { get; } = new(StringComparer.InvariantCultureIgnoreCase);

	protected virtual string SubtypeHelpText { get; }

    public virtual string HelpText => $@"You can use the following options with this magic power:

    #3name <name>#0 - renames the magic power
    #3school <which>#0 - sets the school the power belongs to
    #3blurb <blurb>#0 - sets the blurb for power list
    #3can <prog>#0 - sets a prog that controls if the power can be used
    #3why <prog>#0 - sets a prog that controls an error message if prog can't be used
    #3help#0 - drops you into an editor to write the player help file
    #3cost <verb> <which> <number>#0 - sets the cost of using a particular verb
{SubtypeHelpText}";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "can":
			case "canprog":
			case "caninvokeprog":
				return BuildingCommandCanInvokeProg(actor, command);
			case "why":
			case "whycant":
			case "whycan't":
			case "whycantprog":
				return BuildingCommandWhyCantProg(actor, command);
			case "help":
			case "playerhelp":
				return BuildingCommandHelpText(actor);
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "cost":
				return BuildingCommandCost(actor, command);
			case "school":
				return BuildingCommandSchool(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandSchool(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which magic school do you want to move this power to?");
			return false;
		}

		var school = Gameworld.MagicSchools.GetByIdOrName(command.SafeRemainingArgument);
		if (school is null)
		{
			actor.OutputHandler.Send("There is no such magic school.");
			return false;
		}

		if (Gameworld.MagicPowers.Where(x => x.School == school).Any(x => x.Name.EqualTo(Name)))
		{
			actor.OutputHandler.Send($"There is already a power in the {school.Name.Colour(school.PowerListColour)} named {Name.ColourName()}. Names must be unique per school.");
			return false;
		}

		School = school;
		Changed = true;
		actor.OutputHandler.Send($"This magic power now belongs to the {school.Name.Colour(school.PowerListColour)} magic school.");
		return true;
	}

	private bool BuildingCommandCost(ICharacter actor, StringStack command)
	{
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which verb do you want to change the cost for? The available verbs are {Verbs.Select(x => x.ColourName()).ListToString()}.");
            return false;
        }

        var verb = command.PopSpeech();
        if (Verbs.All(x => !x.EqualTo(verb)))
        {
            actor.OutputHandler.Send($"There is no such verb for this power. The available verbs are {Verbs.Select(x => x.ColourName()).ListToString()}.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which magic resource would you like to alter the cost of?");
            return false;
        }

        var resource = Gameworld.MagicResources.GetByIdOrName(command.PopSpeech());
        if (resource is null)
        {
            actor.OutputHandler.Send("There is no such chargen resource.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"How much {resource.Name.ColourValue()} should be expended when using the {verb.ColourCommand()} verb? Use 0 to remove a cost.");
            return false;
        }

        if (!double.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
            return false;
        }

        InvocationCosts[verb].RemoveAll(x => x.Resource == resource);
        Changed = true;
        if (value <= 0.0)
        {
            actor.OutputHandler.Send($"It will no longer cost any {resource.Name.ColourValue()} to use the {verb.ColourCommand()} verb with this power.");
            return true;
        }

        InvocationCosts[verb].Add((resource, value));
        actor.OutputHandler.Send($"It will now cost {value.ToString("N3", actor).ColourValue()} {resource.ShortName.ColourValue()} to use the {verb.ColourCommand()} verb for this power.");
        return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the blurb for the school's power list be?");
			return false;
		}

		Blurb = command.SafeRemainingArgument.ProperSentences();
		Changed = true;
		actor.OutputHandler.Send($"You change this power's blurb to: {Blurb.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandHelpText(ICharacter actor)
	{
		if (!string.IsNullOrEmpty(_showHelpText))
		{
			actor.OutputHandler.Send($"Replacing:\n\n{_showHelpText.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t")}");
		}

		actor.OutputHandler.Send("Enter the player help text in the editor below.");
		actor.EditorMode(PostAction, CancelAction, suppliedArguments: [actor]);
		return true;
	}

	private void CancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to edit the player help text.");
	}

	private void PostAction(string text, IOutputHandler handler, object[] args)
	{
		var actor = (ICharacter)args[0];
		_showHelpText = text.Trim().ProperSentences();
		Changed = true;
		handler.Send($"You set the player help text of this magic power to:\n\n{_showHelpText.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t")}");
	}

	private bool BuildingCommandWhyCantProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog should be used to send an error message about why this power can't be invoked?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Text,
			[
				[FutureProgVariableTypes.Character],
				[FutureProgVariableTypes.Character, FutureProgVariableTypes.Character],
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WhyCantInvokePowerProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This power now uses the {prog.MXPClickableFunctionName()} prog to generate error messages about power invocation.");
		return true;
	}

	private bool BuildingCommandCanInvokeProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog should be used to control whether this power can be invoked?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Boolean,
			[
				[FutureProgVariableTypes.Character],
				[FutureProgVariableTypes.Character, FutureProgVariableTypes.Character],
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CanInvokePowerProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This power now uses the {prog.MXPClickableFunctionName()} prog to determine whether a power can be invoked.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this power?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MagicPowers.Where(x => x.School == School).Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a magic power in the {School.Name.Colour(School.PowerListColour)} magic school with the name {name.ColourName()}. Names must be unique per school.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the {Name.ColourName()} power to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	protected abstract void ShowSubtype(ICharacter actor, StringBuilder sb);

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Magic Power #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, School.PowerListColour, Telnet.BoldWhite));
		sb.AppendLine($"Type: {PowerType.ColourValue()}");
		sb.AppendLine($"School: {School.Name.Colour(School.PowerListColour)}");
		sb.AppendLine($"Blurb: {Blurb.ColourCommand()}");
		sb.AppendLine($"Can Invoke Prog: {CanInvokePowerProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Why Can't Invoke Prog: {WhyCantInvokePowerProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		ShowSubtype(actor, sb);
		sb.AppendLine();
		sb.AppendLine("Costs:");
		sb.AppendLine();
		foreach (var item in InvocationCosts)
		{
			sb.AppendLine($"\t{item.Key.ColourCommand()}: {item.Value.Select(x => $"{x.Cost.ToString("N2", actor)} {x.Resource.ShortName}".ColourValue()).ListToString()}");
		}
		sb.AppendLine();
		sb.AppendLine("Help Text:");
		sb.AppendLine();
		sb.AppendLine(_showHelpText.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t"));
		return sb.ToString();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.MagicPowers.Find(Id);
		dbitem.Name = Name;
		dbitem.MagicSchoolId = School.Id;
		dbitem.ShowHelp = _showHelpText;
		dbitem.Blurb = Blurb;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	protected abstract XElement SaveDefinition();

	#endregion
}