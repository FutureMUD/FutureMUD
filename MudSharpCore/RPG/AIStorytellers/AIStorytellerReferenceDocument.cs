#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;

namespace MudSharp.RPG.AIStorytellers;

public class AIStorytellerReferenceDocument : SaveableItem, IAIStorytellerReferenceDocument
{
	private readonly HashSet<long> _restrictedStorytellerIds;

	public AIStorytellerReferenceDocument(Models.AIStorytellerReferenceDocument dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name ?? string.Empty;
		Description = dbitem.Description ?? string.Empty;
		FolderName = dbitem.FolderName ?? string.Empty;
		DocumentType = dbitem.DocumentType ?? string.Empty;
		Keywords = dbitem.Keywords ?? string.Empty;
		DocumentContents = dbitem.DocumentContents ?? string.Empty;
		_restrictedStorytellerIds = ParseRestrictedStorytellerIds(dbitem.RestrictedStorytellerIds);
	}

	public AIStorytellerReferenceDocument(IFuturemud gameworld, string name, string description, string folderName,
		string documentType, string keywords, string documentContents)
	{
		Gameworld = gameworld;
		_name = name ?? string.Empty;
		Description = description ?? string.Empty;
		FolderName = folderName ?? string.Empty;
		DocumentType = documentType ?? string.Empty;
		Keywords = keywords ?? string.Empty;
		DocumentContents = documentContents ?? string.Empty;
		_restrictedStorytellerIds = [];

		using (new FMDB())
		{
			var dbitem = new Models.AIStorytellerReferenceDocument
			{
				Name = name,
				Description = description,
				FolderName = folderName,
				DocumentType = documentType,
				Keywords = keywords,
				DocumentContents = documentContents,
				RestrictedStorytellerIds = string.Empty
			};
			FMDB.Context.AIStorytellerReferenceDocuments.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public string Description { get; private set; }
	public string FolderName { get; private set; }
	public string DocumentType { get; private set; }
	public string Keywords { get; private set; }
	public string DocumentContents { get; private set; }
	public IEnumerable<long> RestrictedStorytellerIds => _restrictedStorytellerIds;

	public override string FrameworkItemType => "AIStorytellerReferenceDocument";

	public override void Save()
	{
		var dbitem = FMDB.Context.AIStorytellerReferenceDocuments.Find(Id);
		if (dbitem is null)
		{
			return;
		}

		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.FolderName = FolderName;
		dbitem.DocumentType = DocumentType;
		dbitem.Keywords = Keywords;
		dbitem.DocumentContents = DocumentContents;
		dbitem.RestrictedStorytellerIds = SerialiseRestrictedStorytellerIds(_restrictedStorytellerIds);
		Changed = false;
	}

	public void Delete()
	{
		Changed = false;
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.AIStorytellerReferenceDocuments.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.AIStorytellerReferenceDocuments.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}

		Gameworld.Destroy(this);
	}

	public bool ReturnForSearch(string searchterm)
	{
		if (string.IsNullOrWhiteSpace(searchterm))
		{
			return true;
		}

		var terms = searchterm.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		return terms.All(TermMatchesDocument);
	}

	public bool IsVisibleTo(IAIStoryteller storyteller)
	{
		return _restrictedStorytellerIds.Count == 0 || _restrictedStorytellerIds.Contains(storyteller.Id);
	}

	private static HashSet<long> ParseRestrictedStorytellerIds(string? text)
	{
		var result = new HashSet<long>();
		if (string.IsNullOrWhiteSpace(text))
		{
			return result;
		}

		var trimmed = text.Trim();
		if (trimmed.StartsWith("<"))
		{
			try
			{
				var xml = XElement.Parse(trimmed);
				foreach (var element in xml.Descendants("Storyteller"))
				{
					if (long.TryParse(element.Value, out var id))
					{
						result.Add(id);
					}
				}

				return result;
			}
			catch
			{
				// Fallback to CSV parsing below.
			}
		}

		foreach (var item in trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			if (long.TryParse(item, out var id))
			{
				result.Add(id);
			}
		}

		return result;
	}

	private static string SerialiseRestrictedStorytellerIds(IEnumerable<long> ids)
	{
		return string.Join(",", ids
			.Where(x => x > 0)
			.Distinct()
			.OrderBy(x => x));
	}

	private bool TermMatchesDocument(string term)
	{
		return Name.Contains(term, StringComparison.InvariantCultureIgnoreCase) ||
		       Description.Contains(term, StringComparison.InvariantCultureIgnoreCase) ||
		       FolderName.Contains(term, StringComparison.InvariantCultureIgnoreCase) ||
		       DocumentType.Contains(term, StringComparison.InvariantCultureIgnoreCase) ||
		       Keywords.Contains(term, StringComparison.InvariantCultureIgnoreCase) ||
		       DocumentContents.Contains(term, StringComparison.InvariantCultureIgnoreCase);
	}

	private const string BuildingCommandHelp = @"You can use the following options:

	#3name <name>#0 - renames this reference document
	#3description <description>#0 - changes the document description
	#3folder <folder name>#0 - changes the folder name
	#3type <type>#0 - changes the document type
	#3keywords <keywords>#0 - changes the search keywords
	#3contents#0 - drops you into an editor for the body text of this document
	#3storyteller <which>#0 - toggles storyteller-specific visibility restriction
	#3storytellers clear#0 - removes all storyteller restrictions (global visibility)";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "folder":
				return BuildingCommandFolder(actor, command);
			case "type":
				return BuildingCommandType(actor, command);
			case "keywords":
			case "keyword":
				return BuildingCommandKeywords(actor, command);
			case "contents":
			case "content":
			case "text":
				return BuildingCommandContents(actor, command);
			case "storyteller":
			case "restriction":
			case "restrict":
				return BuildingCommandStoryteller(actor, command);
			case "storytellers":
			case "restrictions":
				return BuildingCommandStorytellers(actor, command);
			default:
				actor.OutputHandler.Send(BuildingCommandHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give this reference document?");
			return false;
		}

		_name = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This reference document is now called {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to use for this reference document?");
			return false;
		}

		Description = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send("You update the reference document description.");
		return true;
	}

	private bool BuildingCommandFolder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What folder name do you want to use for this reference document?");
			return false;
		}

		FolderName = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This reference document now has folder name {FolderName.ColourName()}.");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What document type do you want to use?");
			return false;
		}

		DocumentType = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This reference document now has type {DocumentType.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandKeywords(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What keywords do you want to set?");
			return false;
		}

		Keywords = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This reference document now has keywords {Keywords.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandContents(ICharacter actor, StringStack command)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(DocumentContents))
		{
			sb.AppendLine("Replacing:\n");
			sb.AppendLine(DocumentContents.Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
		}

		sb.AppendLine("Enter the new document contents in the editor below.");
		sb.AppendLine();
		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(BuildingCommandContentsPost, BuildingCommandContentsCancel, 1.0, DocumentContents);
		return true;
	}

	private void BuildingCommandContentsCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the reference document contents.");
	}

	private void BuildingCommandContentsPost(string text, IOutputHandler handler, object[] args)
	{
		DocumentContents = text;
		Changed = true;
		handler.Send("You update the reference document contents.");
	}

	private bool BuildingCommandStoryteller(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which storyteller do you want to toggle visibility restriction for?");
			return false;
		}

		var storyteller = actor.Gameworld.AIStorytellers.GetByIdOrName(command.SafeRemainingArgument);
		if (storyteller is null)
		{
			actor.OutputHandler.Send("There is no such AI storyteller.");
			return false;
		}

		if (_restrictedStorytellerIds.Contains(storyteller.Id))
		{
			_restrictedStorytellerIds.Remove(storyteller.Id);
			actor.OutputHandler.Send(
				$"This document is no longer restricted to storyteller {storyteller.Name.ColourName()}.");
		}
		else
		{
			_restrictedStorytellerIds.Add(storyteller.Id);
			actor.OutputHandler.Send(
				$"This document is now restricted to storyteller {storyteller.Name.ColourName()}.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandStorytellers(ICharacter actor, StringStack command)
	{
		if (!command.IsFinished && command.PopForSwitch() == "clear")
		{
			if (_restrictedStorytellerIds.Count == 0)
			{
				actor.OutputHandler.Send("This document is already globally visible.");
				return false;
			}

			_restrictedStorytellerIds.Clear();
			Changed = true;
			actor.OutputHandler.Send("This document is now globally visible to all storytellers.");
			return true;
		}

		actor.OutputHandler.Send(
			$"Use {"storyteller <which>".ColourCommand()} to toggle restrictions, or {"storytellers clear".ColourCommand()} to make this document globally visible.");
		return false;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"AI Storyteller Reference Document #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Folder: {FolderName.ColourName()}");
		sb.AppendLine($"Type: {DocumentType.ColourValue()}");
		sb.AppendLine($"Keywords: {Keywords.ColourCommand()}");
		sb.AppendLine(
			$"Visibility: {(_restrictedStorytellerIds.Count == 0 ? "Global".ColourValue() : "Restricted".ColourError())}");
		if (_restrictedStorytellerIds.Count > 0)
		{
			sb.AppendLine(
				$"Restricted Storytellers: {_restrictedStorytellerIds.Select(x => actor.Gameworld.AIStorytellers.Get(x)?.Name ?? $"#{x:N0}").ListToString()}");
		}
		sb.AppendLine();
		sb.AppendLine("Description".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine("Contents".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(DocumentContents.Wrap(actor.InnerLineFormatLength));
		return sb.ToString();
	}
}
