using MudSharp.Body;
using MudSharp.GameItems.Prototypes;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;

namespace MudSharp.GameItems.Components;

public class TopicalCreamGameItemComponent : GameItemComponent, IApply, IArmageddonTopicalDoseComponent
{
    protected TopicalCreamGameItemComponentProto _prototype;

    public override IGameItemComponentProto Prototype => _prototype;
    private ArmageddonTopicalDoseBinding? _doseBinding;
    public ArmageddonTopicalDoseBinding? DoseBinding
    {
        get
        {
            var origin = Parent.SpellCreationOrigin;
            return _doseBinding is { IsValid: true } binding && origin is { } source &&
                   source.LifecycleId == binding.LifecycleId && source.SpellId == binding.SpellId &&
                   source.Grade == binding.Grade && source.CreatorId == binding.CreatorId ? binding : null;
        }
    }

    private double _gramsRemaining;

    public double GramsRemaining
    {
        get => _gramsRemaining;
        set
        {
            _gramsRemaining = value;
            if (_gramsRemaining > 0.0)
            {
                Changed = true;
                return;
            }

            Parent.Delete();
        }
    }

    public TopicalCreamGameItemComponent(TopicalCreamGameItemComponentProto proto, IGameItem parent,
        bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
        _doseBinding = null;
        GramsRemaining = proto.TotalGrams;
    }

    public TopicalCreamGameItemComponent(MudSharp.Models.GameItemComponent component,
        TopicalCreamGameItemComponentProto proto, IGameItem parent)
        : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    protected TopicalCreamGameItemComponent(TopicalCreamGameItemComponent rhs, IGameItem newParent,
        bool temporary = false)
        : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _doseBinding = rhs.DoseBinding is { } dose ? dose with { Spent = true } : null;
        _gramsRemaining = rhs.DoseBinding is not null ? 0.0 : rhs.GramsRemaining;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new TopicalCreamGameItemComponent(this, newParent, temporary);
    }

    public void Bind(ArmageddonTopicalDoseBinding binding)
    {
        if (_doseBinding is not null || !binding.IsValid || _prototype.NativeDoseProfile is not { IsValid: true } profile ||
            binding.ReferenceDoseGrams != profile.ReferenceDoseGrams || binding.BudgetPerGrade != profile.BudgetPerGrade)
            throw new InvalidOperationException("Restorative Mud binding must match its native dose profile and exact lifecycle.");
        _doseBinding = binding;
        _gramsRemaining = profile.ReferenceDoseGrams / (Gameworld.UnitManager.BaseWeightToKilograms * 1000.0);
        Changed = true;
    }

    public WhyCannotApply CanApply(IBody target, IBodypart part)
    {
        if (GramsRemaining <= 0.0)
        {
            Parent.Delete();
            return WhyCannotApply.CannotApplyEmpty;
        }

        if (_prototype.NativeDoseProfile is not null &&
            (_prototype.NativeDoseProfile.IsValid != true || DoseBinding is not { IsValid: true, Spent: false } dose || DrugsAreConfigured || _prototype.OnApplyProg is not null ||
             Parent.SpellCreationOrigin is not { } origin || Gameworld.SpellOwnedItems?.HasActiveOrigin(Parent, origin) != true ||
             Math.Abs(_prototype.TotalGrams * Gameworld.UnitManager.BaseWeightToKilograms * 1000.0 - _prototype.NativeDoseProfile.ReferenceDoseGrams) > 0.000001 ||
             !dose.IsExactFullDose(GramsRemaining * Gameworld.UnitManager.BaseWeightToKilograms * 1000.0, dose.ReferenceDoseGrams) || !target.Bodyparts.Contains(part) ||
             !target.Wounds.Any(x => x.Bodypart == part && x.CurrentDamage > 0.0)))
        {
            return WhyCannotApply.CannotApplyEmpty;
        }

        return target.WornItemsProfilesFor(part).Any(x => x.Item2.PreventsRemoval && !x.Item2.NoArmour)
            ? WhyCannotApply.CannotApplyNoAccessToPart
            : WhyCannotApply.CanApply;
    }

    public void Apply(IBody target, IBodypart part, ICharacter applier)
    {
        Apply(target, part, 0.0, applier);
    }

    public void Apply(IBody target, IBodypart part, double amount, ICharacter applier)
    {
        if (!ItemManipulationGuard.CanManipulate(applier, out var reason, Parent))
        {
            applier?.Send(reason);
            return;
        }

        if (applier is not null && (!applier.ColocatedWith(target.Actor) || !target.Bodyparts.Contains(part)) ||
            CanApply(target, part) != WhyCannotApply.CanApply)
        {
            applier?.Send("You can no longer apply that cream to that bodypart.");
            return;
        }

        double actualAmount = amount <= 0.0 || amount > GramsRemaining ? GramsRemaining : amount;
        double actualPhysicalGrams = actualAmount * Gameworld.UnitManager.BaseWeightToKilograms * 1000.0;

        var doseBinding = DoseBinding;
        double healingBudget = 0.0;
        if (_prototype.NativeDoseProfile is { IsValid: true } profile)
        {
			if (doseBinding is not { IsValid: true, Spent: false } || !doseBinding.IsExactFullDose(actualPhysicalGrams, doseBinding.ReferenceDoseGrams) ||
                CanApply(target, part) != WhyCannotApply.CanApply)
            {
                applier?.Send("This restorative dose must be applied in full to a clear bodypart, and can only be used once.");
                return;
            }

			healingBudget = doseBinding.HealingBudgetForConsumedDose(actualPhysicalGrams);
            // Commit consumption before changing wound state so interruption or reload cannot replay healing.
            _gramsRemaining = 0.0;
			doseBinding = doseBinding with { Spent = true };
			_doseBinding = doseBinding;
            Changed = true;
            if (Parent.SpellCreationOrigin is not { } origin ||
                Gameworld.SpellOwnedItems?.TryCommitComponentConsumption(this, origin) != true)
            {
                applier?.Send("This dose's consumption could not be confirmed; no healing was applied.");
                return;
            }
        }

        var originalActor = target.Actor;
        var originalLocation = originalActor.Location;
        var originalLayer = originalActor.RoomLayer;
        MudSharp.Magic.MagicalExposure.Carrier(target, MudSharp.Magic.SubstanceCarrier.Item, Parent.Prototype.Id, actualAmount / _prototype.TotalGrams, DrugVector.Touched);
        foreach (TopicalCreamGameItemComponentProto.CreamDrug drug in _prototype.Drugs)
        {
            target.Dose(drug.Drug, DrugVector.Touched, actualAmount * drug.GramsPerGram * drug.AbsorptionFraction);
        }

        if (healingBudget > 0.0)
        {
            if (ReferenceEquals(originalActor.Body, target) && ReferenceEquals(originalActor.Location, originalLocation) &&
                originalActor.RoomLayer == originalLayer && target.Bodyparts.Contains(part) &&
                (applier is null || applier.ColocatedWith(originalActor)) &&
                !target.WornItemsProfilesFor(part).Any(x => x.Item2.PreventsRemoval && !x.Item2.NoArmour) &&
                target.Wounds.Any(x => x.Bodypart == part && x.CurrentDamage > 0.0))
                HealOrganicWounds(target, part, healingBudget);
            Parent.Delete();
            return;
        }

        _prototype.OnApplyProg?.Execute(target.Actor, part.FullDescription(), actualAmount);
        GramsRemaining -= actualAmount;
    }

    private bool DrugsAreConfigured => _prototype.Drugs.Count > 0;

    private static void HealOrganicWounds(IBody target, IBodypart part, double budget)
    {
        foreach (var wound in target.Wounds.Where(x => x.Bodypart == part && x.CurrentDamage > 0.0)
                     .OrderBy(x => x.Id))
        {
            if (budget <= 0.0)
            {
                break;
            }

            var healing = Math.Min(budget, wound.CurrentDamage);
			wound.CurrentDamage = Math.Max(0.0, wound.CurrentDamage - healing);
			budget -= healing;
		}
	}

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (TopicalCreamGameItemComponentProto)newProto;
    }

    private void LoadFromXml(XElement root)
    {
        // Loading a spent dose happens before native custody is restored. Do not delete during construction.
        _gramsRemaining = double.Parse(root.Attribute("Grams")?.Value ?? "0", System.Globalization.CultureInfo.InvariantCulture);
        if (Guid.TryParse((string?)root.Attribute("LifecycleId"), out var lifecycle) && int.TryParse((string?)root.Attribute("Grade"), out var grade) &&
            long.TryParse((string?)root.Attribute("CreatorId"), out var creator) && long.TryParse((string?)root.Attribute("CreatorInstanceId"), out var creatorInstance) &&
            double.TryParse((string?)root.Attribute("ReferenceGrams"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var grams) &&
            double.TryParse((string?)root.Attribute("BudgetPerGrade"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var budget) &&
            bool.TryParse((string?)root.Attribute("Spent"), out var spent))
        {
            long.TryParse((string?)root.Attribute("SpellId"), out var spellId);
            var binding = new ArmageddonTopicalDoseBinding("arm.item.restorative_mud", lifecycle, spellId, grade, creator, creatorInstance, grams, budget, spent);
            _doseBinding = binding.IsValid ? binding : null;
        }
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition", new XAttribute("Grams", GramsRemaining),
            _doseBinding is null ? null : new XAttribute("LifecycleId", _doseBinding.LifecycleId),
            _doseBinding is null ? null : new XAttribute("SpellId", _doseBinding.SpellId),
            _doseBinding is null ? null : new XAttribute("Grade", _doseBinding.Grade),
            _doseBinding is null ? null : new XAttribute("CreatorId", _doseBinding.CreatorId),
            _doseBinding is null ? null : new XAttribute("CreatorInstanceId", _doseBinding.CreatorInstanceId),
            _doseBinding is null ? null : new XAttribute("ReferenceGrams", _doseBinding.ReferenceDoseGrams),
            _doseBinding is null ? null : new XAttribute("BudgetPerGrade", _doseBinding.BudgetPerGrade),
            _doseBinding is null ? null : new XAttribute("Spent", _doseBinding.Spent)).ToString();
    }
}
