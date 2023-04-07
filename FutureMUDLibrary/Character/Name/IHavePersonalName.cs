using System.Collections.Generic;

namespace MudSharp.Character.Name {
    public interface IHavePersonalName {
        IPersonalName PersonalName { get; }
    }

    public interface IEditableNameData : IHavePersonalName
    {
        new IPersonalName PersonalName { get; set; }
        IList<IPersonalName> Aliases { get; }
        bool NamesChanged { get; set; }
        IPersonalName CurrentName { get; set; }
    }
}