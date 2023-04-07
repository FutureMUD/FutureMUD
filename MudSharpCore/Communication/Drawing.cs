using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Communication;

public static class DrawingExtensions
{
	public static int DocumentLength(this DrawingSize size)
	{
		switch (size)
		{
			case DrawingSize.Scribble:
				return Futuremud.Games.First().GetStaticInt("DocumentLengthDrawingScribble");
			case DrawingSize.Doodle:
				return Futuremud.Games.First().GetStaticInt("DocumentLengthDrawingDoodle");
			case DrawingSize.Figure:
				return Futuremud.Games.First().GetStaticInt("DocumentLengthDrawingFigure");
			case DrawingSize.Sketch:
				return Futuremud.Games.First().GetStaticInt("DocumentLengthDrawingSketch");
			case DrawingSize.Picture:
				return Futuremud.Games.First().GetStaticInt("DocumentLengthDrawingPicture");
			case DrawingSize.Poster:
				return Futuremud.Games.First().GetStaticInt("DocumentLengthDrawingPoster");
			case DrawingSize.Mural:
				return Futuremud.Games.First().GetStaticInt("DocumentLengthDrawingMural");
		}

		throw new ArgumentOutOfRangeException(nameof(size));
	}
}

public class Drawing : LateInitialisingItem, IDrawing, ILazyLoadDuringIdleTime
{
	public override string FrameworkItemType => "Drawing";

	public Drawing(MudSharp.Models.Drawing dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		IdInitialised = true;
		ShortDescription = dbitem.ShortDescription;
		FullDescription = dbitem.FullDescription;
		ImplementType = (WritingImplementType)dbitem.ImplementType;
		DrawingSkill = dbitem.DrawingSkill;
		DrawingSize = (DrawingSize)dbitem.DrawingSize;
		_authorId = dbitem.AuthorId;
		Gameworld.SaveManager.AddLazyLoad(this);
	}

	public Drawing(Drawing rhs)
	{
		Gameworld = rhs.Gameworld;
		Gameworld.SaveManager.AddInitialisation(this);
		_authorId = rhs._authorId;
		_author = rhs.Author;
		ShortDescription = rhs.ShortDescription;
		FullDescription = rhs.FullDescription;
		ImplementType = rhs.ImplementType;
		DrawingSkill = rhs.DrawingSkill;
		DrawingSize = rhs.DrawingSize;
	}

	public Drawing(ICharacter author, double skill, string shortDescription, string fullDescription,
		WritingImplementType implementType, DrawingSize size)
	{
		Gameworld = author.Gameworld;
		Gameworld.SaveManager.AddInitialisation(this);
		_authorId = author.Id;
		_author = author;
		ShortDescription = shortDescription;
		FullDescription = fullDescription;
		ImplementType = implementType;
		DrawingSkill = skill;
		DrawingSize = size;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Drawings.Find(Id);
		dbitem.ShortDescription = ShortDescription;
		dbitem.FullDescription = FullDescription;
		dbitem.ImplementType = (int)ImplementType;
		dbitem.DrawingSkill = DrawingSkill;
		dbitem.DrawingSize = (int)DrawingSize;
		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Models.Drawing();
		FMDB.Context.Drawings.Add(dbitem);
		dbitem.AuthorId = _authorId;
		dbitem.ShortDescription = ShortDescription;
		dbitem.FullDescription = FullDescription;
		dbitem.ImplementType = (int)ImplementType;
		dbitem.DrawingSkill = DrawingSkill;
		dbitem.DrawingSize = (int)DrawingSize;
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((MudSharp.Models.Drawing)dbitem).Id;
	}

	public int DocumentLength
	{
		get
		{
			switch (DrawingSize)
			{
				case DrawingSize.Scribble:
					return Gameworld.GetStaticInt("DocumentLengthDrawingScribble");
				case DrawingSize.Doodle:
					return Gameworld.GetStaticInt("DocumentLengthDrawingDoodle");
				case DrawingSize.Figure:
					return Gameworld.GetStaticInt("DocumentLengthDrawingFigure");
				case DrawingSize.Sketch:
					return Gameworld.GetStaticInt("DocumentLengthDrawingSketch");
				case DrawingSize.Picture:
					return Gameworld.GetStaticInt("DocumentLengthDrawingPicture");
				case DrawingSize.Poster:
					return Gameworld.GetStaticInt("DocumentLengthDrawingPoster");
				case DrawingSize.Mural:
					return Gameworld.GetStaticInt("DocumentLengthDrawingMural");
			}

			return 0;
		}
	}

	public WritingImplementType ImplementType { get; protected set; }
	public string ShortDescription { get; protected set; }
	public string FullDescription { get; protected set; }
	public DrawingSize DrawingSize { get; protected set; }

	private long _authorId;
	private ICharacter _author;

	public ICharacter Author
	{
		get
		{
			if (_author == null)
			{
				_author = Gameworld.TryGetCharacter(_authorId, true);
			}

			return _author;
		}
	}

	public double DrawingSkill { get; protected set; }

	public IDrawing Copy()
	{
		return new Drawing(this);
	}

	public void DoLoad()
	{
		if (_author == null && _authorId != 0)
		{
			_author = Gameworld.TryGetCharacter(_authorId, true);
		}
	}

	public string ParseFor(ICharacter voyeur)
	{
		return FullDescription;
	}

	public string DescribeInLook(ICharacter voyeur)
	{
		return $"{ShortDescription.Colour(Telnet.BoldCyan)} ({DrawingSize.DescribeEnum()})";
	}
}