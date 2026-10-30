#nullable enable

using MudSharp.Form.Material;

namespace MudSharp.GameItems.Interfaces;

/// <summary>A vessel that can publish prepared contents without running exposure or authored callbacks.</summary>
public interface IPreparedLiquidContainer : ILiquidContainer
{
	/// <summary>Replaces already validated contents using raw writes only.</summary>
	void CommitPreparedLiquidMixture(LiquidMixture mixture);
	/// <summary>Marks the committed contents for saving after the purchase has published.</summary>
	void FinishPreparedLiquidMixture();
}
