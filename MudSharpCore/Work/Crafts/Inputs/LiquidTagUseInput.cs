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
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Inputs;

public class LiquidTagUseInput : BaseInput, ICraftInputConsumeLiquid
{
	public static void RegisterCraftInput()
	{
		CraftInputFactory.RegisterCraftInputType("LiquidTagUse",
			(input, craft, game) => new LiquidTagUseInput(input, craft, game));
		CraftInputFactory.RegisterCraftInputTypeForBuilders("tagliquid",
			(craft, game) => new LiquidTagUseInput(craft, game));
	}

	protected LiquidTagUseInput(Models.CraftInput input, ICraft craft, IFuturemud gameworld) : base(
		input, craft, gameworld)
	{
		var root = XElement.Parse(input.Definition);
		TargetTag = Gameworld.Tags.Get(long.Parse(root.Element("Tag").Value));
		TargetLiquidAmount = double.Parse(root.Element("Amount").Value);
	}

	protected LiquidTagUseInput(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
	{
	}

	public ITag TargetTag { get; set; }
	public double TargetLiquidAmount { get; set; }

	public override double ScoreInputDesirability(IPerceivable item)
	{
		return 1.0;
	}

	public override IEnumerable<IPerceivable> ScoutInput(ICharacter character)
	{
		var items = character.ContextualItems
		                     .SelectNotNull(x => x.GetItemType<ILiquidContainer>())
		                     .Select(x => (Container: x,
			                     CountsAs: x.LiquidMixture?.Instances.Any(y => y.Liquid.IsA(TargetTag)) ?? default))
		                     .Where(x => x.Container.IsOpen && x.CountsAs)
		                     .OrderByDescending(x => x.Container.Parent.InInventoryOf == character.Body)
		                     .ToList();

		if (items.Sum(x => x.Container.LiquidMixture.Instances.Where(y => y.Liquid.IsA(TargetTag)).Sum(y => y.Amount)) <
		    TargetLiquidAmount)
		{
			return Enumerable.Empty<IPerceivable>();
		}

		var sum = 0.0;
		var finalItems = new List<IGameItem>();
		foreach (var item in items)
		{
			sum += item.Container.LiquidMixture.Instances.Where(y => y.Liquid.IsA(TargetTag)).Sum(y => y.Amount);
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
		return item is IGameItem gi &&
		       gi.GetItemType<ILiquidContainer>() is ILiquidContainer ilc &&
		       ilc.IsOpen &&
		       (ilc.LiquidMixture?.Instances.Any(x => x.Liquid.IsA(TargetTag)) ?? false);
	}

	public override void UseInput(IPerceivable item, ICraftInputData data)
	{
		// Do nothing
	}

	public override ICraftInputData ReserveInput(IPerceivable input)
	{
		return new LiquidUseInputData(input, TargetTag, TargetLiquidAmount, Gameworld);
	}

	public override ICraftInputData LoadDataFromXml(XElement root, IFuturemud gameworld)
	{
		return new LiquidUseInputData(root, gameworld);
	}

	public override bool IsValid()
	{
		return TargetTag != null && TargetLiquidAmount > 0;
	}

	public override string WhyNotValid()
	{
		if (TargetTag == null)
		{
			return "You must first set a target tag.";
		}

		if (TargetLiquidAmount <= 0.0)
		{
			return "You must set a liquid amount that is higher than 0.";
		}

		throw new ApplicationException("Invalid WhyNotValid reason in LiquidUseInput.");
	}

	protected override string BuildingHelpString =>
		"You can use the following options:\n\t#3tag <tag>#0 - sets the target tag a liquid must have\n\t#3amount <amount>#0 - sets the amount to consume";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "tag":
			case "target":
				return BuildingCommandTag(actor, command);
			case "amount":
			case "quantity":
			case "volume":
				return BuildingCommandAmount(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to set this input to consume?");
			return false;
		}

		var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (matchedtags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (matchedtags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return false;
		}

		var tag = matchedtags.Single();

		TargetTag = tag;
		InputChanged = true;
		actor.OutputHandler.Send(
			$"This input will now consume liquids tagged {TargetTag.FullName.Colour(Telnet.Cyan)}.");
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

	protected override string InputType => "LiquidTagUse";

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Tag", TargetTag?.Id ?? 0),
			new XElement("Amount", TargetLiquidAmount)
		).ToString();
	}

	public override string Name
	{
		get
		{
			if (TargetTag == null)
			{
				return "An unspecified liquid tag";
			}

			return
				$"{Gameworld.UnitManager.DescribeMostSignificantExact(TargetLiquidAmount, UnitType.FluidVolume, DummyAccount.Instance)} of a liquid with the {TargetTag.FullName} tag"
					.Colour(Telnet.Green);
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		if (TargetTag == null)
		{
			return "An unspecified liquid";
		}

		return
			$"{Gameworld.UnitManager.DescribeMostSignificantExact(TargetLiquidAmount, UnitType.FluidVolume, DummyAccount.Instance).Colour(Telnet.Green)} of a liquid tagged {TargetTag.FullName.Colour(Telnet.Cyan)}";
	}

	internal class LiquidUseInputData : ICraftInputConsumeLiquidData
	{
		public LiquidUseInputData(IPerceivable group, ITag target, double amount, IFuturemud gameworld)
		{
			Gameworld = gameworld;
			OriginalItems = ((PerceivableGroup)group).Members.OfType<IGameItem>().ToList();
			Target = target;
			Amount = amount;
			ConsumeInput();
			_perceivable = new DummyPerceivable(ConsumedMixture.ColouredLiquidDescription,
				ConsumedMixture.LiquidDescription);
		}

		public LiquidUseInputData(XElement root, IFuturemud gameworld)
		{
			Gameworld = gameworld;
			Target = gameworld.Tags.Get(long.Parse(root.Element("TagId").Value));
			Amount = double.Parse(root.Element("Amount").Value);
			Quality = (ItemQuality)int.Parse(root.Element("Quality")?.Value ?? "0");
			ConsumedMixture = new LiquidMixture(root.Element("Mix"), gameworld);
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
				new XElement("TagId", Target.Id),
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
			var takenInstances = new List<LiquidInstance>();
			foreach (var container in OriginalItems)
			{
				var lcontainer = container.GetItemType<ILiquidContainer>();
				var instances = lcontainer.LiquidMixture.Instances
				                          .Where(x => x.Liquid.IsA(Target))
				                          .ToList();
				foreach (var instance in instances)
				{
					LiquidInstance newInstance;
					if (instance.Amount > target)
					{
						newInstance = instance.Copy();
						newInstance.Amount = target;
						takenInstances.Add(newInstance);
						qualities.Add((instance.Liquid.LiquidCountsAsQuality(instance.Liquid), target));
						lcontainer.LiquidMixture.RemoveLiquidVolume(instance, target);
						target = 0;
						break;
					}

					target -= instance.Amount;
					newInstance = instance.Copy();
					takenInstances.Add(newInstance);
					qualities.Add((instance.Liquid.LiquidCountsAsQuality(instance.Liquid), instance.Amount));
					lcontainer.LiquidMixture.RemoveLiquidInstance(instance);
				}

				if (target <= 0)
				{
					break;
				}
			}

			Quality = qualities.GetNetQuality();
			ConsumedMixture = new LiquidMixture(takenInstances, OriginalItems.First().Gameworld);
		}

		public List<IGameItem> OriginalItems { get; } = new();
		public IFuturemud Gameworld { get; set; }
		public ITag Target { get; set; }
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
}