using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class AdminSpyMaster : Effect, IEffectSubtype
{
    public List<ICell> SpiedCells { get; } = new();
    public List<AdminSpy> SpyEffects { get; } = new();
    public ICharacter CharacterOwner { get; }

    public AdminSpyMaster(ICharacter owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
    {
        CharacterOwner = owner;
        CharacterOwner.OnQuit += CharacterOwner_OnQuit;
    }

    public AdminSpyMaster(ICharacter owner, AdminSpyMaster other) : base(owner, null)
    {
        CharacterOwner = owner;
        CharacterOwner.OnQuit += CharacterOwner_OnQuit;

        foreach (ICell cell in other.SpiedCells)
        {
            SpiedCells.Add(cell);
            AdminSpy childEffect = new(cell, CharacterOwner);
            cell.AddEffect(childEffect);
            SpyEffects.Add(childEffect);
        }
    }

    #region Overrides of Effect

    /// <inheritdoc />
    public override void RemovalEffect()
    {
        foreach (AdminSpy effect in SpyEffects)
        {
            effect.Owner.RemoveEffect(effect);
        }

        SpyEffects.Clear();
        CharacterOwner.OnQuit -= CharacterOwner_OnQuit;
    }

    #endregion

    private void CharacterOwner_OnQuit(IPerceivable owner)
    {
        RemovalEffect();
    }

    public AdminSpyMaster(XElement effect, IPerceivable owner) : base(effect, owner)
    {
        CharacterOwner = (ICharacter)owner;
        CharacterOwner.OnQuit += CharacterOwner_OnQuit;
        foreach (XElement spy in effect.Element("Effect").Elements("Spy"))
        {
            ICell cell = Gameworld.Cells.Get(long.Parse(spy.Value));
            if (cell != null)
            {
                SpiedCells.Add(cell);
                AdminSpy childEffect = new(cell, CharacterOwner);
                cell.AddEffect(childEffect);
                SpyEffects.Add(childEffect);
            }
        }
    }

    public void RemoveSpiedCell(ICell cell)
    {
        SpiedCells.Remove(cell);
        cell.RemoveEffect(SpyEffects.FirstOrDefault(x => x.Owner == cell), true);
        if (!SpiedCells.Any())
        {
            CharacterOwner.RemoveEffect(this);
        }

        Changed = true;
    }

    public void AddSpiedCell(ICell cell)
    {
        if (SpiedCells.Contains(cell))
        {
            return;
        }

        SpiedCells.Add(cell);
        AdminSpy effect = new(cell, CharacterOwner);
        cell.AddEffect(effect);
        SpyEffects.Add(effect);
        Changed = true;
    }

    #region Overrides of Effect

    public override bool SavingEffect => true;

    public static void InitialiseEffectType()
    {
        RegisterFactory("AdminSpyMaster", (effect, owner) => new AdminSpyMaster(effect, owner));
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            from spy in SpiedCells
            select new XElement("Spy", spy.Id)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Spying on {SpyEffects.Count} locations";
    }

    protected override string SpecificEffectType => "AdminSpyMaster";

    public override void Login()
    {
        CharacterOwner.OnQuit += CharacterOwner_OnQuit;
        foreach (ICell cell in SpiedCells)
        {
            AdminSpy child = new(cell, CharacterOwner);
            cell.AddEffect(child);
            SpyEffects.Add(child);
        }
    }

    #endregion
}