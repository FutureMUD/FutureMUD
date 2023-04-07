using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Body.Grouping;

/// <summary>
///     Describes a group of IBodyparts based on their identity, i.e. that they share the same Prototype
/// </summary>
public class BodypartGroupIDDescriber : BodypartGroupDescriber
{
	/// <summary>
	///     Contains the list of Prototypes and whether or not they are mandatory
	/// </summary>
	protected readonly Dictionary<IBodypart, bool> Prototypes = new();

	public BodypartGroupIDDescriber(MudSharp.Models.BodypartGroupDescriber describer, IFuturemud gameworld)
	{
		_id = describer.Id;
		DescribedAs = describer.DescribedAs;
		Comment = describer.Comment;
	}

	public override string FrameworkItemType => "BodypartGroupIDDescriber";

	#region IBodypartGroupDescriber Members

	public override BodypartGroupResult Match(IEnumerable<IBodypart> parts)
	{
		if (!Prototypes.Any())
		{
			return new BodypartGroupResult(false, 0);
		}

		var matches = parts.Where(part => Prototypes.ContainsKey(part)).ToList();

		if (Prototypes.Where(x => x.Value).Any(x => matches.All(y => y != x.Key)))
		{
			return new BodypartGroupResult(false, 0);
		}

		return new BodypartGroupResult(true, matches.Count, DescribedAs, matches,
			parts.Where(x => !matches.Contains(x)).ToList());
	}

	#endregion

	public override void FinaliseLoad(MudSharp.Models.BodypartGroupDescriber describer, IFuturemud gameworld)
	{
		var bodyProto = gameworld.BodyPrototypes.Get(describer.BodypartGroupDescribersBodyProtos.Single().BodyProtoId);
		foreach (var part in describer.BodypartGroupDescribersBodypartProtos)
		{
			Prototypes.Add(bodyProto.AllBodyparts.First(x => x.Id == part.BodypartProtoId), part.Mandatory);
		}
	}
}