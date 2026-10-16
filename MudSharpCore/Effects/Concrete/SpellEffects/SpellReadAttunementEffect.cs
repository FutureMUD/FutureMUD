#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.RPG.Merits;

namespace MudSharp.Effects.Concrete.SpellEffects;

public sealed class SpellReadAttunementEffect : SimpleSpellStatusEffectBase, IDescriptionAdditionEffect
{
	public static void InitialiseEffectType() => RegisterFactory("SpellReadAttunement", (root, owner) => new SpellReadAttunementEffect(root, owner));
	public SpellReadAttunementEffect(IPerceivable owner, IMagicSpellEffectParent parent, long casterId, long casterInstanceId)
		: base(owner, parent)
	{
		CasterId = casterId;
		CasterInstanceId = casterInstanceId;
	}

	private SpellReadAttunementEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		CasterId = (long?)root.Element("Effect")?.Element("CasterId") ?? 0;
		CasterInstanceId = (long?)root.Element("Effect")?.Element("CasterInstanceId") ?? 0;
	}

	public long CasterId { get; }
	public long CasterInstanceId { get; }
	public bool PlayerSet => false;
	protected override string SpecificEffectType => "SpellReadAttunement";
	public override string Describe(IPerceiver voyeur) => "Reading assigned elemental affinities and learned magical affiliations";
	protected override XElement SaveDefinition() => SimpleSaveDefinition(new XElement("CasterId", CasterId), new XElement("CasterInstanceId", CasterInstanceId));

	public bool DescriptionAdditionApplies(IPerceiver voyeur) => Owner is ICharacter target && voyeur is ICharacter viewer &&
		CasterId > 0 && CasterInstanceId > 0 && CharacterInstanceIdentityComparer.IdentityId(viewer) == CasterId &&
		viewer.InstanceId == CasterInstanceId && viewer.CanSee(target) && Applies(voyeur);

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		if (!DescriptionAdditionApplies(voyeur) || Owner is not ICharacter target) return string.Empty;
		var affinities = target.DeclaredElementalAffinities();
		var affinityText = affinities.Count == 0 ? "Unassigned" : affinities.Select(x => x.ColourName()).ListToString();
		string affiliationText;
		try
		{
			var affiliations = Gameworld.MagicCasting?.LearnedAffiliations(target);
			affiliationText = affiliations is null ? "Unavailable" : affiliations.Count == 0 ? "None" :
				affiliations.Select(x => $"{x.Name.ColourName()} ({(x.Active ? "active" : "inactive")})").ListToString();
		}
		catch (Exception)
		{
			// A failed durable read must not invent an empty history or expose provider details.
			affiliationText = "Unavailable";
		}

		var text = $"Assigned elemental affinities: {affinityText}.\nLearned magical affiliations: {affiliationText}.";
		return colour ? text : text.StripANSIColour();
	}
}
