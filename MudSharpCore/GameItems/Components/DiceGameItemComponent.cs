using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class DiceGameItemComponent : GameItemComponent, IDice
{
    protected DiceGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (DiceGameItemComponentProto)newProto;
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Full || type == DescriptionType.Evaluate;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        switch (type)
        {
            case DescriptionType.Full:
                return
                    $"{description}\nThis is a die and has the following faces: {_prototype.Faces.Select(x => x.ColourValue()).ListToString()}";
            case DescriptionType.Evaluate:
                if (voyeur is ICharacter ch)
                {
                    ICheck cheatCheck = Gameworld.GetCheck(CheckType.EvaluateDiceFairnessCheck);
                    CheckOutcome result = cheatCheck.Check(ch, Difficulty.Normal);
                    if (result.Outcome == Outcome.MajorPass)
                    {
                        bool fair = _prototype.FaceProbabilities.All(x => x.Value == 1.0);
                        if (fair)
                        {
                            return $"This is a fair dice.";
                        }

                        List<int> towards = _prototype.FaceProbabilities.Where(x => x.Value > 1.0).Select(x => x.Key)
                                                .Distinct().ToList();
                        List<int> against = _prototype.FaceProbabilities.Where(x => x.Value < 1.0).Select(x => x.Key)
                                                .Distinct().ToList();

                        if (towards.Any() && against.Any())
                        {
                            return
                                $"It is biased towards {towards.Select(x => _prototype.Faces.ElementAt(x).ColourValue()).ListToString()} and against {towards.Select(x => _prototype.Faces.ElementAt(x).ColourValue()).ListToString()}.";
                        }

                        if (towards.Any())
                        {
                            return
                                $"It is biased towards {towards.Select(x => _prototype.Faces.ElementAt(x).ColourValue()).ListToString()}.";
                        }

                        return
                            $"It is biased against {towards.Select(x => _prototype.Faces.ElementAt(x).ColourValue()).ListToString()}.";
                    }
                }

                return description;
        }

        return base.Decorate(voyeur, name, description, type, colour, flags);
    }

    #region Constructors

    public DiceGameItemComponent(DiceGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
        parent, proto, temporary)
    {
        _prototype = proto;
    }

    public DiceGameItemComponent(MudSharp.Models.GameItemComponent component, DiceGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public DiceGameItemComponent(DiceGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
        newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    protected void LoadFromXml(XElement root)
    {
        // TODO
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new DiceGameItemComponent(this, newParent, temporary);
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition").ToString();
    }

    #endregion

    #region IDice Implementation

    public string Roll()
    {
        return _prototype.Faces.ElementAt(_prototype.FaceProbabilities.GetWeightedRandom(x => x.Value).Key);
    }

    public string Roll(ICharacter cheater, string desiredFace)
    {
        ICheck cheatCheck = Gameworld.GetCheck(CheckType.CheatAtDiceCheck);
        CheckOutcome result = cheatCheck.Check(cheater, Difficulty.Normal);
        double multiplier = 1.0;
        switch (result.Outcome.SuccessDegrees())
        {
            case 1:
                multiplier = 1.2;
                break;
            case 2:
                multiplier = 1.5;
                break;
            case 3:
                multiplier = 2.75;
                break;
        }

        return _prototype.Faces.ElementAt(_prototype.FaceProbabilities.GetWeightedRandom(x =>
            _prototype.Faces.ElementAt(x.Key).EqualTo(desiredFace) ? multiplier * x.Value : x.Value).Key);
    }

    #endregion
}