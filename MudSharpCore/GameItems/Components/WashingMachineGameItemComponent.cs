using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Construction;

namespace MudSharp.GameItems.Components;

public class WashingMachineGameItemComponent : GameItemComponent, ILiquidContainer, IContainer, IOnOff, ISelectable,
	IConsumePower, ILockable
{
	public bool CanBeEmptiedWhenInRoom => true;

	private static ILiquid _waterLiquid;

	// TODO - replace this by plumbing in the future
	public static ILiquid WaterLiquid
	{
		get
		{
			if (_waterLiquid == null)
			{
				_waterLiquid = Futuremud.Games.First().Liquids
				                        .Get(Futuremud.Games.First().GetStaticLong("DefaultWaterLiquid"));
			}

			return _waterLiquid;
		}
	}

	protected WashingMachineGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (WashingMachineGameItemComponentProto)newProto;
	}

	public override void Delete()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		base.Delete();
		foreach (var item in Contents.ToList())
		{
			_laundryContents.Remove(item);
			item.Delete();
		}

		foreach (var item in Locks.ToList())
		{
			_locks.Remove(item);
			item.Parent.Delete();
		}
	}

	public override void Quit()
	{
		Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
		foreach (var item in Contents)
		{
			item.Quit();
		}

		foreach (var item in Locks)
		{
			item.Quit();
		}
	}

	public override void Login()
	{
		foreach (var item in Contents)
		{
			item.Login();
		}

		foreach (var item in Locks)
		{
			item.Login();
		}

		if (SwitchedOn)
		{
			Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
		}
	}

	public override bool Take(IGameItem item)
	{
		if (Contents.Contains(item))
		{
			_laundryContents.Remove(item);
			Changed = true;
			return true;
		}

		return false;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var truth = false;
		foreach (var content in Contents)
		{
			truth = truth || content.HandleEvent(type, arguments);
		}

		return truth;
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemLockable = newItem?.GetItemType<ILockable>();
		if (newItemLockable != null)
		{
			foreach (var thelock in Locks.ToList())
			{
				newItemLockable.InstallLock(thelock);
			}
		}
		else
		{
			foreach (var thelock in Locks.ToList())
			{
				if (location != null)
				{
					location.Insert(thelock.Parent);
					thelock.Parent.ContainedIn = null;
				}
				else
				{
					thelock.Parent.Delete();
				}
			}
		}

		_locks.Clear();

		var newItemContainer = newItem?.GetItemType<IContainer>();
		if (newItemContainer != null)
		{
			var newItemOpenable = newItem.GetItemType<IOpenable>();
			if (newItemOpenable != null)
			{
				if (IsOpen)
				{
					newItemOpenable.Open();
				}
				else
				{
					newItemOpenable.Close();
				}
			}

			if (Contents.Any())
			{
				foreach (var item in Contents.ToList())
				{
					if (newItemContainer.CanPut(item))
					{
						newItemContainer.Put(null, item);
					}
					else if (location != null)
					{
						location.Insert(item);
						item.ContainedIn = null;
					}
					else
					{
						item.Delete();
					}
				}

				_laundryContents.Clear();
			}
		}
		else
		{
			foreach (var item in Contents.ToList())
			{
				if (location != null)
				{
					location.Insert(item);
					item.ContainedIn = null;
				}
				else
				{
					item.Delete();
				}
			}

			_laundryContents.Clear();
		}

		if (LiquidMixture == null)
		{
			return false;
		}

		var newItemLiquid = newItem?.GetItemType<ILiquidContainer>();
		if (newItemLiquid != null)
		{
			newItemLiquid.LiquidMixture = LiquidMixture;
			newItemLiquid.Changed = true;
		}


		return false;
	}

	public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
	{
		if (_laundryContents.Contains(existingItem))
		{
			_laundryContents[_laundryContents.IndexOf(existingItem)] = newItem;
			newItem.ContainedIn = Parent;
			Changed = true;
			existingItem.ContainedIn = null;
			return true;
		}

		if (_locks.Any(x => x.Parent == existingItem) && newItem.IsItemType<ILock>())
		{
			_locks[_locks.IndexOf(existingItem.GetItemType<ILock>())] = newItem.GetItemType<ILock>();
			existingItem.ContainedIn = null;
			newItem.ContainedIn = Parent;
			Changed = true;
			return true;
		}

		return false;
	}

	public override double ComponentWeight
	{
		get
		{
			return Contents.Sum(x => x.Weight) + Locks.Sum(x => x.Parent.Weight) + LiquidMixture?.TotalWeight ?? 0.0;
		}
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Contents.Sum(x => x.Buoyancy(fluidDensity)) + Locks.Sum(x => x.Parent.Buoyancy(fluidDensity)) +
			LiquidMixture?.Instances.Sum(x => (fluidDensity - x.Liquid.Density) * x.Amount * x.Liquid.Density) ?? 0.0;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		switch (type)
		{
			case DescriptionType.Full:
				return true;
			case DescriptionType.Contents:
			case DescriptionType.Evaluate:
				return IsOpen || Transparent;
		}

		return false;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var sb = new StringBuilder();
		switch (type)
		{
			case DescriptionType.Evaluate:
				sb.AppendLine(
					$"It can hold {Gameworld.UnitManager.DescribeMostSignificantExact(_prototype.WeightCapacity, Framework.Units.UnitType.Mass, voyeur).Colour(Telnet.Green)} of items up to {_prototype.MaximumItemSize.Describe().ColourValue()} size in the tub.");
				sb.AppendLine(
					$"It can hold {Gameworld.UnitManager.DescribeMostSignificantExact(LiquidCapacity, Framework.Units.UnitType.FluidVolume, voyeur).ColourValue()} of liquid detergent.");
				sb.AppendLine($"The door is currently {(IsOpen ? "open" : "closed")}.".Colour(Telnet.Yellow));
				return sb.ToString();
			case DescriptionType.Contents:
				sb.AppendLine(description);
				sb.AppendLine();
				if (_laundryContents.Any())
				{
					sb.AppendLine("It has the following contents in the tub:");
					sb.Append((from item in _laundryContents select "\t" + item.HowSeen(voyeur)).ListToString(
						separator: "\n", conjunction: "", twoItemJoiner: "\n"));
				}
				else
				{
					sb.AppendLine("The tub is currently empty.");
				}

				if (CurrentCycle == WashingMachineCycles.None)
				{
					if (LiquidMixture?.IsEmpty == false)
					{
						return string.Format(voyeur, "\n\nIt is approximately {0:P0} full of {1}.",
							Math.Round(20 * LiquidMixture.TotalVolume / LiquidCapacity) / 20,
							LiquidMixture.ColouredLiquidDescription);
					}
					else
					{
						sb.AppendLine("The detergent holder is currently empty.");
					}
				}

				return sb.ToString();
			case DescriptionType.Full:

				sb.Append(description);
				sb.AppendLine($"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}."
					.Colour(Telnet.Yellow));
				if (IsOpen || Transparent)
				{
					sb.AppendLine(
						$"The tub is {(_laundryContents.Sum(x => x.Weight) / _prototype.WeightCapacity).ToString("P2", voyeur).Colour(Telnet.Green)} full.");
				}

				if (Locks.Any())
				{
					sb.AppendLine();
					sb.AppendLine("It has the following locks:");
					foreach (var thelock in Locks)
					{
						sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
					}
				}

				sb.AppendLine(
					$"It has the following selections available (see {"help select".FluentTagMXP("send", "href = 'help select' hint = 'show the helpfile for the select command'")} for more info):\n\tquick - set the duration to quick wash\n\tnormal - set the duration to normal wash\n\tlong - set the duration to long wash\n\twash - start the wash cycle\n\tcancel - cancel a wash cycle in progress");
				return sb.ToString();
		}

		return description;
	}

	#region Constructors

	public WashingMachineGameItemComponent(WashingMachineGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		CurrentCycle = WashingMachineCycles.None;
		_switchedOn = false;
		_isOpen = true;
		CycleLengthMultiplier = 1.0;
		CurrentCycleElapsed = TimeSpan.Zero;
	}

	public WashingMachineGameItemComponent(MudSharp.Models.GameItemComponent component,
		WashingMachineGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public WashingMachineGameItemComponent(WashingMachineGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		if (rhs._liquidMixture != null)
		{
			_liquidMixture = new LiquidMixture(rhs._liquidMixture);
		}
	}

	protected void LoadFromXml(XElement root)
	{
		CurrentCycle = (WashingMachineCycles)int.Parse(root.Element("CurrentCycle").Value);
		CycleLengthMultiplier = double.Parse(root.Element("CycleLengthMultiplier").Value);
		CurrentCycleElapsed = TimeSpan.FromTicks(long.Parse(root.Element("CurrentCycleElapsed").Value));
		var attr = root.Attribute("Open");
		if (attr != null)
		{
			_isOpen = attr.Value == "true";
		}

		var lockelem = root.Element("Locks");
		if (lockelem != null)
		{
			foreach (
				var item in
				lockelem.Elements("Lock")
				        .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
				        .Where(item => item?.IsItemType<ILock>() == true))
			{
				if (item.ContainedIn != null || item.Location != null || item.InInventoryOf != null)
				{
					Changed = true;
					Gameworld.SystemMessage(
						$"Duplicated Item: {item.HowSeen(item, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {item.Id.ToString("N0")}",
						true);
					continue;
				}

				InstallLock(item.GetItemType<ILock>());
			}
		}

		foreach (
			var item in
			root.Elements("Contained")
			    .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
			    .Where(item => item != null))
		{
			if ((item.ContainedIn != null && item.ContainedIn != Parent) || item.Location != null ||
			    item.InInventoryOf != null)
			{
				Changed = true;
				Gameworld.SystemMessage(
					$"Duplicated Item: {item.HowSeen(item, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {item.Id.ToString("N0")}",
					true);
				continue;
			}

			_laundryContents.Add(item);
			item.Get(null);
			item.LoadTimeSetContainedIn(Parent);
		}

		attr = root.Attribute("On");
		if (attr != null)
		{
			SwitchedOn = bool.Parse(attr.Value);
		}

		// Legacy
		attr = root.Attribute("Liquid");
		if (attr != null)
		{
			var liquid = Gameworld.Liquids.Get(long.Parse(attr.Value));
			if (liquid != null)
			{
				LiquidMixture = new LiquidMixture(new[]
				{
					new LiquidInstance
					{
						Liquid = liquid,
						Amount = double.Parse(root.Attribute("LiquidQuantity").Value)
					}
				}, Gameworld);
			}
		}
		// Non-Legacy
		else
		{
			if (root.Element("Mix") != null)
			{
				LiquidMixture = new LiquidMixture(root.Element("Mix"), Gameworld);
			}
		}
	}

	public override void FinaliseLoad()
	{
		foreach (var item in Locks)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}

		foreach (var item in Contents)
		{
			item.FinaliseLoadTimeTasks();
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new WashingMachineGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("CurrentCycle", (int)CurrentCycle),
			new XElement("CycleLengthMultiplier", CycleLengthMultiplier),
			new XElement("CurrentCycleElapsed", CurrentCycleElapsed.Ticks),
			new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
			new XElement("Locks", from thelock in Locks select new XElement("Lock", thelock.Parent.Id)),
			from content in _laundryContents select new XElement("Contained", content.Id),
			new XAttribute("On", SwitchedOn),
			LiquidMixture?.SaveToXml() ?? new XElement("NoLiquid")
		).ToString();
	}

	#endregion

	public WashingMachineCycles CurrentCycle { get; private set; }

	public double CycleLengthMultiplier
	{
		get => _cycleLengthMultiplier;
		private set
		{
			_cycleLengthMultiplier = value;
			MultipliedCycleLength = TimeSpan.FromSeconds(_prototype.NormalCycleTime.Seconds * value);
		}
	}

	public TimeSpan CurrentCycleElapsed { get; private set; }
	public TimeSpan MultipliedCycleLength { get; private set; }

	public void StartWashing()
	{
		switch (CurrentCycle)
		{
			case WashingMachineCycles.None:
				Parent.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins ", Parent)));
				CurrentCycle = WashingMachineCycles.Prewash;
				CurrentCycleElapsed = TimeSpan.Zero;
				break;
			case WashingMachineCycles.Prewash:
				Parent.OutputHandler.Handle(new EmoteOutput(
					new Emote("@ resume|resumes washing its load of laundry at the pre-wash stage.", Parent)));
				break;
			case WashingMachineCycles.Rinse:
				Parent.OutputHandler.Handle(new EmoteOutput(
					new Emote("@ resume|resumes washing its load of laundry at the rinse stage.", Parent)));
				break;
			case WashingMachineCycles.Spin:
				Parent.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ resume|resumes washing its load of laundry at the spin stage.", Parent)));
				break;
			case WashingMachineCycles.Wash:
				Parent.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ resume|resumes washing its load of laundry at the wash stage.", Parent)));
				break;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
		Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
	}

	private LiquidMixture _wetWithWaterInstance = new(WaterLiquid, 0.05, Futuremud.Games.First());

	private void HandleHeartbeatWet(IGameItem item)
	{
		item.ExposeToLiquid(new LiquidMixture(WaterLiquid, 0.05, Gameworld), null, LiquidExposureDirection.Irrelevant);
	}

	private void HandleHeartbeatDetergent(IGameItem item, double detergentPerItem)
	{
		if (LiquidMixture?.IsEmpty == false && detergentPerItem > 0.0)
		{
			item.ExposeToLiquid(LiquidMixture.RemoveLiquidVolume(detergentPerItem), null,
				LiquidExposureDirection.Irrelevant);
		}
	}

	private void HeartbeatManager_SecondHeartbeat()
	{
		CurrentCycleElapsed += TimeSpan.FromSeconds(1);
		Changed = true;
		// For performance reasons, only do the effect schedules every 5 seconds
		if (CurrentCycleElapsed.Seconds % 5 == 0)
		{
			var detergentPerItem = (LiquidMixture?.TotalVolume ?? 0.0) * 5 /
			                       (_laundryContents.Count * MultipliedCycleLength.TotalSeconds);
			switch (CurrentCycle)
			{
				case WashingMachineCycles.Prewash:
					// Make items wet and covered in detergent during the pre-wash
					foreach (var item in _laundryContents)
					{
						HandleHeartbeatWet(item);
						HandleHeartbeatDetergent(item, detergentPerItem);
					}

					break;
				case WashingMachineCycles.Wash:
					// Clean items during the wash and keep the detergent effect alive
					foreach (var item in _laundryContents)
					{
						HandleHeartbeatWet(item);
					}

					break;
				case WashingMachineCycles.Rinse:
					// Clean items of detergent during the rinse
					foreach (var item in _laundryContents)
					{
						HandleHeartbeatWet(item);
					}

					break;
				case WashingMachineCycles.Spin:
					// Partially dry items during the spin
					foreach (var item in _laundryContents)
					foreach (var effect in item.EffectsOfType<ILiquidContaminationEffect>().ToList())
					{
						item.RemoveDuration(effect, TimeSpan.FromSeconds(5), true);
					}

					break;
			}
		}

		if (CurrentCycleElapsed >= MultipliedCycleLength)
		{
			switch (CurrentCycle)
			{
				case WashingMachineCycles.Prewash:
					Parent.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ have|has finished pre-washing and begins the wash cycle.",
							Parent)));
					CurrentCycle = WashingMachineCycles.Wash;
					break;
				case WashingMachineCycles.Rinse:
					Parent.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ have|has finished rinsing and begins the spin cycle.", Parent)));
					CurrentCycle = WashingMachineCycles.Spin;
					break;
				case WashingMachineCycles.Spin:
					Parent.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ have|has finished the spin cycle and is now done with washing.", Parent)));
					CurrentCycle = WashingMachineCycles.Wash;
					break;
				case WashingMachineCycles.Wash:
					Parent.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ have|has finished washing and begins the rinse cycle.", Parent)));
					CurrentCycle = WashingMachineCycles.Rinse;
					LiquidMixture = null;
					break;
				default:
					throw new ApplicationException("Washing machine ticked with invalid washing machine cycle.");
			}

			CurrentCycleElapsed = TimeSpan.Zero;
		}
	}

	public void CancelWashing(bool echo)
	{
		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
		if (echo)
		{
			Parent.OutputHandler.Handle(new EmoteOutput(new Emote("@ stop|stops washing in the middle of a cycle.",
				Parent)));
		}
	}

	#region IConsumePower Members

	private bool _powered = false;

	public double PowerConsumptionInWatts => SwitchedOn && CurrentCycle != WashingMachineCycles.None
		? _prototype.PowerUsageInWatts
		: _prototype.PowerUsageInWatts / 100;

	public void OnPowerCutIn()
	{
		_powered = true;
		if (SwitchedOn)
		{
			Parent.OutputHandler.Handle(new EmoteOutput(new Emote("@ power|powers on.", Parent)));
			if (CurrentCycle != WashingMachineCycles.None)
			{
				StartWashing();
			}
		}
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		if (SwitchedOn)
		{
			Parent.OutputHandler.Handle(new EmoteOutput(new Emote("@ power|powers off.", Parent)));
			if (CurrentCycle != WashingMachineCycles.None)
			{
				CancelWashing(true);
			}
		}
	}

	#endregion

	#region ISelectable Members

	public bool CanSelect(ICharacter character, string argument)
	{
		switch (argument.ToLowerInvariant())
		{
			case "quick":
				return true;
			case "normal":
				return true;
			case "long":
				return true;
			case "wash":
			case "begin":
			case "start":
				return CurrentCycle == WashingMachineCycles.None;
			case "stop":
			case "cancel":
				return CurrentCycle != WashingMachineCycles.None;
		}

		return false;
	}

	public bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)
	{
		if (!SwitchedOn || !_powered)
		{
			character.Send($"{Parent.HowSeen(character, true)} is not on, and so you cannot make any selections.");
			return false;
		}

		switch (argument.ToLowerInvariant())
		{
			case "quick":
				CycleLengthMultiplier = 0.5;
				if (!silent)
				{
					character.OutputHandler.Handle(
						new MixedEmoteOutput(new Emote($"@ select|selects the 'quick wash' option on $1", character,
							character, Parent)).Append(playerEmote));
				}

				return true;
			case "normal":
				CycleLengthMultiplier = 1.0;
				if (!silent)
				{
					character.OutputHandler.Handle(
						new MixedEmoteOutput(new Emote($"@ select|selects the 'normal wash' option on $1", character,
							character, Parent)).Append(playerEmote));
				}

				return true;
			case "long":
				CycleLengthMultiplier = 1.5;
				if (!silent)
				{
					character.OutputHandler.Handle(
						new MixedEmoteOutput(new Emote($"@ select|selects the 'long wash' option on $1", character,
							character, Parent)).Append(playerEmote));
				}

				return true;
			case "wash":
			case "begin":
			case "start":
				if (CurrentCycle != WashingMachineCycles.None)
				{
					character.Send(
						$"{Parent.HowSeen(character, true)} is already in a washing cycle. You must first stop it if you want to restart it for some reason.");
					return false;
				}

				if (!silent)
				{
					character.OutputHandler.Handle(
						new MixedEmoteOutput(new Emote($"@ select|selects the 'begin wash' option on $1", character,
							character, Parent)).Append(playerEmote));
				}

				StartWashing();
				return true;
			case "stop":
			case "cancel":
				if (CurrentCycle == WashingMachineCycles.None)
				{
					character.Send($"{Parent.HowSeen(character, true)} is not currently in a washing cycle.");
					return false;
				}

				if (!silent)
				{
					character.OutputHandler.Handle(
						new MixedEmoteOutput(new Emote($"@ select|selects the 'cancel wash' option on $1", character,
							character, Parent)).Append(playerEmote));
				}

				CancelWashing(true);
				return true;
			default:
				character.Send($"That is not a valid option for selecting things on {Parent.HowSeen(character)}.");
				return false;
		}
	}

	#endregion

	#region IOnOff Members

	private bool _switchedOn;

	public bool SwitchedOn
	{
		get => _switchedOn;
		set
		{
			_switchedOn = value;
			Changed = true;
			var power = Parent.GetItemType<IProducePower>();
			if (value)
			{
				power.BeginDrawdown(this);
			}
			else
			{
				power.EndDrawdown(this);
			}
		}
	}

	#endregion

	#region IContainer Members

	private readonly List<IGameItem> _laundryContents = new();
	public IEnumerable<IGameItem> Contents => _laundryContents;

	public string ContentsPreposition => "in";

	public bool Transparent => _prototype.Transparent;

	public bool CanPut(IGameItem item)
	{
		return
			item != Parent &&
			IsOpen &&
			(item.Size <= _prototype.MaximumItemSize || item.IsItemType<ICommodity>()) &&
			_laundryContents.Sum(x => x.Weight) + item.Weight <= _prototype.WeightCapacity;
	}

	public int CanPutAmount(IGameItem item)
	{
		return (int)((_prototype.WeightCapacity - _laundryContents.Sum(x => x.Weight)) / (item.Weight / item.Quantity));
	}

	public void Put(ICharacter putter, IGameItem item, bool allowMerge = true)
	{
		if (_laundryContents.Contains(item))
		{
#if DEBUG
			throw new ApplicationException("Item duplication in container.");
#endif
			return;
		}

		if (allowMerge)
		{
			var mergeTarget = _laundryContents.FirstOrDefault(x => x.CanMerge(item));
			if (mergeTarget != null)
			{
				mergeTarget.Merge(item);
				item.Delete();
				return;
			}
		}

		_laundryContents.Add(item);
		item.ContainedIn = Parent;
		Changed = true;
	}

	public WhyCannotPutReason WhyCannotPut(IGameItem item)
	{
		if (item == Parent)
		{
			return WhyCannotPutReason.CantPutContainerInItself;
		}

		if (!IsOpen)
		{
			return WhyCannotPutReason.ContainerClosed;
		}

		if (item.Size > _prototype.MaximumItemSize)
		{
			return WhyCannotPutReason.ItemTooLarge;
		}

		if (_laundryContents.Sum(x => x.Weight) + item.Weight > _prototype.WeightCapacity)
		{
			var capacity = (int)((_prototype.WeightCapacity - _laundryContents.Sum(x => x.Weight)) /
			                     (item.Weight / item.Quantity));
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
		return IsOpen && _laundryContents.Contains(item) && item.CanGet(quantity).AsBool();
	}

	public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		Changed = true;
		if (quantity == 0 || item.DropsWhole(quantity))
		{
			_laundryContents.Remove(item);
			item.ContainedIn = null;
			return item;
		}

		return item.Get(null, quantity);
	}

	public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
	{
		if (!IsOpen)
		{
			return WhyCannotGetContainerReason.ContainerClosed;
		}

		return !_laundryContents.Contains(item)
			? WhyCannotGetContainerReason.NotContained
			: WhyCannotGetContainerReason.NotContainer;
	}

	public void Empty(ICharacter emptier, IContainer intoContainer, IEmote playerEmote = null)
	{
		var location = emptier?.Location ?? Parent.TrueLocations.FirstOrDefault();
		var contents = Contents.ToList();
		_laundryContents.Clear();
		if (emptier is not null)
		{
			if (intoContainer == null)
			{
				emptier.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote("@ empty|empties $0 onto the ground.", emptier, Parent)).Append(
						playerEmote));
			}
			else
			{
				emptier.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote($"@ empty|empties $1 {intoContainer.ContentsPreposition}to $2.",
						emptier, emptier, Parent, intoContainer.Parent)).Append(playerEmote));
			}
		}

		foreach (var item in contents)
		{
			item.ContainedIn = null;
			if (intoContainer != null)
			{
				if (intoContainer.CanPut(item))
				{
					intoContainer.Put(emptier, item);
				}
				else if (location != null)
				{
					location.Insert(item);
					if (emptier != null)
					{
						emptier.OutputHandler.Handle(new EmoteOutput(new Emote(
							"@ cannot put $1 into $2, so #0 set|sets it down on the ground.", emptier, emptier, item,
							intoContainer.Parent)));
					}
				}
				else
				{
					item.Delete();
				}

				continue;
			}

			if (location != null)
			{
				location.Insert(item);
			}
			else
			{
				item.Delete();
			}
		}

		Changed = true;
	}

	#endregion

	#region ILiquidContainer Members

	private LiquidMixture _liquidMixture;

	public LiquidMixture LiquidMixture
	{
		get => _liquidMixture;
		set
		{
			_liquidMixture = value;
			Changed = true;
		}
	}

	private void AdjustLiquidQuantity(double amount, ICharacter who, string action)
	{
		if (LiquidMixture == null)
		{
			return;
		}

		LiquidMixture.AddLiquidVolume(amount);
		if (LiquidMixture.IsEmpty)
		{
			LiquidMixture = null;
		}

		Changed = true;
	}

	public void AddLiquidQuantity(double amount, ICharacter who, string action)
	{
		if (LiquidMixture == null)
		{
			return;
		}

		if (LiquidMixture.TotalVolume + amount > LiquidCapacity)
		{
			amount = LiquidCapacity - LiquidMixture.TotalVolume;
		}

		if (LiquidMixture.TotalVolume + amount < 0)
		{
			amount = -1 * LiquidMixture.TotalVolume;
		}

		AdjustLiquidQuantity(amount, who, action);
	}

	public void ReduceLiquidQuantity(double amount, ICharacter who, string action)
	{
		if (LiquidMixture == null)
		{
			return;
		}

		if (LiquidMixture.TotalVolume - amount > LiquidCapacity)
		{
			amount = (LiquidCapacity - LiquidMixture.TotalVolume) * -1;
		}

		if (LiquidMixture.TotalVolume - amount < 0)
		{
			amount = LiquidMixture.TotalVolume;
		}

		AdjustLiquidQuantity(amount * -1, who, action);
	}

	public void MergeLiquid(LiquidMixture otherMixture, ICharacter who, string action)
	{
		if (otherMixture == null)
		{
			return;
		}

		if (LiquidMixture == null)
		{
			LiquidMixture = otherMixture;
		}
		else
		{
			LiquidMixture.AddLiquid(otherMixture);
		}

		if (LiquidMixture.IsEmpty)
		{
			LiquidMixture = null;
		}

		Changed = true;
	}

	public LiquidMixture RemoveLiquidAmount(double amount, ICharacter who, string action)
	{
		if (LiquidMixture == null)
		{
			return null;
		}

		var newMixture = LiquidMixture.RemoveLiquidVolume(amount);
		Changed = true;
		if (LiquidMixture.IsEmpty)
		{
			LiquidMixture = null;
		}

		return newMixture;
	}

	public double LiquidVolume => LiquidMixture?.TotalVolume ?? 0.0;

	public double LiquidCapacity => _prototype.WashingLiquidCapacity;

	#endregion

	#region ILockable Members

	private readonly List<ILock> _locks = new();
	public IEnumerable<ILock> Locks => _locks;

	public bool InstallLock(ILock theLock, ICharacter actor = null)
	{
		_locks.Add(theLock);
		if (_noSave)
		{
			theLock.Parent.LoadTimeSetContainedIn(Parent);
		}
		else
		{
			theLock.Parent.ContainedIn = Parent;
		}

		Changed = true;
		return true;
	}

	public bool RemoveLock(ILock theLock)
	{
		if (_locks.Contains(theLock))
		{
			theLock.Parent.ContainedIn = null;
			_locks.Remove(theLock);
			Changed = true;
			return true;
		}

		return false;
	}

	#endregion

	#region IOpenable Members

	private bool _isOpen = true;
	private double _cycleLengthMultiplier;

	public bool IsOpen
	{
		get => _isOpen;
		protected set
		{
			_isOpen = value;
			Changed = true;
		}
	}

	public bool CanOpen(IBody opener)
	{
		return !IsOpen && Locks.All(x => !x.IsLocked) &&
		       Parent.EffectsOfType<IOverrideLockEffect>().All(x => !x.Applies(opener?.Actor)) &&
		       (!_prototype.DoorLock || CurrentCycle == WashingMachineCycles.None);
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
		if (IsOpen)
		{
			return WhyCannotOpenReason.AlreadyOpen;
		}

		return Locks.Any(x => x.IsLocked) ||
		       Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies(opener?.Actor)) ||
		       (_prototype.DoorLock && CurrentCycle != WashingMachineCycles.None)
			? WhyCannotOpenReason.Locked
			: WhyCannotOpenReason.Unknown;
	}

	public void Open()
	{
		IsOpen = true;
		OnOpen?.Invoke(this);
		if (CurrentCycle != WashingMachineCycles.None)
		{
			CancelWashing(true);
		}
		// TODO - spill water everywhere if opened during the wash cycle
	}

	public bool CanClose(IBody closer)
	{
		return IsOpen;
	}

	public WhyCannotCloseReason WhyCannotClose(IBody closer)
	{
		if (!IsOpen)
		{
			return WhyCannotCloseReason.AlreadyClosed;
		}

		return WhyCannotCloseReason.Unknown;
	}

	public void Close()
	{
		IsOpen = false;
		OnClose?.Invoke(this);
	}

	public event OpenableEvent OnOpen;
	public event OpenableEvent OnClose;

	#endregion
}