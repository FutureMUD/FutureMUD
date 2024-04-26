namespace MudSharp.Body.Needs {
    /// <summary>
    ///     A simple public class to use with IFulfilNeeds. Usually consumed by edible items or spells
    /// </summary>
    public class NeedFulfiller : INeedFulfiller {
        #region IFulfilNeeds Members

        public double Calories { get; init; }

        public double SatiationPoints { get; init; }

        public double WaterLitres { get; init; }

        public double ThirstPoints { get; init; }

        public double AlcoholLitres { get; init; }

        #endregion
    }
}