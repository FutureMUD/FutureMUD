using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using System;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Combat;

public class ExplosiveRangedAttack : RangedNaturalAttackBase, IExplosiveRangedAttack
{
    public ExplosiveRangedAttack(Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld)
    {
    }

    public ExplosiveRangedAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
    {
        ExplosionSize = SizeCategory.Normal;
        MaximumProximity = Proximity.Proximate;
    }

    public SizeCategory ExplosionSize { get; set; }
    public Proximity MaximumProximity { get; set; }

    protected override void LoadFromXElement(XElement root)
    {
        ExplosionSize = Enum.TryParse<SizeCategory>(root.Element("ExplosionSize")?.Value ?? "", true, out SizeCategory size)
            ? size
            : SizeCategory.Normal;
        MaximumProximity = Enum.TryParse<Proximity>(root.Element("MaximumProximity")?.Value ?? "", true, out Proximity proximity)
            ? proximity
            : Proximity.Proximate;
    }

    protected override void SaveToXml(XElement root)
    {
        root.Add(
            new XElement("ExplosionSize", ExplosionSize.ToString()),
            new XElement("MaximumProximity", MaximumProximity.ToString())
        );
    }

    public override string HelpText => $@"{base.HelpText}
	#3explosionsize <size>#0 - sets the reference size of the resulting explosion
	#3proximity <proximity>#0 - sets the maximum proximity affected by the explosion";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "explosionsize":
            case "size":
                return BuildingCommandSize(actor, command);
            case "proximity":
                return BuildingCommandProximity(actor, command);
        }

        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandSize(ICharacter actor, StringStack command)
    {
        if (!command.SafeRemainingArgument.TryParseEnum<SizeCategory>(out SizeCategory value))
        {
            actor.OutputHandler.Send("That is not a valid size category.");
            return false;
        }

        ExplosionSize = value;
        Changed = true;
        actor.OutputHandler.Send($"This attack now detonates as a {ExplosionSize.DescribeEnum().ColourValue()} explosion.");
        return true;
    }

    private bool BuildingCommandProximity(ICharacter actor, StringStack command)
    {
        if (!command.SafeRemainingArgument.TryParseEnum<Proximity>(out Proximity value))
        {
            actor.OutputHandler.Send("That is not a valid proximity.");
            return false;
        }

        MaximumProximity = value;
        Changed = true;
        actor.OutputHandler.Send($"This attack now affects targets out to {MaximumProximity.DescribeEnum().ColourValue()} proximity.");
        return true;
    }

    protected override string ShowBuilderInternal(ICharacter actor)
    {
        StringBuilder sb = new(base.ShowBuilderInternal(actor));
        sb.AppendLine($"Explosion Size: {ExplosionSize.DescribeEnum().ColourValue()}");
        sb.AppendLine($"Maximum Proximity: {MaximumProximity.DescribeEnum().ColourValue()}");
        return sb.ToString();
    }
}
