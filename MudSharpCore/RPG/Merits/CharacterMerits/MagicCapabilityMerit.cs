using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MagicCapabilityMerit : CharacterMeritBase, IMagicCapabilityMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Magic Capability",
			(merit, gameworld) => new MagicCapabilityMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Magic Capability", (gameworld, name) => new MagicCapabilityMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Magic Capability", "Gives a character a magic capability", new MagicCapabilityMerit().HelpText);
	}

	protected MagicCapabilityMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		foreach (var item in root.Element("Capabilities").Elements())
		{
			var capability = gameworld.MagicCapabilities.Get(long.Parse(item.Value));
			if (capability != null)
			{
				_capabilities.Add(capability);
			}
		}
	}

	protected MagicCapabilityMerit(){}

	protected MagicCapabilityMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Magic Capability", "@ have|has magic capability")
	{
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("Capabilities",
			from item in _capabilities
			select new XElement("Capability", item.Id)
		));
		return root;
	}

	private readonly List<IMagicCapability> _capabilities = new();
	public IEnumerable<IMagicCapability> Capabilities => _capabilities;

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Magic Capabilities: {_capabilities.Select(x => x.Name.Colour(x.School.PowerListColour)).ListToString()}");
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3capability <which>#0 - toggles a capability being given by this merit";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "capability":
				return BuildingCommandCapability(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandCapability(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which magic capability do you want to toggle being given by this merit?");
			return false;
		}

		var capability = Gameworld.MagicCapabilities.GetByIdOrName(command.SafeRemainingArgument);
		if (capability is null)
		{
			actor.OutputHandler.Send($"There is no magic capability identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		Changed = true;
		if (_capabilities.Remove(capability))
		{
			actor.OutputHandler.Send($"This merit no longer gives the {capability.Name.Colour(capability.School.PowerListColour)} magic capability.");
			return true;
		}

		_capabilities.Add(capability);
		actor.OutputHandler.Send($"This merit now gives the {capability.Name.Colour(capability.School.PowerListColour)} magic capability.");
		return true;
	}
}