using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Climate
{
    public enum PrecipitationLevel
    {
        Parched = 0,
        Dry = 1,
        Humid = 2,
        LightRain = 3,
        Rain = 4,
        HeavyRain = 5,
        TorrentialRain = 6,
        LightSnow = 7,
        Snow = 8,
        HeavySnow = 9,
        Blizzard = 10,
        Sleet = 11
    }

    public enum WindLevel
    {
        None = 0,
        Still = 1,
        OccasionalBreeze = 2,
        Breeze = 3,
        Wind = 4,
        StrongWind = 5,
        GaleWind = 6,
        HurricaneWind = 7,
        MaelstromWind = 8
    }
}
