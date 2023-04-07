using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory;

namespace MudSharp.GameItems.Prototypes;

public class VariableChangerGameItemComponentProto : VariableGameItemComponentProto
{
	protected VariableChangerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Variable Changer")
	{
	}

	protected VariableChangerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	/// <summary>
	///     If not null, this specifies a Wear Profile which the item must be worn in to function as a Characteristic Changer
	/// </summary>
	public IWearProfile TargetWearProfile { get; protected set; }

	public override string TypeDescription => "Variable Changer";

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tvariable add <which> <profile> - adds a variable with the specified random profile\n\tvariable remove <which> - removes a variable\n\ttarget <wear> - sets a wear profile that must be used\n\ttarget - clears a wear profile requirement";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.Peek().ToLowerInvariant() == "target")
		{
			command.Pop();
			var arg = command.PopSpeech().ToLowerInvariant();
			if (arg.Length == 0)
			{
				TargetWearProfile = null;
				actor.OutputHandler.Send("This Characteristic Changer will no longer target any Wear Profiles.");
				Changed = true;
				return true;
			}

			IWearProfile profile = null;
			profile = long.TryParse(arg, out var value)
				? Gameworld.WearProfiles.Get(value)
				: Gameworld.WearProfiles.Get(arg).FirstOrDefault();

			if (profile == null)
			{
				actor.OutputHandler.Send("There is no such Wear Profile.");
				return false;
			}

			TargetWearProfile = profile;
			actor.OutputHandler.Send("You change the Target Wear Profile to " + profile.Name + " [#" + profile.Id +
			                         "].");
			Changed = true;
			return true;
		}

		return base.BuildingCommand(actor, command);
	}

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("variable changer", false,
			(gameworld, account) => new VariableChangerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("vchanger", true,
			(gameworld, account) => new VariableChangerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("variablechanger", false,
			(gameworld, account) => new VariableChangerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Variable Changer",
			(proto, gameworld) => new VariableChangerGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"VariableChanger",
			$"Changes the values of one characteristic to others when worn, combined with a {"[wearable]".Colour(Telnet.BoldYellow)}",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new VariableChangerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new VariableChangerGameItemComponent(component, this, parent);
	}

	protected override void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("TargetWearProfile");
		if (attr != null && attr.Value.Length > 0)
		{
			TargetWearProfile = Gameworld.WearProfiles.Get(long.Parse(attr.Value));
		}

		base.LoadFromXml(root);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{3:N0}r{4:N0}, {5})\n\nWhen worn{1}, this item {2}",
			"Changer Item Component".Colour(Telnet.Cyan),
			TargetWearProfile != null
				? string.Format(actor, "in the {0} (#{1:N0}) wear profile", TargetWearProfile.Name,
					TargetWearProfile.Id)
				: "",
			_characteristicDefinitions.Any()
				? $"changes the values of {(from value in _characteristicDefinitions select value.Key.Name + " to a value from the " + value.Value.Name + " profile").ListToString()}."
				: "does not change any values.",
			Id,
			RevisionNumber,
			Name
		);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XAttribute("TargetWearProfile", TargetWearProfile?.Id.ToString() ?? ""),
				from value in _characteristicDefinitions
				select
					new XElement("Characteristic", new XAttribute("Profile", value.Value.Id),
						new XAttribute("Value", value.Key.Id))).ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var dbnew = new Models.GameItemComponentProto
			{
				Id = Id,
				RevisionNumber = FMDB.Context.GameItemComponentProtos.Where(x => x.Id == Id)
				                     .Select(x => x.RevisionNumber)
				                     .AsEnumerable()
				                     .DefaultIfEmpty(0)
				                     .Max() + 1,
				EditableItem = new Models.EditableItem()
			};
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
			dbnew.EditableItem.BuilderAccountId = initiator.Account.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;
			dbnew.Description = Description;
			dbnew.Name = Name.Proper();
			dbnew.Definition = SaveToXml();
			dbnew.Type = "Variable Changer";
			FMDB.Context.GameItemComponentProtos.Add(dbnew);
			FMDB.Context.SaveChanges();

			return new VariableChangerGameItemComponentProto(dbnew, Gameworld);
		}
	}
}