using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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

namespace MudSharp.Magic.Powers;

public class MagicArmourPower : SustainedMagicPower
{
	public override string PowerType => "Armour";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("armour", (power, gameworld) => new MagicArmourPower(power, gameworld));
		MagicPowerFactory.RegisterLoader("armor", (power, gameworld) => new MagicArmourPower(power, gameworld));
	}

    protected override XElement SaveDefinition()
    {
        var definition = new XElement("Definition",
            new XElement("BeginVerb", BeginVerb),
            new XElement("EndVerb", EndVerb),
            new XElement("SkillCheckDifficulty", (int)SkillCheckDifficulty),
            new XElement("SkillCheckTrait", SkillCheckTrait.Id),
            new XElement("ArmourAppliesProg", ArmourAppliesProg.Id),
            new XElement("EmoteText", new XCData(EmoteText)),
            new XElement("FailEmoteText", new XCData(FailEmoteText)),
            new XElement("EndPowerEmoteText", new XCData(EndPowerEmoteText)),
			new XElement("Quality", (int)Quality),
			new XElement("ArmourType", ArmourType?.Id ?? 0L),
			new XElement("ArmourMaterial", ArmourMaterial?.Id ?? 0L),
			new XElement("FullDescriptionAddendum", new XCData(FullDescriptionAddendum)),
			new XElement("CanBeObscuredByInventory", ArmourCanBeObscuredByInventory),
			new XElement("MaximumDamageAbsorbed", new XCData(MaximumDamageAbsorbed.OriginalFormulaText)),
            new XElement("BodypartShapes",
                from shape in _coveredShapes
                select new XElement("Shape",
                    shape.Id
                )
            )
        );
        SaveSustainedDefinition(definition);
        return definition;
    }

    protected MagicArmourPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		BeginVerb = root.Element("BeginVerb")?.Value ??
		            throw new ApplicationException(
			            $"MagicArmourPower ID #{Id} ({Name}) did not have a BeginVerb element.");
		EndVerb = root.Element("EndVerb")?.Value ??
		          throw new ApplicationException(
			          $"MagicArmourPower ID #{Id} ({Name}) did not have an EndVerb element.");
		var element = root.Element("EmoteText");
		if (element == null)
		{
			throw new ApplicationException($"There was no EmoteText in the definition XML for power {Id} ({Name}).");
		}

		EmoteText = element.Value;

		element = root.Element("BodypartShapes");
		if (element != null)
		{
			foreach (var sub in element.Elements())
			{
				var shape = long.TryParse(sub.Value, out var idvalue)
					? Gameworld.BodypartShapes.Get(idvalue)
					: Gameworld.BodypartShapes.GetByName(sub.Value);
				if (shape != null)
				{
					_coveredShapes.Add(shape);
				}
			}
		}

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

		element = root.Element("ArmourAppliesProg");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no ArmourAppliesProg in the definition XML for power {Id} ({Name}).");
		}

		ArmourAppliesProg = long.TryParse(element.Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);

		Quality = (ItemQuality)int.Parse(root.Element("Quality")?.Value ?? "0");
		ArmourType = (long.TryParse(root.Element("ArmourType")?.Value ?? "0", out value)
			             ? Gameworld.ArmourTypes.Get(value)
			             : Gameworld.ArmourTypes.GetByName(root.Element("ArmourType")?.Value ?? "")) ??
		             throw new ApplicationException($"Invalid armour type in MagicArmourPower #{Id} ({Name})")
			;
		ArmourMaterial = (long.TryParse(root.Element("ArmourMaterial")?.Value ?? "0", out value)
			                 ? Gameworld.Materials.Get(value) as ISolid
			                 : Gameworld.Materials.GetByName(root.Element("ArmourMaterial")?.Value ?? "") as ISolid
		                 ) ??
		                 throw new ApplicationException(
			                 $"Invalid armour material in MagicArmourPower #{Id} ({Name})");
		FullDescriptionAddendum = root.Element("FullDescriptionAddendum")?.Value ?? string.Empty;
		ArmourCanBeObscuredByInventory = bool.Parse(root.Element("CanBeObscuredByInventory")?.Value ?? "false");
        MaximumDamageAbsorbed = new TraitExpression(root.Element("MaximumDamageAbsorbed")?.Value ?? "0", Gameworld);
    }

	#region Overrides of MagicPowerBase

	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, verb);
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

		var check = Gameworld.GetCheck(CheckType.MagicArmourPower);
		var outcome = check.Check(actor, SkillCheckDifficulty, SkillCheckTrait, null);
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
	public IArmourType ArmourType { get; protected set; }
	public ItemQuality Quality { get; protected set; }
	public ISolid ArmourMaterial { get; protected set; }
	public string BeginVerb { get; protected set; }
	public string EndVerb { get; protected set; }

	public IFutureProg ArmourAppliesProg { get; protected set; }

	public ITraitExpression MaximumDamageAbsorbed { get; protected set; }

	public override IEnumerable<string> Verbs => new[] { BeginVerb, EndVerb };

	private readonly HashSet<IBodypartShape> _coveredShapes = new();

	public bool AppliesToBodypart(IBodypart bodypart)
	{
		if (!_coveredShapes.Any())
		{
			return true;
		}

		return _coveredShapes.Contains(bodypart.Shape);
	}

	public string FullDescriptionAddendum { get; protected set; }
	public bool ArmourCanBeObscuredByInventory { get; protected set; }

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
        sb.AppendLine($"Full Desc Addendum: {FullDescriptionAddendum.ColourCommand()}");
        sb.AppendLine($"Covered Shapes: {(_coveredShapes.Any() ? _coveredShapes.Select(x => x.Name.ColourValue()).ListToString() : "All".ColourValue())}");
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
    #3threshold <outcome>#0 - sets the minimum outcome for skill success";

    /// <inheritdoc />
    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
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

        var skill = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
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

        var verb = command.SafeRemainingArgument.ToLowerInvariant();
        if (BeginVerb.EqualTo(verb))
        {
            actor.OutputHandler.Send("The begin and verb cannot be the same.");
            return false;
        }

        var costs = InvocationCosts[EndVerb].ToList();
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

        var verb = command.SafeRemainingArgument.ToLowerInvariant();
        if (EndVerb.EqualTo(verb))
        {
            actor.OutputHandler.Send("The begin and verb cannot be the same.");
            return false;
        }

        var costs = InvocationCosts[BeginVerb].ToList();
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