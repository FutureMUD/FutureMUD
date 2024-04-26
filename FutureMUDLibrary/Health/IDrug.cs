using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Health
{

    [Flags]
    public enum DrugVector {
        None = 0,
        Injected = 1 << 0,
        Ingested = 1 << 1,
        Inhaled = 1 << 2,
        Touched = 1 << 3
    }

    public class DrugDosage {
        public IDrug Drug { get; init; }
        public double Grams { get; set; }
        public DrugVector OriginalVector { get; init; }
    }

    public interface IDrug : IFrameworkItem, IFutureProgVariable {
        DrugVector DrugVectors { get; }
        IEnumerable<DrugType> DrugTypes { get; }
        string ExtraInfoFor(DrugType type);
        double IntensityPerGram { get; }
        double RelativeMetabolisationRate { get; }
        double IntensityForType(DrugType type);
        string Show(ICharacter voyeur);
        string DescribeEffect(DrugType type, IPerceiver voyeur);
    }

    public static class DrugExtensions {
        public static string Describe(this DrugVector type) {
            var list = new List<string>();
            foreach (var @enum in type.GetFlags()) {
                var subtype = (DrugVector) @enum;
                switch (subtype) {
                    case DrugVector.Ingested:
                        list.Add("Ingested");
                        break;
                    case DrugVector.Inhaled:
                        list.Add("Inhaled");
                        break;
                    case DrugVector.Injected:
                        list.Add("Injected");
                        break;
                    case DrugVector.Touched:
                        list.Add("Touched");
                        break;
                }
            }

            return list.ListToString(conjunction: "or ");
        }
    }
}