#nullable enable

using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellMagicalCopyEffect : SimpleSpellStatusEffectBase, IMagicalCopyEffect
{
	private bool _removing;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellMagicalCopy", (effect, owner) => new SpellMagicalCopyEffect(effect, owner));
	}

	public SpellMagicalCopyEffect(
		IPerceivable owner,
		IMagicSpellEffectParent parent,
		string formKey,
		long copyInstanceId,
		long copyBodyId,
		long anchorInstanceId,
		long sourceSpellId,
		bool playerFocusable,
		bool intangible,
		long planeId,
		CharacterInstancePersistencePolicy persistencePolicy,
		string collapseEcho,
		string backlashEcho,
		IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		FormKey = formKey;
		CopyInstanceId = copyInstanceId;
		CopyBodyId = copyBodyId;
		AnchorInstanceId = anchorInstanceId;
		SourceSpellId = sourceSpellId;
		PlayerFocusable = playerFocusable;
		Intangible = intangible;
		PlaneId = planeId;
		PersistencePolicy = persistencePolicy;
		CollapseEcho = collapseEcho;
		BacklashEcho = backlashEcho;
	}

	private SpellMagicalCopyEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		FormKey = trueRoot?.Element("FormKey")?.Value ?? string.Empty;
		CopyInstanceId = long.Parse(trueRoot?.Element("CopyInstanceId")?.Value ?? "0");
		CopyBodyId = long.Parse(trueRoot?.Element("CopyBodyId")?.Value ?? "0");
		AnchorInstanceId = long.Parse(trueRoot?.Element("AnchorInstanceId")?.Value ?? "0");
		SourceSpellId = long.Parse(trueRoot?.Element("SourceSpellId")?.Value ?? "0");
		PlayerFocusable = bool.Parse(trueRoot?.Element("PlayerFocusable")?.Value ?? "false");
		Intangible = bool.Parse(trueRoot?.Element("Intangible")?.Value ?? "true");
		PlaneId = long.Parse(trueRoot?.Element("PlaneId")?.Value ?? "0");
		PersistencePolicy = (trueRoot?.Element("PersistencePolicy")?.Value ?? string.Empty)
			.TryParseEnum<CharacterInstancePersistencePolicy>(out var persistence)
			? persistence
			: CharacterInstancePersistencePolicy.DespawnOnReboot;
		CollapseEcho = trueRoot?.Element("CollapseEcho")?.Value ?? CopySpellEffect.DefaultCollapseEcho;
		BacklashEcho = trueRoot?.Element("BacklashEcho")?.Value ?? string.Empty;
	}

	public string FormKey { get; }
	public long AnchorInstanceId { get; }
	public long CopyInstanceId { get; }
	public long CopyBodyId { get; }
	public long SourceSpellId { get; }
	public bool PlayerFocusable { get; }
	public bool Intangible { get; }
	public long PlaneId { get; }
	public CharacterInstancePersistencePolicy PersistencePolicy { get; }
	public string CollapseEcho { get; }
	public string BacklashEcho { get; }

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("FormKey", FormKey),
			new XElement("CopyInstanceId", CopyInstanceId),
			new XElement("CopyBodyId", CopyBodyId),
			new XElement("AnchorInstanceId", AnchorInstanceId),
			new XElement("SourceSpellId", SourceSpellId),
			new XElement("PlayerFocusable", PlayerFocusable),
			new XElement("Intangible", Intangible),
			new XElement("PlaneId", PlaneId),
			new XElement("PersistencePolicy", PersistencePolicy),
			new XElement("CollapseEcho", new XCData(CollapseEcho)),
			new XElement("BacklashEcho", new XCData(BacklashEcho))
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Sustaining magical copy instance #{CopyInstanceId.ToString("N0", voyeur)}.";
	}

	protected override string SpecificEffectType => "SpellMagicalCopy";

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (Owner is not ICharacter anchor || !Intangible)
		{
			return;
		}

		if (CopyInstance(anchor) is ICharacter copy)
		{
			ApplyCopyPlanarOverlay(copy);
		}
	}

	public override void Login()
	{
		base.Login();
		if (Owner is not ICharacter anchor)
		{
			return;
		}

		if (CopyInstance(anchor) is ICharacter copy)
		{
			if (Intangible)
			{
				ApplyCopyPlanarOverlay(copy);
			}

			return;
		}

		anchor.RemoveEffect(this, true);
	}

	public override void RemovalEffect()
	{
		if (_removing)
		{
			return;
		}

		_removing = true;
		try
		{
			if (Owner is ICharacter anchor)
			{
				var copy = CopyInstance(anchor);
				if (copy is ICharacter copyCharacter)
				{
					CleanupCopyPlanarOverlay(copyCharacter);
					CharacterInstanceFocusService.TryReturnFocusToPrimary(
						copyCharacter,
						CollapseEcho,
						true,
						suppressAutoLook: true);
					CharacterInstanceService.Retire(copy, out _, deleteTemporaryRows: true,
						deathRetirement: copy.State.HasFlag(CharacterState.Dead), removeOwningEffects: false);
					SendIfNotSuppressed(anchor, BacklashEcho);
				}
				else
				{
					CharacterInstanceFocusService.TryReturnFocusToPrimary(anchor, CollapseEcho, true);
				}
			}

			base.RemovalEffect();
		}
		finally
		{
			_removing = false;
		}
	}

	public override void CancelEffect()
	{
		RemovalEffect();
	}

	private ICharacterInstance? CopyInstance(ICharacter anchor)
	{
		return anchor.Identity.Instances.FirstOrDefault(x => x.InstanceId == CopyInstanceId);
	}

	private void ApplyCopyPlanarOverlay(ICharacter copy)
	{
		CleanupCopyPlanarOverlay(copy);
		copy.AddEffect(new PlanarStateEffect(
			copy,
			CopySpellEffect.CreateMagicalCopyPlanarPresence(Gameworld, PlaneId),
			9000,
			true));
	}

	private static void CleanupCopyPlanarOverlay(ICharacter copy)
	{
		copy.RemoveAllEffects<PlanarStateEffect>(
			x => x.PlanarPresenceDefinition.TransitionProfile.EqualTo(
				CopySpellEffect.MagicalCopyTransitionProfile),
			true);
	}

	private static void SendIfNotSuppressed(ICharacter character, string echo)
	{
		if (!string.IsNullOrWhiteSpace(echo))
		{
			character.OutputHandler?.Send(echo.SubstituteANSIColour());
		}
	}
}

public class SpellPhysicalCloneEffect : SimpleSpellStatusEffectBase, IPhysicalCloneEffect
{
	private bool _removing;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellPhysicalClone", (effect, owner) => new SpellPhysicalCloneEffect(effect, owner));
	}

	public SpellPhysicalCloneEffect(
		IPerceivable owner,
		IMagicSpellEffectParent parent,
		string formKey,
		long cloneInstanceId,
		long cloneBodyId,
		long anchorInstanceId,
		long sourceSpellId,
		bool playerFocusable,
		CharacterInstancePersistencePolicy persistencePolicy,
		string deathEcho,
		string backlashEcho,
		IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		FormKey = formKey;
		CloneInstanceId = cloneInstanceId;
		CloneBodyId = cloneBodyId;
		AnchorInstanceId = anchorInstanceId;
		SourceSpellId = sourceSpellId;
		PlayerFocusable = playerFocusable;
		PersistencePolicy = persistencePolicy;
		DeathEcho = deathEcho;
		BacklashEcho = backlashEcho;
	}

	private SpellPhysicalCloneEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		FormKey = trueRoot?.Element("FormKey")?.Value ?? string.Empty;
		CloneInstanceId = long.Parse(trueRoot?.Element("CloneInstanceId")?.Value ?? "0");
		CloneBodyId = long.Parse(trueRoot?.Element("CloneBodyId")?.Value ?? "0");
		AnchorInstanceId = long.Parse(trueRoot?.Element("AnchorInstanceId")?.Value ?? "0");
		SourceSpellId = long.Parse(trueRoot?.Element("SourceSpellId")?.Value ?? "0");
		PlayerFocusable = bool.Parse(trueRoot?.Element("PlayerFocusable")?.Value ?? "false");
		PersistencePolicy = (trueRoot?.Element("PersistencePolicy")?.Value ?? string.Empty)
			.TryParseEnum<CharacterInstancePersistencePolicy>(out var persistence)
			? persistence
			: CharacterInstancePersistencePolicy.DespawnOnReboot;
		DeathEcho = trueRoot?.Element("DeathEcho")?.Value ?? CloneSpellEffect.DefaultDeathEcho;
		BacklashEcho = trueRoot?.Element("BacklashEcho")?.Value ?? string.Empty;
	}

	public string FormKey { get; }
	public long AnchorInstanceId { get; }
	public long CloneInstanceId { get; }
	public long CloneBodyId { get; }
	public long SourceSpellId { get; }
	public bool PlayerFocusable { get; }
	public CharacterInstancePersistencePolicy PersistencePolicy { get; }
	public string DeathEcho { get; }
	public string BacklashEcho { get; }

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("FormKey", FormKey),
			new XElement("CloneInstanceId", CloneInstanceId),
			new XElement("CloneBodyId", CloneBodyId),
			new XElement("AnchorInstanceId", AnchorInstanceId),
			new XElement("SourceSpellId", SourceSpellId),
			new XElement("PlayerFocusable", PlayerFocusable),
			new XElement("PersistencePolicy", PersistencePolicy),
			new XElement("DeathEcho", new XCData(DeathEcho)),
			new XElement("BacklashEcho", new XCData(BacklashEcho))
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Sustaining physical clone instance #{CloneInstanceId.ToString("N0", voyeur)}.";
	}

	protected override string SpecificEffectType => "SpellPhysicalClone";

	public override void Login()
	{
		base.Login();
		if (Owner is ICharacter anchor && CloneInstance(anchor) is null)
		{
			anchor.RemoveEffect(this, true);
		}
	}

	public override void RemovalEffect()
	{
		if (_removing)
		{
			return;
		}

		_removing = true;
		try
		{
			if (Owner is ICharacter anchor)
			{
				var clone = CloneInstance(anchor);
				if (clone is ICharacter cloneCharacter)
				{
					CharacterInstanceFocusService.TryReturnFocusToPrimary(
						cloneCharacter,
						DeathEcho,
						true,
						suppressAutoLook: true);
					CharacterInstanceService.Retire(clone, out _, deleteTemporaryRows: true,
						deathRetirement: clone.State.HasFlag(CharacterState.Dead), removeOwningEffects: false);
					SendIfNotSuppressed(anchor, BacklashEcho);
				}
				else
				{
					CharacterInstanceFocusService.TryReturnFocusToPrimary(anchor, DeathEcho, true);
				}
			}

			base.RemovalEffect();
		}
		finally
		{
			_removing = false;
		}
	}

	public override void CancelEffect()
	{
		RemovalEffect();
	}

	private ICharacterInstance? CloneInstance(ICharacter anchor)
	{
		return anchor.Identity.Instances.FirstOrDefault(x => x.InstanceId == CloneInstanceId);
	}

	private static void SendIfNotSuppressed(ICharacter character, string echo)
	{
		if (!string.IsNullOrWhiteSpace(echo))
		{
			character.OutputHandler?.Send(echo.SubstituteANSIColour());
		}
	}
}
