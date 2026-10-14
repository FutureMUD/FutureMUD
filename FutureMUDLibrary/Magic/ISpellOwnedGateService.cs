#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems;

namespace MudSharp.Magic;

/// <summary>Builder-selected native presentation; the stock spell binds the owned Nilaz template.</summary>
public sealed record SpellGateConfiguration(long AutobuilderTemplateId, double SecondsPerGrade,
	long UndeadMeritId, int SuffocationPulseSeconds = 60)
{
	public XElement SaveEffect() => new("Effect", new XAttribute("type", "creategate"),
		new XAttribute("version", 1), new XElement("AutobuilderTemplate", AutobuilderTemplateId),
		new XElement("SecondsPerGrade", SecondsPerGrade),
		new XElement("UndeadMerit", UndeadMeritId),
		new XElement("SuffocationPulseSeconds", SuffocationPulseSeconds));
}

public interface ISpellOwnedGateService
{
	string? AdmissionError(ICharacter caster, IRoom source, IGameItem markedItem,
		IMagicSpell spell, SpellGateConfiguration configuration, int grade);
	(IRoom South, IRoom North) CreatePrepared(ICharacter caster, IRoom source, IGameItem markedItem,
		IMagicSpell spell, SpellGateConfiguration configuration, SpellLifecycleOrigin origin, Func<bool> inputsCurrent);
	bool OwnsRoom(long roomId);
	bool OwnsExit(long exitId);
	bool CanEnter(IRoom room, ICharacter entrant, bool nativeLogin);
	bool RequestRetirement(long roomId, SpellRetirementReason reason);
	int ReconcileRetirements(DateTime nowUtc, int limit = 100);
}
