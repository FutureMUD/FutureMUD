#nullable enable

using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

/// <summary>Saved, copyable keys for content that must initialise each item only once.</summary>
public sealed class ItemInitialisation : Effect
{
	private readonly HashSet<string> _keys = new(StringComparer.Ordinal);
	public ItemInitialisation(IGameItem owner) : base(owner) { }
	private ItemInitialisation(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		foreach (var key in xml.Element("Keys")?.Elements("Key") ?? []) _keys.Add(key.Value);
	}
	public bool TryInitialise(string key)
	{
		if (string.IsNullOrWhiteSpace(key) || !_keys.Add(key)) return false;
		Changed = true;
		return true;
	}
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "ItemInitialisation";
	protected override XElement SaveDefinition() => new("Keys", _keys.OrderBy(x => x).Select(x => new XElement("Key", x)));
	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		var copy = new ItemInitialisation(newItem);
		copy._keys.UnionWith(_keys);
		return copy;
	}
	public override bool PreventsItemFromMerging(IGameItem item, IGameItem otherItem) =>
		!otherItem.Effects.OfType<ItemInitialisation>().Any(x => _keys.SetEquals(x._keys));
	public override string Describe(IPerceiver voyeur) => $"Initialised item content: {_keys.OrderBy(x => x).ListToString()}.";
	public static void InitialiseEffectType() => RegisterFactory("ItemInitialisation", (xml, owner) => new ItemInitialisation(xml, owner));
}
