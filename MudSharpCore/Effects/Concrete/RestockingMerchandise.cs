using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class RestockingMerchandise : Effect
{
	public ICharacter CharacterOwner { get; }
	public Queue<ICellExit> CellExitQueue { get; private set; }
	public IMerchandise TargetMerchandise { get; set; }
	public int QuantityToRestock { get; set; }
	public List<IGameItem> CurrentGameItems { get; } = new();
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