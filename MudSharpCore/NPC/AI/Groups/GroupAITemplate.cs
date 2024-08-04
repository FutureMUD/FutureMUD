using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.NPC.AI.Groups.GroupTypes;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character.Heritage;

namespace MudSharp.NPC.AI.Groups;

public class GroupAITemplate : SaveableItem, IGroupAITemplate
{
	public GroupAITemplate(GroupAiTemplate template, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = template.Id;
		_name = template.Name;
		var root = XElement.Parse(template.Definition);
		LoadFromXml(root);
	}

	protected void LoadFromXml(XElement root)
	{
		_avoidCellProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("AvoidCellProg").Value));
		_avoidCellInvoker = CalculateAvoidCellInvoker(_avoidCellProg);
		_considersThreatProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ConsidersThreatProg").Value));
		_considersThreatInvoker = CalculateConsidersThreatInvoker(_considersThreatProg);
		foreach (var element in root.Element("Emotes").Elements())
		{
			_groupEmotes.Add(new GroupEmote(element));
		}

		GroupAIType = GroupAITypeFactory.LoadFromDatabase(root.Element("GroupType"), Gameworld);
	}

	public GroupAITemplate(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbitem = new GroupAiTemplate();
			FMDB.Context.GroupAiTemplates.Add(dbitem);
			dbitem.Name = name;
			dbitem.Definition = SaveToXml().ToString();
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			_name = name;
			Gameworld.Add(this);
		}
	}

	public GroupAITemplate(GroupAITemplate rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		using (new FMDB())
		{
			var dbitem = new GroupAiTemplate();
			FMDB.Context.GroupAiTemplates.Add(dbitem);
			dbitem.Name = newName;
			dbitem.Definition = rhs.SaveToXml().ToString();
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			_name = newName;
			LoadFromXml(XElement.Parse(dbitem.Definition));
			Gameworld.Add(this);
		}
	}

	public IGroupAIType GroupAIType { get; protected set; }

	public (bool Truth, string Error) IsValidForCreatingGroups
	{
		get
		{
			if (GroupAIType == null)
			{
				return (false, "You must first give the template a GroupType");
			}

			return (true, string.Empty);
		}
	}

	private Func<ICell, GroupAlertness, bool> CalculateAvoidCellInvoker(IFutureProg prog)
	{
		if (prog == null)
		{
			return null;
		}

		if (prog.MatchesParameters(new[] { FutureProgVariableTypes.Location }))
		{
			return (cell, alertness) => _avoidCellProg?.Execute<bool?>(cell) == true;
		}

		if (prog.MatchesParameters(new[] { FutureProgVariableTypes.Location, FutureProgVariableTypes.Text }))
		{
			return (cell, alertness) => _avoidCellProg?.Execute<bool?>(cell, alertness.DescribeEnum()) == true;
		}

		if (prog.MatchesParameters(new[] { FutureProgVariableTypes.Location, FutureProgVariableTypes.Number }))
		{
			return (cell, alertness) => _avoidCellProg?.Execute<bool?>(cell, (int)alertness) == true;
		}

		if (prog.MatchesParameters(new[]
			    { FutureProgVariableTypes.Location, FutureProgVariableTypes.Number, FutureProgVariableTypes.Text }))
		{
			return (cell, alertness) =>
				_avoidCellProg?.Execute<bool?>(cell, (int)alertness, alertness.DescribeEnum()) == true;
		}

		if (prog.MatchesParameters(new[]
			    { FutureProgVariableTypes.Location, FutureProgVariableTypes.Text, FutureProgVariableTypes.Number }))
		{
			return (cell, alertness) =>
				_avoidCellProg?.Execute<bool?>(cell, alertness.DescribeEnum(), (int)alertness) == true;
		}

		return null;
	}

	private IFutureProg _avoidCellProg;
	private Func<ICell, GroupAlertness, bool> _avoidCellInvoker;

	public bool AvoidCell(ICell cell, GroupAlertness alertness)
	{
		if (_avoidCellInvoker == null)
		{
			return false;
		}

		return _avoidCellInvoker(cell, alertness);
	}

	private Func<ICharacter, GroupAlertness, bool> CalculateConsidersThreatInvoker(IFutureProg prog)
	{
		if (prog == null)
		{
			return null;
		}

		if (prog.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
		{
			return (ch, alertness) => _considersThreatProg?.Execute<bool?>(ch) == true;
		}

		if (prog.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Text }))
		{
			return (ch, alertness) => _considersThreatProg?.Execute<bool?>(ch, alertness.DescribeEnum()) == true;
		}

		if (prog.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Number }))
		{
			return (ch, alertness) => _considersThreatProg?.Execute<bool?>(ch, (int)alertness) == true;
		}

		if (prog.MatchesParameters(new[]
			    { FutureProgVariableTypes.Character, FutureProgVariableTypes.Number, FutureProgVariableTypes.Text }))
		{
			return (ch, alertness) =>
				_considersThreatProg?.Execute<bool?>(ch, (int)alertness, alertness.DescribeEnum()) == true;
		}

		if (prog.MatchesParameters(new[]
			    { FutureProgVariableTypes.Character, FutureProgVariableTypes.Text, FutureProgVariableTypes.Number }))
		{
			return (ch, alertness) =>
				_considersThreatProg?.Execute<bool?>(ch, alertness.DescribeEnum(), (int)alertness) == true;
		}

		return null;
	}

	private IFutureProg _considersThreatProg;
	private Func<ICharacter, GroupAlertness, bool> _considersThreatInvoker;

	public bool ConsidersThreat(ICharacter ch, GroupAlertness alertness)
	{
		if (_considersThreatInvoker == null)
		{
			return false;
		}

		return _considersThreatInvoker(ch, alertness);
	}

	private readonly List<IGroupEmote> _groupEmotes = new();
	public IEnumerable<IGroupEmote> GroupEmotes => _groupEmotes;

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "avoid":
				return BuildingCommandAvoid(actor, command);
			case "threat":
				return BuildingCommandThreat(actor, command);
			case "type":
				return BuildingCommandType(actor, command);
			case "emote":
				return BuildingCommandEmote(actor, command);
			default:
				actor.OutputHandler.Send(@"You can use the following options with this command:

	#3name <name>#0 - renames this template
	#3avoid <prog>#0 - sets the avoid prog
	#3threat <prog>#0 - sets the threat prog
	#3avoid none#0 - clears the avoid prog
	#3type <newtype>#0 - changes the type of this ai
	#3emote add <text>#0 - adds a new emote. $0 is emoter, $1 is target
	#3emote remove <#>#0 - removes a particular emote
	#3emote <#> text <new text>#0 - changes an emote
	#3emote <#> age <age>|none#0 - sets or clears an age requirement for an emote
	#3emote <#> role <role>|none#0 - sets of clears a role requirement
	#3emote <#> target <role>|none#0 - sets or clears a target role required
	#3emote <#> gender <gender>|none#0 - sets or clears a required gender
	#3emote <#> minalert <alertness>#0 - sets minimum group alertness
	#3emote <#> maxalert <alertness>#0 - sets maximum group alertness".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandThreat(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a prog for the Considers Threat routine or use 'none' to clear an existing one.");
			return false;
		}

		var cmdText = command.PopSpeech();
		if (cmdText.EqualToAny("none", "clear", "remove"))
		{
			_considersThreatProg = null;
			_considersThreatInvoker = null;
			Changed = true;
			actor.OutputHandler.Send(
				$"You clear the Considers Threat Prog for Group AI Template {Name.Colour(Telnet.Cyan)}.");
			return true;
		}

		var prog = long.TryParse(cmdText, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(cmdText);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send("You must specify a prog that returns a boolean value.");
			return false;
		}

		var func = CalculateConsidersThreatInvoker(prog);
		if (func == null)
		{
			actor.OutputHandler.Send(
				"That prog does not have valid arguments. The first argument must be a character and you can have up to two optional text and number arguments that take text or numeric representations of the group's alertness.");
			return false;
		}

		_considersThreatInvoker = func;
		_considersThreatProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The Group AI Template {Name.Colour(Telnet.Cyan)} will now use the {_considersThreatProg.MXPClickableFunctionName()} prog for its Considers THreat routine.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a new name.");
			return false;
		}

		var newName = command.PopSpeech().TitleCase();
		if (Gameworld.GroupAITemplates.Any(x => x.Name.EqualTo(newName)))
		{
			actor.OutputHandler.Send("There is already a Group AI Template with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename the Group AI Template {_name.Colour(Telnet.Cyan)} to {newName.Colour(Telnet.Cyan)}.");
		_name = newName;
		Changed = true;
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		var cmdText = command.PopSpeech().ToLowerInvariant();
		switch (cmdText)
		{
			case "add":
				return BuildingCommandEmoteAdd(actor, command);
			case "remove":
				return BuildingCommandEmoteRemove(actor, command);
		}

		if (!int.TryParse(cmdText, out var value))
		{
			actor.OutputHandler.Send("You must enter a number corresponding to the emote you want to edit.");
			return false;
		}

		var emote = _groupEmotes.ElementAtOrDefault(value - 1);
		if (emote == null)
		{
			actor.OutputHandler.Send("There is no such emote for you to edit.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Do you want to edit the text, gender, role, target, age, minalertness or maxalertness of that emote?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "text":
				return BuildingCommandEmoteText(actor, command, emote);
			case "gender":
				return BuildingCommandEmoteGender(actor, command, emote);
			case "role":
				return BuildingCommandEmoteRole(actor, command, emote);
			case "target":
				return BuildingCommandEmoteTarget(actor, command, emote);
			case "age":
				return BuildingCommandEmoteAge(actor, command, emote);
			case "minalert":
			case "minalertness":
			case "minimumalertness":
				return BuildingCommandEmoteMinAlertness(actor, command, emote);
			case "maxalert":
			case "maxalertness":
			case "maximumalertness":
				return BuildingCommandEmoteMaxAlertness(actor, command, emote);
			case "action":
				return BuildingCommandAction(actor, command, emote);
			default:
				actor.OutputHandler.Send(
					"Do you want to edit the text, gender, role, target, age, minalertness or maxalertness of that emote?");
				return false;
		}
	}

	private bool BuildingCommandAction(ICharacter actor, StringStack command, IGroupEmote emote)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You can either specify a group action type to require, or use {"clear".ColourCommand()} to remove the existing restriction.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("clear", "reset", "none"))
		{
			emote.RequiredAction = null;
			Changed = true;
			actor.OutputHandler.Send(
				$"You clear the requirement for any particular action for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)}.");
			return true;
		}

		if (!command.PopSpeech().TryParseEnum<GroupAction>(out var action))
		{
			actor.OutputHandler.Send(
				$"That is not a valid group action. Options are {Enum.GetValues(typeof(GroupAction)).OfType<GroupAction>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		emote.RequiredAction = action;
		Changed = true;
		actor.OutputHandler.Send(
			$"You set the required action for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)} to {action.DescribeEnum().ColourName()}.");
		return true;
	}

	private bool BuildingCommandEmoteMaxAlertness(ICharacter actor, StringStack command, IGroupEmote emote)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What should be the maximum alertness at which this emote will trigger? Options are {Enum.GetValues(typeof(GroupAlertness)).OfType<GroupAlertness>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!Utilities.TryParseEnum<GroupAlertness>(command.PopSpeech(), out var alertness))
		{
			actor.OutputHandler.Send(
				$"That is not a valid alertness value. Options are {Enum.GetValues(typeof(GroupAlertness)).OfType<GroupAlertness>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (alertness < emote.MinimumAlertness)
		{
			actor.OutputHandler.Send("You cannot set a maximum alertness that is lower than your minimum alertness.");
			return false;
		}

		emote.MaximumAlertness = alertness;
		actor.OutputHandler.Send(
			$"You change the maximum alertness for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)} to {alertness.DescribeEnum().ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandEmoteMinAlertness(ICharacter actor, StringStack command, IGroupEmote emote)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What should be the minimum alertness at which this emote will trigger? Options are {Enum.GetValues(typeof(GroupAlertness)).OfType<GroupAlertness>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!Utilities.TryParseEnum<GroupAlertness>(command.PopSpeech(), out var alertness))
		{
			actor.OutputHandler.Send(
				$"That is not a valid alertness value. Options are {Enum.GetValues(typeof(GroupAlertness)).OfType<GroupAlertness>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (alertness > emote.MaximumAlertness)
		{
			actor.OutputHandler.Send("You cannot set a minimum alertness that is higher than your maximum alertness.");
			return false;
		}

		emote.MinimumAlertness = alertness;
		actor.OutputHandler.Send(
			$"You change the minimum alertness for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)} to {alertness.DescribeEnum().ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandEmoteAge(ICharacter actor, StringStack command, IGroupEmote emote)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must enter an age category to restrict this emote to, or 'none' to clear.");
			return false;
		}

		var cmdText = command.PopSpeech();
		if (cmdText.EqualToAny("none", "clear", "remove"))
		{
			emote.RequiredAgeCategory = default;
			Changed = true;
			actor.OutputHandler.Send(
				$"You clear any age category requirement for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)}.");
			return true;
		}

		if (!Utilities.TryParseEnum<AgeCategory>(cmdText, out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid age category. Valid values are {Enum.GetValues(typeof(AgeCategory)).OfType<AgeCategory>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		emote.RequiredAgeCategory = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the required age category for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)} to {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEmoteTarget(ICharacter actor, StringStack command, IGroupEmote emote)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must enter a target group role to restrict this emote to, or 'none' to clear.");
			return false;
		}

		var cmdText = command.PopSpeech();
		if (cmdText.EqualToAny("none", "clear", "remove"))
		{
			var test = new Emote(emote.EmoteText, new DummyPerceiver(), new DummyPerceivable());
			if (!test.Valid)
			{
				actor.OutputHandler.Send(
					"The emote has issues - most likely it still references $1 (or equivalent), which is the target you are trying to remove. Fix that first.");
				return false;
			}

			emote.RequiredTargetRole = default;
			Changed = true;
			actor.OutputHandler.Send(
				$"You clear any target group role requirement for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)}.");
			return true;
		}

		if (!Utilities.TryParseEnum<GroupRole>(cmdText, out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid group role. Valid values are {Enum.GetValues(typeof(GroupRole)).OfType<GroupRole>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		emote.RequiredTargetRole = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the required group role for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)} to {value.DescribeEnum().ColourValue()}. Don't forget to update the emote to refer to $1, which is the target.");
		return true;
	}

	private bool BuildingCommandEmoteRole(ICharacter actor, StringStack command, IGroupEmote emote)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must enter a group role to restrict this emote to, or 'none' to clear.");
			return false;
		}

		var cmdText = command.PopSpeech();
		if (cmdText.EqualToAny("none", "clear", "remove"))
		{
			emote.RequiredRole = default;
			Changed = true;
			actor.OutputHandler.Send(
				$"You clear any group role requirement for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)}.");
			return true;
		}

		if (!Utilities.TryParseEnum<GroupRole>(cmdText, out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid group role. Valid values are {Enum.GetValues(typeof(GroupRole)).OfType<GroupRole>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		emote.RequiredRole = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the required group role for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)} to {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEmoteGender(ICharacter actor, StringStack command, IGroupEmote emote)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must enter a gender to restrict this emote to, or 'none' to clear.");
			return false;
		}

		var cmdText = command.PopSpeech();
		if (cmdText.EqualToAny("none", "clear", "remove"))
		{
			emote.RequiredGender = default;
			Changed = true;
			actor.OutputHandler.Send(
				$"You clear any gender requirement for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)}.");
			return true;
		}

		if (!Utilities.TryParseEnum<Gender>(cmdText, out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid gender. Valid values are {Enum.GetValues(typeof(Gender)).OfType<Gender>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		emote.RequiredGender = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the required gender for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)} to {value.DescribeEnum().ColourValue()}.");
		return true;
		;
	}

	private bool BuildingCommandEmoteText(ICharacter actor, StringStack command, IGroupEmote emote)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What emote text do you want to set for this emote? Use $0 to refer to the creature doing the emote, and $1 to refer to a target if you have a target role requirement set.");
			return false;
		}

		var test =
			emote.RequiredTargetRole.HasValue
				? new Emote(command.RemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
					new DummyPerceivable())
				: new Emote(command.RemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!test.Valid)
		{
			actor.OutputHandler.Send(
				"That is not a valid emote. Make sure you are only referring to $0 (and $1 if you have a target role requirement).");
			return false;
		}

		emote.EmoteText = command.RemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the emote text for emote #{(_groupEmotes.IndexOf(emote) + 1).ToString("N0", actor)} to {emote.EmoteText.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandEmoteRemove(ICharacter actor, StringStack command)
	{
		if (!int.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number of the emote you want to remove.");
			return false;
		}

		var emote = _groupEmotes.ElementAtOrDefault(value - 1);
		if (emote == null)
		{
			actor.OutputHandler.Send("There is no such emote.");
			return false;
		}

		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				_groupEmotes.Remove(emote);
				Changed = true;
				actor.OutputHandler.Send($"You delete the emote from Group AI Template {Name.Colour(Telnet.Cyan)}.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to delete the emote from the Group AI Template.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide not to delete the emote from the Group AI Template.");
			},
			Keywords = new List<string> { "ai", "groupai", "emote", "delete" },
			DescriptionString = $"Proposing to delete an emote from a GroupAITemplate."
		}), TimeSpan.FromSeconds(120));
		actor.OutputHandler.Send(
			$"Do you really want to delete emote #{(value + 1).ToString("N0", actor)} from Group AI Template {Name.Colour(Telnet.Cyan)}?\n{Accept.StandardAcceptPhrasing}");
		return true;
	}

	private bool BuildingCommandEmoteAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter some emote text to go along with your new emote.");
			return false;
		}

		var test = new Emote(command.RemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!test.Valid)
		{
			actor.OutputHandler.Send(
				"That is not a valid emote text. Remember that you cannot begin with a $1 until you have set a target role requirement.");
			return false;
		}

		_groupEmotes.Add(new GroupEmote(command.RemainingArgument));
		Changed = true;
		actor.OutputHandler.Send(
			$"You add a new emote at position #{_groupEmotes.Count.ToString("N0", actor)} with text {command.RemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a type. The choices are {GroupAITypeFactory.GetBuilderTypeNames().Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		var typeName = command.PopSpeech().ToLowerInvariant();
		var (type, error) = GroupAITypeFactory.LoadFromBuilderArguments(typeName, command.RemainingArgument, Gameworld);
		if (type == null)
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		GroupAIType = type;
		foreach (var group in Gameworld.GroupAIs.Where(x => x.Template == this))
		{
			group.Data = type.GetInitialData(Gameworld);
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"You change the type of the Group AI Template {Name.Colour(Telnet.Cyan)} to {type.Name.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandAvoid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a prog for the Avoid Cell routine or use 'none' to clear an existing one.");
			return false;
		}

		var cmdText = command.PopSpeech();
		if (cmdText.EqualToAny("none", "clear", "remove"))
		{
			_avoidCellProg = null;
			_avoidCellInvoker = null;
			Changed = true;
			actor.OutputHandler.Send(
				$"You clear the Avoid Cell Prog for Group AI Template {Name.Colour(Telnet.Cyan)}.");
			return true;
		}

		var prog = long.TryParse(cmdText, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(cmdText);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send("You must specify a prog that returns a boolean value.");
			return false;
		}

		var func = CalculateAvoidCellInvoker(prog);
		if (func == null)
		{
			actor.OutputHandler.Send(
				"That prog does not have valid arguments. The first argument must be a location and you can have up to two optional text and number arguments that take text or numeric representations of the group's alertness.");
			return false;
		}

		_avoidCellInvoker = func;
		_avoidCellProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The Group AI Template {Name.Colour(Telnet.Cyan)} will now use the {_avoidCellProg.MXPClickableFunctionName()} prog for its Avoid Cell routine.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Group AI Template #{Id.ToString("N0", actor)} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"AI Type: {GroupAIType?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Avoid Prog: {_avoidCellProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Threat Prog: {_considersThreatProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Random Emotes:");
		for (var i = 0; i < _groupEmotes.Count; i++)
		{
			sb.AppendLine($"\t[{(i + 1).ToString("N0", actor)}] {_groupEmotes[i].DescribeForShow()}");
		}

		return sb.ToString();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.GroupAiTemplates.Find(Id);
		dbitem.Name = _name;
		dbitem.Definition = SaveToXml().ToString();
		Changed = false;
	}

	protected XElement SaveToXml()
	{
		return new XElement("Template",
			new XElement("AvoidCellProg", _avoidCellProg?.Id ?? 0),
			new XElement("ConsidersThreatProg", _considersThreatProg?.Id ?? 0),
			GroupAIType?.SaveToXml() ?? new XElement("GroupType", new XAttribute("typename", "invalid")),
			new XElement("Emotes",
				from emote in _groupEmotes
				select emote.SaveToXml()
			)
		);
	}

	public override string FrameworkItemType => "GroupAITemplate";
}