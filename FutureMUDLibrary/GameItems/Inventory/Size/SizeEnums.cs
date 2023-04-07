namespace MudSharp.GameItems.Inventory.Size {
    /// <summary>
    ///     Describes how the item fits based on volumetric measurements and parameters
    /// </summary>
    public enum ItemVolumeFitDescription {
        VeryLoose = 0,
        Loose,
        Normal,
        Perfect,
        Tight,
        VeryTight
    }

    /// <summary>
    ///     Describes how the item fits based on linear measurements and parameters
    /// </summary>
    public enum ItemLinearFitDescription {
        VerySmall = 0,
        Small,
        Normal,
        Perfect,
        Large,
        VeryLarge
    }

    public static class SizeEnumsExtensions {
        public static int Score(this ItemVolumeFitDescription volume) {
            switch (volume) {
                case ItemVolumeFitDescription.Perfect:
                    return 0;
                case ItemVolumeFitDescription.Normal:
                    return 1;
                case ItemVolumeFitDescription.Loose:
                case ItemVolumeFitDescription.Tight:
                    return 2;
                case ItemVolumeFitDescription.VeryLoose:
                case ItemVolumeFitDescription.VeryTight:
                    return 3;
                default:
                    return 4;
            }
        }

        public static int Score(this ItemLinearFitDescription volume) {
            switch (volume) {
                case ItemLinearFitDescription.Perfect:
                    return 0;
                case ItemLinearFitDescription.Normal:
                    return 1;
                case ItemLinearFitDescription.Large:
                case ItemLinearFitDescription.Small:
                    return 2;
                case ItemLinearFitDescription.VeryLarge:
                case ItemLinearFitDescription.VerySmall:
                    return 3;
                default:
                    return 4;
            }
        }

        public static string Describe(this ItemVolumeFitDescription volume) {
            switch (volume) {
                case ItemVolumeFitDescription.Perfect:
                    return "Perfect";
                case ItemVolumeFitDescription.Normal:
                    return "Normal";
                case ItemVolumeFitDescription.Loose:
                    return "Loose";
                case ItemVolumeFitDescription.VeryLoose:
                    return "Very Loose";
                case ItemVolumeFitDescription.Tight:
                    return "Tight";
                case ItemVolumeFitDescription.VeryTight:
                    return "Very Tight";
                default:
                    return "Unknown";
            }
        }

        public static string Describe(this ItemLinearFitDescription volume) {
            switch (volume) {
                case ItemLinearFitDescription.Perfect:
                    return "Perfect";
                case ItemLinearFitDescription.Normal:
                    return "Normal";
                case ItemLinearFitDescription.Large:
                    return "Large";
                case ItemLinearFitDescription.Small:
                    return "Small";
                case ItemLinearFitDescription.VeryLarge:
                    return "Very Large";
                case ItemLinearFitDescription.VerySmall:
                    return "Very Small";
                default:
                    return "Unknown";
            }
        }
    }
}