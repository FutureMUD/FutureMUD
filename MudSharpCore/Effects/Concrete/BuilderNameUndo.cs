#nullable enable
using System;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

/// <summary>A single, guarded scalar compensation, never a transaction or arbitrary object snapshot.</summary>
internal sealed class BuilderNameUndo(ICharacter owner, object helper, IFrameworkItem target,
	string expectedName, Func<bool> restore) : Effect(owner)
{
	public object Helper { get; } = helper;
	public IFrameworkItem Target { get; } = target;
	public string ExpectedName { get; } = expectedName;
	public bool Restore() => restore();
	protected override string SpecificEffectType => "BuilderNameUndo";
	public override string Describe(IPerceiver voyeur) => $"Name recovery for {Target.FrameworkItemType} #{Target.Id}";
}
