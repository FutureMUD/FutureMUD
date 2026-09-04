using MudSharp.Framework;
using System;
using System.Globalization;
using System.Xml.Linq;

namespace MudSharp.Form.Material
{
    public class LiquidInstance
    {
        public static LiquidInstance LoadInstance(XElement root, IFuturemud gameworld)
        {
            switch (root.Attribute("instancetype")?.Value ?? "none")
            {
                case "blood":
                    return new BloodLiquidInstance(root, gameworld);
                case "colour":
                    return new ColourLiquidInstance(root, gameworld);
            }

            return new LiquidInstance(root, gameworld);
        }

        public virtual string LiquidDescription => Liquid.MaterialDescription;
        public virtual string LiquidLongDescription => Liquid.Description;

        private ILiquid _originLiquid;

        /// <summary>The stable liquid lineage used for freshness and merging.</summary>
        public ILiquid OriginLiquid => _originLiquid;

        /// <summary>The effective liquid presented and consumed at the current irreversible freshness stage.</summary>
        public ILiquid Liquid
        {
            get
            {
                var configuration = _originLiquid?.FreshnessConfiguration;
                return FreshnessStage switch
                {
                    LiquidFreshnessStage.Stale when configuration?.StaleLiquid is not null => configuration.StaleLiquid,
                    LiquidFreshnessStage.Spoiled when configuration?.SpoiledLiquid is not null => configuration.SpoiledLiquid,
                    _ => _originLiquid
                };
            }
            init => _originLiquid = value;
        }

        public double Amount { get; set; }
        public TimeSpan EffectiveAge { get; private set; }
        public DateTime LastFreshnessResolution { get; private set; } = DateTime.UtcNow;
        public LiquidFreshnessStage FreshnessStage { get; private set; }

        public bool ResolveFreshness(DateTime utcNow, double rateMultiplier)
        {
            var configuration = OriginLiquid?.FreshnessConfiguration;
            if (configuration is null || utcNow <= LastFreshnessResolution)
            {
                return false;
            }

            EffectiveAge += TimeSpan.FromTicks((long)((utcNow - LastFreshnessResolution).Ticks * Math.Max(0.0, rateMultiplier)));
            LastFreshnessResolution = utcNow;
            var impliedStage = EffectiveAge >= configuration.SpoilAfter
                ? LiquidFreshnessStage.Spoiled
                : EffectiveAge >= configuration.StaleAfter
                    ? LiquidFreshnessStage.Stale
                    : LiquidFreshnessStage.Fresh;
            if (impliedStage > FreshnessStage)
            {
                FreshnessStage = impliedStage;
            }

			// Advancing the resolution timestamp is persisted state even when a zero multiplier
			// pauses age; otherwise a save/reload could count the paused interval later.
            return true;
        }

        public virtual bool CanMergeWith(LiquidInstance other)
        {
            return OriginLiquid == other.OriginLiquid;
        }

        public virtual void MergeOtherIntoSelf(LiquidInstance other)
        {
            var total = Amount + other.Amount;
            if (total <= 0.0)
            {
                Amount = total;
                return;
            }

            EffectiveAge = TimeSpan.FromTicks((long)((EffectiveAge.Ticks * Amount + other.EffectiveAge.Ticks * other.Amount) / total));
            FreshnessStage = (LiquidFreshnessStage)Math.Max((int)FreshnessStage, (int)other.FreshnessStage);
            LastFreshnessResolution = LastFreshnessResolution > other.LastFreshnessResolution
                ? LastFreshnessResolution
                : other.LastFreshnessResolution;
            Amount = total;
        }

        public virtual LiquidInstance SplitVolume(double volume)
        {
            var result = Copy();
            Amount -= volume;
            result.Amount = volume;
            return result;
        }

        public virtual XElement SaveToXml()
        {
            var root = new XElement("Liquid",
                new XAttribute("id", OriginLiquid.Id),
                new XAttribute("amount", Amount.ToString("R", CultureInfo.InvariantCulture)));
            if (OriginLiquid.FreshnessConfiguration is not null || EffectiveAge > TimeSpan.Zero || FreshnessStage != LiquidFreshnessStage.Fresh)
            {
                root.Add(new XAttribute("freshnessAgeSeconds", EffectiveAge.TotalSeconds.ToString("R", CultureInfo.InvariantCulture)));
                root.Add(new XAttribute("freshnessResolvedUtc", LastFreshnessResolution.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
                root.Add(new XAttribute("freshnessStage", FreshnessStage.ToString()));
            }

            return root;
        }

        public LiquidInstance() { }

        public LiquidInstance(XElement root, IFuturemud gameworld)
        {
            _originLiquid = gameworld.Liquids.Get(long.Parse(root.Attribute("id").Value));
            Amount = double.Parse(root.Attribute("amount").Value, CultureInfo.InvariantCulture);
            EffectiveAge = TimeSpan.FromSeconds(double.Parse(root.Attribute("freshnessAgeSeconds")?.Value ?? "0", CultureInfo.InvariantCulture));
            LastFreshnessResolution = DateTime.TryParse(root.Attribute("freshnessResolvedUtc")?.Value,
                CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var resolved)
                ? resolved.ToUniversalTime()
                : DateTime.UtcNow;
            FreshnessStage = Enum.TryParse<LiquidFreshnessStage>(root.Attribute("freshnessStage")?.Value, true, out var stage)
                ? stage
                : LiquidFreshnessStage.Fresh;
        }

        public LiquidInstance(LiquidInstance rhs)
        {
            _originLiquid = rhs.OriginLiquid;
            Amount = rhs.Amount;
            EffectiveAge = rhs.EffectiveAge;
            LastFreshnessResolution = rhs.LastFreshnessResolution;
            FreshnessStage = rhs.FreshnessStage;
        }

        public virtual LiquidInstance Copy()
        {
            return new LiquidInstance(this);
        }
    }
}
