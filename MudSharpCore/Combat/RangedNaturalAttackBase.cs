using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Combat;

public abstract class RangedNaturalAttackBase : WeaponAttack, IRangedNaturalAttack
{
    protected RangedNaturalAttackBase(Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld)
    {
    }

    protected RangedNaturalAttackBase(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
    {
        RangeInRooms = 1;
        ScatterType = RangedScatterType.Arcing;
    }

    public int RangeInRooms { get; set; }
    public RangedScatterType ScatterType { get; set; }

    protected override void LoadFromDatabase(Models.WeaponAttack attack)
    {
        base.LoadFromDatabase(attack);
        XElement root = string.IsNullOrWhiteSpace(attack.AdditionalInfo)
            ? new XElement("Data")
            : XElement.Parse(attack.AdditionalInfo);
        RangeInRooms = int.Parse(root.Element("RangeInRooms")?.Value ?? "1");
        ScatterType = Enum.TryParse<RangedScatterType>(root.Element("ScatterType")?.Value ?? "", true, out RangedScatterType scatterType)
            ? scatterType
            : RangedScatterType.Arcing;
        LoadFromXElement(root);
    }

    protected virtual void LoadFromXElement(XElement root)
    {
    }

    protected override void SeedInitialData(Models.WeaponAttack attack)
    {
        attack.AdditionalInfo = SaveToXml().ToString();
    }

    protected override void AddAttackSpecificCloneData(Models.WeaponAttack attack)
    {
        attack.AdditionalInfo = SaveToXml().ToString();
    }

    protected override void SaveAttackSpecificData(Models.WeaponAttack attack)
    {
        attack.AdditionalInfo = SaveToXml().ToString();
    }

    protected virtual XElement SaveToXml()
    {
        XElement root = new("Data",
            new XElement("RangeInRooms", RangeInRooms),
            new XElement("ScatterType", ScatterType.ToString()));
        SaveToXml(root);
        return root;
    }

    protected virtual void SaveToXml(XElement root)
    {
    }

    public override string HelpText => $@"{base.HelpText}
	#3range <rooms>#0 - sets the maximum effective range for this ranged natural attack
	#3scatter <arcing|ballistic|light|spread>#0 - sets the scatter model for misses";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "range":
                return BuildingCommandRange(actor, command);
            case "scatter":
            case "scattertype":
                return BuildingCommandScatter(actor, command);
        }

        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandRange(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How many rooms of range should this attack have?");
            return false;
        }

        if (!int.TryParse(command.SafeRemainingArgument, out int value) || value < 0)
        {
            actor.OutputHandler.Send("You must enter a non-negative integer.");
            return false;
        }

        RangeInRooms = value;
        Changed = true;
        actor.OutputHandler.Send($"This attack now has a range of {RangeInRooms.ToString("N0", actor).ColourValue()} room(s).");
        return true;
    }

    private bool BuildingCommandScatter(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which scatter type should this attack use?");
            return false;
        }

        if (!Enum.TryParse<RangedScatterType>(command.SafeRemainingArgument, true, out RangedScatterType value))
        {
            actor.OutputHandler.Send(
                $"That is not a valid scatter type. Valid values are {Enum.GetValues<RangedScatterType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
            return false;
        }

        ScatterType = value;
        Changed = true;
        actor.OutputHandler.Send($"This attack now uses {ScatterType.DescribeEnum().ColourValue()} scatter.");
        return true;
    }

    protected override string ShowBuilderInternal(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Range: {RangeInRooms.ToString("N0", actor).ColourValue()} room(s)");
        sb.AppendLine($"Scatter Type: {ScatterType.DescribeEnum().ColourValue()}");
        sb.Append(base.ShowBuilderInternal(actor));
        return sb.ToString();
    }

    protected override void DescribeForAttacksCommandInternal(StringBuilder sb, ICharacter actor)
    {
        base.DescribeForAttacksCommandInternal(sb, actor);
        sb.Append($" Ranged {RangeInRooms.ToString("N0", actor).ColourValue()}r");
    }
}
