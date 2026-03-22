#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

public sealed class NpcHomeBaseEffect : Effect
{
	private long _homeCellId;
	private long _anchorItemId;

	public NpcHomeBaseEffect(ICharacter owner)
		: base(owner)
	{
	}

	private NpcHomeBaseEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var effect = root.Element("Effect") ?? throw new ArgumentException("Invalid NPC home-base effect definition.");
		_homeCellId = long.Parse(effect.Attribute("HomeCellId")?.Value ?? "0");
		_anchorItemId = long.Parse(effect.Attribute("AnchorItemId")?.Value ?? "0");
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("NpcHomeBase", (effect, owner) => new NpcHomeBaseEffect(effect, owner));
	}

	public static NpcHomeBaseEffect GetOrCreate(ICharacter owner)
	{
		var existing = owner.CombinedEffectsOfType<NpcHomeBaseEffect>().FirstOrDefault();
		if (existing is not null)
		{
			return existing;
		}

		existing = new NpcHomeBaseEffect(owner);
		owner.AddEffect(existing);
		return existing;
	}

	public ICell? HomeCell => _homeCellId > 0 ? Gameworld.Cells.Get(_homeCellId) : null;
	public IGameItem? AnchorItem => _anchorItemId > 0 ? Gameworld.TryGetItem(_anchorItemId, true) : null;
	public bool HasHome => HomeCell is not null;
	public bool HasAnchor => AnchorItem is not null;

	public void SetHomeCell(ICell? cell)
	{
		var newId = cell?.Id ?? 0L;
		if (_homeCellId == newId)
		{
			return;
		}

		_homeCellId = newId;
		Changed = true;
	}

	public void SetAnchorItem(IGameItem? item)
	{
		var newId = item?.Id ?? 0L;
		if (_anchorItemId == newId)
		{
			return;
		}

		_anchorItemId = newId;
		Changed = true;
	}

	public void ClearAnchorItem()
	{
		SetAnchorItem(null);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XAttribute("HomeCellId", _homeCellId),
			new XAttribute("AnchorItemId", _anchorItemId));
	}

	public override string Describe(IPerceiver voyeur)
	{
		var home = HomeCell?.HowSeen(voyeur) ?? "no home";
		var anchor = AnchorItem?.HowSeen(voyeur) ?? "no anchor";
		return $"NPC home base at {home}, anchor {anchor}.";
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "NpcHomeBase";
}
