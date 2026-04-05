using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class HasLegalCounsel : Effect, IEffect
{
    #region Static Initialisation
    public static void InitialiseEffectType()
    {
        RegisterFactory("HasLegalCounsel", (effect, owner) => new HasLegalCounsel(effect, owner));
    }
    #endregion

    #region Constructors
    public HasLegalCounsel(ICharacter owner, ICharacter lawyer) : base(owner, null)
    {
        _lawyer = lawyer;
        _lawyerId = lawyer.Id;
    }

    protected HasLegalCounsel(XElement effect, IPerceivable owner) : base(effect, owner)
    {
        XElement root = effect.Element("Effect");
    }
    #endregion

    // Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)
    #region Saving and Loading
    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("Lawyer", _lawyerId)
        );
    }
    #endregion

    #region Overrides of Effect
    protected override string SpecificEffectType => "HasLegalCounsel";

    public override string Describe(IPerceiver voyeur)
    {
        return "An undescribed effect of type HasLegalCounsel.";
    }

    public override bool SavingEffect => true;
    #endregion

#nullable enable
    private long _lawyerId;
    private ICharacter? _lawyer;

    public ICharacter? Lawyer => _lawyer ??= Gameworld.TryGetCharacter(_lawyerId, true);
}
