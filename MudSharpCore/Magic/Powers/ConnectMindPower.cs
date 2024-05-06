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
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("connectmind",
			(power, gameworld) => new ConnectMindPower(power, gameworld));
	}

    /// <inheritdoc />
    protected override XElement SaveDefinition()
    {
        var definition = new XElement("Definition",
            new XElement("ConnectVerb", ConnectVerb),
            new XElement("DisconnectVerb", DisconnectVerb),
            new XElement("PowerDistance", (int)PowerDistance),
            new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
            new XElement("SkillCheckTrait", SkillCheckTrait.Id),
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

		ConnectVerb = element.Value;

		element = root.Element("DisconnectVerb");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no DisconnectVerb in the definition XML for power {Id} ({Name}).");
		}

		DisconnectVerb = element.Value;

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

		if (verb.EqualTo(ConnectVerb))
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
			               .Where(x => (bool?)CanInvokePowerProg?.Execute(actor, x.OriginatorCharacter) != false &&
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

		if ((bool?)CanInvokePowerProg.Execute(actor, target) == false)
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

		ConsumePowerCosts(actor, ConnectVerb);
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
		ConsumePowerCosts(actor, DisconnectVerb);
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

	public override IEnumerable<string> Verbs => new[] { ConnectVerb, DisconnectVerb };

	#endregion

	public string ConnectVerb { get; protected set; }

	public string DisconnectVerb { get; protected set; }

	public MagicPowerDistance PowerDistance { get; protected set; }

	private readonly Dictionary<Outcome, bool> _outcomeEchoDictionary = new();

	public Difficulty SkillCheckDifficulty { get; protected set; }

	public ITraitDefinition SkillCheckTrait { get; protected set; }

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
        sb.AppendLine($"Connect Verb: {ConnectVerb.ColourCommand()}");
        sb.AppendLine($"Disconnect Verb: {DisconnectVerb.ColourCommand()}");
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
        sb.AppendLine($"Emote Target: {SelfEmoteForConnect.ColourCommand()}");
        sb.AppendLine($"Fail Emote: {EmoteForFailConnect.ColourCommand()}");
        sb.AppendLine($"Fail Emote Target: {SelfEmoteForFailConnect.ColourCommand()}");
        sb.AppendLine($"End Emote: {EmoteForDisconnect.ColourCommand()}");
        sb.AppendLine($"End Emote Target: {SelfEmoteForDisconnect.ColourCommand()}");
        sb.AppendLine();
        sb.AppendLine("Outcome Echoes:");
        foreach (var item in _outcomeEchoDictionary.OrderBy(x => x.Key))
        {
            sb.AppendLine($"\t{item.Key.DescribeColour()}: {item.Value.ToColouredString()}");
        }
    }

    public string GetAppropriateConnectEmote(ICharacter connecter, ICharacter connectee)
	{
		if ((bool?)TargetCanSeeIdentityProg.Execute(connecter, connectee) == true)
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

		if ((bool?)TargetCanSeeIdentityProg.Execute(connecter, connectee) == true)
		{
			return string.Format(EmoteForDisconnect, "$0");
		}

		return string.Format(EmoteForDisconnect, UnknownIdentityDescription.ColourCharacter());
	}

	public string GetAppropriateHowSeen(ICharacter connecter, ICharacter connectee)
	{
		if ((bool?)TargetCanSeeIdentityProg.Execute(connecter, connectee) == true)
		{
			return connecter.HowSeen(connectee, flags: PerceiveIgnoreFlags.IgnoreConsciousness);
		}

		return UnknownIdentityDescription.ColourCharacter();
	}
}