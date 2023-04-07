using System;
using System.Collections.Generic;
using MudSharp.Body.PartProtos;
using MudSharp.Framework;

namespace MudSharp.Body {
    public enum LimbType {
        Arm,
        Leg,
        Appendage,
        Head,
        Wing,
        Torso,
        Genitals
    }

    public interface ILimb : IFrameworkItem {
        IBodyPrototype Prototype { get; }
        IEnumerable<IBodypart> Parts { get; }
        IBodypart RootBodypart { get; }
        LimbType LimbType { get; }
        double LimbDamageThresholdMultiplier { get; }
        double LimbPainThresholdMultiplier { get; }
        void AddBodypart(IBodypart part);
        void RemoveBodypart(IBodypart part);
        IEnumerable<ISpineProto> SpineProtos { get; }
    }

    public static class LimbExtensions {
        public static string Describe(this LimbType type) {
            switch (type) {
                case LimbType.Arm:
                    return "Arm";
                case LimbType.Leg:
                    return "Leg";
                case LimbType.Appendage:
                    return "Appendage";
                case LimbType.Head:
                    return "Head";
                case LimbType.Wing:
                    return "Wing";
                case LimbType.Torso:
                    return "Torso";
                case LimbType.Genitals:
                    return "Genitals";
            }

            throw new ApplicationException("Unknown LimbType in LimbExtensions.Describe");
        }

        public static string DescribePlural(this LimbType type) {
            switch (type) {
                case LimbType.Arm:
                    return "Arms";
                case LimbType.Leg:
                    return "Legs";
                case LimbType.Appendage:
                    return "Appendages";
                case LimbType.Head:
                    return "Heads";
                case LimbType.Wing:
                    return "Wings";
                case LimbType.Torso:
                    return "Torsos";
                case LimbType.Genitals:
                    return "Genitals";
            }

            throw new ApplicationException("Unknown LimbType in LimbExtensions.DescribePlural");
        }
    }
}