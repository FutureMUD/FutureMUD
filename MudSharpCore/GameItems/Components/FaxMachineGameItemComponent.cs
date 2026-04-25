#nullable enable
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class FaxMachineGameItemComponent : TelephoneGameItemComponent, IFaxMachine, IContainer, IOpenable, ILockable
{
    private FaxMachineGameItemComponentProto _prototype;
    private readonly List<IGameItem> _contents = [];
    private readonly List<ILock> _locks = [];
    private readonly List<IncomingFaxJob> _pendingFaxes = [];
    private bool _isOpen = true;

    public FaxMachineGameItemComponent(FaxMachineGameItemComponentProto proto, IGameItem parent, bool temporary = false)
        : base(proto, parent, temporary)
    {
        _prototype = proto;
        CurrentInkLevels = _prototype.MaximumCharactersPrintedPerCartridge;
    }

    public FaxMachineGameItemComponent(Models.GameItemComponent component, FaxMachineGameItemComponentProto proto,
        IGameItem parent)
        : base(component, proto, parent)
    {
        _prototype = proto;
        LoadFaxState(XElement.Parse(component.Definition));
    }

    public FaxMachineGameItemComponent(FaxMachineGameItemComponent rhs, IGameItem newParent, bool temporary = false)
        : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _isOpen = rhs._isOpen;
        CurrentInkLevels = _prototype.MaximumCharactersPrintedPerCartridge;
    }

    public override IGameItemComponentProto Prototype => _prototype;
    public bool SupportsVoiceCalls => false;
    public bool CanReceiveFaxes =>
        SwitchedOn &&
        IsPowered &&
        TelecommunicationsGrid != null &&
        !string.IsNullOrWhiteSpace(PhoneNumber) &&
        !IsEngaged &&
        !IsRinging;
    public int CurrentInkLevels { get; protected set; }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        base.UpdateComponentNewPrototype(newProto);
        _prototype = (FaxMachineGameItemComponentProto)newProto;
    }

    private void LoadFaxState(XElement root)
    {
        CurrentInkLevels = int.Parse(root.Element("CurrentInkLevels")?.Value ??
                                     _prototype.MaximumCharactersPrintedPerCartridge.ToString());
        _isOpen = bool.Parse(root.Attribute("Open")?.Value ?? "true");

        XElement? lockElement = root.Element("Locks");
        if (lockElement != null)
        {
            foreach (IGameItem? item in lockElement.Elements("Lock")
                                            .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
                                            .Where(item => item?.IsItemType<ILock>() == true))
            {
                if (item.ContainedIn != null || item.Location != null || item.InInventoryOf != null)
                {
                    Changed = true;
                    Gameworld.SystemMessage(
                        $"Duplicated Item: {item.HowSeen(item, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {item.Id:N0}",
                        true);
                    continue;
                }

                InstallLock(item.GetItemType<ILock>());
            }
        }

        foreach (IGameItem? item in root.Elements("Contained")
                                 .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
                                 .Where(item => item != null))
        {
            if ((item.ContainedIn != null && item.ContainedIn != Parent) || item.Location != null ||
                item.InInventoryOf != null)
            {
                Changed = true;
                Gameworld.SystemMessage(
                    $"Duplicated Item: {item.HowSeen(item, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {item.Id:N0}",
                    true);
                continue;
            }

            _contents.Add(item);
            item.Get(null);
            item.LoadTimeSetContainedIn(Parent);
        }

        XElement? pendingRoot = root.Element("PendingFaxes");
        if (pendingRoot == null)
        {
            return;
        }

        foreach (XElement faxElement in pendingRoot.Elements("Fax"))
        {
            string senderNumber = faxElement.Attribute("sender")?.Value ?? "Unknown";
            List<ICanBeRead> readables = new();
            foreach (XElement readableElement in faxElement.Elements())
            {
                ICanBeRead? readable = readableElement.Name.LocalName.EqualTo("Writing")
                    ? Gameworld.Writings.Get(long.Parse(readableElement.Value))
                    : Gameworld.Drawings.Get(long.Parse(readableElement.Value));
                if (readable != null)
                {
                    readables.Add(readable);
                }
            }

            if (readables.Any())
            {
                _pendingFaxes.Add(new IncomingFaxJob(senderNumber, readables));
            }
        }
    }

    protected override string SaveToXml()
    {
        XElement root = XElement.Parse(base.SaveToXml());
        root.Add(new XAttribute("Open", IsOpen));
        root.Add(new XElement("Locks",
            from thelock in Locks
            select new XElement("Lock", thelock.Parent.Id)
        ));
        foreach (IGameItem item in Contents)
        {
            root.Add(new XElement("Contained", item.Id));
        }

        root.Add(new XElement("CurrentInkLevels", CurrentInkLevels));
        root.Add(new XElement("PendingFaxes",
            from fax in _pendingFaxes
            select new XElement("Fax",
                new XAttribute("sender", fax.SenderNumber),
                from readable in fax.RemainingReadables
                select readable is MudSharp.Communication.Language.IWriting
                    ? new XElement("Writing", readable.Id)
                    : new XElement("Drawing", readable.Id)
            )
        ));
        return root.ToString();
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new FaxMachineGameItemComponent(this, newParent, temporary);
    }

    public override void FinaliseLoad()
    {
        foreach (ILock item in Locks)
        {
            item.Parent.FinaliseLoadTimeTasks();
        }

        foreach (IGameItem item in Contents)
        {
            item.FinaliseLoadTimeTasks();
        }

        base.FinaliseLoad();
        TryPrintPendingFaxes();
    }

    public override void Quit()
    {
        base.Quit();
        foreach (IGameItem item in Contents)
        {
            item.Quit();
        }

        foreach (ILock item in Locks)
        {
            item.Quit();
        }
    }

    public override void Login()
    {
        foreach (IGameItem item in Contents)
        {
            item.Login();
        }

        foreach (ILock item in Locks)
        {
            item.Login();
        }

        base.Login();
        TryPrintPendingFaxes();
    }

    public override void Delete()
    {
        base.Delete();
        foreach (IGameItem? item in Contents.ToList())
        {
            _contents.Remove(item);
            item.Delete();
        }

        foreach (ILock? item in Locks.ToList())
        {
            _locks.Remove(item);
            item.Parent.Delete();
        }
    }

    public override void OnPowerCutIn()
    {
        base.OnPowerCutIn();
        TryPrintPendingFaxes();
    }

    public override bool Switch(ICharacter actor, string setting)
    {
        if (!base.Switch(actor, setting))
        {
            return false;
        }

        if (SwitchedOn && IsPowered)
        {
            TryPrintPendingFaxes();
        }

        return true;
    }

    public bool CanSendFax(ICharacter actor, string number, IReadable document, out string error)
    {
        if (TelecommunicationsGrid == null)
        {
            error = "That fax machine is not connected to a telecommunications grid.";
            return false;
        }

        if (!SwitchedOn || !IsPowered || string.IsNullOrWhiteSpace(PhoneNumber))
        {
            error = "That fax machine is not ready to send faxes right now.";
            return false;
        }

        if (document == null)
        {
            error = "You must specify a readable document to fax.";
            return false;
        }

        if (!document.Readables.Any())
        {
            error = "That document has nothing on it to fax.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(number))
        {
            error = "You must specify a number to fax.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public bool SendFax(ICharacter actor, string number, IReadable document, out string error)
    {
        if (!CanSendFax(actor, number, document, out error))
        {
            return false;
        }

        return TelecommunicationsGrid!.TrySendFax(this, number, document.Readables.ToList(), out error);
    }

    public void ReceiveFax(string senderNumber, IReadOnlyCollection<ICanBeRead> document)
    {
        if (!document.Any())
        {
            return;
        }

        IncomingFaxJob job = new(senderNumber, document.ToList());
        _pendingFaxes.Add(job);
        Parent.Handle(new EmoteOutput(new Emote("@ beep|beeps as it receives an incoming fax.", Parent)),
            OutputRange.Local);
        TryPrintPendingFaxes();
        if (job.RemainingReadables.Any())
        {
            Parent.Handle(new EmoteOutput(
                new Emote("@ store|stores the rest of the incoming fax in memory until it has more paper or ink.",
                    Parent)), OutputRange.Local);
        }
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Short ||
               type == DescriptionType.Full ||
               type == DescriptionType.Contents ||
               type == DescriptionType.Evaluate;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        switch (type)
        {
            case DescriptionType.Evaluate:
                return
                    $"It can hold {Gameworld.UnitManager.DescribeMostSignificantExact(_prototype.PaperWeightCapacity, Framework.Units.UnitType.Mass, voyeur).Colour(Telnet.Green)} of paper.";
            case DescriptionType.Short:
                return description;
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
                StringBuilder sb = new();
                sb.AppendLine(base.Decorate(voyeur, name, description, type, colour, flags));
                sb.AppendLine(
                    $"It is able to be opened and closed, and is currently {(IsOpen ? "open" : "closed")}."
                        .Colour(Telnet.Yellow));
                if (IsOpen || Transparent)
                {
                    sb.AppendLine(
                        $"It is {(_contents.Sum(x => x.Weight) / _prototype.PaperWeightCapacity).ToString("P2", voyeur).Colour(Telnet.Green)} full of paper.");
                }

                sb.AppendLine(
                    $"It is {((double)CurrentInkLevels / _prototype.MaximumCharactersPrintedPerCartridge).ToString("P2", voyeur).ColourValue()} full of ink.");
                sb.AppendLine(
                    $"It currently has {_pendingFaxes.Count.ToString("N0", voyeur).ColourValue()} pending incoming faxes stored in memory.");
                if (Locks.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine("It has the following locks:");
                    foreach (ILock thelock in Locks)
                    {
                        sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
                    }
                }

                sb.AppendLine(
                    $"You can use the {"fax <machine> <number> <document>".ColourCommand()} command to send readable documents with it.");
                sb.AppendLine(
                    $"You can use the {"refill fax".ColourCommand()} command to refill its ink cartridge.");
                return sb.ToString();
        }

        return base.Decorate(voyeur, name, description, type, colour, flags);
    }

    public override double ComponentWeight => _contents.Sum(x => x.Weight) + _locks.Sum(x => x.Parent.Weight);
    public override double ComponentBuoyancy(double fluidDensity)
    {
        return _contents.Sum(x => x.Buoyancy(fluidDensity)) + _locks.Sum(x => x.Parent.Buoyancy(fluidDensity));
    }

    public IEnumerable<IGameItem> Contents => _contents;
    public string ContentsPreposition => "in";
    public bool Transparent => false;

    public bool CanPut(IGameItem item)
    {
        return IsOpen &&
               item.IsItemType<PaperSheetGameItemComponent>() &&
               _contents.Sum(x => x.Weight) + item.Weight <= _prototype.PaperWeightCapacity;
    }

    public int CanPutAmount(IGameItem item)
    {
        return (int)((_prototype.PaperWeightCapacity - _contents.Sum(x => x.Weight)) / (item.Weight / item.Quantity));
    }

    public void Put(ICharacter? putter, IGameItem item, bool allowMerge = true)
    {
        if (_contents.Contains(item))
        {
#if DEBUG
            throw new ApplicationException("Item duplication in container.");
#else
			return;
#endif
        }

        if (allowMerge)
        {
            IGameItem? mergeTarget = _contents.FirstOrDefault(x => x.CanMerge(item));
            if (mergeTarget != null)
            {
                mergeTarget.Merge(item);
                item.Delete();
                TryPrintPendingFaxes();
                return;
            }
        }

        _contents.Add(item);
        item.ContainedIn = Parent;
        Changed = true;
        TryPrintPendingFaxes();
    }

    public WhyCannotPutReason WhyCannotPut(IGameItem item)
    {
        if (!IsOpen)
        {
            return WhyCannotPutReason.ContainerClosed;
        }

        if (!item.IsItemType<PaperSheetGameItemComponent>())
        {
            return WhyCannotPutReason.NotCorrectItemType;
        }

        if (_contents.Sum(x => x.Weight) + item.Weight > _prototype.PaperWeightCapacity)
        {
            double capacity = _prototype.PaperWeightCapacity - _contents.Sum(x => x.Weight);
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

        return !_contents.Contains(item)
            ? WhyCannotGetContainerReason.NotContained
            : WhyCannotGetContainerReason.NotContainer;
    }

    public override bool Take(IGameItem item)
    {
        if (Contents.Contains(item))
        {
            _contents.Remove(item);
            Changed = true;
            return true;
        }

        if (_locks.Any(x => x.Parent == item))
        {
            _locks.Remove(item.GetItemType<ILock>());
            item.ContainedIn = null;
            Changed = true;
            return true;
        }

        return base.Take(item);
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

        return base.SwapInPlace(existingItem, newItem);
    }

    public void Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)
    {
        ICell? location = emptier?.Location ?? Parent.TrueLocations.FirstOrDefault();
        List<IGameItem> contents = Contents.ToList();
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

        foreach (IGameItem? item in contents)
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
        return !IsOpen && Locks.All(x => !x.IsLocked);
    }

    public WhyCannotOpenReason WhyCannotOpen(IBody opener)
    {
        if (IsOpen)
        {
            return WhyCannotOpenReason.AlreadyOpen;
        }

        return Locks.Any(x => x.IsLocked) ? WhyCannotOpenReason.Locked : WhyCannotOpenReason.NotOpenable;
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
        return !IsOpen ? WhyCannotCloseReason.AlreadyClosed : WhyCannotCloseReason.NotOpenable;
    }

    public void Close()
    {
        IsOpen = false;
        OnClose?.Invoke(this);
    }

    public event OpenableEvent? OnOpen;
    public event OpenableEvent? OnClose;

    public IEnumerable<ILock> Locks => _locks;

    public bool InstallLock(ILock theLock, ICharacter? actor = null)
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

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        if (type == EventType.CommandInput && ((string)arguments[2]).Equals("refill"))
        {
            ICharacter ch = (ICharacter)arguments[0];
            StringStack ss = (StringStack)arguments[3];
            if (ss.IsFinished || !ss.Peek().EqualToAny("fax", "faxmachine"))
            {
                return false;
            }

            IGameItem? ink = ch.Body.HeldOrWieldedItems.FirstOrDefault(x => x.Prototype.Id == _prototype.InkCartridgePrototypeId);
            if (ink == null)
            {
                ch.OutputHandler.Send(
                    $"You are not holding an ink cartridge suitable for use in {Parent.HowSeen(ch)}.");
                return true;
            }

            ch.OutputHandler.Handle(new EmoteOutput(new Emote("@ refill|refills $1 with $2.", ch, ch, Parent,
                ink.PeekSplit(1))));
            if (ink.DropsWhole(1))
            {
                ink.Delete();
            }
            else
            {
                ink.GetItemType<IStackable>().Quantity -= 1;
            }

            if (_prototype.SpentInkCartridgePrototypeId != 0)
            {
                IGameItem? spent = Gameworld.ItemProtos.Get(_prototype.SpentInkCartridgePrototypeId)?.CreateNew(ch);
                if (spent != null)
                {
                    Gameworld.Add(spent);
                    if (ch.Body.CanGet(spent, 0))
                    {
                        ch.Body.Get(spent);
                    }
                    else
                    {
                        spent.RoomLayer = ch.RoomLayer;
                        ch.Location.Insert(spent);
                    }

                    spent.Login();
                    spent.HandleEvent(EventType.ItemFinishedLoading, spent);
                }
            }

            CurrentInkLevels = _prototype.MaximumCharactersPrintedPerCartridge;
            Changed = true;
            TryPrintPendingFaxes();
            return true;
        }

        foreach (IGameItem content in Contents)
        {
            if (content.HandleEvent(type, arguments))
            {
                return true;
            }
        }

        return base.HandleEvent(type, arguments);
    }

    public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
    {
        ILockable? newItemLockable = newItem?.GetItemType<ILockable>();
        if (newItemLockable != null)
        {
            foreach (ILock? thelock in Locks.ToList())
            {
                newItemLockable.InstallLock(thelock);
            }
        }
        else
        {
            foreach (ILock? thelock in Locks.ToList())
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

        IContainer? newItemContainer = newItem?.GetItemType<IContainer>();
        if (newItemContainer != null)
        {
            IOpenable? newItemOpenable = newItem!.GetItemType<IOpenable>();
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

            foreach (IGameItem? item in Contents.ToList())
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
        else
        {
            foreach (IGameItem? item in Contents.ToList())
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

        return base.HandleDieOrMorph(newItem!, location!);
    }

    private void TryPrintPendingFaxes()
    {
        if (!SwitchedOn || !IsPowered || !_pendingFaxes.Any())
        {
            return;
        }

        List<IGameItem> printedPages = new();
        bool ranOutOfPaper = false;
        bool ranOutOfInk = false;

        foreach (IncomingFaxJob? fax in _pendingFaxes.ToList())
        {
            IGameItem? currentSheet = null;
            IWriteable? currentWritable = default;
            bool sheetHasContent = false;

            while (fax.RemainingReadables.Any())
            {
                if (CurrentInkLevels <= 0)
                {
                    ranOutOfInk = true;
                    break;
                }

                if (currentSheet == null)
                {
                    currentSheet = TakeBlankSheet();
                    if (currentSheet == null)
                    {
                        ranOutOfPaper = true;
                        break;
                    }

                    currentWritable = currentSheet.GetItemType<IWriteable>();
                    sheetHasContent = false;
                }

                ICanBeRead readable = fax.RemainingReadables[0];
                bool added = readable switch
                {
                    MudSharp.Communication.Language.IWriting writing => currentWritable?.CanAddWriting(writing) == true &&
                                                                       currentWritable.AddWriting(writing),
                    IDrawing drawing => currentWritable?.CanAddDrawing(drawing) == true &&
                                        currentWritable.AddDrawing(drawing),
                    _ => false
                };

                if (!added)
                {
                    if (!sheetHasContent)
                    {
                        ranOutOfPaper = true;
                        break;
                    }

                    printedPages.Add(currentSheet);
                    currentSheet = null;
                    currentWritable = null;
                    sheetHasContent = false;
                    continue;
                }

                fax.RemainingReadables.RemoveAt(0);
                CurrentInkLevels = Math.Max(0, CurrentInkLevels - readable.DocumentLength);
                sheetHasContent = true;

                if (!fax.RemainingReadables.Any())
                {
                    break;
                }
            }

            if (currentSheet != null && sheetHasContent)
            {
                printedPages.Add(currentSheet);
            }
            else if (currentSheet != null)
            {
                ReturnUnusedSheet(currentSheet);
            }

            if (!fax.RemainingReadables.Any())
            {
                _pendingFaxes.Remove(fax);
            }

            if (ranOutOfPaper || ranOutOfInk)
            {
                break;
            }
        }

        if (printedPages.Any())
        {
            Parent.Handle(new EmoteOutput(new Emote("@ print|prints out $1.", Parent, Parent,
                new PerceivableGroup(printedPages))), OutputRange.Local);
            EjectPrintedPages(printedPages);
        }

        if (ranOutOfInk)
        {
            Parent.Handle(new EmoteOutput(
                new Emote("@ stop|stops printing because it has run out of ink.", Parent)), OutputRange.Local);
        }
        else if (ranOutOfPaper)
        {
            Parent.Handle(new EmoteOutput(
                new Emote("@ stop|stops printing because it has run out of paper.", Parent)), OutputRange.Local);
        }
    }

    private IGameItem? TakeBlankSheet()
    {
        IGameItem? paperItem = _contents.FirstOrDefault();
        if (paperItem == null)
        {
            return null;
        }

        if (paperItem.IsItemType<IStackable>() && paperItem.Quantity > 1)
        {
            return paperItem.Get(null, 1);
        }

        _contents.Remove(paperItem);
        paperItem.ContainedIn = null;
        Changed = true;
        return paperItem;
    }

    private void EjectPrintedPages(IEnumerable<IGameItem> pages)
    {
        ICell? location = Parent.TrueLocations.FirstOrDefault();
        foreach (IGameItem page in pages)
        {
            if (location != null)
            {
                page.RoomLayer = Parent.RoomLayer;
                location.Insert(page, true);
                continue;
            }

            if (Parent.InInventoryOf != null && Parent.InInventoryOf.CanGet(page, 0))
            {
                Parent.InInventoryOf.Get(page, silent: true);
                continue;
            }

            page.Delete();
        }
    }

    private void ReturnUnusedSheet(IGameItem sheet)
    {
        if (_contents.Contains(sheet))
        {
            return;
        }

        _contents.Insert(0, sheet);
        sheet.ContainedIn = Parent;
        Changed = true;
    }

    private sealed class IncomingFaxJob
    {
        public IncomingFaxJob(string senderNumber, List<ICanBeRead> remainingReadables)
        {
            SenderNumber = senderNumber;
            RemainingReadables = remainingReadables;
        }

        public string SenderNumber { get; }
        public List<ICanBeRead> RemainingReadables { get; }
    }
}
