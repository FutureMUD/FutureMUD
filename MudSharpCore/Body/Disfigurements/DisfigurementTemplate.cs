using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body.Disfigurements;

public abstract class DisfigurementTemplate : Framework.Revision.EditableItem, IDisfigurementTemplate
{
	#region Overrides of Item

	public sealed override string FrameworkItemType => "DisfigurementTemplate";

	#region Overrides of EditableItem

	protected virtual string BuildingHelpText =>
		@"You can use the following building commmands:

	#3name <name>#0 - sets the name for this template
	#3sdesc <description>#0 - this item's short description
	#3desc <description>#0 - this item's full (i.e. look) description";

	/// <summary>Handles OLC Building related commands from an Actor</summary>
	/// <param name="actor">The ICharacter requesting the edit</param>
	/// <param name="command">The command they have entered</param>
	/// <returns>True if anything was changed, false if the command was invalid or did not change anything</returns>
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "sdesc":
			case "short":
			case "shortdescription":
			case "short_description":
			case "short description":
				return BuildingCommandShortDescription(actor, command);
			case "full":
			case "desc":
			case "description":
			case "fdesc":
			case "fulldesc":
			case "full desc":
			case "full_desc":
			case "fulldescription":
			case "full description":
			case "full_description":
				return BuildingCommandDescription(actor, command);
			case "prog":
			case "canselectprog":
			case "chargenprog":
			case "chargen_prog":
			case "chargen prog":
				if (!actor.IsAdministrator())
				{
					goto default;
				}

				return BuildingCommandProg(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	protected abstract IEnumerable<IDisfigurementTemplate> TemplatesForCharacter(ICharacter builder, bool currentOnly);

	protected abstract string SubtypeName { get; }

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (!string.IsNullOrEmpty(FullDescription))
		{
			actor.OutputHandler.Send("Replacing:\n" +
			                         FullDescription.ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
		}

		actor.OutputHandler.Send("Enter the description in the editor below.");
		actor.EditorMode(BuildingCommandDescPost, BuildingCommandDescCancel, 1.0);
		return true;
	}

	private void BuildingCommandDescCancel(IOutputHandler handler, object[] parameters)
	{
		handler.Send("You decide not to change the description.");
	}

	private void BuildingCommandDescPost(string description, IOutputHandler handler, object[] parameters)
	{
		FullDescription = description.Trim().ProperSentences();
		Changed = true;
		handler.Send("You set the description for " + EditHeader() + " to:\n\n" + FullDescription.Wrap(80, "\t"));
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What name do you want to set for this {SubtypeName.ToLowerInvariant()}?");
			return false;
		}

		var name = command.PopSpeech();
		if (TemplatesForCharacter(actor, false).Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a {SubtypeName.ToLowerInvariant()} with that name. Names must be unique.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"You set the name for this {SubtypeName.ToLowerInvariant()} to {Name}.");
		return true;
	}

	private bool BuildingCommandShortDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the short description to?");
			return false;
		}

		ShortDescription = command.SafeRemainingArgument.ToLowerInvariant();
		actor.OutputHandler.Send(
			$"You set the short description for this {SubtypeName.ToLowerInvariant()} to {ShortDescription}.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Either specify a prog or use the keyword clear to clear an existing one.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("clear", "none", "delete", "remove"))
		{
			CanSelectInChargenProg = null;
			Changed = true;
			actor.OutputHandler.Send(
				$"This {SubtypeName} template will no longer use any prog to control whether it appears in chargen.");
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send("You must specify a prog that returns a boolean value.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Chargen }))
		{
			actor.OutputHandler.Send("You must specify a prog that accepts a single chargen parameter.");
			return false;
		}

		CanSelectInChargenProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This {SubtypeName} will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine whether it appears in chargen.");
		return true;
	}

	#endregion

	#endregion

	protected abstract string DefaultShortDescription { get; }
	protected abstract string DefaultFullDescription { get; }

	protected DisfigurementTemplate(MudSharp.Models.DisfigurementTemplate dbitem, IFuturemud gameworld) : base(
		dbitem.EditableItem)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		RevisionNumber = dbitem.RevisionNumber;
		_name = dbitem.Name;
		_shortDescription = dbitem.ShortDescription;
		_fullDescription = dbitem.FullDescription;
	}

	protected DisfigurementTemplate(IAccount originator, string name) : base(originator)
	{
		Gameworld = originator.Gameworld;
		_id = Gameworld.DisfigurementTemplates.NextID();
		RevisionNumber = 0;
		_name = name;
		_shortDescription = DefaultShortDescription;
		_fullDescription = DefaultFullDescription;
		using (new FMDB())
		{
			var dbitem = new Models.DisfigurementTemplate
			{
				Type = SubtypeName,
				EditableItem = new Models.EditableItem
				{
					BuilderAccountId = originator.Id,
					BuilderDate = DateTime.UtcNow,
					RevisionStatus = (int)RevisionStatus.UnderDesign,
					RevisionNumber = 0
				}
			};
			FMDB.Context.DisfigurementTemplates.Add(dbitem);
			FMDB.Context.EditableItems.Add(dbitem.EditableItem);
			Save(dbitem);
			FMDB.Context.SaveChanges();
		}
	}

	protected DisfigurementTemplate(DisfigurementTemplate rhs, IAccount originator, string name) : base(rhs, originator)
	{
		Gameworld = rhs.Gameworld;
		if (string.IsNullOrEmpty(name))
		{
			_id = rhs.Id;
			RevisionNumber = Gameworld.DisfigurementTemplates.GetAll(Id).Max(x => x.RevisionNumber) + 1;
		}
		else
		{
			_id = Gameworld.DisfigurementTemplates.NextID();
			RevisionNumber = 0;
		}

		_name = name ?? rhs._name;
		_shortDescription = rhs._shortDescription;
		_fullDescription = rhs._fullDescription;
		using (new FMDB())
		{
			var dbitem = new Models.DisfigurementTemplate
			{
				Type = SubtypeName,
				Definition = rhs.SaveDefinition().ToString(),
				EditableItem = new Models.EditableItem
				{
					BuilderAccountId = originator.Id,
					BuilderDate = DateTime.UtcNow,
					RevisionStatus = (int)RevisionStatus.UnderDesign,
					RevisionNumber = RevisionNumber
				}
			};
			FMDB.Context.EditableItems.Add(dbitem.EditableItem);
			FMDB.Context.DisfigurementTemplates.Add(dbitem);
			Save(dbitem);
			FMDB.Context.SaveChanges();
		}
	}

	private void Save(MudSharp.Models.DisfigurementTemplate template)
	{
		template.Id = Id;
		template.RevisionNumber = RevisionNumber;
		Save(template.EditableItem);
		template.Name = _name;
		template.ShortDescription = ShortDescription;
		template.FullDescription = FullDescription;
		template.Definition = SaveDefinition().ToString();
	}

	protected abstract XElement SaveDefinition();

	public abstract IDisfigurementTemplate Clone(IAccount originator, string newName);

	#region Overrides of SavableKeywordedItem

	/// <summary>Tells the object to perform whatever save action it needs to do</summary>
	public override void Save()
	{
		var dbitem = FMDB.Context.DisfigurementTemplates.Find(Id, RevisionNumber);
		Save(dbitem);
		Changed = false;
	}

	#endregion

	#region Implementation of IDisfigurementTemplate

	private string _shortDescription;

	public string ShortDescription
	{
		get => _shortDescription;
		protected set
		{
			_shortDescription = value;
			Changed = true;
		}
	}

	private string _fullDescription;

	public string FullDescription
	{
		get => _fullDescription;
		protected set
		{
			_fullDescription = value;
			Changed = true;
		}
	}


	public string ShortDescriptionForChargen => ShortDescription.SubstituteWrittenLanguage(null, Gameworld);
	public string FullDescriptionForChargen => FullDescription.SubstituteWrittenLanguage(null, Gameworld);
	public abstract IEnumerable<IBodypartShape> BodypartShapes { get; }

	public abstract bool CanBeAppliedToBodypart(IBody body, IBodypart part);

	public Counter<IChargenResource> ChargenCosts { get; } = new();

	public bool CanSelectInChargen { get; protected set; }

	public IFutureProg CanSelectInChargenProg { get; protected set; }

	public bool AppearInChargenList(IChargen chargen)
	{
		return CanSelectInChargen && CanSelectInChargenProg?.Execute<bool?>(chargen) != false;
	}

	#endregion
}