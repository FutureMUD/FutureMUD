#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellPossessedBodyEffect : SimpleSpellStatusEffectBase, IPossessedBodyEffect
{
	private bool _removing;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellPossessedBody", (effect, owner) => new SpellPossessedBodyEffect(effect, owner));
	}

	public SpellPossessedBodyEffect(
		IPerceivable owner,
		IMagicSpellEffectParent parent,
		string formKey,
		long anchorCharacterId,
		long anchorInstanceId,
		long shellInstanceId,
		long shellBodyId,
		long sourceTargetCharacterId,
		long sourceTargetInstanceId,
		long sourceSpellId,
		CharacterInstancePersistencePolicy persistencePolicy,
		string possessionEcho,
		string targetEcho,
		string roomEcho,
		string collapseEcho,
		string backlashEcho,
		IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		FormKey = formKey;
		AnchorCharacterId = anchorCharacterId;
		AnchorInstanceId = anchorInstanceId;
		ShellInstanceId = shellInstanceId;
		ShellBodyId = shellBodyId;
		SourceTargetCharacterId = sourceTargetCharacterId;
		SourceTargetInstanceId = sourceTargetInstanceId;
		SourceSpellId = sourceSpellId;
		PersistencePolicy = persistencePolicy;
		PossessionEcho = possessionEcho;
		TargetEcho = targetEcho;
		RoomEcho = roomEcho;
		CollapseEcho = collapseEcho;
		BacklashEcho = backlashEcho;
	}

	private SpellPossessedBodyEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		FormKey = trueRoot?.Element("FormKey")?.Value ?? string.Empty;
		AnchorCharacterId = long.Parse(trueRoot?.Element("AnchorCharacterId")?.Value ?? "0");
		AnchorInstanceId = long.Parse(trueRoot?.Element("AnchorInstanceId")?.Value ?? "0");
		ShellInstanceId = long.Parse(trueRoot?.Element("ShellInstanceId")?.Value ?? "0");
		ShellBodyId = long.Parse(trueRoot?.Element("ShellBodyId")?.Value ?? "0");
		SourceTargetCharacterId = long.Parse(trueRoot?.Element("SourceTargetCharacterId")?.Value ?? "0");
		SourceTargetInstanceId = long.Parse(trueRoot?.Element("SourceTargetInstanceId")?.Value ?? "0");
		SourceSpellId = long.Parse(trueRoot?.Element("SourceSpellId")?.Value ?? "0");
		PersistencePolicy = (trueRoot?.Element("PersistencePolicy")?.Value ?? string.Empty)
			.TryParseEnum<CharacterInstancePersistencePolicy>(out var persistence)
			? persistence
			: CharacterInstancePersistencePolicy.DespawnOnReboot;
		PossessionEcho = trueRoot?.Element("PossessionEcho")?.Value ??
		                 PossessBodySpellEffect.DefaultPossessionEcho;
		TargetEcho = trueRoot?.Element("TargetEcho")?.Value ?? string.Empty;
		RoomEcho = trueRoot?.Element("RoomEcho")?.Value ?? PossessBodySpellEffect.DefaultRoomEcho;
		CollapseEcho = trueRoot?.Element("CollapseEcho")?.Value ??
		               PossessBodySpellEffect.DefaultCollapseEcho;
		BacklashEcho = trueRoot?.Element("BacklashEcho")?.Value ?? string.Empty;
	}

	public string FormKey { get; }
	public long AnchorCharacterId { get; }
	public long AnchorInstanceId { get; }
	public long ShellInstanceId { get; }
	public long ShellBodyId { get; }
	public long SourceTargetCharacterId { get; }
	public long SourceTargetInstanceId { get; }
	public long SourceSpellId { get; }
	public CharacterInstancePersistencePolicy PersistencePolicy { get; }
	public string PossessionEcho { get; }
	public string TargetEcho { get; }
	public string RoomEcho { get; }
	public string CollapseEcho { get; }
	public string BacklashEcho { get; }

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("FormKey", FormKey),
			new XElement("AnchorCharacterId", AnchorCharacterId),
			new XElement("AnchorInstanceId", AnchorInstanceId),
			new XElement("ShellInstanceId", ShellInstanceId),
			new XElement("ShellBodyId", ShellBodyId),
			new XElement("SourceTargetCharacterId", SourceTargetCharacterId),
			new XElement("SourceTargetInstanceId", SourceTargetInstanceId),
			new XElement("SourceSpellId", SourceSpellId),
			new XElement("PersistencePolicy", PersistencePolicy),
			new XElement("PossessionEcho", new XCData(PossessionEcho)),
			new XElement("TargetEcho", new XCData(TargetEcho)),
			new XElement("RoomEcho", new XCData(RoomEcho)),
			new XElement("CollapseEcho", new XCData(CollapseEcho)),
			new XElement("BacklashEcho", new XCData(BacklashEcho))
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Sustaining possessed shell instance #{ShellInstanceId.ToString("N0", voyeur)} from target #{SourceTargetCharacterId.ToString("N0", voyeur)}.";
	}

	protected override string SpecificEffectType => "SpellPossessedBody";

	public override void InitialEffect()
	{
		base.InitialEffect();
		var anchor = AnchorCharacter();
		if (anchor is null)
		{
			Owner.RemoveEffect(this, true);
			return;
		}

		var shell = ShellInstance(anchor);
		if (shell is not ICharacter shellCharacter)
		{
			Owner.RemoveEffect(this, true);
			return;
		}

		if (Owner is ICharacter target)
		{
			SendIfNotSuppressed(target, TargetEcho);
		}

		var focus = CharacterInstanceFocusService.Focus(anchor, shell, false, suppressAutoLook: true);
		if (!focus.Success)
		{
			anchor.OutputHandler.Send(focus.Message);
			return;
		}

		EmitRoomEcho(shellCharacter, RoomEcho, shellCharacter, Owner);
		SendIfNotSuppressed(shellCharacter, PossessionEcho);
		shellCharacter.Body.Look();
	}

	public override void Login()
	{
		base.Login();
		var anchor = AnchorCharacter();
		if (anchor is null || ShellInstance(anchor) is null)
		{
			Owner.RemoveEffect(this, true);
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
			var anchor = AnchorCharacter();
			if (anchor is not null)
			{
				var shell = ShellInstance(anchor);
				if (shell is ICharacter shellCharacter)
				{
					CharacterInstanceFocusService.TryReturnFocusToPrimary(
						shellCharacter,
						CollapseEcho,
						true,
						suppressAutoLook: true);
					CharacterInstanceService.Retire(shell, out _, deleteTemporaryRows: true,
						deathRetirement: shell.State.HasFlag(CharacterState.Dead), removeOwningEffects: false);
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

	private ICharacter? AnchorCharacter()
	{
		return AnchorCharacterId > 0 ? Gameworld.TryGetCharacter(AnchorCharacterId, true) : null;
	}

	private ICharacterInstance? ShellInstance(ICharacter anchor)
	{
		return anchor.Identity.Instances.FirstOrDefault(x => x.InstanceId == ShellInstanceId);
	}

	private static void SendIfNotSuppressed(ICharacter character, string echo)
	{
		if (!string.IsNullOrWhiteSpace(echo))
		{
			character.OutputHandler?.Send(echo.SubstituteANSIColour());
		}
	}

	private static void EmitRoomEcho(ICharacter source, string echo, params IPerceivable[] targets)
	{
		if (string.IsNullOrWhiteSpace(echo) || source.Location is null)
		{
			return;
		}

		source.Location.Handle(
			source.RoomLayer,
			new EmoteOutput(
				new Emote(echo, source, targets),
				flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
	}
}
