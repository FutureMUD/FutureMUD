using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public class MagicArmourPower : SustainedMagicPower
{
    public override string PowerType => "Armour";
    public override string DatabaseType => "armour";
    public static void RegisterLoader()
    {
        MagicPowerFactory.RegisterLoader("armour", (power, gameworld) => new MagicArmourPower(power, gameworld));
        MagicPowerFactory.RegisterLoader("armor", (power, gameworld) => new MagicArmourPower(power, gameworld));
        MagicPowerFactory.RegisterBuilderLoader("armour", (gameworld, school, name, actor, command) =>
        {
            if (command.IsFinished)
            {
                actor.OutputHandler.Send("Which skill do you want to use for the skill check?");
                return null;
            }

            ITraitDefinition skill = gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
            if (skill is null)
            {
                actor.OutputHandler.Send("There is no such skill or attribute.");
                return null;
            }

            return new MagicArmourPower(gameworld, school, name, skill);
        });
    }

    protected override XElement SaveDefinition()
    {
        XElement definition = new("Definition",
                new XElement("BeginVerb", BeginVerb),
    new XElement("EndVerb", EndVerb),
    new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
    new XElement("SkillCheckTrait", SkillCheckTrait.Id),
    new XElement("EmoteText", new XCData(EmoteText)),
    new XElement("FailEmoteText", new XCData(FailEmoteText)),
    new XElement("EndPowerEmoteText", new XCData(EndPowerEmoteText))
        );
        ArmourConfiguration.SaveToXml(definition);
        AddBaseDefinition(definition);
        SaveSustainedDefinition(definition);
        return definition;
    }

    private MagicArmourPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name)
    {
        Blurb = "Create magical armour";
        _showHelpText = @$"You can use #3{school.SchoolVerb.ToUpperInvariant()} ARMOUR#0 to invoke your armour and #3{school.SchoolVerb.ToUpperInvariant()} ENDARMOUR#0 to end this armour effect."; ;
        BeginVerb = "armour";
        EndVerb = "endarmour";
        SkillCheckTrait = trait;
        SkillCheckDifficulty = Difficulty.VeryEasy;
        MinimumSuccessThreshold = Outcome.Fail;
        ConcentrationPointsToSustain = 1.0;
        ArmourConfiguration = new MagicArmourConfiguration(Gameworld);
        ArmourAppliesProg = Gameworld.AlwaysTrueProg;
        Quality = ItemQuality.Standard;
        ArmourType = Gameworld.ArmourTypes.First();
        ArmourCanBeObscuredByInventory = false;
        ArmourMaterial = Gameworld.Materials.First();
        MaximumDamageAbsorbed = new TraitExpression($"skill:{trait.Id} * 5", Gameworld);
        EmoteText = "@ are|is surrounded by a shimmering veil of protective magic.";
        FailEmoteText = "@ are|is briefly surrounded by a shimmering veil of protective magic, but it quickly fades.";
        EndPowerEmoteText = "The shimmering veil of protective magic surrounding @ shatters and disappears.";
        FullDescriptionAddendum = string.Empty;
        DoDatabaseInsert();
    }

    protected MagicArmourPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
    {
        XElement root = XElement.Parse(power.Definition);
        BeginVerb = root.Element("BeginVerb")?.Value ??
                    throw new ApplicationException(
                        $"MagicArmourPower ID #{Id} ({Name}) did not have a BeginVerb element.");
        EndVerb = root.Element("EndVerb")?.Value ??
                  throw new ApplicationException(
                      $"MagicArmourPower ID #{Id} ({Name}) did not have an EndVerb element.");
        XElement element = root.Element("EmoteText");
        if (element == null)
        {
            throw new ApplicationException($"There was no EmoteText in the definition XML for power {Id} ({Name}).");
        }

        EmoteText = element.Value;

        element = root.Element("FailEmoteText");
        if (element == null)
        {
            throw new ApplicationException(
                $"There was no FailEmoteText in the definition XML for power {Id} ({Name}).");
        }

        FailEmoteText = element.Value;

        element = root.Element("EndPowerEmoteText");
        if (element == null)
        {
            throw new ApplicationException(
                $"There was no EndPowerEmoteText in the definition XML for power {Id} ({Name}).");
        }

        EndPowerEmoteText = element.Value;

        element = root.Element("SkillCheckDifficulty");
        if (element == null)
        {
            throw new ApplicationException(
                $"There was no SkillCheckDifficulty in the definition XML for power {Id} ({Name}).");
        }

        SkillCheckDifficulty = (Difficulty)int.Parse(element.Value);

        element = root.Element("SkillCheckTrait");
        if (element == null)
        {
            throw new ApplicationException(
                $"There was no SkillCheckTrait in the definition XML for power {Id} ({Name}).");
        }

        SkillCheckTrait = Gameworld.Traits.Get(long.Parse(element.Value));

        element = root.Element("MinimumSuccessThreshold");
        if (element == null)
        {
            throw new ApplicationException(
                $"There was no MinimumSuccessThreshold in the definition XML for power {Id} ({Name}).");
        }

        MinimumSuccessThreshold = (Outcome)int.Parse(element.Value);
        ArmourConfiguration = new MagicArmourConfiguration(root, Gameworld);
    }

    #region Overrides of MagicPowerBase

    public override void UseCommand(ICharacter actor, string verb, StringStack command)
    {
        (bool truth, IMagicResource missing) = CanAffordToInvokePower(actor, verb);
        if (!truth)
        {
            actor.OutputHandler.Send(
                $"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
            return;
        }

        if (verb.EqualTo(EndVerb))
        {
            if (!actor.AffectedBy<MagicArmour>(this))
            {
                actor.OutputHandler.Send("You are not currently using that power.");
                return;
            }

            actor.RemoveAllEffects<MagicArmour>(x => x.Power == this, true);
            actor.OutputHandler.Handle(new EmoteOutput(new Emote(EndPowerEmoteText, actor, actor)));
            ConsumePowerCosts(actor, verb);
            return;
        }

        if (actor.AffectedBy<MagicArmour>(this))
        {
            actor.OutputHandler.Send("You are already using that power.");
            return;
        }

        if (CanInvokePowerProg?.Execute<bool?>(actor) == false)
        {
            actor.OutputHandler.Send(WhyCantInvokePowerProg?.Execute<string>(actor) ?? "You cannot use that power.");
            return;
        }

        ICheck check = Gameworld.GetCheck(CheckType.MagicArmourPower);
        CheckOutcome outcome = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait, null);
        if (outcome < MinimumSuccessThreshold)
        {
            actor.OutputHandler.Handle(new EmoteOutput(new Emote(FailEmoteText, actor, actor)));
            return;
        }

        actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor)));
        actor.AddEffect(new MagicArmour(actor, this), GetDuration(outcome.SuccessDegrees()));
        ConsumePowerCosts(actor, verb);
    }

    public string EmoteText { get; protected set; }
    public string FailEmoteText { get; protected set; }
    public string EndPowerEmoteText { get; protected set; }
    public Difficulty SkillCheckDifficulty { get; protected set; }
    public ITraitDefinition SkillCheckTrait { get; protected set; }
    public Outcome MinimumSuccessThreshold { get; protected set; }
    public MagicArmourConfiguration ArmourConfiguration { get; protected set; }
    public IArmourType ArmourType
    {
        get => ArmourConfiguration.ArmourType;
        protected set => ArmourConfiguration.ArmourType = value;
    }
    public ItemQuality Quality
    {
        get => ArmourConfiguration.Quality;
        protected set => ArmourConfiguration.Quality = value;
    }
    public ISolid ArmourMaterial
    {
        get => ArmourConfiguration.ArmourMaterial;
        protected set => ArmourConfiguration.ArmourMaterial = value;
    }
    public string BeginVerb { get; protected set; }
    public string EndVerb { get; protected set; }

    public IFutureProg ArmourAppliesProg
    {
        get => ArmourConfiguration.ArmourAppliesProg;
        protected set => ArmourConfiguration.ArmourAppliesProg = value;
    }

    public ITraitExpression MaximumDamageAbsorbed
    {
        get => ArmourConfiguration.MaximumDamageAbsorbed;
        protected set => ArmourConfiguration.MaximumDamageAbsorbed = value;
    }

    public override IEnumerable<string> Verbs => new[] { BeginVerb, EndVerb };

    public bool AppliesToBodypart(IBodypart bodypart)
    {
        return ArmourConfiguration.AppliesToBodypart(bodypart);
    }

    public string FullDescriptionAddendum
    {
        get => ArmourConfiguration.FullDescriptionAddendum;
        protected set => ArmourConfiguration.FullDescriptionAddendum = value;
    }
    public bool ArmourCanBeObscuredByInventory
    {
        get => ArmourConfiguration.ArmourCanBeObscuredByInventory;
        protected set => ArmourConfiguration.ArmourCanBeObscuredByInventory = value;
    }

    /// <inheritdoc />
    protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
    {
        sb.AppendLine($"Begin Verb: {BeginVerb.ColourCommand()}");
        sb.AppendLine($"End Verb: {EndVerb.ColourCommand()}");
        sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
        sb.AppendLine($"Skill Check Difficulty: {SkillCheckDifficulty.DescribeColoured()}");
        sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
        sb.AppendLine($"Armour Applies Prog: {ArmourAppliesProg.MXPClickableFunctionName()}");
        sb.AppendLine($"Maximum Damage Absorbed: {MaximumDamageAbsorbed.OriginalFormulaText.ColourCommand()}");
        sb.AppendLine($"Armour Type: {ArmourType.Name.ColourValue()}");
        sb.AppendLine($"Armour Quality: {Quality.Describe().ColourValue()}");
        sb.AppendLine($"Armour Material: {ArmourMaterial.Name.ColourValue()}");
        sb.AppendLine($"Can Be Obscured By Inventory: {ArmourCanBeObscuredByInventory.ToColouredString()}");
        sb.AppendLine($"Full Desc Addendum: {FullDescriptionAddendum.SubstituteANSIColour()}");
        sb.AppendLine($"Covered Shapes: {(ArmourConfiguration.CoveredShapes.Any() ? ArmourConfiguration.CoveredShapes.Select(x => x.Name.ColourValue()).ListToString() : "All".ColourValue())}");
        sb.AppendLine();
        sb.AppendLine("Emotes:");
        sb.AppendLine();
        sb.AppendLine($"Emote: {EmoteText.ColourCommand()}");
        sb.AppendLine($"Fail Emote: {FailEmoteText.ColourCommand()}");
        sb.AppendLine($"End Emote: {EndPowerEmoteText.ColourCommand()}");
    }

    #endregion

    #region Overrides of SustainedMagicPower

    protected override void ExpireSustainedEffect(ICharacter actor)
    {
        actor.RemoveAllEffects<MagicArmour>(x => x.Power == this, true);
        actor.OutputHandler.Handle(new EmoteOutput(new Emote(EndPowerEmoteText, actor, actor)));
    }

    #endregion

    #region Building Commands
    /// <inheritdoc />
    protected override string SubtypeHelpText => @"	#3begin <verb>#0 - sets the verb to activate this power
	#3end <verb>#0 - sets the verb to end this power
	#3skill <which>#0 - sets the skill used in the skill check
	#3difficulty <difficulty>#0 - sets the difficulty of the skill check
	#3threshold <outcome>#0 - sets the minimum outcome for skill success
	#3part <which>#0 - toggles a bodypart shape being covered
	#3part all#0 - sets the armour as applying to all bodyparts
	#3obscured#0 - toggles armour being hidden by items worn on the bodypart
	#3desc <text>#0 - sets a full desc addendum when the armour affect applies
	#3desc none#0 - clears a full desc addendum
	#3absorb <formula>#0 - sets the formula for maximum damage absorbed
	#3applies <prog>#0 - sets a prog for whether the armour applies
	#3armourtype <type>#0 - sets the armour type
	#3armourmaterial <material>#0 - sets the armour material
	#3armourquality <quality>#0 - sets the armour quality
	#3emote <emote>#0 - sets the power use emote. $0 is the power user
	#3failemote <emote>#0 - sets the fail emote for power use. $0 is the power user
	#3endemote <emote>#0 - sets the emote for ending the power. $0 is the power user";

    /// <inheritdoc />
    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "part":
            case "bodypart":
            case "shape":
                return BuildingCommandShape(actor, command);
            case "obscured":
                return BuildingCommandObscured(actor);
            case "applies":
                return BuildingCommandApplies(actor, command);
            case "armourtype":
                return BuildingCommandArmourType(actor, command);
            case "armourmaterial":
                return BuildingCommandArmourMaterial(actor, command);
            case "armourquality":
                return BuildingCommandArmourQuality(actor, command);
            case "absorb":
                return BuildingCommandAbsorb(actor, command);
            case "desc":
            case "description":
            case "addendum":
            case "descaddendum":
            case "descriptionaddendum":
                return BuildingCommandDescriptionAddendum(actor, command);
            case "failemote":
                return BuildingCommandFailEmote(actor, command);
            case "endemote":
                return BuildingCommandEndEmote(actor, command);
            case "emote":
                return BuildingCommandEmote(actor, command);
            case "beginverb":
            case "begin":
            case "startverb":
            case "start":
                return BuildingCommandBeginVerb(actor, command);
            case "endverb":
            case "end":
            case "cancelverb":
            case "cancel":
                return BuildingCommandEndVerb(actor, command);
            case "skill":
            case "trait":
                return BuildingCommandSkill(actor, command);
            case "difficulty":
                return BuildingCommandDifficulty(actor, command);
            case "threshold":
                return BuildingCommandThreshold(actor, command);
        }
        return base.BuildingCommand(actor, command.GetUndo());
    }


    #region Building Subcommands

    private bool BuildingCommandShape(ICharacter actor, StringStack command)
    {
        if (command.SafeRemainingArgument.EqualTo("all"))
        {
            actor.OutputHandler.Send("This armour now applies to all bodyparts of the owner.");
            ArmourConfiguration.CoveredShapes.Clear();
            Changed = true;
            return true;
        }

        IBodypartShape shape = Gameworld.BodypartShapes.GetByIdOrName(command.SafeRemainingArgument);
        if (shape is null)
        {
            actor.OutputHandler.Send($"There is no shape identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
            return false;
        }

        Changed = true;
        if (ArmourConfiguration.CoveredShapes.Remove(shape))
        {
            actor.OutputHandler.Send($"This armour no longer covers {shape.Name.ColourValue()} bodypart shapes.");
        }
        else
        {
            ArmourConfiguration.CoveredShapes.Add(shape);
            actor.OutputHandler.Send($"This armour now covers {shape.Name.ColourValue()} bodypart shapes.");
        }

        return true;
    }

    private bool BuildingCommandObscured(ICharacter actor)
    {
        ArmourCanBeObscuredByInventory = !ArmourCanBeObscuredByInventory;
        Changed = true;
        actor.OutputHandler.Send($"This magical armour can {ArmourCanBeObscuredByInventory.NowNoLonger()} be obscured by worn items.");
        return true;
    }

    private bool BuildingCommandApplies(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What prog do you want to use for whether the armour applies?");
            return false;
        }

        IFutureProg prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
            [
                [ProgVariableTypes.Character],
                [ProgVariableTypes.Character, ProgVariableTypes.Character]
            ]
        ).LookupProg();
        if (prog is null)
        {
            return false;
        }

        ArmourAppliesProg = prog;
        Changed = true;
        actor.OutputHandler.Send($"The prog {prog.MXPClickableFunctionName()} will now control whether the armour effect applies.");
        return true;
    }

    private bool BuildingCommandArmourType(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which armour type should this magical armour be?");
            return false;
        }

        IArmourType type = Gameworld.ArmourTypes.GetByIdOrName(command.SafeRemainingArgument);
        if (type is null)
        {
            actor.OutputHandler.Send($"There is no such armour type identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
            return false;
        }

        ArmourType = type;
        Changed = true;
        actor.OutputHandler.Send($"This magic armour effect now uses the armour type {type.Name.ColourValue()}.");
        return true;
    }

    private bool BuildingCommandArmourMaterial(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which armour material should this magical armour be?");
            return false;
        }

        ISolid material = Gameworld.Materials.GetByIdOrName(command.SafeRemainingArgument);
        if (material is null)
        {
            actor.OutputHandler.Send($"There is no such material identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
            return false;
        }

        ArmourMaterial = material;
        Changed = true;
        actor.OutputHandler.Send($"This magic armour effect now uses the armour material {material.Name.ColourValue()}.");
        return true;
    }

    private bool BuildingCommandArmourQuality(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What effective quality should this magical armour effect be?");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum<ItemQuality>(out ItemQuality value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid item quality. See {"show qualities".MXPSend()} for a list of valid qualities.");
            return false;
        }

        Quality = value;
        Changed = true;
        actor.OutputHandler.Send($"This magical armour now has an effective armour quality of {value.DescribeEnum().ColourValue()}.");
        return true;
    }

    private bool BuildingCommandAbsorb(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What should be the formula for how much damage this magical armour can absorb?\nSee {"te".MXPSend()} for a list of possible functions that can be used.");
            return false;
        }

        TraitExpression te = new(command.SafeRemainingArgument, Gameworld);
        if (te.HasErrors())
        {
            actor.OutputHandler.Send(te.Error);
            return false;
        }

        MaximumDamageAbsorbed = te;
        Changed = true;
        actor.OutputHandler.Send($"The formula for how much damage this armour can absorb is now {command.SafeRemainingArgument.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandDescriptionAddendum(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must either specify a description addendum or use #3none#0 to clear one.".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            FullDescriptionAddendum = string.Empty;
            Changed = true;
            actor.OutputHandler.Send("This magical armour will no longer apply a description addendum.");
            return true;
        }

        FullDescriptionAddendum = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"This magical armour now applies the following description addendum:\n\n{FullDescriptionAddendum.SubstituteANSIColour()}");
        return true;
    }

    private bool BuildingCommandFailEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What do you want to set the power fail use emote to?");
            return false;
        }

        Emote emote = new(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }

        FailEmoteText = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"The power fail use emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandEndEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What do you want to set the power end emote to?");
            return false;
        }

        Emote emote = new(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }

        EndPowerEmoteText = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"The power end emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What do you want to set the power use emote to?");
            return false;
        }

        Emote emote = new(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }

        EmoteText = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"The power use emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What is the minimum success threshold for this power to work? See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum(out Outcome value))
        {
            actor.OutputHandler.Send($"That is not a valid outcome. See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
            return false;
        }

        MinimumSuccessThreshold = value;
        Changed = true;
        actor.OutputHandler.Send($"The power user will now need to achieve a {value.DescribeColour()} in order to activate this power.");
        return true;
    }

    private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What difficulty should the skill check for this power be? See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
        {
            actor.OutputHandler.Send($"That is not a valid difficulty. See {"show difficulties".MXPSend("show difficulties")} for a list of values.");
            return false;
        }

        SkillCheckDifficulty = value;
        Changed = true;
        actor.OutputHandler.Send($"This power's skill check will now be at a difficulty of {value.DescribeColoured()}.");
        return true;
    }

    private bool BuildingCommandSkill(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which skill or trait should be used for this power's skill check?");
            return false;
        }

        ITraitDefinition skill = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
        if (skill is null)
        {
            actor.OutputHandler.Send("That is not a valid skill or trait.");
            return false;
        }

        SkillCheckTrait = skill;
        Changed = true;
        actor.OutputHandler.Send($"This magic power will now use the {skill.Name.ColourName()} skill for its skill check.");
        return true;
    }

    private bool BuildingCommandEndVerb(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which verb should be used to end this power when active?");
            return false;
        }

        string verb = command.SafeRemainingArgument.ToLowerInvariant();
        if (BeginVerb.EqualTo(verb))
        {
            actor.OutputHandler.Send("The begin and verb cannot be the same.");
            return false;
        }

        List<(IMagicResource Resource, double Cost)> costs = InvocationCosts[EndVerb].ToList();
        InvocationCosts[verb] = costs;
        InvocationCosts.Remove(EndVerb);
        EndVerb = verb;
        Changed = true;
        actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to end the power.");
        return true;
    }

    private bool BuildingCommandBeginVerb(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which verb should be used to activate this power?");
            return false;
        }

        string verb = command.SafeRemainingArgument.ToLowerInvariant();
        if (EndVerb.EqualTo(verb))
        {
            actor.OutputHandler.Send("The begin and verb cannot be the same.");
            return false;
        }

        List<(IMagicResource Resource, double Cost)> costs = InvocationCosts[BeginVerb].ToList();
        InvocationCosts[verb] = costs;
        InvocationCosts.Remove(BeginVerb);
        BeginVerb = verb;
        Changed = true;
        actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to begin the power.");
        return true;
    }
    #endregion Building Subcommands
    #endregion Building Commands
}
