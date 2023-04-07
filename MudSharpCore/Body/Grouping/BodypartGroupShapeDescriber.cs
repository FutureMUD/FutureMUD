using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp.Body.Grouping;

public class BodypartGroupShapeDescriber : BodypartGroupDescriber
{
	public BodypartGroupShapeDescriber()
	{
		Initialise();
	}

	public BodypartGroupShapeDescriber(MudSharp.Models.BodypartGroupDescriber rule, IFuturemud gameworld)
	{
		// TODO: Update database to reflect class redesign

		Initialise();
		Comment = rule.Comment;
		DescribedAs = rule.DescribedAs;
		_id = rule.Id;
		foreach (var shape in rule.BodypartGroupDescribersShapeCount)
		{
			ShapeCount.Add(gameworld.BodypartShapes.Get(shape.TargetId),
				Tuple.Create(shape.MinCount, shape.MaxCount));
		}
	}

	public override string FrameworkItemType => "BodypartGroupShapeDescriber";

	public Dictionary<IBodypartShape, Tuple<int, int>> ShapeCount { get; protected set; }

	protected void Initialise()
	{
		ShapeCount = new Dictionary<IBodypartShape, Tuple<int, int>>();
	}

	public int MatchCount(IEnumerable<IBodypart> shapes)
	{
		return ShapeCount.Sum(x => Math.Min(shapes.Count(y => y.Shape == x.Key), x.Value.Item2));
	}

	public IEnumerable<IBodypart> RemainderShapes(IEnumerable<IBodypart> shapes)
	{
		return
			shapes.Select(x => x.Shape)
			      .Distinct()
			      .SelectMany(
				      x =>
					      ShapeCount.ContainsKey(x)
						      ? shapes.Where(y => y.Shape == x).Skip(ShapeCount[x].Item2)
						      : shapes.Where(y => y.Shape == x));
	}

	public override BodypartGroupResult Match(IEnumerable<IBodypart> parts)
	{
		if (ShapeCount.All(x => parts.Count(y => y.Shape == x.Key) >= x.Value.Item1))
		{
			var remains = RemainderShapes(parts);
			return new BodypartGroupResult(true, MatchCount(parts), DescribedAs,
				parts.Where(x => !remains.Contains(x)), remains);
		}

		return new BodypartGroupResult(false, 0);
	}
}