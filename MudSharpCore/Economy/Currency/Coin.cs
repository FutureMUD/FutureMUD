using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System.Text;
using Microsoft.Extensions.Logging;
using MudSharp.Database;
using MudSharp.Framework.Units;
using MudSharp.PerceptionEngine;

namespace MudSharp.Economy.Currency;

public class Coin : SaveableItem, ICoin
{
	public ICurrency Currency { get; }

	public Coin(IFuturemud gameworld, MudSharp.Models.Coin coin, ICurrency currency)
	{
		Gameworld = gameworld;
		_id = coin.Id;
		_name = coin.Name;
		ShortDescription = coin.ShortDescription;
		FullDescription = coin.FullDescription;
		Weight = coin.Weight;
		Value = coin.Value;
		GeneralForm = coin.GeneralForm;
		PluralWord = coin.PluralWord;
		Currency = currency;
		UseForChange = coin.UseForChange;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Coins.Find(Id);
		dbitem.Name = Name;
		dbitem.ShortDescription = ShortDescription;
		dbitem.FullDescription = FullDescription;
		dbitem.Weight = Weight;
		dbitem.Value = Value;
		dbitem.GeneralForm = GeneralForm;
		dbitem.PluralWord = PluralWord;
		dbitem.UseForChange = UseForChange;
		Changed = false;
	}

	public override string FrameworkItemType => "Coin";

	#region ICoin Members

	public string GeneralForm { get; private set; }

	public string PluralWord { get; private set; }

	public string ShortDescription { get; private set; }

	public string FullDescription { get; private set; }

	public decimal Value { get; private set; }

	public double Weight { get; private set; }
	public bool UseForChange { get; private set; }

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames the coin
	#3general <general>#0 - the general form of the coin (e.g. note, coin, etc)
	#3plural <word>#0 - the keyword from the sdesc to pluralise (e.g. coin, penny, bill)
	#3sdesc <sdesc>#0 - sets the short description of the coin
	#3desc#0 - drops you into an editor to edit the full description of the coin
	#3value <##>#0 - sets the base value of the coin
	#3weight <weight>#0 - sets the weight of each coin
	#3change#0 - toggles this coin being used for giving change";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "general":
				return BuildingCommandGeneral(actor, command);
			case "plural":
				return BuildingCommandPlural(actor, command);
			case "sdesc":
				return BuildingCommandSDesc(actor, command);
			case "fdesc":
			case "desc":
				return BuildingCommandDesc(actor);
			case "value":
				return BuildingCommandValue(actor, command);
			case "weight":
				return BuildingCommandWeight(actor, command);
			case "change":
				return BuildingCommandChange(actor);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandChange(ICharacter actor)
	{
		if (UseForChange && Currency.Coins.Except(this).All(x => !x.UseForChange))
		{
			actor.OutputHandler.Send("At least one coin per currency must be usable for change.");
			return false;
		}

		UseForChange = !UseForChange;
		Changed = true;
		actor.OutputHandler.Send($"This coin will {UseForChange.NowNoLonger()} be used by the engine to give change.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename this coin to?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Currency.Coins.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a coin for the {Currency.Name.ColourValue()} currency with the name {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the coin {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandGeneral(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What general form do you want for this coin (e.g. coin, note, etc)?");
			return false;
		}

		GeneralForm = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send($"The general form of this coin (how it will be grouped for describing piles of mixed currency) is now {GeneralForm.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPlural(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What word from the short description of this coin do you want to pluralise (e.g. coin, penny, bill, etc)?");
			return false;
		}

		PluralWord = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send($"The plural form of this coin is now {PluralWord.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What short description should this coin have?");
			return false;
		}

		ShortDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The short description of this coin is now {ShortDescription.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDesc(ICharacter actor)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(FullDescription))
		{
			sb.AppendLine("Replacing:\n");
			sb.AppendLine(FullDescription.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
		}

		sb.AppendLine("Enter the description in the editor below.");
		sb.AppendLine();
		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(DescPost, CancelAction, suppliedArguments: new object[]{ actor.InnerLineFormatLength});
		return true;
	}

	private void CancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send("\nYou decide not to change the description.");
	}

	private void DescPost(string text, IOutputHandler handler, object[] args)
	{
		FullDescription = text;
		Changed = true;
		handler.Send($"\nYou chanage the description to the following:\n\n{text.SubstituteANSIColour().Wrap((int)args[0])}");
	}

	private bool BuildingCommandValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either enter a number for the base currency, or enter a currency amount.");
			return false;
		}

		if (decimal.TryParse(command.SafeRemainingArgument, out var value))
		{
			if (value <= 0.0M)
			{
				actor.OutputHandler.Send("You must enter a valid number greater than zero for the value.");
				return false;
			}

			Value = value;
		}
		else
		{
			if (!Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out value))
			{
				actor.OutputHandler.Send($"You must either enter a number for the base currency, or enter a currency amount, and the text {command.SafeRemainingArgument.ColourCommand()} is neither.");
				return false;
			}

			Value = value;
		}

		Changed = true;
		actor.OutputHandler.Send($"The value of this coin in the base currency is now {value.ToString("N3", actor).ColourValue()} (or {Currency.Describe(value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}).");
		return true;
	}

	private bool BuildingCommandWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How heavy should each of these coins be?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, actor, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid weight.");
			return false;
		}

		Weight = value;
		Changed = true;
		actor.OutputHandler.Send($"Each of these coins now weigh {Gameworld.UnitManager.Describe(Weight, UnitType.Mass, actor).ColourValue()}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Coin #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"SDesc: {ShortDescription.ColourValue()}");
		sb.AppendLine($"General: {GeneralForm.ColourValue()}");
		sb.AppendLine($"Plural: {PluralWord.ColourValue()}");
		sb.AppendLine($"Value: {Value.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Weight: {Gameworld.UnitManager.Describe(Weight, Framework.Units.UnitType.Mass, actor).ColourValue()}");
		sb.AppendLine($"Use For Change: {UseForChange.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Full Description:");
		sb.AppendLine();
		sb.AppendLine(FullDescription.Wrap(actor.InnerLineFormatLength, "\t").SubstituteANSIColour());
		return sb.ToString();
	}

	#endregion
}