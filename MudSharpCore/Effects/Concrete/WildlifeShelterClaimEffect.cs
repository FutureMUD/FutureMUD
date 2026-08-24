#nullable enable
using System.Globalization;
using MudSharp.GameItems;
using MudSharp.NPC;
using MudSharp.NPC.AI.Groups;

namespace MudSharp.Effects.Concrete;

/// <summary>
/// A durable claim placed on a wildlife shelter anchor. The claim deliberately lives on the
/// item rather than in a new table so AI-created shelters survive normal save/load cycles.
/// </summary>
public sealed class WildlifeShelterClaimEffect : Effect
{
	private static readonly TimeSpan ReclaimAfter = TimeSpan.FromDays(7);
	private long _ownerCharacterId;
	private long? _ownerInstanceId;
	private long? _groupId;
	private bool _allowGroupSharing;
	private DateTime _lastOccupiedUtc;

	public WildlifeShelterClaimEffect(IGameItem owner, ICharacter claimant, bool allowGroupSharing)
		: base(owner)
	{
		_ownerCharacterId = CharacterInstanceIdentityComparer.IdentityId(claimant);
		_ownerInstanceId = CharacterInstanceIdentityComparer.InstanceId(claimant);
		_groupId = (claimant as INPC)?.GroupAI?.Id;
		_allowGroupSharing = allowGroupSharing;
		_lastOccupiedUtc = RuntimeClock.UtcNow;
	}

	private WildlifeShelterClaimEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		XElement effect = root.Element("Effect") ??
		                  throw new ArgumentException("Invalid wildlife shelter claim definition.");
		_ownerCharacterId = long.Parse(effect.Attribute("OwnerCharacterId")?.Value ?? "0");
		_ownerInstanceId = long.TryParse(effect.Attribute("OwnerInstanceId")?.Value, out long instanceId) &&
		                   instanceId > 0
			? instanceId
			: null;
		_groupId = long.TryParse(effect.Attribute("GroupId")?.Value, out long groupId) && groupId > 0
			? groupId
			: null;
		_allowGroupSharing = bool.Parse(effect.Attribute("AllowGroupSharing")?.Value ?? "false");
		_lastOccupiedUtc = DateTime.TryParse(effect.Attribute("LastOccupiedUtc")?.Value,
			CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
			out DateTime lastOccupied)
			? lastOccupied
			: RuntimeClock.UtcNow;
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("WildlifeShelterClaim", (effect, owner) => new WildlifeShelterClaimEffect(effect, owner));
	}

	public static WildlifeShelterClaimEffect? Get(IGameItem item)
	{
		return item.EffectsOfType<WildlifeShelterClaimEffect>().FirstOrDefault();
	}

	public static bool CanUse(IGameItem item, ICharacter claimant, bool allowGroupSharing)
	{
		WildlifeShelterClaimEffect? effect = Get(item);
		if (effect is null)
		{
			return true;
		}

		if (effect.IsReclaimable())
		{
			item.RemoveEffect(effect, true);
			return true;
		}

		return effect.BelongsTo(claimant, allowGroupSharing);
	}

	public static bool ClaimOrRefresh(IGameItem item, ICharacter claimant, bool allowGroupSharing)
	{
		if (!CanUse(item, claimant, allowGroupSharing))
		{
			return false;
		}

		WildlifeShelterClaimEffect? effect = Get(item);
		if (effect is null)
		{
			effect = new WildlifeShelterClaimEffect(item, claimant, allowGroupSharing);
			item.AddEffect(effect);
		}
		else
		{
			effect.SetOwner(claimant, allowGroupSharing);
		}

		effect.RefreshOccupancy();
		item.ResetMorphTimer();
		return true;
	}

	public bool BelongsTo(ICharacter claimant, bool allowGroupSharing)
	{
		long claimantId = CharacterInstanceIdentityComparer.IdentityId(claimant);
		if (_ownerCharacterId == claimantId &&
		    (_ownerInstanceId is null || _ownerInstanceId == CharacterInstanceIdentityComparer.InstanceId(claimant)))
		{
			return true;
		}

		long? claimantGroupId = (claimant as INPC)?.GroupAI?.Id;
		return allowGroupSharing && _allowGroupSharing && _groupId.HasValue &&
		       claimantGroupId == _groupId;
	}

	public bool IsReclaimable()
	{
		return RuntimeClock.UtcNow - _lastOccupiedUtc >= ReclaimAfter && !HasValidOwner();
	}

	public void RefreshOccupancy()
	{
		_lastOccupiedUtc = RuntimeClock.UtcNow;
		Changed = true;
	}

	private void SetOwner(ICharacter claimant, bool allowGroupSharing)
	{
		_ownerCharacterId = CharacterInstanceIdentityComparer.IdentityId(claimant);
		_ownerInstanceId = CharacterInstanceIdentityComparer.InstanceId(claimant);
		_groupId = (claimant as INPC)?.GroupAI?.Id;
		_allowGroupSharing = allowGroupSharing;
		Changed = true;
	}

	private bool HasValidOwner()
	{
		if (_groupId.HasValue)
		{
			IGroupAI? group = Gameworld.GroupAIs.FirstOrDefault(x => x.Id == _groupId.Value);
			return group?.GroupMembers.Any(x => !x.State.IsDead()) == true;
		}

		return Gameworld.NPCs.Any(x =>
			CharacterInstanceIdentityComparer.IdentityId(x) == _ownerCharacterId &&
			(_ownerInstanceId is null || CharacterInstanceIdentityComparer.InstanceId(x) == _ownerInstanceId) &&
			!x.State.IsDead());
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XAttribute("OwnerCharacterId", _ownerCharacterId),
			_ownerInstanceId.HasValue ? new XAttribute("OwnerInstanceId", _ownerInstanceId.Value) : null,
			_groupId.HasValue ? new XAttribute("GroupId", _groupId.Value) : null,
			new XAttribute("AllowGroupSharing", _allowGroupSharing),
			new XAttribute("LastOccupiedUtc", _lastOccupiedUtc.ToString("O")));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return _groupId.HasValue
			? $"Wildlife shelter claimed by group #{_groupId.Value.ToString("N0", voyeur)}."
			: $"Wildlife shelter claimed by NPC #{_ownerCharacterId.ToString("N0", voyeur)}.";
	}

	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "WildlifeShelterClaim";
}
