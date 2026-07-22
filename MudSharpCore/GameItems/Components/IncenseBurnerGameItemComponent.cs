using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.GameItems.Components;

public class IncenseBurnerGameItemComponent : GameItemComponent, IIncenseBurner
{
	private IncenseBurnerGameItemComponentProto _prototype;
	private readonly List<IGameItem> _contents = [];
	private long _pendingBurningItemId;
	private IGameItem? _currentFuelItem;
	private double _remainingBurnSeconds;
	private int _drugPulseSecondsElapsed;
	private bool _lit;

	public IncenseBurnerGameItemComponent(IncenseBurnerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public IncenseBurnerGameItemComponent(MudSharp.Models.GameItemComponent component,
		IncenseBurnerGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public IncenseBurnerGameItemComponent(IncenseBurnerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_remainingBurnSeconds = rhs._remainingBurnSeconds;
		_pendingBurningItemId = rhs._pendingBurningItemId;
		_drugPulseSecondsElapsed = rhs._drugPulseSecondsElapsed;
		_lit = rhs._lit;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public bool HasFuel => _currentFuelItem is not null || _contents.Any();
	public int ScentRange => _prototype.ScentRange;

	public bool Lit
	{
		get => _lit;
		set
		{
			if (_lit == value)
			{
				return;
			}

			_lit = value;
			Changed = true;
			UpdateHeartbeatSubscription();
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new IncenseBurnerGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (IncenseBurnerGameItemComponentProto)newProto;
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
		foreach (var item in _contents)
		{
			item.FinaliseLoadTimeTasks();
		}

		if (_pendingBurningItemId != 0)
		{
			_currentFuelItem = _contents.FirstOrDefault(x => x.Id == _pendingBurningItemId);
			_pendingBurningItemId = 0;
		}
	}

	public override void Login()
	{
		base.Login();
		foreach (var item in _contents)
		{
			item.Login();
		}

		UpdateHeartbeatSubscription();
	}

	public override void Quit()
	{
		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
		foreach (var item in _contents)
		{
			item.Quit();
		}

		base.Quit();
	}

	public override void Delete()
	{
		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
		foreach (var item in _contents.ToList())
		{
			_contents.Remove(item);
			item.ContainedIn = null;
			item.Delete();
		}

		base.Delete();
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			from item in _contents
			select new XElement("Contained", item.Id),
			new XElement("Lit", Lit),
			new XElement("RemainingBurnSeconds", _remainingBurnSeconds),
			new XElement("BurningItem", _currentFuelItem?.Id ?? 0),
			new XElement("DrugPulseSecondsElapsed", _drugPulseSecondsElapsed)).ToString();
	}

	private void LoadFromXml(XElement root)
	{
		_lit = bool.Parse(root.Element("Lit")?.Value ?? "false");
		_remainingBurnSeconds = double.Parse(root.Element("RemainingBurnSeconds")?.Value ?? "0.0");
		_pendingBurningItemId = long.Parse(root.Element("BurningItem")?.Value ?? "0");
		_drugPulseSecondsElapsed = int.Parse(root.Element("DrugPulseSecondsElapsed")?.Value ?? "0");
		foreach (var item in root.Elements("Contained")
		                         .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
		                         .Where(item => item is not null))
		{
			_contents.Add(item!);
			item!.Get(null);
			item.LoadTimeSetContainedIn(Parent);
		}
	}

	private void UpdateHeartbeatSubscription()
	{
		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
		if (Lit)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManagerOnSecondHeartbeat;
		}
	}

	private void HeartbeatManagerOnSecondHeartbeat()
	{
		BurnFuel(1.0);
	}

	private bool CanEmitIntoLocation => Parent.Location is not null && Parent.ContainedIn is null;

	internal void BurnFuel(double seconds)
	{
		if (!Lit)
		{
			return;
		}

		if (_currentFuelItem is null && !PrimeFuel())
		{
			Lit = false;
			return;
		}

		if (CanEmitIntoLocation)
		{
			RefreshScentEffects((int)Math.Ceiling(seconds));
			PulseDrug(seconds);
		}

		_remainingBurnSeconds -= seconds;
		Changed = true;
		if (_remainingBurnSeconds > 0.0 || _currentFuelItem is null)
		{
			return;
		}

		Parent.Handle(new EmoteOutput(new Emote("$0 burn|burns away in $1.", Parent, _currentFuelItem, Parent),
			flags: OutputFlags.SuppressObscured), OutputRange.Local);
		_contents.RemoveAll(x => ReferenceEquals(x, _currentFuelItem) || x.Id == _currentFuelItem.Id);
		_currentFuelItem.Delete();
		_currentFuelItem = null;
		_remainingBurnSeconds = 0.0;
		if (!_contents.Any())
		{
			Lit = false;
		}
	}

	private bool PrimeFuel()
	{
		_currentFuelItem = _contents.FirstOrDefault();
		if (_currentFuelItem is null)
		{
			return false;
		}

		_remainingBurnSeconds = Math.Max(1.0, _currentFuelItem.Weight * _prototype.SecondsPerUnitWeight);
		Changed = true;
		return true;
	}

	public void RefreshScentEffects(int seconds)
	{
		if (!CanEmitIntoLocation)
		{
			return;
		}

		var sourceDescription = Parent.HowSeen(null, flags: PerceiveIgnoreFlags.IgnoreCanSee);
		foreach (var (cell, distance) in AffectedCells(_prototype.ScentRange))
		{
			var text = distance == 0 ? _prototype.SourceScentDescription : _prototype.DistantScentDescription;
			if (string.IsNullOrWhiteSpace(text))
			{
				continue;
			}

			var difficulty = _prototype.ScentDifficulty.StageUp(distance);
			double? routePosition = cell.RouteDefinition is not null && ReferenceEquals(cell, Parent.Location)
				? RouteSpatialService.Instance.GetEffectiveLocation(Parent).RoutePositionMetres
				: null;
			double? maximumRouteDistance = cell.RouteDefinition is { } route
				? Math.Max(
					RouteSpatialConfiguration.FromGameworld(Gameworld).ImmediateDistanceMetres,
					_prototype.ScentRange * route.MetresPerRoomEquivalent)
				: null;
			var existing = cell.EffectsOfType<IScentTrailEffect>()
			                   .FirstOrDefault(x => x.SourceItemId == Parent.Id && x.RoomLayer == Parent.RoomLayer);
			var duration = TimeSpan.FromSeconds(Math.Max(1.0, seconds * _prototype.LingeringMultiplier + 2.0));
			if (existing is AmbientScent ambientScent &&
			    ambientScent.Matches(sourceDescription, text, Parent.RoomLayer, distance, difficulty,
				    routePosition, maximumRouteDistance))
			{
				Gameworld.EffectScheduler.ExtendSchedule(ambientScent, duration);
				continue;
			}

			existing?.ExpireEffect();
			var effect = new AmbientScent(cell, Parent.Id, sourceDescription, text, Parent.RoomLayer, distance,
				difficulty, routePositionMetres: routePosition,
				maximumRouteDistanceMetres: maximumRouteDistance);
			cell.AddEffect(effect, duration);
		}
	}

	private IEnumerable<(ICell Cell, int Distance)> AffectedCells(int range)
	{
		return Parent.CellsAndDistancesInVicinity((uint)Math.Max(0, range), true, false)
		             .Where(x => x.Cell is not null);
	}

	private void PulseDrug(double seconds)
	{
		if (!CanEmitIntoLocation ||
		    _prototype.Drug is null ||
		    _prototype.GramsPerPulse <= 0.0 ||
		    _prototype.DrugPulseSeconds <= 0)
		{
			return;
		}

		_drugPulseSecondsElapsed += (int)Math.Ceiling(seconds);
		if (_drugPulseSecondsElapsed < _prototype.DrugPulseSeconds)
		{
			return;
		}

		_drugPulseSecondsElapsed = 0;
		foreach (var (cell, distance) in AffectedCells(Math.Min(_prototype.DrugRange, _prototype.ScentRange)))
		{
			var dose = _prototype.GramsPerPulse / (distance + 1.0);
			var range = Math.Min(_prototype.DrugRange, _prototype.ScentRange);
			var recipients = cell.RouteDefinition is { } route && ReferenceEquals(cell, Parent.Location)
				? cell.CharactersInSpatialVicinity(
					Parent,
					maximumDistanceMetres: Math.Max(
						RouteSpatialConfiguration.FromGameworld(Gameworld).ImmediateDistanceMetres,
						range * route.MetresPerRoomEquivalent))
				: cell.LayerCharacters(Parent.RoomLayer);
			foreach (var character in recipients)
			{
				character.Body.Dose(_prototype.Drug, DrugVector.Inhaled, dose, Parent);
			}
		}
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Short or DescriptionType.Full or DescriptionType.Contents;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return Lit ? $"{description} {"(lit)".FluentColour(Telnet.Red, colour)}" : description;
			case DescriptionType.Contents:
				return _contents.Any()
					? description + "\n\nIt contains:\n" + _contents.Select(x => "\t" + x.HowSeen(voyeur)).ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
					: description + "\n\nIt is empty.";
			case DescriptionType.Full:
				return
					$"{description}\n\nIt is {(Lit ? "lit".Colour(Telnet.Red) : "unlit".Colour(Telnet.Yellow))} and {(_currentFuelItem is null ? "has no fuel currently burning" : $"is burning {_currentFuelItem.HowSeen(voyeur)}")}.\nIt contains {_contents.Count.ToString("N0", voyeur).ColourValue()} fuel item{(_contents.Count == 1 ? string.Empty : "s")}.";
			default:
				return description;
		}
	}

	public override bool Take(IGameItem item)
	{
		if (_contents.Remove(item))
		{
			item.ContainedIn = null;
			Changed = true;
			return true;
		}

		return false;
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		var newContainer = newItem?.GetItemType<IContainer>();
		foreach (var item in _contents.ToList())
		{
			if (newContainer is not null && newContainer.CanPut(item))
			{
				newContainer.Put(null, item);
			}
			else if (location is not null)
			{
				InsertAtParentSpatialLocation(item, location);
				item.ContainedIn = null;
			}
			else
			{
				item.Delete();
			}
		}

		_contents.Clear();
		_currentFuelItem = null;
		_remainingBurnSeconds = 0.0;
		return false;
	}

	public IEnumerable<IGameItem> Contents => _contents;
	public string ContentsPreposition => "in";
	public bool Transparent => true;

	public bool CanPut(IGameItem item)
	{
		return item != Parent &&
		       (_prototype.FuelTag is null || item.IsA(_prototype.FuelTag)) &&
		       _contents.Sum(x => x.Weight) + item.Weight <= _prototype.MaximumFuelWeight;
	}

	public void Put(ICharacter? putter, IGameItem item, bool allowMerge = true)
	{
		_contents.Add(item);
		item.ContainedIn = Parent;
		Changed = true;
	}

	public WhyCannotPutReason WhyCannotPut(IGameItem item)
	{
		if (item == Parent)
		{
			return WhyCannotPutReason.CantPutContainerInItself;
		}

		if (_prototype.FuelTag is not null && !item.IsA(_prototype.FuelTag))
		{
			return WhyCannotPutReason.NotCorrectItemType;
		}

		if (_contents.Sum(x => x.Weight) + item.Weight > _prototype.MaximumFuelWeight)
		{
			var capacity = CanPutAmount(item);
			if (item.Quantity <= 1 || capacity <= 0)
			{
				return WhyCannotPutReason.ContainerFull;
			}

			return WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity;
		}

		return WhyCannotPutReason.NotContainer;
	}

	public bool CanTake(ICharacter taker, IGameItem item, int quantity)
	{
		return _contents.Contains(item) &&
		       item != _currentFuelItem &&
		       item.CanGet(quantity).AsBool();
	}

	public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		if (!CanTake(taker, item, quantity))
		{
			return null!;
		}

		Changed = true;
		if (quantity == 0 || item.DropsWhole(quantity))
		{
			_contents.Remove(item);
			item.ContainedIn = null;
			return item;
		}

		return item.Get(null, quantity);
	}

	public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
	{
		if (item == _currentFuelItem)
		{
			return WhyCannotGetContainerReason.UnlawfulAction;
		}

		return _contents.Contains(item)
			? WhyCannotGetContainerReason.NotContainer
			: WhyCannotGetContainerReason.NotContained;
	}

	public int CanPutAmount(IGameItem item)
	{
		var remainingWeight = _prototype.MaximumFuelWeight - _contents.Sum(x => x.Weight);
		if (remainingWeight <= 0.0)
		{
			return 0;
		}

		if (item.Quantity <= 0 || item.Weight <= 0.0)
		{
			return int.MaxValue;
		}

		return Math.Max(0, (int)(remainingWeight / (item.Weight / item.Quantity)));
	}

	public void Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)
	{
		foreach (var item in _contents.ToList())
		{
			if (item == _currentFuelItem)
			{
				continue;
			}

			_contents.Remove(item);
			item.ContainedIn = null;
			if (intoContainer is not null && intoContainer.CanPut(item))
			{
				intoContainer.Put(emptier, item);
				continue;
			}

			item.InsertAtSource(emptier is null ? Parent.LocationLevelPerceivable : emptier);
		}

		Changed = true;
	}

	public bool CanLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		return (Parent.Location?.CanGetAccess(Parent, lightee) ?? true) &&
		       !Lit &&
		       HasFuel;
	}

	public string WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return Parent.Location.WhyCannotGetAccess(Parent, lightee);
		}

		if (Lit)
		{
			return $"{Parent.HowSeen(lightee, true)} is already lit.";
		}

		if (!HasFuel)
		{
			return $"{Parent.HowSeen(lightee, true)} has no fuel to burn.";
		}

		return $"You cannot light {Parent.HowSeen(lightee)}.";
	}

	public bool Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)
	{
		if (!CanLight(lightee, ignitionSource))
		{
			lightee.OutputHandler.Send(WhyCannotLight(lightee, ignitionSource));
			return false;
		}

		lightee.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ light|lights $1$?2| with $2||$", lightee, lightee, Parent,
				ignitionSource)).Append(playerEmote));
		Lit = true;
		RefreshScentEffects(1);
		return true;
	}

	public bool CanExtinguish(ICharacter lightee)
	{
		return (Parent.Location?.CanGetAccess(Parent, lightee) ?? true) && Lit;
	}

	public string WhyCannotExtinguish(ICharacter lightee)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return Parent.Location.WhyCannotGetAccess(Parent, lightee);
		}

		return !Lit
			? $"{Parent.HowSeen(lightee, true)} is not lit."
			: $"You cannot extinguish {Parent.HowSeen(lightee)}.";
	}

	public bool Extinguish(ICharacter lightee, IEmote playerEmote)
	{
		if (!CanExtinguish(lightee))
		{
			lightee.OutputHandler.Send(WhyCannotExtinguish(lightee));
			return false;
		}

		lightee.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ extinguish|extinguishes $1", lightee, lightee, Parent)).Append(playerEmote));
		Lit = false;
		return true;
	}
}
