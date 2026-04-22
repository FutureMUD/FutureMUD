using ExpressionEngine;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;
using MudSharp.Communication.Language;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Work.Projects.Impacts;
using System;
using System.Collections.Generic;
using System.Linq;
using CharacteristicValue = MudSharp.Form.Characteristics.CharacteristicValue;

namespace MudSharp.Character;

public partial class Character
{
    #region IUseTools Members

    public Difficulty GetDifficultyForTool(IGameItem tool, Difficulty baseDifficulty)
    {
        Difficulty difficulty = baseDifficulty;
        // Penalty for off-handedness
        if (!tool.DesignedForOffhandUse)
        {
            IBodypart loc = Body.HoldOrWieldLocFor(tool);
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

    public double GetPhysicalBonusLevel()
    {
        return Body.GetPhysicalBonusLevel()
            // TODO - what else?
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
        ICollection<PerceiverMerit> characterMerits = character.PerceiverMerits;
        List<PerceiverMerit> meritsToRemove =
            characterMerits.Where(x => _merits.All(y => y.Id != x.MeritId))
                           .ToList();
        FMDB.Context.PerceiverMerits.RemoveRange(meritsToRemove);
        List<PerceiverMerit> meritsToAdd = new();
        foreach (IMerit merit in _merits.Where(merit => characterMerits.All(x => x.MeritId != merit.Id)))
        {
            PerceiverMerit newMerit = new()
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

    private readonly List<ITrait> _characterTraits = new();

    public bool AddTrait(ITraitDefinition trait, double value)
    {
        if (trait.OwnerScope == TraitOwnerScope.Body)
        {
            return Body.AddTrait(trait, value);
        }

        if (_characterTraits.Any(x => x.Definition == trait))
        {
            return false;
        }

        _characterTraits.Add(trait.NewTrait(this, value));
        Changed = true;
		foreach (ILanguage language in Gameworld.Languages.Where(x => x.LinkedTrait == trait))
		{
			LearnLanguage(language);
			LearnAccent(language.DefaultLearnerAccent, Difficulty.Automatic);
		}

        return true;
    }

    public bool RemoveTrait(ITraitDefinition trait)
    {
        if (trait.OwnerScope == TraitOwnerScope.Body)
        {
            return Body.RemoveTrait(trait);
        }

        if (_characterTraits.All(x => x.Definition != trait))
        {
            return false;
        }

        _characterTraits.RemoveAll(x => x.Definition == trait);
        Changed = true;
        foreach (ILanguage language in Languages.Where(x => x.LinkedTrait == trait).ToList())
        {
            ForgetLanguage(language);
        }

        using (new FMDB())
        {
            Gameworld.SaveManager.Flush();
            Models.CharacterTrait dbtrait = FMDB.Context.CharacterTraits.Find(Id, trait.Id);
            if (dbtrait != null)
            {
                FMDB.Context.CharacterTraits.Remove(dbtrait);
                FMDB.Context.SaveChanges();
            }
        }

        return true;
    }

    public bool SetTraitValue(ITraitDefinition trait, double value)
    {
        if (trait.OwnerScope == TraitOwnerScope.Body)
        {
            return Body.SetTraitValue(trait, value);
        }

        ITrait characterTrait = _characterTraits.FirstOrDefault(x => x.Definition == trait);
        if (characterTrait == null)
        {
            AddTrait(trait, value);
            return true;
        }

        characterTrait.Value = value;
        Changed = true;
        return true;
    }

    public double TraitValue(ITraitDefinition trait, TraitBonusContext context = TraitBonusContext.None)
    {
        if (trait.OwnerScope == TraitOwnerScope.Body)
        {
            return Body.TraitValue(trait, context);
        }

        ITrait characterTrait = _characterTraits.FirstOrDefault(x => x.Definition == trait);
        double baseValue = characterTrait?.Value ?? 0.0;
        baseValue +=
            Merits.OfType<ITraitBonusMerit>().Where(x => x.Applies(this))
                  .Sum(x => x.BonusForTrait(trait, context));
        baseValue +=
            EffectsOfType<ITraitBonusEffect>()
                .Where(x => x.Applies(this))
                .Where(x => x.AppliesToTrait(characterTrait))
                .Sum(x => x.GetBonus(characterTrait));
        baseValue += Body.ExternalItems.SelectNotNull(x => x.GetItemType<IChangeTraitsInInventory>())
                              .Sum(x => x.BonusForTrait(trait, context));
        baseValue += CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(this))
                         .OfType<ILabourImpactTraits>().Sum(x => x.EffectOnTrait(characterTrait, context)) ?? 0.0;
        return baseValue;
    }

    public double TraitRawValue(ITraitDefinition trait)
    {
        return trait.OwnerScope == TraitOwnerScope.Body
            ? Body.TraitRawValue(trait)
            : _characterTraits.FirstOrDefault(x => x.Definition == trait)?.Value ?? 0.0;
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
        return trait.OwnerScope == TraitOwnerScope.Body
            ? Body.HasTrait(trait)
            : _characterTraits.Any(x => x.Definition == trait);
    }

    public ITrait GetTrait(ITraitDefinition definition)
    {
        return definition.OwnerScope == TraitOwnerScope.Body
            ? Body.GetTrait(definition)
            : _characterTraits.FirstOrDefault(x => x.Definition == definition);
    }

    public IEnumerable<ITrait> Traits => Body.Traits;

    public string GetTraitDecorated(ITraitDefinition trait)
    {
        return Body.GetTraitDecorated(trait);
    }

    public IEnumerable<ITrait> TraitsOfType(TraitType type)
    {
        return Traits.Where(x => x.Definition.TraitType == type);
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
