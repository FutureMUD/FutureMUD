using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Communication
{
    public interface ICanBeRead : IFrameworkItem, ISaveable
    {
        int DocumentLength { get; }
        ICharacter Author { get; }
        WritingImplementType ImplementType { get; }
        string ParseFor(ICharacter voyeur);
        string DescribeInLook(ICharacter voyeur);
    }
}
