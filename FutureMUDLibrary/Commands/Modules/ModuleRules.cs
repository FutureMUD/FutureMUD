namespace MudSharp.Commands.Modules {
    // TODO: There is room to make this more complicated
    public static class ModuleCompatibility {
        public delegate bool Test(IModule a, IModule b);

        public static bool IsCompatible<T>(this IModule thisModule, IModule otherModule) {

            thisModule.CompatibilityRules.TryGetValue(otherModule, out Test testFromThis);
            otherModule.CompatibilityRules.TryGetValue(thisModule, out Test testFromOther);

            return (testFromThis?.Invoke(thisModule, otherModule) != false) &
                   (testFromOther?.Invoke(otherModule, thisModule) != false);
        }

        public static class Blocks {
            public static bool Test(IModule a, IModule b) {
                return false;
            }
        }

        public static class Requires {
            public static bool Test(IModule a, IModule b) {
                return true;
            }
        }
    }
}