using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionEngine;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;
using CharacteristicValue = MudSharp.Form.Characteristics.CharacteristicValue;

namespace MudSharp.Character;

public partial class Character
{
	#region IUseTools Members

	public Difficulty GetDifficultyForTool(IGameItem tool, Difficulty baseDifficulty)
	{
		var difficulty = baseDifficulty;
		// Penalty for off-handedness
		if (!tool.DesignedForOffhandUse)
		{
			var loc = Body.HoldOrWieldLocFor(tool);
			if (loc != null && !Merits.OfType<IAmbidextrousMerit>().Any(x => x.Applies(this)) &&
			    ((Handedness.LeftRightOnly() != loc.Alignment.LeftRightOnly() &&
			      Handedness.LeftRightOnly() != Alignment.Irrelevant) ||
			     (Handedness.FrontRearOnly() != loc.Alignment.FrontRearOnly() &&
			      Handedness.FrontRearOnly() != Alignment.Irrelevant)
			    ))
			{
				difficulty = difficulty.StageUp(Gameworld.GetStaticInt("NonDominantHandToolDifficultyPenalty"));
			}
		}

		return difficulty;
	}

	#endregion

	#region IPerceivableHaveTraits Members

	private static Expression _overworkExpression;

	private static Expression OverworkExpression
	{
		get
		{
			if (_overworkExpression is null)
			{
				_overworkExpression =
					new Expression(Futuremud.Games.First().GetStaticConfiguration("OverworkExpression"));
			}

			return _overworkExpression;
		}
	}

	public double GetCurrentBonusLevel()
	{
		// TODO - things that affect current bonus level
		return Body.GetCurrentBonusLevel()
		       + OverworkExpression.EvaluateDoubleWith(("workload", ActiveJobs.Sum(x => x.FullTimeEquivalentRatio)))
			;
	}

	#endregion IPerceivableHaveTraits Members

	#region IHaveMerits Members

	private bool _meritsChanged;

	public bool MeritsChanged
	{
		get => _meritsChanged;
		set
		{
			if (!_meritsChanged && value)
			{
				Changed = true;
			}

			_meritsChanged = value;
		}
	}

	private void SaveMerits(MudSharp.Models.Character character)
	{
		var characterMerits = character.PerceiverMerits;
		var meritsToRemove =
			characterMerits.Where(x => _merits.All(y => y.Id != x.MeritId))
			               .ToList();
		FMDB.Context.PerceiverMerits.RemoveRange(meritsToRemove);
		var meritsToAdd = new List<PerceiverMerit>();
		foreach (var merit in _merits.Where(merit => characterMerits.All(x => x.MeritId != merit.Id)))
		{
			var newMerit = new PerceiverMerit
			{
				Character = character,
				MeritId = merit.Id
			};
			meritsToAdd.Add(newMerit);
		}

		if (meritsToAdd.Any())
		{
			FMDB.Context.PerceiverMerits.AddRange(meritsToAdd);
		}

		FMDB.Context.SaveChanges();
		_meritsChanged = false;
	}

	private readonly List<IMerit> _merits = new();
	public IEnumerable<IMerit> Merits => _merits.Concat(Body?.Merits ?? Enumerable.Empty<IMerit>());

	public bool AddMerit(IMerit merit)
	{
		if (merit == null || Merits.Contains(merit) || merit.MeritScope != MeritScope.Character)
		{
			return false;
		}

		_merits.Add(merit);
		MeritsChanged = true;
		return true;
	}

	public bool RemoveMerit(IMerit merit)
	{
		if (merit == null || !_merits.Contains(merit))
		{
			return false;
		}

		_merits.Remove(merit);
		MeritsChanged = true;
		return true;
	}

	#endregion

	#region IHaveCharacteristics Members

	public IEnumerable<ICharacteristicDefinition> CharacteristicDefinitions => Body.CharacteristicDefinitions;

	public IEnumerable<ICharacteristicValue> RawCharacteristicValues => Body.RawCharacteristicValues;

	public IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> RawCharacteristics =>
		Body.RawCharacteristics;

	public ICharacteristicValue GetCharacteristic(string type, IPerceiver voyeur)
	{
		return Body.GetCharacteristic(type, voyeur);
	}

	public ICharacteristicValue GetCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur)
	{
		return Body.GetCharacteristic(type, voyeur);
	}

	public void SetCharacteristic(ICharacteristicDefinition type, ICharacteristicValue value)
	{
		Body.SetCharacteristic(type, value);
	}

	public string DescribeCharacteristic(ICharacteristicDefinition definition, IPerceiver voyeur,
		CharacteristicDescriptionType type = CharacteristicDescriptionType.Normal)
	{
		return Body.DescribeCharacteristic(definition, voyeur, type);
	}

	public string DescribeCharacteristic(string type, IPerceiver voyeur)
	{
		return Body.DescribeCharacteristic(type, voyeur);
	}

	public IObscureCharacteristics GetObscurer(ICharacteristicDefinition type, IPerceiver voyeur)
	{
		return Body.GetObscurer(type, voyeur);
	}

	public Tuple<ICharacteristicDefinition, CharacteristicDescriptionType> GetCharacteristicDefinition(
		string pattern)
	{
		return Body.GetCharacteristicDefinition(pattern);
	}

	public void ExpireDefinition(ICharacteristicDefinition definition)
	{
		Body.ExpireDefinition(definition);
	}

	public void RecalculateCharacteristicsDueToExternalChange()
	{
		Body.RecalculateCharacteristicsDueToExternalChange();
	}

	#endregion

	#region IHaveTraits Members

	public bool AddTrait(ITraitDefinition trait, double value)
	{
		return Body.AddTrait(trait, value);
	}

	public bool RemoveTrait(ITraitDefinition trait)
	{
		return Body.RemoveTrait(trait);
	}

	public bool SetTraitValue(ITraitDefinition trait, double value)
	{
		return Body.SetTraitValue(trait, value);
	}

	public double TraitValue(ITraitDefinition trait, TraitBonusContext context = TraitBonusContext.None)
	{
		return Body.TraitValue(trait, context);
	}

	public double TraitRawValue(ITraitDefinition trait)
	{
		return Body.TraitRawValue(trait);
	}

	public double TraitMaxValue(ITraitDefinition trait)
	{
		return Body.TraitMaxValue(trait);
	}

	public double TraitMaxValue(ITrait trait)
	{
		return Body.TraitMaxValue(trait);
	}

	public bool HasTrait(ITraitDefinition trait)
	{
		return Body.HasTrait(trait);
	}

	public ITrait GetTrait(ITraitDefinition definition)
	{
		return Body.GetTrait(definition);
	}

	public IEnumerable<ITrait> Traits => Body.Traits;

	public string GetTraitDecorated(ITraitDefinition trait)
	{
		return Body.GetTraitDecorated(trait);
	}

	public IEnumerable<ITrait> TraitsOfType(TraitType type)
	{
		return Body.TraitsOfType(type);
	}

	#endregion IHaveTraits Members

	#region IHaveContextualSizeCategoryMembers

	public SizeCategory SizeStanding => Body.SizeStanding;

	public SizeCategory SizeProne => Body.SizeProne;

	public SizeCategory SizeSitting => Body.SizeSitting;

	public SizeCategory CurrentContextualSize(SizeContext context)
	{
		return Body.CurrentContextualSize(context);
	}

	#endregion
}