#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;

namespace MudSharp.GameItems.Components;

public sealed class FoldedPocketGameItemComponent : ContainerGameItemComponent, ISpellPocket
{
	private XElement? _unreadable;
	public Guid LifecycleId { get; private set; }
	public SpellPocketAnchor? Anchor { get; private set; }
	protected override double WeightCapacity => Anchor?.Capacity ?? 0;
	protected override SizeCategory MaximumContentsSize => Anchor?.Configuration.MaximumSize ?? SizeCategory.Nanoscopic;
	public FoldedPocketGameItemComponent(FoldedPocketGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary) { }
	public FoldedPocketGameItemComponent(Models.GameItemComponent row, FoldedPocketGameItemComponentProto proto, IGameItem parent) : base(row, proto, parent)
	{
		try
		{
			var root = XElement.Parse(row.Definition);
			if (root.Name != "Definition" || root.Attributes().Any(x => x.Name != "Open") ||
				root.Elements().Any(x => x.Name != "Contained" && x.Name != "Locks" && x.Name != "Binding") ||
				root.Elements("Locks").Count() > 1 || root.Element("Locks")?.Elements().Any() == true ||
				root.Elements("Contained").Any(x => x.HasElements || x.HasAttributes)) throw new FormatException("Invalid pocket component envelope.");
			var binding = root.Elements("Binding").SingleOrDefault();
			if (binding is null) return;
			if ((string?)binding.Attribute("version") != "1" || binding.Attributes().Count() != 1 ||
				binding.Elements().Count() != 2 || binding.Elements("Lifecycle").Count() != 1 || binding.Elements("PocketAnchor").Count() != 1 ||
				binding.Element("Lifecycle")!.HasElements || binding.Element("Lifecycle")!.HasAttributes)
				throw new FormatException("Invalid pocket binding.");
			LifecycleId = Guid.Parse(binding.Element("Lifecycle")!.Value);
			Anchor = SpellPocketAnchor.Load(binding.Element("PocketAnchor")!.ToString());
			if (LifecycleId == Guid.Empty) throw new FormatException("Missing pocket lifecycle.");
		}
		catch (Exception ex) when (ex is FormatException or ArgumentException or InvalidOperationException or OverflowException)
		{ _unreadable = XElement.Parse(row.Definition); Anchor = null; }
	}
	internal void Bind(SpellLifecycleOrigin origin, SpellPocketAnchor anchor)
	{
		if (LifecycleId != Guid.Empty || Anchor is not null || origin.Family != SpellPocketAnchor.Family ||
			origin.Mode != SpellLifecycleMode.TemporaryCleanup || origin.Provenance != anchor.Save()) throw new InvalidOperationException("Pocket binding needs exact fresh creation authority.");
		LifecycleId = origin.Id; Anchor = anchor; Changed = true;
	}
	public bool CanAccess(ICharacter actor) => Anchor is not null && ReferenceEquals(actor.Gameworld, Gameworld) &&
		Gameworld.SpellOwnedPockets?.CanWithdraw(Parent) == true &&
		(Anchor.Configuration.Access == SpellPocketAccess.Bearer || Parent.SpellCreationOrigin?.CreatorId == CharacterInstanceIdentityComparer.IdentityId(actor));
	public override bool CanPut(IGameItem item) => Anchor is not null && base.CanPut(item);
	public override int CanPutAmount(IGameItem item) => CanPut(item) ? item.Quantity : 0;
	public override void Put(ICharacter? putter, IGameItem item, bool allowMerge = true)
	{
		if (putter is null || !CanAccess(putter) || !CanPut(item)) return;
		base.Put(putter, item, allowMerge: false);
	}
	public override bool CanTake(ICharacter taker, IGameItem item, int quantity) => CanAccess(taker) && base.CanTake(taker, item, quantity);
	public override IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		if (!CanTake(taker, item, quantity)) throw new InvalidOperationException("This pocket refuses access.");
		return base.Take(taker, item, quantity);
	}
	public override WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item) => !CanAccess(taker) ? WhyCannotGetContainerReason.UnlawfulAction : base.WhyCannotTake(taker, item);
	public override void Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)
	{ if (CanAccess(emptier)) base.Empty(emptier, intoContainer, playerEmote); }
	public override bool PreventsMerging(IGameItemComponent component) => true;
	public override bool InstallLock(ILock theLock, ICharacter? actor = null) => false;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new FoldedPocketGameItemComponent((FoldedPocketGameItemComponentProto)_prototype, newParent, temporary);
	protected override string SaveToXml()
	{
		if (_unreadable is not null) return _unreadable.ToString();
		var root = XElement.Parse(base.SaveToXml());
		if (Anchor is not null) root.Add(new XElement("Binding", new XAttribute("version", 1), new XElement("Lifecycle", LifecycleId), XElement.Parse(Anchor.Save())));
		return root.ToString();
	}
	public override void Delete()
	{ if (Contents.Any()) throw new InvalidOperationException("Pocket contents must be conserved before carrier deletion."); base.Delete(); }
	public override bool HandleDieOrMorph(IGameItem newItem, IRoom location) => throw new InvalidOperationException("Pocket collapse requires its conservation adapter.");
}
