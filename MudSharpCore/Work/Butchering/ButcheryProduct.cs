using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;

namespace MudSharp.Work.Butchering;

public class ButcheryProduct : SaveableItem, IButcheryProduct
{
	public ButcheryProduct(ButcheryProducts product, IFuturemud gameworld)
	{
		_id = product.Id;
		_name = product.Name;
		Subcategory = product.Subcategory.ToLowerInvariant();
		TargetBody = gameworld.BodyPrototypes.Get(product.TargetBodyId);
		foreach (var item in product.ButcheryProductsBodypartProtos)
		{
			var bodypart = gameworld.BodypartPrototypes.Get(item.BodypartProtoId);
			if (!bodypart.Body.CountsAs(TargetBody))
			{
				continue;
			}

			_requiredBodyparts.Add(bodypart);
		}

		IsPelt = product.IsPelt;
		CanProduceProg = gameworld.FutureProgs.Get(product.CanProduceProgId ?? 0L);
		foreach (var item in product.ButcheryProductItems)
		{
			_productItems.Add(new ButcheryProductItem(item, gameworld));
		}
	}

	#region Overrides of Item

	public override string FrameworkItemType => "ButcheryProduct";

	#endregion

	#region Implementation of IButcheryProduct

	/// <summary>
	/// The body prototype to which this product applies
	/// </summary>
	public IBodyPrototype TargetBody { get; set; }

	private readonly HashSet<IBodypart> _requiredBodyparts = new();

	/// <summary>
	/// The bodyparts that an item or corpse is required to contain to be butchered
	/// </summary>
	public IEnumerable<IBodypart> RequiredBodyparts => _requiredBodyparts;

	/// <summary>
	/// Whether this counts as a pelt, i.e. requires the SKIN verb rather than the BUTCHER/SALVAGE verb.
	/// </summary>
	public bool IsPelt { get; set; }

	/// <summary>
	/// A prog accepting a character and an item parameter that determines whether this product can be produced
	/// </summary>
	public IFutureProg CanProduceProg { get; set; }

	/// <summary>
	/// Determines whether a butcher can produce this product from a given item
	/// </summary>
	/// <param name="butcher">The character who is butchering</param>
	/// <param name="targetItem">The bodypart or corpse being butchered</param>
	/// <returns>True if this product applies</returns>
	public bool CanProduce(ICharacter butcher, IGameItem targetItem)
	{
		return (bool?)CanProduceProg?.Execute(butcher, targetItem) ?? true;
	}

	/// <summary>
	/// An optional string specifying a category to which this product belongs, i.e. if someone does SALVAGE CORPSE ELECTRONICS to only salvage electronics, electronics would be the subcategory
	/// </summary>
	public string Subcategory { get; set; }

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
			case "":
				actor.OutputHandler.Send(Show(actor));
				return true;
		}

		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename this butchery product to?");
			return false;
		}

		var name = command.SafeRemainingArgument.ToLowerInvariant().TitleCase();
		if (Gameworld.ButcheryProducts.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a butchery product with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"");
		return true;
	}

	private bool BuildingCommandSubcategory(ICharacter actor, StringStack command)
	{
		return true;
	}

	private bool BuildingCommandPelt(ICharacter actor)
	{
		return true;
	}

	private bool BuildingCommandBody(ICharacter actor, StringStack command)
	{
		return true;
	}

	private bool BuildingCommandBodypart(ICharacter actor, StringStack command)
	{
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		return true;
	}

	/// <summary>
	/// Returns a builder-specific view of this butchery product
	/// </summary>
	/// <param name="voyeur">The builder viewing the info</param>
	/// <returns>A textual representation of the butchery product</returns>
	public string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Butchery Product #{Id.ToString("N0", voyeur)} - {Name.ColourName()}");
		sb.AppendLine(
			$"Subcategory: {(string.IsNullOrWhiteSpace(Subcategory) ? "None".Colour(Telnet.Red) : Subcategory.TitleCase().ColourValue())}");
		sb.AppendLine($"Is Pelt: {IsPelt.ToColouredString()}");
		sb.AppendLine($"Target Body: {TargetBody.Name.ColourName()}");
		sb.AppendLine(
			$"Required Bodyparts: {RequiredBodyparts.Select(x => x.FullDescription().ColourValue()).ListToString()}");
		sb.AppendLine(
			$"Can Produce Prog: {CanProduceProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine();
		sb.AppendLine("Items:");
		var i = 1;
		sb.AppendLine(StringUtilities.GetTextTable(
			from product in _productItems
			select new List<string>
			{
				i++.ToString("N0", voyeur),
				$"{product.NormalQuantity.ToString("N0", voyeur)}x {product.NormalProto.EditHeader()}",
				product.DamagedProto != null
					? $"{product.DamagedQuantity.ToString("N0", voyeur)}x {product.DamagedProto.EditHeader()}"
					: "None",
				product.DamagedThreshold.ToString("P2", voyeur)
			},
			new List<string>
			{
				"No.",
				"Item",
				"Damaged Item",
				"Threshold"
			},
			voyeur.LineFormatLength,
			unicodeTable: voyeur.Account.UseUnicode
		));
		return sb.ToString();
	}

	private readonly List<IButcheryProductItem> _productItems = new();

	/// <summary>
	/// The actual items produced when this product is invoked
	/// </summary>
	public IEnumerable<IButcheryProductItem> ProductItems => _productItems;

	#endregion

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.ButcheryProducts.Find(Id);
		dbitem.Name = Name;
		dbitem.IsPelt = IsPelt;
		dbitem.TargetBodyId = TargetBody.Id;
		dbitem.Subcategory = Subcategory;
		dbitem.CanProduceProgId = CanProduceProg?.Id;
		FMDB.Context.ButcheryProductsBodypartProtos.RemoveRange(dbitem.ButcheryProductsBodypartProtos);
		foreach (var item in _requiredBodyparts)
		{
			dbitem.ButcheryProductsBodypartProtos.Add(new ButcheryProductsBodypartProtos
				{ ButcheryProduct = dbitem, BodypartProtoId = item.Id });
		}

		Changed = false;
	}

	#endregion
}