using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.Form.Material;

public class Solid : Material, ISolid
{
    public Solid(MudSharp.Models.Material material, IFuturemud gameworld)
        : base(material, gameworld)
    {
        _aliases.AddRange(material.MaterialAliases
            .Select(x => x.Alias)
            .Where(x => !string.IsNullOrWhiteSpace(x)));
        ImpactFracture = material.ImpactFracture ?? 0;
        ImpactYield = material.ImpactYield ?? 0;
        ImpactStrainAtYield = material.ImpactStrainAtYield ?? 100;
        ShearFracture = material.ShearFracture ?? 0;
        ShearYield = material.ShearYield ?? 0;
        ShearStrainAtYield = material.ShearStrainAtYield ?? 100;
        HeatDamagePoint = material.HeatDamagePoint;
        YoungsModulus = material.YoungsModulus ?? 0;
        IgnitionPoint = material.IgnitionPoint;
        MeltingPoint = material.MeltingPoint;
        _solventId = material.SolventId;
        SolventRatio = material.SolventVolumeRatio;
        ResidueSdesc = material.ResidueSdesc;
        ResidueDesc = material.ResidueDesc;
        ResidueColour = Telnet.GetColour(material.ResidueColour);
        Absorbency = material.Absorbency;
        _liquidFormId = material.LiquidFormId;
    }

    public Solid(string name, MaterialBehaviourType behaviour, IFuturemud gameworld) : base(name, behaviour, gameworld)
    {
        ImpactFracture = 10000;
        ImpactYield = 10000;
        ImpactStrainAtYield = 100000;
        ShearFracture = 25000;
        ShearYield = 25000;
        ShearStrainAtYield = 100000;
        SolventRatio = 1.0;
        ResidueColour = Telnet.White;
        Absorbency = 0;
        switch (behaviour)
        {
            case MaterialBehaviourType.Leather:
                Absorbency = 0.2;
                break;
            case MaterialBehaviourType.Fabric:
                Absorbency = 2.0;
                break;
            case MaterialBehaviourType.Hair:
                Absorbency = 0.2;
                break;
            case MaterialBehaviourType.Food:
                Absorbency = 0.2;
                break;
            case MaterialBehaviourType.Feather:
                Absorbency = 0.5;
                break;
        }

        using (new FMDB())
        {
            Models.Material dbnew = new();
            FMDB.Context.Materials.Add(dbnew);

            dbnew.Absorbency = Absorbency;
            dbnew.BehaviourType = (int)BehaviourType;
            dbnew.Density = Density;
            dbnew.ElectricalConductivity = ElectricalConductivity;
            dbnew.IgnitionPoint = IgnitionPoint;
            dbnew.ImpactFracture = ImpactFracture;
            dbnew.ImpactStrainAtYield = ImpactStrainAtYield;
            dbnew.ImpactYield = ImpactYield;
            dbnew.MaterialDescription = MaterialDescription;
            dbnew.MeltingPoint = MeltingPoint;
            dbnew.Name = Name;
            dbnew.Organic = Organic;
            dbnew.ResidueColour = ResidueColour.Name;
            dbnew.ResidueDesc = ResidueDesc;
            dbnew.ResidueSdesc = ResidueSdesc;
            dbnew.ShearFracture = ShearFracture;
            dbnew.ShearStrainAtYield = ShearStrainAtYield;
            dbnew.ShearYield = ShearYield;
            dbnew.SolventId = Solvent?.Id;
            dbnew.SolventVolumeRatio = SolventRatio;
            dbnew.SpecificHeatCapacity = SpecificHeatCapacity;
            dbnew.ThermalConductivity = ThermalConductivity;
            dbnew.Type = (int)MaterialType;
            dbnew.YoungsModulus = YoungsModulus;

            FMDB.Context.SaveChanges();
            _id = dbnew.Id;
            Gameworld.Add(this);
        }
    }

    public override MaterialType MaterialType => MaterialType.Solid;

    public override string FrameworkItemType => "Solid";

    protected override string MaterialNoun => "solid";

    #region ISolid Members

    private readonly List<string> _aliases = [];

    public IEnumerable<string> Aliases => _aliases;

    public IEnumerable<string> Names
    {
        get
        {
            yield return Name;
            foreach (string alias in _aliases)
            {
                yield return alias;
            }
        }
    }

    public double ImpactFracture { get; set; }

    public double ImpactYield { get; set; }

    public double ImpactStrainAtYield { get; set; }

    public double ShearFracture { get; set; }

    public double ShearYield { get; set; }

    public double ShearStrainAtYield { get; set; }

    public double? HeatDamagePoint { get; set; }

    public double YoungsModulus { get; set; }

    public double? IgnitionPoint { get; set; }

    public double? MeltingPoint { get; set; }

    public double Absorbency { get; set; }

    private long? _solventId;
    private ILiquid _solvent;

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
    }

    public double SolventRatio { get; set; }

    public string ResidueSdesc { get; set; }

    public string ResidueDesc { get; set; }

    public ANSIColour ResidueColour { get; set; }

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

    public IGas GasForm { get; set; }

    internal bool TryAddAlias(string alias, out string errorMessage)
    {
        string normalisedAlias = alias.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalisedAlias))
        {
            errorMessage = "You must specify some text for the alias.";
            return false;
        }

        if (normalisedAlias.EqualTo(Name))
        {
            errorMessage = "That is already this material's primary name.";
            return false;
        }

        if (_aliases.Any(x => x.EqualTo(normalisedAlias)))
        {
            errorMessage = $"{normalisedAlias.ColourName()} is already an alias for this material.";
            return false;
        }

        ISolid conflictingMaterial = Gameworld.Materials.FirstOrDefault(x =>
            !ReferenceEquals(x, this) &&
            x.Names.Any(y => y.EqualTo(normalisedAlias)));
        if (conflictingMaterial is not null)
        {
            errorMessage =
                $"{normalisedAlias.ColourName()} is already used by {conflictingMaterial.Name.ColourName()}. Solid names and aliases must be unique.";
            return false;
        }

        _aliases.Add(normalisedAlias);
        Changed = true;
        errorMessage = string.Empty;
        return true;
    }

    internal bool TryRemoveAlias(string alias, out string removedAlias, out string errorMessage)
    {
        string normalisedAlias = alias.ToLowerInvariant();
        removedAlias = _aliases.FirstOrDefault(x => x.EqualTo(normalisedAlias));
        if (removedAlias is null)
        {
            errorMessage = $"{normalisedAlias.ColourName()} is not an alias for this material.";
            return false;
        }

        _aliases.Remove(removedAlias);
        Changed = true;
        errorMessage = string.Empty;
        return true;
    }

    internal bool ClearAliases()
    {
        if (!_aliases.Any())
        {
            return false;
        }

        _aliases.Clear();
        Changed = true;
        return true;
    }

    public ISolid Clone(string newName, string newDescription)
    {
        using (new FMDB())
        {
            Models.Material dbnew = new();
            FMDB.Context.Materials.Add(dbnew);

            dbnew.Absorbency = Absorbency;
            dbnew.BehaviourType = (int)BehaviourType;
            dbnew.Density = Density;
            dbnew.ElectricalConductivity = ElectricalConductivity;
            dbnew.HeatDamagePoint = HeatDamagePoint;
            dbnew.IgnitionPoint = IgnitionPoint;
            dbnew.ImpactFracture = ImpactFracture;
            dbnew.ImpactStrainAtYield = ImpactStrainAtYield;
            dbnew.ImpactYield = ImpactYield;
            dbnew.MaterialDescription = newDescription;
            dbnew.MeltingPoint = MeltingPoint;
            dbnew.Name = newName;
            dbnew.Organic = Organic;
            dbnew.ResidueColour = ResidueColour.Name;
            dbnew.ResidueDesc = ResidueDesc;
            dbnew.ResidueSdesc = ResidueSdesc;
            dbnew.ShearFracture = ShearFracture;
            dbnew.ShearStrainAtYield = ShearStrainAtYield;
            dbnew.ShearYield = ShearYield;
            dbnew.SolventId = Solvent?.Id;
            dbnew.SolventVolumeRatio = SolventRatio;
            dbnew.SpecificHeatCapacity = SpecificHeatCapacity;
            dbnew.ThermalConductivity = ThermalConductivity;
            dbnew.Type = (int)MaterialType;
            dbnew.YoungsModulus = YoungsModulus;

            foreach (ITag tag in Tags)
            {
                dbnew.MaterialsTags.Add(new MaterialsTags { Material = dbnew, TagId = tag.Id });
            }

            FMDB.Context.SaveChanges();
            Solid newSolid = new(dbnew, Gameworld);
            Gameworld.Add(newSolid);
            return newSolid;
        }
    }

    #endregion

    #region IFutureProgVariable Implementation

    public override ProgVariableTypes Type => ProgVariableTypes.Solid;

    public override IProgVariable GetProperty(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "liquidform":
                return LiquidForm;
            case "gasform":
                return GasForm;
        }

        return base.GetProperty(property);
    }

    private new static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
    {
        Dictionary<string, ProgVariableTypes> dict = new(Material.DotReferenceHandler());
        dict.Add("liquidform", ProgVariableTypes.Liquid);
        dict.Add("gasform", ProgVariableTypes.Gas);
        return dict;
    }

    private new static IReadOnlyDictionary<string, string> DotReferenceHelp()
    {
        Dictionary<string, string> dict = new(Material.DotReferenceHelp());
        dict.Add("liquidform", "The liquid form of this solid");
        dict.Add("gasform", "The gas form of this solid");
        return dict;
    }

    public new static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Solid, DotReferenceHandler(),
            DotReferenceHelp());
    }

    #endregion

    #region Overrides of Material

    /// <inheritdoc />
    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.Append($"Material #{Id.ToString("N0", actor)}".Colour(Telnet.BoldYellow));
        sb.AppendLine($" - {Name.Colour(ResidueColour)}");
        sb.AppendLine($"Description: {MaterialDescription.ColourValue()}");
        sb.AppendLine(
            $"Aliases: {(Aliases.Any() ? Aliases.Select(x => x.ColourName()).ListToString() : "None".Colour(Telnet.Red))}");
        sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
        {
            $"Density: {$"{Density.ToString("N5", actor)} kg/m3".ColourValue()}",
            $"Type: {BehaviourType.DescribeEnum().ColourValue()}",
            $"Organic: {Organic.ToColouredString()}"
        });
        sb.AppendLine();
        sb.AppendLine("Engineering Properties".GetLineWithTitle(actor.LineFormatLength,
            actor.Account.UseUnicode, Telnet.Cyan,
            Telnet.BoldYellow));
        sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
        {
            $"Impact Fracture: {actor.Gameworld.UnitManager.DescribeDecimal(ImpactFracture, UnitType.Stress, actor).ColourValue()}",
            $"Impact Yield: {actor.Gameworld.UnitManager.DescribeDecimal(ImpactYield, UnitType.Stress, actor).ColourValue()}",
            $"Impact Strain @ Yield: {ImpactStrainAtYield.ToString("N", actor).ColourValue()}"
        });

        sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
        {
            $"Shear Fracture: {actor.Gameworld.UnitManager.DescribeDecimal(ShearFracture, UnitType.Stress, actor).ColourValue()}",
            $"Shear Yield: {actor.Gameworld.UnitManager.DescribeDecimal(ShearYield, UnitType.Stress, actor).ColourValue()}",
            $"Shear Strain @ Yield: {ShearStrainAtYield.ToString("N", actor).ColourValue()}"
        });

        sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
        {
            $"Young's Modulus: {actor.Gameworld.UnitManager.DescribeDecimal(YoungsModulus, UnitType.Stress, actor).ColourValue()}",
            $"Liquid Form: {LiquidForm?.Name.Colour(Telnet.Cyan) ?? "None".Colour(Telnet.Red)}",
            ""
        });
        sb.AppendLine();
        sb.AppendLine("Energy Properties".GetLineWithTitle(actor.LineFormatLength, actor.Account.UseUnicode,
            Telnet.Cyan,
            Telnet.BoldYellow));
        sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
        {
            $"Ignition Point: {(IgnitionPoint != null ? actor.Gameworld.UnitManager.Describe(IgnitionPoint.Value, UnitType.Temperature, actor).ColourValue() : "N/A".Colour(Telnet.Yellow))}",
            $"Heat Damage Point: {(HeatDamagePoint.HasValue ? actor.Gameworld.UnitManager.Describe(HeatDamagePoint.Value, UnitType.Temperature, actor).ColourValue() : "N/A".Colour(Telnet.Yellow))}",
            $"Melting Point: {(MeltingPoint != null ? actor.Gameworld.UnitManager.Describe(MeltingPoint.Value, UnitType.Temperature, actor).ColourValue() : "N/A".Colour(Telnet.Yellow))}"
        });
        sb.AppendLine(
            $"Thermal Conductivity: {$"{ThermalConductivity.ToString("N9", actor)} Watts/Kelvin".ColourValue()}");
        sb.AppendLine(
            $"Specific Heat Capacity: {$"{SpecificHeatCapacity.ToString("N9", actor)} J/°C/kg".ColourValue()}");
        sb.AppendLine(
            $"Electrical Conductivity: {$"{ElectricalConductivity.ToString("N9", actor)} Siemens".ColourValue()}");
        sb.AppendLine();
        sb.AppendLine("Cleaning & Residency".GetLineWithTitle(actor.LineFormatLength, actor.Account.UseUnicode,
            Telnet.Cyan,
            Telnet.BoldYellow));
        sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
        {
            $"Residue Sdesc: {(string.IsNullOrWhiteSpace(ResidueSdesc) ? "None".Colour(Telnet.Red) : ResidueSdesc.Colour(ResidueColour))}",
            $"Residue Desc: {(string.IsNullOrWhiteSpace(ResidueDesc) ? "None".Colour(Telnet.Red) : ResidueDesc.Colour(ResidueColour))}",
            $"Residue Colour: {ResidueColour.Name.Colour(ResidueColour)}"
        });

        sb.AppendLineColumns((uint)actor.LineFormatLength, 3, new[]
        {
            $"Solvent: {Solvent?.Name.Colour(Solvent.DisplayColour) ?? "None".Colour(Telnet.Red)}",
            $"Solvent Ratio: {SolventRatio.ToString("P3", actor).ColourValue()}",
            $"Absorbency: {Absorbency.ToString("P3", actor).ColourValue()}"
        });
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
            foreach (ITag tag in Tags)
            {
                sb.AppendLine($"\t{tag.FullName.ColourName()}");
            }
        }

        return sb.ToString();
    }

    /// <inheritdoc />
    protected override string HelpText => $@"{base.HelpText}
	#3alias add <text>#0 - adds an alias for this material
	#3alias remove <text>#0 - removes an alias from this material
	#3alias clear#0 - removes all aliases from this material
	#3impactyield <value>#0 - sets the impact yield strength in kPa
	#3impactfracture <value>#0 - sets the impact fracture strength in kPa
	#3impactstrain <value>#0 - sets the strain at yield for impact
	#3shearyield <value>#0 - sets the shear yield strength in kPa
	#3shearfracture <value>#0 - sets the shear fracture strength in kPa
	#3shearstrain <value>#0 - sets the strain at yield for shear
	#3heatdamage <temp>|none#0 - sets or clears the temperature for heat damage
	#3ignition <temp>|none#0 - sets or clears the temperature for ignition
	#3melting <temp>|none#0 - sets or clears the temperature for melting
	#3absorbency <%>#0 - sets the absorbency for liquids
	#3solvent <liquid>|none#0 - sets or clears the required solvent for residues
	#3solventratio <%>#0 - sets the ratio of solvent to removed contaminant by mass
	#3liquidform <liquid>|none#0 - sets or clears a liquid as the liquid form of this
	#3residuecolour <colour>#0 - sets the colour of this material and its residues
	#3residuesdesc <tag>|none#0 - sets or clears the sdesc tag for residues of this
	#3residuedesc <desc>|none#0 - sets or clears the added description for residues of this";

    /// <inheritdoc />
    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "alias":
                return BuildingCommandAlias(actor, command);
            case "impactyield":
                return BuildingCommandImpactYield(actor, command);
            case "impactstrain":
                return BuildingCommandImpactStrain(actor, command);
            case "impactfracture":
                return BuildingCommandImpactFracture(actor, command);
            case "shearyield":
                return BuildingCommandShearYield(actor, command);
            case "shearstrain":
                return BuildingCommandShearStrain(actor, command);
            case "shearfracture":
                return BuildingCommandShearFracture(actor, command);
            case "heatdamagepoint":
            case "heatpoint":
            case "heatdamage":
                return BuildingCommandHeatDamagePoint(actor, command);
            case "ignitionpoint":
            case "ignition":
                return BuildingCommandIgnitionPoint(actor, command);
            case "meltingpoint":
            case "melting":
                return BuildingCommandMeltingPoint(actor, command);
            case "absorb":
            case "absorbency":
                return BuildingCommandAbsorbency(actor, command);
            case "solvent":
                return BuildingCommandSolvent(actor, command);
            case "solventratio":
                return BuildingCommandSolventRatio(actor, command);
            case "residuecolour":
            case "residuecolor":
                return BuildingCommandResidueColour(actor, command);
            case "residuesdesc":
                return BuildingCommandSDesc(actor, command);
            case "residuedesc":
                return BuildingCommandResidueDesc(actor, command);
            case "liquidform":
                return BuildingCommandLiquidForm(actor, command);
        }

        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandAlias(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "add":
                return BuildingCommandAliasAdd(actor, command);
            case "remove":
            case "delete":
                return BuildingCommandAliasRemove(actor, command);
            case "clear":
                return BuildingCommandAliasClear(actor);
            default:
                actor.OutputHandler.Send(
                    $"You can use the {"alias add <text>".ColourCommand()}, {"alias remove <text>".ColourCommand()}, or {"alias clear".ColourCommand()} options.");
                return false;
        }
    }

    private bool BuildingCommandAliasAdd(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What alias do you want to add?");
            return false;
        }

        if (!TryAddAlias(command.SafeRemainingArgument, out string errorMessage))
        {
            actor.OutputHandler.Send(errorMessage);
            return false;
        }

        actor.OutputHandler.Send(
            $"This material can now also be targeted by the alias {command.SafeRemainingArgument.ToLowerInvariant().ColourName()}.");
        return true;
    }

    private bool BuildingCommandAliasRemove(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which alias do you want to remove?");
            return false;
        }

        if (!TryRemoveAlias(command.SafeRemainingArgument, out string removedAlias, out string errorMessage))
        {
            actor.OutputHandler.Send(errorMessage);
            return false;
        }

        actor.OutputHandler.Send(
            $"This material will no longer be targeted by the alias {removedAlias.ColourName()}.");
        return true;
    }

    private bool BuildingCommandAliasClear(ICharacter actor)
    {
        if (!ClearAliases())
        {
            actor.OutputHandler.Send("This material does not have any aliases to clear.");
            return false;
        }

        actor.OutputHandler.Send("This material no longer has any aliases.");
        return true;
    }

    private bool BuildingCommandImpactYield(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out double value) || value <= 0.0)
        {
            actor.OutputHandler.Send("You must enter a valid positive number.");
            return false;
        }

        ImpactYield = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"You set the impact yield strength for this material to {value.ToString("N0").ColourValue()} kilopascals.");
        return true;
    }

    private bool BuildingCommandImpactStrain(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out double value) || value <= 0.0)
        {
            actor.OutputHandler.Send("You must enter a valid positive number.");
            return false;
        }

        ImpactStrainAtYield = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"You set the impact strain at yield for this material to {value.ToString("N0").ColourValue()}.");
        return true;
    }

    private bool BuildingCommandImpactFracture(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out double value) || value <= 0.0)
        {
            actor.OutputHandler.Send("You must enter a valid positive number.");
            return false;
        }

        ImpactFracture = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"You set the impact fracture strength for this material to {value.ToString("N0").ColourValue()} kilopascals.");
        return true;
    }

    private bool BuildingCommandShearYield(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out double value) || value <= 0.0)
        {
            actor.OutputHandler.Send("You must enter a valid positive number.");
            return false;
        }

        ShearYield = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"You set the shear yield strength for this material to {value.ToString("N0").ColourValue()} kilopascals.");
        return true;
    }

    private bool BuildingCommandShearStrain(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out double value) || value <= 0.0)
        {
            actor.OutputHandler.Send("You must enter a valid positive number.");
            return false;
        }

        ShearStrainAtYield = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"You set the shear strain at yield for this material to {value.ToString("N0").ColourValue()}.");
        return true;
    }

    private bool BuildingCommandShearFracture(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out double value) || value <= 0.0)
        {
            actor.OutputHandler.Send("You must enter a valid positive number.");
            return false;
        }

        ShearFracture = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"You set the shear fracture strength for this material to {value.ToString("N0").ColourValue()} kilopascals.");
        return true;
    }

    private bool BuildingCommandHeatDamagePoint(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either enter a temperature, or use {"none".ColourCommand()} to remove it.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            HeatDamagePoint = null;
            Changed = true;
            actor.OutputHandler.Send($"This material no longer has a heat damage temperature.");
            return true;
        }

        if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Temperature, actor,
                out double value))
        {
            actor.OutputHandler.Send($"That is not a valid temperature.");
            return false;
        }

        HeatDamagePoint = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"This material will now have a heat damage point of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.Temperature, actor).ColourValue()}.");
        return true;
    }

    private bool BuildingCommandIgnitionPoint(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either enter a temperature, or use {"none".ColourCommand()} to remove it.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            IgnitionPoint = null;
            Changed = true;
            actor.OutputHandler.Send($"This material no longer has an ignition temperature.");
            return true;
        }

        if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Temperature, actor,
                out double value))
        {
            actor.OutputHandler.Send($"That is not a valid temperature.");
            return false;
        }

        IgnitionPoint = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"This material will now have an ignition temperature of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.Temperature, actor).ColourValue()}.");
        return true;
    }

    private bool BuildingCommandMeltingPoint(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either enter a temperature, or use {"none".ColourCommand()} to remove it.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            MeltingPoint = null;
            Changed = true;
            actor.OutputHandler.Send($"This material no longer has a melting point temperature.");
            return true;
        }

        if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Temperature, actor,
                out double value))
        {
            actor.OutputHandler.Send($"That is not a valid temperature.");
            return false;
        }

        MeltingPoint = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"This material will now have a melting point of {Gameworld.UnitManager.DescribeDecimal(value, UnitType.Temperature, actor).ColourValue()}.");
        return true;
    }

    private bool BuildingCommandAbsorbency(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out double value) || value < 0.0)
        {
            actor.OutputHandler.Send("You must enter a valid percentage.");
            return false;
        }

        Absorbency = value;
        Changed = true;
        actor.OutputHandler.Send($"You set the absorbency for this material to {value.ToString("P0").ColourValue()}.");
        return true;
    }

    private bool BuildingCommandSolvent(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either specify a required solvent liquid for residues of this material, or use {"none".ColourCommand()} to clear one.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            _solvent = null;
            _solventId = null;
            SolventRatio = 0.0;
            Changed = true;
            actor.OutputHandler.Send(
                $"This material will no longer require any particular solvent to clean as a residue.");
            return true;
        }

        ILiquid liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
        if (liquid is null)
        {
            actor.OutputHandler.Send("There is no such liquid.");
            return false;
        }

        if (_solventId is null)
        {
            SolventRatio = 1.0;
        }

        _solvent = liquid;
        _solventId = liquid.Id;
        Changed = true;
        actor.OutputHandler.Send(
            $"This material will now require {liquid.Name.Colour(liquid.DisplayColour)} as a solvent to clean residues.");
        return true;
    }

    private bool BuildingCommandSolventRatio(ICharacter actor, StringStack command)
    {
        if (Solvent is null)
        {
            actor.OutputHandler.Send("You must set a solvent before you can set a solvent ratio.");
            return false;
        }

        if (command.IsFinished ||
            !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out double value) || value < 0.0)
        {
            actor.OutputHandler.Send("You must enter a valid percentage.");
            return false;
        }

        SolventRatio = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"This material will now require {value.ToString("P2").ColourValue()} by weight of solvent liquid to clean.");
        return true;
    }

    private bool BuildingCommandResidueColour(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What colour should residues of this material be? The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
            return false;
        }

        ANSIColour colour = Telnet.GetColour(command.SafeRemainingArgument);
        if (colour == null)
        {
            actor.OutputHandler.Send(
                $"That is not a valid colour. The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
            return false;
        }

        ResidueColour = colour;
        Changed = true;
        actor.OutputHandler.Send(
            $"This material's residues now have a display colour of {colour.Name.Colour(colour)}.");
        return true;
    }

    private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either enter text to put inside the residue tag, or use {"none".ColourCommand()} to clear it and not have one.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            ResidueSdesc = null;
            Changed = true;
            actor.OutputHandler.Send(
                $"Residues of this material will no longer have any tag added to the contaminated thing's short description.");
            return true;
        }

        ResidueSdesc = command.SafeRemainingArgument.ToLowerInvariant().ParenthesesIfNot();
        Changed = true;
        actor.OutputHandler.Send(
            $"This material's residues will now append the tag {ResidueSdesc.Colour(ResidueColour)} to the contaminated things' short description.");
        return true;
    }

    private bool BuildingCommandResidueDesc(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either enter text to append to the contaminated item, or use {"none".ColourCommand()} to clear it and not have one.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            ResidueDesc = null;
            Changed = true;
            actor.OutputHandler.Send(
                $"Residues of this material will no longer have any tag added to the contaminated thing's short description.");
            return true;
        }

        ResidueDesc = command.SafeRemainingArgument.ProperSentences();
        Changed = true;
        actor.OutputHandler.Send(
            $"This material's residues will now be described as {ResidueDesc.Colour(ResidueColour)}.");
        return true;
    }

    private bool BuildingCommandLiquidForm(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either specify a liquid form of this material, or use {"none".ColourCommand()} to clear one.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            _liquidForm = null;
            _liquidFormId = null;
            Changed = true;
            actor.OutputHandler.Send($"This material will no longer have a liquid form.");
            return true;
        }

        ILiquid liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
        if (liquid is null)
        {
            actor.OutputHandler.Send("There is no such liquid.");
            return false;
        }

        _liquidForm = liquid;
        _liquidFormId = liquid.Id;
        Changed = true;
        actor.OutputHandler.Send(
            $"This material will now have {liquid.Name.Colour(liquid.DisplayColour)} as its liquid form.");
        return true;
    }

    #endregion

    #region Overrides of SaveableItem

    /// <inheritdoc />
    public override void Save()
    {
        Models.Material dbitem = FMDB.Context.Materials.Find(Id);
        dbitem.Name = Name;
        dbitem.Organic = Organic;
        dbitem.MaterialDescription = MaterialDescription;
        dbitem.SpecificHeatCapacity = SpecificHeatCapacity;
        dbitem.ElectricalConductivity = ElectricalConductivity;
        dbitem.ThermalConductivity = ThermalConductivity;
        dbitem.Density = Density;
        dbitem.ImpactFracture = ImpactFracture;
        dbitem.ImpactStrainAtYield = ImpactStrainAtYield;
        dbitem.ImpactYield = ImpactYield;
        dbitem.ShearFracture = ShearFracture;
        dbitem.ShearStrainAtYield = ShearStrainAtYield;
        dbitem.ShearYield = ShearYield;
        dbitem.HeatDamagePoint = HeatDamagePoint;
        dbitem.YoungsModulus = YoungsModulus;
        dbitem.IgnitionPoint = IgnitionPoint;
        dbitem.MeltingPoint = MeltingPoint;
        dbitem.Absorbency = Absorbency;
        dbitem.SolventId = _solventId;
        dbitem.SolventVolumeRatio = SolventRatio;
        dbitem.ResidueSdesc = ResidueSdesc;
        dbitem.ResidueDesc = ResidueDesc;
        dbitem.ResidueColour = ResidueColour.Name;
        dbitem.LiquidFormId = LiquidForm?.Id;
        FMDB.Context.MaterialAliases.RemoveRange(dbitem.MaterialAliases);
        foreach (string alias in _aliases)
        {
            dbitem.MaterialAliases.Add(new MaterialAlias
            {
                Material = dbitem,
                Alias = alias
            });
        }
        FMDB.Context.MaterialsTags.RemoveRange(dbitem.MaterialsTags);
        foreach (ITag tag in Tags)
        {
            dbitem.MaterialsTags.Add(new MaterialsTags
            {
                Material = dbitem,
                TagId = tag.Id
            });
        }

        Changed = false;
    }

    #endregion
}
