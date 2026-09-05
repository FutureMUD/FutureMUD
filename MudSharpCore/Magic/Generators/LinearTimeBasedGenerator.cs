using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework.Scheduling;
using MudSharp.Models;

namespace MudSharp.Magic.Generators;

public class LinearTimeBasedGenerator : BaseMagicResourceGenerator
{
    public override IMagicResourceRegenerator Clone(string name)
    {
        return new LinearTimeBasedGenerator(this, name);
    }

    protected LinearTimeBasedGenerator(LinearTimeBasedGenerator rhs, string newName) : base(rhs.Gameworld, newName)
    {
        WhichResource = rhs.WhichResource;
        AmountPerMinute = rhs.AmountPerMinute;
		ConsciousOnly = rhs.ConsciousOnly;
		RestMultiplier = rhs.RestMultiplier;
        using (new FMDB())
        {
            MagicGenerator dbitem = new()
            {
                Name = newName,
                Type = "linear",
                Definition = SaveDefinition().ToString()
            };
            FMDB.Context.MagicGenerators.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }

    public LinearTimeBasedGenerator(IFuturemud gameworld, string name, IMagicResource resource) : base(gameworld, name)
    {
        WhichResource = resource;
        AmountPerMinute = 1.0;
        using (new FMDB())
        {
            MagicGenerator dbitem = new()
            {
                Name = name,
                Type = "linear",
                Definition = SaveDefinition().ToString()
            };
            FMDB.Context.MagicGenerators.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }

    public LinearTimeBasedGenerator(Models.MagicGenerator generator, IFuturemud gameworld) : base(generator, gameworld)
    {
        XElement root = XElement.Parse(generator.Definition);
		ConsciousOnly = (bool?)root.Element("ConsciousOnly") ?? false;
		RestMultiplier = (double?)root.Element("RestMultiplier") ?? 1.0;
		if (!double.IsFinite(RestMultiplier) || RestMultiplier < 0 || RestMultiplier > 10) throw new ApplicationException("Invalid regenerator rest multiplier.");
        XElement element = root.Element("WhichResource");
        if (element == null)
        {
            throw new ApplicationException(
                $"LinearTimeBasedGenerator #{Id} ({Name}) is missing a WhichResource element.");
        }

        WhichResource = long.TryParse(element.Value, out long value)
            ? gameworld.MagicResources.Get(value)
            : gameworld.MagicResources.GetByName(element.Value);
        if (WhichResource == null)
        {
            throw new ApplicationException(
                $"LinearTimeBasedGenerator #{Id} ({Name}) specified an incorrect magic resource.");
        }

        element = root.Element("AmountPerMinute");
        if (element == null)
        {
            throw new ApplicationException(
                $"LinearTimeBasedGenerator #{Id} ({Name}) is missing a AmountPerMinute element.");
        }

        if (!double.TryParse(element.Value, out double dvalue))
        {
            throw new ApplicationException(
                $"LinearTimeBasedGenerator #{Id} ({Name}) specified an AboutPerMinute element that wasn't a number.");
        }

        AmountPerMinute = dvalue;
    }

    public IMagicResource WhichResource { get; set; }
    public double AmountPerMinute { get; set; }
	public bool ConsciousOnly { get; private set; }
	public double RestMultiplier { get; private set; } = 1;

    #region Overrides of BaseMagicResourceGenerator

    protected override HeartbeatManagerDelegate InternalGetOnMinuteDelegate(IHaveMagicResource thing)
    {
		return () =>
		{
			if (ConsciousOnly && (thing is not ICharacter conscious || !conscious.State.IsConscious())) return;
			var resting = thing is ICharacter ch && ch.Combat is null && ch.Movement is null &&
				ch.PositionState.Name.EqualToAny("Sitting", "Lying Down", "Lounging", "Reclining");
			thing.AddResource(WhichResource, AmountPerMinute * (resting ? RestMultiplier : 1));
		};
    }

    /// <inheritdoc />
    public override IEnumerable<IMagicResource> GeneratedResources => new[] { WhichResource };

    protected override XElement SaveDefinition()
    {
        return new XElement("Definition",
            new XElement("WhichResource", WhichResource.Id),
            new XElement("AmountPerMinute", AmountPerMinute),
			new XElement("ConsciousOnly", ConsciousOnly), new XElement("RestMultiplier", RestMultiplier)
        );
    }

    protected override string SubtypeHelpText => @"	#3resource <which>#0 - sets the resource gained
	#3amount <##>#0 - sets the amount of resource gained per minute
	#3conscious#0 - toggles requiring a conscious character
	#3restmultiplier <0-10>#0 - changes regeneration while resting out of combat";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
			case "conscious":
				ConsciousOnly = !ConsciousOnly;
				Changed = true;
				actor.Send($"Conscious characters only: {ConsciousOnly.ToColouredString()}.");
				return true;
			case "restmultiplier":
				if (!double.TryParse(command.SafeRemainingArgument, out var multiplier) || !double.IsFinite(multiplier) || multiplier < 0 || multiplier > 10) return false;
				RestMultiplier = multiplier;
				Changed = true;
				actor.Send($"Rest multiplier: {multiplier.ToString("N2", actor).ColourValue()}.");
				return true;
            case "resource":
                return BuildingCommandResource(actor, command);
            case "amount":
                return BuildingCommandAmount(actor, command);
        }
        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandAmount(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"How many {WhichResource.Name.ColourValue()} should be regenerated per minute?");
            return false;
        }

        if (!double.TryParse(command.SafeRemainingArgument, out double value))
        {
            actor.OutputHandler.Send("That is not a valid number.");
            return false;
        }

        AmountPerMinute = value;
        Changed = true;
        actor.OutputHandler.Send($"This regenerator now regenerates {AmountPerMinute.ToString("N3", actor).ColourValue()} {WhichResource.Name.Pluralise().ColourValue()} per minute.");
        return true;
    }

    private bool BuildingCommandResource(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which resource do you want to change this regenerator to producing?");
            return false;
        }

        IMagicResource resource = Gameworld.MagicResources.GetByIdOrName(command.SafeRemainingArgument);
        if (resource is null)
        {
            actor.OutputHandler.Send("There is no such resource.");
            return false;
        }

        WhichResource = resource;
        Changed = true;
        actor.OutputHandler.Send($"This regenerator will now produce the {resource.Name.ColourValue()} magic resource.");
        return true;
    }

    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Magic Regenerator #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.BoldMagenta, Telnet.BoldWhite));
        sb.AppendLine();
        sb.AppendLine($"Type: #2Linear Gain Per Minute#0".SubstituteANSIColour());
        sb.AppendLine($"Resource: {WhichResource.Name.ColourName()}");
        sb.AppendLine($"Amount Per Minute: {AmountPerMinute.ToString("N3", actor).ColourValue()}");
        return sb.ToString();
    }

    public override string RegeneratorTypeName => "Simple Linear";

    #endregion
}
