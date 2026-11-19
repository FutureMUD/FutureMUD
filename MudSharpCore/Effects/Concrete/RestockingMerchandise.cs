using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class RestockingMerchandise : Effect
{
    public ICharacter CharacterOwner { get; }
    public IMerchandise TargetMerchandise { get; set; }
    public int QuantityToRestock { get; set; }
    public bool Ready { get; set; }
    public IGameItem? CarriedItem { get; set; }
    public int CarriedQuantity { get; set; }
    public IRoom? DisplayRoom { get; set; }
    public IFutureProg OnStartRestockingProg { get; }

    public RestockingMerchandise(ICharacter owner, IMerchandise targetMerchandise, int quantityToRestock,
        IFutureProg onStartRestockingProg) : base(owner)
    {
        CharacterOwner = owner;
        TargetMerchandise = targetMerchandise;
        QuantityToRestock = quantityToRestock;
        OnStartRestockingProg = onStartRestockingProg;
    }

    #region Overrides of Effect

    public override string Describe(IPerceiver voyeur)
    {
        return $"Restocking {QuantityToRestock}x {TargetMerchandise.Name}";
    }

    protected override string SpecificEffectType => "RestockingMerchandise";

    #endregion
}