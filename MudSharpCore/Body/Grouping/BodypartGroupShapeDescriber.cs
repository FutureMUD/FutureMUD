using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg.Functions.BuiltIn;
using MudSharp.Models;

namespace MudSharp.Body.Grouping;

public class BodypartGroupShapeDescriber : BodypartGroupDescriber
{
	public BodypartGroupShapeDescriber(MudSharp.Models.BodypartGroupDescriber rule, IFuturemud gameworld)
	{
		Comment = rule.Comment;
		DescribedAs = rule.DescribedAs;
		_id = rule.Id;
		foreach (var shape in rule.BodypartGroupDescribersShapeCount)
		{
			ShapeCount.Add(gameworld.BodypartShapes.Get(shape.TargetId),
				(shape.MinCount, shape.MaxCount));
		}
	}

	private BodypartGroupShapeDescriber(BodypartGroupShapeDescriber rhs)
	{
		Gameworld = rhs.Gameworld;
		DescribedAs = rhs.DescribedAs;
		Comment = rhs.Comment;
		foreach (var item in rhs.ShapeCount)
		{
			ShapeCount[item.Key] = item.Value;
		}
		DoDatabaseInsert();
	}

	public BodypartGroupShapeDescriber(IFuturemud gameworld, string describedAs, IBodyPrototype body)
	{
		Gameworld = gameworld;
		DescribedAs = describedAs;
		Comment = string.Empty;
		BodyPrototype = body;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	public override IBodypartGroupDescriber Clone()
	{
		return new BodypartGroupShapeDescriber(this);
	}

	/// <inheritdoc />
	protected override string SubtypeHelpText => @"	#3shape <which> <min> <max>#0 - sets a shape to be included
	#3shape <which> remove#0 - removes a shape";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "shape":
				return BuildingCommandShape(actor, ss);
		}
		return base.BuildingCommand(actor, ss.GetUndo());
	}

	private bool BuildingCommandShape(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a shape.");
			return false;
		}

		var shape = Gameworld.BodypartShapes.GetByIdOrName(ss.PopSpeech());
		if (shape is null)
		{
			actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} does not represent a valid bodypart shape.");
			return false;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a minimum and maximum count for that shape, or the keyword remove to remove it.");
			return false;
		}

		if (ss.SafeRemainingArgument.EqualTo("remove"))
		{
			ShapeCount.Remove(shape);
			Changed = true;
			actor.OutputHandler.Send($"The bodypart shape {shape.Name.ColourValue()} will no longer be included in this group.");
			return false;
		}

		if (!int.TryParse(ss.PopSpeech(), out var min) || min < 0)
		{
			actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid number 0 or greater.");
			return false;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a maximum number of this shape.");
			return false;
		}

		if (!int.TryParse(ss.PopSpeech(), out var max) || max < 0)
		{
			actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid number 0 or greater.");
			return false;
		}

		if (min == 0 && max == 0)
		{
			ShapeCount.Remove(shape);
			Changed = true;
			actor.OutputHandler.Send($"This group will no longer consider the {shape.Name.ColourValue()} shape.");
			return true;
		}

		if (min > max)
		{
			(min, max) = (max, min);
		}

		ShapeCount[shape] = (min, max);
		Changed = true;
		actor.OutputHandler.Send($"This group will now require minimum {min.ToStringN0Colour(actor)} and maximum {max.ToStringN0Colour(actor)} of the {shape.Name.ColourValue()} shape.");
		return true;
	}

	/// <inheritdoc />
	public override void FinaliseLoad(Models.BodypartGroupDescriber describer, IFuturemud gameworld)
	{
		if (describer.BodyProtoId is not null)
		{
			BodyPrototype = Gameworld.BodyPrototypes.Get(describer.BodyProtoId.Value)!;
		}
		else
		{
			BodyPrototype = gameworld.BodyPrototypes.Get(describer.BodypartGroupDescribersBodyProtos.Single().BodyProtoId)!;
			describer.BodyProtoId = BodyPrototype.Id;
			Changed = true;
		}
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.BodypartGroupDescribers.Find(Id);
		dbitem.Comment = Comment;
		dbitem.DescribedAs = DescribedAs;
		dbitem.BodyProtoId = BodyPrototype.Id;
		FMDB.Context.BodypartGroupDescribersShapeCount.RemoveRange(dbitem.BodypartGroupDescribersShapeCount);
		foreach (var item in ShapeCount)
		{
			dbitem.BodypartGroupDescribersShapeCount.Add(new Models.BodypartGroupDescribersShapeCount
			{
				BodypartGroupDescriptionRule = dbitem,
				TargetId = item.Key.Id,
				MaxCount = item.Value.Max,
				MinCount = item.Value.Min
			});
		}
		Changed = false;
	}

	private void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.BodypartGroupDescriber
			{
				Comment = Comment,
				DescribedAs = DescribedAs,
				Type = "shape",
				BodyProtoId = BodyPrototype.Id,
			};
			FMDB.Context.BodypartGroupDescribers.Add(dbitem);
			foreach (var item in ShapeCount)
			{
				dbitem.BodypartGroupDescribersShapeCount.Add(new BodypartGroupDescribersShapeCount()
				{
					BodypartGroupDescriptionRule = dbitem,
					MinCount = item.Value.Min,
					MaxCount = item.Value.Max
				});
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Bodypart Group Describer #{Id.ToStringN0(actor)}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {"Shape".ColourValue()}");
		sb.AppendLine($"Body: {BodyPrototype.Name.ColourValue()}");
		sb.AppendLine($"Described As: {DescribedAs.ColourValue()}");
		sb.AppendLine($"Comment: {Comment.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Shapes:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in ShapeCount
			select new List<string>
			{
				item.Key.Id.ToStringN0(actor),
				item.Key.Name,
				item.Value.Min.ToStringN0(actor),
				item.Value.Max.ToStringN0(actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Min #",
				"Max #"
			},
			actor,
			Telnet.Yellow
		));
		return sb.ToString();
	}

	public Dictionary<IBodypartShape, (int Min, int Max)> ShapeCount { get; } = new();

	public int MatchCount(IEnumerable<IBodypart> shapes)
	{
		return ShapeCount.Sum(x => Math.Min(shapes.Count(y => y.Shape == x.Key), x.Value.Max));
	}

	public IEnumerable<IBodypart> RemainderShapes(IEnumerable<IBodypart> shapes)
	{
		return
			shapes.Select(x => x.Shape)
			      .Distinct()
			      .SelectMany(
				      x =>
					      ShapeCount.ContainsKey(x)
						      ? shapes.Where(y => y.Shape == x).Skip(ShapeCount[x].Max)
						      : shapes.Where(y => y.Shape == x));
	}

	public override BodypartGroupResult Match(IEnumerable<IBodypart> parts)
	{
		if (ShapeCount.All(x => parts.Count(y => y.Shape == x.Key) >= x.Value.Min))
		{
			var remains = RemainderShapes(parts);
			return new BodypartGroupResult(true, MatchCount(parts), DescribedAs,
				parts.Where(x => !remains.Contains(x)), remains);
		}

		return new BodypartGroupResult(false, 0);
	}
}