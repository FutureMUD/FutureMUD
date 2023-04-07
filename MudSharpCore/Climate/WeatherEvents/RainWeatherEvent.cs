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
using MudSharp.Character.Heritage;

namespace MudSharp.Climate.WeatherEvents
{
	internal class RainWeatherEvent : SimpleWeatherEvent
	{
		public RainWeatherEvent(MudSharp.Models.WeatherEvent weather, IFuturemud gameworld) : base(weather, gameworld)
		{
			var definition = XElement.Parse(weather.AdditionalInfo);
			RainLiquid = Gameworld.Liquids.Get(long.Parse(definition.Element("Liquid").Value));
		}

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
