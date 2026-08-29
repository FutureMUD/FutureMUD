#nullable enable

using MudSharp.Body;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.AIStorytellers;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language;

public static class SignedCommunicationService
{
	public static void Sign(IBody body, IPerceivable? target, string message, IEmote? emote)
	{
		var language = body.CurrentSignedLanguage;
		if (language is null)
		{
			body.OutputHandler.Send("You must select a signed language before you can sign.");
			return;
		}

		var articulation = language.EvaluateArticulation(body);
		if (!articulation.CanSign)
		{
			body.OutputHandler.Send(articulation.Error.ColourError());
			return;
		}

		var difficulty = Difficulty.Normal.StageUp(articulation.MissingPreferredParts);
		var outcome = body.Gameworld.GetCheck(CheckType.SignedLanguageExpressCheck)
			.Check(body.Actor, difficulty, language.LinkedTrait).Outcome;
		var languageInfo = new SignedLanguageInfo(language, body.CurrentSignedLanguageVariety, message, outcome,
			body.Actor, target);
		var actionText = target is null
			? message.EndsWith('?') ? "sign|signs questioningly" : message.EndsWith('!') ? "sign|signs emphatically" : "sign|signs"
			: message.EndsWith('?') ? "sign|signs questioningly to $0" : message.EndsWith('!') ? "sign|signs emphatically to $0" : "sign|signs to $0";
		body.OutputHandler.Handle(new LanguageOutput(new Emote($"@ {actionText}", body.Actor, target), languageInfo,
			emote, flags: OutputFlags.PurelyVisual));
		HandleEvents(body, target, message, language, body.CurrentSignedLanguageVariety, outcome);
		AIStoryteller.HandleCharacterSignInRoomEvent(body.Actor, target, message, language,
			body.CurrentSignedLanguageVariety);
	}

	public static void HandleEvents(IBody body, IPerceivable? target, string message, ISignedLanguage language,
		ISignedLanguageVariety? variety, Outcome outcome)
	{
		var languageName = language.Name;
		var varietyName = variety?.Name ?? string.Empty;
		var witnesses = body.Location.EventHandlers
			.OfType<IPerceiver>()
			.Where(x => !x.IsSelf(body.Actor) && !x.IsSelf(target) && x.CanSee(body.Actor))
			.OfType<IHandleEvents>()
			.Distinct()
			.ToList();
		if (target is null)
		{
			body.Actor.HandleEvent(EventType.CharacterSigns, body.Actor, languageName, varietyName, message,
				(int)outcome);
			foreach (var witness in witnesses)
			{
				witness.HandleEvent(EventType.CharacterSignsWitness, body.Actor, witness, languageName, varietyName,
					message, (int)outcome);
			}
			return;
		}

		body.Actor.HandleEvent(EventType.CharacterSignsDirect, body.Actor, target, languageName, varietyName, message,
			(int)outcome);
		target.HandleEvent(EventType.CharacterSignsDirectTarget, body.Actor, target, languageName, varietyName, message,
			(int)outcome);
		foreach (var witness in witnesses)
		{
			witness.HandleEvent(EventType.CharacterSignsDirectWitness, body.Actor, target, witness, languageName,
				varietyName, message, (int)outcome);
		}
	}
}
