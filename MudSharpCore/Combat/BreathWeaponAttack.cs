using MudSharp.Character;
using MudSharp.Framework;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Combat;

public class BreathWeaponAttack : RangedNaturalAttackBase, IBreathWeaponAttack
{
    public BreathWeaponAttack(Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld)
    {
    }

    public BreathWeaponAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
    {
        AdditionalTargetLimit = 3;
        BodypartsHitPerTarget = 2;
        FireProfile = new FireProfile(gameworld);
        IgniteChance = 0.0;
    }

    public int AdditionalTargetLimit { get; set; }
    public int BodypartsHitPerTarget { get; set; }
    public IFireProfile FireProfile { get; set; }
    public double IgniteChance { get; set; }

    protected override void LoadFromXElement(XElement root)
    {
        AdditionalTargetLimit = int.Parse(root.Element("AdditionalTargetLimit")?.Value ?? "0");
        BodypartsHitPerTarget = int.Parse(root.Element("BodypartsHitPerTarget")?.Value ?? "1");
        IgniteChance = double.Parse(root.Element("IgniteChance")?.Value ?? "0");
        FireProfile = root.Element("FireProfile") is XElement fireRoot ? new FireProfile(fireRoot, Gameworld) : null;
    }

    protected override void SaveToXml(XElement root)
    {
        root.Add(
            new XElement("AdditionalTargetLimit", AdditionalTargetLimit),
            new XElement("BodypartsHitPerTarget", BodypartsHitPerTarget),
            new XElement("IgniteChance", IgniteChance)
        );
        if (FireProfile is FireProfile fireProfile)
        {
            root.Add(fireProfile.SaveToXml());
        }
    }

    public override string HelpText => $@"{base.HelpText}
	#3targets <number>#0 - sets how many extra nearby targets can be hit
	#3bodyparts <number>#0 - sets how many bodyparts are struck per victim
	#3ignite <percentage>#0 - sets the ignition chance applied by this breath weapon";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "targets":
                return BuildingCommandTargets(actor, command);
            case "bodyparts":
                return BuildingCommandBodyparts(actor, command);
            case "ignite":
            case "ignitechance":
                return BuildingCommandIgnite(actor, command);
        }

        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandTargets(ICharacter actor, StringStack command)
    {
        if (!int.TryParse(command.SafeRemainingArgument, out int value) || value < 0)
        {
            actor.OutputHandler.Send("You must enter a non-negative integer.");
            return false;
        }

        AdditionalTargetLimit = value;
        Changed = true;
        actor.OutputHandler.Send($"This breath attack can now affect {AdditionalTargetLimit.ToString("N0", actor).ColourValue()} additional target(s).");
        return true;
    }

    private bool BuildingCommandBodyparts(ICharacter actor, StringStack command)
    {
        if (!int.TryParse(command.SafeRemainingArgument, out int value) || value <= 0)
        {
            actor.OutputHandler.Send("You must enter a positive integer.");
            return false;
        }

        BodypartsHitPerTarget = value;
        Changed = true;
        actor.OutputHandler.Send($"This breath attack now strikes {BodypartsHitPerTarget.ToString("N0", actor).ColourValue()} bodypart(s) per target.");
        return true;
    }

    private bool BuildingCommandIgnite(ICharacter actor, StringStack command)
    {
        if (!command.SafeRemainingArgument.TryParsePercentage(out double value))
        {
            actor.OutputHandler.Send("You must enter a valid percentage.");
            return false;
        }

        IgniteChance = value;
        Changed = true;
        actor.OutputHandler.Send($"This breath attack now has an ignition chance of {IgniteChance.ToString("P2", actor).ColourValue()}.");
        return true;
    }

    protected override string ShowBuilderInternal(ICharacter actor)
    {
        StringBuilder sb = new(base.ShowBuilderInternal(actor));
        sb.AppendLine($"Additional Targets: {AdditionalTargetLimit.ToString("N0", actor).ColourValue()}");
        sb.AppendLine($"Bodyparts Per Target: {BodypartsHitPerTarget.ToString("N0", actor).ColourValue()}");
        sb.AppendLine($"Ignite Chance: {IgniteChance.ToString("P2", actor).ColourValue()}");
        sb.AppendLine($"Fire Profile: {FireProfile?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}");
        return sb.ToString();
    }
}
