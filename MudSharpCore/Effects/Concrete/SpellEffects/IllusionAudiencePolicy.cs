#nullable enable

using MudSharp.Construction;

namespace MudSharp.Effects.Concrete.SpellEffects;

internal static class IllusionAudiencePolicy
{
	public static bool Applies(IPerceivable owner, IPerceiver voyeur, IllusionAudienceScope scope, long casterId,
		long targetId, long? clanId, IFutureProg? viewerProg)
	{
		if (!ScopeApplies(owner, voyeur, scope, casterId, targetId, clanId))
		{
			return false;
		}

		return viewerProg is null || ViewerProgApplies(viewerProg, owner, voyeur, ResolveCaster(owner, casterId));
	}

	private static bool ScopeApplies(IPerceivable owner, IPerceiver voyeur, IllusionAudienceScope scope,
		long casterId, long targetId, long? clanId)
	{
		return scope switch
		{
			IllusionAudienceScope.Caster => voyeur.Id == casterId,
			IllusionAudienceScope.Target => voyeur.Id == targetId,
			IllusionAudienceScope.Everyone => true,
			IllusionAudienceScope.SameCell => SameCell(owner, voyeur),
			IllusionAudienceScope.SameZone => SameZone(owner, voyeur),
			IllusionAudienceScope.Party => PartyApplies(owner, voyeur, casterId, targetId),
			IllusionAudienceScope.Clan => ClanApplies(voyeur, clanId),
			_ => false
		};
	}

	private static bool SameCell(IPerceivable owner, IPerceiver voyeur)
	{
		var ownerLocation = owner is ICell cell ? cell : owner.Location;
		return ownerLocation?.Id == voyeur.Location?.Id;
	}

	private static bool SameZone(IPerceivable owner, IPerceiver voyeur)
	{
		var ownerLocation = owner is ICell cell ? cell : owner.Location;
		return ownerLocation?.Zone?.Id == voyeur.Location?.Zone?.Id;
	}

	private static bool PartyApplies(IPerceivable owner, IPerceiver voyeur, long casterId, long targetId)
	{
		if (voyeur is not ICharacter viewer || viewer.Party is null)
		{
			return false;
		}

		var targetIds = new[] { casterId, targetId, owner.Id };
		return viewer.Party.CharacterMembers.Any(x => targetIds.Contains(x.Id));
	}

	private static bool ClanApplies(IPerceiver voyeur, long? clanId)
	{
		if (clanId is null || voyeur is not ICharacter viewer)
		{
			return false;
		}

		return viewer.ClanMemberships.Any(x => !x.IsArchivedMembership && x.Clan.Id == clanId.Value);
	}

	private static ICharacter? ResolveCaster(IPerceivable owner, long casterId)
	{
		return casterId > 0L
			? owner.Gameworld.Actors.Get(casterId) ?? owner.Gameworld.Characters.Get(casterId)
			: null;
	}

	private static bool ViewerProgApplies(IFutureProg viewerProg, IPerceivable owner, IPerceiver voyeur,
		ICharacter? caster)
	{
		if (caster is not null &&
		    viewerProg.MatchesParameters(
			    [ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver, ProgVariableTypes.Character]))
		{
			return viewerProg.ExecuteBool(owner, voyeur, caster);
		}

		if (viewerProg.MatchesParameters([ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver]))
		{
			return viewerProg.ExecuteBool(owner, voyeur);
		}

		if (viewerProg.MatchesParameters([ProgVariableTypes.Perceivable]))
		{
			return viewerProg.ExecuteBool(owner);
		}

		return false;
	}
}
