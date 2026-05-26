#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class SealStampGameItemComponentProto : GameItemComponentProto, ISealStampPrototype
{
	public override string TypeDescription => "SealStamp";

	public string SealDesign { get; protected set; } = "an anonymous seal";
	public string IssuerText { get; protected set; } = string.Empty;
	public string OwnerText { get; protected set; } = string.Empty;
	public string ClanText { get; protected set; } = string.Empty;
	public string OfficeText { get; protected set; } = string.Empty;
	public string StampMaterial { get; protected set; } = string.Empty;
	public Difficulty ForgeryDifficulty { get; protected set; } = Difficulty.Normal;
	public IFutureProg? AuthorityProg { get; protected set; }

	protected SealStampGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "SealStamp")
	{
	}

	protected SealStampGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		SealDesign = root.Element("SealDesign")?.Value ?? "an anonymous seal";
		IssuerText = root.Element("IssuerText")?.Value ?? string.Empty;
		OwnerText = root.Element("OwnerText")?.Value ?? string.Empty;
		ClanText = root.Element("ClanText")?.Value ?? string.Empty;
		OfficeText = root.Element("OfficeText")?.Value ?? string.Empty;
		StampMaterial = root.Element("StampMaterial")?.Value ?? string.Empty;
		ForgeryDifficulty = (Difficulty)int.Parse(root.Element("ForgeryDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		AuthorityProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("AuthorityProg")?.Value ?? "0"));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SealDesign", new XCData(SealDesign)),
			new XElement("IssuerText", new XCData(IssuerText)),
			new XElement("OwnerText", new XCData(OwnerText)),
			new XElement("ClanText", new XCData(ClanText)),
			new XElement("OfficeText", new XCData(OfficeText)),
			new XElement("StampMaterial", new XCData(StampMaterial)),
			new XElement("ForgeryDifficulty", (int)ForgeryDifficulty),
			new XElement("AuthorityProg", AuthorityProg?.Id ?? 0L)).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new SealStampGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SealStampGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("sealstamp", true,
			(gameworld, account) => new SealStampGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("SealStamp",
			(proto, gameworld) => new SealStampGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"SealStamp",
			$"Turns an item into a {"[seal stamp]".Colour(Telnet.Yellow)} for sealing sealable items",
			BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new SealStampGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3design <text>#0 - sets the seal design text
	#3issuer <text>#0 - sets the issuer metadata
	#3owner <text>#0 - sets the owner metadata
	#3clan <text>#0 - sets the clan metadata
	#3office <text>#0 - sets the office metadata
	#3material <text>#0 - sets the stamp material metadata
	#3forgery <difficulty>#0 - sets how hard the seal is to convincingly forge
	#3prog <prog>|clear#0 - optional boolean prog taking character, target item, stamp item, and optional medium item";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "design":
				return BuildingCommandText(actor, command, "design", value => SealDesign = value);
			case "issuer":
				return BuildingCommandText(actor, command, "issuer metadata", value => IssuerText = value);
			case "owner":
				return BuildingCommandText(actor, command, "owner metadata", value => OwnerText = value);
			case "clan":
				return BuildingCommandText(actor, command, "clan metadata", value => ClanText = value);
			case "office":
				return BuildingCommandText(actor, command, "office metadata", value => OfficeText = value);
			case "material":
				return BuildingCommandText(actor, command, "stamp material metadata", value => StampMaterial = value);
			case "forgery":
			case "difficulty":
				return BuildingCommandForgery(actor, command);
			case "prog":
			case "authority":
				return BuildingCommandProg(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandText(ICharacter actor, StringStack command, string field, System.Action<string> setter)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What {field} should this seal stamp use?");
			return false;
		}

		var text = command.SafeRemainingArgument;
		setter(text);
		Changed = true;
		actor.OutputHandler.Send($"This seal stamp's {field} is now {text.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandForgery(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.OutputHandler.Send($"What difficulty should it be to forge this seal? Valid values are {Enum.GetValues<Difficulty>().Select(x => x.Describe().ColourValue()).ListToString()}.");
			return false;
		}

		ForgeryDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"This seal will now be {ForgeryDifficulty.DescribeColoured()} to forge convincingly.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which boolean FutureProg should control whether this stamp can seal something? Use #3clear#0 to remove one.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			AuthorityProg = null;
			Changed = true;
			actor.OutputHandler.Send("This seal stamp no longer uses an authority prog.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[
				ProgVariableTypes.Character,
				ProgVariableTypes.Item,
				ProgVariableTypes.Item,
				ProgVariableTypes.Item
			]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AuthorityProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This seal stamp will now use the {prog.MXPClickableFunctionNameWithId()} prog to control sealing authority.");
		return true;
	}

	public bool CanSeal(ICharacter actor, IGameItem stamp, IGameItem target, IGameItem? medium, out string error)
	{
		error = string.Empty;
		if (AuthorityProg is null)
		{
			return true;
		}

		var permitted = AuthorityProg.MatchesParameters([
			ProgVariableTypes.Character,
			ProgVariableTypes.Item,
			ProgVariableTypes.Item,
			ProgVariableTypes.Item
		])
			? AuthorityProg.Execute<bool?>(actor, target, stamp, medium) == true
			: AuthorityProg.Execute<bool?>(actor, target, stamp) == true;

		if (permitted)
		{
			return true;
		}

		error = $"{stamp.HowSeen(actor, true)} is not authorised to seal {target.HowSeen(actor)}.";
		return false;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $@"{"SealStamp Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

Design: {SealDesign.ColourValue()}
Issuer: {(string.IsNullOrEmpty(IssuerText) ? "None".ColourError() : IssuerText.ColourValue())}
Owner: {(string.IsNullOrEmpty(OwnerText) ? "None".ColourError() : OwnerText.ColourValue())}
Clan: {(string.IsNullOrEmpty(ClanText) ? "None".ColourError() : ClanText.ColourValue())}
Office: {(string.IsNullOrEmpty(OfficeText) ? "None".ColourError() : OfficeText.ColourValue())}
Material: {(string.IsNullOrEmpty(StampMaterial) ? "Unspecified".ColourError() : StampMaterial.ColourValue())}
Forgery Difficulty: {ForgeryDifficulty.DescribeColoured()}
Authority Prog: {(AuthorityProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError())}";
	}
}
