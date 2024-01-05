using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

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
		using (new FMDB())
		{
			var dbitem = new Models.MagicGenerator
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
			var dbitem = new Models.MagicGenerator
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
		var root = XElement.Parse(generator.Definition);
		var element = root.Element("WhichResource");
		if (element == null)
		{
			throw new ApplicationException(
				$"LinearTimeBasedGenerator #{Id} ({Name}) is missing a WhichResource element.");
		}

		WhichResource = long.TryParse(element.Value, out var value)
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

		if (!double.TryParse(element.Value, out var dvalue))
		{
			throw new ApplicationException(
				$"LinearTimeBasedGenerator #{Id} ({Name}) specified an AboutPerMinute element that wasn't a number.");
		}

		AmountPerMinute = dvalue;
	}

	public IMagicResource WhichResource { get; set; }
	public double AmountPerMinute { get; set; }

	#region Overrides of BaseMagicResourceGenerator

	protected override HeartbeatManagerDelegate InternalGetOnMinuteDelegate(IHaveMagicResource thing)
	{
		return () => { thing.AddResource(WhichResource, AmountPerMinute); };
	}

	/// <inheritdoc />
	public override IEnumerable<IMagicResource> GeneratedResources => new[] { WhichResource };

	protected override XElement SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("WhichResource", WhichResource.Id),
			new XElement("AmountPerMinute", AmountPerMinute)
		);
	}

	protected override string SubtypeHelpText => @"	#3resource <which>#0 - sets the resource gained
	#3amount <##>#0 - sets the amount of resource gained per minute";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
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

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
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

		var resource = Gameworld.MagicResources.GetByIdOrName(command.SafeRemainingArgument);
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
		var sb = new StringBuilder();
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