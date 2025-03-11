using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.Models;

namespace MudSharp.Form.Material;

public class Gas : Fluid, IGas
{
	public Gas(MudSharp.Models.Gas gas, IFuturemud gameworld) : base(gas, gameworld)
	{
		SmellText = gas.SmellText;
		VagueSmellText = gas.VagueSmellText;
		SmellIntensity = gas.SmellIntensity;
		Viscosity = gas.Viscosity;
		_countsAsId = gas.CountAsId;
		CountsAsQuality = (ItemQuality)(gas.CountsAsQuality ?? 0);
		DisplayColour = Telnet.GetColour(gas.DisplayColour);
		BehaviourType = MaterialBehaviourType.Gas;
		CondensationTemperature = gas.BoilingPoint;
		_liquidFormId = gas.PrecipitateId;
		Drug = Gameworld.Drugs.Get(gas.DrugId ?? 0);
		DrugGramsPerUnitVolume = gas.DrugGramsPerUnitVolume;
	}

	public Gas(string name, IFuturemud gameworld) : base(name, MaterialBehaviourType.Gas, gameworld)
	{
		DisplayColour = Telnet.BoldCyan;
		CondensationTemperature = double.MinValue;
		CountsAsQuality = ItemQuality.Standard;

		using (new FMDB())
		{
			var dbitem = new Models.Gas
			{
				Name = name,
				Description = name,
				Density = Density,
				ThermalConductivity = ThermalConductivity,
				ElectricalConductivity = ElectricalConductivity,
				Organic = Organic,
				SpecificHeatCapacity = SpecificHeatCapacity,
				BoilingPoint = CondensationTemperature,
				CountAsId = _countsAsId,
				CountsAsQuality = (int)CountsAsQuality,
				DisplayColour = DisplayColour.Name,
				PrecipitateId = _liquidFormId,
				SmellIntensity = SmellIntensity,
				SmellText = SmellText,
				VagueSmellText = VagueSmellText,
				Viscosity = Viscosity,
				DrugId = Drug?.Id,
				DrugGramsPerUnitVolume = DrugGramsPerUnitVolume
			};
			FMDB.Context.Gases.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Gas(Gas rhs, string newName) : base(rhs, newName, MaterialBehaviourType.Gas)
	{
		_countsAsId = rhs._countsAsId;
		_liquidFormId = rhs._liquidFormId;
		CondensationTemperature = rhs.CondensationTemperature;
		CountsAsQuality = rhs.CountsAsQuality;

		using (new FMDB())
		{
			var dbitem = new Models.Gas
			{
				Name = Name,
				Description = Name,
				Density = Density,
				ThermalConductivity = ThermalConductivity,
				ElectricalConductivity = ElectricalConductivity,
				Organic = Organic,
				SpecificHeatCapacity = SpecificHeatCapacity,
				BoilingPoint = CondensationTemperature,
				CountAsId = _countsAsId,
				CountsAsQuality = (int)CountsAsQuality,
				DisplayColour = DisplayColour.Name,
				PrecipitateId = _liquidFormId,
				SmellIntensity = SmellIntensity,
				SmellText = SmellText,
				VagueSmellText = VagueSmellText,
				Viscosity = Viscosity,
				DrugId = Drug?.Id,
				DrugGramsPerUnitVolume = DrugGramsPerUnitVolume
			};
			FMDB.Context.Gases.Add(dbitem);

			foreach (var tag in Tags)
			{
				dbitem.GasesTags.Add(new GasesTags
				{
					Gas = dbitem,
					TagId = tag.Id
				});
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IGas Clone(string newName)
	{
		return new Gas(this, newName);
	}

	public override string FrameworkItemType => "Gas";
	protected override string MaterialNoun => "gas";

	#region Implementation of IMaterial

	public override MaterialType MaterialType => MaterialType.Gas;

	#endregion

	#region Implementation of IGas

	private long? _countsAsId;
	private IGas _countsAs;

	public IGas CountsAsGas
	{
		get
		{
			if (_countsAs is null && _countsAsId is not null)
			{
				_countsAs = Gameworld.Gases.Get(_countsAsId.Value);
			}

			return _countsAs;
		}
	}

	public bool GasCountAs(IGas otherGas)
	{
		return CountsAsGas == otherGas || (CountsAsGas?.GasCountAs(otherGas) ?? false);
	}

	public override bool CountsAs(IFluid other)
	{
		if (other == null)
		{
			return false;
		}

		if (other == this)
		{
			return true;
		}

		var otherGas = other as IGas;
		if (otherGas is null)
		{
			return false;
		}

		if (CountsAsGas is null)
		{
			return false;
		}

		return CountsAsGas.CountsAs(other);
	}

	public override ItemQuality CountAsQuality(IFluid other)
	{
		if (other == null)
		{
			return ItemQuality.Terrible;
		}

		if (other == this)
		{
			return ItemQuality.Legendary;
		}

		var otherGas = other as IGas;
		if (otherGas is null)
		{
			return ItemQuality.Terrible;
		}

		if (otherGas == CountsAsGas)
		{
			return CountsAsQuality;
		}

		if (CountsAsGas is null)
		{
			return ItemQuality.Terrible;
		}

		return CountsAsGas.CountAsQuality(other);
	}

	#region Overrides of Fluid

	/// <inheritdoc />
	public override double CountsAsMultiplier(IFluid other)
	{
		if (other == null)
		{
			return 0.0;
		}

		if (other == this)
		{
			return 1.0;
		}

		var otherGas = other as IGas;
		if (otherGas is null)
		{
			return 0.0;
		}

		if (otherGas == CountsAsGas)
		{
			return (int)CountsAsQuality / 11.0;
		}

		if (CountsAsGas is null)
		{
			return 0.0;
		}

		return CountsAsGas.CountsAsMultiplier(other) * ((int)CountsAsGas.CountsAsQuality / 11.0);
	}

	#endregion

	public ItemQuality CountsAsQuality { get; set; }

	private long? _liquidFormId;
	private ILiquid _liquidForm;

	public ILiquid LiquidForm
	{
		get
		{
			if (_liquidForm is null && _liquidFormId is not null)
			{
				_liquidForm = Gameworld.Liquids.Get(_liquidFormId.Value);
			}

			return _liquidForm;
		}
	}

	public double CondensationTemperature { get; set; }

	#endregion

	#region IFutureProgVariable Implementation

	public override ProgVariableTypes Type => ProgVariableTypes.Gas;

	public override IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
		}

		return base.GetProperty(property);
	}

	private new static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		var dict = new Dictionary<string, ProgVariableTypes>(Material.DotReferenceHandler());
		return dict;
	}

	private new static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		var dict = new Dictionary<string, string>(Material.DotReferenceHelp());
		return dict;
	}

	public new static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Gas, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.Gases.Find(Id);
		dbitem.Organic = Organic;
		dbitem.Description = MaterialDescription;
		dbitem.SpecificHeatCapacity = SpecificHeatCapacity;
		dbitem.ElectricalConductivity = ElectricalConductivity;
		dbitem.ThermalConductivity = ThermalConductivity;
		dbitem.Density = Density;
		dbitem.CountAsId = _countsAsId;
		dbitem.CountsAsQuality = (int)CountsAsQuality;
		dbitem.BoilingPoint = CondensationTemperature;
		dbitem.PrecipitateId = _liquidFormId;
		dbitem.DrugId = Drug?.Id;
		dbitem.DrugGramsPerUnitVolume = DrugGramsPerUnitVolume;
		dbitem.Viscosity = Viscosity;
		dbitem.SmellIntensity = SmellIntensity;
		dbitem.SmellText = SmellText;
		dbitem.VagueSmellText = VagueSmellText;
		FMDB.Context.GasesTags.RemoveRange(dbitem.GasesTags);
		foreach (var tag in Tags)
		{
			dbitem.GasesTags.Add(new GasesTags
			{
				Gas = dbitem,
				TagId = tag.Id
			});
		}

		Changed = false;
	}

	#endregion

	#region Implementation of IEditableItem

	#region Overrides of Fluid

	/// <inheritdoc />
	protected override string HelpText => $@"{base.HelpText}
	#3countsas <gas>#0 - sets a gas that this counts as
	#3countsas none#0 - clears a counts-as gas
	#3quality <quality>#0 - sets the maximum quality of the gas when counting-as
	#3condensation <temp>|none#0 - sets or clears the temperature at which this gas becomes a liquid
	#3liquid <liquid>|none#0 - sets or clears the liquid form of this gas";

	#endregion

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "countsas":
			case "counts":
			case "count":
			case "countas":
				return BuildingCommandCountsAs(actor, command);
			case "countasquality":
			case "countsasquality":
			case "countquality":
			case "quality":
				return BuildingCommandCountsAsQuality(actor, command);
			case "liquidform":
			case "liquid":
				return BuildingCommandLiquidForm(actor, command);
			case "condensation":
			case "condensationtemperature":
			case "condensationtemp":
			case "boiling":
			case "boilingpoint":
			case "boilingtemperature":
			case "boilingtemp":
				return BuildingCommandCondensationTemperature(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandCondensationTemperature(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must enter a temperature.");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Temperature, actor,
				out var value))
		{
			actor.OutputHandler.Send($"That is not a valid temperature.");
			return false;
		}

		CondensationTemperature = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This material will now have a condensation temperature of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.Temperature, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandLiquidForm(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a liquid form of this gas, or use {"none".ColourCommand()} to clear one.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_liquidForm = null;
			_liquidFormId = null;
			Changed = true;
			actor.OutputHandler.Send($"This gas will no longer have a liquid form.");
			return true;
		}

		var liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
		if (liquid is null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return false;
		}

		_liquidForm = liquid;
		_liquidFormId = liquid.Id;
		Changed = true;
		actor.OutputHandler.Send(
			$"This gas will now have {liquid.Name.Colour(liquid.DisplayColour)} as its liquid form.");
		return true;
	}

	private bool BuildingCommandCountsAsQuality(ICharacter actor, StringStack command)
	{
		if (CountsAsGas is null)
		{
			actor.OutputHandler.Send("You must set a counts as gas before you set a counts as quality.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What is the maximum quality that this gas should be considered when being substituted for {CountsAsGas.Name.Colour(CountsAsGas.DisplayColour)}?\nThe valid options are {Enum.GetValues<ItemQuality>().Select(x => x.Describe().ColourValue()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<ItemQuality>(out var quality))
		{
			actor.OutputHandler.Send(
				$"That is not a valid quality.\nThe valid options are {Enum.GetValues<ItemQuality>().Select(x => x.Describe().ColourValue()).ListToString()}.");
			return false;
		}

		CountsAsQuality = quality;
		Changed = true;
		actor.OutputHandler.Send(
			$"This gas will now be a maximum quality of {quality.Describe().ColourValue()} when substituting for {CountsAsGas.Name.Colour(CountsAsGas.DisplayColour)}.");
		return true;
	}

	private bool BuildingCommandCountsAs(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a gas that this gas can count as, or use {"none".ColourCommand()} to clear one.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_countsAs = null;
			_countsAsId = null;
			Changed = true;
			actor.OutputHandler.Send($"This material will no longer count as any other gas.");
			return true;
		}

		var gas = Gameworld.Gases.GetByIdOrName(command.SafeRemainingArgument);
		if (gas is null)
		{
			actor.OutputHandler.Send("There is no such gas.");
			return false;
		}

		_countsAs = gas;
		_countsAsId = gas.Id;
		CountsAsQuality = ItemQuality.Standard;
		Changed = true;
		actor.OutputHandler.Send($"This material will now count as {gas.Name.Colour(gas.DisplayColour)}.");
		return true;
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append($"Gas #{Id.ToString("N0", actor)}".Colour(Telnet.BoldYellow));
		sb.AppendLine($" - {Name.Colour(DisplayColour)}");
		sb.AppendLineFormat(actor, "Description: {0}", MaterialDescription.Colour(DisplayColour));
		sb.AppendLine();
		sb.AppendLine("Material Properties".GetLineWithTitle(actor.LineFormatLength, actor.Account.UseUnicode,
			Telnet.Cyan,
			Telnet.BoldYellow));
		sb.AppendLine();
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
		{
			$"Density: {$"{Density.ToString("N9", actor)} kg/m3".ColourValue()}",
			$"Viscosity: {Viscosity.ToString("N2", actor).ColourValue()}",
			$"Organic: {Organic.ToColouredString()}"
		});
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
		{
			$"Condensation Temp: {Gameworld.UnitManager.DescribeDecimal(CondensationTemperature, UnitType.Temperature, actor).ColourValue()}",
			$"Liquid Form: {LiquidForm?.Name.Colour(LiquidForm.DisplayColour) ?? "None".Colour(Telnet.Red)}",
			$"Display Colour: {DisplayColour.Name.Colour(DisplayColour)}"
		});

		sb.AppendLine(
			$"Thermal Conductivity: {$"{ThermalConductivity.ToString("N9", actor)} Watts/Kelvin".ColourValue()}");
		sb.AppendLine(
			$"Specific Heat Capacity: {$"{SpecificHeatCapacity.ToString("N9", actor)} J/°C/kg".ColourValue()}");
		sb.AppendLine(
			$"Electrical Conductivity: {$"{ElectricalConductivity.ToString("N9", actor)} Siemens".ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Fluid Properties".GetLineWithTitle(actor.LineFormatLength, actor.Account.UseUnicode, Telnet.Cyan,
			Telnet.BoldYellow));
		sb.AppendLine();
		sb.AppendLineFormat(actor, "Smell Intensity: {0}", SmellIntensity.ToString("N1", actor).ColourCommand());
		sb.AppendLineFormat(actor, "Smell Text: {0}", SmellText.ColourCommand());
		sb.AppendLineFormat(actor, "Vague Smell Text: {0}", VagueSmellText.ColourCommand());
		sb.AppendLine(
			$"Counts As: {(CountsAsGas != null ? $"{CountsAsGas.Name.Colour(CountsAsGas.DisplayColour)} @ max quality {CountsAsQuality.Describe().Colour(Telnet.Green)}" : "None".Colour(Telnet.Red))}");
		sb.AppendLine(
			$"Drug: {(Drug is not null ? $"{Drug.Name.ColourValue()} @ {DrugGramsPerUnitVolume.ToString("N3", actor).ColourValue()}g/L" : "None".Colour(Telnet.Red))}");
		sb.AppendLine();
		sb.AppendLine("Tags".GetLineWithTitle(actor.LineFormatLength, actor.Account.UseUnicode, Telnet.Cyan,
			Telnet.BoldYellow));
		sb.AppendLine();
		if (!Tags.Any())
		{
			sb.AppendLine("\tNone".Colour(Telnet.Red));
		}
		else
		{
			foreach (var tag in Tags)
			{
				sb.AppendLine($"\t{tag.FullName.ColourName()}");
			}
		}

		return sb.ToString();
	}

	#endregion
}