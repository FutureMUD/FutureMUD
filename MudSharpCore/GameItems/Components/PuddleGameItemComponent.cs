using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework.Units;

namespace MudSharp.GameItems.Components
{
	public class PuddleGameItemComponent : GameItemComponent, ILiquidContainer
	{
		protected PuddleGameItemComponentProto _prototype;
		public override IGameItemComponentProto Prototype => _prototype;

		protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
		{
			_prototype = (PuddleGameItemComponentProto)newProto;
		}

		public static ITag PuddleResidueTag(IFuturemud gameworld) => gameworld.Tags.Get(gameworld.GetStaticLong("PuddleResidueTagId"));

		#region Constructors
		public PuddleGameItemComponent(PuddleGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
		{
			_prototype = proto;
		}

		public PuddleGameItemComponent(Models.GameItemComponent component, PuddleGameItemComponentProto proto, IGameItem parent) : base(component, parent)
		{
			_prototype = proto;
			_noSave = true;
			LoadFromXml(XElement.Parse(component.Definition));
			_noSave = false;
		}

		public PuddleGameItemComponent(PuddleGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
		{
			_prototype = rhs._prototype;
		}

		protected void LoadFromXml(XElement root)
		{
			if (root.Element("Mix") != null)
			{
				LiquidMixture = new LiquidMixture(root.Element("Mix"), Gameworld);
			}
		}

		public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
		{
			return new PuddleGameItemComponent(this, newParent, temporary);
		}
		#endregion

		#region Saving
		protected override string SaveToXml()
		{
			return
				new XElement("Definition", LiquidMixture?.SaveToXml() ?? new XElement("NoLiquid"))
					.ToString();
		}
		#endregion

		public override void Login()
		{
			RegisterEvents();
		}

		public override void Quit()
		{
			DeregisterEvents();
		}

		private void RegisterEvents()
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= DoFiveSecondUpdate;
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += DoFiveSecondUpdate;
		}

		public static double EvaporationRatePerSecond(double liquidVolume, double liquidRelativeEnthalpy, double airTemperature, Climate.PrecipitationLevel precipitation, Climate.WindLevel wind)
		{
			var humidity = 1.0;
			switch (precipitation)
			{
				case Climate.PrecipitationLevel.Humid:
					humidity = 0.8;
					break;
				case Climate.PrecipitationLevel.Dry:
					humidity = 0.5;
					break;
				case Climate.PrecipitationLevel.Parched:
					humidity = 0.1;
					break;
			}

			var tempKelvin = airTemperature + 273.0;
			var saturationPressurePascals =
				(tempKelvin > 0.0 ?
				Math.Exp(77.3450 + (0.0057 * tempKelvin) - (7235 / tempKelvin)) / Math.Pow(tempKelvin, 8.2) :
				0.0) * liquidRelativeEnthalpy;

			var maximumHumidityRatio = 0.62198 * saturationPressurePascals / (1013000 - saturationPressurePascals);
			// TODO - 1013000 is hardcoded 1013 barometric pressure. Get actual barometric pressure from atmosphere model

			var airVelocityMetersPerSecond = 0.0;
			switch (wind)
			{
				case Climate.WindLevel.Still:
					airVelocityMetersPerSecond = 0.3;
					break;
				case Climate.WindLevel.OccasionalBreeze:
					airVelocityMetersPerSecond = 1.0;
					break;
				case Climate.WindLevel.Breeze:
					airVelocityMetersPerSecond = 3.33;
					break;
				case Climate.WindLevel.Wind:
					airVelocityMetersPerSecond = 8.0;
					break;
				case Climate.WindLevel.StrongWind:
					airVelocityMetersPerSecond = 16.0;
					break;
				case Climate.WindLevel.GaleWind:
					airVelocityMetersPerSecond = 27.0;
					break;
				case Climate.WindLevel.HurricaneWind:
					airVelocityMetersPerSecond = 50.0;
					break;
				case Climate.WindLevel.MaelstromWind:
					airVelocityMetersPerSecond = 100.0;
					break;
			}

			var evaporationCoefficient = 25 + 19 * airVelocityMetersPerSecond * 1000.0;
			var evaporationPerSecondPerUnitArea = evaporationCoefficient * (maximumHumidityRatio - (maximumHumidityRatio * humidity)) / 3600.0;
			// Assume a fixed surface area / volume ratio of 1m2 / 1000ml
			return Math.Max(evaporationPerSecondPerUnitArea * liquidVolume * 0.005, 0.00001);
		}

		private void DoFiveSecondUpdate()
		{
			var location = Parent.TrueLocations.FirstOrDefault();
			if (location is null)
			{
				return;
			}

			var ambientTemperature = location.CurrentTemperature(Parent);
			var weather = location.CurrentWeather(Parent);
			var totalEvaporation = ChangingNeedsModelBase.StaticRealSecondsToInGameSeconds * 5.0 * EvaporationRatePerSecond(LiquidVolume, LiquidMixture.RelativeEnthalpy, ambientTemperature, weather?.Precipitation ?? Climate.PrecipitationLevel.Parched, weather?.Wind ?? Climate.WindLevel.None);
			if (totalEvaporation <= 0.0)
			{
				return;
			}

			var removed = LiquidMixture.RemoveLiquidVolume(totalEvaporation);
			var ratios = removed.Instances
			                    .Select(x => (Instance: x, x.Amount / removed.TotalVolume))
			                    .ToList();
			var residueTag = PuddleResidueTag(Gameworld);
			foreach (var (instance, amount) in ratios)
			{
				if (instance.Liquid.DriedResidue is null || !instance.Liquid.LeaveResiduesInRooms)
				{
					continue;
				}

				var residueWeight = amount * instance.Liquid.ResidueVolumePercentage;
				if (residueWeight <= 0.0)
				{
					continue;
				}

				var existingResidue = location
					.LayerGameItems(Parent.RoomLayer)
					.SelectNotNull(x => x.GetItemType<CommodityGameItemComponent>())
					.FirstOrDefault(x =>
						x.Material == instance.Liquid.DriedResidue &&
						x.Tag == residueTag && 
						x.Parent.PositionTarget == Parent
					);					;
				if (existingResidue is null)
				{
					var newItem = CommodityGameItemComponentProto.CreateNewCommodity(instance.Liquid.DriedResidue, residueWeight, residueTag, true);
					newItem.RoomLayer = Parent.RoomLayer;
					location.Insert(newItem, true);
					newItem.PositionModifier = Body.Position.PositionModifier.None;
					newItem.PositionTarget = Parent;
					continue;
				}

				existingResidue.Weight += residueWeight;
			}

			Changed = true;
			if (LiquidMixture.IsEmpty)
			{
				Parent.Delete();
			}
		}

		private void DeregisterEvents()
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= DoFiveSecondUpdate;
		}

		private LiquidMixture _liquidMixture;
		public LiquidMixture LiquidMixture
		{
			get => _liquidMixture;
			set
			{
				_liquidMixture = value;
				Changed = true;
			}
		}

		private void AdjustLiquidQuantity(double amount, ICharacter who, string action)
		{
			if (LiquidMixture == null)
			{
				return;
			}

			LiquidMixture.AddLiquidVolume(amount);
			if (LiquidMixture.IsEmpty)
			{
				LiquidMixture = null;
				Parent.Delete();
			}
			
			Changed = true;
		}

		public void AddLiquidQuantity(double amount, ICharacter who, string action)
		{
			if (LiquidMixture == null)
			{
				return;
			}
			if (LiquidMixture.TotalVolume + amount > LiquidCapacity)
			{
				amount = LiquidCapacity - LiquidMixture.TotalVolume;
			}

			if (LiquidMixture.TotalVolume + amount < 0)
			{
				amount = -1 * LiquidMixture.TotalVolume;
			}

			AdjustLiquidQuantity(amount, who, action);
		}

		public void ReduceLiquidQuantity(double amount, ICharacter who, string action)
		{
			if (LiquidMixture == null)
			{
				return;
			}

			if (LiquidMixture.TotalVolume - amount < 0)
			{
				amount = LiquidMixture.TotalVolume;
			}

			AdjustLiquidQuantity(amount * -1, who, action);
		}

		public void MergeLiquid(LiquidMixture otherMixture, ICharacter who, string action)
		{
			if (LiquidMixture == null)
			{
				LiquidMixture = otherMixture;
			}
			else
			{
				LiquidMixture.AddLiquid(otherMixture);
			}

			if (LiquidMixture.IsEmpty)
			{
				LiquidMixture = null;
				Parent.Delete();
			}
			Changed = true;
		}

		public LiquidMixture RemoveLiquidAmount(double amount, ICharacter who, string action)
		{
			if (LiquidMixture == null)
			{
				return null;
			}

			var newMixture = LiquidMixture.RemoveLiquidVolume(amount);
			Changed = true;
			if (LiquidMixture.IsEmpty)
			{
				LiquidMixture = null;
				Parent.Delete();
			}
			return newMixture;
		}

		public bool CanBeEmptiedWhenInRoom => false;

		public double LiquidVolume => LiquidMixture?.TotalVolume ?? 0.0;

		public double LiquidCapacity => double.MaxValue;
		public override int DecorationPriority => -1;

		public string PuddleDescription(double amount)
		{
			if (Gameworld.GetStaticDouble("EnormousPoolLiquidQuantity") <= amount)
			{
				return "enormous pool";
			}


			if (Gameworld.GetStaticDouble("HugePoolLiquidQuantity") <= amount)
			{
				return "huge pool";
			}


			if (Gameworld.GetStaticDouble("LargePoolLiquidQuantity") <= amount)
			{
				return "large pool";
			}


			if (Gameworld.GetStaticDouble("PoolLiquidQuantity") <= amount)
			{
				return "pool";
			}


			if (Gameworld.GetStaticDouble("LargePuddleLiquidQuantity") <= amount)
			{
				return "large puddle";
			}


			if (Gameworld.GetStaticDouble("PuddleLiquidQuantity") <= amount)
			{
				return "puddle";
			}


			if (Gameworld.GetStaticDouble("SmallPuddleLiquidQuantity") <= amount)
			{
				return "small puddle";
			}

			if (Gameworld.GetStaticDouble("SplashLiquidQuantity") <= amount)
			{
				return "splash";
			}
			
			return "spot";
		}

		public override bool DescriptionDecorator(DescriptionType type)
		{
			return (type == DescriptionType.Full) || (type == DescriptionType.Short);
		}

		public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
			bool colour, PerceiveIgnoreFlags flags)
		{
			switch (type)
			{
				case DescriptionType.Full:
					if (LiquidMixture?.IsEmpty == false)
					{
						if (voyeur is ICharacter ch && ch.IsAdministrator())
						{
							var location = Parent.TrueLocations.FirstOrDefault();
							if (location is null)
							{
								return "It is a puddle in the void.";
							}

							var ambientTemperature = location.CurrentTemperature(Parent);
							var weather = location.CurrentWeather(Parent);
							var totalEvaporation = EvaporationRatePerSecond(LiquidVolume, LiquidMixture.RelativeEnthalpy, ambientTemperature, weather?.Precipitation ?? Climate.PrecipitationLevel.Parched, weather?.Wind ?? Climate.WindLevel.None);
							return $@"It is a puddle of liquid spilled on the ground.

It is best described as {PuddleDescription(LiquidMixture.TotalVolume).A_An()} containing {Gameworld.UnitManager.DescribeExact(LiquidMixture.TotalVolume, Framework.Units.UnitType.FluidVolume, ch).ColourValue()} of {LiquidMixture.ColouredLiquidLongDescription}.

It evaporates at a rate of {Gameworld.UnitManager.DescribeDecimal(totalEvaporation, UnitType.FluidVolume, ch).ColourValue()} per second.";
						}

						return @$"It is a puddle of liquid spilled on the ground.

It is best described as {PuddleDescription(LiquidMixture.TotalVolume).A_An()} containing approximately {Gameworld.UnitManager.DescribeMostSignificant(LiquidMixture.TotalVolume, Framework.Units.UnitType.FluidVolume, voyeur).ColourValue()} of {LiquidMixture.ColouredLiquidDescription}.";
					}
					return "It is an empty puddle, whatever that may be.";
				case DescriptionType.Short:
					if (LiquidMixture?.IsEmpty == false && !flags.HasFlag(PerceiveIgnoreFlags.IgnoreLiquidsAndFlags))
					{
						return $"{PuddleDescription(LiquidMixture.TotalVolume).A_An()} of {LiquidMixture.LiquidDescription}".Colour(LiquidMixture.LiquidColour);
					}
					return description;
			
			}
			return description;
		}

		public override bool PreventsMerging(IGameItemComponent component)
		{
			return true;
		}

		public override double ComponentWeight
		{
			get
			{
				return (LiquidMixture?.TotalWeight ?? 0.0);
			}
		}
		public override double ComponentBuoyancy(double fluidDensity)
		{
			return LiquidMixture?.Instances.Sum(x => (fluidDensity - x.Liquid.Density) * x.Amount * x.Liquid.Density) ?? 0.0;
		}

		public override bool Die(IGameItem newItem, ICell location)
		{
			if (LiquidMixture == null)
			{
				return false;
			}

			var newItemLiquid = newItem?.GetItemType<ILiquidContainer>();
			if (newItemLiquid != null)
			{
				newItemLiquid.LiquidMixture = LiquidMixture;
				newItemLiquid.Changed = true;
			}

			return false;
		}

		public override bool ExposeToLiquid(LiquidMixture mixture)
		{
			if (LiquidMixture.CanMerge(mixture))
			{
				MergeLiquid(mixture, null, "");
				return true;
			}
			return true;
		}

		#region IsOpenable

		public bool IsOpen => true;

		public bool CanOpen(IBody opener)
		{
			return false;
		}

		public WhyCannotOpenReason WhyCannotOpen(IBody opener)
		{
			return WhyCannotOpenReason.NotOpenable;
		}

		public void Open()
		{
		}

		public bool CanClose(IBody closer)
		{
			return false;
		}

		public WhyCannotCloseReason WhyCannotClose(IBody closer)
		{
			return WhyCannotCloseReason.NotOpenable;
		}

		public void Close()
		{
		}

		public event OpenableEvent OnOpen;
		public event OpenableEvent OnClose;
		#endregion

	}
}
