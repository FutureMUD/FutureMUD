using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;

#nullable enable

namespace MudSharp.Body.Implementations;

public partial class Body
{
	private SurfaceLiquidState? _surfaceLiquidState;
	private bool _surfaceLiquidChanged;

	public ISurfaceLiquidState SurfaceLiquidState => _surfaceLiquidState ??= new SurfaceLiquidState(Gameworld, SurfaceLiquidChanged);

	public void SurfaceLiquidChanged()
	{
		if (_noSave || _loading)
		{
			return;
		}

		_surfaceLiquidChanged = true;
		Changed = true;
		EnsureSurfaceContaminationEffect();
	}

	private void LoadSurfaceLiquidState(string? xml)
	{
		_surfaceLiquidState = string.IsNullOrWhiteSpace(xml)
			? new SurfaceLiquidState(Gameworld, SurfaceLiquidChanged)
			: new SurfaceLiquidState(Gameworld, XElement.Parse(xml), SurfaceLiquidChanged);
		var effectsChanged = EffectsChanged;
		var changed = Changed;
		EnsureSurfaceContaminationEffect();
		EffectsChanged = effectsChanged;
		Changed = changed;
		_surfaceLiquidChanged = false;
	}

	private string? SaveSurfaceLiquidState()
	{
		return SurfaceLiquidState.IsEmpty ? null : SurfaceLiquidState.SaveToXml().ToString();
	}

	private void ResolveSurfaceLiquidDrying()
	{
		if (_surfaceLiquidState is null || _surfaceLiquidState.ContaminatingLiquid.IsEmpty)
		{
			return;
		}

		var duration = TimeSpan.FromSeconds(Math.Max(1.0,
			Gameworld.GetStaticDouble("BodyLiquidContaminationEffectDuration") *
			Math.Max(_surfaceLiquidState.ContaminatingLiquid.RelativeEnthalpy, double.Epsilon)));
		if (_surfaceLiquidState.ResolveDrying(duration, 0.02 / Gameworld.UnitManager.BaseFluidToLitres, 0.1))
		{
			EnsureSurfaceContaminationEffect();
		}
	}

	private void EnsureSurfaceContaminationEffect()
	{
		if (SurfaceLiquidState.IsEmpty)
		{
			foreach (var effect in EffectsOfType<SurfaceContaminationEffect>().ToList())
			{
				RemoveEffect(effect, true);
			}
			return;
		}

		if (!EffectsOfType<SurfaceContaminationEffect>().Any())
		{
			AddEffect(new SurfaceContaminationEffect(this));
		}
	}
}
