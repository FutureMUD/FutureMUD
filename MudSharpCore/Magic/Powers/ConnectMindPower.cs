using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public class ConnectMindPower : SustainedMagicPower
{
	public override string PowerType => "Connect Mind";

	public override string DatabaseType => "connectmind";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("connectmind",
			(power, gameworld) => new ConnectMindPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("connectmind", (gameworld, school, name, actor, command) => {
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which skill do you want to use for the skill check?");
				return null;
			}

			var skill = gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
			if (skill is null)
			{
				actor.OutputHandler.Send("There is no such skill or attribute.");
				return null;
			}

			return new ConnectMindPower(gameworld, school, name, skill);
		});
	}

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
		var definition = new XElement("Definition",
			new XElement("ConnectVerb", BeginVerb),
			new XElement("DisconnectVerb", EndVerb),
			new XElement("PowerDistance", (int)PowerDistance),
			new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
			new XElement("SkillCheckTrait", SkillCheckTrait.Id),
			new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
			new XElement("TargetCanSeeIdentityProg", TargetCanSeeIdentityProg.Id),
			new XElement("ExclusiveConnection", ExclusiveConnection),
			new XElement("EmoteForConnect", new XCData(EmoteForConnect)),
			new XElement("SelfEmoteForConnect", new XCData(SelfEmoteForConnect)),
			new XElement("EmoteForDisconnect", new XCData(EmoteForDisconnect)),
			new XElement("SelfEmoteForDisconnect", new XCData(SelfEmoteForDisconnect)),
			new XElement("UnknownIdentityDescription", new XCData(UnknownIdentityDescription)),
			new XElement("EmoteForFailConnect", new XCData(EmoteForFailConnect)),
			new XElement("SelfEmoteForFailConnect", new XCData(SelfEmoteForFailConnect)),
			new XElement("OutcomeEchoes",
				from echo in _outcomeEchoDictionary
				select new XElement("OutcomeEcho",
					new XAttribute("outcome", (int)echo.Key),
					new XAttribute("shouldecho", echo.Value)
				)
			)
		);
		SaveSustainedDefinition(definition);
		return definition;
	}

	protected ConnectMindPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("ConnectVerb");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no ConnectVerb in the definition XML for power {Id} ({Name}).");
		}

		BeginVerb = element.Value;

		element = root.Element("DisconnectVerb");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no DisconnectVerb in the definition XML for power {Id} ({Name}).");
		}

		EndVerb = element.Value;

		element = root.Element("PowerDistance");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no PowerDistance in the definition XML for power {Id} ({Name}).");
		}

		if (!Enum.TryParse<MagicPowerDistance>(element.Value, true, out var dist))
		{
			throw new ApplicationException(
				$"The PowerDistance value specified in power {Id} ({Name}) was not valid. The value was {element.Value}.");
		}

		PowerDistance = dist;

		element = root.Element("SkillCheckDifficulty");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SkillCheckDifficulty in the definition XML for power {Id} ({Name}).");
		}

		SkillCheckDifficulty = (Difficulty)int.Parse(element.Value);

		element = root.Element("SkillCheckTrait");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SkillCheckTrait in the definition XML for power {Id} ({Name}).");
		}

		SkillCheckTrait = Gameworld.Traits.Get(long.Parse(element.Value));

		element = root.Element("MinimumSuccessThreshold");

		MinimumSuccessThreshold = (Outcome)int.Parse(element?.Value ?? ((int)Outcome.MinorFail).ToString());

		element = root.Element("EmoteForConnect");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EmoteForConnect in the definition XML for power {Id} ({Name}).");
		}

		EmoteForConnect = element.Value;

		element = root.Element("SelfEmoteForConnect");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SelfEmoteForConnect in the definition XML for power {Id} ({Name}).");
		}

		SelfEmoteForConnect = element.Value;

		element = root.Element("EmoteForDisconnect");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EmoteForDisconnect in the definition XML for power {Id} ({Name}).");
		}

		EmoteForDisconnect = element.Value;

		element = root.Element("SelfEmoteForDisconnect");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SelfEmoteForDisconnect in the definition XML for power {Id} ({Name}).");
		}

		SelfEmoteForDisconnect = element.Value;

		element = root.Element("UnknownIdentityDescription");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no UnknownIdentityDescription in the definition XML for power {Id} ({Name}).");
		}

		UnknownIdentityDescription = element.Value;

		element = root.Element("TargetCanSeeIdentityProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no TargetCanSeeIdentityProg in the definition XML for power {Id} ({Name}).");
		}

		TargetCanSeeIdentityProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		element = root.Element("ExclusiveConnection");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no ExclusiveConnection in the definition XML for power {Id} ({Name}).");
		}

		ExclusiveConnection = bool.Parse(element.Value);

		element = root.Element("EmoteForFailConnect");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no EmoteForFailConnect in the definition XML for power {Id} ({Name}).");
		}

		EmoteForFailConnect = element.Value;

		element = root.Element("SelfEmoteForFailConnect");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SelfEmoteForFailConnect in the definition XML for power {Id} ({Name}).");
		}

		SelfEmoteForFailConnect = element.Value;

		element = root.Element("OutcomeEchoes");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no OutcomeEchoes in the definition XML for power {Id} ({Name}).");
		}

		foreach (var item in element.Elements("OutcomeEcho"))
		{
			var outcomeAttribute = item.Attribute("outcome");
			if (outcomeAttribute == null)
			{
				throw new ApplicationException(
					$"The OutcomeEcho \"{item}\" had an invalid outcome Attribute in the definition XML for power {Id} ({Name}).");
			}

			var shouldEchoAttribute = item.Attribute("shouldecho");
			if (shouldEchoAttribute == null)
			{
				throw new ApplicationException(
					$"The OutcomeEcho \"{item}\" had an invalid shouldecho Attribute in the definition XML for power {Id} ({Name}).");
			}

			_outcomeEchoDictionary[(Outcome)int.Parse(outcomeAttribute.Value)] =
				bool.Parse(shouldEchoAttribute.Value);
		}

		foreach (var outcome in Enum.GetValues(typeof(Outcome)).OfType<Outcome>())
		{
			if (outcome < Outcome.MajorFail)
			{
				continue;
			}

			if (_outcomeEchoDictionary.ContainsKey(outcome))
			{
				continue;
			}

			_outcomeEchoDictionary[outcome] = true;
		}
	}

	protected ConnectMindPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name)
	{
		Blurb = "Connect to the mind of someone you're familiar with";
		_showHelpText = @$"You can use #3{school.SchoolVerb.ToUpperInvariant()} CHOKE <person>#0 to choke someone in your presence, and #3{school.SchoolVerb.ToUpperInvariant()} STOPCHOKE [<person>]#0 to stop choking them. While choked, they will not be able to breathe."; ;
		BeginVerb = "connect";
		EndVerb = "disconnect";
		PowerDistance = MagicPowerDistance.SameShardOnly;
		SkillCheckTrait = trait;
		SkillCheckDifficulty = Difficulty.VeryEasy;
		MinimumSuccessThreshold = Outcome.Fail;
		ConcentrationPointsToSustain = 1.0;
		TargetCanSeeIdentityProg = Gameworld.AlwaysFalseProg;
		ExclusiveConnection = true;
		UnknownIdentityDescription = "an unknown entity";
		_outcomeEchoDictionary[Outcome.MajorFail] = false;
		_outcomeEchoDictionary[Outcome.Fail] = false;
		_outcomeEchoDictionary[Outcome.MinorFail] = false;
		_outcomeEchoDictionary[Outcome.MinorPass] = false;
		_outcomeEchoDictionary[Outcome.Pass] = false;
		_outcomeEchoDictionary[Outcome.MajorPass] = false;
		EmoteForConnect = "You feel the presence of {0} at the back of your mind.";
		SelfEmoteForConnect = "You reach out and connect to $1's mind.";
		EmoteForFailConnect = "You feel the presence of {0} at the back of your mind, but it does not take hold.";
		SelfEmoteForFailConnect = "You reach out and try to connect to $1's mind, but cannot reach stability.";
		EmoteForDisconnect = "You feel the presence of {0} withdraw from your mind.";
		SelfEmoteForDisconnect = "You lose your connection to $1's mind.";
		DoDatabaseInsert();
	}

	#region Overrides of MagicPowerBase

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, verb);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		if (verb.EqualTo(BeginVerb))
		{
			UseCommandConnect(actor, command);
			return;
		}

		UseCommandDisconnect(actor, command);
	}

	public void UseCommandConnect(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a target.");
			return;
		}

		if (ExclusiveConnection && actor.EffectsOfType<MindConnectedToEffect>().Any(x => x.School == School))
		{
			actor.OutputHandler.Send(
				"Your mind is already connect to that of another. You must close the other connection before you can open a new one with this power.");
			return;
		}

		ICharacter target;
		if (command.Peek().EqualTo("last"))
		{
			var potentialTargets = actor.EffectsOfType<MindConnectedToEffect>().Where(x =>
				actor.EffectsOfType<ConnectMindEffect>().All(y => y.TargetCharacter != x.CharacterOwner)).ToList();
			var eligable = potentialTargets
						   .Where(x => CanInvokePowerProg?.ExecuteBool(actor, x.OriginatorCharacter) != false &&
									   TargetIsInRange(actor, x.OriginatorCharacter, PowerDistance)).ToList();
			if (eligable.Any())
			{
				target = eligable.First().OriginatorCharacter;
			}
			else
			{
				if (potentialTargets.Any())
				{
					var sb = new StringBuilder();
					foreach (var badtarget in potentialTargets)
					{
						sb.AppendLine(string.Format(
							WhyCantInvokePowerProg.Execute(actor, badtarget.OriginatorCharacter)?.ToString() ??
							"You cannot reconnect with {0}.",
							badtarget.OriginatorEffect.MindPower.GetAppropriateHowSeen(
								badtarget.OriginatorCharacter, actor)));
					}

					actor.OutputHandler.Send(sb.ToString());
					return;
				}

				actor.OutputHandler.Send("You don't have any presences in your mind that you can connect back to.");
				return;
			}
		}
		else
		{
			var targetText = command.PopSpeech();
			var keywords = new[] { targetText };
			target = actor.TargetActor(targetText, PerceiveIgnoreFlags.IgnoreDark);
			if (target == null)
			{
				var dub = actor.Dubs.Where(x => x.TargetType.EqualTo("Character"))
							   .GetFromItemListByKeyword(targetText, actor);
				if (dub == null)
				{
					actor.OutputHandler.Send("You don't see anyone here or know anyone by a dub like that.");
					return;
				}

				target = actor.Gameworld.Actors.FirstOrDefault(x => x.Id == dub.TargetId);
				if (target == null)
				{
					actor.OutputHandler.Send(
						$"You cannot locate {dub.LastDescription.ColourCharacter()}'s mind right now.");
					return;
				}
			}
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You cannot connect to your own mind.");
			return;
		}

		if (!TargetIsInRange(actor, target, PowerDistance))
		{
			actor.OutputHandler.Send(
				$"You cannot locate {target.HowSeen(actor, type: Form.Shape.DescriptionType.Possessive, flags: PerceiveIgnoreFlags.IgnoreDark)} mind right now.");
			return;
		}

		if (CanInvokePowerProg.ExecuteBool(actor, target))
		{
			actor.OutputHandler.Send(string.Format(
				WhyCantInvokePowerProg.Execute(actor, target)?.ToString() ??
				"You cannot connect your mind with {0}.", GetAppropriateHowSeen(target, actor)));
			return;
		}

		var difficulty = SkillCheckDifficulty;
		var barrier = target.EffectsOfType<MindBarrierEffect>().FirstOrDefault(x => x.Applies(actor));
		if (barrier is not null)
		{
			difficulty.ApplyBonus(barrier.Bonus);
		}

		var check = Gameworld.GetCheck(CheckType.ConnectMindPower);
		var results = check.MultiDifficultyCheck(actor, difficulty, SkillCheckDifficulty, target, SkillCheckTrait);
		if (results.Item1.IsFail() && results.Item2.IsPass())
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(barrier.MindPower.BlockEmoteTarget, actor, actor, target)));
			target.OutputHandler.Send(new EmoteOutput(new Emote(barrier.MindPower.BlockEmoteSelf, actor, actor, target)));
			return;
		}

		if (barrier.MindPower.FailIfOvercome && results.Item1.IsPass())
		{
			barrier.Shatter(actor);
		}

		var result = results.Item1;
		if (_outcomeEchoDictionary[result.Outcome])
		{
			if (result.IsFail())
			{
				if (!string.IsNullOrWhiteSpace(EmoteForFailConnect))
				{
					target.OutputHandler.Send(new EmoteOutput(new Emote(EmoteForFailConnect, actor, actor, target)));
				}

				actor.OutputHandler.Handle(new EmoteOutput(new Emote(SelfEmoteForFailConnect, actor, actor, target)));
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(EmoteForConnect))
				{
					target.OutputHandler.Send(new EmoteOutput(new Emote(GetAppropriateConnectEmote(actor, target),
						actor, actor, target)));
				}

				actor.OutputHandler.Handle(new EmoteOutput(new Emote(SelfEmoteForConnect, actor, actor, target)));
			}
		}
		else
		{
			if (result.IsFail())
			{
				actor.OutputHandler.Send(new EmoteOutput(new Emote(SelfEmoteForFailConnect, actor, actor, target)));
			}
			else
			{
				actor.OutputHandler.Send(new EmoteOutput(new Emote(SelfEmoteForConnect, actor, actor, target)));
			}
		}

		if (result.IsPass())
		{
			actor.AddEffect(new ConnectMindEffect(actor, target, this), GetDuration(result.SuccessDegrees()));
		}

		ConsumePowerCosts(actor, BeginVerb);
	}

	public void UseCommandDisconnect(ICharacter actor, StringStack command)
	{
		ConnectMindEffect targetEffect = null;
		if (command.IsFinished)
		{
			targetEffect = actor.EffectsOfType<ConnectMindEffect>().FirstOrDefault(x => x.PowerOrigin == this);
			if (targetEffect == null)
			{
				actor.OutputHandler.Send("You aren't currently connected to any other minds via that power.");
				return;
			}
		}
		else
		{
			var targetCharacter = actor.EffectsOfType<ConnectMindEffect>().Where(x => x.PowerOrigin == this)
									   .Select(x => x.TargetCharacter)
									   .GetFromItemListByKeyword(command.PopSpeech(), actor);
			if (targetCharacter == null)
			{
				actor.OutputHandler.Send("You aren't connected to the mind of anyone like that via that power.");
				return;
			}

			targetEffect = actor.EffectsOfType<ConnectMindEffect>()
								.First(x => x.PowerOrigin == this && x.TargetCharacter == targetCharacter);
		}

		actor.RemoveEffect(targetEffect, true);
		ConsumePowerCosts(actor, EndVerb);
	}

	#region Overrides of SustainedMagicPower

	protected override void ExpireSustainedEffect(ICharacter actor)
	{
		var effects = actor.EffectsOfType<ConnectMindEffect>().Where(x => x.PowerOrigin == this).ToList();
		foreach (var effect in effects)
		{
			actor.RemoveEffect(effect, true);
		}
	}

	#endregion

	public override IEnumerable<string> Verbs => new[] { BeginVerb, EndVerb };

	#endregion

	public string BeginVerb { get; protected set; }

	public string EndVerb { get; protected set; }

	public MagicPowerDistance PowerDistance { get; protected set; }

	private readonly Dictionary<Outcome, bool> _outcomeEchoDictionary = new();

	public Difficulty SkillCheckDifficulty { get; protected set; }

	public ITraitDefinition SkillCheckTrait { get; protected set; }
	public Outcome MinimumSuccessThreshold { get; protected set; }

	public string EmoteForConnect { get; protected set; }
	public string SelfEmoteForConnect { get; protected set; }
	public string EmoteForFailConnect { get; protected set; }
	public string SelfEmoteForFailConnect { get; protected set; }

	public string EmoteForDisconnect { get; protected set; }
	public string SelfEmoteForDisconnect { get; protected set; }

	public IFutureProg TargetCanSeeIdentityProg { get; protected set; }
	public string UnknownIdentityDescription { get; protected set; }

	public bool ExclusiveConnection { get; protected set; }

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Connect Verb: {BeginVerb.ColourCommand()}");
		sb.AppendLine($"Disconnect Verb: {EndVerb.ColourCommand()}");
		sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
		sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Power Distance: {PowerDistance.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Unknown Identity Desc: {UnknownIdentityDescription.ColourCharacter()}");
		sb.AppendLine($"Exclusive: {ExclusiveConnection.ToColouredString()}");
		sb.AppendLine($"Target Knows Identity Prog: {TargetCanSeeIdentityProg.MXPClickableFunctionName()}");
		sb.AppendLine();
		sb.AppendLine("Emotes:");
		sb.AppendLine();
		sb.AppendLine($"Emote: {EmoteForConnect.ColourCommand()}");
		sb.AppendLine($"Self Emote: {SelfEmoteForConnect.ColourCommand()}");
		sb.AppendLine($"Fail Emote: {EmoteForFailConnect.ColourCommand()}");
		sb.AppendLine($"Self Fail Emote: {SelfEmoteForFailConnect.ColourCommand()}");
		sb.AppendLine($"End Emote: {EmoteForDisconnect.ColourCommand()}");
		sb.AppendLine($"Self End Emote: {SelfEmoteForDisconnect.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Outcome Echoes:");
		foreach (var item in _outcomeEchoDictionary.OrderBy(x => x.Key))
		{
			sb.AppendLine($"\t{item.Key.DescribeColour()}: {item.Value.ToColouredString()}");
		}
	}

	public string GetAppropriateConnectEmote(ICharacter connecter, ICharacter connectee)
	{
		if (TargetCanSeeIdentityProg.ExecuteBool(connecter, connectee))
		{
			return string.Format(EmoteForConnect, "$0");
		}

		return string.Format(EmoteForConnect, UnknownIdentityDescription.ColourCharacter());
	}

	public string GetAppropriateDisconnectEmote(ICharacter connecter, ICharacter connectee)
	{
		if (string.IsNullOrWhiteSpace(EmoteForDisconnect))
		{
			return null;
		}

		if (TargetCanSeeIdentityProg.ExecuteBool(connecter, connectee))
		{
			return string.Format(EmoteForDisconnect, "$0");
		}

		return string.Format(EmoteForDisconnect, UnknownIdentityDescription.ColourCharacter());
	}

	public string GetAppropriateHowSeen(ICharacter connecter, ICharacter connectee)
	{
		if (TargetCanSeeIdentityProg.ExecuteBool(connecter, connectee))
		{
			return connecter.HowSeen(connectee, flags: PerceiveIgnoreFlags.IgnoreConsciousness);
		}

		return UnknownIdentityDescription.ColourCharacter();
	}

	#region Building Commands
	/// <inheritdoc />
	protected override string SubtypeHelpText => @"	#3begin <verb>#0 - sets the verb to activate this power
	#3end <verb>#0 - sets the verb to end this power
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <difficulty>#0 - sets the difficulty of the skill check
	#3threshold <outcome>#0 - sets the minimum outcome for skill success
	#3distance <distance>#0 - sets the distance that this power can be used at
	#3connect <emote>#0 - sets the emote for connecting. $0 is the power user, $1 is the target
	#3selfconnect <emote>#0 - sets the self emote for connecting. $0 is the power user, $1 is the target
	#3disconnect <emote>#0 - sets the emote for disconnecting. $0 is the power user, $1 is the target
	#3selfdisconnect <emote>#0 - sets the self emote for disconnecting. $0 is the power user, $1 is the target
	#3failconnect <emote>#0 - sets the emote for failed connecting. $0 is the power user, $1 is the target
	#3faiilselfconnect <emote>#0 - sets the self emote for failed connecting. $0 is the power user, $1 is the target
	#3targetprog <prog>#0 - sets the prog that controls if the target can see the sdesc
	#3unknown <desc>#0 - sets the unknown description if the target prog fails
	#3exclusive#0 - toggles whether this connection is exclusive or not

#6Note - the connect, disconnect and failconnect emotes can all be blank and this means there will be no echo.#0";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "beginverb":
			case "begin":
			case "startverb":
			case "start":
				return BuildingCommandBeginVerb(actor, command);
			case "endverb":
			case "end":
			case "cancelverb":
			case "cancel":
				return BuildingCommandEndVerb(actor, command);
			case "skill":
			case "trait":
				return BuildingCommandSkill(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "threshold":
				return BuildingCommandThreshold(actor, command);
			case "distance":
				return BuildingCommandDistance(actor, command);
			case "connect":
			case "connectemote":
				return BuildingCommandConnectEmote(actor, command);
			case "selfconnect":
			case "selfconnectemote":
				return BuildingCommandSelfConnectEmote(actor, command);
			case "disconnect":
			case "disconnectemote":
				return BuildingCommandDisconnectEmote(actor, command);
			case "selfdisconnect":
			case "selfdisconnectemote":
				return BuildingCommandSelfDisconnectEmote(actor, command);
			case "failconnect":
			case "failconnectemote":
				return BuildingCommandFailConnectEmote(actor, command);
			case "selffailconnect":
			case "selffailconnectemote":
				return BuildingCommandSelfFailConnectEmote(actor, command);
			case "targetprog":
				return BuildingCommandTargetProg(actor, command);
			case "unknown":
				return BuildingCommandUnknown(actor, command);
			case "exclusive":
				return BuildingCommandExclusive(actor);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	#region Building Subcommands

	private bool BuildingCommandExclusive(ICharacter actor)
	{
		ExclusiveConnection = !ExclusiveConnection;
		Changed = true;
		actor.OutputHandler.Send($"This connect mind power is {ExclusiveConnection.NowNoLonger()} exclusive (only one at a time).");
		return true;
	}

	private bool BuildingCommandUnknown(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the unknown description when the target identity prog returns false?");
			return false;
		}

		UnknownIdentityDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The unknown description will now be {UnknownIdentityDescription.ColourCharacter()}.");
		return true;
	}

	private bool BuildingCommandTargetProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog should be used to control whether the target sees the short description or the unknown description of the power user?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, 
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.Character]
			]
			).LookupProg();
		if (prog is null)
		{
			return false;
		}

		TargetCanSeeIdentityProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The {prog.MXPClickableFunctionName()} is now used to control whether the target can see the real short description of the power user.");
		return true;
	}

	private bool BuildingCommandConnectEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			EmoteForConnect = string.Empty;
			actor.OutputHandler.Send($"This power will no longer give an echo to the room when used.");
			Changed = true;
			return true;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EmoteForConnect = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The connect emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSelfConnectEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the self connect emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		SelfEmoteForConnect = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The self connect emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandDisconnectEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			EmoteForDisconnect = string.Empty;
			actor.OutputHandler.Send($"This power will no longer give an echo to the room when stopped.");
			Changed = true;
			return true;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EmoteForDisconnect = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The disconnect emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSelfDisconnectEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the self disconnect emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		SelfEmoteForDisconnect = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The self disconnect emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandFailConnectEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			EmoteForFailConnect = string.Empty;
			actor.OutputHandler.Send($"This power will no longer give an echo to the room when failed to be used.");
			Changed = true;
			return true;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		EmoteForFailConnect = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The fail connect emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSelfFailConnectEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the self fail connect emote to?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		SelfEmoteForFailConnect = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The self fail connect emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandDistance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"At what distance should this power be able to be used? The valid options are {Enum.GetValues<MagicPowerDistance>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out MagicPowerDistance value))
		{
			actor.OutputHandler.Send($"That is not a valid distance. The valid options are {Enum.GetValues<MagicPowerDistance>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		PowerDistance = value;
		Changed = true;
		actor.OutputHandler.Send($"This magic power can now be used against {value.LongDescription().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What is the minimum success threshold for this power to work? See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Outcome value))
		{
			actor.OutputHandler.Send($"That is not a valid outcome. See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
			return false;
		}

		MinimumSuccessThreshold = value;
		Changed = true;
		actor.OutputHandler.Send($"The power user will now need to achieve a {value.DescribeColour()} in order to activate this power.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty should the skill check for this power be? See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
			return false;
		}

		SkillCheckDifficulty = value;
		Changed = true;
		actor.OutputHandler.Send($"This power's skill check will now be at a difficulty of {value.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which skill or trait should be used for this power's skill check?");
			return false;
		}

		var skill = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (skill is null)
		{
			actor.OutputHandler.Send("That is not a valid skill or trait.");
			return false;
		}

		SkillCheckTrait = skill;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the {skill.Name.ColourName()} skill for its skill check.");
		return true;
	}

	private bool BuildingCommandEndVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should be used to end this power when active?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();
		if (BeginVerb.EqualTo(verb))
		{
			actor.OutputHandler.Send("The begin and verb cannot be the same.");
			return false;
		}

		var costs = InvocationCosts[EndVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(EndVerb);
		EndVerb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to end the power.");
		return true;
	}

	private bool BuildingCommandBeginVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which verb should be used to activate this power?");
			return false;
		}

		var verb = command.SafeRemainingArgument.ToLowerInvariant();
		if (EndVerb.EqualTo(verb))
		{
			actor.OutputHandler.Send("The begin and verb cannot be the same.");
			return false;
		}

		var costs = InvocationCosts[BeginVerb].ToList();
		InvocationCosts[verb] = costs;
		InvocationCosts.Remove(BeginVerb);
		BeginVerb = verb;
		Changed = true;
		actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to begin the power.");
		return true;
	}
	#endregion Building Subcommands
	#endregion Building Commands
}