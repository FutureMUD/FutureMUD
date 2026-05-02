#nullable enable

using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Effects.Interfaces;

public interface IMagicCraftToolEnhancementEffect : IEffectSubtype
{
	double ToolFitnessBonus { get; }
	double ToolSpeedMultiplier { get; }
	double ToolUsageMultiplier { get; }
	bool AppliesToCraftTool(IGameItem tool, ITag? toolTag);
}
