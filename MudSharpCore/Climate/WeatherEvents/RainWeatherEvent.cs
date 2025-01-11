using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Framework.Units;

namespace MudSharp.Climate.WeatherEvents
{
	internal class RainWeatherEvent : SimpleWeatherEvent
	{
		public RainWeatherEvent(MudSharp.Models.WeatherEvent weather, IFuturemud gameworld) : base(weather, gameworld)
		{
			var definition = XElement.Parse(weather.AdditionalInfo);
			RainLiquid = Gameworld.Liquids.Get(long.Parse(definition.Element("Liquid").Value));
		}

		#region Overrides of SimpleWeatherEvent

		/// <inheritdoc />
		protected override XElement SaveToXml()
		{
			var parent = base.SaveToXml();
			parent.Add(new XElement("Liquid", RainLiquid?.Id ?? 0));
			return parent;
		}

		/// <inheritdoc />
		public override string Show(ICharacter actor)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Rain Weather Event #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
			sb.AppendLine();
			sb.AppendLine($"Counts As: {CountsAs?.Name.ColourValue() ?? "None".ColourError()}");
			sb.AppendLine($"Rain Liquid: {RainLiquid?.Name.Colour(RainLiquid.DisplayColour) ?? "None".ColourError()}");
			sb.AppendLine($"Description: {WeatherDescription.ColourCommand()}");
			sb.AppendLine($"Room Addendum: {WeatherRoomAddendum?.SubstituteANSIColour()}");
			sb.AppendLine($"Precipitation: {Precipitation.DescribeEnum().ColourValue()}");
			sb.AppendLine($"Wind: {Precipitation.DescribeEnum().ColourValue()}");
			sb.AppendLine($"Temperature Effect: {Gameworld.UnitManager.DescribeExact(TemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}");
			sb.AppendLine($"Wind Temperature Effect: {Gameworld.UnitManager.DescribeExact(WindTemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}");
			sb.AppendLine($"Precipitation Temperature Effect: {Gameworld.UnitManager.DescribeExact(PrecipitationTemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}");
			sb.AppendLine($"Obscure Sky: {ObscuresViewOfSky.ToColouredString()}");
			sb.AppendLine($"Light Level Multiplier: {LightLevelMultiplier.ToStringP2Colour(actor)}");
			sb.AppendLine($"Permitted Times: {PermittedTimesOfDay.ListToColouredString()}");
			sb.AppendLine($"Default Transition Echo: {_defaultTransitionEcho?.SubstituteANSIColour()}");
			sb.AppendLine();
			sb.AppendLine("Special Transition Echoes:");
			sb.AppendLine();
			if (_transitionEchoOverrides.Count > 0)
			{
				foreach (var item in _transitionEchoOverrides)
				{
					var we = Gameworld.WeatherEvents.Get(item.Key);
					if (we is null)
					{
						continue;
					}
					sb.AppendLine($"\t{we.Name.ColourName()} (#{we.Id.ToStringN0(actor)}): {item.Value.SubstituteANSIColour()}");
				}
			}
			else
			{
				sb.AppendLine("\tNone");
			}
			sb.AppendLine();
			sb.AppendLine("Random Flavour Echoes:");
			sb.AppendLine();
			var total = _randomEchoes.Sum(x => x.Chance);
			if (total > 0.0)
			{
				foreach (var echo in _randomEchoes)
				{
					sb.AppendLine($"\t{echo.Chance.ToStringN2Colour(actor)} ({(echo.Chance / total).ToStringP2Colour(actor)}): {echo.Echo.SubstituteANSIColour()}");
				}
			}
			return sb.ToString();
		}

		#endregion

		public ILiquid RainLiquid { get; protected set; }
		
		public override void OnFiveSecondEvent(ICell cell)
		{
			if (cell.OutdoorsType(null) != CellOutdoorsType.Outdoors)
			{
				return;
			}

			foreach (var item in cell.GameItems.ToArray())
			{
				if (item.PositionModifier == Body.Position.PositionModifier.Under && item.PositionTarget is not null && item.PositionTarget.Size > item.Size)
				{
					continue;
				}

				if (item.RoomLayer.IsUnderwater())
				{
					continue;
				}

				item.ExposeToPrecipitation(Precipitation, RainLiquid);
			}

			foreach (var ch in cell.Characters.ToArray())
			{
				if (ch.PositionModifier == Body.Position.PositionModifier.Under && ch.PositionTarget is not null && ch.PositionTarget.Size > ch.CurrentContextualSize(SizeContext.RainfallExposure))
				{
					continue;
				}

				if (ch.RoomLayer.IsUnderwater())
				{
					continue;
				}

				ch.Body.ExposeToPrecipitation(Precipitation, RainLiquid);
			}
		}
	}
}
