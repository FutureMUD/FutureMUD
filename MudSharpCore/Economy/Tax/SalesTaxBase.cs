using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Economy.Tax;

public abstract class SalesTaxBase : SaveableItem, ISalesTax
{
	protected IEconomicZone EconomicZone { get; set; }

	protected SalesTaxBase(Models.EconomicZoneTax tax, IEconomicZone zone)
	{
		Gameworld = zone.Gameworld;
		EconomicZone = zone;
		ApplicabilityProg = Gameworld.FutureProgs.Get(tax.MerchandiseFilterProgId ?? 0);
		_id = tax.Id;
		_name = tax.Name;
		MerchantDescription = tax.MerchantDescription;
	}

	protected SalesTaxBase(string name, IEconomicZone zone, string defaultDefinition, string type)
	{
		Gameworld = zone.Gameworld;
		EconomicZone = zone;
		_name = name;
		MerchantDescription = "An undescribed tax";
		using (new FMDB())
		{
			var dbitem = new Models.EconomicZoneTax
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

	#region Overrides of FrameworkItem

	public sealed override string FrameworkItemType => "Tax";

	#endregion

	#region Implementation of IEditableItem

	protected virtual string HelpText =>
		"You can use the following options with this building command:\n\n\tname <name> - sets the name of the tax\n\tdescription <description> - sets the description of the tax\n\tprog <which> - sets the prog that controls whether this tax applies";

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

		actor.OutputHandler.Send(HelpText);
		return false;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to give to this tax?");
			return false;
		}

		MerchantDescription = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"You changed the description of the {_name.ColourName()} sales tax to {MerchantDescription.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this tax?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (EconomicZone.SalesTaxes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"The {EconomicZone.Name.ColourName()} economic zone already has a sales tax with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the sales tax from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog do you want to set as the filter prog for this sales tax?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must supply a prog that returns boolean whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Merchandise }) &&
		    !prog.MatchesParameters(new List<FutureProgVariableTypes>
			    { FutureProgVariableTypes.Merchandise, FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must supply a prog that accepts either a single merchandise parameter or a merchandise and a character parameter, while {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		ApplicabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} sales tax will now use the {prog.MXPClickableFunctionNameWithId()} prog to deteremine its applicability at point of sale.");
		return true;
	}

	public abstract string Show(ICharacter actor);

	#endregion

	#region Implementation of ISalesTax

	public string MerchantDescription { get; private set; }
	public IFutureProg ApplicabilityProg { get; private set; }
	public abstract bool Applies(IMerchandise merchandise, ICharacter purchaser);
	public abstract decimal TaxValue(IMerchandise merchandise, ICharacter purchaser);

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
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

	#endregion
}