namespace MudSharp.Body.Needs {
    public interface INeedsModel {
        NeedsResult Status { get; }

        /// <summary>
        ///     True if the body should save the values for the needs of this needs model
        /// </summary>
        bool NeedsSave { get; }

        double AlcoholLitres { get; set; }
        double WaterLitres { get; }
        double FoodSatiatedHours { get; }
        double DrinkSatiatedHours { get; }
        double Calories { get; }
        NeedsResult FulfilNeeds(INeedFulfiller fulfiller, bool ignoreDelays = false);
        void NeedsHeartbeat();
    }
}