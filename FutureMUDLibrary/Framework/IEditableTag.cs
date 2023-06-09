﻿using MudSharp.Framework.Save;

namespace MudSharp.Framework
{
    public interface IEditableTag : ISaveable, ITag {
        new ITag Parent { get; set; }
        void Delete();
    }
}