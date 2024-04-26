using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Communication;

/// <summary>
/// Dummy drawings are used specifically in the DRAW command only
/// </summary>
public class DummyDrawing : IDrawing
{
	public DrawingSize DrawingSize { get; init; }
	public string ShortDescription { get; set; }
	public string FullDescription { get; set; }
	public double DrawingSkill { get; set; }

	public IDrawing Copy()
	{
		throw new InvalidOperationException();
	}

	public int DocumentLength => DrawingSize.DocumentLength();
	public ICharacter Author { get; init; }
	public WritingImplementType ImplementType { get; init; }

	public string ParseFor(ICharacter voyeur)
	{
		return FullDescription;
	}

	public string DescribeInLook(ICharacter voyeur)
	{
		return ShortDescription;
	}

	public string Name => string.Empty;
	public long Id => 0;
	public string FrameworkItemType => "Writing";
	public bool Changed { get; set; }

	public void Save()
	{
		throw new InvalidOperationException();
	}

	public IFuturemud Gameworld { get; }
}