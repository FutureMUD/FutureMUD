#nullable enable

using System.Globalization;
using MudSharp.Character;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete.SpellEffects;

public sealed class SpellPowerTagSuppressionEffect : MagicSpellEffectBase, IMagicPowerTagSuppression, IIndependentlyExpiringSpellEffect
{
	private readonly long _identity;
	private readonly long _instance;
	private readonly long _body;
	public IReadOnlyCollection<string> SuppressedPowerTags { get; }
	public DateTime? ExpiryUtc { get; }
	public bool SeverMatchingMindContacts { get; }
	public SpellPowerTagSuppressionEffect(ICharacter owner, IMagicSpellEffectParent parent, IEnumerable<string> tags, DateTime deadline, bool severContacts)
		: base(owner, parent, null)
	{
		_identity = CharacterInstanceIdentityComparer.IdentityId(owner); _instance = owner.InstanceId; _body = owner.Body.Id;
		SuppressedPowerTags = tags.ToArray(); ExpiryUtc = deadline; SeverMatchingMindContacts = severContacts;
	}
	private SpellPowerTagSuppressionEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!; _identity = (long)root.Element("Identity")!; _instance = (long)root.Element("Instance")!; _body = (long)root.Element("Body")!;
		SuppressedPowerTags = root.Element("Tags")!.Elements("Tag").Select(x => x.Value).ToArray();
		if (SuppressedPowerTags.Count is < 1 or > 64 || SuppressedPowerTags.Any(x => !Magic.Powers.MagicPowerBase.ValidPowerTag(x)) ||
			SuppressedPowerTags.Distinct(StringComparer.Ordinal).Count() != SuppressedPowerTags.Count || _identity <= 0 || _instance <= 0 || _body <= 0)
			throw new FormatException("Unsupported native power-tag suppression binding.");
		ExpiryUtc = DateTime.Parse(root.Element("ExpiryUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime();
		SeverMatchingMindContacts = (bool?)root.Element("SeverMatchingMindContacts") ?? false;
	}
	public static void InitialiseEffectType() => RegisterFactory("SpellPowerTagSuppression", (xml, owner) => new SpellPowerTagSuppressionEffect(xml, owner));
	private bool BoundTo(ICharacter actor) => ReferenceEquals(actor, Owner) && actor.InstanceId == _instance && actor.Body.Id == _body &&
		CharacterInstanceIdentityComparer.IdentityId(actor) == _identity && RuntimeClock.UtcNow < ExpiryUtc && !actor.State.IsDead() &&
		ReferenceEquals(actor.Body.Actor, actor) && actor.Effects.Any(x => ReferenceEquals(x, this));
	public bool IsActiveFor(ICharacter actor) => BoundTo(actor) && ParentEffect is { } parent &&
		actor.Effects.Any(x => ReferenceEquals(x, parent)) && parent.SpellEffects.Any(x => ReferenceEquals(x, this));
	public override void InitialEffect()
	{
		// Native AddEffect invokes this before its spell parent is attached. The child itself
		// must already be attached to the exact physical owner; activation admission waits for both.
		if (!SeverMatchingMindContacts || Owner is not ICharacter actor || !BoundTo(actor)) return;
		var contacts = actor.Effects.OfType<ConnectMindEffect>().Concat(actor.Effects.OfType<MindConnectedToEffect>().Select(x => x.OriginatorEffect))
			.Distinct<ConnectMindEffect>(ReferenceEqualityComparer.Instance).ToArray();
		foreach (var contact in contacts)
		{
			if (!BoundTo(actor)) return;
			if ((ReferenceEquals(contact.CharacterOwner, actor) || ReferenceEquals(contact.TargetCharacter, actor)) &&
				SuppressedPowerTags.Intersect(((IMagicPower)contact.MindPower).PowerTags, StringComparer.Ordinal).Any())
				contact.CharacterOwner.RemoveEffect(contact, true);
		}
	}
	protected override string SpecificEffectType => "SpellPowerTagSuppression";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Identity", _identity), new XElement("Instance", _instance), new XElement("Body", _body),
		new XElement("ExpiryUtc", ExpiryUtc!.Value.ToString("O", CultureInfo.InvariantCulture)), new XElement("SeverMatchingMindContacts", SeverMatchingMindContacts),
		new XElement("Tags", SuppressedPowerTags.Select(x => new XElement("Tag", x))));
	public override string Describe(IPerceiver voyeur) => "Activation of native powers with these explicit tags is suppressed: " + string.Join(", ", SuppressedPowerTags) + ".";
}
