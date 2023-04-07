namespace MudSharp.Body.Needs {
    /// <summary>
    ///     A simple public class to use with IFulfilNeeds. Usually consumed by edible items or spells
    /// </summary>
    public class NeedFulfiller : INeedFulfiller {
        #region IFulfilNeeds Members

        public double Calories { get; set; }

        public double SatiationPoints { get; set; }

        public double WaterLitres { get; set; }

        public double ThirstPoints { get; set; }

        public double AlcoholLitres { get; set; }

        #endregion
    }
}