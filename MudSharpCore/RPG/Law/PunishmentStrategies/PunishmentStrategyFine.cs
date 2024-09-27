using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Law.PunishmentStrategies;

public class PunishmentStrategyFine : PunishmentStrategyBase
{
	public ICurrency Currency { get; set; }
	public decimal FineAmount { get; set; }
	public decimal MaximumFineAmount { get; set; }

	public PunishmentStrategyFine(IFuturemud gameworld, ILegalAuthority authority) : base(gameworld)
	{
		Currency = authority.Currency;
		FineAmount = Gameworld.GetStaticDecimal("DefaultFineAmount");
		MaximumFineAmount = Gameworld.GetStaticDecimal("DefaultMaximumFineAmount");
	}

	public PunishmentStrategyFine(IFuturemud gameworld, XElement root, ILegalAuthority authority) : base(gameworld,
		root)
	{
		Currency = authority.Currency;
		FineAmount = decimal.Parse(root.Element("DefaultFineAmount").Value);
		MaximumFineAmount = decimal.Parse(root.Element("MaximumFineAmount").Value);
	}

	public override string TypeSpecificHelpText => @"
	fine <amount> - sets the standard fine
	maxfine <amount> - sets the maximum fine that can be manually imposed";

	public override bool BuildingCommand(ICharacter actor, ILegalAuthority authority, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "fine":
				return BuildingCommandFine(actor, command);
			case "max":
			case "maximum":
			case "maxfine":
			case "maximumfine":
				return BuildingCommandMaximumFine(actor, command);
		}

		return base.BuildingCommand(actor, authority, command.GetUndo());
	}

	private bool BuildingCommandMaximumFine(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set as the maximum fine?");
			return false;
		}

		if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of {Currency.Name.ColourName()}.");
			return false;
		}

		MaximumFineAmount = amount;
		if (FineAmount > MaximumFineAmount)
		{
			FineAmount = MaximumFineAmount;
		}

		actor.OutputHandler.Send(
			$"The maximum fine is now {Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandFine(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set as the standard fine?");
			return false;
		}

		if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of {Currency.Name.ColourName()}.");
			return false;
		}

		FineAmount = amount;
		if (FineAmount > MaximumFineAmount)
		{
			MaximumFineAmount = FineAmount;
		}

		actor.OutputHandler.Send(
			$"The standard fine is now {Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()}.");
		return true;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"a fine of between {Currency.Describe(FineAmount, CurrencyDescriptionPatternType.Short).ColourValue()} and {Currency.Describe(MaximumFineAmount, CurrencyDescriptionPatternType.Short).ColourValue()}";
	}

	public override PunishmentResult GetResult(ICharacter actor, ICrime crime, double severity = 0)
	{
		return new PunishmentResult
		{
			Fine = FineAmount + ((decimal)severity * MaximumFineAmount - FineAmount)
		};
	}

	/// <inheritdoc />
	public override PunishmentOptions GetOptions(ICharacter actor, ICrime crime)
	{
		return new PunishmentOptions
		{
			MinimumFine = FineAmount,
			MaximumFine = MaximumFineAmount
		};
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Fine".ColourName());
		sb.AppendLine($"Currency: {Currency.Name.ColourValue()}");
		sb.AppendLine(
			$"Standard Fine: {Currency.Describe(FineAmount, CurrencyDescriptionPatternType.Short).ColourValue()}");
		sb.AppendLine(
			$"Maximum Fine: {Currency.Describe(MaximumFineAmount, CurrencyDescriptionPatternType.Short).ColourValue()}");
		BaseShowText(actor, sb);
		return sb.ToString();
	}

	protected override void SaveSpecificType(XElement root)
	{
		root.Add(new XAttribute("type", "fine"));
		root.Add(new XElement("DefaultFineAmount", FineAmount));
		root.Add(new XElement("MaximumFineAmount", MaximumFineAmount));
	}
}