#nullable enable

using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellSubjectiveDescriptionEffect : MagicSpellEffectBase, IOverrideDescEffect, IPrioritisedOverrideDescEffect,
	IIllusionEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellSubjectiveDescription",
			(effect, owner) => new SpellSubjectiveDescriptionEffect(effect, owner));
	}

	public SpellSubjectiveDescriptionEffect(IPerceivable owner, IMagicSpellEffectParent parent,
		DescriptionType descriptionType, string description, long fixedPerceiverId, IFutureProg? prog = null,
		int priority = 0, string overrideKey = "")
		: this(owner, parent, descriptionType, description,
			fixedPerceiverId == 0L ? IllusionAudienceScope.Everyone : IllusionAudienceScope.Caster,
			fixedPerceiverId, owner.Id, null, prog, null, priority, overrideKey)
	{
	}

	public SpellSubjectiveDescriptionEffect(IPerceivable owner, IMagicSpellEffectParent parent,
		DescriptionType descriptionType, string description, IllusionAudienceScope audienceScope, long casterId,
		long targetId, long? clanId, IFutureProg? prog = null, IFutureProg? viewerProg = null, int priority = 0,
		string overrideKey = "")
		: base(owner, parent, prog)
	{
		DescriptionType = descriptionType;
		DescriptionText = description;
		AudienceScope = audienceScope;
		CasterId = casterId;
		TargetId = targetId;
		ClanId = clanId;
		ViewerProg = viewerProg;
		FixedPerceiverId = audienceScope == IllusionAudienceScope.Caster ? casterId : 0L;
		OverridePriority = priority;
		OverrideKey = overrideKey;
	}

	private SpellSubjectiveDescriptionEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		DescriptionType = (DescriptionType)int.Parse(trueRoot?.Element("DescriptionType")?.Value ?? "0");
		DescriptionText = trueRoot?.Element("Description")?.Value ?? string.Empty;
		FixedPerceiverId = long.Parse(trueRoot?.Element("FixedPerceiver")?.Value ?? "0");
		AudienceScope = ParseAudienceScope(trueRoot?.Element("AudienceScope")?.Value, FixedPerceiverId);
		CasterId = long.Parse(trueRoot?.Element("CasterId")?.Value ?? FixedPerceiverId.ToString());
		TargetId = long.Parse(trueRoot?.Element("TargetId")?.Value ?? owner.Id.ToString());
		ClanId = long.TryParse(trueRoot?.Element("ClanId")?.Value, out var clanId) && clanId > 0L
			? clanId
			: null;
		ViewerProg = Gameworld.FutureProgs.Get(long.Parse(trueRoot?.Element("ViewerProg")?.Value ?? "0"));
		OverridePriority = int.Parse(trueRoot?.Element("Priority")?.Value ?? "0");
		OverrideKey = trueRoot?.Element("OverrideKey")?.Value ?? string.Empty;
	}

	public DescriptionType DescriptionType { get; }
	public string DescriptionText { get; }
	public long FixedPerceiverId { get; }
	public int OverridePriority { get; }
	public string OverrideKey { get; }
	public IllusionAudienceScope AudienceScope { get; }
	public long CasterId { get; }
	public long TargetId { get; }
	public long? ClanId { get; }
	public IFutureProg? ViewerProg { get; }
	public int IllusionPriority => OverridePriority;
	public string IllusionKey => OverrideKey;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("DescriptionType", (int)DescriptionType),
			new XElement("Description", new XCData(DescriptionText)),
			new XElement("FixedPerceiver", FixedPerceiverId),
			new XElement("AudienceScope", AudienceScope.ToString()),
			new XElement("CasterId", CasterId),
			new XElement("TargetId", TargetId),
			new XElement("ClanId", ClanId ?? 0L),
			new XElement("ViewerProg", ViewerProg?.Id ?? 0L),
			new XElement("Priority", OverridePriority),
			new XElement("OverrideKey", new XCData(OverrideKey))
		);
	}

	public bool OverrideApplies(IPerceiver voyeur, DescriptionType type)
	{
		return type == DescriptionType && IllusionApplies(voyeur);
	}

	public bool IllusionApplies(IPerceiver voyeur)
	{
		return IllusionAudiencePolicy.Applies(Owner, voyeur, AudienceScope, CasterId, TargetId, ClanId, ViewerProg) &&
		       Applies(voyeur);
	}

	public string Description(DescriptionType type, bool colour)
	{
		return DescriptionText;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Magically overridden {DescriptionType.DescribeEnum().ToLowerInvariant()} description.";
	}

	protected override string SpecificEffectType => "SpellSubjectiveDescription";

	private static IllusionAudienceScope ParseAudienceScope(string? text, long fixedPerceiverId)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return fixedPerceiverId == 0L ? IllusionAudienceScope.Everyone : IllusionAudienceScope.Caster;
		}

		return Enum.TryParse<IllusionAudienceScope>(text, true, out var value)
			? value
			: fixedPerceiverId == 0L
				? IllusionAudienceScope.Everyone
				: IllusionAudienceScope.Caster;
	}
}
