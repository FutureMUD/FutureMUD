#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;

namespace MudSharp.Effects;

/// <summary>Direct, non-lazy properties of the concrete live peer-effect contracts.</summary>
internal static class EffectPhysicalReferences
{
	public static IEnumerable<PhysicalEntityReference> Read(Effect effect)
	{
		IEnumerable<IFrameworkItem?> items = effect switch
		{
			AIPosturingEffect x => x.PosturingTargets,
			BeingBound x => [x.Binder],
			Butchering.BeingButchered x => [x.Butcher],
			Skinning.BeingSkinned x => [x.Skinner],
			CharacterHitch x => [x.Target],
			SurgicalProcedureEffect x => [x.Surgeon, x.Patient],
			CharacterActionWithTarget x => [x.Target],
			ClinchEffect x => [x.Clincher, x.Target],
			Grappling x => [x.Target],
			ConnectMindEffect x => [x.TargetCharacter],
			MindConnectedToEffect x => [x.OriginatorCharacter],
			BeingStrangled x => [x.Strangler],
			BeingMagicChoked x => [x.OriginatorCharacter],
			BeingMagicallyAnesthetised x => [x.OriginatorCharacter],
			MagicChoking x => [x.CharacterTarget],
			MagicAnesthesia x => [x.CharacterTarget],
			Strangling x => [x.Target],
			Rescue x => [x.RescueTarget],
			RecentlyRescuedTarget x => [x.Rescued, x.Rescuer],
			BeDressedEffect x => [x.Dresser, x.Dressee],
			Switched x => [x.OriginalCharacter],
			AdminSpy x => [x.AdminOwner],
			Notify x => [x.NotifyTarget],
			PsionicClairaudienceEffect x => [x.TargetCharacter, x.Observer],
			PassiveInterdiction x => [x.Intercessor],
			PassiveInterdiction.PassivelyInterceding x => [x.Target],
			Dragging x => new IFrameworkItem?[] { x.Target }.Concat(x.Helpers),
			Dragging.DragHelper x => [x.Drag.Owner, x.Drag.Target],
			Dragging.DragTarget x => [x.Drag.Owner, x.Drag.Target],
			WildAnimalHerdEffect x => new IFrameworkItem?[] { x.HerdLeader }
				.Concat(x.SubordinateEffects.Select(y => y.Owner)),
			MagicClairaudienceConcentrationEffect x => [x.TargetCharacter],
			ZeroGravityTether x => [x.Anchor, x.PhysicalTether],
			SpellZeroGravityTetherEffect x => [x.Anchor],
			_ => []
		};
		foreach (var item in items)
			foreach (var reference in PhysicalEntityReference.FromItem(item, effect.FrameworkItemType)) yield return reference;
		IEnumerable<PhysicalEntityReference> ids = effect switch
		{
			MonsterStateEffect x => [new(PhysicalEntityKind.Character, x.ProvokerId, "Provoker"),
				new(PhysicalEntityKind.Character, x.WarningTargetId, "WarningTarget")],
			CreaturePursuitEffect x => [new(PhysicalEntityKind.Character, x.TargetId, "Target")],
			PsionicTraceEffect x => [new(PhysicalEntityKind.Character, x.SourceCharacterId, "SourceCharacterId"),
				new(PhysicalEntityKind.Character, x.TargetCharacterId ?? 0, "TargetCharacterId")],
			DelayedPsychicSuggestionEffect x => [new(PhysicalEntityKind.Character, x.SourceId, "SourceId")],
			TrapEffect x => [new(PhysicalEntityKind.Character, x.CreatorId, "CreatorId")],
			TrapPayloadScheduleEffect x => [new(PhysicalEntityKind.Character, x.CreatorId, "CreatorId"), new(PhysicalEntityKind.Character, x.TargetCharacterId, "TargetCharacterId")],
			SpellPhantomIllusionEffect x => [new(PhysicalEntityKind.Character, x.CasterId, "CasterId")],
			SpellSubjectiveDescriptionEffect x => [new(PhysicalEntityKind.Character, x.CasterId, "CasterId")],
			BodyBackupEffect x => [new(PhysicalEntityKind.Body, x.BackupBodyId, "BackupBodyId")],
			SpellBodyBackupEffect x => [new(PhysicalEntityKind.Body, x.BackupBodyId, "BackupBodyId")],
			ForcedTransformationBaselineEffect x => [new(PhysicalEntityKind.Body, x.BaselineBodyId, "BaselineBodyId")],
			SpellTransformFormEffect x => [new(PhysicalEntityKind.Body, x.FormBodyId, "FormBodyId"), new(PhysicalEntityKind.Body, x.PriorBodyId, "PriorBodyId")],
			AstralProjectionAnchorEffect x => [new(PhysicalEntityKind.CharacterInstance, x.ProjectionInstanceId, "ProjectionInstanceId")],
			SpellIdentifyEffect x => Caster(x.CasterId, x.CasterInstanceId),
			SpellReciteProxyEffect x => Caster(x.CasterId, x.CasterInstanceId)
				.Concat(Caster(x.LinkedCharacterId, x.LinkedInstanceId)),
			SpellLiveBodyPossessionEffect x => Caster(x.AnchorCharacterId, x.AnchorInstanceId)
				.Concat(Caster(x.TargetCharacterId, x.TargetInstanceId)).Append(new(PhysicalEntityKind.Body, x.TargetBodyId, "TargetBodyId")),
			SpellPossessedBodyEffect x => Caster(x.AnchorCharacterId, x.AnchorInstanceId)
				.Concat(Caster(x.SourceTargetCharacterId, x.SourceTargetInstanceId))
				.Append(new(PhysicalEntityKind.CharacterInstance, x.ShellInstanceId, "ShellInstanceId"))
				.Append(new(PhysicalEntityKind.Body, x.ShellBodyId, "ShellBodyId")),
			SpellCorpsePossessionEffect x => Caster(x.AnchorCharacterId, x.AnchorInstanceId)
				.Concat(Caster(x.OriginalCharacterId, x.AnimatedInstanceId))
				.Append(new(PhysicalEntityKind.Body, x.OriginalBodyId, "OriginalBodyId")),
			SpellAnimatedCorpseEffect x => Caster(x.AnchorCharacterId, x.AnchorInstanceId)
				.Concat(Caster(x.OriginalCharacterId, x.AnimatedInstanceId))
				.Append(new(PhysicalEntityKind.Body, x.OriginalBodyId, "OriginalBodyId")),
			SpellDeadSpeakEffect x => Caster(x.AnchorCharacterId, x.AnchorInstanceId)
				.Concat(Caster(x.OriginalCharacterId, x.AnimatedInstanceId))
				.Concat(Caster(x.LinkedCharacterId, x.LinkedInstanceId))
				.Append(new(PhysicalEntityKind.Body, x.OriginalBodyId, "OriginalBodyId")),
			_ => []
		};
		foreach (var reference in ids) yield return reference;
	}

	private static IEnumerable<PhysicalEntityReference> Caster(long character, long instance) =>
		[new(PhysicalEntityKind.Character, character, "CharacterId"), new(PhysicalEntityKind.CharacterInstance, instance, "InstanceId")];
}
