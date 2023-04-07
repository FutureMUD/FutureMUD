using System;
using System.Collections.Generic;

namespace MudSharp.CharacterCreation
{
    public interface ICharacterIntro
    {
        Queue<string> Echoes { get; set; }
        Queue<TimeSpan> Delays { get; set; }
    }
}