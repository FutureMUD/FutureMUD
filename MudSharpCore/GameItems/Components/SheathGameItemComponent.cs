using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

#nullable enable annotations

namespace MudSharp.GameItems.Components;

public class SheathGameItemComponent : GameItemComponent, IMultiSlotSheath, IContainer
{
    protected SheathGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    public override void Delete()
    {
        base.Delete();
        foreach (var content in _contents.ToList())
        {
            content.Parent.Delete();
        }
    }

    public override void Quit()
    {
        base.Quit();
        foreach (var content in _contents)
        {
            content.Parent.Quit();
        }
    }

    public override void Login()
    {
        foreach (var content in _contents)
        {
            content.Parent.Login();
        }
    }

    public override bool Take(IGameItem item)
    {
        var content = _contents.FirstOrDefault(x => x.Parent == item);
        if (content is not null)
        {
            _contents.Remove(content);
            Changed = true;
            return true;
        }

        return false;
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new SheathGameItemComponent(this, newParent, temporary);
    }

    public override bool PreventsMerging(IGameItemComponent component)
    {
        return _contents.Count > 0;
    }

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Contents || type == DescriptionType.Short;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        switch (type)
        {
            case DescriptionType.Short:
                return
                    $"{description}{(_contents.Count > 0 ? $" bearing {_contents.Select(x => x.Parent.Name.ToLowerInvariant().A_An(colour: Telnet.Green)).ListToString()}" : "")}";
            case DescriptionType.Contents:
                return
                    $"{description}{(_contents.Count > 0 ? $"\n\nIt contains {_contents.Select(x => x.Parent.HowSeen(voyeur)).ListToString()}." : "\n\nIt is currently empty.")}";
        }

        return description;
    }

    public override int DecorationPriority => -1;
    public override double ComponentWeight => _contents.Sum(x => x.Parent.Weight);

    public override double ComponentBuoyancy(double fluidDensity)
    {
        return _contents.Sum(x => x.Parent.Buoyancy(fluidDensity));
    }

    public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
    {
        var content = _contents.FirstOrDefault(x => x == existingItem.GetItemType<IWieldable>());
        if (content is not null)
        {
            _contents.Remove(content);
            TryAdd(newItem.GetItemType<IWieldable>());
            return true;
        }

        return false;
    }

    public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
    {
        if (_contents.Count == 0)
        {
            return false;
        }

        ISheath newItemSheath = newItem?.GetItemType<ISheath>();
        if (newItemSheath != null && _contents.All(x => newItemSheath.CanSheath(x.Parent)))
        {
            foreach (var content in _contents.ToList())
            {
                if (newItemSheath is IMultiSlotSheath multiSlotSheath)
                {
                    multiSlotSheath.TryAdd(content);
                }
                else
                {
                    newItemSheath.Content = content;
                }
            }

            _contents.Clear();
        }
        else
        {
            if (location != null)
            {
                foreach (var content in _contents.ToList())
                {
                    InsertAtParentSpatialLocation(content.Parent, location);
                    content.Parent.ContainedIn = null;
                }

                _contents.Clear();
            }
            else
            {
                foreach (var content in _contents)
                {
                    content.Parent.Delete();
                }

                _contents.Clear();
            }
        }

        return false;
    }

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (SheathGameItemComponentProto)newProto;
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Contents", _contents.Select(x => new XElement("Content", x.Parent.Id)))
        ).ToString();
    }

    #region ISheath Members

    public SizeCategory MaximumSize => _prototype.MaximumSize;

    private readonly List<IWieldable> _contents = new();

    public int Capacity => _prototype.Capacity;

    public IEnumerable<IWieldable> WieldableContents => _contents;

    public IWieldable Content
    {
        get => _contents.FirstOrDefault();
        set
        {
            foreach (var existing in _contents.Where(x => x != value).ToList())
            {
                existing.Parent.ContainedIn = null;
            }

            _contents.Clear();
            if (value != null)
            {
                TryAdd(value);
            }

            Changed = true;
        }
    }

    public bool TryAdd(IWieldable content)
    {
        if (content is null || _contents.Contains(content) || _contents.Count >= Capacity)
        {
            return false;
        }

        _contents.Add(content);
        if (_noSave)
        {
            content.Parent.LoadTimeSetContainedIn(Parent);
        }
        else
        {
            content.Parent.ContainedIn = Parent;
        }

        Changed = true;
        return true;
    }

    public bool TryRemove(IWieldable content)
    {
        if (!_contents.Remove(content))
        {
            return false;
        }

        content.Parent.ContainedIn = null;
        Changed = true;
        return true;
    }

    public Difficulty StealthDrawDifficulty => _prototype.StealthDrawDifficulty;

    public bool DesignedForGuns => _prototype.DesignedForGuns;

    public bool CanSheath(IGameItem item)
    {
        if (_contents.Count >= Capacity)
        {
            return false;
        }

        if (!item.IsItemType<IWieldable>())
        {
            return false;
        }

        if (MaximumSize < item.Size)
        {
            return false;
        }

        IRangedWeapon rw = item.GetItemType<IRangedWeapon>();
        if (DesignedForGuns)
        {
            if (rw?.WeaponType.RangedWeaponType.IsFirearm() != true)
            {
                return false;
            }

            return true;
        }

        return true;
    }

    public string WhyCannotSheath(IGameItem item)
    {
        if (_contents.Count >= Capacity)
        {
            return "the sheathe already has something in it";
        }

        if (!item.IsItemType<IWieldable>())
        {
            return "that is not a wieldable item";
        }

        if (MaximumSize < item.Size)
        {
            return "that is too large to fit in that sheathe";
        }

        IRangedWeapon rw = item.GetItemType<IRangedWeapon>();
        if (DesignedForGuns)
        {
            if (rw?.WeaponType.RangedWeaponType.IsFirearm() != true)
            {
                return "only firearms can be sheathed in that sheathe";
            }
        }

        return "an unknown reason";
    }

    #endregion

    #region Constructors

    public SheathGameItemComponent(SheathGameItemComponentProto proto, IGameItem parent, bool temporary = false)
        : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public SheathGameItemComponent(MudSharp.Models.GameItemComponent component, SheathGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    private void LoadFromXml(XElement root)
    {
        var contents = root.Element("Contents")?.Elements("Content") ?? root.Elements("Content");
        foreach (var content in contents)
        {
            if (string.IsNullOrEmpty(content.Value))
            {
                continue;
            }

            IGameItem contentItem = Gameworld.TryGetItem(long.Parse(content.Value), true);
            contentItem.Get(null);
            IWieldable wieldable = contentItem.GetItemType<IWieldable>();
            if (wieldable != null)
            {
                TryAdd(wieldable);
            }
            else
            {
                Console.WriteLine("Warning: sheath content was not wieldable.");
            }
        }
    }

    public SheathGameItemComponent(SheathGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
        newParent, temporary)
    {
        _prototype = rhs._prototype;
    }

    public override void FinaliseLoad()
    {
        foreach (var content in _contents)
        {
            content.Parent.FinaliseLoadTimeTasks();
        }
    }

    #endregion

    #region Implementation of IContainer

    /// <inheritdoc />
    public IEnumerable<IGameItem> Contents => _contents.Select(x => x.Parent);

    /// <inheritdoc />
    public string ContentsPreposition => "in";

    /// <inheritdoc />
    public bool Transparent => false;

    /// <inheritdoc />
    public bool CanPut(IGameItem item)
    {
        return false;
    }

    /// <inheritdoc />
    public void Put(ICharacter? putter, IGameItem item, bool allowMerge = true)
    {
        // Do nothing
    }

    /// <inheritdoc />
    public WhyCannotPutReason WhyCannotPut(IGameItem item)
    {
        return WhyCannotPutReason.NotContainer;
    }

    /// <inheritdoc />
    public bool CanTake(ICharacter taker, IGameItem item, int quantity)
    {
        return _contents.Any(x => x.Parent == item);
    }

    /// <inheritdoc />
    public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
    {
        TryRemove(item.GetItemType<IWieldable>()!);
        return item;
    }

    /// <inheritdoc />
    public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
    {
        return WhyCannotGetContainerReason.NotContained;
    }

    /// <inheritdoc />
    public int CanPutAmount(IGameItem item)
    {
        return 0;
    }

    /// <inheritdoc />
    public void Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)
    {
        ICell location = emptier?.Location ?? Parent.TrueLocations.FirstOrDefault();
        List<IGameItem> contents = Contents.ToList();
        foreach (var content in _contents.ToList())
        {
            TryRemove(content);
        }
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

        foreach (IGameItem item in contents)
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
                    InsertAtParentSpatialLocation(item, location, preferredSource: emptier);
                    emptier?.OutputHandler.Handle(new EmoteOutput(new Emote(
                            "@ cannot put $1 into $2, so #0 set|sets it down on the ground.", emptier, emptier, item,
                            intoContainer.Parent)));
                }
                else
                {
                    item.Delete();
                }

                continue;
            }

            if (location != null)
            {
                InsertAtParentSpatialLocation(item, location, preferredSource: emptier);
            }
            else
            {
                item.Delete();
            }
        }

        Changed = true;
    }

    #endregion
}
