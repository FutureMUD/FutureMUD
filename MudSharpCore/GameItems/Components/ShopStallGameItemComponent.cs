using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components
{
	public class ShopStallGameItemComponent : GameItemComponent, IContainer, IOpenable, ILockable, ILock, IShopStall
	{
		protected ShopStallGameItemComponentProto _prototype;
		public override IGameItemComponentProto Prototype => _prototype;

		protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
		{
			_prototype = (ShopStallGameItemComponentProto)newProto;
		}

		#region Constructors
		public ShopStallGameItemComponent(ShopStallGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
		{
			_prototype = proto;
		}

		public ShopStallGameItemComponent(Models.GameItemComponent component, ShopStallGameItemComponentProto proto, IGameItem parent) : base(component, parent)
		{
			_prototype = proto;
			_noSave = true;
			LoadFromXml(XElement.Parse(component.Definition));
			_noSave = false;
		}

		public ShopStallGameItemComponent(ShopStallGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
		{
			_prototype = rhs._prototype;
			_isOpen = rhs._isOpen;
			_isLocked = rhs._isLocked;
			_pattern = rhs._pattern;
		}

		protected void LoadFromXml(XElement root)
		{
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
				if (item.ContainedIn != null || item.Location != null || item.InInventoryOf != null)
				{
					Changed = true;
					Gameworld.SystemMessage(
						$"Duplicated Item: {item.HowSeen(item, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {item.Id.ToString("N0")}",
						true);
					continue;
				}

				_contents.Add(item);
				item.Get(null);
				item.LoadTimeSetContainedIn(Parent);
			}

			var element = root.Element("IsLocked");
			if (element != null)
			{
				IsLocked = bool.Parse(element.Value);
			}

			element = root.Element("IsTrading");
			if (element != null)
			{
				_isTrading = bool.Parse(element.Value);
			}

			element = root.Element("Pattern");
			if (element != null)
			{
				_pattern = int.Parse(element.Value);
			}

			element = root.Element("Shop");
			if (element != null)
			{
				Shop = Gameworld.Shops.Get(long.Parse(element.Value)) as ITransientShop;
				if (Shop is not null)
				{
					Shop.CurrentStall = this;
				}
			}

		}

		public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
		{
			return new ShopStallGameItemComponent(this, newParent, temporary);
		}
		#endregion

		#region Saving
		protected override string SaveToXml()
		{
			return new XElement("Definition",
				   new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
						new XElement("Shop", Shop?.Id ?? 0L),
						new XElement("Pattern", Pattern),
						new XElement("IsLocked", IsLocked),
						new XElement("IsTrading", IsTrading),
						new XElement("Locks", from thelock in Locks select new XElement("Lock", thelock.Parent.Id)),
						from content in Contents select new XElement("Contained", content.Id)
					).ToString();
		}


		#endregion

		#region GameItemComponent Overrides
		public override bool PreventsMerging(IGameItemComponent component)
		{
			return Contents.Any() || Locks.Any() || Pattern != 0;
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

			if (Shop is not null)
			{
				Shop.CurrentStall = this;
			}
		}

		public override void Delete()
		{
			base.Delete();
			foreach (var item in Contents.ToList())
			{
				_contents.Remove(item);
				item.Delete();
			}

			foreach (var item in Locks.ToList())
			{
				_locks.Remove(item);
				item.Parent.Delete();
			}

			if (Shop is not null)
			{
				Shop.CurrentStall = null;
			}
		}

		public override void Quit()
		{
			foreach (var item in Contents)
			{
				item.Quit();
			}

			foreach (var item in Locks)
			{
				item.Quit();
			}

			if (Shop is not null)
			{
				Shop.CurrentStall = null;
			}
		}

		public override bool Take(IGameItem item)
		{
			if (Contents.Contains(item))
			{
				_contents.Remove(item);
				Changed = true;
				return true;
			}
			return false;
		}

		public override bool DescriptionDecorator(DescriptionType type)
		{
			return ((IsOpen || Transparent) && (type == DescriptionType.Contents || type == DescriptionType.Evaluate)) ||
				   type == DescriptionType.Full;
		}

		public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
			bool colour, PerceiveIgnoreFlags flags)
		{
			var sb = new StringBuilder();
			switch (type)
			{
				case DescriptionType.Evaluate:
					sb.AppendLine(
						$"It can hold {Gameworld.UnitManager.DescribeMostSignificantExact(_prototype.WeightLimit, Framework.Units.UnitType.Mass, voyeur).Colour(Telnet.Green)} of items up to {_prototype.MaximumContentsSize.Describe().ColourValue()} size.");
					sb.AppendLine(
							$"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}.".Colour(
								Telnet.Yellow));

					return sb.ToString();
				case DescriptionType.Contents:
					if (_contents.Any())
					{
						return description + "\n\nIt has the following contents:\n" +
							   (from item in _contents
								select "\t" + item.HowSeen(voyeur)).ListToString(separator: "\n", conjunction: "",
								   twoItemJoiner: "\n");
					}

					return description + "\n\nIt is currently empty.";
				case DescriptionType.Full:
					sb.Append(description);

					sb.AppendLine();
					sb.AppendLine(
							$"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}."
								.Colour(Telnet.Yellow));


					if (IsOpen || Transparent)
					{
						sb.AppendLine($"It is {(_contents.Sum(x => x.Weight) / _prototype.WeightLimit).ToString("P2", voyeur).Colour(Telnet.Green)} full.");
					}

					sb.AppendLine($"{description}\nIt has a built-in lock that accepts key of type {LockType.ColourName()} and {(IsLocked ? "is currently locked." : "is currently unlocked.").Colour(Telnet.Yellow)}");
					if (Locks.Any())
					{
						sb.AppendLine();
						sb.AppendLine("It has the following locks:");
						foreach (var thelock in Locks)
						{
							sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
						}
					}
					if (Shop is null)
					{
						sb.AppendLine("It has not been configured to serve any shop yet.");
					}
					else
					{
						sb.AppendLine($"It is serving the {Shop.Name.ColourName()} shop.");
						sb.AppendLine($"It is {(IsTrading ? "currently" : "not currently")} trading.");
					}
					return sb.ToString();
			}
			return description;
		}

		public override double ComponentWeight
		{
			get { return Contents.Sum(x => x.Weight) + Locks.Sum(x => x.Parent.Weight); }
		}

		public override double ComponentBuoyancy(double fluidDensity)
		{
			return Contents.Sum(x => x.Buoyancy(fluidDensity)) + Locks.Sum(x => x.Parent.Buoyancy(fluidDensity));
		}

		public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
		{
			if (_contents.Contains(existingItem))
			{
				_contents[_contents.IndexOf(existingItem)] = newItem;
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

		public override bool Die(IGameItem newItem, ICell location)
		{
			if (Shop is not null)
			{
				Shop.CurrentStall = null;
				Shop = null;
			}

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

					_contents.Clear();
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

				_contents.Clear();
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

		#endregion

		#region IContainer Members
		protected readonly List<IGameItem> _contents = new();
		public IEnumerable<IGameItem> Contents => _contents;
		public string ContentsPreposition => _prototype.ContentsPreposition;
		public bool CanPut(IGameItem item)
		{
			return
				item != Parent &&
				IsOpen &&
				(item.Size <= _prototype.MaximumContentsSize || item.IsItemType<ICommodity>()) &&
				_contents.Sum(x => x.Weight) + item.Weight <= _prototype.WeightLimit;
		}

		public int CanPutAmount(IGameItem item)
		{
			return (int)((_prototype.WeightLimit - _contents.Sum(x => x.Weight)) / (item.Weight / item.Quantity));
		}

		public void Put(ICharacter putter, IGameItem item, bool allowMerge = true)
		{
			if (_contents.Contains(item))
			{
#if DEBUG
				throw new ApplicationException("Item duplication in container.");
#endif
				return;
			}

			if (allowMerge)
			{
				var mergeTarget = _contents.FirstOrDefault(x => x.CanMerge(item));
				if (mergeTarget != null)
				{
					mergeTarget.Merge(item);
					item.Delete();
					return;
				}
			}

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

			if (!IsOpen)
			{
				return WhyCannotPutReason.ContainerClosed;
			}

			if (item.Size > _prototype.MaximumContentsSize)
			{
				return WhyCannotPutReason.ItemTooLarge;
			}

			if (_contents.Sum(x => x.Weight) + item.Weight > _prototype.WeightLimit)
			{
				var capacity =
					(int)((_prototype.WeightLimit - _contents.Sum(x => x.Weight)) / (item.Weight / item.Quantity));
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
			return IsOpen && _contents.Contains(item) && item.CanGet(quantity).AsBool();
		}

		public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
		{
			Changed = true;
			if (quantity == 0 || item.DropsWhole(quantity))
			{
				_contents.Remove(item);
				item.ContainedIn = null;
				if (!IsAllowedToInteract(taker))
				{
					CrimeExtensions.CheckPossibleCrimeAllAuthorities(taker, CrimeTypes.Theft, null, item, "shoplifting");
				}
				var newItem = item.Get(null, quantity);
				if (!IsAllowedToInteract(taker))
				{
					CrimeExtensions.CheckPossibleCrimeAllAuthorities(taker, CrimeTypes.Theft, null, newItem, "shoplifting");
				}

				return newItem;
			}

			return item.Get(null, quantity);
		}

		public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
		{
			if (!IsOpen)
			{
				return WhyCannotGetContainerReason.ContainerClosed;
			}

			if (taker?.Account.ActLawfully == true && !IsAllowedToInteract(taker))
			{
				return WhyCannotGetContainerReason.UnlawfulAction;
			}

			return !_contents.Contains(item)
				? WhyCannotGetContainerReason.NotContained
				: WhyCannotGetContainerReason.NotContainer;
		}

		public bool Transparent => _prototype.Transparent;

		public void Empty(ICharacter emptier, IContainer intoContainer, IEmote playerEmote = null)
		{
			var location = emptier?.Location ?? Parent.TrueLocations.FirstOrDefault();
			var contents = Contents.ToList();
			_contents.Clear();
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

			var crime = !IsAllowedToInteract(emptier);
			foreach (var item in contents)
			{
				if (crime)
				{
					CrimeExtensions.CheckPossibleCrimeAllAuthorities(emptier, CrimeTypes.Theft, null, item,
						"shoplifting");
				}
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

		private bool IsAllowedToInteract(ICharacter character)
		{
			if (Shop is null)
			{
				return true;
			}
			return Shop?.IsEmployee(character) != false;
		}

		#endregion

		#region IOpenable Members

		private bool _isOpen = true;

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
			return !IsOpen && !IsLocked && Locks.All(x => !x.IsLocked) &&
				   Parent.EffectsOfType<IOverrideLockEffect>().All(x => !x.Applies(opener?.Actor));
		}

		public WhyCannotOpenReason WhyCannotOpen(IBody opener)
		{

			if (IsOpen)
			{
				return WhyCannotOpenReason.AlreadyOpen;
			}

			return Locks.Any(x => x.IsLocked) ||
				IsLocked ||
				   Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies(opener?.Actor))
				? WhyCannotOpenReason.Locked
				: WhyCannotOpenReason.Unknown;
		}

		public void Open()
		{
			IsOpen = true;
			OnOpen?.Invoke(this);
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

		#region ILock Members

		public bool CanBeInstalled => false;

		public void InstallLock(ILockable lockable, IExit exit, ICell installLocation)
		{
			// Do nothing
		}

		public bool SetLocked(bool locked, bool echo)
		{
			if (locked == IsLocked)
			{
				return false;
			}

			IsLocked = locked;
			if (echo)
			{
				Parent.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(IsLocked ? _prototype.LockEmoteNoActor : _prototype.UnlockEmoteNoActor, Parent, Parent),
						flags: OutputFlags.SuppressObscured));
			}

			return true;
		}

		public bool CanUnlock(ICharacter actor, IKey key)
		{
			if (actor?.IsAdministrator() != false)
			{
				return true;
			}

			if (Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies(actor)))
			{
				return false;
			}

			return key?.Unlocks(LockType, Pattern) == true;
		}

		public bool Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
		{
			if (!CanUnlock(actor, key))
			{
				return false;
			}

			IsLocked = false;
			if (actor != null && key != null)
			{
				actor.OutputHandler.Handle(new MixedEmoteOutput(
					new Emote(_prototype.UnlockEmote, actor, actor, Parent, key?.Parent),
					flags: OutputFlags.SuppressObscured).Append(playerEmote));
			}
			else
			{
				foreach (var cell in Parent.TrueLocations)
				{
					cell.Handle(
						new EmoteOutput(new Emote(_prototype.UnlockEmoteNoActor, Parent, Parent)));
				}
			}

			return true;
		}

		public bool CanLock(ICharacter actor, IKey key)
		{
			if (actor?.IsAdministrator() != false)
			{
				return true;
			}

			return key?.Unlocks(LockType, Pattern) == true;
		}

		public bool Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
		{
			if (!CanLock(actor, key))
			{
				return false;
			}

			IsLocked = true;
			if (actor != null && key != null)
			{
				actor.OutputHandler.Handle(new MixedEmoteOutput(
					new Emote(_prototype.LockEmote, actor, actor, Parent, key.Parent),
					flags: OutputFlags.SuppressObscured).Append(playerEmote));
			}
			else
			{
				foreach (var cell in Parent.TrueLocations)
				{
					cell.Handle(
						new EmoteOutput(new Emote(_prototype.LockEmoteNoActor, Parent, Parent)));
				}
			}

			return true;
		}

		private bool _isLocked;

		public bool IsLocked
		{
			get { return _isLocked || Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies()); }
			set
			{
				_isLocked = value;
				Changed = true;
			}
		}

		public string LockType => _prototype.LockType;

		private int _pattern;

		public int Pattern
		{
			get => _pattern;
			set
			{
				_pattern = value;
				Changed = true;
			}
		}

		public Difficulty ForceDifficulty => _prototype.ForceDifficulty;
		public Difficulty PickDifficulty => _prototype.PickDifficulty;

		public string Inspect(ICharacter actor, string description)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"You identify the following information about {Parent.HowSeen(actor)}");
			sb.AppendLine("");
			sb.AppendLine($"\tThis door's in-built lock is considered a {LockType.Colour(Telnet.Green)} style lock.");
			sb.AppendLine(Pattern == 0
				? "\tThis door's in-built lock has no combination set."
				: "\tThis door's in-built lock has a combination set so can have keys paired to it.");
			sb.AppendLine($"\tThis door's in-built lock appears to be {PickDifficulty.DescribeColoured()} to pick.");
			sb.AppendLine($"\tThis door's in-built lock appears to be {ForceDifficulty.DescribeColoured()} to force.");
			if (actor.IsAdministrator())
			{
				sb.AppendLine(
					$"  The combination for this door's in-built lock is: {Pattern.ToString("N0", actor).ColourValue()}");
			}

			return sb.ToString();
		}

		#endregion

		#region IShopStall Members
#nullable enable
		private ITransientShop? _shop;
		public ITransientShop? Shop
		{
			get => _shop; set
			{
				_shop = value;
				Changed = true;
			}
		}

		private bool _isTrading = false;
		public bool IsTrading
		{
			get => _isTrading; 
			set
			{
				_isTrading = value;
				Changed = true;
				if (value)
				{
					Parent.RemoveAllEffects<ShopStallNoGetEffect>(fireRemovalAction: true);
					Parent.AddEffect(new ShopStallNoGetEffect(Parent));
				}
				else
				{
					Parent.RemoveAllEffects<ShopStallNoGetEffect>(fireRemovalAction: true);
				}
			}
		}
#nullable restore
		#endregion
	}
}
