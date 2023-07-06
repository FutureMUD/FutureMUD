using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Combat.AuxiliaryEffects;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;

namespace MudSharp.Combat;

internal class AuxiliaryCombatAction : CombatAction, IAuxiliaryCombatAction
{
	static AuxiliaryCombatAction()
	{
		var iType = typeof(IAuxiliaryEffect);
		foreach (
			var type in
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(x => x.GetInterfaces().Contains(iType)))
		{
			var method = type.GetMethod("RegisterTypeHelp", BindingFlags.Static | BindingFlags.Public);
			method?.Invoke(null, new object[] { });
		}
	}

	protected AuxiliaryCombatAction(AuxiliaryCombatAction rhs)
	{
		Gameworld = rhs.Gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.CombatAction();
			dbitem.BaseDelay = rhs.BaseDelay;
			dbitem.ExertionLevel = (int)rhs.ExertionLevel;
			dbitem.UsabilityProgId = rhs.UsabilityProg?.Id;
			dbitem.Intentions = (long)rhs.Intentions;
			dbitem.MoveType = (int)rhs.MoveType;
			dbitem.Name = rhs.Name;
			dbitem.RecoveryDifficultyFailure = (int)rhs.RecoveryDifficultyFailure;
			dbitem.RecoveryDifficultySuccess = (int)rhs.RecoveryDifficultySuccess;
			dbitem.StaminaCost = rhs.StaminaCost;
			dbitem.Weighting = rhs.Weighting;
			dbitem.MoveDifficulty = (int)rhs.MoveDifficulty;
			dbitem.RequiredPositionStateIds =
				rhs._requiredPositionStates.Select(x => x.Id.ToString("F0")).ListToCommaSeparatedValues(" ");
			SaveMoveSpecificData(dbitem);
			FMDB.Context.CombatActions.Add(dbitem);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbitem);
		}
	}

	public AuxiliaryCombatAction(Models.CombatAction dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDatabase(dbitem);
	}

	public AuxiliaryCombatAction(string name, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.CombatAction();
			dbitem.BaseDelay = 1.0;
			dbitem.ExertionLevel = (int)ExertionLevel.Heavy;
			dbitem.UsabilityProgId = gameworld.AlwaysTrueProg.Id;
			dbitem.Intentions = (long)CombatMoveIntentions.Advantage;
			dbitem.MoveType = (int)BuiltInCombatMoveType.AuxiliaryMove;
			dbitem.Name = name;
			dbitem.RecoveryDifficultyFailure = (int)Difficulty.Normal;
			dbitem.RecoveryDifficultySuccess = (int)Difficulty.Easy;
			dbitem.StaminaCost = 1.0;
			dbitem.Weighting = 100;
			dbitem.MoveDifficulty = (int)Difficulty.Normal;
			dbitem.RequiredPositionStateIds =
				$"{PositionStanding.Instance.Id} {PositionFlying.Instance.Id} {PositionFloatingInWater.Instance.Id} {PositionSwimming.Instance.Id}";
			SaveMoveSpecificData(dbitem);
			FMDB.Context.CombatActions.Add(dbitem);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbitem);
		}
	}

	private void LoadFromDatabase(Models.CombatAction dbitem)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		UsabilityProg = Gameworld.FutureProgs.Get(dbitem.UsabilityProgId ?? 0);
		MoveType = (BuiltInCombatMoveType)dbitem.MoveType;
		Intentions = (CombatMoveIntentions)dbitem.Intentions;
		RecoveryDifficultySuccess = (Difficulty)dbitem.RecoveryDifficultySuccess;
		RecoveryDifficultyFailure = (Difficulty)dbitem.RecoveryDifficultyFailure;
		StaminaCost = dbitem.StaminaCost;
		BaseDelay = dbitem.BaseDelay;
		Weighting = dbitem.Weighting;
		MoveDifficulty = (Difficulty)dbitem.MoveDifficulty;
		ExertionLevel = (ExertionLevel)dbitem.ExertionLevel;
		_requiredPositionStates.AddRange(dbitem.RequiredPositionStateIds.Split(' ').Select(x => long.Parse(x))
		                                       .Select(x => PositionState.GetState(x)));

		var root = XElement.Parse(dbitem.AdditionalInfo);
		foreach (var effect in root.Elements("Effect"))
		{
			_auxiliaryEffects.Add(LoadEffect(effect, Gameworld, this));
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.CombatActions.Find(Id);
		dbitem.BaseDelay = BaseDelay;
		dbitem.ExertionLevel = (int)ExertionLevel;
		dbitem.UsabilityProgId = UsabilityProg?.Id;
		dbitem.Intentions = (long)Intentions;
		dbitem.MoveType = (int)MoveType;
		dbitem.Name = Name;
		dbitem.RecoveryDifficultyFailure = (int)RecoveryDifficultyFailure;
		dbitem.RecoveryDifficultySuccess = (int)RecoveryDifficultySuccess;
		dbitem.StaminaCost = StaminaCost;
		dbitem.Weighting = Weighting;
		dbitem.MoveDifficulty = (int)MoveDifficulty;
		dbitem.RequiredPositionStateIds =
			_requiredPositionStates.Select(x => x.Id.ToString("F0")).ListToCommaSeparatedValues(" ");
		SaveMoveSpecificData(dbitem);
		Changed = false;
	}

	protected static IAuxiliaryEffect LoadEffect(XElement definition, IFuturemud gameworld, AuxiliaryCombatAction action)
	{
		switch (definition.Attribute("type").Value)
		{
			case "attackeradvantage":
				return new AttackerAdvantage(definition, gameworld);
			case "defenderadvantage":
				return new DefenderAdvantage(definition, gameworld);
			default:
				throw new NotImplementedException();
		}
	}
#nullable enable
	private static readonly
		DictionaryWithDefault<string, Func<AuxiliaryCombatAction, ICharacter, StringStack, IAuxiliaryEffect?>>
		_builderParsers = new(StringComparer.InvariantCultureIgnoreCase);

	private static readonly List<string> _builderTypeNames = new();

	private static readonly DictionaryWithDefault<string, string> _builderTypeHelps =
		new(StringComparer.InvariantCultureIgnoreCase);

	public static void RegisterBuilderParser(string name,
		Func<AuxiliaryCombatAction, ICharacter, StringStack, IAuxiliaryEffect?> func, string typeHelp, bool isPrimary)
	{
		_builderParsers[name] = func;
		_builderTypeHelps[name] = typeHelp;
		if (isPrimary)
		{
			_builderTypeNames.Add(name);
		}
	}

	protected static IAuxiliaryEffect? CreateEffectFromBuilderInput(AuxiliaryCombatAction action, ICharacter actor, StringStack ss)
	{
		var parser = _builderParsers[ss.PopSpeech().ToLowerInvariant().CollapseString()];
		if (parser is null)
		{
			actor.OutputHandler.Send($"There is no such auxiliary effect type. The valid types are {_builderTypeNames.Select(x => x.ColourValue()).ListToString()}.");
			return null;
		}

		return parser(action, actor, ss);
	}

	protected void SaveMoveSpecificData(Models.CombatAction dbitem)
	{
		var root = new XElement("Root");
		foreach (var effect in _auxiliaryEffects)
		{
			root.Add(effect.Save());
		}
		dbitem.AdditionalInfo = root.ToString();
	}

	public override string FrameworkItemType => "AuxiliaryCombatAction";
		
	public string ShowBuilder(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Combat Action {Id.ToString("N0", actor)} - {Name}");
		sb.AppendLine($"Move Type: {MoveType.Describe().Colour(Telnet.Green)}");
		sb.AppendLine($"Position States: {RequiredPositionStates.Select(x => x.DescribeLocationMovementParticiple.TitleCase().ColourValue()).ListToCommaSeparatedValues(", ")}");
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Base Delay: {BaseDelay.ToString("N2", actor)}s".ColourValue(),
			$"Base Stamina: {StaminaCost.ToString("N3", actor).Colour(Telnet.Green)}",
			$"Weighting: {Weighting.ToString("N2", actor).Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Exertion: {ExertionLevel.Describe().Colour(Telnet.Green)}",
			$"Recover Failure: {RecoveryDifficultyFailure.Describe().Colour(Telnet.Green)}",
			$"Recover Success: {RecoveryDifficultySuccess.Describe().Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Usability Prog: {(UsabilityProg != null ? $"{UsabilityProg.FunctionName}".FluentTagMXP("send", $"href='show futureprog {UsabilityProg.Id}'") : "None".Colour(Telnet.Red))}",
			$"Difficulty: {MoveDifficulty.DescribeColoured()}",
			""
		);
		sb.AppendLine($"Intentions: {Intentions.Describe()}");
		sb.AppendLine();
		sb.AppendLine("Effects:");
		sb.AppendLine();
		var i = 1;
		foreach (var effect in _auxiliaryEffects)
		{
			sb.AppendLine($"{i++.ToString("N0", actor)})\t{effect.DescribeForShow(actor)}");
		}
		sb.AppendLine();
		sb.AppendLine("Combat Message Hierarchy:");
		var messages = Gameworld.CombatMessageManager.CombatMessages.Where(x => x.CouldApply(this))
		                        .OrderByDescending(x => x.Priority).ThenByDescending(x => x.Outcome ?? Outcome.None)
		                        .ThenBy(x => x.Prog != null).ToList();
		i = 1;
		foreach (var message in messages)
		{
			sb.AppendLine(
				$"{i++.ToOrdinal()})\t[#{message.Id.ToString("N0", actor)}] {message.Message.ColourCommand()} [{message.Chance.ToString("P3", actor).Colour(Telnet.Green)}]{(message.Outcome.HasValue ? $" [{message.Outcome.Value.DescribeColour()}]" : "")}{(message.Prog != null ? $" [{message.Prog.FunctionName} (#{message.Prog.Id})]".FluentTagMXP("send", $"href='show futureprog {message.Prog.Id}'") : "")}");
		}

		return sb.ToString();
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "difficulty":
				case "diff":
				return BuildingCommandDifficulty(actor, command);
			case "add":
			case "addeffect":
			case "effectadd":
				return BuildingCommandAddEffect(actor, command);
			case "delete":
			case "del":
			case "deleteeffect":
			case "deleffect":
			case "effectdelete":
			case "effectdel":
				return BuildingCommandDeleteEffect(actor, command);
			case "show":
			case "view":
			case "showeffect":
			case "vieweffect":
				return BuildingCommandShowEffect(actor, command);
			case "edit":
			case "set":
			case "editeffect":
			case "seteffect":
				return BuildingCommandSet(actor, command);
			case "typehelp":
			case "help":
			case "type":
				return BuildingCommandTypeHelp(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What difficulty should the check for this move be? Valid difficulties are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send(
				$"That is not a valid difficulty. Valid difficulties are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		MoveDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"This move will now be {MoveDifficulty.DescribeColoured()} for the attacker.");
		return true;
	}

	private bool BuildingCommandSet(ICharacter actor, StringStack command)
	{
		if (_auxiliaryEffects.Count == 0)
		{
			actor.OutputHandler.Send("There aren't any effects to edit.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify which number effect you want to edit, in the order they appear on the action's SHOW.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1 ||
		    value > _auxiliaryEffects.Count)
		{
			actor.OutputHandler.Send($"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_auxiliaryEffects.Count.ToString("N0", actor).ColourValue()}");
			return false;
		}

		var effect = _auxiliaryEffects.ElementAt(value - 1);
		if (effect.BuildingCommand(actor, command))
		{
			Changed = true;
			return true;
		}

		return false;
	}

	private bool BuildingCommandShowEffect(ICharacter actor, StringStack command)
	{
		if (_auxiliaryEffects.Count == 0)
		{
			actor.OutputHandler.Send("There aren't any effects to show you.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify which number effect you want to be shown, in the order they appear on the action's SHOW.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1 ||
		    value > _auxiliaryEffects.Count)
		{
			actor.OutputHandler.Send($"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_auxiliaryEffects.Count.ToString("N0", actor).ColourValue()}");
			return false;
		}

		var effect = _auxiliaryEffects.ElementAt(value - 1);
		actor.OutputHandler.Send(effect.Show(actor));
		return true;
	}

	private bool BuildingCommandTypeHelp(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which auxiliary effect type did you want to view help for? The valid types are {_builderTypeNames.Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		var type = _builderTypeHelps[command.SafeRemainingArgument.CollapseString()];
		if (type is null)
		{
			actor.OutputHandler.Send($"There is no such auxiliary effect type. The valid types are {_builderTypeNames.Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		actor.OutputHandler.Send(type.SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandDeleteEffect(ICharacter actor, StringStack command)
	{
		if (_auxiliaryEffects.Count == 0)
		{
			actor.OutputHandler.Send("There aren't any effects to delete.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify which number effect you want to delete, in the order they appear on SHOW.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1 ||
		    value > _auxiliaryEffects.Count)
		{
			actor.OutputHandler.Send($"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_auxiliaryEffects.Count.ToString("N0", actor).ColourValue()}");
			return false;
		}

		var effect = _auxiliaryEffects.ElementAt(value - 1);
		actor.OutputHandler.Send($"Are you sure you want to delete the following effect:\n\t{effect.DescribeForShow(actor)}\n\nThis cannot be undone.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "Deleting an auxiliary combat effect",
			AcceptAction = text =>
			{
				_auxiliaryEffects.Remove(effect);
				Changed = true;
				actor.OutputHandler.Send("You delete the auxiliary combat effect.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to delete the auxiliary combat effect.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide not to delete the auxiliary combat effect.");
			},
			Keywords = new List<string> {"effect", "delete", "auxiliary"}
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandAddEffect(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which auxiliary effect type did you want to add? The valid types are {_builderTypeNames.Select(x => x.ColourValue()).ListToString()}.");
			return false;
		}

		var type = CreateEffectFromBuilderInput(this, actor, command);
		if (type is null)
		{
			return false;
		}

		_auxiliaryEffects.Add(type);
		Changed = true;
		actor.OutputHandler.Send($"You add the following auxiliary effect:\n\n\t{type.DescribeForShow(actor)}.");
		return true;
	}

	public IAuxiliaryCombatAction Clone()
	{
		return new AuxiliaryCombatAction(this);
	}

	public string DescribeForCombatMessageShow(ICharacter actor)
	{
		return $"\t{Name.ColourName()} (#{Id.ToString("N0", actor)})";
	}

	public override string ActionTypeName => "auxiliary move";
	public override string HelpText => @"The following options are available for this building command:

	#3name#0 - the name of the attack
	#3delay <number>#0 - the number of seconds delay after using this attack
	#3weight <number>#0 - the relative weighting of the engine selecting this attack
	#3difficulty <difficulty>#0 - the difficulty of the attack roll
	#3exertion <exertion>#0 - the minimum exertion level to set (if not already higher) when the attack is used
	#3recover <difficulty pass> <difficulty fail>#0 - the difficulty of a check made after pass/fail that slightly alters the delay of the attack
	#3stamina <amount>#0 - the base stamina cost of the attack
	#3prog <prog>#0 - a prog taking character, item, character as parameters and returning a boolean, to determine whether this attack can be used
	#3intention <intention1> [<intention2>...<intentionn>]#0 - toggles the specified attack intentions
	#3position <name>#0 - toggles a particular position (standing, swimming, etc) required to use this attack
	
The following options pertain to auxiliary effects:

	#3typehelp#0 - lists the available auxiliary effect types
	#3typehelp <which>#0 - shows the type help for a type
	#3add <type> <...>#0 - adds a type effect. See #3typehelp#0 for detailed syntax.
	#3delete <##>#0 - deletes a particular effect
	#3show <##>#0 - shows detailed information about an effect
	#3edit <##> <...>#0 - changes the properties of an effect";

	private readonly List<IAuxiliaryEffect> _auxiliaryEffects = new();
	public IEnumerable<IAuxiliaryEffect> AuxiliaryEffects => _auxiliaryEffects;

	public Difficulty MoveDifficulty { get; private set; }
}