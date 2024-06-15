namespace MudSharp.Effects;

public interface IBuilderEditingEffect : IEffect {
	object EditingItem { get; }
}

public interface IBuilderEditingEffect<T> : IBuilderEditingEffect
{
	new T EditingItem { get; }

	object IBuilderEditingEffect.EditingItem => EditingItem;
}