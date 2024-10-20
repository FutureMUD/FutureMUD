using System.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Form.Characteristics;

public class ClientCharacteristicDefinition : CharacteristicDefinition
{
	private ICharacteristicDefinition _parent;

	public ClientCharacteristicDefinition(string name, string pattern, string description,
		ICharacteristicDefinition baseDefinition, IFuturemud gameworld)
		: base(name, pattern, description, baseDefinition.Type, gameworld)
	{
		_parent = baseDefinition;
	}

	public ClientCharacteristicDefinition(MudSharp.Models.CharacteristicDefinition definition, IFuturemud gameworld)
		: base(definition, gameworld)
	{
		_parent = gameworld.Characteristics.Get(definition.ParentId ?? 0L);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.CharacteristicDefinitions.Find(Id);
		dbitem.ParentId = _parent?.Id;
		base.Save();
	}

	public override ICharacteristicDefinition Parent => _parent;

	public override string HelpText => $@"{base.HelpText}
	#3parent <other>#0 - sets the parent of this definition";

	public override void BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PopSpeech().ToLowerInvariant().EqualTo("parent"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					"What other characteristic definition would you like to set as this one's parent?");
				return;
			}

			var name = command.SafeRemainingArgument;
			ICharacteristicDefinition definition = null;
			definition = Gameworld.Characteristics.GetByIdOrName(name);

			if (definition == null)
			{
				actor.OutputHandler.Send("There is no such definition.");
				return;
			}

			// Check for loops
			var proposed = definition.Parent;
			while (proposed != null)
			{
				if (proposed.Parent == this)
				{
					actor.OutputHandler.Send(
						$"Setting the parent definition in that way would create a loop, which is prohibited.");
					return;
				}

				proposed = proposed.Parent;
			}

			_parent = definition;
			Changed = true;
			actor.OutputHandler.Send(
				$"You set the parent of this characteristic definition to {_parent.Name.ColourName()} ID #{Id.ToString("N0", actor)}.");
			return;
		}

		base.BuildingCommand(actor, command.GetUndo());
	}

	public override string ToString()
	{
		return $"ClientCharacteristicDefinition ID {Id} Name: {Name} Description: {Description} Parent: {Parent.Name}";
	}
}