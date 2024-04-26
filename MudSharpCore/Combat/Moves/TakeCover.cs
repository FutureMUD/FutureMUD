using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class TakeCover : CombatMoveBase
{
	public override string Description => "Seeking cover.";

	public CombatantCover Cover { get; init; }

	private bool _calculatedStamina = false;
	private double _staminaCost = 0.0;

	public override double StaminaCost
	{
		get
		{
			if (!_calculatedStamina)
			{
				_staminaCost = MoveStaminaCost(Assailant);
				_calculatedStamina = true;
			}

			return _staminaCost;
		}
	}

	public static double BaseStaminaCost(IFuturemud gameworld)
	{
		return gameworld.GetStaticDouble("TakeCoverMoveStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	public override double BaseDelay => 0.5;

	public static string WhyZeroCoverFitness(ICharacter assailant, bool seekMovableCover, IRangedCover cover,
		IProvideCover coverItem)
	{
		if (seekMovableCover && !cover.CoverStaysWhileMoving)
		{
			return "That cover cannot be used while moving.";
		}

		if (cover.MaximumSimultaneousCovers > 0)
		{
			if (
				assailant.Location.LayerCharacters(assailant.RoomLayer).Except(assailant)
				         .Count(x => x.Cover?.Cover == cover && x.Cover?.CoverItem == coverItem) >=
				cover.MaximumSimultaneousCovers)
			{
				return "There are already too many people taking cover there.";
			}
		}

		if (cover.HighestPositionState.CompareTo(assailant.PositionState) == PositionHeightComparison.Lower)
		{
			if (!assailant.CanMovePosition(cover.HighestPositionState))
			{
				return
					$"You cannot use that cover because you cannot become {cover.HighestPositionState.Name.ToLowerInvariant()}, because {assailant.WhyCannotMovePosition(cover.HighestPositionState, assailant.PositionModifier, assailant.PositionTarget, false, false).ToLowerInvariant()}";
			}
		}

		throw new NotImplementedException();
	}

	public static double CoverFitness(ICharacter assailant, bool seekMovableCover, IRangedCover cover,
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

		if (cover.HighestPositionState.CompareTo(assailant.PositionState) == PositionHeightComparison.Lower)
		{
			if (!assailant.CanMovePosition(cover.HighestPositionState))
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

	public static CombatantCover GetCoverFor(ICharacter assailant, bool seekMovableCover)
	{
		var locationCovers = assailant.Location.GetCoverFor(assailant).ToList();
		var itemCovers =
			assailant.Location.LayerGameItems(assailant.RoomLayer).SelectNotNull(x => x.GetItemType<IProvideCover>())
			         .Where(x => x.Cover != null && assailant.CanSee(x.Parent))
			         .ToList();

		var fitness = new List<Tuple<IRangedCover, IProvideCover, double>>();
		fitness.AddRange(
			locationCovers.Select(
				x =>
					new Tuple<IRangedCover, IProvideCover, double>(x, null,
						CoverFitness(assailant, seekMovableCover, x, null))));
		fitness.AddRange(
			itemCovers.Select(
				x =>
					new Tuple<IRangedCover, IProvideCover, double>(x.Cover, x,
						CoverFitness(assailant, seekMovableCover, x.Cover, x))));


		var random = fitness.WhereMax(x => x.Item3).GetRandomElement();
		return random == null ? null : new CombatantCover(assailant, random.Item1, random.Item2);
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (!Assailant.CanMove())
		{
			Assailant.Send("You need to be able to move to be able to seek cover.");
			return new CombatMoveResult
			{
				RecoveryDifficulty = Difficulty.Automatic
			};
		}

		if (Cover.CoverItem != null &&
		    (Cover.CoverItem.Parent.Location != Assailant.Location || Cover.CoverItem.Parent.Destroyed))
		{
			Assailant.Send("Your desired cover is no longer there.");
			return new CombatMoveResult
			{
				RecoveryDifficulty = Difficulty.Automatic
			};
		}

		Assailant.Cover?.LeaveCover();
		Assailant.OutputHandler.Handle(
			new EmoteOutput(Cover.Cover.DescribeAction(Assailant, Cover.CoverItem?.Parent),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		Assailant.SetPosition(
			Cover.Cover.HighestPositionState.CompareTo(Assailant.PositionState) == PositionHeightComparison.Lower
				? Assailant.PositionState = Cover.Cover.HighestPositionState
				: Assailant.PositionState, Cover.CoverItem != null ? PositionModifier.Behind : PositionModifier.None,
			Cover.CoverItem?.Parent, null);
		Assailant.Cover = Cover;
		Cover.RegisterEvents();
		return new CombatMoveResult
		{
			RecoveryDifficulty = Difficulty.Normal,
			MoveWasSuccessful = true
		};
	}
}