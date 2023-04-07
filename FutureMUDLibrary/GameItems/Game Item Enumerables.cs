using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.GameItems
{
    public enum ItemQuality {
        Terrible = 0,
        ExtremelyBad = 1,
        Bad = 2,
        Poor = 3,
        Substandard = 4,
        Standard = 5,
        Good = 6,
        VeryGood = 7,
        Great = 8,
        Excellent = 9,
        Heroic = 10,
        Legendary = 11
    }

    public enum ItemSaturationLevel
    {
        Dry,
        Damp,
        Wet,
        Soaked,
        Saturated
    }

    public static class GameItemEnumExtensions {
        public static string Describe(this ItemQuality quality) {
            switch (quality) {
                case ItemQuality.Terrible:
                    return "Terrible";
                case ItemQuality.ExtremelyBad:
                    return "Extremely Bad";
                case ItemQuality.Bad:
                    return "Bad";
                case ItemQuality.Poor:
                    return "Poor";
                case ItemQuality.Substandard:
                    return "Substandard";
                case ItemQuality.Standard:
                    return "Standard";
                case ItemQuality.Good:
                    return "Good";
                case ItemQuality.VeryGood:
                    return "Very Good";
                case ItemQuality.Great:
                    return "Great";
                case ItemQuality.Excellent:
                    return "Excellent";
                case ItemQuality.Heroic:
                    return "Heroic";
                case ItemQuality.Legendary:
                    return "Legendary";
                default:
                    return "Unknown";
            }
        }

        public static bool TryParseQuality(string text, out ItemQuality quality) {
            if (Enum.TryParse(text, true, out quality)) {
                return true;
            }

            var values = Enum.GetValues(typeof(ItemQuality)).OfType<ItemQuality>().ToList();
            if (values.Any(x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase))) {
                quality =
                    values.FirstOrDefault(
                        x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
                return true;
            }

            return false;
        }

        public static bool TryParseSize(string text, out SizeCategory size)
        {
            if (Enum.TryParse(text, true, out size))
            {
                return true;
            }

            var values = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>().ToList();
            if (values.Any(x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase)))
            {
                size =
                    values.FirstOrDefault(
                        x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
                return true;
            }

            return false;
        }

        public static ItemQuality StageUp(this ItemQuality quality, int stages) {
            var newQuality = (int) quality + stages;
            if (newQuality > (int) ItemQuality.Legendary) {
                return ItemQuality.Legendary;
            }
            if (newQuality < 0) {
                return ItemQuality.Terrible;
            }

            return (ItemQuality) newQuality;
        }

        public static SizeCategory ChangeSize(this SizeCategory size, int steps) {
            var newSize = (int) size + steps;
            if (newSize > (int) SizeCategory.Titanic) {
                return SizeCategory.Titanic;
            }
            if (newSize < 0) {
                return SizeCategory.Nanoscopic;
            }

            return (SizeCategory) newSize;
        }

        public static string Describe(this SizeCategory category) {
            switch (category) {
                case SizeCategory.Enormous:
                    return "Enormous";
                case SizeCategory.Gigantic:
                    return "Gigantic";
                case SizeCategory.Huge:
                    return "Huge";
                case SizeCategory.Large:
                    return "Large";
                case SizeCategory.Microscopic:
                    return "Microscopic";
                case SizeCategory.Miniscule:
                    return "Miniscule";
                case SizeCategory.Nanoscopic:
                    return "Nanoscopic";
                case SizeCategory.Normal:
                    return "Normal";
                case SizeCategory.Small:
                    return "Small";
                case SizeCategory.Tiny:
                    return "Tiny";
                case SizeCategory.Titanic:
                    return "Titanic";
                case SizeCategory.VeryLarge:
                    return "Very Large";
                case SizeCategory.VerySmall:
                    return "Very Small";
                default:
                    return "Unknown Size";
            }
        }

        public static ItemQuality GetNetQuality(
            this IEnumerable<(ItemQuality Quality, double Weight)> qualitiesAndWeights) {
            var qawList = qualitiesAndWeights.ToList();
            if (qawList.Count == 0) {
                return ItemQuality.Terrible;
            }
            var result = qawList.Sum(x => (int)x.Quality * x.Weight) /
                         (qawList.Sum(x => x.Weight) * qawList.Count);

            var intResult = (int) result;
            if (intResult < (int)ItemQuality.Terrible) {
                intResult = (int) ItemQuality.Terrible;
            }

            if (intResult > (int) ItemQuality.Legendary) {
                intResult = (int) ItemQuality.Legendary;
            }

            return (ItemQuality) intResult;
        }
    }
}