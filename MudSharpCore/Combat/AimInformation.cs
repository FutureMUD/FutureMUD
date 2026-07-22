using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy.Currency;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Form.Shape;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat;

public class AimInformation : IAimInformation
{
    public static int GetEffectiveRange(IEnumerable<ICellExit> exits)
    {
        switch (Futuremud.Games.First().GetStaticConfiguration("RangeCountingMode"))
        {
            case "exits":
                return exits.Count();
            case "axial":
                return exits.MaximumAxialDistance();
            case "pythagoreantruncate":
                return exits.PythagoreanDistance();
            case "pythagoreanround":
                return exits.PythagoreanDistance(RoundingMode.Round);
            default:
                goto case "exits";
        }
    }

    public IPerceiver Shooter { get; set; }
    public IPerceiver Target { get; set; }
    private double _aimPercentage;

    public double AimPercentage
    {
        get => _aimPercentage;
        set => _aimPercentage = Math.Max(0.0, Math.Min(value, 1.0));
    }

    public IEnumerable<ICellExit> Path { get; set; }
    public IRangedWeapon Weapon { get; set; }

    public event EventHandler AimInvalidated;

    public AimInformation(IPerceiver target, IPerceiver shooter, IEnumerable<ICellExit> path, IRangedWeapon weapon)
    {
        Shooter = shooter;
        Target = target;
        Path = path;
        Weapon = weapon;
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        if (Target != null)
        {
            Target.OnQuit += Target_NoLongerValid;
            Target.OnDeleted += Target_NoLongerValid;
            Target.OnLocationChanged += Target_OnLocationChanged;
			Target.OnSpatialPositionChanged += Target_OnSpatialPositionChanged;
            if (Target is IMortalPerceiver mp)
            {
                mp.OnDeath += Target_NoLongerValid;
            }

            if (Target is IGameItem gi)
            {
                gi.OnRemovedFromLocation += Target_NoLongerValid;
            }
        }

        Weapon.Parent.OnDeath += WeaponDestroyed;
        Weapon.Parent.OnDeleted += WeaponLost;
        Weapon.Parent.OnQuit += WeaponLost;
        if (Weapon is IRangedWeaponWithUnreadyEvent bow)
        {
            bow.OnUnready += WeaponUnreadied;
        }

        if (Shooter is ICharacter characterOwner)
        {
            characterOwner.OnMoved += CharacterOwner_OnMoved;
			characterOwner.OnSpatialPositionChanged += Shooter_OnSpatialPositionChanged;
            characterOwner.Body.OnInventoryChange += WeaponInventoryStateChange;
        }

        RegisterPathEvents();
    }

    private void WeaponUnreadied(IPerceivable owner)
    {
        Shooter.OutputHandler.Handle(
            new EmoteOutput(new Emote(
                "You stop aiming $2 at $1=0 because $2 is no longer readied.", Shooter, Shooter,
                Target, Weapon.Parent)), OutputRange.Personal);
        Shooter.OutputHandler.Handle(new EmoteOutput(
            new Emote("$0 stops aiming $2 at $1=0.", Shooter, Shooter, Target, Weapon.Parent),
            flags: OutputFlags.SuppressSource));
        ReleaseEvents();
    }

    private void CharacterOwner_OnMoved(object sender, Movement.MoveEventArgs e)
    {
        int oldDistance = Path.Count();
        RecalculatePath();

        // Can't aim at the target any more
		if (!AimRemainsValid())
        {
            Shooter.OutputHandler.Handle(
                new EmoteOutput(new Emote(
                    "You stop aiming $2 at $1=0 because you have lost sight of &1 due to movement.", Shooter, Shooter,
                    Target, Weapon.Parent)), OutputRange.Personal);
            Shooter.OutputHandler.Handle(new EmoteOutput(
                new Emote("$0 stops aiming $2 at $1=0.", Shooter, Shooter, Target, Weapon.Parent),
                flags: OutputFlags.SuppressSource));
            ReleaseEvents();
        }

        if (Shooter is IPerceivableHaveTraits iphtShooter)
        {
            Tuple<CheckOutcome, CheckOutcome> outcomes = Shooter.Gameworld.GetCheck(CheckType.KeepAimTargetMoved).MultiDifficultyCheck(iphtShooter,
				Weapon.AimDifficulty,
				Target is ICharacter targetCharacter
					? VehicleCombatService.Instance.ResolveEffectiveRangedCover(Shooter, targetCharacter)?.Cover.MinimumRangedDifficulty ?? Difficulty.Automatic
					: Target?.Cover?.Cover.MinimumRangedDifficulty ?? Difficulty.Automatic, Target,
                Weapon.WeaponType.FireTrait);
            Outcome worstOutcome = outcomes.Item1.Outcome.Worst(outcomes.Item2);
            if (worstOutcome.IsFail())
            {
                Shooter.OutputHandler.Send("You lose some of your aim as you move.");
                AimPercentage -= 0.33 * worstOutcome.FailureDegrees();
            }
        }
    }

    private void RegisterPathEvents()
    {
        foreach (ICellExit exit in Path)
        {
            if (exit.Exit.Door != null)
            {
                exit.Exit.Door.OnClose += Door_OnClose;
                exit.Exit.Door.OnRemovedFromExit += Door_OnRemovedFromExit;
                exit.Exit.Door.Parent.OnDeath += Door_Destroyed;
                exit.Exit.Door.Parent.OnDeleted += Door_Destroyed;
                exit.Exit.Door.Parent.OnQuit += Door_Destroyed;
            }
        }
    }

    private void RecalculatePath()
    {
        ReleasePathEvents();
        Path = Shooter.PathBetween(Target, 20, PathSearch.IncludeFireableDoors).ToList();
        if (Shooter.Location != Target.Location)
        {
            if (!Path.Any())
            {
                return;
            }

            int range = GetEffectiveRange(Path);
            if (range > Weapon.WeaponType.DefaultRangeInRooms)
            {
                Path = Enumerable.Empty<ICellExit>();
                return;
            }
        }
        RegisterPathEvents();
    }

	private bool AimRemainsValid()
	{
		if (Target is null || !Shooter.CanSee(Target))
		{
			return false;
		}

		if (Shooter.Location?.RouteDefinition is not null || Target.Location?.RouteDefinition is not null)
		{
			var range = Shooter.RoomEquivalentDistanceBetween(Target);
			return range >= 0.0 && range <= Weapon.WeaponType.DefaultRangeInRooms;
		}

		return Shooter.Location == Target.Location || Path.Any();
	}

	private void Target_OnSpatialPositionChanged(
		ILocateable locatable,
		SpatialLocation previousLocation,
		SpatialLocation currentLocation)
	{
		HandleSpatialPositionChange("as your target moves", 0.33);
	}

	private void Shooter_OnSpatialPositionChanged(
		ILocateable locatable,
		SpatialLocation previousLocation,
		SpatialLocation currentLocation)
	{
		HandleSpatialPositionChange("as you move", 0.33);
	}

	private void HandleSpatialPositionChange(string reason, double aimLoss)
	{
		RecalculatePath();
		if (!AimRemainsValid())
		{
			Shooter.OutputHandler.Handle(new EmoteOutput(new Emote(
				"You stop aiming $2 at $1=0 because &1 has moved out of range.",
				Shooter,
				Shooter,
				Target,
				Weapon.Parent)), OutputRange.Personal);
			ReleaseEvents();
			return;
		}

		if (AimPercentage > 0.0)
		{
			Shooter.OutputHandler.Send($"You lose some of your aim {reason}.");
			AimPercentage = Math.Max(0.0, AimPercentage - aimLoss);
		}
	}

    protected void WeaponDestroyed(IPerceivable perceivable)
    {
        Shooter.OutputHandler.Handle(
            new EmoteOutput(new Emote("@ stop|stops aiming at $1=0 because $2 has been destroyed!", Shooter, Shooter,
                Target, Weapon.Parent)));
        ReleaseEvents();
    }

    protected void WeaponLost(IPerceivable perceivable)
    {
        Shooter.OutputHandler.Handle(
            new EmoteOutput(new Emote("@ stop|stops aiming at $1=0 because #0 no longer have|has $2.", Shooter, Shooter,
                Target, Weapon.Parent)));
        ReleaseEvents();
    }

    protected void WeaponInventoryStateChange(InventoryState oldState, InventoryState newState, IGameItem item)
    {
        if (item == Weapon.Parent)
        {
            if (newState != InventoryState.Wielded)
            {
                Shooter.OutputHandler.Handle(
                    new EmoteOutput(new Emote("@ stop|stops aiming at $1=0 because #0 are|is no longer wielding $2.",
                        Shooter, Shooter, Target, Weapon.Parent)));
                ReleaseEvents();
                return;
            }

            if (!Weapon.ReadyToFire && !Weapon.WeaponType.RangedWeaponType.IsFirearm())
            {
                Shooter.OutputHandler.Handle(
                    new EmoteOutput(new Emote("@ stop|stops aiming at $1=0 because #2 $2|are|is no longer ready to fire.",
                        Shooter, Shooter, Target, Weapon.Parent)));
                ReleaseEvents();
                return;
            }
        }
    }

    private void Target_OnLocationChanged(ILocateable locatable, ICellExit exit)
    {
        if (exit != null)
        {
            Shooter.OutputHandler.Send($"Your target has moved to {exit.OutboundDirectionDescription}.");
        }

        int oldDistance = Path.Count();
        RecalculatePath();

        // Can't aim at the target any more
		if (!AimRemainsValid())
        {
            Shooter.OutputHandler.Handle(
                new EmoteOutput(new Emote(
                    $"You stop aiming $2 at $1=0 because you have lost sight of &1 due to movement.", Shooter, Shooter,
                    Target, Weapon.Parent)), OutputRange.Personal);
            Shooter.OutputHandler.Handle(new EmoteOutput(
                new Emote("$0 stops aiming $2 at $1=0.", Shooter, Shooter, Target, Weapon.Parent),
                flags: OutputFlags.SuppressSource));
            ReleaseEvents();
        }

        bool movingTowards = oldDistance > Path.Count();
        if (Shooter is IPerceivableHaveTraits iphtShooter)
        {
            Tuple<CheckOutcome, CheckOutcome> outcomes = Shooter.Gameworld.GetCheck(CheckType.KeepAimTargetMoved).MultiDifficultyCheck(iphtShooter,
                Weapon.AimDifficulty,
				Target is ICharacter targetCharacter
					? VehicleCombatService.Instance.ResolveEffectiveRangedCover(Shooter, targetCharacter)?.Cover.MinimumRangedDifficulty ?? Difficulty.Automatic
					: (Target as IPerceiver)?.Cover?.Cover.MinimumRangedDifficulty ?? Difficulty.Automatic, Target,
                Weapon.WeaponType.FireTrait);
            Outcome worstOutcome = outcomes.Item1.Outcome.Worst(outcomes.Item2);
            if (worstOutcome.IsFail())
            {
                Shooter.OutputHandler.Send("You lose some of your aim as your target moves.");
                AimPercentage -= (movingTowards ? 0.15 : 0.33) * worstOutcome.FailureDegrees();
            }
        }
    }

    private void Door_Destroyed(IPerceivable owner)
    {
        IGameItem item = owner as IGameItem;
        item.OnDeath -= Door_Destroyed;
        item.OnDeleted -= Door_Destroyed;
        item.OnQuit -= Door_Destroyed;
        IDoor door = item.GetItemType<IDoor>();
        Door_OnRemovedFromExit(door);
    }

    private void Door_OnRemovedFromExit(IDoor door)
    {
        door.OnRemovedFromExit -= Door_OnRemovedFromExit;
        door.OnClose -= Door_OnClose;
    }

    private void Door_OnClose(IOpenable openable)
    {
        IDoor door = openable as IDoor;
        if (!door.CanFireThrough)
        {
            RecalculatePath();
			if (!AimRemainsValid())
            {
                Shooter.OutputHandler.Handle(
                    new EmoteOutput(new Emote(
                        $"You stop aiming $2 at $1=0 because you have lost sight of &1 due to a door closing.", Shooter,
                        Shooter, Target, Weapon.Parent)), OutputRange.Personal);
                Shooter.OutputHandler.Handle(new EmoteOutput(
                    new Emote("$0 stops aiming $2 at $1=0.", Shooter, Shooter, Target, Weapon.Parent),
                    flags: OutputFlags.SuppressSource));
                ReleaseEvents();
            }
        }
    }

    private void Target_NoLongerValid(IPerceivable owner)
    {
        Shooter.OutputHandler.Handle(
            new EmoteOutput(new Emote("You stop aiming $2 at $1=0 because you have lost sight of &1.", Shooter, Shooter,
                Target, Weapon.Parent)), OutputRange.Personal);
        Shooter.OutputHandler.Handle(new EmoteOutput(
            new Emote("$0 stops aiming $2 at $1=0.", Shooter, Shooter, Target, Weapon.Parent),
            flags: OutputFlags.SuppressSource));
        ReleaseEvents();
    }

    private void ReleasePathEvents()
    {
        foreach (ICellExit exit in Path)
        {
            if (exit.Exit.Door != null)
            {
                exit.Exit.Door.OnClose -= Door_OnClose;
                exit.Exit.Door.OnRemovedFromExit -= Door_OnRemovedFromExit;
                exit.Exit.Door.Parent.OnDeath -= Door_Destroyed;
                exit.Exit.Door.Parent.OnDeleted -= Door_Destroyed;
                exit.Exit.Door.Parent.OnQuit -= Door_Destroyed;
            }
        }
    }

    private bool _eventsReleased;

    public void ReleaseEvents()
    {
        if (!_eventsReleased)
        {
            _eventsReleased = true;
            AimInvalidated?.Invoke(this, null);
            if (Target != null)
            {
                Target.OnQuit -= Target_NoLongerValid;
                Target.OnDeleted -= Target_NoLongerValid;
                Target.OnLocationChanged -= Target_OnLocationChanged;
				Target.OnSpatialPositionChanged -= Target_OnSpatialPositionChanged;
                if (Target is IMortalPerceiver mp)
                {
                    mp.OnDeath -= Target_NoLongerValid;
                }

                if (Target is IGameItem gi)
                {
                    gi.OnRemovedFromLocation -= Target_NoLongerValid;
                }
            }

            Weapon.Parent.OnDeath -= WeaponDestroyed;
            Weapon.Parent.OnDeleted -= WeaponLost;
            Weapon.Parent.OnQuit -= WeaponLost;
            if (Weapon is IRangedWeaponWithUnreadyEvent bow)
            {
                bow.OnUnready -= WeaponUnreadied;
            }
            if (Shooter is ICharacter characterOwner)
            {
                characterOwner.OnMoved -= CharacterOwner_OnMoved;
				characterOwner.OnSpatialPositionChanged -= Shooter_OnSpatialPositionChanged;
                characterOwner.Body.OnInventoryChange -= WeaponInventoryStateChange;
            }

            ReleasePathEvents();
        }
    }

    ~AimInformation()
    {
        if (!_eventsReleased)
        {
            ReleaseEvents();
        }
    }
}
