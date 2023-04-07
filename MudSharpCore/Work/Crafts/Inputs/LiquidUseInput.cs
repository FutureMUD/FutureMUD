using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Inputs;

public class LiquidUseInput : BaseInput, ICraftInputConsumeLiquid
{
	public static void RegisterCraftInput()
	{
		CraftInputFactory.RegisterCraftInputType("LiquidUse",
			(input, craft, game) => new LiquidUseInput(input, craft, game));
		CraftInputFactory.RegisterCraftInputTypeForBuilders("liquid", (craft, game) => new LiquidUseInput(craft, game));
	}

	protected LiquidUseInput(CraftInput input, ICraft craft, IFuturemud gameworld) : base(input, craft, gameworld)
	{
		var root = XElement.Parse(input.Definition);
		TargetLiquid = Gameworld.Liquids.Get(long.Parse(root.Element("Liquid").Value));
		TargetLiquidAmount = double.Parse(root.Element("Amount").Value);
	}

	protected LiquidUseInput(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
	{
	}

	public ILiquid TargetLiquid { get; set; }
	public double TargetLiquidAmount { get; set; }

	public override double ScoreInputDesirability(IPerceivable item)
	{
		return 1.0;
	}

	internal class LiquidUseInputData : ICraftInputConsumeLiquidData
	{
		public LiquidUseInputData(IPerceivable group, ILiquid target, double amount, IFuturemud gameworld)
		{
			Gameworld = gameworld;
			OriginalItems = ((PerceivableGroup)group).Members.OfType<IGameItem>().ToList();
			Liquid = target;
			Amount = amount;
			_perceivable = new DummyPerceivable(
				voyeur =>
					$"{voyeur.Gameworld.UnitManager.DescribeMostSignificantExact(Amount, UnitType.FluidVolume, voyeur)} of {Liquid.Name}",
				voyeur =>
					$"{voyeur.Gameworld.UnitManager.DescribeMostSignificantExact(Amount, UnitType.FluidVolume, voyeur)} of {Liquid.Name}");
			ConsumeInput();
		}

		public LiquidUseInputData(XElement root, IFuturemud gameworld)
		{
			Gameworld = gameworld;
			Liquid = gameworld.Liquids.Get(long.Parse(root.Element("Liquid").Value));
			Amount = double.Parse(root.Element("Amount").Value);
			Quality = (ItemQuality)int.Parse(root.Element("Quality")?.Value ?? "0");
			ConsumedMixture = new LiquidMixture(root.Element("Mix"), Gameworld);
			foreach (var item in root.Elements("OriginalItem"))
			{
				var gitem = gameworld.TryGetItem(long.Parse(item.Value));
				if (gitem != null)
				{
					OriginalItems.Add(gitem);
				}
			}
		}

		public XElement SaveToXml()
		{
			return new XElement("Input",
				new XElement("Liquid", Liquid.Id),
				new XElement("Amount", Amount),
				new XElement("Quality", (int)Quality),
				ConsumedMixture.SaveToXml(),
				from item in OriginalItems
				select new XElement("OriginalItem", item.Id)
			);
		}

		public void ConsumeInput()
		{
			var target = Amount;
			var qualities = new List<(ItemQuality Quality, double Weight)>();
			ConsumedMixture = new LiquidMixture(Enumerable.Empty<LiquidInstance>(), Gameworld);
			foreach (var container in OriginalItems)
			{
				var lcontainer = container.GetItemType<ILiquidContainer>();
				if (lcontainer.LiquidVolume > target)
				{
					qualities.Add((lcontainer.LiquidMixture.CountsAs(Liquid).Quality, target));
					ConsumedMixture.AddLiquid(lcontainer.LiquidMixture.RemoveLiquidVolume(target));
					break;
				}

				target -= lcontainer.LiquidVolume;
				qualities.Add((lcontainer.LiquidMixture.CountsAs(Liquid).Quality, lcontainer.LiquidVolume));
				ConsumedMixture.AddLiquid(lcontainer.LiquidMixture);
				lcontainer.ReduceLiquidQuantity(lcontainer.LiquidVolume, null, "liquidcraft");
				if (target <= 0)
				{
					break;
				}
			}

			Quality = qualities.GetNetQuality();
		}

		public List<IGameItem> OriginalItems { get; } = new();
		public IFuturemud Gameworld { get; set; }
		public ILiquid Liquid { get; set; }
		public double Amount { get; set; }
		public ItemQuality Quality { get; set; }
		public LiquidMixture ConsumedMixture { get; set; }

		private IPerceivable _perceivable;
		public IPerceivable Perceivable => _perceivable;
		public ItemQuality InputQuality => Quality;

		public void FinaliseLoadTimeTasks()
		{
			foreach (var item in OriginalItems)
			{
				item.FinaliseLoadTimeTasks();
			}
		}

		public void Delete()
		{
			// Do nothing
		}

		public void Quit()
		{
			// Do nothing
		}
	}

	public override IEnumerable<IPerceivable> ScoutInput(ICharacter character)
	{
		var items = character.ContextualItems
		                     .SelectNotNull(x => x.GetItemType<ILiquidContainer>())
		                     .Select(x => (Container: x, CountsAs: x.LiquidMixture?.CountsAs(TargetLiquid) ?? default))
		                     .Where(x => x.Container.IsOpen && x.CountsAs.Truth)
		                     .OrderByDescending(x => x.Container.Parent.InInventoryOf == character.Body)
		                     .ThenByDescending(x => x.CountsAs.Quality)
		                     .ToList();

		if (items.Sum(x => x.Container.LiquidVolume) < TargetLiquidAmount)
		{
			return Enumerable.Empty<IPerceivable>();
		}

		var sum = 0.0;
		var finalItems = new List<IGameItem>();
		foreach (var item in items)
		{
			sum += item.Container.LiquidVolume;
			finalItems.Add(item.Container.Parent);
			if (sum >= TargetLiquidAmount)
			{
				break;
			}
		}

		return new[] { new PerceivableGroup(finalItems) };
	}

	public override bool IsInput(IPerceivable item)
	{
		return item is IGameItem gi && gi.GetItemType<ILiquidContainer>() is ILiquidContainer ilc && ilc.IsOpen &&
		       ilc.LiquidMixture?.CountsAs(TargetLiquid).Truth == true && ilc.LiquidVolume > TargetLiquidAmount;
	}

	public override void UseInput(IPerceivable item, ICraftInputData data)
	{
		// Do nothing
	}

	public override ICraftInputData ReserveInput(IPerceivable input)
	{
		return new LiquidUseInputData(input, TargetLiquid, TargetLiquidAmount, Gameworld);
	}

	public override ICraftInputData LoadDataFromXml(XElement root, IFuturemud gameworld)
	{
		return new LiquidUseInputData(root, gameworld);
	}

	protected override string BuildingHelpString =>
		"You can use the following options:\n\tliquid <liquid> - sets the liquid to consume\n\tamount <amount> - sets the amount to consume";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "liquid":
				return BuildingCommandLiquid(actor, command);
			case "amount":
			case "quantity":
			case "volume":
				return BuildingCommandAmount(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandLiquid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which liquid do you want to set this input to consume?");
			return false;
		}

		var liquid = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(command.SafeRemainingArgument);

		if (liquid == null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return false;
		}

		TargetLiquid = liquid;
		InputChanged = true;
		actor.OutputHandler.Send($"This input will now consume {liquid.Name} or equivalent.");
		return true;
	}

	private bool BuildingCommandAmount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much liquid do you want this input to consume?");
			return false;
		}

		var amount =
			Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid amount of liquid to consume.");
			return false;
		}

		if (amount <= 0)
		{
			actor.OutputHandler.Send("You must enter an amount greater than zero.");
			return false;
		}

		TargetLiquidAmount = amount;
		InputChanged = true;
		actor.OutputHandler.Send(
			$"This input will now consume {Gameworld.UnitManager.Describe(amount, UnitType.FluidVolume, actor).Colour(Telnet.Green)} of liquid.");
		return true;
	}

	public override bool IsValid()
	{
		return TargetLiquid != null && TargetLiquidAmount > 0;
	}

	public override string WhyNotValid()
	{
		if (TargetLiquid == null)
		{
			return "You must first set a target liquid.";
		}

		if (TargetLiquidAmount <= 0.0)
		{
			return "You must set a liquid amount that is higher than 0.";
		}

		throw new ApplicationException("Invalid WhyNotValid reason in LiquidUseInput.");
	}

	protected override string InputType => "LiquidUse";

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Liquid", TargetLiquid?.Id ?? 0),
			new XElement("Amount", TargetLiquidAmount)
		).ToString();
	}

	public override string Name
	{
		get
		{
			if (TargetLiquid == null)
			{
				return "An unspecified liquid";
			}

			return
				$"{Gameworld.UnitManager.DescribeMostSignificantExact(TargetLiquidAmount, UnitType.FluidVolume, DummyAccount.Instance)} of {TargetLiquid.Name}"
					.Colour(TargetLiquid.DisplayColour);
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		if (TargetLiquid == null)
		{
			return "An unspecified liquid";
		}

		return
			$"{Gameworld.UnitManager.DescribeMostSignificantExact(TargetLiquidAmount, UnitType.FluidVolume, DummyAccount.Instance)} of {TargetLiquid.Name}"
				.Colour(TargetLiquid.DisplayColour);
	}
}