using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Form.Characteristics;

/// <summary>
///     A CharacterProfileAll is a form of CharacterProfile which always dynamically includes all values of its target
///     definition, with an even spread of randomness
/// </summary>
public class CharacterProfileAll : CharacteristicProfile
{
	public CharacterProfileAll(MudSharp.Models.CharacteristicProfile profile, IFuturemud gameworld)
		: base(profile, gameworld)
	{
	}

	protected CharacterProfileAll(CharacterProfileAll rhs, string newName) : base(rhs, newName)
	{
	}

	public CharacterProfileAll(string name, ICharacteristicDefinition definition) : base(name, definition, "All",
		"<Definition/>")
	{
	}

	public override ICharacteristicProfile Clone(string newName)
	{
		return new CharacterProfileAll(this, newName);
	}

	public override IEnumerable<ICharacteristicValue> Values
	{
		get { return Gameworld.CharacteristicValues.Where(x => TargetDefinition.IsValue(x)).ToList(); }
	}

	public override string Type => "All";

	public override ICharacteristicValue GetCharacteristic(string value)
	{
		return
			Gameworld.CharacteristicValues.FirstOrDefault(
				x =>
					TargetDefinition.IsValue(x) &&
					string.Equals(x.Name, value, StringComparison.InvariantCultureIgnoreCase));
	}

	public override ICharacteristicValue GetRandomCharacteristic()
	{
		return Gameworld.CharacteristicValues.Where(x => TargetDefinition.IsValue(x)).ToList().GetRandomElement();
	}

	public override string HelpText => @"You can use the following options when editing this profile:

	name <name> - changes the name of this profile
	desc <description> - changes the description of this profile";

	public override void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "value":
				actor.OutputHandler.Send("Profiles of type 'All' can't manually add or remove values.");
				return;
		}

		base.BuildingCommand(actor, command.GetUndo());
	}

	protected override void LoadFromXml(XElement root)
	{
		// Do nothing
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}
}