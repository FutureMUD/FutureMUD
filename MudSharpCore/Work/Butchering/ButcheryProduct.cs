using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MoreLinq.Extensions;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Work.Butchering;

public class ButcheryProduct : SaveableItem, IButcheryProduct
{
	public ButcheryProduct(string name, IBodyPrototype targetBody, IBodypart bodypart, IGameItemProto proto,
		int quantity)
	{
		Gameworld = targetBody.Gameworld;
		_name = name;
		TargetBody = targetBody;
		Subcategory = string.Empty;
		_requiredBodyparts.Add(bodypart);
		using (new FMDB())
		{
			var dbitem = new Models.ButcheryProducts
			{
				Name = _name,
				IsPelt = IsPelt,
				TargetBodyId = TargetBody.Id,
				Subcategory = Subcategory,
			};
			foreach (var item in _requiredBodyparts)
			{
				dbitem.ButcheryProductsBodypartProtos.Add(new Models.ButcheryProductsBodypartProtos
					{ ButcheryProduct = dbitem, BodypartProtoId = item.Id });
			}

			FMDB.Context.ButcheryProducts.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
		_productItems.Add(new ButcheryProductItem(this, proto, quantity));
	}

	public ButcheryProduct(ButcheryProduct rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		IsPelt = rhs.IsPelt;
		TargetBody = rhs.TargetBody;
		Subcategory = rhs.Subcategory;
		CanProduceProg = rhs.CanProduceProg;

		foreach (var item in rhs.RequiredBodyparts)
		{
			_requiredBodyparts.Add(item);
		}

		using (new FMDB())
		{
			var dbitem = new Models.ButcheryProducts
			{
				Name = _name,
				IsPelt = IsPelt,
				TargetBodyId = TargetBody.Id,
				Subcategory = Subcategory,
				CanProduceProgId = CanProduceProg?.Id
			};
			foreach (var item in _requiredBodyparts)
			{
				dbitem.ButcheryProductsBodypartProtos.Add(new Models.ButcheryProductsBodypartProtos
					{ ButcheryProduct = dbitem, BodypartProtoId = item.Id });
			}
			
			FMDB.Context.ButcheryProducts.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		
		foreach (var item in rhs.ProductItems)
		{
			_productItems.Add(new ButcheryProductItem(this, item.NormalProto, item.NormalQuantity, item.DamagedProto, item.DamagedQuantity, item.DamagedThreshold));
		}
	}

	public ButcheryProduct(Models.ButcheryProducts product, IFuturemud gameworld)
	{
		Gameworld = gameworld;
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

	public IButcheryProduct Clone(string newName)
	{
		return new ButcheryProduct(this, newName);
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

	#region Building

	private const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this product
	#3category <category>#0 - changes the category of this product
	#3pelt#0 - toggles whether this is a pelt (i.e. from SKIN) or not (i.e. BUTCHER/SALVAGE)
	#3body <which> [<parts ...>]#0 - changes which body type this profile targets
	#3part <which>#0 - toggles requiring the corpse to have this bodypart
	#3prog <which>#0 - sets the prog that controls whether the item is produced
	#3item add <number> <proto> [<number> <damaged> <damage%>]#0 - adds a new item product
	#3item delete <##>#0 - deletes an item product
	#3item <##> quantity <number>#0 - changes the quantity of items produced
	#3item <##> proto <id>#0 - changes the proto produced
	#3item <##> threshold <%>#0 - changes the damage percentage for normal/damaged items
	#3item <##> damaged <quantity> <proto>#0 - changes the damaged proto
	#3item <##> nodamaged#0 - clears the damaged proto";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "subcategory":
			case "sub":
			case "category":
			case "cat":
				return BuildingCommandSubcategory(actor, command);
			case "pelt":
				return BuildingCommandPelt(actor);
			case "body":
				return BuildingCommandBody(actor, command);
			case "bodypart":
			case "part":
			case "bp":
				return BuildingCommandBodypart(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "item":
				return BuildingCommandItem(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return true;
		}

		return false;
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandItemAdd(actor, command);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandItemRemove(actor, command);
		}

		if (!long.TryParse(command.Last, out var index))
		{
			actor.OutputHandler.Send($"You must either use {"add".ColourCommand()} or {"delete".ColourCommand()}, or specify the ID of an existing product item.");
			return false;
		}

		var product = ProductItems.FirstOrDefault(x => x.Id == index);
		if (product is null)
		{
			actor.OutputHandler.Send("This product has no such product item.");
			return false;
		}
		
		switch (command.PopForSwitch())
		{
			case "quantity":
			case "number":
			case "amount":
				return BuildingCommandItemQuantity(actor, command, product);
			case "proto":
			case "item":
			case "prototype":
				return BuildingCommandItemPrototype(actor, command, product);
			case "threshold":
				return BuildingCommandThreshold(actor, command, product);
			case "damaged":
			case "damage":
			case "fail":
			case "failed":
				return BuildingCommandDamaged(actor, command, product);
			case "nodamaged":
				return BuildingCommandNoDamaged(actor, command, product);
			default:
				actor.OutputHandler.Send(@"You can use the following options to edit the product items:

	#3item <##> quantity <number>#0 - changes the quantity of items produced
	#3item <##> proto <id>#0 - changes the proto produced
	#3item <##> threshold <%>#0 - changes the damage percentage for normal/damaged items
	#3item <##> damaged <quantity> <proto>#0 - changes the damaged proto
	#3item <##> nodamaged#0 - clears the damaged proto".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandNoDamaged(ICharacter actor, StringStack command, IButcheryProductItem product)
	{
		product.DamagedQuantity = 0;
		product.DamagedProto = null;
		product.DamagedThreshold = 1.0;
		product.Changed = true;
		actor.OutputHandler.Send($"Product item #{product.Id.ToString("N0", actor)} will no longer produce any damaged product.");
		return true;
	}

	private bool BuildingCommandDamaged(ICharacter actor, StringStack command, IButcheryProductItem product)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many items should this product item produce in the damaged case?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var quantity) || quantity < 1)
		{
			actor.OutputHandler.Send("You must enter a valid quantity of items to load.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which item prototype should this product item load when the bodyparts are too damaged?");
			return false;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var id))
		{
			actor.OutputHandler.Send("You must enter a valid ID number for the item prototype.");
			return false;
		}

		var proto = Gameworld.ItemProtos.Get(id);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"{proto.EditHeader()} is not approved for use.");
			return false;
		}

		product.DamagedQuantity=quantity;
		product.DamagedProto = proto;
		product.Changed = true;
		actor.OutputHandler.Send($"Product item #{product.Id.ToString("N0", actor)} will now produce {quantity.ToString("N0", actor)}x {proto.EditHeader().ColourObject()} when bodyparts are too damaged.");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command, IButcheryProductItem product)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage of damage to bodyparts should trigger damaged items instead of normal items?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var percentage))
		{
			actor.OutputHandler.Send("That is not a valid percentage.");
			return false;
		}

		product.DamagedThreshold = percentage;
		product.Changed = true;
		actor.OutputHandler.Send($"Damage above {percentage.ToString("P2", actor).ColourValue()} of total health will now lead to damaged items being produced.");
		return true;
	}

	private bool BuildingCommandItemPrototype(ICharacter actor, StringStack command, IButcheryProductItem product)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which item prototype should this product item load?");
			return false;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var id))
		{
			actor.OutputHandler.Send("You must enter a valid ID number for the item prototype.");
			return false;
		}

		var proto = Gameworld.ItemProtos.Get(id);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"{proto.EditHeader()} is not approved for use.");
			return false;
		}

		product.NormalProto = proto;
		product.Changed = true;
		actor.OutputHandler.Send($"Product item #{product.Id.ToString("N0", actor)} will now produce {product.NormalQuantity.ToString("N0", actor)}x {proto.EditHeader().ColourObject()}.");
		return true;
	}

	private bool BuildingCommandItemQuantity(ICharacter actor, StringStack command, IButcheryProductItem product)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many items should this product item produce in the normal case?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var quantity) || quantity < 1)
		{
			actor.OutputHandler.Send("You must enter a valid quantity of items to load.");
			return false;
		}
		
		product.NormalQuantity = quantity;
		product.Changed = true;
		actor.OutputHandler.Send($"Product item #{product.Id.ToString("N0", actor)} will now produce {product.NormalQuantity.ToString("N0", actor)}x {product.NormalProto.EditHeader().ColourObject()}.");
		return true;
	}

	private bool BuildingCommandItemRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which product item do you want to remove?");
			return false;
		}

		if (!long.TryParse(command.Last, out var index))
		{
			actor.OutputHandler.Send("That is not a valid ID.");
			return false;
		}

		var product = ProductItems.FirstOrDefault(x => x.Id == index);
		if (product is null)
		{
			actor.OutputHandler.Send("This product has no such product item.");
			return false;
		}

		actor.OutputHandler.Send($"You delete product item #{product.Id.ToString("N0", actor)}.");
		product.Delete();
		_productItems.Remove(product);
		return true;
	}

	private bool BuildingCommandItemAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many items should this product item produce in the normal case?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var quantity) || quantity < 1)
		{
			actor.OutputHandler.Send("You must enter a valid quantity of items to load.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which item prototype should this product item load?");
			return false;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var id))
		{
			actor.OutputHandler.Send("You must enter a valid ID number for the item prototype.");
			return false;
		}

		var proto = Gameworld.ItemProtos.Get(id);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"{proto.EditHeader()} is not approved for use.");
			return false;
		}

		IGameItemProto damagedProto = null;
		var damagedQuantity = 0;
		if (!command.IsFinished)
		{
			if (!int.TryParse(command.PopSpeech(), out damagedQuantity) || damagedQuantity < 1)
			{
				actor.OutputHandler.Send("You must enter a valid quantity of damaged items to load.");
				return false;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which item prototype should this product item load when the bodyparts are too damaged?");
				return false;
			}

			if (!long.TryParse(command.SafeRemainingArgument, out var damagedId))
			{
				actor.OutputHandler.Send("You must enter a valid ID number for the damaged item prototype.");
				return false;
			}

			damagedProto = Gameworld.ItemProtos.Get(damagedId);
			if (damagedProto is null)
			{
				actor.OutputHandler.Send("There is no such damaged item prototype.");
				return false;
			}

			if (damagedProto.Status != RevisionStatus.Current)
			{
				actor.OutputHandler.Send($"{damagedProto.EditHeader()} is not approved for use.");
				return false;
			}
		}

		double percentage = 1.0;
		if (!command.IsFinished)
		{
			if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out percentage))
			{
				actor.OutputHandler.Send("That is not a valid percentage.");
				return false;
			}
		}

		var productItem = new ButcheryProductItem(this, proto, quantity, damagedProto, damagedQuantity, percentage);
		_productItems.Add(productItem);
		actor.OutputHandler.Send($"You create	");
		return true;
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

		Changed = true;
		actor.OutputHandler.Send($"You rename this butchery product from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		return true;
	}

	private bool BuildingCommandSubcategory(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which category do you want this product to belong to?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			Subcategory = "";
			Changed = true;
			actor.OutputHandler.Send($"This product no longer belongs to any specific subcategory.");
			return true;
		}

		Subcategory = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send($"This product now belongs to the {Subcategory.ColourValue()} subcategory.");
		return true;
	}

	private bool BuildingCommandPelt(ICharacter actor)
	{
		IsPelt = !IsPelt;
		Changed = true;
		actor.OutputHandler.Send($"This product is {IsPelt.NowNoLonger()} a pelt.");
		return true;
	}

	private bool BuildingCommandBody(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which body prototype would you like this product to be tied to?");
			return false;
		}

		var body = Gameworld.BodyPrototypes.GetByIdOrName(command.PopSpeech());
		if (body is null)
		{
			actor.OutputHandler.Send("There is no such body.");
			return false;
		}

		var parts = new List<IBodypart>();
		foreach (var part in RequiredBodyparts)
		{
			if (body.AllBodyparts.Any(x => part.CountsAs(x)))
			{
				parts.Add(part);
			}
		}

		if (command.IsFinished)
		{
			if (!parts.Any())
			{
				actor.OutputHandler.Send(
					$"You must specify at least one bodypart from the {body.Name.ColourName()} body for this product to be associated with.");
				return false;
			}
		}
		else
		{
			while (!command.IsFinished)
			{
				var part = body.AllBodyparts.GetByNameOrAbbreviation(command.PopSpeech());
				if (part is null)
				{
					actor.OutputHandler.Send($"The {body.Name.ColourName()} body does not contain any bodyparts identified by {command.Last.ColourCommand()}.");
					return false;
				}

				if (parts.Contains(part))
				{
					continue;
				}

				parts.Add(part);
			}
		}

		TargetBody = body;
		_requiredBodyparts.Clear();
		foreach (var part in parts)
		{
			_requiredBodyparts.Add(part);
		}

		Changed = true;
		actor.OutputHandler.Send($"This product now targets the {body.Name.ColourName()} body type and the {parts.Select(x => x.FullDescription().ColourValue()).ListToString()} body {"part".Pluralise(parts.Count > 1)}.");
		return true;
	}

	private bool BuildingCommandBodypart(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which bodypart would you like this product to require?");
			return false;
		}
		var part = TargetBody.AllBodyparts.GetByNameOrAbbreviation(command.SafeRemainingArgument);
		if (part is null)
		{
			actor.OutputHandler.Send($"The {TargetBody.Name.ColourName()} body does not contain any bodyparts identified by {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		if (_requiredBodyparts.Contains(part))
		{
			_requiredBodyparts.Remove(part);
			actor.OutputHandler.Send($"This product no longer requires the {part.FullDescription().ColourValue()} bodypart.");
		}
		else
		{
			_requiredBodyparts.Add(part);
			actor.OutputHandler.Send($"This product now requires the {part.FullDescription().ColourValue()} bodypart.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether this product is produced for a given corpse?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Item
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CanProduceProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This product will now use the {prog.MXPClickableFunctionName()} prog to determine whether it is produced.");
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
	#endregion

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
			dbitem.ButcheryProductsBodypartProtos.Add(new Models.ButcheryProductsBodypartProtos
				{ ButcheryProduct = dbitem, BodypartProtoId = item.Id });
		}

		Changed = false;
	}

	#endregion
}