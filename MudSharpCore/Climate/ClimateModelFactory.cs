using MudSharp.Climate.ClimateModels;

namespace MudSharp.Climate;

public static class ClimateModelFactory
{
    public static IClimateModel LoadClimateModel(Models.ClimateModel model, IFuturemud gameworld)
    {
        switch (model.Type)
        {
            case "terrestrial":
                return new TerrestrialClimateModel(model, gameworld);
        }

        throw new ApplicationException($"Unknown ClimateModel type {model.Type}");
    }
}