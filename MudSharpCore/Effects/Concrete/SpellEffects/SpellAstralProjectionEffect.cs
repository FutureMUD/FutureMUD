#nullable enable

using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellAstralProjectionEffect : SimpleSpellStatusEffectBase, IAstralProjectionEffect
{
	private bool _anchorPolicyApplied;
	private bool _appliedSleep;
	private bool _appliedStasis;
	private bool _removing;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellAstralProjection", (effect, owner) => new SpellAstralProjectionEffect(effect, owner));
	}

	public SpellAstralProjectionEffect(
		IPerceivable owner,
		IMagicSpellEffectParent parent,
		string formKey,
		long projectionInstanceId,
		long projectionBodyId,
		long anchorInstanceId,
		long planeId,
		AstralProjectionAnchorPolicy anchorPolicy,
		string projectionEcho,
		string anchorEcho,
		string anchorRoomEcho,
		string projectionRoomEcho,
		string collapseEcho,
		string backlashEcho,
		IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		FormKey = formKey;
		ProjectionInstanceId = projectionInstanceId;
		ProjectionBodyId = projectionBodyId;
		AnchorInstanceId = anchorInstanceId;
		PlaneId = planeId;
		AnchorPolicy = anchorPolicy;
		ProjectionEcho = projectionEcho;
		AnchorEcho = anchorEcho;
		AnchorRoomEcho = anchorRoomEcho;
		ProjectionRoomEcho = projectionRoomEcho;
		CollapseEcho = collapseEcho;
		BacklashEcho = backlashEcho;
	}

	private SpellAstralProjectionEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		FormKey = trueRoot?.Element("FormKey")?.Value ?? string.Empty;
		ProjectionInstanceId = long.Parse(trueRoot?.Element("ProjectionInstanceId")?.Value ?? "0");
		ProjectionBodyId = long.Parse(trueRoot?.Element("ProjectionBodyId")?.Value ?? "0");
		AnchorInstanceId = long.Parse(trueRoot?.Element("AnchorInstanceId")?.Value ?? "0");
		PlaneId = long.Parse(trueRoot?.Element("PlaneId")?.Value ?? "0");
		AnchorPolicy = (trueRoot?.Element("AnchorPolicy")?.Value ?? string.Empty)
			.TryParseEnum<AstralProjectionAnchorPolicy>(out var policy)
			? policy
			: AstralProjectionAnchorPolicy.Helpless;
		ProjectionEcho = trueRoot?.Element("ProjectionEcho")?.Value ??
		                  AstralProjectionSpellEffect.DefaultProjectionEcho;
		AnchorEcho = trueRoot?.Element("AnchorEcho")?.Value ?? AstralProjectionSpellEffect.DefaultAnchorEcho;
		AnchorRoomEcho = trueRoot?.Element("AnchorRoomEcho")?.Value ??
		                 AstralProjectionSpellEffect.DefaultAnchorRoomEcho;
		ProjectionRoomEcho = trueRoot?.Element("ProjectionRoomEcho")?.Value ??
		                     AstralProjectionSpellEffect.DefaultProjectionRoomEcho;
		CollapseEcho = trueRoot?.Element("CollapseEcho")?.Value ??
		               AstralProjectionSpellEffect.DefaultCollapseEcho;
		BacklashEcho = trueRoot?.Element("BacklashEcho")?.Value ?? string.Empty;
		_anchorPolicyApplied = bool.Parse(trueRoot?.Element("AnchorPolicyApplied")?.Value ?? "false");
		_appliedSleep = bool.Parse(trueRoot?.Element("AppliedSleep")?.Value ?? "false");
		_appliedStasis = bool.Parse(trueRoot?.Element("AppliedStasis")?.Value ?? "false");
	}

	public string FormKey { get; }
	public long AnchorInstanceId { get; }
	public long ProjectionInstanceId { get; }
	public long ProjectionBodyId { get; }
	public long PlaneId { get; }
	public AstralProjectionAnchorPolicy AnchorPolicy { get; }
	public string ProjectionEcho { get; }
	public string AnchorEcho { get; }
	public string AnchorRoomEcho { get; }
	public string ProjectionRoomEcho { get; }
	public string CollapseEcho { get; }
	public string BacklashEcho { get; }

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("FormKey", FormKey),
			new XElement("ProjectionInstanceId", ProjectionInstanceId),
			new XElement("ProjectionBodyId", ProjectionBodyId),
			new XElement("AnchorInstanceId", AnchorInstanceId),
			new XElement("PlaneId", PlaneId),
			new XElement("AnchorPolicy", AnchorPolicy),
			new XElement("ProjectionEcho", new XCData(ProjectionEcho)),
			new XElement("AnchorEcho", new XCData(AnchorEcho)),
			new XElement("AnchorRoomEcho", new XCData(AnchorRoomEcho)),
			new XElement("ProjectionRoomEcho", new XCData(ProjectionRoomEcho)),
			new XElement("CollapseEcho", new XCData(CollapseEcho)),
			new XElement("BacklashEcho", new XCData(BacklashEcho)),
			new XElement("AnchorPolicyApplied", _anchorPolicyApplied),
			new XElement("AppliedSleep", _appliedSleep),
			new XElement("AppliedStasis", _appliedStasis)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Projecting through instance #{ProjectionInstanceId.ToString("N0", voyeur)} on plane #{PlaneId.ToString("N0", voyeur)}.";
	}

	protected override string SpecificEffectType => "SpellAstralProjection";

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (Owner is not ICharacter anchor)
		{
			return;
		}

		var projection = ProjectionInstance(anchor);
		if (projection is not ICharacter projectionCharacter)
		{
			return;
		}

		ApplyAnchorPolicy(anchor);
		ApplyProjectionPlanarOverlay(projectionCharacter);
		EmitRoomEcho(anchor, AnchorRoomEcho, anchor, projectionCharacter);
		SendIfNotSuppressed(anchor, AnchorEcho);
		var focus = CharacterInstanceFocusService.Focus(anchor, projection, false, suppressAutoLook: true);
		if (!focus.Success)
		{
			anchor.OutputHandler.Send(focus.Message);
			return;
		}

		EmitRoomEcho(projectionCharacter, ProjectionRoomEcho, projectionCharacter, anchor);
		SendIfNotSuppressed(projectionCharacter, ProjectionEcho);
		projectionCharacter.Body.Look();
	}

	public override void Login()
	{
		base.Login();
		if (Owner is not ICharacter anchor)
		{
			return;
		}

		if (ProjectionInstance(anchor) is null)
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
				CleanupAnchorPolicy(anchor);
				var projection = ProjectionInstance(anchor);
				if (projection is ICharacter projectionCharacter)
				{
					CleanupProjectionPlanarOverlay(projectionCharacter);
					CharacterInstanceFocusService.TryReturnFocusToPrimary(
						projectionCharacter,
						CollapseEcho,
						true,
						suppressAutoLook: true);
					CharacterInstanceService.Retire(projection, out _, deleteTemporaryRows: true,
						deathRetirement: projection.State.HasFlag(CharacterState.Dead), removeOwningEffects: false);
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

	private ICharacterInstance? ProjectionInstance(ICharacter anchor)
	{
		return anchor.Identity.Instances.FirstOrDefault(x => x.InstanceId == ProjectionInstanceId);
	}

	private void ApplyAnchorPolicy(ICharacter anchor)
	{
		if (_anchorPolicyApplied)
		{
			return;
		}

		switch (AnchorPolicy)
		{
			case AstralProjectionAnchorPolicy.Helpless:
				if (!anchor.EffectsOfType<AstralProjectionAnchorEffect>(
					    x => x.ProjectionInstanceId == ProjectionInstanceId).Any())
				{
					anchor.AddEffect(new AstralProjectionAnchorEffect(anchor, ProjectionInstanceId));
				}

				break;
			case AstralProjectionAnchorPolicy.Sleep:
				if (!anchor.State.HasFlag(CharacterState.Sleeping))
				{
					_appliedSleep = true;
					anchor.Sleep();
				}

				break;
			case AstralProjectionAnchorPolicy.Stasis:
				if (!anchor.State.HasFlag(CharacterState.Stasis))
				{
					_appliedStasis = true;
					anchor.State |= CharacterState.Stasis;
				}

				break;
		}

		_anchorPolicyApplied = true;
		Changed = true;
	}

	private void CleanupAnchorPolicy(ICharacter anchor)
	{
		anchor.RemoveAllEffects<AstralProjectionAnchorEffect>(
			x => x.ProjectionInstanceId == ProjectionInstanceId,
			true);

		if (_appliedSleep &&
		    anchor.State.HasFlag(CharacterState.Sleeping) &&
		    !anchor.EffectsOfType<ISleepEffect>().Any())
		{
			anchor.Awaken();
		}

		if (_appliedStasis && anchor is MudSharp.Character.Character concrete &&
		    anchor.State.HasFlag(CharacterState.Stasis))
		{
			concrete.ResumeAfterProjectionAnchorStasis();
		}

		_anchorPolicyApplied = false;
		_appliedSleep = false;
		_appliedStasis = false;
		Changed = true;
	}

	private void ApplyProjectionPlanarOverlay(ICharacter projection)
	{
		CleanupProjectionPlanarOverlay(projection);
		projection.AddEffect(new PlanarStateEffect(
			projection,
			AstralProjectionSpellEffect.CreateAstralProjectionPlanarPresence(Gameworld, PlaneId),
			10000,
			true));
	}

	private static void CleanupProjectionPlanarOverlay(ICharacter projection)
	{
		projection.RemoveAllEffects<PlanarStateEffect>(
			x => x.PlanarPresenceDefinition.TransitionProfile.EqualTo(
				AstralProjectionSpellEffect.AstralProjectionTransitionProfile),
			true);
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
