#nullable enable
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public sealed class NpcBurrowFoodEffect : Effect
{
	private long _pendingVictimId;
	private long _foodItemId;

	public NpcBurrowFoodEffect(ICharacter owner)
		: base(owner)
	{
	}

	private NpcBurrowFoodEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		XElement effect = root.Element("Effect") ??
		                  throw new ArgumentException("Invalid NPC burrow food effect definition.");
		_pendingVictimId = long.Parse(effect.Attribute("PendingVictimId")?.Value ?? "0");
		_foodItemId = long.Parse(effect.Attribute("FoodItemId")?.Value ?? "0");
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("NpcBurrowFood", (effect, owner) => new NpcBurrowFoodEffect(effect, owner));
	}

	public static NpcBurrowFoodEffect GetOrCreate(ICharacter owner)
	{
		NpcBurrowFoodEffect? existing = owner.CombinedEffectsOfType<NpcBurrowFoodEffect>().FirstOrDefault();
		if (existing is not null)
		{
			return existing;
		}

		existing = new NpcBurrowFoodEffect(owner);
		owner.AddEffect(existing);
		return existing;
	}

	public static NpcBurrowFoodEffect? Get(ICharacter owner)
	{
		return owner.CombinedEffectsOfType<NpcBurrowFoodEffect>().FirstOrDefault();
	}

	public long PendingVictimId => _pendingVictimId;
	public ICharacter? PendingVictim => _pendingVictimId > 0 ? Gameworld.Characters.Get(_pendingVictimId) : null;
	public IGameItem? FoodItem => _foodItemId > 0 ? Gameworld.TryGetItem(_foodItemId, true) : null;
	public ICorpse? FoodCorpse => FoodItem?.GetItemType<ICorpse>();
	public bool HasFood => FoodCorpse is not null;
	public bool HasPendingVictim => PendingVictim is not null;
	public bool HasAnyTarget => HasFood || HasPendingVictim;

	public void SetPendingVictim(ICharacter? victim)
	{
		long newId = victim?.Id ?? 0L;
		if (_pendingVictimId == newId)
		{
			return;
		}

		_pendingVictimId = newId;
		Changed = true;
	}

	public void SetFoodItem(IGameItem? item)
	{
		long newId = item?.Id ?? 0L;
		if (_foodItemId == newId)
		{
			return;
		}

		_foodItemId = newId;
		Changed = true;
	}

	public void ClearPendingVictim()
	{
		SetPendingVictim(null);
	}

	public void ClearFood()
	{
		SetFoodItem(null);
	}

	public void Clear()
	{
		bool changed = _pendingVictimId != 0 || _foodItemId != 0;
		_pendingVictimId = 0;
		_foodItemId = 0;
		if (changed)
		{
			Changed = true;
		}
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XAttribute("PendingVictimId", _pendingVictimId),
			new XAttribute("FoodItemId", _foodItemId));
	}

	public override string Describe(IPerceiver voyeur)
	{
		string food = FoodItem?.HowSeen(voyeur) ?? "no food";
		string victim = PendingVictim?.HowSeen(voyeur) ?? "no pending victim";
		return $"NPC burrow food target {food}, pending victim {victim}.";
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "NpcBurrowFood";
}
