using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Form.Characteristics;

public class CharacteristicDefinition : FrameworkItem, ICharacteristicDefinition
{
	public CharacteristicDefinition(string name, string pattern, string description, CharacteristicType type,
		IFuturemud gameworld, ICharacteristicDefinition parent = null, XElement definition = null)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbdef = new Models.CharacteristicDefinition
			{
				Name = name,
				Pattern = pattern,
				Description = description,
				Type = (int)type,
				ParentId = parent?.Id,
				ChargenDisplayType = Type == CharacteristicType.Coloured
					? (int)CharacterGenerationDisplayType.GroupByBasic
					: (int)CharacterGenerationDisplayType.DisplayAll,
				Definition = definition?.ToString() ?? string.Empty
			};
			FMDB.Context.CharacteristicDefinitions.Add(dbdef);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbdef);
			Gameworld.Add(this);

			var dbprof = new Models.CharacteristicProfile
			{
				Name = $"All {Name.Pluralise()}",
				Type = "all",
				Definition = "<Definition/>",
				TargetDefinitionId = _id,
				Description = $"All Defined {Name} Values"
			};
			FMDB.Context.CharacteristicProfiles.Add(dbprof);
			FMDB.Context.SaveChanges();
			var prof = CharacteristicProfile.LoadProfile(dbprof, Gameworld);
			Gameworld.Add(prof);
		}
	}

	public CharacteristicDefinition(MudSharp.Models.CharacteristicDefinition definition, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDatabase(definition);
	}

	public sealed override string FrameworkItemType => "CharacteristicDefinition";

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; protected set; }

	#endregion

	protected virtual void LoadFromDatabase(MudSharp.Models.CharacteristicDefinition definition)
	{
		_id = definition.Id;
		_name = definition.Name;
		Description = definition.Description;
		Pattern = new Regex(definition.Pattern, RegexOptions.IgnoreCase);
		Type = (CharacteristicType)definition.Type;
		if (definition.ChargenDisplayType.HasValue)
		{
			ChargenDisplayType = (CharacterGenerationDisplayType)definition.ChargenDisplayType;
		}
	}

	public override string ToString()
	{
		return $"CharacteristicDefinition ID {Id} Name: {Name} Description: {Description}";
	}

	#region ICharacteristicDefinition Members

	public string Description { get; protected set; }
	public CharacteristicType Type { get; protected set; }

	public CharacterGenerationDisplayType ChargenDisplayType { get; protected set; }

	public ICharacteristicValue DefaultValue { get; private set; }

	public bool IsDefaultValue(ICharacteristicValue value)
	{
		return DefaultValue != null && DefaultValue == value;
	}

	public void SetDefaultValue(ICharacteristicValue theDefault)
	{
		DefaultValue = theDefault;
	}

	public ICharacteristicValue GetRandomValue()
	{
		return Gameworld.CharacteristicValues.Where(x => IsValue(x)).GetRandomElement();
	}

	public Regex Pattern { get; protected set; }

	public bool IsValue(ICharacteristicValue value)
	{
		return 
			value != null && 
			(
				value.Definition == this ||
				Parent?.IsValue(value) == true
			)
			;
	}

	public virtual ICharacteristicDefinition Parent => null;

	public virtual string HelpText => @"You can use the following options with this command:

	#3name <name>#0 - renames this characteristic definition
	#3desc <desc>#0 - gives a description of this characteristic definition (only seen by admins)
	#3pattern <regex>#0 - sets a regex pattern that is used in description markup to match this variable";

	public virtual void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "name":
				var newname = command.PopSpeech();
				if (string.IsNullOrEmpty(newname))
				{
					actor.OutputHandler.Send("You must supply a name.");
					return;
				}

				_name = newname;
				Changed = true;
				actor.OutputHandler.Send(
					$"You set the name of characteristic definition #{Id.ToString("N0", actor)} to \"{Name}\".");
				return;
			case "desc":
			case "description":
				var desc = command.SafeRemainingArgument;
				if (string.IsNullOrEmpty(desc))
				{
					actor.OutputHandler.Send("You must supply a description.");
					return;
				}

				Description = desc;
				Changed = true;
				actor.OutputHandler.Send(
					$"You set the description of characteristic definition #{Id.ToString("N0", actor)} to \"{Description}\".");
				return;
			case "pattern":
				var pattern = command.SafeRemainingArgument;
				if (string.IsNullOrEmpty(pattern))
				{
					actor.OutputHandler.Send("You must supply a pattern.");
					return;
				}

				try
				{
					var parsed = new Regex(pattern, RegexOptions.IgnoreCase);
					Pattern = parsed;
				}
				catch (RegexParseException e)
				{
					actor.OutputHandler.Send(e.Message);
					return;
				}

				Changed = true;
				actor.OutputHandler.Send(
					$"You set the pattern of characteristic definition #{Id.ToString("N0", actor)} to \"{Pattern}\".");
				return;
			case "parent":
				actor.OutputHandler.Send("You cannot change the parent of this type.");
				return;
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return;
		}
	}

	public virtual string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Characteristic Definition - {Name.ColourName()} (#{Id.ToString("N0", actor)})");
		sb.AppendLine($"Parent: {Parent?.Name.ColourName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Pattern Regex: {Pattern.ToString().ColourValue()}");
		sb.AppendLine($"Type: {Type.Describe().ColourValue()}");
		sb.AppendLine($"Chargen Display: {ChargenDisplayType.Describe().ColourValue()}");
		sb.AppendLine($"Description: {Description.ColourCommand()}");
		sb.AppendLine($"Default Value: {DefaultValue?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}");
		return sb.ToString();
	}

	#endregion

	#region ISaveable Members

	protected bool _noSave = false;
	private bool _changed;

	public bool Changed
	{
		get => _changed;
		set
		{
			if (value && !_changed && !_noSave)
			{
				Gameworld.SaveManager.Add(this);
			}

			_changed = value;
		}
	}

	public virtual void Save()
	{
		using (new FMDB())
		{
			var dbdef = FMDB.Context.CharacteristicDefinitions.Find(Id);
			dbdef.Name = Name;
			dbdef.Description = Description;
			dbdef.Pattern = Pattern.ToString();
			dbdef.Type = (int)Type;
			dbdef.ParentId = Parent?.Id;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion
}