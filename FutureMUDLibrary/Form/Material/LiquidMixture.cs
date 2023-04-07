using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.Body.Needs;

namespace MudSharp.Form.Material
{
    public delegate void LiquidMixtureEvent(LiquidMixture mixture);

    public enum LiquidExposureDirection
    {
        FromInside,
        FromUnderneath,
        FromOnTop,
        Irrelevant,
        FromContainer
    }

    public class LiquidMixture
    {
        public XElement SaveToXml()
        {
            return new XElement("Mix", 
                    from instance in _instances
                    select instance.SaveToXml()
                );
        }

        public IFuturemud Gameworld { get; }

        public LiquidMixture (IEnumerable<LiquidInstance> instances, IFuturemud gameworld)
        {
            Gameworld = gameworld;
            _instances = instances.ToList();
            ContentsUpdated();
        }

        public LiquidMixture(LiquidInstance instance, IFuturemud gameworld)
        {
            Gameworld = gameworld;
            _instances = new List<LiquidInstance> { instance };
            ContentsUpdated();
        }

        public LiquidMixture (ILiquid liquid, double amount, IFuturemud gameworld)
        {
            Gameworld = gameworld;
            _instances = new List<LiquidInstance>
            {
                new()
                {
                    Liquid = liquid,
                    Amount = amount,
                }
            };
            ContentsUpdated();
        }

        public LiquidMixture(XElement root, IFuturemud gameworld)
        {
            Gameworld = gameworld;
            _instances = new List<LiquidInstance>(root.Elements("Liquid").Count());
            foreach (var item in root.Elements("Liquid"))
            {
                _instances.Add(LiquidInstance.LoadInstance(item, gameworld));
            }
            ContentsUpdated();
        }

        public LiquidMixture(LiquidMixture rhs)
        {
            Gameworld = rhs.Gameworld;
            _instances = rhs.Instances.Select(x => x.Copy()).ToList();
            ContentsUpdated();
        }

        public LiquidMixture(LiquidMixture rhs, double newVolume)
        {
            Gameworld = rhs.Gameworld;
            _instances = rhs.Instances.Select(x => x.Copy()).ToList();
            ContentsUpdated();
            SetLiquidVolume(newVolume);
        }

        public LiquidMixture Clone()
        {
	        return new LiquidMixture(this);
        }

        public LiquidMixture Clone(double newVolume)
        {
            return new LiquidMixture(this, newVolume);
        }

        public IEnumerable<LiquidMixture> Split(int number)
        {
	        if (number <= 1)
	        {
		        return new[] { Clone() };
	        }

	        var volumeEach = TotalVolume / number;
            var list = new List<LiquidMixture>();
            for (var i = 0; i < number; i++)
            {
	            var mixture = Clone();
	            mixture.SetLiquidVolume(volumeEach);
	            list.Add(mixture);
            }

            return list;
        }

        public static LiquidMixture CreateEmpty(IFuturemud gameworld) => new(new LiquidInstance[] { }, gameworld);

        private readonly List<LiquidInstance> _instances;

        public IEnumerable<LiquidInstance> Instances => _instances;

        public (bool Truth, ItemQuality Quality) CountsAs(ILiquid liquid)
        {
            if (TotalVolume <= 0)
            {
                return (false, ItemQuality.Terrible);
            }

            var ratio = _instances.Where(x => x.Liquid.LiquidCountsAs(liquid)).Sum(x => x.Amount) / TotalVolume;
            if (ratio >= 0.95)
            {
                return (true, _instances.Where(x => x.Liquid.LiquidCountsAs(liquid)).Select(x => (x.Liquid.LiquidCountsAsQuality(liquid), x.Amount)).GetNetQuality());
            }

            return (false, ItemQuality.Terrible);
        }

        public bool IsEmpty => !_instances.Any() || TotalVolume <= 0.0;

        public double TotalVolume { get; private set; }

        public double TotalWeight { get; private set; }

        public double RelativeEnthalpy => 
            !_instances.Any() ?
            1.0
            : _instances.Sum(x => x.Liquid.RelativeEnthalpy * x.Amount) / TotalVolume;

        public void ContentsUpdated() {
            _instances.RemoveAll(x => x.Amount <= 0.0);
            TotalVolume = _instances.Sum(x => x.Amount);
            TotalWeight = _instances.Sum(x => x.Amount * x.Liquid.Density) * Gameworld.UnitManager.BaseFluidToLitres / Gameworld.UnitManager.BaseWeightToKilograms;
            
            OnLiquidMixtureChanged?.Invoke(this);
        }

        public bool CanMerge(LiquidMixture otherMixture)
        {
            // TODO - add in unmergable liquids
            return true;
        }

        public bool CanMerge(LiquidInstance otherInstance)
        {
            // TODO - add in unmergable liquids
            return true;
        }

        public bool CanMerge(ILiquid otherLiquid)
        {
            // TODO - add in unmergable liquids
            return true;
        }

        public void AddLiquid(LiquidInstance instance)
        {
            var compatibleInstance = _instances.FirstOrDefault(x => x.CanMergeWith(instance));
            if (compatibleInstance != null)
            {
                compatibleInstance.MergeOtherIntoSelf(instance);
                ContentsUpdated();
                return;
            }

            _instances.Add(instance);
            ContentsUpdated();
        }

        public void AddLiquid(LiquidMixture mixture)
        {
            foreach (var liquid in mixture._instances)
            {
                AddLiquid(liquid);
            }
        }

        public void RemoveLiquid(LiquidMixture otherLiquid)
        {
            foreach (var instance in otherLiquid.Instances)
            {
                LiquidInstance matching;
                switch (instance)
                {
                    case BloodLiquidInstance bi:
                        matching =
                            Instances.OfType<BloodLiquidInstance>().First(x =>
                                x.Liquid == instance.Liquid &&
                                (bi.Race == null || bi.Race == x.Race) &&
                                (bi.BloodType == null || bi.BloodType == x.BloodType) &&
                                (bi.SourceId == 0 || bi.SourceId == x.SourceId) &&
                                x.Amount >= instance.Amount
                            );
                        break;
                    case ColourLiquidInstance co:
                        matching = Instances.OfType<ColourLiquidInstance>()
                            .First(x =>
                                x.Liquid == instance.Liquid &&
                                x.Colour == co.Colour &&
                                x.Amount >= instance.Amount);

                        break;
                    default:
                        matching = Instances.First(x => x.Liquid == instance.Liquid && x.Amount >= instance.Amount);
                        break;
                }

                RemoveLiquidVolume(matching, instance.Amount);
            }
        }

        public void RemoveLiquidVolume(LiquidInstance instance, double volume)
        {
            if (!_instances.Contains(instance)){
                return;
            }

            instance.Amount -= volume;
            ContentsUpdated();
        }

        public bool CanRemoveLiquid(LiquidMixture otherLiquid)
        {
            foreach (var instance in otherLiquid.Instances)
            {
                switch (instance)
                {
                    case BloodLiquidInstance bi:
                        if (!Instances.OfType<BloodLiquidInstance>().Any(x =>
                            x.Liquid == instance.Liquid &&
                            (bi.Race == null || bi.Race == x.Race) &&
                            (bi.BloodType == null || bi.BloodType == x.BloodType) &&
                            (bi.SourceId == 0 || bi.SourceId == x.SourceId) &&
                            x.Amount >= instance.Amount
                        ))
                        {
                            return false;
                        }
                        break;
                    case ColourLiquidInstance co:
                        if (!Instances.OfType<ColourLiquidInstance>()
                            .Any(x =>
                                x.Liquid == instance.Liquid &&
                                x.Colour == co.Colour &&
                                x.Amount >= instance.Amount))
                        {
                            return false;
                        }

                        break;
                    default:
                        if (!Instances.Any(x => x.Liquid == instance.Liquid && x.Amount >= instance.Amount))
                        {
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        public LiquidMixture RemoveLiquidVolume(double volume)
        {
            if (TotalVolume <= 0) {
                return this;
            }
            var ratio = volume / TotalVolume;
            if (ratio < 0.0)
            {
                ratio = 0.0;
            }

            var newInstances = new List<LiquidInstance>();

            foreach (var liquid in _instances)
            {
                newInstances.Add(liquid.SplitVolume(liquid.Amount * ratio));
            }

            _instances.RemoveAll(x => x.Amount <= 0.0);
            ContentsUpdated();
            if (newInstances.Sum(x => x.Amount) <= 0.0) {
                return null;
            }
            return new LiquidMixture(newInstances, Gameworld);
        }

        public void SetLiquidVolume(double volume)
        {
            if (TotalVolume <= 0)
            {
                foreach (var liquid in _instances)
                {
                    liquid.Amount = volume / _instances.Count;
                }
                ContentsUpdated();
                return;
            }

            var ratio = volume / TotalVolume;
            foreach (var liquid in _instances)
            {
                liquid.Amount = liquid.Amount * ratio;
            }
            ContentsUpdated();
        }

        public void AddLiquidVolume(double volume)
        {
            if (volume < 0.0)
            {
                RemoveLiquidVolume(-1 * volume);
                return;
            }
            var ratio = 1.0 + TotalVolume > 0 ? volume / TotalVolume : 0;
            foreach (var liquid in _instances)
            {
                liquid.Amount = liquid.Amount * ratio;
            }
            ContentsUpdated();
        }

        public void RemoveLiquidInstance(LiquidInstance instance) {
            _instances.Remove(instance);
            ContentsUpdated();
        }

        public string LiquidDescription {
            get {
                // TODO - special override descriptions
                if (_instances.Count == 1)
                {
                    return _instances[0].LiquidDescription;
                }

                return "a mixture of " + _instances
                    .Where(x => (x.Amount / (TotalVolume != 0.0 ? TotalVolume : 1.0)) >= 0.05)
                    .DefaultIfEmpty(_instances.First())
					.Select(x => x.LiquidDescription)
                    .Distinct()
                    .ListToString();
            }
        }

        public ANSIColour LiquidColour
        {
	        get
	        {
		        if (_instances.Count == 1)
		        {
			        return _instances[0].Liquid.DisplayColour;
		        }

		        return _instances
		               .Where(x => (x.Amount / (TotalVolume != 0.0 ? TotalVolume : 1.0)) >= 0.05)
		               .DefaultIfEmpty(_instances.First())
		               .OrderByDescending(x => x.Amount)
		               .First()
		               .Liquid.DisplayColour;
	        }
        }

        public string ColouredLiquidDescription {
            get {
                if (_instances.Count == 1)
                {
                    return _instances[0].LiquidDescription.Colour(_instances[0].Liquid.DisplayColour);
                }

                return "a mixture of " + _instances
                    .Where(x => (x.Amount / (TotalVolume != 0.0 ? TotalVolume : 1.0)) >= 0.05)
                    .DefaultIfEmpty(_instances.First())
                    .Select(x => x.LiquidDescription.Colour(x.Liquid.DisplayColour))
                    .Distinct()
                    .ListToString();
            }
        }

        public string ColouredLiquidLongDescription
        {
	        get
	        {
		        if (_instances.Count == 1)
		        {
			        return _instances[0].LiquidLongDescription.Colour(_instances[0].Liquid.DisplayColour);
		        }

		        return "a mixture of " + _instances
			        .Where(x => (x.Amount / (TotalVolume != 0.0 ? TotalVolume : 1.0)) >= 0.05)
			        .DefaultIfEmpty(_instances.First())
					.Select(x => x.LiquidLongDescription.Colour(x.Liquid.DisplayColour))
			        .Distinct()
			        .ListToString();
	        }
        }

        public INeedFulfiller GetNeedFulfiller()
        {
            if (TotalVolume <= 0)
            {
                return new NeedFulfiller();
            }

            var multiplier = Gameworld.UnitManager.BaseFluidToLitres;
            double satiationPoints = 0.0, calories = 0.0, thirstPoints = 0.0, waterLitres = 0.0, alcoholLitres = 0.0;
            foreach (var liquid in Instances)
            {
                satiationPoints += liquid.Liquid.FoodSatiatedHoursPerLitre * liquid.Amount * multiplier;
                calories += liquid.Liquid.CaloriesPerLitre * liquid.Amount * multiplier;
                thirstPoints += liquid.Liquid.DrinkSatiatedHoursPerLitre * liquid.Amount * multiplier;
                waterLitres += liquid.Liquid.WaterLitresPerLitre * liquid.Amount * multiplier;
                alcoholLitres += liquid.Liquid.AlcoholLitresPerLitre * liquid.Amount * multiplier;
            }

            return new NeedFulfiller
            {
                SatiationPoints = satiationPoints,
                Calories = calories,
                ThirstPoints = thirstPoints,
                WaterLitres = waterLitres,
                AlcoholLitres = alcoholLitres
            };
        }

        public void OnDraught(ICharacter actor, ILiquidContainer container)
        {
            foreach (var instance in _instances)
            {
                instance.Liquid.DraughtProg?.Execute(actor, container, instance.Amount);
            }
        }

        public string TasteString(ICharacter actor)
        {
            // TODO - checks and more complexity
            var tasteDictionary = new Dictionary<(string Taste, string Vague), double>();
            foreach (var instance in _instances)
            {
                var tt = (instance.Liquid.TasteText, instance.Liquid.VagueTasteText);
                if (tasteDictionary.ContainsKey(tt))
                {
                    tasteDictionary[tt] += instance.Amount * instance.Liquid.TasteIntensity;
                }
                else
                {
                    tasteDictionary[tt] = instance.Amount * instance.Liquid.TasteIntensity;
                }
            }

            var sum = _instances.Sum(x => x.Amount * x.Liquid.TasteIntensity);
            var sb = new StringBuilder();
            foreach (var taste in tasteDictionary.OrderByDescending(x => x.Value))
            {
                if (taste.Value / sum >= 0.2)
                {
                    sb.AppendLine(taste.Key.Taste.Fullstop());
                }
                else if (taste.Value / sum >= 0.05)
                {
                    sb.AppendLine(taste.Key.Vague.Fullstop());
                }
            }

            return sb.ToString();
        }

        public event LiquidMixtureEvent OnLiquidMixtureChanged;
    }
}

