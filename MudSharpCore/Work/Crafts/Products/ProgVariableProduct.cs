using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Work.Crafts.Products
{
	internal class ProgVariableProduct : SimpleProduct
	{
		protected ProgVariableProduct(Models.CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft,
		gameworld)
		{
			var root = XElement.Parse(product.Definition);
			foreach (var item in root.Elements("Variable"))
			{
				Characteristics.Add((gameworld.Characteristics.Get(long.Parse(item.Value)),
					Gameworld.FutureProgs.Get(long.Parse(item.Attribute("inputindex").Value))));
			}
		}

		protected ProgVariableProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld,
			failproduct)
		{
		}
		public new static void RegisterCraftProduct()
		{
			CraftProductFactory.RegisterCraftProductType("ProgVariableProduct",
				(product, craft, game) => new ProgVariableProduct(product, craft, game));
			CraftProductFactory.RegisterCraftProductTypeForBuilders("progvariable",
				(craft, game, fail) => new ProgVariableProduct(craft, game, fail));
		}

		public override string ProductType => "ProgVariableProduct";

		public List<(ICharacteristicDefinition Definition, IFutureProg Prog)> Characteristics { get; } = new();

		protected override string SaveDefinition()
		{
			return new XElement("Definition",
				new XElement("ProductProducedId", ProductProducedId),
				new XElement("Quantity", Quantity),
				from item in Characteristics
				select new XElement("Variable", new XAttribute("prog", item.Prog.Id), item.Definition.Id)
			).ToString();
		}

		protected override string BuildingHelpText =>
		$@"{base.BuildingHelpText}
	#3variable <definition> <prog>#0 - toggles a variable to be set on this item
	#3variable <definition>#0 - toggles a variable off for this item";

		public override bool BuildingCommand(ICharacter actor, StringStack command)
		{
			switch (command.PopSpeech().ToLowerInvariant())
			{
				case "variable":
				case "var":
				case "characteristic":
				case "char":
				case "definition":
					return BuildingCommandVariable(actor, command);
			}

			return base.BuildingCommand(actor, new StringStack($"{command.Last} {command.RemainingArgument}"));
		}

		private bool BuildingCommandVariable(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which characteristic definition did you want to change?");
				return false;
			}

			var definition = Gameworld.Characteristics.GetByIdOrName(command.PopSpeech());
			if (definition == null)
			{
				actor.OutputHandler.Send("There is no such characteristic definition.");
				return false;
			}

			if (command.IsFinished)
			{
				if (Characteristics.Any(x => x.Definition == definition))
				{
					Characteristics.RemoveAll(x => x.Definition == definition);
					ProductChanged = true;
					actor.OutputHandler.Send(
						$"This product will no longer be supplied the variable {definition.Name.Colour(Telnet.Yellow)}.");
					return true;
				}
			}

			var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Number, new FutureProgVariableTypes[]
			{
				FutureProgVariableTypes.Item | FutureProgVariableTypes.Collection
			}).LookupProg();
			if (prog is null)
			{
				return false;
			}

			if (Characteristics.Any(x => x.Definition == definition))
			{
				Characteristics.RemoveAll(x => x.Definition == definition);
			}

			Characteristics.Add((definition, prog));
			actor.OutputHandler.Send(
				$"This input will now be supplied the variable {definition.Name.Colour(Telnet.Yellow)} from the prog {prog.MXPClickableFunctionName()}.");
			ProductChanged = true;
			return true;
		}

		public override bool IsValid()
		{
			return base.IsValid() && 
				Characteristics.All(x => 
					string.IsNullOrWhiteSpace(x.Prog.CompileError) &&
					x.Prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean) &&
					x.Prog.MatchesParameters(new FutureProgVariableTypes[]
					{
						FutureProgVariableTypes.Item | FutureProgVariableTypes.Collection
					})
				);
		}

		public override string WhyNotValid()
		{
			var sb = new StringBuilder();
			foreach (var (_, prog) in Characteristics)
			{
				if (!string.IsNullOrWhiteSpace(prog.CompileError))
				{
					sb.AppendLine($"Variable Product Prog {prog.MXPClickableFunctionName()} is not compiled");
				}

				if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
				{
					sb.AppendLine($"Variable Product Prog {prog.MXPClickableFunctionName()} does not return a number");
				}

				if (!prog.MatchesParameters(new FutureProgVariableTypes[]
					{
						FutureProgVariableTypes.Perceivable | FutureProgVariableTypes.Collection
					}))
				{
					sb.AppendLine($"Variable Product Prog {prog.MXPClickableFunctionName()} does not accept the right parameters");
				}
			}

			if (sb.Length > 0)
			{
				return sb.ToString();
			}

			return base.WhyNotValid();
		}

		public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
		{
			var proto = Gameworld.ItemProtos.Get(ProductProducedId);
			if (proto is null)
			{
				throw new ApplicationException("Couldn't find a valid proto for craft product to load.");
			}

			var variables = new List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>();
			foreach (var (definition, prog) in Characteristics)
			{
				var value = Gameworld.CharacteristicValues.Get(prog.ExecuteLong(0L, new List<IPerceivable>(component.ConsumedInputs.Values.SelectNotNull(x => x.Data.Perceivable).GetIndividualPerceivables())));
				value ??= definition.GetRandomValue();
				variables.Add((definition, value));
			}

			var material = DetermineOverrideMaterial(component);

			if (Quantity > 1 && proto.IsItemType<StackableGameItemComponentProto>())
			{
				var newItem = proto.CreateNew();
				newItem.Skin = Skin;
				newItem.RoomLayer = component.Parent.RoomLayer;
				Gameworld.Add(newItem);
				newItem.GetItemType<IStackable>().Quantity = Quantity;
				newItem.Quality = referenceQuality;
				if (material != null)
				{
					newItem.Material = material;
				}

				var varItem = newItem.GetItemType<IVariable>();
				if (varItem != null)
				{
					foreach (var (definition, value) in variables)
					{
						varItem.SetCharacteristic(definition, value);
					}
				}

				return new SimpleProductData(new[] { newItem });
			}

			var items = new List<IGameItem>();
			for (var i = 0; i < Quantity; i++)
			{
				var item = proto.CreateNew();
				item.Skin = Skin;
				item.RoomLayer = component.Parent.RoomLayer;
				Gameworld.Add(item);
				item.Quality = referenceQuality;
				if (material != null)
				{
					item.Material = material;
				}

				var varItem = item.GetItemType<IVariable>();
				if (varItem != null)
				{
					foreach (var (definition, value) in variables)
					{
						varItem.SetCharacteristic(definition, value);
					}
				}

				items.Add(item);
			}

			return new SimpleProductData(items);
		}

		public override string Name
		{
			get
			{
				var sb = new StringBuilder();
				sb.Append(Quantity)
				  .Append("x ");

				if (Skin is not null)
				{
					sb.Append((Skin.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription)
						.ConcatIfNotEmpty($" [reskinned: #{Skin.Id:N0}]") ?? "an unspecified item".Colour(Telnet.Red));
				}
				else
				{
					sb.Append(Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ??
							  "an unspecified item".Colour(Telnet.Red));
				}

				foreach (var (definition, input) in Characteristics)
				{
					sb.Append(" ").Append(definition.Name).Append(" <- ").Append(input.MXPClickableFunctionName());
				}

				return sb.ToString();
			}
		}

		public override string HowSeen(IPerceiver voyeur)
		{
			var sb = new StringBuilder();
			sb.Append(Quantity)
			  .Append("x ");

			if (Skin is not null && voyeur is ICharacter ch && ch.IsAdministrator())
			{
				sb.Append((Skin.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription)
						  .ConcatIfNotEmpty($" [reskinned: #{Skin.Id.ToString("N0", voyeur)}]") ??
						  "an unspecified item".Colour(Telnet.Red));
			}
			else
			{
				sb.Append(Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ??
						  "an unspecified item".Colour(Telnet.Red));
			}

			foreach (var (definition, input) in Characteristics)
			{
				sb.Append(" ").Append(definition.Name).Append(" <- ").Append(input.MXPClickableFunctionName());
			}

			return sb.ToString();
		}
	}
}
