using MudSharp.Combat;
using MudSharp.Events;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.NPC.AI;

/// <summary>
/// An explicit, opt-in NPC drill AI. It deliberately only operates a nearby artillery
/// platform and does not change any of the generic ranged combat strategies.
/// </summary>
public sealed class ArtilleryCrewAI : ArtificialIntelligenceBase
{
	public ArtilleryCrewAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private ArtilleryCrewAI(IFuturemud gameworld, string name) : base(gameworld, name, "ArtilleryCrew")
	{
		DatabaseInitialise();
	}

	private ArtilleryCrewAI()
	{
	}

	public static void RegisterLoader()
	{
		RegisterAIType("ArtilleryCrew", (ai, gameworld) => new ArtilleryCrewAI(ai, gameworld));
		RegisterAIBuilderInformation("artillerycrew", (gameworld, name) => new ArtilleryCrewAI(gameworld, name), new ArtilleryCrewAI().HelpText);
	}

	protected override string SaveToXml() => "<Definition/>";
	protected override string TypeHelpText => "This opt-in AI claims a nearby artillery role and performs its permitted drill actions against its existing combat target.";

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type != EventType.TenSecondTick || arguments[0] is not ICharacter character || !IsGenerallyAble(character))
		{
			return false;
		}

		var piece = character.Location?.GameItems
			.Select(x => x.GetItemType<IArtilleryPiece>())
			.FirstOrDefault(x => x is not null && (x.IsCrewedBy(character) || x.Crew.Count() < x.CrewRoles.Count()));
		if (piece is null)
		{
			return false;
		}

		if (!piece.IsCrewedBy(character) && !TryClaimRole(character, piece))
		{
			return false;
		}

		if (!piece.IsEmplaced && !piece.IsMounted && piece.CanPerform(character, ArtilleryCrewAction.Command, out _))
		{
			piece.Emplace(character);
			return true;
		}

		var requiredAction = piece.NextRequiredAction;
		if (requiredAction is not null && piece.CanPerform(character, requiredAction.Value, out _) && piece.CanLoad(character))
		{
			piece.Load(character);
			return true;
		}

		if (piece.LoadingStage == ArtilleryLoadingStage.Primed && piece.CanReady(character))
		{
			piece.Ready(character);
			return true;
		}

		if (character.CombatTarget is IPerceiver target && piece.CanFire(character, target))
		{
			piece.Fire(character, target, Outcome.NotTested, Outcome.NotTested,
				new OpposedOutcome(Outcome.NotTested, Outcome.NotTested), null!, null!, null!);
			return true;
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types) => types.Contains(EventType.TenSecondTick);

	private static bool TryClaimRole(ICharacter character, IArtilleryPiece piece)
	{
		foreach (var role in new[] { "captain", "loader", "primer", "sponger", "crew" }
			.Concat(piece.CrewRoles.Where(x => !x.EqualToAny("captain", "loader", "primer", "sponger", "crew"))))
		{
			if (piece.TryJoinCrew(character, role, out _))
			{
				return true;
			}
		}

		return false;
	}
}
