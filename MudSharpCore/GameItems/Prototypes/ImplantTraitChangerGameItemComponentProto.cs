using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class ImplantTraitChangerGameItemComponentProto : ImplantBaseGameItemComponentProto
{
	public override string TypeDescription => "ImplantTraitChanger";

	private readonly List<(ITraitDefinition Trait, double AdditiveBonus, double MultiplierBonus)> _bonuses = new();

	public IEnumerable<(ITraitDefinition Trait, double AdditiveBonus, double MultiplierBonus)> Bonuses => _bonuses;

	#region Constructors

	protected ImplantTraitChangerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ImplantTraitChanger")
	{
	}

	protected ImplantTraitChangerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		foreach (var item in root.Element("Bonuses").Elements())
		{
			var trait = Gameworld.Traits.Get(long.Parse(item.Attribute("trait").Value));
			if (trait == null)
			{
				continue;
			}

			_bonuses.Add((trait, double.Parse(item.Attribute("additive").Value),
				double.Parse(item.Attribute("multiplier").Value)));
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		var def = SaveToXmlWithoutConvertingToString();
		def.Add(new XElement("Bonuses",
			from item in _bonuses
			select new XElement("Bonus", new XAttribute("trait", item.Trait.Id),
				new XAttribute("additive", item.AdditiveBonus), new XAttribute("multiplier", item.MultiplierBonus))
		));
		return def.ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ImplantTraitChangerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ImplantTraitChangerGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ImplantTraitChanger".ToLowerInvariant(), true,
			(gameworld, account) => new ImplantTraitChangerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImplantTraitChanger",
			(proto, gameworld) => new ImplantTraitChangerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ImplantTraitChanger",
			$"An {"[implant]".Colour(Telnet.Pink)} that gives a bonus/penalty to a skill or attribute when {"[powered]".Colour(Telnet.Magenta)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImplantTraitChangerGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbody <body> - sets the body prototype this implant is used with\n\tbodypart <bodypart> - sets the bodypart prototype this implant is used with\n\texternal - toggles whether this implant is external\n\texternaldesc <desc> - an alternate sdesc used when installed and external\n\tpower <watts> - how many watts of power to use\n\tdiscount <watts> - how many watts of power usage to discount per point of quality\n\tgrace <percentage> - the grace percentage of hp damage before implant function reduces\n\tspace <#> - the amount of 'space' in a bodypart that the implant takes up\n\tdifficulty <difficulty> - how difficulty it is for surgeons to install this implant\n\tbonus <trait> - removes an existing bonus\n\tbonus <trait> <add> [<multiplier>] - sets a bonus for a trait. The multiplier is a percentage of the base value of the trait. The add is added after this multiplication.";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "bonus":
				return BuildingCommandBonus(actor, command);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
		}
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must use the syntax {"comp set bonus <trait> <add> [<multiplier>]".ColourCommand()} to add or edit a bonus, or just {"comp set bonus <trait>".ColourCommand()} to remove any bonus associated with that trait.");
			return false;
		}

		var trait = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(command.Last);

		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		if (command.IsFinished)
		{
			_bonuses.RemoveAll(x => x.Trait == trait);
			Changed = true;
			actor.OutputHandler.Send(
				$"You remove all bonuses (if any) associated with the {trait.Name.TitleCase().ColourValue()} trait.");
			return true;
		}

		if (!double.TryParse(command.PopSpeech(), out var dvalue))
		{
			actor.OutputHandler.Send("You must specify a number for the additive component of that bonus.");
			return false;
		}

		var multiplier = 0.0;
		if (!command.IsFinished)
		{
			if (!double.TryParse(command.PopSpeech(), out multiplier))
			{
				actor.OutputHandler.Send("You must specify a number for the multiplier component of that bonus.");
				return false;
			}
		}

		_bonuses.RemoveAll(x => x.Trait == trait);
		_bonuses.Add((trait, dvalue, multiplier));
		Changed = true;
		actor.OutputHandler.Send(
			$"This implant now provides an additive bonus of {dvalue.ToString("N3", actor).ColourValue()} and a multiplier bonus of {multiplier.ToString("N3", actor).ColourValue()} to the {trait.Name.TitleCase().ColourValue()} trait.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis implant modifies traits when powered. It has the following bonuses: {4}",
			"ImplantTraitChanger Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			_bonuses.Select(x =>
				        $"\n\t{x.Trait.Name.TitleCase().ColourValue()}) {x.MultiplierBonus.ToString("N3", actor)} * base + {x.AdditiveBonus.ToString("N3", actor)}")
			        .ListToString(separator: "", conjunction: "", twoItemJoiner: "")
		);
	}
}