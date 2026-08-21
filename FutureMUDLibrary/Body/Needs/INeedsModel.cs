namespace MudSharp.Body.Needs
{
    public interface INeedsModel
    {
        /// <summary>
        ///     The stable persisted discriminator for this needs-model implementation.
        /// </summary>
        string ModelName { get; }

        NeedsResult Status { get; }

        /// <summary>
        ///     True if the body should save the values for the needs of this needs model
        /// </summary>
        bool NeedsSave { get; }

        double AlcoholLitres { get; set; }
        double WaterLitres { get; }
        double FoodSatiatedHours { get; }
        double DrinkSatiatedHours { get; }
        double SatiationReserve { get; }
        double StarvationLevel { get; }
        double OversatiationLevel { get; }
        double SatiationExcess { get; }
        double SatiationDeficit { get; }
        NeedsResult FulfilNeeds(INeedFulfiller fulfiller, bool ignoreDelays = false);
        void NeedsHeartbeat();
    }
}
