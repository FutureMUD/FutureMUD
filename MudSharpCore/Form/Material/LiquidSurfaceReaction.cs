using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Health;

namespace MudSharp.Form.Material;

public class LiquidSurfaceReaction : ILiquidSurfaceReaction
{
	private readonly IFuturemud _gameworld;
	private readonly List<ITag> _targetTags = new();

	public LiquidSurfaceReaction(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public LiquidSurfaceReaction(ILiquidSurfaceReaction rhs, IFuturemud gameworld)
	{
		_gameworld = gameworld;
		_targetTags.AddRange(rhs.TargetTags);
		DamageType = rhs.DamageType;
		DamagePerTick = rhs.DamagePerTick;
		PainPerTick = rhs.PainPerTick;
		StunPerTick = rhs.StunPerTick;
	}

	public LiquidSurfaceReaction(XElement root, IFuturemud gameworld)
	{
		_gameworld = gameworld;
		DamageType = (DamageType)int.Parse(root.Attribute("DamageType")?.Value ?? "0");
		DamagePerTick = double.Parse(root.Attribute("DamagePerTick")?.Value ?? "0");
		PainPerTick = double.Parse(root.Attribute("PainPerTick")?.Value ?? "0");
		StunPerTick = double.Parse(root.Attribute("StunPerTick")?.Value ?? "0");
		foreach (var tag in root.Element("Tags")?.Elements("Tag") ?? Enumerable.Empty<XElement>())
		{
			if (!long.TryParse(tag.Value, out var value))
			{
				continue;
			}

			var loadedTag = _gameworld.Tags.Get(value);
			if (loadedTag is not null)
			{
				_targetTags.Add(loadedTag);
			}
		}
	}

	public IEnumerable<ITag> TargetTags => _targetTags;
	public DamageType DamageType { get; set; }
	public double DamagePerTick { get; set; }
	public double PainPerTick { get; set; }
	public double StunPerTick { get; set; }
}
