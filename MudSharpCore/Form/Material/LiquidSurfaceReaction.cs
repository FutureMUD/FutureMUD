using MudSharp.Combat;
using MudSharp.Health;
using System.Globalization;

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
		DamagePerTick = double.Parse(root.Attribute("DamagePerTick")?.Value ?? "0", CultureInfo.InvariantCulture);
		PainPerTick = double.Parse(root.Attribute("PainPerTick")?.Value ?? "0", CultureInfo.InvariantCulture);
		StunPerTick = double.Parse(root.Attribute("StunPerTick")?.Value ?? "0", CultureInfo.InvariantCulture);
        foreach (XElement tag in root.Element("Tags")?.Elements("Tag") ?? Enumerable.Empty<XElement>())
        {
            if (!long.TryParse(tag.Value, out long value))
            {
                continue;
            }

            ITag loadedTag = _gameworld.Tags.Get(value);
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

	public bool ToggleTargetTag(ITag tag)
	{
		if (_targetTags.Remove(tag))
		{
			return false;
		}

		_targetTags.Add(tag);
		return true;
	}
}
