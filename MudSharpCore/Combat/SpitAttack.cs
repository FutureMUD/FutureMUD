using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Combat;

public class SpitAttack : RangedNaturalAttackBase, ISpitAttack
{
    public SpitAttack(Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld)
    {
    }

    public SpitAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
    {
        Liquid = Gameworld.Liquids.First();
        MaximumQuantity = 0.01 / Gameworld.UnitManager.BaseFluidToLitres;
    }

    public ILiquid Liquid { get; set; }
    public double MaximumQuantity { get; set; }

    protected override void LoadFromXElement(XElement root)
    {
        Liquid = Gameworld.Liquids.Get(long.Parse(root.Element("Liquid")?.Value ?? "0"));
        MaximumQuantity = double.Parse(root.Element("MaximumQuantity")?.Value ?? "0");
    }

    protected override void SaveToXml(XElement root)
    {
        root.Add(
            new XElement("Liquid", Liquid?.Id ?? 0),
            new XElement("MaximumQuantity", MaximumQuantity)
        );
    }

    public override string HelpText => $@"{base.HelpText}
	#3liquid <liquid>#0 - sets the contaminating liquid
	#3amount <volume>#0 - sets the maximum quantity delivered on a clean hit";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "liquid":
                return BuildingCommandLiquid(actor, command);
            case "amount":
            case "quantity":
                return BuildingCommandAmount(actor, command);
        }

        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandLiquid(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which liquid should this spit attack apply?");
            return false;
        }

        ILiquid liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
        if (liquid is null)
        {
            actor.OutputHandler.Send("There is no such liquid.");
            return false;
        }

        Liquid = liquid;
        Changed = true;
        actor.OutputHandler.Send($"This spit attack will now apply {Liquid.Name.Colour(Liquid.DisplayColour)}.");
        return true;
    }

    private bool BuildingCommandAmount(ICharacter actor, StringStack command)
    {
        double amount = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume, out bool success);
        if (!success)
        {
            actor.OutputHandler.Send("That is not a valid fluid volume.");
            return false;
        }

        MaximumQuantity = amount;
        Changed = true;
        actor.OutputHandler.Send(
            $"This spit attack will now deliver up to {Gameworld.UnitManager.DescribeMostSignificantExact(MaximumQuantity, UnitType.FluidVolume, actor).ColourValue()}.");
        return true;
    }

    protected override string ShowBuilderInternal(ICharacter actor)
    {
        StringBuilder sb = new(base.ShowBuilderInternal(actor));
        sb.AppendLine($"Liquid: {Liquid?.Name.Colour(Liquid?.DisplayColour ?? Telnet.Cyan) ?? "None".Colour(Telnet.Red)}");
        sb.AppendLine($"Maximum Quantity: {Gameworld.UnitManager.DescribeMostSignificantExact(MaximumQuantity, UnitType.FluidVolume, actor).ColourValue()}");
        return sb.ToString();
    }
}
