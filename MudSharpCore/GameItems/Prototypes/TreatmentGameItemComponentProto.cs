using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class TreatmentGameItemComponentProto : GameItemComponentProto
{
	private readonly List<TreatmentType> _treatmentTypes = new();
	public IEnumerable<TreatmentType> TreatmentTypes => _treatmentTypes;
	public int MaximumUses { get; private set; } = -1;
	public bool Refillable { get; private set; }
	public int DifficultyStages { get; private set; }
	public override string TypeDescription => "Treatment";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("treatment", true,
			(gameworld, account) => new TreatmentGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Treatment",
			(proto, gameworld) => new TreatmentGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Treatment",
			$"A kit that can be used for medical treatments like suturing, binding, cleaning, etc",
			BuildingHelpText
		);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0})\n\nThis item can be used to treat wounds by {3}. {6}. It has {4} uses before exhaustion and {5} be recharged from other similar treatment items.",
			"Treatment Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			TreatmentTypes.Any()
				? $"treatment types {TreatmentTypes.Select(x => x.Describe().Colour(Telnet.Green)).ListToString()}"
				: "no treatment types selected".Colour(Telnet.Red),
			MaximumUses == -1 ? "unlimited" : MaximumUses.ToString("N0"),
			Refillable ? "can" : "cannot",
			DifficultyStages == 0
				? "It does not impact upon the difficulty of treatment positively or negatively"
				: string.Format(actor, "It makes treatment {0:N0} degrees {1}", Math.Abs(DifficultyStages),
					DifficultyStages < 0 ? "harder" : "easier")
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("MaximumUses");
		if (element != null)
		{
			MaximumUses = int.Parse(element.Value);
		}

		element = root.Element("Refillable");
		if (element != null)
		{
			Refillable = bool.Parse(element.Value);
		}

		element = root.Element("DifficultyStages");
		if (element != null)
		{
			DifficultyStages = int.Parse(element.Value);
		}

		foreach (var item in root.Elements("TreatmentType"))
		{
			_treatmentTypes.Add((TreatmentType)int.Parse(item.Value));
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MaximumUses", MaximumUses),
			new XElement("Refillable", Refillable),
			new XElement("DifficultyStages", DifficultyStages),
			from item in _treatmentTypes select new XElement("TreatmentType", (int)item)
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new TreatmentGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new TreatmentGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new TreatmentGameItemComponentProto(proto, gameworld));
	}

	#region Constructors

	protected TreatmentGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected TreatmentGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Treatment")
	{
		Changed = true;
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttreatment <type> - sets the type of treatment this item is good for\n\tstages <number> - sets the difficulty stages (+ve is bonus) that the kit gives\n\tuses <number> - sets the number of uses for the kit. -1 for unlimited.\n\trefill - sets whether the kit can be refilled.";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "use":
			case "uses":
				return BuildingCommandUses(actor, command);
			case "refillable":
			case "refill":
				return BuildingCommandRefill(actor, command);
			case "difficulty":
			case "stages":
			case "bonus":
			case "stage":
			case "diff":
				return BuildingCommandDifficulty(actor, command);
			case "treatment":
				return BuildingCommandTreatment(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"You must specify a number of degrees of difficulty to add or remove. Negative numbers make it harder.");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value))
		{
			actor.Send(
				"You must specify a number of degrees of difficulty to add or remove. Negative numbers make it harder.");
			return false;
		}

		DifficultyStages = value;
		Changed = true;
		actor.Send(DifficultyStages == 0
			? "This item now does not impact upon the difficulty of treatment positively or negatively"
			: string.Format(actor, "This item now makes treatment {0:N0} degrees {1}.", Math.Abs(DifficultyStages),
				DifficultyStages < 0 ? "harder" : "easier"));
		return true;
	}

	private bool BuildingCommandTreatment(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which treatment types do you want this item to be able to treat? See SHOW TREATMENTTYPES.");
			return false;
		}

		if (!WoundExtensions.GetTreatmentType(command.SafeRemainingArgument, out var value))
		{
			actor.Send("That is not a valid treatment type.");
			return false;
		}

		if (_treatmentTypes.Contains(value))
		{
			_treatmentTypes.Remove(value);
			actor.Send("This treatment item will no longer help with {0}.", value.Describe());
			Changed = true;
			return true;
		}

		_treatmentTypes.Add(value);
		Changed = true;
		actor.Send("This treatment type will now help with {0}.", value.Describe());
		return true;
	}

	private bool BuildingCommandRefill(ICharacter actor, StringStack command)
	{
		if (Refillable)
		{
			actor.Send("This treatment item will no longer be refillable.");
			Refillable = false;
			Changed = true;
			return true;
		}

		actor.Send("This treatment item will now be refillable.");
		Refillable = true;
		Changed = true;
		return true;
	}

	private bool BuildingCommandUses(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many uses should this treatment item have? Use -1 for unlimited.");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value))
		{
			if (command.Last.Equals("unlimited", StringComparison.InvariantCultureIgnoreCase))
			{
				value = -1;
			}
			else
			{
				actor.Send("You must enter a number of uses for this treatment item to have.");
				return false;
			}
		}

		if (value <= 0 && value != -1)
		{
			actor.Send("You must enter a positive number of uses, or -1 for unlimited.");
			return false;
		}

		MaximumUses = value;
		actor.Send("You set the maximum number of uses for this treatment item to {0}.",
			MaximumUses == -1 ? "unlimited" : MaximumUses.ToString("N0", actor));
		Changed = true;
		return true;
	}

	#endregion Building Commands
}