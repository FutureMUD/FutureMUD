using System;
using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces {
    public interface IActionEffect : IEffectSubtype {
        string ActionDescription { get; }
        Action<IPerceivable> Action { get; }
    }
}