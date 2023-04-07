using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Framework;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat.Strategies;

public abstract class CoverSeekingRangedStrategy : RangeBaseStrategy
{
	protected virtual bool PreferBeingAbleToFireWeaponOverCover => true;
	protected virtual bool RequireMovingCover => false;

	protected virtual bool RequireUprightCover => true;

	private double CoverFitness(ICharacter assailant, bool seekMovableCover, IRangedCover cover,
		IProvideCover coverItem)
	{
		var fitness = 0.0;
		if (seekMovableCover && !cover.CoverStaysWhileMoving)
		{
			return 0.0;
		}

		if (cover.MaximumSimultaneousCovers > 0)
		{
			if (
				assailant.Location.LayerCharacters(assailant.RoomLayer).Except(assailant)
				         .Count(x => x.Cover?.Cover == cover && x.Cover?.CoverItem == coverItem) >=
				cover.MaximumSimultaneousCovers)
			{
				return 0.0;
			}
		}

		if (PreferBeingAbleToFireWeaponOverCover)
		{
			var weapons = GetReadyRangedWeapons(assailant).Concat(GetNotReadyButLoadableWeapons(assailant))
			                                              .ToList();
			if (weapons.Any())
			{
				PositionState minPosition = PositionProne.Instance;
				foreach (var weapon in weapons)
				{
					if (weapon.WeaponType.RangedWeaponType.MinimumFiringPosition().CompareTo(minPosition) ==
					    PositionHeightComparison.Higher)
					{
						minPosition = weapon.WeaponType.RangedWeaponType.MinimumFiringPosition();
					}
				}

				if (cover.HighestPositionState.CompareTo(minPosition) == PositionHeightComparison.Lower)
				{
					return 0.0;
				}
			}
		}

		if (cover.HighestPositionState.CompareTo(assailant.PositionState) == PositionHeightComparison.Lower)
		{
			if (!assailant.CanMovePosition(cover.HighestPositionState))
			{
				return 0.0;
			}

			if (RequireUprightCover && !cover.HighestPositionState.Upright)
			{
				return 0.0;
			}
		}

		switch (cover.CoverType)
		{
			case CoverType.Hard:
				fitness += 10.0;
				break;
		}

		switch (cover.CoverExtent)
		{
			case CoverExtent.Marginal:
				fitness += 1.0;
				break;
			case CoverExtent.Partial:
				fitness += 2.0;
				break;
			case CoverExtent.NearTotal:
				fitness += 5.0;
				break;
			case CoverExtent.Total:
				fitness += 10.0;
				break;
		}

		if (cover.CoverStaysWhileMoving)
		{
			fitness += 1.0;
		}

		return fitness;
	}

	protected CombatantCover ScoutCover(ICharacter ch)
	{
		var locationCovers = ch.Location.GetCoverFor(ch).ToList();
		var itemCovers =
			ch.Location.LayerGameItems(ch.RoomLayer).SelectNotNull(x => x.GetItemType<IProvideCover>())
			  .Where(x => x.Cover != null && ch.CanSee(x.Parent))
			  .ToList();

		var fitness = new List<(IRangedCover Cover, IProvideCover Provider, double Fitness)>();
		fitness.AddRange(
			locationCovers.Select(
				x =>
					(x, default(IProvideCover),
						CoverFitness(ch, RequireMovingCover, x, null))));
		fitness.AddRange(
			itemCovers.Select(
				x =>
					(x.Cover, x,
						CoverFitness(ch, RequireMovingCover, x.Cover, x))));


		var random = fitness.Where(x => x.Fitness > 0.0).WhereMax(x => x.Fitness).GetRandomElement();
		return random.Cover == null ? null : new CombatantCover(ch, random.Cover, random.Provider);
	}

	protected override ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		ICombatMove move;
		if ((move = base.HandleCombatMovement(combatant)) != null)
		{
			return move;
		}

		if (combatant.CombatSettings.MovementManagement == AutomaticMovementSettings.FullyManual)
		{
			return null;
		}

		if (combatant is ICharacter ch)
		{
			if (!ch.IsEngagedInMelee && ch.Cover == null)
			{
				var cover = ScoutCover(ch);
				if (cover != null)
				{
					return ch.CanSpendStamina(TakeCover.MoveStaminaCost(ch))
						? (ICombatMove)new TakeCover { Assailant = ch, Cover = cover }
						: new TooExhaustedMove { Assailant = ch };
				}
			}
		}

		return null;
	}
}