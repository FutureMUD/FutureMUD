#nullable enable

using System.Globalization;

namespace MudSharp.Effects.Concrete;

/// <summary>One owner and one AI's durable pacing and event receipts, never stored on a shared definition.</summary>
public sealed class MonsterStateEffect : Effect
{
	public long AiId { get; }
	public long ProvokerId { get; private set; }
	public DateTime ProvokedUntil { get; private set; }
	public DateTime CooldownUntil { get; private set; }
	public long WarningTargetId { get; private set; }
	public DateTime WarningUntil { get; private set; }
	public long CorpseId { get; private set; }
	public int BitesRemaining { get; private set; }
	public DateTime FeedUntil { get; private set; }
	public DateTime ReturnRetryUntil { get; private set; }
	public string LastEndReason { get; private set; } = "none";

	public MonsterStateEffect(ICharacter owner, long aiId) : base(owner) => AiId = aiId;
	private MonsterStateEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var xml = root.Element("Effect")!;
		long IdValue(string name) => long.TryParse(xml.Element(name)?.Value, out var id) ? id : 0;
		DateTime DateValue(string name) => DateTime.TryParse(xml.Element(name)?.Value, CultureInfo.InvariantCulture,
			DateTimeStyles.RoundtripKind, out var date) ? date : DateTime.MinValue;
		AiId = IdValue("Ai"); ProvokerId = IdValue("Provoker"); ProvokedUntil = DateValue("ProvokedUntil");
		CooldownUntil = DateValue("CooldownUntil"); WarningTargetId = IdValue("WarningTarget"); WarningUntil = DateValue("WarningUntil");
		CorpseId = IdValue("Corpse"); BitesRemaining = (int)Math.Clamp(IdValue("BitesRemaining"), 0, 100);
		FeedUntil = DateValue("FeedUntil"); LastEndReason = xml.Element("LastEndReason")?.Value ?? "none";
		ReturnRetryUntil = DateValue("ReturnRetryUntil");
	}
	public static void InitialiseEffectType() => RegisterFactory("MonsterState", (xml, owner) => new MonsterStateEffect(xml, owner));
	protected override string SpecificEffectType => "MonsterState";
	public override bool SavingEffect => true;
	public void Provoke(long targetId, TimeSpan duration)
	{
		ProvokerId = targetId; ProvokedUntil = RuntimeClock.UtcNow + duration; Changed = true;
	}
	public void Warn(long targetId, TimeSpan duration)
	{
		WarningTargetId = targetId; WarningUntil = RuntimeClock.UtcNow + duration; Changed = true;
	}
	public void Finish(TimeSpan cooldown, string reason)
	{
		CooldownUntil = RuntimeClock.UtcNow + cooldown; WarningTargetId = 0; WarningUntil = DateTime.MinValue;
		ProvokerId = 0; ProvokedUntil = DateTime.MinValue; LastEndReason = reason; Changed = true;
	}
	public void Feed(long corpseId, int bites, TimeSpan duration)
	{
		CorpseId = corpseId; BitesRemaining = bites; FeedUntil = RuntimeClock.UtcNow + duration; Changed = true;
	}
	public void AteBite() { BitesRemaining = Math.Max(0, BitesRemaining - 1); Changed = true; }
	public void DeferHomeReturn()
	{
		ReturnRetryUntil = RuntimeClock.UtcNow + TimeSpan.FromSeconds(30);
		LastEndReason = "home route unavailable; retrying in 30 seconds";
		Changed = true;
	}
	public override string Describe(IPerceiver voyeur) => $"Monster AI #{AiId.ToString("N0", voyeur)}: {LastEndReason}; cooldown until {CooldownUntil:O}.";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Ai", AiId),
		new XElement("Provoker", ProvokerId), new XElement("ProvokedUntil", ProvokedUntil.ToString("O")),
		new XElement("CooldownUntil", CooldownUntil.ToString("O")), new XElement("WarningTarget", WarningTargetId),
		new XElement("WarningUntil", WarningUntil.ToString("O")), new XElement("Corpse", CorpseId),
		new XElement("BitesRemaining", BitesRemaining), new XElement("FeedUntil", FeedUntil.ToString("O")),
		new XElement("ReturnRetryUntil", ReturnRetryUntil.ToString("O")), new XElement("LastEndReason", LastEndReason));
}
