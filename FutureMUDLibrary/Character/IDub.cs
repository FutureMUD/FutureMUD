using System;
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Character
{
    public interface IDub : IKeyworded, ISaveable, IFrameworkItem
    {
        ICharacter Owner { get; set; }
        long TargetId { get; set; }
        string TargetType { get; set; }
        string LastDescription { get; set; }
        DateTime LastUsage { get; set; }
        bool WasIdentityConcealed { get; set; }
        string IntroducedName { get; set; }
        new IList<string> Keywords { get; }
    }
}