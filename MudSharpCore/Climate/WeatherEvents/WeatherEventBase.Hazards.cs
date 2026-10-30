#nullable enable

using System.Globalization;
using MudSharp.Form.Material;

namespace MudSharp.Climate.WeatherEvents;

public abstract partial class WeatherEventBase
{
	protected string HazardShow(ICharacter actor) => $"Forecast: {Hazards.ForecastDescription.ColourValue()}\n" +
		$"Atmosphere: {(Gameworld.Gases.Get(Hazards.AtmosphereGasId)?.Name ?? "Unchanged").ColourValue()}\n" +
		$"Residue: {(Gameworld.Materials.Get(Hazards.ResidueMaterialId)?.Name ?? "None").ColourValue()}; per minute/maximum: {Gameworld.UnitManager.Describe(Hazards.ResiduePerMinute, Framework.Units.UnitType.Mass, actor)}/{Gameworld.UnitManager.Describe(Hazards.ResidueMaximum, Framework.Units.UnitType.Mass, actor)}\n" +
		$"Lightning per minute: {Hazards.LightningChance.ToString("P4", actor)}; atmospheric: {Hazards.AtmosphericLightningChance.ToString("P2", actor)}\n" +
		$"Lightning damage/pain/stun: {Hazards.Damage.ToStringN2(actor)}/{Hazards.Pain.ToStringN2(actor)}/{Hazards.Stun.ToStringN2(actor)}; ground factor: {Hazards.GroundDamageFactor.ToString("P0", actor)}\n" +
		$"Target weights (ground/character/item): {Hazards.GroundWeight}/{Hazards.CharacterWeight}/{Hazards.ItemWeight}; thunder distance: {Hazards.ThunderDistance}\n" +
		$"Flash: {Hazards.Flash}\nThunder: {Hazards.Thunder}";

	private bool BuildingCommandHazard(ICharacter actor, StringStack command, string option)
	{
		var candidate = Hazards;
		if (option == "forecast") candidate = candidate with { ForecastDescription = command.SafeRemainingArgument };
		else if (option == "residue")
		{
			var name = command.PopSpeech();
			if (name.EqualTo("none")) candidate = candidate with { ResidueMaterialId = 0, ResiduePerMinute = 0, ResidueMaximum = 0 };
			else
			{
				var material = Gameworld.Materials.GetByIdOrName(name);
				if (material is null || !Gameworld.UnitManager.TryGetBaseUnits(command.PopSpeech(), Framework.Units.UnitType.Mass, actor, out var amount) ||
					!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, Framework.Units.UnitType.Mass, actor, out var maximum) ||
					!double.IsFinite(amount) || !double.IsFinite(maximum) || amount <= 0 || maximum <= 0)
				{ actor.OutputHandler.Send("Use residue <material> <mass-per-minute> <maximum-mass>, e.g. residue dust 0.1g 5g; or residue none."); return false; }
				candidate = candidate with { ResidueMaterialId = material.Id, ResiduePerMinute = amount, ResidueMaximum = maximum };
			}
		}
		else if (option == "atmosphere")
		{
			var argument = command.SafeRemainingArgument;
			var gas = Gameworld.Gases.GetByIdOrName(argument);
			if (gas is null && !argument.EqualTo("none")) { actor.OutputHandler.Send("Specify a gas or 'none'."); return false; }
			candidate = candidate with { AtmosphereGasId = gas?.Id ?? 0 };
		}
		else
		{
			var setting = command.PopForSwitch();
			if (setting is "flash" or "thunder") candidate = setting == "flash" ? candidate with { Flash = command.SafeRemainingArgument } : candidate with { Thunder = command.SafeRemainingArgument };
			else
			{
				var count = setting is "damage" or "targets" ? 3 : 1;
				var values = new double[count];
				for (var i = 0; i < count; i++)
					if (!double.TryParse(command.PopSpeech(), NumberStyles.Float, actor, out values[i]) || !double.IsFinite(values[i]) || values[i] < 0)
					{ actor.OutputHandler.Send("Supply finite, non-negative numbers."); return false; }
				candidate = setting switch
				{
					"chance" => candidate with { LightningChance = values[0] },
					"atmospheric" => candidate with { AtmosphericLightningChance = values[0] },
					"damage" => candidate with { Damage = values[0], Pain = values[1], Stun = values[2] },
					"targets" => candidate with { GroundWeight = values[0], CharacterWeight = values[1], ItemWeight = values[2] },
					"ground" => candidate with { GroundDamageFactor = values[0] },
					"distance" => candidate with { ThunderDistance = values[0] },
					_ => candidate
				};
				if (setting is not ("chance" or "atmospheric" or "damage" or "targets" or "ground" or "distance"))
				{ actor.OutputHandler.Send("Use lightning chance|atmospheric|damage|targets|ground|distance|flash|thunder."); return false; }
			}
		}
		try { candidate = WeatherHazardSettings.FromXml(candidate.ToXml()); }
		catch (FormatException ex) { actor.OutputHandler.Send(ex.Message); return false; }
		using var change = EnvironmentalExposureService.ChangingDefinitions(Gameworld);
		Hazards = candidate;
		Changed = true;
		WeatherForecastInvalidation.Invalidate(Gameworld, this);
		actor.OutputHandler.Send("Weather hazard settings updated.");
		return true;
	}
}
