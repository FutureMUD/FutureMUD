using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Community.Boards;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;

namespace MudSharp.GameItems.Components;

public class BoardGameItemComponent : GameItemComponent, IBoardItem
{
	protected BoardGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;
	public IBoard Board => _prototype.Board;
	public IFutureProg CanViewBoard => _prototype.CanViewBoard;
	public IFutureProg CanPostToBoard => _prototype.CanPostToBoard;
	public string CantViewBoardEcho => _prototype.CantViewBoardEcho;
	public string CantPostToBoardEcho => _prototype.CantPostToBoardEcho;
	public bool ShowAuthorName => _prototype.ShowAuthorName;
	public bool ShowAuthorDescription => _prototype.ShowAuthorDescription;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BoardGameItemComponentProto)newProto;
	}

	#region Constructors

	public BoardGameItemComponent(BoardGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
		parent, proto, temporary)
	{
		_prototype = proto;
	}

	public BoardGameItemComponent(Models.GameItemComponent component, BoardGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public BoardGameItemComponent(BoardGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BoardGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	#region Overrides of GameItemComponent

	public override int DecorationPriority => int.MinValue;

	public override bool DescriptionDecorator(DescriptionType type)
	{
		if (type == DescriptionType.Full)
		{
			return true;
		}

		return base.DescriptionDecorator(type);
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour,
		PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Full && voyeur is ICharacter ch)
		{
			var sb = new StringBuilder();
			sb.AppendLine(description);
			sb.AppendLine(
				$"This is a notice board and you can use the {"board read".ColourCommand()} and {"board write".ColourCommand()} commands here."
					.ColourIncludingReset(Telnet.Cyan));
			if (CanViewBoard.Execute<bool?>(ch) == false)
			{
				sb.AppendLine("\nYou are not permitted to view posts on this board.".ColourCommand());
			}
			else if (!_prototype.Board.Posts.Any())
			{
				sb.AppendLine("\nThe board does not currently have any posts.".ColourCommand());
			}
			else
			{
				sb.AppendLine();
				var i = _prototype.Board.Posts.Count();
				foreach (var post in _prototype.Board.Posts.OrderByDescending(x => x.PostTime)
				                               .ThenByDescending(x => x.Id))
				{
					sb.AppendLine($"\t{i--.ToString("N0", voyeur)}) {post.Title.Colour(Telnet.BoldWhite)}");
				}
			}

			return sb.ToString();
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	#endregion
}