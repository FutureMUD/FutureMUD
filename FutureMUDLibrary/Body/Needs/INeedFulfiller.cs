namespace MudSharp.Body.Needs {
    public interface INeedFulfiller {
        double Calories { get; }
        double SatiationPoints { get; }
        double WaterLitres { get; }
        double ThirstPoints { get; }
        double AlcoholLitres { get; }
    }
}