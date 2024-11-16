using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Economy.Tax;
public abstract class ProfitTaxBase : SaveableItem, IProfitTax
{
	#region Overrides of FrameworkItem

	public sealed override string FrameworkItemType => "Tax";

	#endregion

	protected ProfitTaxBase(Models.EconomicZoneTax tax, IEconomicZone zone)
	{
		Gameworld = zone.Gameworld;
		EconomicZone = zone;
		ApplicabilityProg = Gameworld.FutureProgs.Get(tax.MerchandiseFilterProgId ?? 0);
		_id = tax.Id;
		_name = tax.Name;
		MerchantDescription = tax.MerchantDescription;
	}

	protected ProfitTaxBase(string name, IEconomicZone zone, string defaultDefinition, string type)
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

	protected IEconomicZone EconomicZone { get; set; }

	protected string HelpText =>
		$@"You can use the following options with this building command:

	#3name <name>#0 - sets the name of the tax
	#3description <description>#0 - sets the description of the tax
	#3prog <which>#0 - sets the prog that controls whether this tax applies
	{SubtypeHelp}

Note - the available parameters for the applicability prog are #6Shop#0, #6Gross Revenue#0, #6Net Profit#0";

	protected abstract string SubtypeHelp { get; }

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
		if (EconomicZone.ProfitTaxes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"The {EconomicZone.Name.ColourName()} economic zone already has a profit tax with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the profit tax from {_name.ColourName()} to {name.ColourName()}.");
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

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Shop],
				[ProgVariableTypes.Shop, ProgVariableTypes.Number],
				[ProgVariableTypes.Shop, ProgVariableTypes.Number, ProgVariableTypes.Number],
			]
			).LookupProg();
		if (prog == null)
		{
			return false;
		}

		ApplicabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} profit tax will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine its applicability at financial period end.");
		return true;
	}

	/// <inheritdoc />
	public abstract string Show(ICharacter actor);

	public string MerchantDescription { get; private set; }
	public IFutureProg ApplicabilityProg { get; private set; }

	/// <inheritdoc />
	public bool Applies(IShop shop, decimal grossProfit, decimal netProfit)
	{
		return ApplicabilityProg?.ExecuteBool(false, shop, grossProfit, netProfit) ?? false;
	}

	/// <inheritdoc />
	public abstract decimal TaxValue(IShop shop, decimal grossProfit, decimal netProfit);
}
