using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class PaperSheetGameItemComponent : GameItemComponent, IWriteable, IReadable
{
	protected PaperSheetGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PaperSheetGameItemComponentProto)newProto;
	}

	#region Constructors

	public PaperSheetGameItemComponent(PaperSheetGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public PaperSheetGameItemComponent(MudSharp.Models.GameItemComponent component,
		PaperSheetGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public PaperSheetGameItemComponent(PaperSheetGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		Readables.AddRange(rhs.Readables);
		Title = rhs.Title;
	}

	protected void LoadFromXml(XElement root)
	{
		Title = root.Element("Title")?.Value;
		foreach (var item in root.Element("Writings")?.Elements() ?? Enumerable.Empty<XElement>())
		{
			if (item.Name.LocalName.EqualTo("Writing"))
			{
				Readables.Add(Gameworld.Writings.Get(long.Parse(item.Value)));
			}
			else
			{
				Readables.Add(Gameworld.Drawings.Get(long.Parse(item.Value)));
			}
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PaperSheetGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("Title", new XCData(Title ?? "")),
				new XElement("Writings",
					from item in Readables
					select
						item is IWriting ? new XElement("Writing", item.Id) : new XElement("Drawing", item.Id)
				)
			).ToString();
	}

	#endregion

	#region IReadable Implementation

	IEnumerable<IWriting> IReadable.Writings => Readables.OfType<IWriting>();

	IEnumerable<IDrawing> IReadable.Drawings => Readables.OfType<IDrawing>();

	IEnumerable<ICanBeRead> IReadable.Readables => Readables;

	#endregion

	#region IWriteable Implementation

	public List<ICanBeRead> Readables { get; } = new();

	public int DocumentLengthUsed => Readables.Sum(x => x.DocumentLength);

	public bool HasSpareRoom => DocumentLengthUsed < _prototype.MaximumCharacterLengthOfText;

	public string Title { get; set; }

	public bool CanAddWriting(IWriting writing)
	{
		if (writing.DocumentLength > _prototype.MaximumCharacterLengthOfText - DocumentLengthUsed)
		{
			return false;
		}

		return true;
	}

	public bool AddWriting(IWriting newWriting)
	{
		if (!CanAddWriting(newWriting))
		{
			return false;
		}

		Readables.Add(newWriting);
		Gameworld.Add(newWriting);
		Changed = true;
		return true;
	}

	public bool CanWrite(ICharacter character, IWritingImplement implement, IWriting writing)
	{
		if (Parent.Quantity > 1)
		{
			return false;
		}

		if (implement != null)
		{
			switch (implement.WritingImplementType)
			{
				case WritingImplementType.ComputerStylus:
				case WritingImplementType.Stylus:
				case WritingImplementType.Chisel:
					return false;
			}

			if (!implement.Primed)
			{
				return false;
			}
		}

		if (writing != null &&
		    writing.DocumentLength > _prototype.MaximumCharacterLengthOfText - DocumentLengthUsed)
		{
			return false;
		}

		return true;
	}

	public string WhyCannotWrite(ICharacter character, IWritingImplement implement, IWriting writing)
	{
		if (Parent.Quantity > 1)
		{
			return "You must separate a single sheet of paper from the stack in order to write on it.";
		}

		if (implement != null)
		{
			switch (implement.WritingImplementType)
			{
				case WritingImplementType.ComputerStylus:
				case WritingImplementType.Stylus:
				case WritingImplementType.Chisel:
					return
						$"{implement.Parent.HowSeen(character)} is not an appropriate writing instrument for writing on {Parent.HowSeen(character)}.";
			}

			if (!implement.Primed)
			{
				return $"{implement.Parent.HowSeen(character)} has not yet been primed for writing.";
			}
		}

		if (writing != null && writing.DocumentLength > _prototype.MaximumCharacterLengthOfText - DocumentLengthUsed)
		{
			return $"There is not enough space left on {Parent.HowSeen(character)} to write all that.";
		}

		throw new ApplicationException("Unknown WhyCannotWrite reason in PaperSheetGameItemComponent.");
	}

	public bool Write(ICharacter character, IWritingImplement implement, IWriting writing)
	{
		if (!CanWrite(character, implement, writing))
		{
			character.Send(WhyCannotWrite(character, implement, writing));
			return false;
		}

		var difficulty = (Difficulty)Math.Max(0, Math.Min(10, (writing.Style.MinimumHandwritingSkill() + 10.0) / 15.0));
		var check = Gameworld.GetCheck(CheckType.HandwritingImprovementCheck);
		for (var i = 0; i < 3; i++)
		{
			check.Check(character, difficulty);
		}

		Readables.Add(writing);
		implement?.Use(writing.DocumentLength);
		Gameworld.Add(writing);
		Changed = true;
		return true;
	}

	public string WhyCannotGiveTitle(ICharacter character, string title)
	{
		throw new ApplicationException(
			"PaperSheetGameItemComponent had WhyCannotGiveTitle called - which is an invalid operation.");
	}

	public bool CanGiveTitle(ICharacter character, string title)
	{
		return true;
	}

	public bool GiveTitle(ICharacter character, string title)
	{
		Title = title;
		Changed = true;
		return true;
	}

	public bool CanAddDrawing(IDrawing drawing)
	{
		if (drawing.DocumentLength > _prototype.MaximumCharacterLengthOfText - DocumentLengthUsed)
		{
			return false;
		}

		return true;
	}

	public bool AddDrawing(IDrawing drawing)
	{
		if (!CanAddDrawing(drawing))
		{
			return false;
		}

		Readables.Add(drawing);
		Gameworld.Add(drawing);
		Changed = true;
		return true;
	}

	/// <summary>
	/// Determines whether the character can draw on this writeable with the given implement and text
	/// </summary>
	/// <param name="character">The character doing the drawing</param>
	/// <param name="implement">The writing implement</param>
	/// <param name="drawing">The proposed drawing to put on the writeable. Can be null if this is a check prior to having dropped into the editor.</param>
	/// <returns>True if the character can draw on the proposed writeable in the proposed way</returns>
	public bool CanDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)
	{
		if (Parent.Quantity > 1)
		{
			return false;
		}

		if (implement != null)
		{
			switch (implement.WritingImplementType)
			{
				case WritingImplementType.ComputerStylus:
				case WritingImplementType.Stylus:
				case WritingImplementType.Chisel:
					return false;
			}

			if (!implement.Primed)
			{
				return false;
			}
		}

		if (drawing != null &&
		    drawing.DocumentLength > _prototype.MaximumCharacterLengthOfText - DocumentLengthUsed)
		{
			return false;
		}

		return true;
	}

	public string WhyCannotDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)
	{
		if (Parent.Quantity > 1)
		{
			return "You must separate a single sheet of paper from the stack in order to draw on it.";
		}

		if (implement != null)
		{
			switch (implement.WritingImplementType)
			{
				case WritingImplementType.ComputerStylus:
				case WritingImplementType.Stylus:
				case WritingImplementType.Chisel:
					return
						$"{implement.Parent.HowSeen(character)} is not an appropriate writing instrument for drawing on {Parent.HowSeen(character)}.";
			}

			if (!implement.Primed)
			{
				return $"{implement.Parent.HowSeen(character)} has not yet been primed for drawing.";
			}
		}

		if (drawing != null && drawing.DocumentLength > _prototype.MaximumCharacterLengthOfText - DocumentLengthUsed)
		{
			return $"There is not enough space left on {Parent.HowSeen(character)} to draw a drawing of that size.";
		}

		throw new ApplicationException("Unknown WhyCannotDraw reason in PaperSheetGameItemComponent.");
	}

	public bool Draw(ICharacter character, IWritingImplement implement, IDrawing drawing)
	{
		if (!CanDraw(character, implement, drawing))
		{
			character.Send(WhyCannotDraw(character, implement, drawing));
			return false;
		}

		var difficulty = Difficulty.Easy;
		switch (drawing.DrawingSize)
		{
			case DrawingSize.Doodle:
				difficulty = Difficulty.Normal;
				break;
			case DrawingSize.Figure:
				difficulty = Difficulty.Hard;
				break;
			case DrawingSize.Sketch:
				difficulty = Difficulty.VeryHard;
				break;
			case DrawingSize.Picture:
				difficulty = Difficulty.ExtremelyHard;
				break;
			case DrawingSize.Mural:
				difficulty = Difficulty.Insane;
				break;
		}

		var check = Gameworld.GetCheck(CheckType.DrawingImprovementCheck);
		for (var i = 0; i < 5; i++)
		{
			check.Check(character, difficulty);
		}

		Readables.Add(drawing);
		implement?.Use(drawing.DocumentLength);
		Gameworld.Add(drawing);
		Changed = true;
		return true;
	}

	#endregion

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short || type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return DecorateShortDesc(voyeur, description);
			case DescriptionType.Full:
				return DecorateFullDesc(voyeur, description);
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	private string DecorateFullDesc(IPerceiver voyeur, string description)
	{
		var sb = new StringBuilder();
		if (voyeur is not ICharacter character)
		{
			return description;
		}

		sb.AppendLine(description);
		sb.AppendLine();
		sb.AppendLine(
			$"This item can contain {_prototype.MaximumCharacterLengthOfText.ToString("N0", voyeur).ColourValue()} characters of written text.");
		sb.AppendLine($"It is {(string.IsNullOrEmpty(Title) ? "not currently titled" : $"titled as \"{Title.Colour(Telnet.BoldWhite)}\"")}.");
		if (!Readables.Any())
		{
			sb.AppendLine("It does not presently have anything written or drawn on it.");
		}
		else
		{
			var itemNum = 1;
			sb.AppendLine(
				$"It has {Readables.Count.ToString("N0", voyeur).ColourValue()} separate pieces of writing and drawing. Type {"read <number>".Colour(Telnet.Yellow)} to read each piece:\n");

			foreach (var item in Readables)
			{
				sb.AppendLine($"\t#{(itemNum++).ToString("N0", voyeur)}) {item.DescribeInLook(character)}");
			}
		}

		return sb.ToString();
	}

	private string DecorateShortDesc(IPerceiver voyeur, string description)
	{
		if (string.IsNullOrEmpty(Title))
		{
			return description;
		}

		if (voyeur is ILanguagePerceiver lp && !lp.IsLiterate)
		{
			return description;
		}

		var titleSetting = Gameworld.GetStaticConfiguration("WrittenItemSDescStyle");
		switch (titleSetting.ToLowerInvariant())
		{
			case "title":
				return $"\"{Title}\"";
			case "desc":
				return description;
			case "desc+title":
				return $"{description} titled {"\"".ColourBold(Telnet.White)}{Title.ColourBold(Telnet.White)}\"";
			default:
				throw new ApplicationException(
					$"Invalid option for WrittenItemSDescStyle: '{titleSetting}'. Valid options are 'title', 'desc' and 'desc+title'");
		}
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		if (component is PaperSheetGameItemComponent psc && psc.Readables.Any())
		{
			return true;
		}

		return false;
	}
}