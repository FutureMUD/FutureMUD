#nullable enable

using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Health;

namespace MudSharp.Climate;

internal static class WeatherHazardService
{
	internal static bool AtmosphereExposed(ICell cell) => cell.OutdoorsType(null) is CellOutdoorsType.Outdoors or CellOutdoorsType.IndoorsClimateExposed;
	internal static IFluid? WeatherAtmosphere(ICell cell) => AtmosphereExposed(cell) && cell.CurrentWeather(null)?.Hazards is { AtmosphereGasId: > 0 } hazard
		? cell.Gameworld.Gases.Get(hazard.AtmosphereGasId) : null;

	internal static bool Exposed(ICell cell, ICharacter character) => !cell.IsUnderwaterLayer(character.RoomLayer) &&
		!(character.PositionModifier == PositionModifier.Under && character.PositionTarget is { } shelter &&
		  shelter.Size > character.CurrentContextualSize(SizeContext.RainfallExposure));
	internal static bool Exposed(ICell cell, IGameItem item) => !cell.IsUnderwaterLayer(item.RoomLayer) &&
		item.ContainedIn is null && item.InInventoryOf is null &&
		!(item.PositionModifier == PositionModifier.Under && item.PositionTarget is { } shelter && shelter.Size > item.Size);

	internal static void Tick(IWeatherEvent weather, ICell cell, Func<double> random)
	{
		var config = weather.Hazards;
		if (cell.CurrentWeather(null) != weather || cell.OutdoorsType(null) != CellOutdoorsType.Outdoors ||
			config is null || config.LightningChance <= 0 && config.AtmosphericLightningChance <= 0) return;
		var strike = config.LightningChance > 0 && random() < config.LightningChance;
		if (!strike && !(config.AtmosphericLightningChance > 0 && random() < config.AtmosphericLightningChance)) return;
		foreach (var character in cell.Characters.Where(x => !cell.IsUnderwaterLayer(x.RoomLayer)).ToArray())
			if (character.CanSee(cell)) character.OutputHandler.Send(config.Flash.SubstituteANSIColour());
		cell.HandleAudioEcho(config.Thunder.SubstituteANSIColour(), AudioVolume.ExtremelyLoud,
			config.ThunderDistance, AudioPropagationMode.Topological, new DummyPerceiver("the sky", location: cell),
			RoomLayer.GroundLevel, false, "thunder");
		if (!strike) return;
		var characters = cell.Characters.Where(x => Exposed(cell, x)).ToArray();
		var items = cell.GameItems.Where(x => Exposed(cell, x)).ToArray();
		var choices = new List<(int Event, double Chance)> { (0, config.GroundWeight) };
		if (characters.Length > 0) choices.Add((1, config.CharacterWeight));
		if (items.Length > 0) choices.Add((2, config.ItemWeight));
		var total = choices.Sum(x => x.Chance);
		if (total <= 0) return;
		switch (WeatherSelection.Choose(choices, total, random))
		{
			case 1:
				Strike(characters[Math.Min(characters.Length - 1, (int)(random() * characters.Length))], config, 1);
				break;
			case 2:
				Strike(items[Math.Min(items.Length - 1, (int)(random() * items.Length))], config, 1);
				break;
			default:
				foreach (var character in characters.Where(x => x.RoomLayer == RoomLayer.GroundLevel)) Strike(character, config, config.GroundDamageFactor);
				foreach (var item in items.Where(x => x.RoomLayer == RoomLayer.GroundLevel)) Strike(item, config, config.GroundDamageFactor);
				break;
		}
	}

	internal static void Strike(IHaveWounds target, WeatherHazardSettings config, double factor)
	{
		if (factor <= 0) return;
		var damage = new Damage
		{
			DamageType = DamageType.Electrical, DamageAmount = config.Damage * factor,
			PainAmount = config.Pain * factor, StunAmount = config.Stun * factor,
			Bodypart = (target as ICharacter)?.Body.RandomBodypart,
			AngleOfIncidentRadians = Math.PI / 2
		};
		if (target is ICharacter character)
		{
			character.OutputHandler.Send(factor >= 1 ? "Lightning strikes you!".ColourError() : "A nearby lightning strike sends a shock through you!".ColourError());
			character.SufferDamage(damage).ProcessPassiveWounds();
		}
		else target.PassiveSufferDamage(damage).ProcessPassiveWounds();
	}
}
