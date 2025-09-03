using System.Xml.Linq;
using System.Linq;
using MudSharp.Body;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Character;

namespace MudSharp.GameItems.Components;

public class TopicalCreamGameItemComponent : GameItemComponent, IApply {
    protected TopicalCreamGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    private double _gramsRemaining;
    public double GramsRemaining {
        get => _gramsRemaining;
        set {
            _gramsRemaining = value;
            if (_gramsRemaining > 0) {
                Changed = true;
            } else {
                Parent.Delete();
            }
        }
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) {
        return new TopicalCreamGameItemComponent(this, newParent, temporary);
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) {
        _prototype = (TopicalCreamGameItemComponentProto)newProto;
    }

    public TopicalCreamGameItemComponent(TopicalCreamGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary) {
        _prototype = proto;
        GramsRemaining = proto.TotalGrams;
    }

    public TopicalCreamGameItemComponent(MudSharp.Models.GameItemComponent component, TopicalCreamGameItemComponentProto proto, IGameItem parent) : base(component, parent) {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    protected TopicalCreamGameItemComponent(TopicalCreamGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary) {
        _prototype = rhs._prototype;
        GramsRemaining = rhs.GramsRemaining;
    }

    private void LoadFromXml(XElement root) {
        GramsRemaining = double.Parse(root.Attribute("Grams")?.Value ?? "0");
    }

    protected override string SaveToXml() {
        return new XElement("Definition", new XAttribute("Grams", GramsRemaining)).ToString();
    }

    public WhyCannotApply CanApply(IBody target, IBodypart part) {
        if (GramsRemaining <= 0.0) {
            Parent.Delete();
            return WhyCannotApply.CannotApplyEmpty;
        }
        return target.WornItemsProfilesFor(part).Any(x => x.Item2.PreventsRemoval && !x.Item2.NoArmour)
            ? WhyCannotApply.CannotApplyNoAccessToPart
            : WhyCannotApply.CanApply;
    }

    public void Apply(IBody target, IBodypart part, ICharacter applier) {
        Apply(target, part, 0.0, applier);
    }

    public void Apply(IBody target, IBodypart part, double amount, ICharacter applier) {
        if (amount <= 0.0 || amount > GramsRemaining) {
            amount = GramsRemaining;
        }
        foreach (var drug in _prototype.Drugs) {
            target.Dose(drug.Drug, DrugVector.Touched, amount * drug.GramsPerGram * drug.AbsorptionFraction);
        }
        GramsRemaining -= amount;
    }
}
