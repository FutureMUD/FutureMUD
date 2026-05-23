using ExpressionEngine;
using MudSharp.Form.Material;
using MudSharp.Framework.Units;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MudSharp.Work.Foraging;

public class Foragable : EditableItem, IForagable
{
    #region Static Members

    public static string BaseForageTimeExpression
        => Futuremud.Games.First().GetStaticConfiguration("BaseForageTimeExpression");

    #endregion

    private string DescribeOutput(ICharacter actor)
    {
        if (ItemProto != null)
        {
            return string.Format(actor, "item #{0:N0}r{1:N0} - {2} ({3})", ItemProto.Id, ItemProto.RevisionNumber,
                ItemProto.ShortDescription, QuantityDiceExpression.ColourCommand());
        }

        if (CommodityMaterial != null)
        {
            return $"{DescribeCommodityWeightExpression(actor)} of {CommodityMaterial.Name.Colour(CommodityMaterial.ResidueColour)}{(CommodityTag != null ? $" tagged {CommodityTag.FullName.ColourName()}" : "")}";
        }

        return "Not Selected".ColourError();
    }

    private string DescribeCommodityWeightExpression(ICharacter actor)
    {
        if (string.IsNullOrWhiteSpace(CommodityWeightExpression))
        {
            return "Not Selected".ColourError();
        }

        return double.TryParse(CommodityWeightExpression, NumberStyles.Float, CultureInfo.InvariantCulture, out var weight)
            ? Gameworld.UnitManager.DescribeExact(weight, UnitType.Mass, actor).ColourValue()
            : CommodityWeightExpression.ColourCommand();
    }

    private bool HasItemOutput => ItemProto != null;
    private bool HasCommodityMaterial => CommodityMaterial != null;
    private bool HasCommodityOutput => HasCommodityMaterial && !string.IsNullOrWhiteSpace(CommodityWeightExpression);
    private bool HasExactlyOneOutput => HasItemOutput && !HasCommodityMaterial || !HasItemOutput && HasCommodityOutput;

    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine(
            $"Foragable #{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} - {Name.Colour(Telnet.Cyan)} - {Status.DescribeColour()}");
        sb.AppendLine();
        sb.Append(new List<string>
        {
            $"Minimum Outcome: {MinimumOutcome.DescribeColour()}",
            $"Maximum Outcome: {MaximumOutcome.DescribeColour()}"
        }.ArrangeStringsOntoLines(2, (uint)actor.LineFormatLength));
        sb.Append(new List<string>
        {
            $"Forage Difficulty: {ForageDifficulty.Describe().Colour(Telnet.Green)}",
            $"Relative Chance: {RelativeChance.ToString("N0", actor).ColourValue()}"
        }.ArrangeStringsOntoLines(2, (uint)actor.LineFormatLength));
        sb.AppendLineFormat("Output: {0}", DescribeOutput(actor));
        sb.AppendLineFormat("Foragable Types: {0}",
            ForagableTypes.Any()
                ? ForagableTypes.Select(x => x.Colour(Telnet.Green)).ListToString()
                : "None".Colour(Telnet.Red));
        sb.Append(new List<string>
        {
            $"Can Forage Prog: {CanForageProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Yellow)}",
            $"On Forage Prog: {OnForageProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Yellow)}"
        }.ArrangeStringsOntoLines(2, (uint)actor.LineFormatLength));

        return sb.ToString();
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        using (new FMDB())
        {
            Models.Foragable dbnew = new()
            {
                Id = Id,
                RevisionNumber = FMDB.Context.Foragables.Where(x => x.Id == Id).Select(x => x.RevisionNumber)
                                     .AsEnumerable().DefaultIfEmpty(0).Max() +
                                 1,
                Name = Name,
                CanForageProgId = CanForageProg?.Id,
                OnForageProgId = OnForageProg?.Id,
                ForagableTypes = SerialisedForagableTypes,
                ForageDifficulty = (int)ForageDifficulty,
                ItemProtoId = ItemProto?.Id ?? 0,
                MinimumOutcome = (int)MinimumOutcome,
                MaximumOutcome = (int)MaximumOutcome,
                QuantityDiceExpression = QuantityDiceExpression,
                CommodityMaterialId = CommodityMaterial?.Id,
                CommodityTagId = HasCommodityMaterial ? CommodityTag?.Id : null,
                CommodityWeightExpression = HasCommodityMaterial ? CommodityWeightExpression : null,
                RelativeChance = RelativeChance,
                EditableItem = new Models.EditableItem()
            };

            FMDB.Context.EditableItems.Add(dbnew.EditableItem);
            dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
            dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
            dbnew.EditableItem.BuilderAccountId = initiator.Account.Id;
            dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;

            FMDB.Context.Foragables.Add(dbnew);
            FMDB.Context.SaveChanges();

            return new Foragable(dbnew, Gameworld);
        }
    }

    public override string EditHeader()
    {
        return $"Foragable {Name} ({Id:N0}r{RevisionNumber:N0})";
    }

    public override bool CanSubmit()
    {
        return ForagableTypes.Any() && HasExactlyOneOutput;
    }

    public override string WhyCannotSubmit()
    {
        if (!ForagableTypes.Any())
        {
            return "You must set at least one foragable type.";
        }

        if (HasItemOutput && HasCommodityMaterial)
        {
            return "You must set either an item prototype or a commodity output, not both.";
        }

        if (HasCommodityMaterial && string.IsNullOrWhiteSpace(CommodityWeightExpression))
        {
            return "You must set a commodity weight expression.";
        }

        return !HasItemOutput && !HasCommodityMaterial
            ? "You must set either an item prototype or a commodity output."
            : "I don't know why you can't submit.";
    }

    public override string FrameworkItemType => "Foragable";

    public override void Save()
    {
        using (new FMDB())
        {
            Models.Foragable dbitem = FMDB.Context.Foragables.Find(Id, RevisionNumber);
            if (_statusChanged)
            {
                base.Save(dbitem.EditableItem);
            }

            dbitem.Name = Name;
            dbitem.CanForageProgId = CanForageProg?.Id;
            dbitem.OnForageProgId = OnForageProg?.Id;
            dbitem.ForagableTypes = SerialisedForagableTypes;
            dbitem.ForageDifficulty = (int)ForageDifficulty;
            dbitem.ItemProtoId = ItemProto?.Id ?? 0;
            dbitem.CommodityMaterialId = CommodityMaterial?.Id;
            dbitem.CommodityTagId = HasCommodityMaterial ? CommodityTag?.Id : null;
            dbitem.CommodityWeightExpression = HasCommodityMaterial ? CommodityWeightExpression : null;
            dbitem.MinimumOutcome = (int)MinimumOutcome;
            dbitem.MaximumOutcome = (int)MaximumOutcome;
            dbitem.QuantityDiceExpression = QuantityDiceExpression ?? "1";
            dbitem.RelativeChance = RelativeChance;
            FMDB.Context.SaveChanges();
        }

        Changed = false;
    }

    #region Constructors

    public Foragable(IAccount originator)
        : base(originator)
    {
        Gameworld = originator.Gameworld;
        using (new FMDB())
        {
            Models.Foragable dbitem = new()
            {
                Id = Gameworld.Foragables.NextID()
            };
            FMDB.Context.Foragables.Add(dbitem);
            Models.EditableItem dbedit = new();
            FMDB.Context.EditableItems.Add(dbedit);
            dbitem.EditableItem = dbedit;
            dbedit.BuilderAccountId = BuilderAccountID;
            dbedit.BuilderDate = BuilderDate;
            dbedit.RevisionStatus = (int)Status;
            dbedit.RevisionNumber = 0;

            _name = "Unnamed Foragable";
            _forageDifficulty = Difficulty.Normal;
            _relativeChance = 100;
            _minimumOutcome = Outcome.MajorFail;
            _maximumOutcome = Outcome.MajorPass;
            _quantityDiceExpression = "1";
            _commodityWeightExpression = null;

            dbitem.Name = _name;
            dbitem.RelativeChance = RelativeChance;
            dbitem.ForageDifficulty = (int)ForageDifficulty;
            dbitem.MinimumOutcome = (int)MinimumOutcome;
            dbitem.MaximumOutcome = (int)MaximumOutcome;
            dbitem.QuantityDiceExpression = QuantityDiceExpression;
            dbitem.ForagableTypes = "";
            FMDB.Context.SaveChanges();
            LoadFromDb(dbitem);
        }
    }

    public Foragable(MudSharp.Models.Foragable foragable, IFuturemud gameworld)
        : base(foragable.EditableItem)
    {
        Gameworld = gameworld;
        LoadFromDb(foragable);
    }

    private void LoadFromDb(MudSharp.Models.Foragable foragable)
    {
        _id = foragable.Id;
        _name = foragable.Name;
        _foragableTypes = ParseForagableTypes(foragable.ForagableTypes);
        _forageDifficulty = (Difficulty)foragable.ForageDifficulty;
        _relativeChance = foragable.RelativeChance;
        _minimumOutcome = (Outcome)foragable.MinimumOutcome;
        _maximumOutcome = (Outcome)foragable.MaximumOutcome;
        _onForageProg = Gameworld.FutureProgs.Get(foragable.OnForageProgId ?? 0);
        _canForageProg = Gameworld.FutureProgs.Get(foragable.CanForageProgId ?? 0);
        _quantityDiceExpression = foragable.QuantityDiceExpression;
        _itemProtoId = foragable.ItemProtoId;
        _commodityMaterialId = foragable.CommodityMaterialId ?? 0;
        _commodityTagId = foragable.CommodityTagId ?? 0;
        _commodityWeightExpression = foragable.CommodityWeightExpression;
    }

    protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
    {
        return Gameworld.Foragables.GetAll(Id);
    }

    #endregion

    #region Building Commands

    private const string BuildingCommandHelp = @"You can use the following options with this command:

	#3name <name>#0 - renames this foragable
	#3proto <which>#0 - sets the item prototype for this foragable to load and clears commodity output
	#3commodity material <material> [tag <tag>|notag] weight <weight>#0 - switches this foragable to commodity output
	#3commodity clear#0 - clears commodity output
	#3material <which>#0 - sets the commodity material and clears item prototype output
	#3tag <which>#0 - sets the commodity tag
	#3tag none#0 - clears the commodity tag
	#3weight <weight>#0 - sets a fixed commodity weight
	#3weight variable <expression>#0 - sets a variable commodity weight expression, in base mass units, with #6outcome#0 available
	#3chance <#>#0 - the relative weight of this option being found
	#3quantity <# or dice>#0 - a number or dice expression for the item quantity found
	#3difficulty <difficulty>#0 - the difficulty that the result is evaluated against for this output
	#3outcome <min> <max>#0 - the minimum and maximum check outcome that this item can appear on
	#3types <type1> [<type2>] ... [<typen>]#0 - sets the yield types that this foragable appears against
	#3canforage <prog>#0 - sets a boolean prog that controls whether this foragable can be found
	#3canforage clear#0 - clears the can-forage prog
	#3onforage <prog>#0 - sets a prog that will run when this item is foraged
	#3onforage clear#0 - clears the on-forage prog";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "name":
                return BuildingCommandName(actor, command);
            case "types":
                return BuildingCommandTypes(actor, command);
            case "difficulty":
                return BuildingCommandDifficulty(actor, command);
            case "outcome":
                return BuildingCommandOutcome(actor, command);
            case "chance":
                return BuildingCommandChance(actor, command);
            case "onforage":
                return BuildingCommandOnForage(actor, command);
            case "canforage":
                return BuildingCommandCanForage(actor, command);
            case "proto":
                return BuildingCommandProto(actor, command);
            case "commodity":
                return BuildingCommandCommodity(actor, command);
            case "material":
                return BuildingCommandMaterial(actor, command);
            case "tag":
                return BuildingCommandTag(actor, command);
            case "weight":
                return BuildingCommandWeight(actor, command);
            case "quantity":
                return BuildingCommandQuantity(actor, command);
            default:
                actor.OutputHandler.Send(BuildingCommandHelp.SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandCommodity(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Use {0} to set commodity output.", "commodity material <material> [tag <tag>|notag] weight <weight>".ColourCommand());
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("clear"))
        {
            if (CommodityMaterial == null && CommodityTag == null)
            {
                actor.Send("This foragable does not have any commodity output to clear.");
                return false;
            }

            ClearCommodityOutput();
            actor.Send("You clear the commodity output from this foragable.");
            return true;
        }

        ISolid material = null;
        ITag tag = null;
        string weightExpression = null;
        string weightDescription = null;
        while (!command.IsFinished)
        {
            var token = command.PopSpeech();
            if (token.EqualTo("material"))
            {
                var materialText = ConsumeUntilKeyword(command, "tag", "notag", "none", "weight");
                if (string.IsNullOrWhiteSpace(materialText))
                {
                    actor.Send("Which commodity material do you want this foragable to produce?");
                    return false;
                }

                material = Gameworld.Materials.GetByIdOrName(materialText);
                if (material == null)
                {
                    actor.Send("There is no material identified by {0}.", materialText.ColourCommand());
                    return false;
                }

                continue;
            }

            if (token.EqualTo("tag"))
            {
                var tagText = ConsumeUntilKeyword(command, "weight", "material");
                if (string.IsNullOrWhiteSpace(tagText))
                {
                    actor.Send("Which commodity tag do you want this foragable to use?");
                    return false;
                }

                tag = Gameworld.Tags.GetByIdOrName(tagText);
                if (tag == null)
                {
                    actor.Send("There is no tag identified by {0}.", tagText.ColourCommand());
                    return false;
                }

                continue;
            }

            if (token.EqualToAny("notag", "none"))
            {
                tag = null;
                continue;
            }

            if (token.EqualTo("weight"))
            {
                if (!TryParseCommodityWeightExpression(actor, command.SafeRemainingArgument, out weightExpression, out weightDescription))
                {
                    return false;
                }

                break;
            }

            actor.Send("The option {0} is not valid for commodity output. Use {1}.", token.ColourCommand(), "commodity material <material> [tag <tag>|notag] weight <weight>".ColourCommand());
            return false;
        }

        if (material == null)
        {
            actor.Send("You must specify a commodity material.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(weightExpression))
        {
            actor.Send("You must specify a commodity weight.");
            return false;
        }

        SetCommodityOutput(material, tag, weightExpression);
        actor.Send("This foragable will now produce {0} of {1}{2} when foraged.", weightDescription,
            material.Name.Colour(material.ResidueColour), tag != null ? $" tagged {tag.FullName.ColourName()}" : "");
        return true;
    }

    private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Which commodity material do you want this foragable to produce?");
            return false;
        }

        var materialText = command.SafeRemainingArgument;
        var material = Gameworld.Materials.GetByIdOrName(materialText);
        if (material == null)
        {
            actor.Send("There is no material identified by {0}.", materialText.ColourCommand());
            return false;
        }

        _itemProtoId = 0;
        CommodityMaterial = material;
        actor.Send("This foragable will now produce commodity piles of {0} instead of item prototypes.", material.Name.Colour(material.ResidueColour));
        if (string.IsNullOrWhiteSpace(CommodityWeightExpression))
        {
            actor.Send("Warning: you still need to set a commodity weight before this foragable can be submitted.".Colour(Telnet.Yellow));
        }

        return true;
    }

    private bool BuildingCommandTag(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Which commodity tag do you want this foragable to use? Use {0} to clear it.", "none".ColourCommand());
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("clear", "none", "notag"))
        {
            _itemProtoId = 0;
            CommodityTag = null;
            actor.Send("This foragable will now produce an untagged commodity pile.");
            return true;
        }

        var tagText = command.SafeRemainingArgument;
        var tag = Gameworld.Tags.GetByIdOrName(tagText);
        if (tag == null)
        {
            actor.Send("There is no tag identified by {0}.", tagText.ColourCommand());
            return false;
        }

        _itemProtoId = 0;
        CommodityTag = tag;
        actor.Send("This foragable will now apply the {0} tag to its commodity output.", tag.FullName.ColourName());
        return true;
    }

    private bool BuildingCommandWeight(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What weight of commodity should this foragable produce?");
            return false;
        }

        if (!TryParseCommodityWeightExpression(actor, command.SafeRemainingArgument, out var expression, out var description))
        {
            return false;
        }

        _itemProtoId = 0;
        CommodityWeightExpression = expression;
        actor.Send("This foragable will now produce {0} of commodity when foraged.", description);
        if (CommodityMaterial == null)
        {
            actor.Send("Warning: you still need to set a commodity material before this foragable can be submitted.".Colour(Telnet.Yellow));
        }

        return true;
    }

    private bool TryParseCommodityWeightExpression(ICharacter actor, string text, out string expression, out string description)
    {
        expression = null;
        description = null;
        var command = new StringStack(text);
        var isFormula = command.PeekSpeech().EqualToAny("variable", "formula", "expression");
        if (isFormula)
        {
            command.PopSpeech();
            text = command.SafeRemainingArgument;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            actor.Send("What weight expression do you want to use for this commodity output?");
            return false;
        }

        if (!isFormula && Gameworld.UnitManager.TryGetBaseUnits(text, UnitType.Mass, actor, out var weight))
        {
            if (weight <= 0.0)
            {
                actor.Send("Commodity weight must be greater than zero.");
                return false;
            }

            expression = weight.ToString("R", CultureInfo.InvariantCulture);
            description = Gameworld.UnitManager.DescribeExact(weight, UnitType.Mass, actor).ColourValue();
            return true;
        }

        TraitExpression testExpression = new(text, Gameworld);
        if (testExpression.HasErrors())
        {
            actor.OutputHandler.Send($"Your formula had the following error: {testExpression.Error.ColourCommand()}");
            return false;
        }

        expression = text;
        description = text.ColourCommand();
        return true;
    }

    private static string ConsumeUntilKeyword(StringStack command, params string[] keywords)
    {
        List<string> parts = new();
        while (!command.IsFinished && !command.PeekSpeech().EqualToAny(keywords))
        {
            parts.Add(command.PopSpeech());
        }

        return string.Join(" ", parts);
    }

    private void SetCommodityOutput(ISolid material, ITag tag, string weightExpression)
    {
        _itemProtoId = 0;
        _commodityMaterialId = material?.Id ?? 0;
        _commodityTagId = tag?.Id ?? 0;
        _commodityWeightExpression = weightExpression;
        Changed = true;
    }

    private void ClearCommodityOutput()
    {
        _commodityMaterialId = 0;
        _commodityTagId = 0;
        _commodityWeightExpression = null;
        Changed = true;
    }

    private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What dice expression do you want to use for quantity when this item is foraged?");
            return false;
        }

        string diceExpression = command.SafeRemainingArgument;
        TraitExpression testExpression = new(diceExpression, Gameworld);
        if (testExpression.HasErrors())
        {
            actor.OutputHandler.Send($"Your formula had the following error: {testExpression.Error.ColourCommand()}");
            return false;
        }

        QuantityDiceExpression = diceExpression;
        actor.OutputHandler.Send(
            $"When foraged, this foragable will now yield {QuantityDiceExpression.Colour(Telnet.Yellow)} items.");
        return true;
    }

    private bool BuildingCommandProto(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Which item prototype do you want to load when someone forages this foragable?");
            return false;
        }

        string targetText = command.SafeRemainingArgument;
        IGameItemProto proto = Gameworld.ItemProtos.GetByIdOrName(targetText);

        if (proto == null)
        {
            actor.Send("There is no such item prototype for you to use.");
            return false;
        }

        if (proto.Status != RevisionStatus.Current)
        {
            actor.Send("You may only use item prototypes with a status of current.");
            return false;
        }

        if (proto.ReadOnly)
        {
            actor.Send("Read only item prototypes may not be used in foragables.");
            return false;
        }

        ItemProto = proto;
        ClearCommodityOutput();
        actor.OutputHandler.Send(
            $"This foragable will now load item prototype {proto.Name} (#{proto.Id}) when foraged.");
        return true;
    }

    private bool BuildingCommandCanForage(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("You must either specify the prog, or {0} to clear an existing prog.",
                "clear".Colour(Telnet.Yellow));
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("clear"))
        {
            if (CanForageProg == null)
            {
                actor.Send("That foragable does not have a CanForage Prog to clear.");
                return false;
            }

            CanForageProg = null;
            actor.Send("You clear the CanForage prog from this foragable. It will now always be foragable.");
            return true;
        }

        string targetText = command.SafeRemainingArgument;
        IFutureProg prog = Gameworld.FutureProgs.GetByIdOrName(targetText);

        if (prog == null)
        {
            actor.Send("There is no such prog for you to set as the CanForage prog for this foragable.");
            return false;
        }

        if (prog.ReturnType != ProgVariableTypes.Boolean)
        {
            actor.Send("Only progs that return a boolean can be used for the CanForage prog. {0} returns {1}.",
                prog.FunctionName.Colour(Telnet.Yellow), prog.ReturnType.Describe().Colour(Telnet.Cyan));
            return false;
        }

        if (
            !prog.MatchesParameters(new List<ProgVariableTypes>
            {
                ProgVariableTypes.Character,
                ProgVariableTypes.Number
            }))
        {
            actor.Send(
                "The CanForage prog must accept a Character and a Number parameter. {0} does not match that pattern.",
                prog.FunctionName);
            return false;
        }

        CanForageProg = prog;
        actor.Send("This foragable will now use the {0} prog to determine who can forage it.", prog.FunctionName);
        return true;
    }

    private bool BuildingCommandOnForage(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("You must either specify the prog, or {0} to clear an existing prog.",
                "clear".Colour(Telnet.Yellow));
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("clear"))
        {
            if (OnForageProg == null)
            {
                actor.Send("That foragable does not have a OnForage Prog to clear.");
                return false;
            }

            OnForageProg = null;
            actor.Send("You clear the OnForage prog from this foragable.");
            return true;
        }

        string targetText = command.SafeRemainingArgument;
        IFutureProg prog = Gameworld.FutureProgs.GetByIdOrName(targetText);

        if (prog == null)
        {
            actor.Send("There is no such prog for you to set as the OnForage prog for this foragable.");
            return false;
        }

        if (
            !prog.MatchesParameters(new List<ProgVariableTypes>
            {
                ProgVariableTypes.Character,
                ProgVariableTypes.Number,
                ProgVariableTypes.Item,
                ProgVariableTypes.Number
            }))
        {
            actor.Send(
                "The OnForage prog must accept a Character, Number, Item and Number parameter. The final number is item quantity or commodity weight. {0} does not match that pattern.",
                prog.FunctionName);
            return false;
        }

        OnForageProg = prog;
        actor.Send("This foragable will now execute the {0} prog when it is foraged.", prog.FunctionName);
        return true;
    }

    private bool BuildingCommandChance(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What relative chance do you want to give this foragable to be foraged?");
            return false;
        }

        if (!int.TryParse(command.PopSpeech(), out int value))
        {
            actor.Send("You must enter a number for the relative chance.");
            return false;
        }

        if (value < 1)
        {
            actor.Send("You must enter a number greater than zero.");
            return false;
        }

        RelativeChance = value;
        actor.Send("This foragable now has a {0} relative chance to be foraged.",
            RelativeChance.ToString("N0", actor).Colour(Telnet.Green));
        return true;
    }

    private bool BuildingCommandOutcome(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send(
                "You must specify a minimum outcome for individuals to successfully forage this item. Use {0} to make it always foragable.",
                Outcome.MajorFail.DescribeColour());
            return false;
        }

        if (!command.PopSpeech().TryParseEnum<Outcome>(out Outcome minimumOutcome))
        {
            actor.Send("That is not a valid minimum outcome.");
            return false;
        }


        Outcome maximumOutcome;
        if (!command.IsFinished)
        {
            if (!command.PopSpeech().TryParseEnum<Outcome>(out maximumOutcome))
            {
                actor.Send("That is not a valid maximum outcome.");
                return false;
            }
        }
        else
        {
            maximumOutcome = minimumOutcome;
        }

        if (minimumOutcome > maximumOutcome)
        {
            actor.Send("The minimum outcome must be less than or equal to the maximum outcome.");
            return false;
        }

        MinimumOutcome = minimumOutcome;
        MaximumOutcome = maximumOutcome;

        actor.Send(
            "Foragers will now require a minimum outcome of {0} and a maximum outcome of {1} to forage this item.",
            MinimumOutcome.DescribeColour(), MaximumOutcome.DescribeColour());
        return true;
    }

    private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send(
                "You must specify a difficulty for individuals to specifically forage for this item. Use {0} if you do not wish it to be specifiable.",
                "impossible".Colour(Telnet.Cyan));
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out Difficulty difficulty))
        {
            actor.Send("That is not a valid difficulty.");
            return false;
        }

        ForageDifficulty = difficulty;
        actor.Send("It will now be {0} to specifically forage for this item.",
            difficulty.Describe().Colour(Telnet.Cyan));
        return true;
    }

    private bool BuildingCommandTypes(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("You must enter the types that you want to set, separated by spaces.");
            return false;
        }

        command = new StringStack(command.RemainingArgument);
        command.PopSpeechAll();
        List<string> choices = command.Memory
                                       .Select(x => x.Trim().ToLowerInvariant())
                                       .Where(x => !string.IsNullOrWhiteSpace(x))
                                       .Distinct(StringComparer.InvariantCultureIgnoreCase)
                                       .ToList();
        if (!choices.Any())
        {
            actor.Send("You must enter at least one non-blank forage type.");
            return false;
        }

        actor.Send("This foragable can now be found using the keywords {0}",
            choices.Select(x => x.Colour(Telnet.Green)).ListToString(conjunction: "or "));
        List<string> existing =
            Gameworld.Foragables.SelectMany(x => x.ForagableTypes)
                     .Select(x => x.ToLowerInvariant())
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Distinct()
                     .ToList();
        List<string> newChoices =
            choices.Where(x => !existing.Any(y => y.Equals(x, StringComparison.InvariantCultureIgnoreCase))).ToList();
        if (newChoices.Any())
        {
            actor.Send(
                "Warning: Options {0} have not been used before. Check that this is correct.".Colour(Telnet.Yellow),
                newChoices.ListToString());
        }

        _foragableTypes.Clear();
        _foragableTypes.AddRange(choices);
        Changed = true;
        return true;
    }

    private bool BuildingCommandName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("You must specify a name to set for this foragable.");
            return false;
        }

        _name = command.SafeRemainingArgument;
        Changed = true;
        actor.Send("You set the name of this foragable to {0}", Name.Colour(Telnet.Green));
        return true;
    }

    #endregion

    #region IForagable Members

    private List<string> _foragableTypes = new();
    public IEnumerable<string> ForagableTypes => _foragableTypes;

    private string SerialisedForagableTypes =>
        ForagableTypes.Where(x => !string.IsNullOrWhiteSpace(x))
                      .Select(x => x.Trim().ToLowerInvariant())
                      .Distinct(StringComparer.InvariantCultureIgnoreCase)
                      .ListToString(separator: ",", conjunction: "", twoItemJoiner: ",");

    private static List<string> ParseForagableTypes(string types)
    {
        return (types ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                      .Where(x => !string.IsNullOrWhiteSpace(x))
                                      .Select(x => x.ToLowerInvariant())
                                      .Distinct(StringComparer.InvariantCultureIgnoreCase)
                                      .ToList();
    }

    private Difficulty _forageDifficulty;

    public Difficulty ForageDifficulty
    {
        get => _forageDifficulty;
        set
        {
            _forageDifficulty = value;
            Changed = true;
        }
    }

    private int _relativeChance;

    public int RelativeChance
    {
        get => _relativeChance;
        set
        {
            _relativeChance = value;
            Changed = true;
        }
    }

    private Outcome _minimumOutcome;

    public Outcome MinimumOutcome
    {
        get => _minimumOutcome;
        set
        {
            _minimumOutcome = value;
            Changed = true;
        }
    }

    private Outcome _maximumOutcome;

    public Outcome MaximumOutcome
    {
        get => _maximumOutcome;
        set
        {
            _maximumOutcome = value;
            Changed = true;
        }
    }

    private string _quantityDiceExpression;

    public string QuantityDiceExpression
    {
        get => _quantityDiceExpression;
        set
        {
            _quantityDiceExpression = value;
            Changed = true;
        }
    }

    private long _commodityMaterialId;

    public ISolid CommodityMaterial
    {
        get => Gameworld.Materials.Get(_commodityMaterialId);
        set
        {
            _commodityMaterialId = value?.Id ?? 0;
            Changed = true;
        }
    }

    private long _commodityTagId;

    public ITag CommodityTag
    {
        get => Gameworld.Tags.Get(_commodityTagId);
        set
        {
            _commodityTagId = value?.Id ?? 0;
            Changed = true;
        }
    }

    private string _commodityWeightExpression;

    public string CommodityWeightExpression
    {
        get => _commodityWeightExpression;
        set
        {
            _commodityWeightExpression = value;
            Changed = true;
        }
    }

    private long _itemProtoId;

    public IGameItemProto ItemProto
    {
        get => Gameworld.ItemProtos.Get(_itemProtoId);
        set
        {
            _itemProtoId = value?.Id ?? 0;
            Changed = true;
        }
    }

    private IFutureProg _onForageProg;

    public IFutureProg OnForageProg
    {
        get => _onForageProg;
        set
        {
            _onForageProg = value;
            Changed = true;
        }
    }

    private IFutureProg _canForageProg;

    public IFutureProg CanForageProg
    {
        get => _canForageProg;
        set
        {
            _canForageProg = value;
            Changed = true;
        }
    }

    public bool CanForage(ICharacter character, Outcome outcome)
    {
        return
            (MaximumOutcome == Outcome.None || outcome <= MaximumOutcome) &&
            (MinimumOutcome == Outcome.None || outcome >= MinimumOutcome) &&
            (CanForageProg == null || (CanForageProg.ExecuteBool(character, Id)));
    }

    #endregion
}
