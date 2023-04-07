using MudSharp.Body;

namespace MudSharp.GameItems.Interfaces {
    public interface IEdible : IGameItemComponent {
        double Calories { get; }
        double SatiationPoints { get; }
        double WaterLitres { get; }
        double ThirstPoints { get; }
        double AlcoholLitres { get; }
        string TasteString { get; }
        double TotalBites { get; }
        double BitesRemaining { get; set; }
        void Eat(IBody body, double bites);
    }
}