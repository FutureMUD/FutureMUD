using MudSharp.Combat.Moves;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Combat.ScatterStrategies;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.RPG.Law;
using MoreLinq;

namespace MudSharp.GameItems.Components;

public class AmmunitionGameItemComponent : GameItemComponent, IAmmo
{
    private RangedFireContext _currentFireContext;
    private ProjectileCustodyCompletion _projectileCompletion;
	internal bool IsQuiescentForStackMerge => _currentFireContext is null && _projectileCompletion is null;
    protected AmmunitionGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new AmmunitionGameItemComponent(this, newParent, temporary);
    }

    protected override string SaveToXml()
    {
        return "<Definition/>";
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (AmmunitionGameItemComponentProto)newProto;
    }

    #region Constructors

    public AmmunitionGameItemComponent(AmmunitionGameItemComponentProto proto, IGameItem parent,
        bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public AmmunitionGameItemComponent(MudSharp.Models.GameItemComponent component,
        AmmunitionGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
    }

    public AmmunitionGameItemComponent(AmmunitionGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    #endregion

    #region IAmmo Implementation

    public IAmmunitionType AmmoType => _prototype.AmmoType;

    public IGameItem GetFiredItem
    {
        get
        {
            if (_prototype.BulletProto == null)
            {
                return null;
            }

            return new GameItem(_prototype.BulletProto, quality: Parent.Quality);
        }
    }

    public IGameItem GetFiredWasteItem
    {
        get
        {
            if (_prototype.CasingProto == null)
            {
                return null;
            }

            return new GameItem(_prototype.CasingProto, quality: Parent.Quality);
        }
    }

    private void HandleAmmunitionAftermath(ICharacter actor, IPerceiver target, IGameItem ammo, bool hit = false,
            IEmoteOutput emoteOnBreak = null, IEmoteOutput emoteOnFallToGround = null)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);
        if (target == null || _projectileCompletion is not { } completion ||
            !ReferenceEquals(completion.Projectile, ammo)) return;
        var impact = completion.CaptureImpact(target);
        ResolveAftermath(actor, target, impact, ammo,
            hit ? AmmoType.BreakChanceOnHit : AmmoType.BreakChanceOnMiss,
            emoteOnBreak, emoteOnFallToGround,
            output => target.OutputHandler.Handle(output));
    }

    private void HandleAmmunitionScatterToCell(ICharacter actor, SpatialLocation impactLocation, IGameItem ammo,
        IEmoteOutput emoteOnBreak = null, IEmoteOutput emoteOnFallToGround = null)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);
        ResolveAftermath(actor, null, impactLocation, ammo, AmmoType.BreakChanceOnMiss,
            emoteOnBreak, emoteOnFallToGround, output => impactLocation.Cell.Handle(output));
    }

    private void ResolveAftermath(ICharacter actor, IPerceiver positionTarget, SpatialLocation impact,
        IGameItem ammo, double breakChance, IEmoteOutput emoteOnBreak, IEmoteOutput emoteOnFallToGround,
        Action<IEmoteOutput> emit)
    {
        var completion = _projectileCompletion;
        if (completion == null || !ReferenceEquals(completion.Projectile, ammo) || !completion.IsUnclaimed) return;

        // The detached participant may finish placement after refusal. Breakage/effects/posture remain
        // separate operations under the original principal. No independent scope is installed here.
        var breakProjectile = MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) &&
                              RandomUtilities.Roll(1.0, breakChance);
        completion.PlaceAt(impact, allowMerge: !breakProjectile);
        if (!completion.IsAt(impact) || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return;

        if (breakProjectile)
        {
            if (emoteOnBreak != null) emit(emoteOnBreak);
            if (!completion.IsAt(impact) || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return;
            var result = ammo.Die();
            if (result == null || result.Deleted || result.Destroyed || ReferenceEquals(result, ammo)) return;

            // Native Die normally places its replacement already. Never rehome a callback-claimed result.
            var replacement = new ProjectileCustodyCompletion(actor, result, null, impact);
            if (replacement.IsUnclaimed) replacement.PlaceAt(impact, allowMerge: true);
            if (replacement.IsAt(impact)) AddAftermathState(actor, positionTarget, replacement, impact);
            return;
        }

        if (emoteOnFallToGround != null) emit(emoteOnFallToGround);
        AddAftermathState(actor, positionTarget, completion, impact);
    }

    private static void AddAftermathState(ICharacter actor, IPerceiver target,
        ProjectileCustodyCompletion completion, SpatialLocation impact)
    {
        if (!completion.IsAt(impact) || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return;
        if (actor.Combat != null)
        {
            completion.Projectile.AddEffect(new CombatNoGetEffect(completion.Projectile, actor.Combat),
                TimeSpan.FromSeconds(20));
        }
        if (!completion.IsAt(impact) || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return;
        completion.Projectile.PositionTarget =
            target != null && ProjectileCustodyCompletion.SourceIsAt(target, impact) ? target : null;
    }

    private void BroadcastProjectileFlight(ICharacter actor, IPerceivable destination, IGameItem ammo,
        IReadOnlyList<ICellExit> precomputedPath = null)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);

        if (actor?.Location == null || destination?.Location == null)
        {
            return;
        }

        IReadOnlyList<ICellExit> path = precomputedPath ?? actor.PathBetween(destination, 10, false, false, true)?.ToList() ??
                   new List<ICellExit>();
        List<CardinalDirection> directions = path.Select(x => x.OutboundDirection).ToList();
        string dirDesc = directions.DescribeDirection();
        string oppDirDesc = directions.DescribeOppositeDirection();
        string actionDescription = "fly|flies overhead";
        string actionDescriptionTargetRoom = "fly|flies in";
        OutputFlags flags = OutputFlags.InnerWrap;
        switch (AmmoType.EchoType)
        {
            case AmmunitionEchoType.Arcing:
                flags |= OutputFlags.NoticeCheckRequired;
                break;
            case AmmunitionEchoType.Laser:
                actionDescription = "flash|flashes through the area";
                actionDescriptionTargetRoom = "flash|flashes in";
                break;
            case AmmunitionEchoType.Subsonic:
                actionDescription = "fly|flies through the area";
                break;
            case AmmunitionEchoType.Supersonic:
                actionDescription = "whizz|whizzes past";
                actionDescriptionTargetRoom = "whizz|whizzes in";
                flags |= OutputFlags.PurelyAudible;
                break;
        }

        foreach (ICell cell in actor.CellsUnderneathFlight(destination, 10).ToArray())
        {
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
                _projectileCompletion?.IsUnclaimed != true) return;
            cell.Handle(
                new EmoteOutput(
                    new Emote($"@ {actionDescription} from the {oppDirDesc} towards the {dirDesc}", ammo),
                    style: OutputStyle.CombatMessage, flags: flags)
                { NoticeCheckDifficulty = Difficulty.VeryHard });
        }

        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
            _projectileCompletion?.IsUnclaimed != true) return;
        if (destination is not IPerceiver destinationPerceiver)
        {
            return;
        }

        if (!Equals(actor.Location, destinationPerceiver.Location))
        {
            destinationPerceiver.OutputHandler.Handle(new EmoteOutput(
                new Emote($"$0 {actionDescriptionTargetRoom} from the {oppDirDesc}.", destinationPerceiver, ammo),
                style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
            return;
        }

        if (actor.RoomLayer == destinationPerceiver.RoomLayer)
        {
            return;
        }

        string relativeDirection = destinationPerceiver.RoomLayer.IsHigherThan(actor.RoomLayer) ? "below" : "above";
        destinationPerceiver.OutputHandler.Handle(new EmoteOutput(
            new Emote($"$0 {actionDescriptionTargetRoom} from {relativeDirection}.", destinationPerceiver, ammo),
            style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
    }

    private bool ShouldAttemptScatter(Outcome shotOutcome, ICharacter actor, string context)
    {
        int baseChance = Math.Max(0, 10 * (8 - (int)shotOutcome));
        double multiplier = actor.Merits.OfType<IScatterChanceMerit>()
                             .Aggregate(1.0, (current, merit) => current * merit.ScatterMultiplier);
        double finalChance = baseChance * multiplier;
        if (finalChance <= 0)
        {
            Gameworld.DebugMessage(
                $"[Scatter:{context}] No scatter possible for {actor.HowSeen(actor)} - final chance {finalChance:F2}% (base {baseChance}% * mult {multiplier:F2}).");
            return false;
        }

        int roll = Dice.Roll(1, 100);
        bool success = roll <= finalChance;
        Gameworld.DebugMessage(
            $"[Scatter:{context}] {actor.HowSeen(actor)} rolled {roll:N0} vs {finalChance:F2}% (base {baseChance}% * mult {multiplier:F2}) -> {(success ? "scatter triggered" : "no scatter")}.");
        return success;
    }

    private bool TryResolveScatter(ICharacter actor, IPerceiver originalTarget, IRangedWeaponType weaponType,
        IGameItem ammo, IReadOnlyList<ICellExit> path, string context, RangedScatterType? scatterType = null)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);

        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
            _projectileCompletion?.IsUnclaimed != true) { FinishRefusedProjectile(actor, originalTarget, ammo); return true; }
        scatterType ??= _currentFireContext?.ScatterType;
        RangedScatterResult scatterResult = (scatterType is null
                ? RangedScatterStrategyFactory.GetStrategy(weaponType)
                : RangedScatterStrategyFactory.GetStrategy(scatterType.Value))
            .GetScatterTarget(actor, originalTarget, path ?? Array.Empty<ICellExit>());

        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
            _projectileCompletion?.IsUnclaimed != true) { FinishRefusedProjectile(actor, originalTarget, ammo); return true; }
        if (scatterResult == null)
        {
            Gameworld.DebugMessage(
                $"[Scatter:{context}] {actor.HowSeen(actor)} had no valid ricochet destinations from {originalTarget?.HowSeen(actor) ?? "unknown target"}.");
            return false;
        }

        ResolveScatterResult(actor, weaponType, ammo, scatterResult, context);
        return true;
    }

    private void ResolveScatterResult(ICharacter actor, IRangedWeaponType weaponType, IGameItem ammo,
        RangedScatterResult scatterResult, string context)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);

        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
            _projectileCompletion?.IsUnclaimed != true) return;
        if (scatterResult.Target != null)
        {
            _projectileCompletion.CaptureImpact(scatterResult.Target);
            if (scatterResult.Target is ICharacter collateral)
            {
                var lawful = actor.IsLawfulEnforcementActionAgainst(collateral, CrimeTypes.AssaultWithADeadlyWeapon);
                if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
                    _projectileCompletion?.IsUnclaimed != true) return;
                if (!lawful)
                    CrimeExtensions.CheckPossibleCrimeAllAuthorities(actor, CrimeTypes.AssaultWithADeadlyWeapon,
                        collateral, null, "collateral fire");
            }
            else if (scatterResult.Target is IGameItem collateralItem)
            {
                CrimeExtensions.CheckPossibleCrimeAllAuthorities(actor, CrimeTypes.Vandalism, null, collateralItem,
                    "collateral fire");
            }

            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
                _projectileCompletion?.IsUnclaimed != true) return;
            List<ICellExit> scatterPath = actor.PathBetween(scatterResult.Target, 10, false, false, true)?.ToList() ??
                              new List<ICellExit>();
            BroadcastProjectileFlight(actor, scatterResult.Target, ammo, scatterPath);
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
                _projectileCompletion?.IsUnclaimed != true) return;
            IBodypart bodypart = (scatterResult.Target as IHaveABody)?.Body?.RandomBodyPartGeometry(Orientation.Centre,
                Alignment.Front, Facing.Front);
            Hit(actor, scatterResult.Target, Outcome.Pass, Outcome.Pass,
                new OpposedOutcome(OpposedOutcomeDirection.Proponent, OpposedOutcomeDegree.Marginal), bodypart, ammo,
                weaponType, null);
            Gameworld.DebugMessage(
                $"[Scatter:{context}] Ricochet struck {scatterResult.Target.HowSeen(actor)} in {scatterResult.Cell.HowSeen(actor)} after deviating{ScatterStrategyUtilities.DescribeFromDirection(scatterResult.DirectionFromTarget)} (distance {scatterResult.DistanceFromTarget:N0}).");
            return;
        }

        DummyPerceiver dummy = new(location: scatterResult.Cell)
        {
            RoomLayer = scatterResult.RoomLayer
        };
        List<ICellExit> scatterCellPath = actor.PathBetween(dummy, 10, false, false, true)?.ToList() ?? new List<ICellExit>();
        BroadcastProjectileFlight(actor, dummy, ammo, scatterCellPath);
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
            _projectileCompletion?.IsUnclaimed != true) return;
        string directionText = ScatterStrategyUtilities.DescribeFromDirection(scatterResult.DirectionFromTarget);
        EmoteOutput breakOutput = new(
            new Emote($"$0 ricochets{directionText} and shatters!", dummy, ammo),
            style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap);
        EmoteOutput fallOutput = new(
            new Emote($"$0 ricochets{directionText} and falls to the ground.", dummy, ammo),
            style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap);
		HandleAmmunitionScatterToCell(actor, scatterResult.ImpactLocation, ammo, breakOutput,
            fallOutput);
        Gameworld.DebugMessage(
            $"[Scatter:{context}] Ricochet landed in {scatterResult.Cell.HowSeen(actor)}{directionText} without hitting a new target.");
    }

    private bool TryResolveCoverInterception(ICharacter actor, IPerceiver target, Outcome shotOutcome,
        Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IGameItem ammo,
        IRangedWeaponType weaponType, IReadOnlyList<ICellExit> path)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);

		var effectiveCover = target is ICharacter targetCharacter
			? VehicleCombatService.Instance.ResolveEffectiveRangedCover(actor, targetCharacter)
			: null;
		var cover = effectiveCover?.Cover ?? target?.Cover?.Cover;
		var coverItem = effectiveCover?.Provider ?? target?.Cover?.CoverItem?.Parent;
		if (!shotOutcome.IsPass() || coverOutcome.IsPass() || cover is null)
        {
            return false;
        }

		bool strikeCover = cover.CoverType == CoverType.Hard || shotOutcome == Outcome.MajorPass ||
                          coverOutcome == Outcome.MinorFail;
        if (!strikeCover)
        {
            return false;
        }

        BroadcastProjectileFlight(actor, target, ammo, path);
        string actorText = actor.HowSeen(actor);
        string targetText = target.HowSeen(actor);
        string coverText = coverItem?.HowSeen(actor) ?? "environmental cover";
        Gameworld.DebugMessage(
            $"[Ranged] Cover interception: {actorText} vs {targetText} (shot {shotOutcome}, cover {coverOutcome}) stopped by {coverText}.");
        target.OutputHandler.Handle(
            new EmoteOutput(
                new Emote($"The {ammo.Name.ToLowerInvariant()} strikes $?1|$1, ||$$0's cover!", target, target,
                    coverItem), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
        actor.Send("You hit your target's cover instead.".Colour(Telnet.Yellow));

        Damage damage = BuildDamage(actor, target, bodypart, ammo, weaponType, defenseOutcome);
        if (damage is null || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) { FinishRefusedProjectile(actor, target, ammo); return true; }
        List<IWound> wounds = new();
        if (_projectileCompletion?.IsUnclaimed != true) return true;
        _projectileCompletion.SetFallback(_projectileCompletion.CaptureImpact(target));
		if (effectiveCover?.IsVehicleCover != true)
		{
			wounds.AddRange(coverItem?.CommandSufferDamage(damage) ?? Enumerable.Empty<IWound>());
		}
        _projectileCompletion?.PreserveWoundClaim(wounds);
        wounds.ProcessPassiveWounds();

        if (wounds.Any(x => x.Lodged == ammo))
        {
            return true;
        }

        string scatterContext = $"cover {actorText}->{targetText}";
        if (ShouldAttemptScatter(shotOutcome, actor, scatterContext) &&
            TryResolveScatter(actor, target, weaponType, ammo, path, scatterContext))
        {
            return true;
        }

        HandleAmmunitionAftermath(actor, target, ammo);
        Gameworld.DebugMessage(
            $"[Ranged] Cover fully absorbed the shot from {actorText} to {targetText}; ammunition resolved at the target.");
        return true;
    }

    private bool TryResolveMiss(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
        OpposedOutcome defenseOutcome, IGameItem ammo, IRangedWeaponType weaponType, IEmoteOutput defenseEmote,
        IReadOnlyList<ICellExit> path)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);

        if (shotOutcome.IsPass() && defenseOutcome.Outcome != OpposedOutcomeDirection.Opponent)
        {
            return false;
        }

        BroadcastProjectileFlight(actor, target, ammo, path);

        if (defenseEmote != null)
        {
            target.OutputHandler.Handle(defenseEmote);
        }

        target.OutputHandler.Handle(
            new EmoteOutput(
                new Emote($"$0 {(shotOutcome.IsPass() ? "narrowly misses @!" : "misses @ by a wide margin.")}", target,
                    ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

        if (!actor.ColocatedWith(target))
        {
            actor.Send("You missed your target.".Colour(Telnet.Red));
        }

        string actorText = actor.HowSeen(actor);
        string targetText = target.HowSeen(actor);
        string missReason = shotOutcome.IsPass()
            ? $"defended ({defenseOutcome.Outcome}:{defenseOutcome.Degree})"
            : "wild shot";
        Gameworld.DebugMessage($"[Ranged] Miss: {actorText} vs {targetText} - {missReason} (shot {shotOutcome}, cover {coverOutcome}).");

        if (!shotOutcome.IsPass() && !coverOutcome.IsPass())
        {
            string scatterContext = $"miss {actorText}->{targetText}";
            if (ShouldAttemptScatter(shotOutcome, actor, scatterContext) &&
                TryResolveScatter(actor, target, weaponType, ammo, path, scatterContext))
            {
                return true;
            }
        }

        HandleAmmunitionAftermath(actor, target, ammo);
        Gameworld.DebugMessage($"[Ranged] Miss resolved with ammunition falling near {targetText}.");
        return true;
    }

    private bool TryResolveObstruction(ICharacter actor, IPerceiver target, IGameItem ammo,
        IRangedWeaponType weaponType, OpposedOutcome defenseOutcome, IBodypart bodypart, IReadOnlyList<ICellExit> path)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);

        var obstructions = new List<IRangedObstructionEffect>();
        foreach (var candidate in target.EffectsOfType<IRangedObstructionEffect>().ToArray())
        {
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) { FinishRefusedProjectile(actor, target, ammo); return true; }
            var applies = candidate.Applies(actor);
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) { FinishRefusedProjectile(actor, target, ammo); return true; }
            if (applies && target.EffectsOfType<IRangedObstructionEffect>().Contains(candidate)) obstructions.Add(candidate);
        }
        var obstructionEffect = obstructions.Shuffle(Constants.Random).FirstOrDefault();
        if (obstructionEffect == null)
        {
            return false;
        }

        IPerceivable obstruction = obstructionEffect.Obstruction;
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
            _projectileCompletion?.IsUnclaimed != true) { FinishRefusedProjectile(actor, target, ammo); return true; }
        if (obstruction is IPerceiver capturedObstruction) _projectileCompletion.CaptureImpact(capturedObstruction);
        IPerceiver obstructionPerceiver = obstruction as IPerceiver;
        string actorText = actor.HowSeen(actor);
        string targetText = target.HowSeen(actor);
        string obstructionText = obstruction.HowSeen(actor);
        Gameworld.DebugMessage(
            $"[Ranged] Obstruction: {actorText}'s shot at {targetText} intercepted by {obstructionText}.");
        if (obstructionPerceiver != null)
        {
            List<ICellExit> obstructionPath = actor.PathBetween(obstructionPerceiver, 10, false, false, true)?.ToList() ??
                                  new List<ICellExit>();
            BroadcastProjectileFlight(actor, obstructionPerceiver, ammo, obstructionPath);
        }
        else
        {
            BroadcastProjectileFlight(actor, target, ammo, path);
        }

        target.OutputHandler.Handle(
            new EmoteOutput(
                new Emote($"The {ammo.Name.ToLowerInvariant()} strikes $1 instead of $0!", target, target, obstruction),
                style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
        actor.Send($"You hit {obstruction.HowSeen(actor)} instead!");

        Damage damage = BuildDamage(actor, target, bodypart, ammo, weaponType, defenseOutcome);
        if (damage is null || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) { FinishRefusedProjectile(actor, target, ammo); return true; }
        List<IWound> wounds = new();

        if (obstruction is IHaveWounds ihw)
        {
            if (obstruction is IHaveABody ihab)
            {
                IBodypart oldTarget = damage.Bodypart;
                damage = new Damage(damage)
                {
                    Bodypart = ihab.Body.RandomBodyPartGeometry(oldTarget?.Orientation ?? Orientation.Centre,
                        Alignment.Front, Facing.Front, true)
                };
            }

            _projectileCompletion?.SetFallback(_projectileCompletion.CaptureImpact(obstructionPerceiver ?? target));
            wounds.AddRange(ihw.CommandSufferDamage(damage) ?? Enumerable.Empty<IWound>());
            _projectileCompletion?.PreserveWoundClaim(wounds);
            if (obstruction is IHaveABody && wounds.Any())
            {
                if (MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) WeaponPoisonDeliveryHelper.DeliverFromWeapon(actor, ammo, wounds, false);
            }
            wounds.ProcessPassiveWounds();
        }

        if (wounds.All(x => x.Lodged != ammo))
        {
            if (obstructionPerceiver != null)
            {
                HandleAmmunitionAftermath(actor, obstructionPerceiver, ammo);
            }
            else
            {
                HandleAmmunitionAftermath(actor, target, ammo);
            }
            Gameworld.DebugMessage(
                $"[Ranged] Obstruction aftermath: ammunition resolved at {(obstructionPerceiver != null ? obstructionText : targetText)}.");
        }

        return true;
    }

	private Damage BuildDamage(ICharacter actor, IPerceiver target, IBodypart bodypart,
        IGameItem ammo, IRangedWeaponType weaponType, OpposedOutcome defenseOutcome,
        double damageMultiplier = 1.0)
    {
        if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return null;
        var payloadEffects = new List<IMagicProjectilePayloadEffect>();
        foreach (var effect in ammo.EffectsOfType<IMagicProjectilePayloadEffect>().ToArray())
        {
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return null;
            var applies = effect.AppliesToProjectileAttack(actor, target, ammo);
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) return null;
            if (applies && ammo.EffectsOfType<IMagicProjectilePayloadEffect>().Contains(effect)) payloadEffects.Add(effect);
        }
        var effectiveQuality = (int)ammo.Quality + (int)Math.Round(payloadEffects.Sum(x => x.ProjectileQualityBonus));
        var profileValues = new (string Name, object Value)[]
        {
            ("quality", effectiveQuality),
            ("degree", (int)defenseOutcome.Degree),
            ("pointblank", actor == target ? 1 : 0),
            ("inmelee", actor.MeleeRange ? 1 : 0),
            ("range", target.DistanceBetween(actor, 10))
        };
        var weaponBonusValues = new (string Name, object Value)[]
        {
            ("range", target.DistanceBetween(actor, 10)),
            ("quality", (int)Parent.Quality),
            ("degree", (int)defenseOutcome.Degree),
            ("pointblank", actor == target ? 1 : 0),
            ("inmelee", actor.MeleeRange ? 1 : 0)
        };

        damageMultiplier *= _currentFireContext?.DamageMultiplier ?? 1.0;
        double finalDamage = (AmmoType.DamageProfile.DamageExpression.EvaluateWith(actor, values: profileValues) +
                          weaponType.DamageBonusExpression.EvaluateWith(actor, weaponType.FireTrait,
                              values: weaponBonusValues) +
                          payloadEffects.Sum(x => x.ProjectileDamageBonus)) * damageMultiplier;
        double finalPain = (AmmoType.DamageProfile.PainExpression.EvaluateWith(actor, values: profileValues) +
                           payloadEffects.Sum(x => x.ProjectilePainBonus)) * damageMultiplier;
        double finalStun = (AmmoType.DamageProfile.StunExpression.EvaluateWith(actor, values: profileValues) +
                           payloadEffects.Sum(x => x.ProjectileStunBonus)) * damageMultiplier;
        return new Damage
        {
            ActorOrigin = actor,
            ToolOrigin = Parent,
            Bodypart = bodypart,
            DamageAmount = finalDamage,
            DamageType = AmmoType.DamageProfile.DamageType,
            PainAmount = finalPain,
            StunAmount = finalStun,
            LodgableItem = ammo
        };
    }

    private void FinishRefusedProjectile(ICharacter actor, IPerceiver target, IGameItem ammo)
    {
        if (_projectileCompletion is { } completion && ReferenceEquals(completion.Projectile, ammo))
            completion.Finish();
    }

    protected virtual void Hit(ICharacter actor, IPerceiver target, Outcome shotOutcome,
        Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IGameItem ammo,
        IRangedWeaponType weaponType, IEmoteOutput defenseEmote)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);

        Damage damage = BuildDamage(actor, target, bodypart, ammo, weaponType, defenseOutcome);
        if (damage is null || !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) { FinishRefusedProjectile(actor, target, ammo); return; }
        List<IWound> wounds = new();
        string actorText = actor.HowSeen(actor);
        string targetText = target.HowSeen(actor);
        string locationText = target.Location?.HowSeen(actor) ?? "unknown location";
        Gameworld.DebugMessage(
            $"[Ranged] Hit resolved: {actorText} struck {targetText} at {locationText} (shot {shotOutcome}, cover {coverOutcome}, defense {defenseOutcome.Outcome}:{defenseOutcome.Degree}).");

        if (defenseEmote != null)
        {
            target.OutputHandler.Handle(defenseEmote);
        }

        if (!target.ColocatedWith(actor))
        {
            actor.Send("You hit your target.".Colour(Telnet.BoldGreen));
        }

        IHaveABody targetHB = target as IHaveABody;
        if (targetHB?.Body != null)
        {
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
                _projectileCompletion?.IsUnclaimed != true) { FinishRefusedProjectile(actor, target, ammo); return; }
            _projectileCompletion.SetFallback(_projectileCompletion.CaptureImpact(target));
            wounds.AddRange(targetHB.Body.CommandSufferDamage(damage));
            _projectileCompletion.PreserveWoundClaim(wounds);
            if (!wounds.Any())
            {
                HandleAmmunitionAftermath(actor, target, ammo, true,
                        new EmoteOutput(new Emote($"$1 hits $0 on &0's {bodypart.FullDescription()} and breaks!",
                                target, target, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap),
                        new EmoteOutput(new Emote($"$1 hits $0 on &0's {bodypart.FullDescription()} but ricochets off without causing any damage!",
                                target, target, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap)
                );
                return;
            }

            if (MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor)) WeaponPoisonDeliveryHelper.DeliverFromWeapon(actor, ammo, wounds, false);

            if (wounds.Any(x => x.Lodged == ammo))
            {
                IWound lodgedWound = wounds.First(x => x.Lodged == ammo);
                if (lodgedWound.Parent == target)
                {
                    target.OutputHandler.Handle(
                            new EmoteOutput(new Emote($"$0 lodges in $1's {lodgedWound.Bodypart.FullDescription()}!",
                                    target, ammo, target)));
                }
                else
                {
                    target.OutputHandler.Handle(
                            new EmoteOutput(new Emote($"$0 lodges in $1's !2!", target, ammo, target,
                                    lodgedWound.Parent)));
                }

                wounds.ProcessPassiveWounds();
                return;
            }

            HandleAmmunitionAftermath(actor, target, ammo, true,
                    new EmoteOutput(new Emote($"$0 strikes $1's {bodypart.FullDescription()}, and then breaks!",
                            target, ammo, target)),
                    new EmoteOutput(new Emote($"$0 strikes $1's {bodypart.FullDescription()}, but falls to the ground!",
                            target, ammo, target)));
            wounds.ProcessPassiveWounds();
            return;
        }

        if (target is IGameItem targetItem)
        {
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) ||
                _projectileCompletion?.IsUnclaimed != true) { FinishRefusedProjectile(actor, target, ammo); return; }
            _projectileCompletion.SetFallback(_projectileCompletion.CaptureImpact(target));
            wounds.AddRange(targetItem.CommandSufferDamage(damage));
            _projectileCompletion.PreserveWoundClaim(wounds);
            if (!wounds.Any())
            {
                HandleAmmunitionAftermath(actor, target, ammo, true,
                        new EmoteOutput(new Emote(
                                "$1 hit|hits $0 but ricochets off without causing any damage and shatters!",
                                targetItem, targetItem, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap),
                        new EmoteOutput(new Emote("$1 hit|hits $0 but ricochets off without causing any damage!",
                                targetItem, targetItem, ammo), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
                return;
            }

            if (wounds.Any(x => x.Lodged == ammo))
            {
                target.OutputHandler.Handle(
                        new EmoteOutput(new Emote($"$0 lodges in $1!", actor, ammo, targetItem)));
                wounds.ProcessPassiveWounds();
                return;
            }

            HandleAmmunitionAftermath(actor, target, ammo, true,
                    new EmoteOutput(new Emote($"$0 strikes $1, and then breaks!",
                            target, ammo, target)),
                    new EmoteOutput(new Emote($"$0 strikes $1, but falls to the ground!",
                            target, ammo, target)));
            wounds.ProcessPassiveWounds();
            return;
        }

        throw new NotImplementedException("Unknown target type in Fire.");
    }

    public virtual void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
        OpposedOutcome defenseOutcome, IBodypart bodypart, IGameItem ammo, IRangedWeaponType weaponType,
        IEmoteOutput defenseEmote, RangedFireContext context = null)
    {
        using var orderedComponentExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(actor);
        var completion = new ProjectileCustodyCompletion(actor, ammo, target);
        FirePrepared(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo, weaponType,
            defenseEmote, context, completion);
    }

    private protected void FirePrepared(ICharacter actor, IPerceiver target, Outcome shotOutcome,
        Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IGameItem ammo,
        IRangedWeaponType weaponType, IEmoteOutput defenseEmote, RangedFireContext context,
        ProjectileCustodyCompletion completion)
    {
        var previousCompletion = _projectileCompletion;
        var previousContext = _currentFireContext;
        _projectileCompletion = completion;
        try
        {
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) || !completion.IsUnclaimed) { FinishRefusedProjectile(actor, target, ammo); return; }

            _currentFireContext = context ?? new RangedFireContext();
            // Ammunition that is just created and lodges can cause a crash if we don't flush now
            if (_currentFireContext.ProjectileIndex == 0)
            {
                Gameworld.SaveManager.Flush();
                if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) || !completion.IsUnclaimed) { FinishRefusedProjectile(actor, target, ammo); return; }
            }

            if (target == null)
            {
                // Fired at sky
                if (actor.Location.CurrentOverlay.OutdoorsType != CellOutdoorsType.Outdoors)
                {
                    HandleAmmunitionAftermath(actor, actor, ammo, true,
                        new EmoteOutput(new Emote("$1 $1|hit|hits the ceiling and shatters!", actor, actor, ammo)),
                        new EmoteOutput(
                            new Emote("$1 $1|hit|hits the ceiling and drops to the ground!", actor, actor, ammo))
                    );
                }

                return;
            }

            List<ICellExit> pathToTarget = actor.PathBetween(target, 10, false, false, true)?.ToList() ?? new List<ICellExit>();

            // Cover checks run first – they may absorb the shot or trigger a ricochet.
            if (TryResolveCoverInterception(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo,
                    weaponType, pathToTarget))
            {
                return;
            }

            // Handle outright misses (bad shot or a successful defense), including scatter fall-through.
            if (TryResolveMiss(actor, target, shotOutcome, coverOutcome, defenseOutcome, ammo, weaponType, defenseEmote,
                    pathToTarget))
            {
                return;
            }

            // Interposing effects redirect successful shots before they land on the original target.
            if (TryResolveObstruction(actor, target, ammo, weaponType, defenseOutcome, bodypart, pathToTarget))
            {
                return;
            }

            BroadcastProjectileFlight(actor, target, ammo, pathToTarget);
            if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue(actor) || !completion.IsUnclaimed) { FinishRefusedProjectile(actor, target, ammo); return; }
            Hit(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo, weaponType, defenseEmote);
        }
        finally
        {
            try { completion.Finish(); }
            finally
            {
                _projectileCompletion = previousCompletion;
                _currentFireContext = previousContext;
            }
        }
    }

    #endregion
}


/// <summary>
/// One exact, already-detached projectile may finish custody without granting authority to
/// another operation or to callbacks. Wound claims and callback relocation/deletion win.
/// </summary>
internal sealed class ProjectileCustodyCompletion
{
    private readonly bool _ordered;
    private readonly Dictionary<IPerceiver, SpatialLocation> _impacts =
        new(ReferenceEqualityComparer.Instance);
    private SpatialLocation _fallback;
    private bool _used;
    private bool _armed;

    internal ProjectileCustodyCompletion(ICharacter actor, IGameItem projectile, IPerceiver target,
        SpatialLocation? launch = null, bool awaitDetach = false)
    {
        _ordered = MudSharp.NPC.AI.CommandExecutionScope.IsOrdered(actor);
        Projectile = projectile;
        _armed = !awaitDetach && ComponentItemTransfer.IsDetached(projectile);
        _fallback = launch ?? RouteSpatialService.Instance.GetEffectiveLocation(actor);
        if (target != null) CaptureImpact(target);
        if (launch is null) CaptureImpact(actor);
    }

    internal IGameItem Projectile { get; }
    internal bool IsUnclaimed => _armed && !_used && ComponentItemTransfer.IsDetached(Projectile);

    internal SpatialLocation CaptureImpact(IPerceiver target)
    {
        if (_impacts.TryGetValue(target, out var point)) return point;
        point = RouteSpatialService.Instance.GetEffectiveLocation(target);
        _impacts.Add(target, point);
        return point;
    }

    internal void SetFallback(SpatialLocation point)
    {
        if (IsUnclaimed) _fallback = point;
    }

    internal void ArmAfterDetach()
    {
        if (!_used && ComponentItemTransfer.IsDetached(Projectile)) _armed = true;
    }

    internal void PreserveExternalClaim() => _used = true;
    internal void PreserveWoundClaim(IEnumerable<IWound> wounds)
    {
        if (wounds.Any(x => ReferenceEquals(x.Lodged, Projectile))) _used = true;
    }

    internal void Finish() => PlaceAt(_fallback);

    internal void PlaceAt(SpatialLocation point, bool allowMerge = false)
    {
        if (!IsUnclaimed || point.Cell == null) return;
        if (!RouteSpatialService.Instance.TryValidateLocation(point, out var error))
            throw new InvalidOperationException(error);

        // Spend before callbacks; callbacks execute under the original scope and cannot reuse this receipt.
        _used = true;
        Projectile.RoomLayer = point.Layer;
        if (!ComponentItemTransfer.IsDetached(Projectile)) return;
        if (point.Cell.RouteDefinition != null)
        {
            Projectile.MoveTo(point);
            if (!IsAt(point)) return;
        }

        // Ordered completion must never merge into a different stack after authority has expired.
        // Normal direct/foreign fire retains the ordinary merge path where requested.
        point.Cell.Insert(Projectile, _ordered || !allowMerge);
    }

    internal bool IsAt(SpatialLocation point) =>
        !Projectile.Deleted && !Projectile.Destroyed &&
        Projectile.InInventoryOf == null && Projectile.ContainedIn == null &&
        Projectile.GetItemType<MudSharp.GameItems.Interfaces.IBeltable>()?.ConnectedTo == null &&
        ReferenceEquals(ComponentItemTransfer.DirectLocationOf(Projectile), point.Cell) &&
        Projectile.RoomLayer == point.Layer &&
        (point.Cell.RouteDefinition == null || Projectile.RoutePositionMetres == point.RoutePositionMetres);

    internal static bool SourceIsAt(IPerceiver target, SpatialLocation point)
    {
        var current = RouteSpatialService.Instance.GetEffectiveLocation(target);
        return ReferenceEquals(current.Cell, point.Cell) && current.Layer == point.Layer &&
               current.RoutePositionMetres == point.RoutePositionMetres;
    }
}
