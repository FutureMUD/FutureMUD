#nullable enable

using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.AI;
using MudSharp.Planes;

namespace MudSharp.Magic.SpellEffects;

public sealed partial class CreateNPCEffect
{
	public SpellNpcMissionKind MissionKind { get; private set; }
	private string? _missionLoadError;
	private XElement[]? _unreadableMissions;
	private SpellNpcMissionInputs? _boundMissionInputs;
	private bool HasMissionPolicy => MissionKind != SpellNpcMissionKind.None || _missionLoadError is not null;

	private void LoadMissionPolicy(XElement root)
	{
		var schemas = root.Elements().Where(x => x.Name.LocalName == "Mission").ToArray();
		if (schemas.Length == 0) return;
		var schema = schemas[0];
		if (schemas.Length != 1 || schema.Name != "Mission" || (string?)schema.Attribute("version") != "1" ||
			schema.HasElements || schema.Attributes().Any(x => x.Name != "version" && x.Name != "kind") ||
			!string.IsNullOrWhiteSpace(schema.Value) || !Enum.TryParse<SpellNpcMissionKind>((string?)schema.Attribute("kind"), true, out var kind) ||
			!Enum.IsDefined(kind))
		{
			_missionLoadError = "Invalid NPC mission schema; use version 1 and a declared mission kind.";
			_unreadableMissions = schemas.Select(x => new XElement(x)).ToArray();
			return;
		}
		MissionKind = kind;
	}

	private IEnumerable<XElement> SaveMissionPolicy()
	{
		if (_unreadableMissions is not null)
		{
			foreach (var schema in _unreadableMissions) yield return new XElement(schema);
			yield break;
		}
		if (MissionKind != SpellNpcMissionKind.None)
			yield return new XElement("Mission", new XAttribute("version", 1), new XAttribute("kind", MissionKind));
	}

	private string? MissionPolicyDefinitionError()
	{
		if (_missionLoadError is not null) return _missionLoadError;
		if (MissionKind == SpellNpcMissionKind.None) return null;
		if (LifecycleMode is not (SpellLifecycleMode.TemporaryCleanup or SpellLifecycleMode.DeathOnExpiry))
			return "NPC missions require an explicit temporary native lifecycle.";
		if (MissionKind == SpellNpcMissionKind.Courier && (!int.TryParse(_count, out var count) || count != 1))
			return "One exact courier parcel requires exactly one summoned courier.";
		var purpose = MissionKind switch { SpellNpcMissionKind.Seek => "owned_seek", SpellNpcMissionKind.Courier => "owned_courier", _ => "owned_haunt" };
		foreach (var template in DefinitionTemplates)
		{
			var controllers = template?.ArtificialIntelligences.OfType<SpellCompanionAI>().ToArray();
			if (controllers is not { Length: 1 } || controllers[0].Purpose != purpose)
				return $"Every mission template requires one native companion controller with purpose {purpose}.";
		}
		return null;
	}

	public bool TryBindInvocationInputs(ICharacter caster, IPerceivable recipient,
		IReadOnlyList<SpellAdditionalParameter> parameters, out string? error)
	{
		error = MissionPolicyDefinitionError();
		if (error is not null) return false;
		if (MissionKind == SpellNpcMissionKind.None) return true;
		if (Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } || parameters.Count != 1 ||
			parameters[0].ParameterName != SpellNpcMissionInputs.ParameterName || parameters[0].Item is not SpellNpcMissionInputs inputs ||
			inputs.Kind != MissionKind || inputs.ExactTarget is null ||
			(MissionKind == SpellNpcMissionKind.Courier ? inputs.ExactParcel is null : inputs.ExactParcel is not null))
		{
			error = "A configured native NPC mission requires its exact typed target and parcel inputs.";
			return false;
		}
		if (_boundMissionInputs is not null && !SameMissionInputs(_boundMissionInputs, inputs))
		{
			error = "The bound NPC mission inputs changed during this invocation.";
			return false;
		}
		_boundMissionInputs = inputs;
		var inputsCurrent = CaptureMissionInputFence(caster);
		if (!ValidateMissionInvocation(caster, recipient, out error)) return false;
		if (inputsCurrent()) return true;
		error = "The exact NPC mission body, target or parcel inputs changed during binding callbacks.";
		return false;
	}

	private bool ValidateMissionInvocation(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = null;
		if (MissionKind == SpellNpcMissionKind.None) return true;
		if (_boundMissionInputs is not { } inputs || inputs.Kind != MissionKind ||
			recipient is not IRoom || !ReferenceEquals(recipient, caster.Location))
		{
			error = "An NPC mission needs explicit inputs and the caster's current spawn room.";
			return false;
		}
		var target = inputs.ExactTarget;
		if (!ReferenceEquals(target.Gameworld, Gameworld) || !Gameworld.Actors.Any(x => ReferenceEquals(x, target)) ||
			CharacterInstanceIdentityComparer.IdentityId(target) <= 0 || target.InstanceId <= 0 || target.Body is not { Id: > 0 } ||
			target.State.HasFlag(CharacterState.Dead) || target.State.HasFlag(CharacterState.Stasis) ||
			!caster.CanSee(target) || !caster.CanInteractPlanar(target, PlanarInteractionKind.Magic) ||
			MissionKind != SpellNpcMissionKind.Courier && !caster.ColocatedWith(target))
		{
			error = "The exact mission target must be loaded, visible, physically valid and reachable from the spawn frame.";
			return false;
		}
		if (MissionKind == SpellNpcMissionKind.Haunt && !SpellNpcControlEffect.IsDurablyPrimaryTarget(target))
		{
			error = "Haunting requires the target's exact persisted primary body instance.";
			return false;
		}
		if (MissionKind == SpellNpcMissionKind.Courier && inputs.ExactParcel is { } parcel &&
			(parcel.Id <= 0 || parcel.Quantity <= 0 || parcel.Deleted || parcel.Destroyed ||
			 !ReferenceEquals(parcel.Gameworld, Gameworld) || !ReferenceEquals(Gameworld.Items.Get(parcel.Id), parcel) ||
			 !caster.Body.HeldOrWieldedItems.Any(x => ReferenceEquals(x, parcel)) || !caster.CanSee(parcel)))
		{
			error = "The courier parcel must remain one exact registered item held or wielded by the caster.";
			return false;
		}
		return true;
	}

	private static bool SameMissionInputs(SpellNpcMissionInputs? first, SpellNpcMissionInputs? second) =>
		first is null ? second is null : second is not null && first.Kind == second.Kind &&
		ReferenceEquals(first.ExactTarget, second.ExactTarget) && ReferenceEquals(first.ExactParcel, second.ExactParcel);

	private Func<bool> CaptureMissionInputFence(ICharacter caster)
	{
		var input = _boundMissionInputs;
		if (input is null) return () => _boundMissionInputs is null;
		var target = input.ExactTarget;
		var body = target.Body; var bodyId = body?.Id;
		var identity = CharacterInstanceIdentityComparer.IdentityId(target); var instanceId = target.InstanceId;
		var state = target.State; var location = target.Location; var layer = target.RoomLayer; var position = target.RoutePositionMetres;
		var parcel = input.ExactParcel; var parcelId = parcel?.Id; var quantity = parcel?.Quantity;
		return () => SameMissionInputs(input, _boundMissionInputs) &&
			ReferenceEquals(target.Gameworld, Gameworld) && Gameworld.Actors.Any(x => ReferenceEquals(x, target)) &&
			ReferenceEquals(target.Body, body) && body?.Id == bodyId && target.InstanceId == instanceId &&
			CharacterInstanceIdentityComparer.IdentityId(target) == identity && target.State == state &&
			ReferenceEquals(target.Location, location) && target.RoomLayer == layer && target.RoutePositionMetres == position &&
			(parcel is null || parcel.Id == parcelId && parcel.Quantity == quantity && !parcel.Deleted && !parcel.Destroyed &&
			 ReferenceEquals(parcel.Gameworld, Gameworld) && ReferenceEquals(Gameworld.Items.Get(parcel.Id), parcel) &&
			 caster.Body.HeldOrWieldedItems.Any(x => ReferenceEquals(x, parcel)));
	}

	private void ApplyBoundMission(ICharacter creature, ICharacter caster, SpellNpcMissionInputs? inputs, Func<bool> inputsCurrent)
	{
		if (inputs is null) return;
		if (inputs.Kind != MissionKind || !SameMissionInputs(inputs, _boundMissionInputs))
			throw new InvalidOperationException("The prepared NPC mission changed during native creation; do not replay.");
		var controllers = creature.EffectsOfType<SpellNpcControlEffect>().ToArray();
		var assigned = controllers.Length == 1 && controllers[0].AssignPreparedMission(caster, inputs, inputsCurrent);
		if (!assigned) throw new InvalidOperationException("Native NPC mission assignment did not durably confirm; do not replay the paid creation.");
	}

	private bool BuildingCommandMission(ICharacter actor, StringStack command)
	{
		if (!Enum.TryParse<SpellNpcMissionKind>(command.SafeRemainingArgument, true, out var kind) || !Enum.IsDefined(kind))
		{
			actor.OutputHandler.Send("Specify mission none, seek, courier or haunt.".ColourCommand());
			return false;
		}
		var oldKind = MissionKind; var oldError = _missionLoadError;
		MissionKind = kind; _missionLoadError = null;
		if (MissionPolicyDefinitionError() is { } error)
		{
			MissionKind = oldKind; _missionLoadError = oldError;
			actor.OutputHandler.Send(error.ColourError());
			return false;
		}
		_unreadableMissions = null; Spell.Changed = true;
		actor.OutputHandler.Send($"The native summon mission is now {kind.DescribeEnum().ColourName()}.");
		return true;
	}
}
