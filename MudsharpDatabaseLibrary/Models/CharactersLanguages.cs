using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharactersLanguages
    {
        public long CharacterId { get; set; }
        public long LanguageId { get; set; }

        public virtual Character Character { get; set; }
        public virtual Language Language { get; set; }
    }
}
