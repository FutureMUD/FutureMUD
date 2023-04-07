using System.Collections.Generic;

namespace MudSharp.Commands.Modules {
    public enum CommandDisplayOptions {
        None,
        DisplayCommandWords,
        Hidden
    }

    public interface IModule {
        string Name { get; }

        Dictionary<IModule, ModuleCompatibility.Test> CompatibilityRules { get; }
    }
}