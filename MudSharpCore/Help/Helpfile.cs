using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp.Help;

public class Helpfile : SaveableItem, IEditableHelpfile
{
	private readonly List<Tuple<IFutureProg, string>> _additionalTexts = new();

	/// <summary>
	///     Private variable representing all tags for this helpfile
	/// </summary>
	private readonly List<string> _keywords;

	private string _category;

	private string _lastEditedBy;

	private DateTime _lastEditedDate;

	private string _publicText;

	private IFutureProg _rule;

	private string _subcategory;

	private string _tagLine;

	public Helpfile(Models.Helpfile helpfile, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = helpfile.Id;
		_name = helpfile.Name;
		_category = helpfile.Category;
		_subcategory = helpfile.Subcategory;
		_publicText = helpfile.PublicText;
		_tagLine = helpfile.TagLine;
		_keywords = helpfile.Keywords.Split(' ').ToList();
		_lastEditedBy = helpfile.LastEditedBy;
		_lastEditedDate = helpfile.LastEditedDate;
		if (helpfile.RuleId.HasValue)
		{
			_rule = gameworld.FutureProgs.Get(helpfile.RuleId.Value);
		}

		foreach (var et in helpfile.HelpfilesExtraTexts)
		{
			_additionalTexts.Add(Tuple.Create(gameworld.FutureProgs.Get(et.RuleId), et.Text));
		}
	}

	public override string FrameworkItemType => "NuHelpfile";

	string IHelpInformation.HelpName => Name;
	
	/// <summary>
	///     Public property accessor for all Tags related to this helpfile
	/// </summary>
	List<string> IEditableHelpfile.Keywords => _keywords;

	public IEnumerable<string> Keywords => _keywords;

	/// <summary>
	///     The help category to which this helpfile belongs
	/// </summary>
	public string Category
	{
		get => _category;
		set
		{
			_category = value;
			Changed = true;
		}
	}

	/// <summary>
	///     The Subcategory to which this helpfile belongs
	/// </summary>
	public string Subcategory
	{
		get => _subcategory;
		set
		{
			_subcategory = value;
			Changed = true;
		}
	}

	/// <summary>
	///     A short one-line summary of the content of the article, designed to be shown in help searches
	/// </summary>
	public string TagLine
	{
		get => _tagLine;
		set
		{
			_tagLine = value;
			Changed = true;
		}
	}

	/// <summary>
	///     The public text of this helpfile, which all can see
	/// </summary>
	public string PublicText
	{
		get => _publicText;
		set
		{
			_publicText = value;
			Changed = true;
		}
	}

	/// <summary>
	///     Contains rules about who may view this helpfile. Only people who meet all of the rules (if there are any) may view
	///     the helpfile.
	/// </summary>
	public IFutureProg Rule
	{
		get => _rule;
		set
		{
			_rule = value;
			Changed = true;
		}
	}

	/// <summary>
	///     The account name of the person who last edited this helpfile
	/// </summary>
	public string LastEditedBy
	{
		get => _lastEditedBy;
		set
		{
			_lastEditedBy = value;
			Changed = true;
		}
	}

	/// <summary>
	///     The date on which this helpfile was last edited
	/// </summary>
	public DateTime LastEditedDate
	{
		get => _lastEditedDate;
		set
		{
			_lastEditedDate = value;
			Changed = true;
		}
	}

	public void SetName(string name)
	{
		_name = name;
		Changed = true;
	}

	/// <summary>
	///     Additional fragments of text that will be displayed after the helpfile if certain rules are met
	/// </summary>
	public IEnumerable<Tuple<IFutureProg, string>> AdditionalTexts => _additionalTexts;

	List<Tuple<IFutureProg, string>> IEditableHelpfile.AdditionalTexts => _additionalTexts;

	public bool CanView(ICharacter actor)
	{
		return Rule == null || ((bool?)Rule.Execute(actor) ?? false);
	}

	public IEditableHelpfile GetEditableHelpfile => this;

	public string DisplayHelpFile(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Help on {Name.TitleCase()}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Category: {Category.TitleCase().ColourValue()}");
		sb.AppendLine($"Subcategory: {Subcategory.TitleCase().ColourValue()}");
		sb.AppendLine($"Keywords: {Keywords.ListToColouredStringOr()}");
		sb.AppendLine($"Tagline: {TagLine.ProperSentences().ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine(PublicText.Wrap(actor.InnerLineFormatLength));

		foreach (var addition in AdditionalTexts.Where(x => x.Item1?.ExecuteBool(actor) == true))
		{
			sb.AppendLine();
			sb.AppendLine(addition.Item2.Wrap(actor.InnerLineFormatLength));
		}

		sb.AppendLine();
		sb.AppendLine(
			$"Lasted edited by {LastEditedBy.Proper().Colour(Telnet.Green)} on {LastEditedDate.GetLocalDateString(actor).Colour(Telnet.Green)}.");

		return sb.ToString();
	}

	public string DisplayHelpFile(IChargen chargen)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Help on {Name.TitleCase()}".GetLineWithTitleInner(chargen, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Category: {Category.TitleCase().ColourValue()}");
		sb.AppendLine($"Subcategory: {Subcategory.TitleCase().ColourValue()}");
		sb.AppendLine($"Keywords: {Keywords.ListToColouredStringOr()}");
		sb.AppendLine($"Tagline: {TagLine.ProperSentences().ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine(PublicText.Wrap(chargen.Account.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine(
			$"Lasted edited by {LastEditedBy.Proper().Colour(Telnet.Green)} on {LastEditedDate.GetLocalDateString(chargen.Account).Colour(Telnet.Green)}.");

		return sb.ToString();
	}

	#region ISaveable Members

	public override void Save()
	{
		using (new FMDB())
		{
			var dbhelp = FMDB.Context.Helpfiles.Find(Id);
			dbhelp.Name = Name;
			dbhelp.Category = Category;
			dbhelp.Subcategory = Subcategory;
			dbhelp.PublicText = PublicText;
			dbhelp.TagLine = TagLine;
			dbhelp.LastEditedBy = LastEditedBy;
			dbhelp.LastEditedDate = LastEditedDate;
			dbhelp.RuleId = Rule?.Id;
			dbhelp.Keywords = _keywords.ListToString(separator: " ", conjunction: "");
			FMDB.Context.HelpfilesExtraTexts.RemoveRange(dbhelp.HelpfilesExtraTexts);
			var i = 0;
			foreach (var et in _additionalTexts)
			{
				var dbet = new HelpfilesExtraText
				{
					DisplayOrder = i++,
					RuleId = et.Item1.Id,
					Text = et.Item2
				};
				dbhelp.HelpfilesExtraTexts.Add(dbet);
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.Helpfiles.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.Helpfiles.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public void DeleteExtraText(int index)
	{
		_additionalTexts.RemoveAt(index);
		Changed = true;
	}

	public void ReorderExtraText(int oldIndex, int newIndex)
	{
		_additionalTexts.Swap(oldIndex, newIndex);
		Changed = true;
	}
}