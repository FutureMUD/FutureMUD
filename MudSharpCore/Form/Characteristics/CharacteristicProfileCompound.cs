using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Form.Characteristics;

public class CharacteristicProfileCompound : CharacteristicProfile
{
	private readonly List<long> _profileIdList = new();
	private readonly List<string> _profileNameList = new();
	private readonly List<ICharacteristicProfile> _profiles = new();

	public CharacteristicProfileCompound(MudSharp.Models.CharacteristicProfile profile, IFuturemud gameworld)
		: base(profile, gameworld)
	{
	}

	public CharacteristicProfileCompound(string name, ICharacteristicDefinition definition) : base(name, definition,
		"Compound", "<Definition/>")
	{
	}

	protected CharacteristicProfileCompound(CharacteristicProfileCompound rhs, string newName) : base(rhs, newName)
	{
	}

	public override ICharacteristicProfile Clone(string newName)
	{
		return new CharacteristicProfileCompound(this, newName);
	}

	public bool DependsOnProfile(ICharacteristicProfile profile)
	{
		return profile == this ||
		       Profiles.Any(x => x == profile) ||
		       Profiles.OfType<CharacteristicProfileCompound>().Any(x => x.DependsOnProfile(profile));
	}

	protected List<ICharacteristicProfile> Profiles
	{
		get
		{
			if (!_profiles.Any())
			{
				_profiles.AddRange(Gameworld.CharacteristicProfiles.Where(x =>
					_profileIdList.Contains(x.Id) || _profileNameList.Any(y => x.Name.EqualTo(y))));
			}

			return _profiles;
		}
	}

	public override IEnumerable<ICharacteristicValue> Values
	{
		get { return Profiles.SelectMany(x => x.Values).Distinct().WhereNotNull(x => x).ToList(); }
	}

	public override string HelpText => @"You can use the following options when editing this profile:

	#3name <name>#0 - changes the name of this profile
	#3desc <description>#0 - changes the description of this profile
	#3profile <which>#0 - toggles a parent profile for this one";

	public override void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "value":
				actor.OutputHandler.Send("Profiles of type 'Compound' can't manually add or remove values.");
				return;
			case "profile":
				BuildingCommandProfile(actor, command);
				return;
		}

		base.BuildingCommand(actor, command.GetUndo());
	}

	private void BuildingCommandProfile(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which other profile do you want to toggle this one inheriting values from?");
			return;
		}

		var profile = Gameworld.CharacteristicProfiles.GetByIdOrName(command.SafeRemainingArgument);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such characteristic profile.");
			return;
		}

		if (Profiles.Contains(profile))
		{
			_profiles.Remove(profile);
			_profileIdList.Clear();
			_profileNameList.Clear();
			_profileIdList.AddRange(_profiles.Select(x => x.Id));
			Changed = true;
			actor.OutputHandler.Send(
				$"The {profile.Name.ColourName()} profile will no longer give values for this one to inherit from.");
			return;
		}

		// Check for loops
		if (DependsOnProfile(profile))
		{
			actor.OutputHandler.Send("Doing that would create a loop, which is forbidden.");
			return;
		}

		_profiles.Add(profile);
		_profileIdList.Clear();
		_profileNameList.Clear();
		_profileIdList.AddRange(_profiles.Select(x => x.Id));
		Changed = true;
		actor.OutputHandler.Send($"This profile will now inherit values from the {profile.Name.ColourName()} profile.");
	}

	protected override void LoadFromXml(XElement root)
	{
		foreach (var item in root.Elements("Profile"))
		{
			if (long.TryParse(item.Value, out var value))
			{
				_profileIdList.Add(value);
			}
			else
			{
				_profileNameList.Add(item.Value);
			}
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			from item in _profileIdList
			select new XElement("Profile", item)
		).ToString();
	}
}