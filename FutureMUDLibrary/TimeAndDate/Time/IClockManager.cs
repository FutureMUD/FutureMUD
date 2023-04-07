using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Time
{
    public interface IClockManager : IHaveFuturemud
    {
        void UpdateClocks();
        void Initialise();
    }
}