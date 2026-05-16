using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Components;

public class InscribableSurfaceGameItemComponent : GameItemComponent, IWriteable, IReadable
{
	private InscribableSurfaceGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (InscribableSurfaceGameItemComponentProto)newProto;
	}

	#region Constructors

	public InscribableSurfaceGameItemComponent(InscribableSurfaceGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public InscribableSurfaceGameItemComponent(MudSharp.Models.GameItemComponent component,
		InscribableSurfaceGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public InscribableSurfaceGameItemComponent(InscribableSurfaceGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		foreach (var readable in rhs.Readables)
		{
			var newReadable = temporary ? readable : readable.CopyReadable();
			Readables.Add(newReadable);
			if (!temporary)
			{
				RegisterReadable(newReadable);
			}
		}

		Title = rhs.Title;
	}

	private void RegisterReadable(ICanBeRead readable)
	{
		switch (readable)
		{
			case IWriting writing:
				Gameworld.Add(writing);
				break;
			case IDrawing drawing:
				Gameworld.Add(drawing);
				break;
		}
	}

	private void LoadFromXml(XElement root)
	{
		Title = root.Element("Title")?.Value ?? string.Empty;
		foreach (var item in root.Element("Writings")?.Elements() ?? Enumerable.Empty<XElement>())
		{
			if (item.Name.LocalName.EqualTo("Writing"))
			{
				var writing = Gameworld.Writings.Get(long.Parse(item.Value));
				if (writing is not null)
				{
					Readables.Add(writing);
				}
			}
			else
			{
				var drawing = Gameworld.Drawings.Get(long.Parse(item.Value));
				if (drawing is not null)
				{
					Readables.Add(drawing);
				}
			}
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new InscribableSurfaceGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Title", new XCData(Title)),
			new XElement("Writings",
				from item in Readables
				select item is IWriting ? new XElement("Writing", item.Id) : new XElement("Drawing", item.Id))
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

	public string Title { get; private set; } = string.Empty;

	public bool CanAddWriting(IWriting writing)
	{
		return writing.DocumentLength <= _prototype.MaximumCharacterLengthOfText - DocumentLengthUsed;
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

		if (implement is not null)
		{
			if (!_prototype.AllowedImplementTypes.Contains(implement.WritingImplementType))
			{
				return false;
			}

			if (!implement.Primed)
			{
				return false;
			}
		}

		if (writing is not null &&
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
			return "You must separate a single writing surface from the stack in order to write on it.";
		}

		if (implement is not null)
		{
			if (!_prototype.AllowedImplementTypes.Contains(implement.WritingImplementType))
			{
				return
					$"{implement.Parent.HowSeen(character)} is not an appropriate writing instrument for writing on {Parent.HowSeen(character)}.";
			}

			if (!implement.Primed)
			{
				return $"{implement.Parent.HowSeen(character)} has not yet been primed for writing.";
			}
		}

		if (writing is not null && writing.DocumentLength > _prototype.MaximumCharacterLengthOfText - DocumentLengthUsed)
		{
			return $"There is not enough space left on {Parent.HowSeen(character)} to write all that.";
		}

		throw new ApplicationException("Unknown WhyCannotWrite reason in InscribableSurfaceGameItemComponent.");
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
			"InscribableSurfaceGameItemComponent had WhyCannotGiveTitle called - which is an invalid operation.");
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
		return drawing.DocumentLength <= _prototype.MaximumCharacterLengthOfText - DocumentLengthUsed;
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

	public bool CanDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)
	{
		if (Parent.Quantity > 1)
		{
			return false;
		}

		if (implement is not null)
		{
			if (!_prototype.AllowedImplementTypes.Contains(implement.WritingImplementType))
			{
				return false;
			}

			if (!implement.Primed)
			{
				return false;
			}
		}

		if (drawing is not null &&
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
			return "You must separate a single writing surface from the stack in order to draw on it.";
		}

		if (implement is not null)
		{
			if (!_prototype.AllowedImplementTypes.Contains(implement.WritingImplementType))
			{
				return
					$"{implement.Parent.HowSeen(character)} is not an appropriate writing instrument for drawing on {Parent.HowSeen(character)}.";
			}

			if (!implement.Primed)
			{
				return $"{implement.Parent.HowSeen(character)} has not yet been primed for drawing.";
			}
		}

		if (drawing is not null && drawing.DocumentLength > _prototype.MaximumCharacterLengthOfText - DocumentLengthUsed)
		{
			return $"There is not enough space left on {Parent.HowSeen(character)} to draw a drawing of that size.";
		}

		throw new ApplicationException("Unknown WhyCannotDraw reason in InscribableSurfaceGameItemComponent.");
	}

	public bool Draw(ICharacter character, IWritingImplement implement, IDrawing drawing)
	{
		if (!CanDraw(character, implement, drawing))
		{
			character.Send(WhyCannotDraw(character, implement, drawing));
			return false;
		}

		var difficulty = drawing.DrawingSize switch
		{
			DrawingSize.Doodle => Difficulty.Normal,
			DrawingSize.Figure => Difficulty.Hard,
			DrawingSize.Sketch => Difficulty.VeryHard,
			DrawingSize.Picture => Difficulty.ExtremelyHard,
			DrawingSize.Poster => Difficulty.ExtremelyHard,
			DrawingSize.Mural => Difficulty.Insane,
			_ => Difficulty.Easy
		};

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
}
