using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Economy.Tax;

public abstract class HotelTaxBase : SaveableItem, IHotelTax
{
	protected IEconomicZone EconomicZone { get; set; }

	protected HotelTaxBase(EconomicZoneTax tax, IEconomicZone zone)
	{
		Gameworld = zone.Gameworld;
		EconomicZone = zone;
		ApplicabilityProg = Gameworld.FutureProgs.Get(tax.MerchandiseFilterProgId ?? 0);
		_id = tax.Id;
		_name = tax.Name;
		MerchantDescription = tax.MerchantDescription;
	}

	protected HotelTaxBase(string name, IEconomicZone zone, string defaultDefinition, string type)
	{
		Gameworld = zone.Gameworld;
		EconomicZone = zone;
		_name = name;
		MerchantDescription = "An undescribed hotel tax";
		using (new FMDB())
		{
			var dbitem = new EconomicZoneTax
			{
				Name = name,
				MerchantDescription = MerchantDescription,
				EconomicZoneId = zone.Id,
				TaxType = type,
				Definition = defaultDefinition
			};
			FMDB.Context.EconomicZoneTaxes.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public sealed override string FrameworkItemType => "Tax";
	public string MerchantDescription { get; private set; }
	public IFutureProg ApplicabilityProg { get; private set; }

	protected virtual string HelpText =>
		@"You can use the following options with this building command:

	#3name <name>#0 - sets the name of the tax
	#3description <description>#0 - sets the description of the tax
	#3prog <which>|none#0 - sets or clears the prog that controls whether this tax applies";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to give to this hotel tax?");
			return false;
		}

		MerchantDescription = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You changed the description of the {_name.ColourName()} hotel tax to {MerchantDescription.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this hotel tax?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (EconomicZone.HotelTaxes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"The {EconomicZone.Name.ColourName()} economic zone already has a hotel tax with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the hotel tax from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog do you want to set as the filter prog for this hotel tax?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			ApplicabilityProg = null;
			Changed = true;
			actor.OutputHandler.Send($"The {Name.ColourName()} hotel tax will no longer use a filter prog.");
			return true;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must supply a prog that returns boolean whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return false;
		}

		if (!prog.MatchesParameters(Enumerable.Empty<ProgVariableTypes>()) &&
		    !prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must supply a prog that accepts either no parameters or a single character parameter, while {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		ApplicabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} hotel tax will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine its applicability.");
		return true;
	}

	public bool Applies(IProperty property, ICharacter patron)
	{
		if (ApplicabilityProg is null)
		{
			return true;
		}

		return ApplicabilityProg.MatchesParameters(new[] { ProgVariableTypes.Character })
			? ApplicabilityProg.ExecuteBool(false, patron)
			: ApplicabilityProg.ExecuteBool(false);
	}

	public abstract decimal TaxValue(IProperty property, ICharacter patron, decimal rentalCharge);
	public abstract string Show(ICharacter actor);

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id == 0)
		{
			return;
		}

		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.EconomicZoneTaxes.Find(Id);
			if (dbitem != null)
			{
				FMDB.Context.EconomicZoneTaxes.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}
}
