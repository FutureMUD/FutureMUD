using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Form.Characteristics;

public class CharacteristicProfile : FrameworkItem, ICharacteristicProfile
{
	protected readonly List<ICharacteristicValue> _values = new();

	public CharacteristicProfile(MudSharp.Models.CharacteristicProfile profile, IFuturemud gameworld)
	{
		_id = profile.Id;
		_name = profile.Name;
		Description = profile.Description;
		Gameworld = gameworld;
		TargetDefinition = gameworld.Characteristics.Get(profile.TargetDefinitionId);
		LoadFromXml(XElement.Parse(profile.Definition));
	}

	public CharacteristicProfile(string name, ICharacteristicDefinition target, string type, string definition = null)
	{
		using (new FMDB())
		{
			var dbitem = new Models.CharacteristicProfile
			{
				Name = name,
				Description = "An undescribed profile",
				TargetDefinitionId = target.Id,
				Type = type,
				Definition = definition ?? "<Values/>"
			};
			FMDB.Context.CharacteristicProfiles.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			_name = name;
			Description = dbitem.Description;
			TargetDefinition = target;
			Gameworld = TargetDefinition.Gameworld;
			LoadFromXml(XElement.Parse(dbitem.Definition));
		}
	}

	protected CharacteristicProfile(CharacteristicProfile rhs, string newName)
	{
		using (new FMDB())
		{
			var dbitem = new Models.CharacteristicProfile
			{
				Name = newName,
				Description = rhs.Description,
				TargetDefinitionId = rhs.TargetDefinition.Id,
				Type = rhs.Type.ToLowerInvariant(),
				Definition = rhs.SaveToXml()
			};
			FMDB.Context.CharacteristicProfiles.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			_name = newName;
			Description = rhs.Description;
			Gameworld = rhs.Gameworld;
			TargetDefinition = rhs.TargetDefinition;
			LoadFromXml(XElement.Parse(dbitem.Definition));
		}
	}

	public sealed override string FrameworkItemType => "CharacteristicProfile";

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; protected set; }

	#endregion

	protected virtual void LoadFromXml(XElement root)
	{
		foreach (var value in root.Elements("Value"))
		{
			var item = long.TryParse(value.Value, out var val)
				? Gameworld.CharacteristicValues.Get(val)
				: Gameworld.CharacteristicValues.Where(x => TargetDefinition.IsValue(x))
				           .FirstOrDefault(x => x.Name.EqualTo(value.Value));
			if (item != null)
			{
				_values.Add(item);
			}
		}
	}

	public static ICharacteristicProfile LoadProfile(MudSharp.Models.CharacteristicProfile profile,
		IFuturemud gameworld)
	{
		switch (profile.Type.ToLowerInvariant())
		{
			case "standard":
				return new CharacteristicProfile(profile, gameworld);
			case "all":
				return new CharacterProfileAll(profile, gameworld);
			case "compound":
				return new CharacteristicProfileCompound(profile, gameworld);
			case "weighted":
				return new WeightedCharacteristicProfile(profile, gameworld);
			default:
				throw new NotImplementedException();
		}
	}

	#region ICharacteristicProfile Members

	public virtual ICharacteristicProfile Clone(string newName)
	{
		return new CharacteristicProfile(this, newName);
	}

	public virtual IEnumerable<ICharacteristicValue> Values => _values;

	public ICharacteristicDefinition TargetDefinition { get; protected set; }

	public string Description { get; protected set; }
	public virtual string Type => "Standard";

	public bool ContainsCharacteristic(ICharacteristicValue value)
	{
		return Values.Contains(value);
	}

	public virtual void ExpireCharacteristic(ICharacteristicValue value)
	{
		_values.Remove(value);
		Changed = true;
	}

	public virtual ICharacteristicValue GetCharacteristic(string value)
	{
		return Values.FirstOrDefault(x => string.Equals(x.Name, value, StringComparison.InvariantCultureIgnoreCase));
	}

	public virtual ICharacteristicValue GetRandomCharacteristic()
	{
		return Values.GetRandomElement();
	}

	public virtual ICharacteristicValue GetRandomCharacteristic(ICharacterTemplate template)
	{
		return Values
		       .Where(x => x.ChargenApplicabilityProg?.Execute<bool?>(template) != false)
		       .GetRandomElement();
	}

	public virtual ICharacteristicValue GetRandomCharacteristic(ICharacter character)
	{
		return Values
		       .Where(x => x.ChargenApplicabilityProg?.Execute<bool?>(character) != false)
		       .GetRandomElement();
	}

	public bool IsProfileFor(ICharacteristicDefinition definition)
	{
		return definition != null && (definition == TargetDefinition || IsProfileFor(definition.Parent));
	}

	public virtual string HelpText => @"You can use the following options when editing this profile:

	name <name> - changes the name of this profile
	desc <description> - changes the description of this profile
	value <which> - toggles a value being a part of this profile";

	public virtual void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				BuildingCommandName(actor, command);
				return;
			case "desc":
			case "description":
				BuildingCommandDescription(actor, command);
				return;
			case "value":
				BuildingCommandValue(actor, command);
				return;
			default:
				actor.OutputHandler.Send(HelpText);
				return;
		}
	}

	private void BuildingCommandValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic value do you want to toggle for this profile?");
			return;
		}

		var value = Gameworld.CharacteristicValues.GetByIdOrName(command.SafeRemainingArgument);
		if (value == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value.");
			return;
		}

		if (!TargetDefinition.IsValue(value))
		{
			actor.OutputHandler.Send(
				$"The value {value.Name.ColourName()} is not a valid value for the {TargetDefinition.Name.ColourName()} definition.");
			return;
		}

		if (_values.Contains(value))
		{
			_values.Remove(value);
			actor.OutputHandler.Send($"The value {value.Name.ColourName()} is no longer an option for this profile.");
		}
		else
		{
			_values.Add(value);
			actor.OutputHandler.Send($"The value {value.Name.ColourName()} is now an option for this profile.");
		}

		Changed = true;
	}

	private void BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to set for this profile?");
			return;
		}

		Description = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"You change the description of this profile to {Description.ColourCommand()}.");
	}

	private void BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this profile?");
			return;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.CharacteristicProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a characteristic profile with that name. Names must be unique.");
			return;
		}

		actor.OutputHandler.Send(
			$"You rename the characteristic profile previously known as {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
	}

	public virtual string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Characteristic Profile #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Definition: {TargetDefinition.Name.ColourValue()}");
		sb.AppendLine($"Type: {Type.TitleCase().ColourValue()}");
		sb.AppendLine($"Description: {Description.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Values:");
		foreach (var value in Values)
		{
			sb.AppendLine($"\t[{value.Id.ToString("N0", actor)}] {value.Name.ColourValue()}");
		}

		return sb.ToString();
	}

	#endregion

	#region ISaveable Members

	protected bool NoSave = false;
	private bool _changed;

	public bool Changed
	{
		get => _changed;
		set
		{
			if (value && !_changed && !NoSave)
			{
				Gameworld.SaveManager.Add(this);
			}

			_changed = value;
		}
	}

	protected virtual string SaveToXml()
	{
		return new XElement("Definition", new object[]
			{
				from value in _values
				select new XElement("Value", value.Id)
			})
			.ToString();
	}

	public void Save()
	{
		using (new FMDB())
		{
			var dbprofile = FMDB.Context.CharacteristicProfiles.Find(Id);
			dbprofile.Name = Name;
			dbprofile.Description = Description;
			dbprofile.TargetDefinitionId = TargetDefinition.Id;
			dbprofile.Definition = SaveToXml();
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion
}