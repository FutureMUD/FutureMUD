namespace MudSharp.Body.Needs
{
    public interface INeedFulfiller
    {
        double SatiationPoints { get; }
        double WaterLitres { get; }
        double ThirstPoints { get; }
        double AlcoholLitres { get; }
    }
}
