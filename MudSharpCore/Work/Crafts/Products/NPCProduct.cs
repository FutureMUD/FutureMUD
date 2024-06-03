using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.CharacterCreation;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.NPC.Templates;

namespace MudSharp.Work.Crafts.Products;
internal class NPCProduct : BaseProduct
{
	internal class NPCProductData : ICraftProductData
	{
		/// <inheritdoc />
		public IPerceivable Perceivable { get; set; }

		public long NPCId { get; set; }

		public ICharacterTemplate CharacterTemplate { get; set; }

		/// <inheritdoc />
		public XElement SaveToXml()
		{
			return new XElement("Data",
				new XElement("NPCId", NPCId)
			);
		}

		/// <inheritdoc />
		public void FinaliseLoadTimeTasks()
		{
			// Do nothing
		}

		/// <inheritdoc />
		public void ReleaseProducts(ICell location, RoomLayer layer)
		{
			// Do nothing
		}

		/// <inheritdoc />
		public void Delete()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void Quit()
		{
			throw new NotImplementedException();
		}
	}

	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("NPCProduct",
			(product, craft, game) => new NPCProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("npc",
			(craft, game, fail) => new NPCProduct(craft, game, fail));
	}

	/// <inheritdoc />
	public NPCProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft, gameworld)
	{
	}

	/// <inheritdoc />
	public NPCProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld, failproduct)
	{
	}

	/// <inheritdoc />
	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component, ItemQuality referenceQuality)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override bool IsValid()
	{
		return _npcId != 0 && NPCTemplate?.Status == RevisionStatus.Current;
	}

	/// <inheritdoc />
	public override string WhyNotValid()
	{
		if (_npcId == 0)
		{
			return "You must first set an NPC for this product to load.";
		}

		var npc = NPCTemplate;
		if (npc is null)
		{
			return "Couldn't find the NPC associated with the set ID. It might have been deleted.";
		}

		if (npc.Status != RevisionStatus.Current)
		{
			return $"The NPC Template {npc.ReferenceDescription(new DummyPerceiver())} is not approved for use.";
		}

		return "You can't do that for an unknown reason.";
	}

	/// <inheritdoc />
	public override string HowSeen(IPerceiver voyeur)
	{
		return $"{Quantity.ToString("N0", voyeur)}x {NPCTemplate?.ReferenceDescription(voyeur)}{(OnLoadProg is not null ? $" [OnLoad: {OnLoadProg.MXPClickableFunctionName()}]" : "")}";
	}

	/// <inheritdoc />
	public override string ProductType { get; }

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		throw new NotImplementedException();
	}

	private long _npcId;

	public INPCTemplate NPCTemplate => Gameworld.NpcTemplates.Get(_npcId);

	public int Quantity { get; set; }

	public IFutureProg OnLoadProg { get; set; }

	/// <inheritdoc />
	public override string Name => $"{Quantity}x {NPCTemplate?.ReferenceDescription(new DummyPerceiver())}{(OnLoadProg is not null ? $" [OnLoad: {OnLoadProg.MXPClickableFunctionName()}]" : "")}";

}
