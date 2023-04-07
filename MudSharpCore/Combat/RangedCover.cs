using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public class RangedCover : FrameworkItem, IRangedCover
{
	public RangedCover(MudSharp.Models.RangedCover cover)
	{
		_id = cover.Id;
		_name = cover.Name;
		CoverType = (CoverType)cover.CoverType;
		CoverExtent = (CoverExtent)cover.CoverExtent;
		HighestPositionState = PositionState.GetState(cover.HighestPositionState);
		DescriptionString = cover.DescriptionString;
		ActionDescriptionString = cover.ActionDescriptionString;
		MaximumSimultaneousCovers = cover.MaximumSimultaneousCovers;
		CoverStaysWhileMoving = cover.CoverStaysWhileMoving;
	}

	public CoverType CoverType { get; set; }
	public CoverExtent CoverExtent { get; set; }
	public IPositionState HighestPositionState { get; set; }
	public string DescriptionString { get; set; }
	public string ActionDescriptionString { get; set; }
	public int MaximumSimultaneousCovers { get; set; }
	public bool CoverStaysWhileMoving { get; set; }

	public string Describe(ICharacter covered, IPerceivable coverProvider, IPerceiver voyeur)
	{
		return new EmoteOutput(new NoFormatEmote(DescriptionString, covered, coverProvider)).ParseFor(voyeur);
	}

	public IEmote DescribeAction(ICharacter covered, IPerceivable coverProvider)
	{
		return new Emote(ActionDescriptionString, covered, covered, coverProvider);
	}

	public override string FrameworkItemType => "RangedCover";

	public Difficulty MinimumRangedDifficulty
	{
		get
		{
			switch (CoverExtent)
			{
				case CoverExtent.Marginal:
					return Difficulty.Hard;
				case CoverExtent.Partial:
					return Difficulty.VeryHard;
				case CoverExtent.NearTotal:
					return Difficulty.ExtremelyHard;
				case CoverExtent.Total:
					return Difficulty.Impossible;
				default:
					return Difficulty.Normal;
			}
		}
	}

	#region Implementation of IKeyworded

	public IEnumerable<string> Keywords => Name.Split(' ').ToList();

	#endregion
}