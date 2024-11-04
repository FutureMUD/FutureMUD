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

public class Liquid : Fluid, ILiquid
{
	private ILiquid _countsAs;

	private long? _countsAsId;
	private ISolid _driedResidue;

	private long? _driedResidueId;
	private ILiquid _solvent;

	private long? _solventId;
	private long? _gasFormId;
	private IGas _gasForm;

	public ILiquid Clone(string newName)
	{
		return new Liquid(this, newName);
	}

	public Liquid(Liquid rhs, string newName) : base(rhs, newName, MaterialBehaviourType.Liquid)
	{
		_countsAsId = rhs._countsAsId;
		_driedResidueId = rhs._driedResidueId;
		_solventId = rhs._solventId;
		_gasFormId = rhs._gasFormId;
		TasteIntensity = rhs.TasteIntensity;
		TasteText = rhs.TasteText;
		VagueTasteText = rhs.VagueTasteText;
		AlcoholLitresPerLitre = rhs.AlcoholLitresPerLitre;
		WaterLitresPerLitre = rhs.WaterLitresPerLitre;
		CaloriesPerLitre = rhs.CaloriesPerLitre;
		FoodSatiatedHoursPerLitre = rhs.FoodSatiatedHoursPerLitre;
		DrinkSatiatedHoursPerLitre = rhs.DrinkSatiatedHoursPerLitre;
		DampDescription = rhs.DampDescription;
		WetDescription = rhs.WetDescription;
		DrenchedDescription = rhs.DrenchedDescription;
		DampShortDescription = rhs.DampShortDescription;
		WetShortDescription = rhs.WetShortDescription;
		DrenchedShortDescription = rhs.DrenchedShortDescription;
		InjectionConsequence = rhs.InjectionConsequence;
		SolventVolumeRatio = rhs.SolventVolumeRatio;
		FreezingPoint = rhs.FreezingPoint;
		BoilingPoint = rhs.BoilingPoint;
		ResidueVolumePercentage = rhs.ResidueVolumePercentage;
		RelativeEnthalpy = rhs.RelativeEnthalpy;

		using (new FMDB())
		{
			var dbitem = new Models.Liquid
			{
				Name = Name,
				Description = MaterialDescription,
				LongDescription = Description,
				TasteText = TasteText,
				VagueTasteText = VagueTasteText,
				SmellText = SmellText,
				VagueSmellText = VagueSmellText,
				TasteIntensity = TasteIntensity,
				SmellIntensity = SmellIntensity,
				AlcoholLitresPerLitre = AlcoholLitresPerLitre,
				WaterLitresPerLitre = WaterLitresPerLitre,
				FoodSatiatedHoursPerLitre = FoodSatiatedHoursPerLitre,
				DrinkSatiatedHoursPerLitre = DrinkSatiatedHoursPerLitre,
				CaloriesPerLitre = CaloriesPerLitre,
				Viscosity = Viscosity,
				Density = Density,
				Organic = Organic,
				ThermalConductivity = ThermalConductivity,
				ElectricalConductivity = ElectricalConductivity,
				SpecificHeatCapacity = SpecificHeatCapacity,
				IgnitionPoint = IgnitionPoint,
				FreezingPoint = FreezingPoint,
				BoilingPoint = BoilingPoint,
				DraughtProgId = DraughtProg?.Id,
				SolventId = _solventId,
				CountAsId = _countsAsId,
				CountAsQuality = (int)CountsAsQuality,
				DisplayColour = DisplayColour.Name,
				DampDescription = DampDescription,
				WetDescription = WetDescription,
				DrenchedDescription = DrenchedDescription,
				DampShortDescription = DampShortDescription,
				WetShortDescription = WetShortDescription,
				DrenchedShortDescription = DrenchedShortDescription,
				SolventVolumeRatio = SolventVolumeRatio,
				DriedResidueId = _driedResidueId,
				DrugId = Drug?.Id,
				DrugGramsPerUnitVolume = DrugGramsPerUnitVolume,
				InjectionConsequence = (int)InjectionConsequence,
				ResidueVolumePercentage = ResidueVolumePercentage,
				RelativeEnthalpy = RelativeEnthalpy,
				GasFormId = _gasFormId
			};
			FMDB.Context.Liquids.Add(dbitem);
			foreach (var tag in Tags)
			{
				dbitem.LiquidsTags.Add(new LiquidsTags
				{
					Liquid = dbitem,
					TagId = tag.Id
				});
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Liquid(string name, IFuturemud gameworld) : base(name, MaterialBehaviourType.Liquid, gameworld)
	{
		Description = "a clear liquid";
		TasteText = "It doesn't really taste like anything.";
		VagueTasteText = "It doesn't really taste like anything.";
		TasteIntensity = 100;
		SolventVolumeRatio = 1.0;
		DampDescription = "It is damp";
		WetDescription = "It is wet";
		DrenchedDescription = "It is drenched";
		DampShortDescription = "(damp)";
		WetShortDescription = "(wet)";
		DrenchedShortDescription = "(drenched)";
		InjectionConsequence = LiquidInjectionConsequence.Harmful;
		ResidueVolumePercentage = 0.05;
		RelativeEnthalpy = 1.0;

		using (new FMDB())
		{
			var dbitem = new Models.Liquid
			{
				Name = Name,
				Description = MaterialDescription,
				LongDescription = Description,
				TasteText = TasteText,
				VagueTasteText = VagueTasteText,
				SmellText = SmellText,
				VagueSmellText = VagueSmellText,
				TasteIntensity = TasteIntensity,
				SmellIntensity = SmellIntensity,
				AlcoholLitresPerLitre = AlcoholLitresPerLitre,
				WaterLitresPerLitre = WaterLitresPerLitre,
				FoodSatiatedHoursPerLitre = FoodSatiatedHoursPerLitre,
				DrinkSatiatedHoursPerLitre = DrinkSatiatedHoursPerLitre,
				CaloriesPerLitre = CaloriesPerLitre,
				Viscosity = Viscosity,
				Density = Density,
				Organic = Organic,
				ThermalConductivity = ThermalConductivity,
				ElectricalConductivity = ElectricalConductivity,
				SpecificHeatCapacity = SpecificHeatCapacity,
				IgnitionPoint = IgnitionPoint,
				FreezingPoint = FreezingPoint,
				BoilingPoint = BoilingPoint,
				DraughtProgId = DraughtProg?.Id,
				SolventId = _solventId,
				CountAsId = _countsAsId,
				CountAsQuality = (int)CountsAsQuality,
				DisplayColour = DisplayColour?.Name,
				DampDescription = DampDescription,
				WetDescription = WetDescription,
				DrenchedDescription = DrenchedDescription,
				DampShortDescription = DampShortDescription,
				WetShortDescription = WetShortDescription,
				DrenchedShortDescription = DrenchedShortDescription,
				SolventVolumeRatio = SolventVolumeRatio,
				DriedResidueId = _driedResidueId,
				DrugId = Drug?.Id,
				DrugGramsPerUnitVolume = DrugGramsPerUnitVolume,
				InjectionConsequence = (int)InjectionConsequence,
				ResidueVolumePercentage = ResidueVolumePercentage,
				RelativeEnthalpy = RelativeEnthalpy,
				GasFormId = _gasFormId
			};
			FMDB.Context.Liquids.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Liquid(MudSharp.Models.Liquid liquid, IFuturemud gameworld) : base(liquid, gameworld)
	{
		Description = liquid.LongDescription;
		TasteText = liquid.TasteText;
		VagueTasteText = liquid.VagueTasteText;
		TasteIntensity = liquid.TasteIntensity;
		SmellText = liquid.SmellText;
		VagueSmellText = liquid.VagueSmellText;
		SmellIntensity = liquid.SmellIntensity;
		AlcoholLitresPerLitre = liquid.AlcoholLitresPerLitre;
		WaterLitresPerLitre = liquid.WaterLitresPerLitre;
		FoodSatiatedHoursPerLitre = liquid.FoodSatiatedHoursPerLitre;
		DrinkSatiatedHoursPerLitre = liquid.DrinkSatiatedHoursPerLitre;
		CaloriesPerLitre = liquid.CaloriesPerLitre;
		BoilingPoint = liquid.BoilingPoint;
		FreezingPoint = liquid.FreezingPoint;
		Viscosity = liquid.Viscosity;
		IgnitionPoint = liquid.IgnitionPoint;
		_solventId = liquid.SolventId;
		_countsAsId = liquid.CountAsId;
		CountsAsQuality = (ItemQuality)liquid.CountAsQuality;
		DisplayColour = Telnet.GetColour(liquid.DisplayColour);
		DampDescription = liquid.DampDescription;
		DampShortDescription = liquid.DampShortDescription;
		WetDescription = liquid.WetDescription;
		WetShortDescription = liquid.WetShortDescription;
		DrenchedDescription = liquid.DrenchedDescription;
		DrenchedShortDescription = liquid.DrenchedShortDescription;
		SolventVolumeRatio = liquid.SolventVolumeRatio;
		_driedResidueId = liquid.DriedResidueId;
		InjectionConsequence = (LiquidInjectionConsequence)liquid.InjectionConsequence;
		Drug = Gameworld.Drugs.Get(liquid.DrugId ?? 0);
		DrugGramsPerUnitVolume = liquid.DrugGramsPerUnitVolume;
		ResidueVolumePercentage = liquid.ResidueVolumePercentage;
		RelativeEnthalpy = liquid.RelativeEnthalpy;
		_gasFormId = liquid.GasFormId;

		if (liquid.DraughtProgId.HasValue)
		{
			DraughtProg = gameworld.FutureProgs.Get(liquid.DraughtProgId.Value);
		}
	}

	public double TasteIntensity { get; set; }

	public string TasteText { get; set; }

	public string VagueTasteText { get; set; }

	/// <summary>
	/// The description used when examining a liquid closely. Contrast with MaterialDescription which is used more like a short description.
	/// </summary>
	public string Description { get; set; }

	public double SolventVolumeRatio { get; set; }

	public ILiquid Solvent
	{
		get
		{
			if (_solvent is null && _solventId is not null)
			{
				_solvent = Gameworld.Liquids.Get(_solventId.Value);
			}

			return _solvent;
		}
		set
		{
			_solvent = value;
			_solventId = _solvent?.Id;
		}
	}

	public LiquidInjectionConsequence InjectionConsequence { get; set; }

	public double AlcoholLitresPerLitre { get; set; }

	public double WaterLitresPerLitre { get; set; }

	public double FoodSatiatedHoursPerLitre { get; set; }

	public double DrinkSatiatedHoursPerLitre { get; set; }

	public double CaloriesPerLitre { get; set; }

	public ISolid DriedResidue
	{
		get
		{
			if (_driedResidue is null && _driedResidueId is not null)
			{
				_driedResidue = Gameworld.Materials.Get(_driedResidueId.Value);
			}

			return _driedResidue;
		}
	}

	public bool LeaveResiduesInRooms { get; set; }

	public double ResidueVolumePercentage { get; set; }

	public double RelativeEnthalpy { get; set; }

	public ILiquid CountsAs
	{
		get
		{
			if (_countsAs is null && _countsAsId is not null)
			{
				_countsAs = Gameworld.Liquids.Get(_countsAsId.Value);
			}

			return _countsAs;
		}
		set
		{
			_countsAs = value;
			_countsAsId = value?.Id;
		}
	}

	public IGas GasForm
	{
		get
		{
			if (_gasForm is null && _gasFormId is not null)
			{
				_gasForm = Gameworld.Gases.Get(_gasFormId.Value);
			}

			return _gasForm;
		}
	}

	public bool LiquidCountsAs(ILiquid otherLiquid)
	{
		return otherLiquid != null && (otherLiquid == this || CountsAs == otherLiquid ||
		                               (CountsAs?.LiquidCountsAs(otherLiquid) ?? false));
	}

	public ItemQuality LiquidCountsAsQuality(ILiquid otherLiquid)
	{
		return otherLiquid == this ? ItemQuality.Legendary : CountsAsQuality;
	}

	public ItemQuality CountsAsQuality { get; set; }
	public string DampDescription { get; set; }
	public string WetDescription { get; set; }
	public string DrenchedDescription { get; set; }
	public string DampShortDescription { get; set; }
	public string WetShortDescription { get; set; }
	public string DrenchedShortDescription { get; set; }
	public double? IgnitionPoint { get; set; }

	public double? FreezingPoint { get; set; }

	public double? BoilingPoint { get; set; }

	public override string FrameworkItemType => "Liquid";
	protected override string MaterialNoun => "liquid";

	/// <inheritdoc />
	public override MaterialType MaterialType => MaterialType.Liquid;

	public IFutureProg DraughtProg { get; set; }

	#region IFutureProgVariable Implementation

	public override ProgVariableTypes Type => ProgVariableTypes.Liquid;


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
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Liquid, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion

	#region Overrides of Fluid

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append($"Liquid #{Id.ToString("N0", actor)}".Colour(Telnet.BoldYellow));
		sb.AppendLine($" - {Name.Colour(DisplayColour)}");
		sb.AppendLineFormat(actor, "Description: {0}", MaterialDescription.Colour(DisplayColour));
		sb.AppendLineFormat(actor, "Long Description: {0}", Description.Colour(DisplayColour));
		sb.AppendLine();
		sb.AppendLine("Material Properties".GetLineWithTitle(actor.LineFormatLength, actor.Account.UseUnicode,
			Telnet.Cyan,
			Telnet.BoldYellow));
		sb.AppendLine();
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
		{
			$"Density: {$"{Density.ToString("N5", actor)} kg/m3".ColourValue()}",
			$"Viscosity: {Viscosity.ToString("N2").ColourValue()}",
			$"Organic: {Organic.ToColouredString()}"
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

		sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
		{
			$"Boiling Temp: {(BoilingPoint is not null ? Gameworld.UnitManager.DescribeDecimal(BoilingPoint.Value, UnitType.Temperature, actor).ColourValue() : "N/A".Colour(Telnet.Yellow))}",
			$"Gas Form: {GasForm?.Name.Colour(GasForm.DisplayColour) ?? "None".Colour(Telnet.Red)}",
			$"Display Colour: {DisplayColour.Name.Colour(DisplayColour)}"
		});

		sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
		{
			$"Freezing Temp: {(FreezingPoint is not null ? Gameworld.UnitManager.DescribeDecimal(FreezingPoint.Value, UnitType.Temperature, actor).ColourValue() : "N/A".Colour(Telnet.Yellow))}",
			$"Relative Enthalpy: {RelativeEnthalpy.ToString("N3", actor).ColourValue()}",
			""
		});
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
		{
			$"Injection Consequence: {InjectionConsequence.DescribeEnum(true).ColourCommand()}",
			$"Solvent: {Solvent?.Name.Colour(Solvent.DisplayColour) ?? "None".Colour(Telnet.Red)}",
			$"Solvent Ratio: {SolventVolumeRatio.ToString("P3", actor).ColourValue()}"
		});
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
		{
			$"Solid Form: {DriedResidue?.Name.Colour(DriedResidue.ResidueColour) ?? "None".Colour(Telnet.Red)}",
			$"Residue Volume: {ResidueVolumePercentage.ToString("P3", actor).ColourValue()}",
			$"Residue In Rooms: {LeaveResiduesInRooms.ToColouredString()}"
		});
		sb.AppendLine();
		sb.AppendLineFormat(actor, "Taste Intensity: {0}", TasteIntensity.ToString("N1", actor).ColourCommand());
		sb.AppendLineFormat(actor, "Taste Text: {0}", TasteText.ColourCommand());
		sb.AppendLineFormat(actor, "Vague Taste Text: {0}", VagueTasteText.ColourCommand());
		sb.AppendLineFormat(actor, "Smell Intensity: {0}", SmellIntensity.ToString("N1", actor).ColourCommand());
		sb.AppendLineFormat(actor, "Smell Text: {0}", SmellText.ColourCommand());
		sb.AppendLineFormat(actor, "Vague Smell Text: {0}", VagueSmellText.ColourCommand());
		sb.AppendLine(
			$"Counts As: {(CountsAs != null ? $"{CountsAs.Name.Colour(CountsAs.DisplayColour)} @ max quality {CountsAsQuality.Describe().Colour(Telnet.Green)}" : "None".Colour(Telnet.Red))}");
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

	/// <inheritdoc />
	protected override string HelpText => $@"{base.HelpText}
	#3taste <intensity> <taste> [<vague taste>]#0 - sets the taste
	#3ldesc <desc>#0 - sets the more detailed description when looked at
	#3alcohol <litres per litre>#0 - how many litres of pure alcohol per litre of liquid
	#3thirst <hours>#0 - how many hours of thirst quenched per litre drunk
	#3hunger <hours>#0 - how many hours of hunger quenched per litre drunk
	#3water <litres per litre>#0 - how many litres of hydrating water per litre of liquid
	#3calories <calories per litre>#0 - how many calories per litre of liquid
	#3prog <which>#0 - sets a prog to be executed when the liquid is drunk
	#3prog none#0 - clears the draught prog
	#3solvent <liquid>#0 - sets a solvent required for cleaning this liquid off things
	#3solvent none#0 - no solvent required for cleaning
	#3solventratio <percentage>#0 - sets the volume of solvent to contaminant required
	#3residue <solid>#0 - sets a material to leave behind as a residue when dry
	#3residue none#0 - dry clean, leave no residue
	#3residueamount <percentage>#0 - percentage of weight of liquid that is residue
	#3residueroom#0 - toggles leaving residue in rooms
	#3relativeenthalpy <percentage>#0 - sets the rate of evaporation relative to water
	#3countsas <liquid>#0 - sets another liquid that this one counts as
	#3countsas none#0 - this liquid counts as no other liquid
	#3countquality <quality>#0 - sets the maximum quality for this when counting as
	#3freeze <temp>#0 - sets the freeze temperature of this liquid
	#3boil <temp>#0 - sets the boil temperature of this liquid
	#3ignite <temp>#0 - sets the ignite temperature of this liquid
	#3ignite none#0 - clears the ignite temperature of this liquid
	#3gas <which>#0 - sets the gas form of this liquid
	#3gas none#0 - clears the gas form of this liquid";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "taste":
				return BuildingCommandTaste(actor, command);
			case "longdescription":
			case "ldesc":
			case "longdesc":
			case "ldescription":
			case "ld":
				return BuildingCommandLongDescription(actor, command);
			case "alcohol":
				return BuildingCommandAlcohol(actor, command);
			case "water":
				return BuildingCommandWater(actor, command);
			case "thirst":
				return BuildingCommandThirst(actor, command);
			case "hunger":
				return BuildingCommandHunger(actor, command);
			case "calories":
				return BuildingCommandCalories(actor, command);
			case "prog":
			case "draught":
			case "draughtprog":
				return BuildingCommandDraughtProg(actor, command);
			case "solvent":
				return BuildingCommandSolvent(actor, command);
			case "solventratio":
			case "solventmultiplier":
			case "ratio":
				return BuildingCommandSolventRatio(actor, command);
			case "residue":
				return BuildingCommandResidue(actor, command);
			case "residuevolume":
			case "residuepercentage":
			case "residueamount":
			case "residuevolumepercentage":
			case "residueratio":
				return BuildingCommandResidueVolume(actor, command);
			case "residueroom":
				return BuildingCommandResidueRoom(actor);
			case "enthalpy":
			case "relativeenthalpy":
			case "relenthalpy":
			case "evaporation":
			case "evaporationrate":
			case "evaprate":
				return BuildingCommandRelativeEnthalpy(actor, command);
			case "countsas":
			case "countas":
			case "counts":
			case "count":
				return BuildingCommandCountsAs(actor, command);
			case "countasquality":
			case "countsasquality":
			case "countquality":
			case "quality":
				return BuildingCommandCountsAsQuality(actor, command);
			case "freezing":
			case "freeze":
			case "freezingpoint":
			case "freezepoint":
				return BuildingCommandFreezingPoint(actor, command);
			case "ignitionpoint":
			case "ignition":
			case "ignite":
				return BuildingCommandIgnitionPoint(actor, command);
			case "boilingpoint":
			case "boiling":
			case "boil":
				return BuildingCommandBoilingPoint(actor, command);
			case "gas":
				return BuildingCommandGas(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandResidueRoom(ICharacter actor)
	{
		LeaveResiduesInRooms = !LeaveResiduesInRooms;
		Changed = true;
		actor.OutputHandler.Send($"This liquid will {LeaveResiduesInRooms.NowNoLonger()} leave residues on room contamination.");
		return true;
	}

	private bool BuildingCommandGas(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a gas form of this liquid or use #1none#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_gasForm = null;
			_gasFormId = null;
			Changed = true;
			actor.OutputHandler.Send($"This liquid no longer has a gaseous form.");
			return true;
		}

		var gas = Gameworld.Gases.GetByIdOrName(command.SafeRemainingArgument);
		if (gas is null)
		{
			actor.OutputHandler.Send($"There is no such gas identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		_gasForm = gas;
		_gasFormId = gas.Id;
		Changed = true;
		actor.OutputHandler.Send($"The gaseous form of this liquid will now be {gas.Name.Colour(gas.DisplayColour)}.");
		return true;
	}

	private bool BuildingCommandRelativeEnthalpy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the relative enthalpy (evaporation rate) of this liquid relative to water?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		RelativeEnthalpy = value;
		Changed = true;
		actor.OutputHandler.Send($"This liquid now dries at {value.ToString("P3", actor).ColourValue()} the rate of water.");
		return true;
	}

	private bool BuildingCommandBoilingPoint(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must enter a temperature.");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Temperature,
			    out var value))
		{
			actor.OutputHandler.Send($"That is not a valid temperature.");
			return false;
		}

		BoilingPoint = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now have a boiling temperature of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.Temperature, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandIgnitionPoint(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must enter a temperature or {"none".ColourCommand()} to clear.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			IgnitionPoint = null;
			Changed = true;
			actor.OutputHandler.Send($"This liquid will no longer ignite at any temperature.");
			return true;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Temperature,
			    out var value))
		{
			actor.OutputHandler.Send($"That is not a valid temperature.");
			return false;
		}

		IgnitionPoint = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now have a ignition temperature of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.Temperature, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandFreezingPoint(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must enter a temperature.");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Temperature,
			    out var value))
		{
			actor.OutputHandler.Send($"That is not a valid temperature.");
			return false;
		}

		FreezingPoint = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now have a freezing temperature of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.Temperature, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandCountsAs(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a liquid that this liquid can count as, or use {"none".ColourCommand()} to clear one.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_countsAs = null;
			_countsAsId = null;
			Changed = true;
			actor.OutputHandler.Send($"This liquid will no longer count as any other liquid.");
			return true;
		}

		var liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
		if (liquid is null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return false;
		}

		_countsAs = liquid;
		_countsAsId = liquid.Id;
		CountsAsQuality = ItemQuality.Standard;
		Changed = true;
		actor.OutputHandler.Send($"This liquid will now count as {liquid.Name.Colour(liquid.DisplayColour)}.");
		return true;
	}

	private bool BuildingCommandCountsAsQuality(ICharacter actor, StringStack command)
	{
		if (CountsAs is null)
		{
			actor.OutputHandler.Send("You must set a counts as liquid before you set a counts as quality.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What is the maximum quality that this liquid should be considered when being substituted for {CountsAs.Name.Colour(CountsAs.DisplayColour)}?\nThe valid options are {Enum.GetValues<ItemQuality>().Select(x => x.Describe().ColourValue()).ListToString()}.");
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
			$"This liquid will now be a maximum quality of {quality.Describe().ColourValue()} when substituting for {CountsAs.Name.Colour(CountsAs.DisplayColour)}.");
		return true;
	}

	private bool BuildingCommandResidue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a solid material to be the residue for this liquid when it dries, or use {"none".ColourCommand()} to make it dry clean.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_driedResidueId = null;
			_driedResidue = null;
			Changed = true;
			actor.OutputHandler.Send($"This liquid will now dry clean and leave no residue.");
			return true;
		}

		var material = Gameworld.Materials.GetByIdOrName(command.SafeRemainingArgument);
		if (material is null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return false;
		}

		_driedResidueId = material.Id;
		_driedResidue = material;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now leave behind a residue of {material.Name.Colour(material.ResidueColour)} when it dries.");
		return true;
	}

	private bool BuildingCommandResidueVolume(ICharacter actor, StringStack command)
	{
		if (_driedResidue is null)
		{
			actor.OutputHandler.Send(
				"You must first give this liquid a residue material before you can set its volume.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage of the liquid should be left behind as a residue?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		ResidueVolumePercentage = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"When this liquid dries it will now leave {value.ToString("P3", actor).ColourValue()} of its weight as a residue of {DriedResidue.Name.Colour(DriedResidue.ResidueColour)}.");
		return true;
	}


	private bool BuildingCommandSolvent(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a liquid that is required to clean contamination of this liquid, or use {"none".ColourCommand()} to clear one.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_solvent = null;
			_solventId = null;
			Changed = true;
			actor.OutputHandler.Send($"This liquid will no longer require any other liquid to clean it.");
			return true;
		}

		var liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
		if (liquid is null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return false;
		}

		_solvent = liquid;
		_solventId = liquid.Id;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now require {liquid.Name.Colour(liquid.DisplayColour)} to clean it.");
		return true;
	}

	private bool BuildingCommandSolventRatio(ICharacter actor, StringStack command)
	{
		if (_solvent is null)
		{
			actor.OutputHandler.Send(
				$"This liquid does not currently require a solvent. You must set a solvent first.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What percentage of the volume of the contaminant should the required volume of the solvent be?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.OutputHandler.Send($"The solvent volume ratio must be greater than zero.");
			return false;
		}

		SolventVolumeRatio = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now require {value.ToString("P2", actor).ColourValue()} of the volume of the contaminant to ");
		return true;
	}

	private bool BuildingCommandDraughtProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a prog to be executed when someone drinks this liquid, or use {"none".ColourCommand()} to remove one.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			DraughtProg = null;
			Changed = true;
			actor.OutputHandler.Send("This liquid will no longer cause a prog to be executed when it is drunk.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Void, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Item,
				ProgVariableTypes.Number
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		DraughtProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now execute the prog {prog.MXPClickableFunctionName()} when it is drunk.");
		return true;
	}

	private bool BuildingCommandCalories(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		CaloriesPerLitre = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now be worth {CaloriesPerLitre.ToString("N3", actor).ColourValue()} calories per litre of liquid consumed.");
		return true;
	}

	private bool BuildingCommandHunger(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		FoodSatiatedHoursPerLitre = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now satiate hunger for {FoodSatiatedHoursPerLitre.ToString("N3", actor).ColourValue()} hours per litre of liquid consumed.");
		return true;
	}

	private bool BuildingCommandThirst(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		DrinkSatiatedHoursPerLitre = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now quench thirst for {DrinkSatiatedHoursPerLitre.ToString("N3", actor).ColourValue()} hours per litre of liquid consumed.");
		return true;
	}

	private bool BuildingCommandWater(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		WaterLitresPerLitre = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now be worth {WaterLitresPerLitre.ToString("N3", actor).ColourValue()} litres of water hydration per litre of liquid consumed.");
		return true;
	}

	private bool BuildingCommandAlcohol(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		AlcoholLitresPerLitre = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This liquid will now have {AlcoholLitresPerLitre.ToString("N3", actor).ColourValue()} litres of pure alcohol per litre of liquid consumed, giving it an alcohol content of {(AlcoholLitresPerLitre / 1.0).ToString("P3", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandLongDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the long description to?");
			return false;
		}

		Description = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the long description of this {MaterialNoun} to {MaterialDescription.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandTaste(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the intensity of this taste be, relative to water?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var intensity) || intensity < 0)
		{
			actor.OutputHandler.Send("The taste intensity must be a valid positive value.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should the taste of this {MaterialNoun} be?");
			return false;
		}

		var taste = command.PopSpeech().ProperSentences();
		var vague = taste;
		if (!command.IsFinished)
		{
			vague = command.SafeRemainingArgument.ProperSentences();
		}

		TasteIntensity = intensity;
		TasteText = taste;
		VagueTasteText = vague;
		Changed = true;
		actor.OutputHandler.Send($@"This {MaterialNoun} now has the following taste parameters:

	Intensity: {intensity.ToString("N2", actor).ColourValue()}
	Taste Text: {TasteText.Colour(DisplayColour)}
	Vague Smell: {VagueTasteText.Colour(DisplayColour)}");
		return true;
	}

	#endregion

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.Liquids.Find(Id);
		dbitem.Organic = Organic;
		dbitem.Description = MaterialDescription;
		dbitem.LongDescription = Description;
		dbitem.SpecificHeatCapacity = SpecificHeatCapacity;
		dbitem.ElectricalConductivity = ElectricalConductivity;
		dbitem.ThermalConductivity = ThermalConductivity;
		dbitem.Density = Density;
		dbitem.CountAsId = _countsAsId;
		dbitem.CountAsQuality = (int)CountsAsQuality;
		dbitem.BoilingPoint = BoilingPoint;
		dbitem.Viscosity = Viscosity;
		dbitem.SmellIntensity = SmellIntensity;
		dbitem.SmellText = SmellText;
		dbitem.VagueSmellText = VagueSmellText;
		//dbitem.FreezingPoint = FreezingPoint;
		dbitem.DrugId = Drug?.Id;
		dbitem.DrugGramsPerUnitVolume = DrugGramsPerUnitVolume;
		dbitem.SolventId = _solventId;
		dbitem.SolventVolumeRatio = SolventVolumeRatio;
		dbitem.DraughtProgId = DraughtProg?.Id;
		dbitem.DriedResidueId = DriedResidue?.Id;
		dbitem.ResidueVolumePercentage = ResidueVolumePercentage;
		dbitem.RelativeEnthalpy = RelativeEnthalpy;
		dbitem.AlcoholLitresPerLitre = AlcoholLitresPerLitre;
		dbitem.WaterLitresPerLitre = WaterLitresPerLitre;
		dbitem.FoodSatiatedHoursPerLitre = FoodSatiatedHoursPerLitre;
		dbitem.DrinkSatiatedHoursPerLitre = DrinkSatiatedHoursPerLitre;
		dbitem.CaloriesPerLitre = CaloriesPerLitre;
		dbitem.TasteIntensity = TasteIntensity;
		dbitem.TasteText = TasteText;
		dbitem.VagueTasteText = VagueTasteText;
		dbitem.InjectionConsequence = (int)InjectionConsequence;
		dbitem.GasFormId = _gasFormId;
		FMDB.Context.LiquidsTags.RemoveRange(dbitem.LiquidsTags);
		foreach (var tag in Tags)
		{
			dbitem.LiquidsTags.Add(new LiquidsTags
			{
				Liquid = dbitem,
				TagId = tag.Id
			});
		}

		Changed = false;
	}

	#endregion
}