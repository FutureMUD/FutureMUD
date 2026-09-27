#nullable enable

namespace MudSharp.Form.Material;

public partial class Solid
{
	public MaterialExposureProperties ExposureProperties { get; private set; } = new();
	private bool BuildingCommandThermalResponse(ICharacter actor, StringStack command)
	{
		var field = command.PopForSwitch();
		var text = command.SafeRemainingArgument;
		if (field == "intensity")
		{
			var prog = text.EqualTo("none") ? null : Gameworld.FutureProgs.GetByIdOrName(text);
			if (prog is null && !text.EqualTo("none") || prog is not null && !ExposureProgContract.Valid(prog, "intensity"))
			{
				actor.OutputHandler.Send(ExposureProgContract.Description);
				return false;
			}
			ExposureProperties.ThermalIntensityProgId = prog?.Id;
			Changed = true;
			actor.OutputHandler.Send($"Ambient thermal intensity prog is now {(prog?.Name ?? "none").ColourValue()}.");
			return true;
		}
		if (field is not ("slope" or "cap") ||
			!text.EqualTo("default") && (!double.TryParse(text, actor, out var parsed) || !ExposureArithmetic.Valid(parsed) || parsed > 1000000))
		{
			actor.OutputHandler.Send("Use thermalresponse slope|cap <0-1000000>|default, or thermalresponse intensity <prog>|none.".ColourCommand());
			return false;
		}
		double? value = text.EqualTo("default") ? null : double.Parse(text, actor);
		if (field == "slope") ExposureProperties.ThermalSlope = value; else ExposureProperties.ThermalCap = value;
		Changed = true;
		actor.OutputHandler.Send($"Thermal {field} is now {(value?.ToString("N3", actor) ?? "the world default").ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTransmission(ICharacter actor, StringStack command)
	{
		var route = command.PopForSwitch();
		if (!double.TryParse(command.SafeRemainingArgument, actor, out var fraction) || !double.IsFinite(fraction) || fraction < 0 || fraction > 1)
		{ actor.OutputHandler.Send("Supply liquid, gas, thermal or soak and a fraction between 0 and 1."); return false; }
		EnvironmentalExposureService.For(Gameworld).Settle(actor);
		switch (route)
		{
			case "liquid": ExposureProperties.LiquidTransmission = fraction; break;
			case "gas": ExposureProperties.GasTransmission = fraction; break;
			case "thermal": ExposureProperties.ThermalTransmission = fraction; break;
			case "soak": ExposureProperties.SoakPerSecond = fraction; break;
			default: actor.OutputHandler.Send("Choose liquid, gas, thermal or soak."); return false;
		}
		Changed = true; EnvironmentalExposureService.For(Gameworld).Refresh();
		actor.OutputHandler.Send($"{route.ColourName()} transmission is now {fraction.ToString("P2", actor).ColourValue()}."); return true;
	}
}
