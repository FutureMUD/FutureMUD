﻿using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Form.Shape {
    public delegate void LocatableEvent(ILocateable locatable, ICellExit exit);
    public interface ILocateable : IFrameworkItem, IKeyworded {
        ICell Location { get; }
        event LocatableEvent OnLocationChanged;
        event LocatableEvent OnLocationChangedIntentionally;
    }
}