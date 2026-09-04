using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

#nullable enable
#nullable disable warnings

namespace MudSharp.Framework;

public class Tag : SaveableItem, ILoadingTag
{
    private ITag _parent;

    public Tag(MudSharp.Models.Tag tag, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _id = tag.Id;
        _name = tag.Name;
        ShouldSeeProg = Gameworld.FutureProgs.Get(tag.ShouldSeeProgId ?? 0);
    }

    public Tag(string name, ITag parent, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _name = name;
        _parent = parent;

        using (new FMDB())
        {
            Models.Tag dbitem = new();
            FMDB.Context.Tags.Add(dbitem);
            dbitem.Name = name;
            dbitem.ParentId = parent?.Id;
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }

    public ITag Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            Changed = true;
        }
    }

    public bool IsA(ITag? otherTag)
    {
        return otherTag == this || _parent == otherTag || (_parent?.IsA(otherTag) ?? false);
    }

    public string FullName
    {
        get
        {
            if (Parent == null)
            {
                return Name;
            }

            return $"{Parent.FullName} / {Name}";
        }
    }

    public IFutureProg ShouldSeeProg { get; set; }

    public bool ShouldSee(ICharacter actor)
    {
        return ShouldSeeProg?.ExecuteBool(actor) != false;
    }

    public IEditableTag GetEditable => this;

    void ILoadingTag.FinaliseLoad(MudSharp.Models.Tag tag)
    {
        _parent = Gameworld.Tags.FirstOrDefault(x => x.Id == tag.ParentId);
    }

    public override void Save()
    {
        using (new FMDB())
        {
            Models.Tag dbitem = FMDB.Context.Tags.Find(Id);
            dbitem.Name = _name;
            dbitem.ParentId = Parent?.Id;
            dbitem.ShouldSeeProgId = ShouldSeeProg?.Id;
            FMDB.Context.SaveChanges();
        }

        Changed = false;
    }

    public override string FrameworkItemType => "Tag";

    public ProgVariableTypes Type => ProgVariableTypes.Tag;
    public object GetObject => this;

    public IProgVariable GetProperty(string property)
    {
        return property.ToLowerInvariant() switch
        {
            "id" => new NumberVariable(Id),
            "name" => new TextVariable(Name),
            "fullname" => new TextVariable(FullName),
            "parent" => Parent is IProgVariable parent ? parent : new NullVariable(ProgVariableTypes.Tag),
            _ => throw new NotSupportedException($"Unsupported tag property {property}.")
        };
    }

    public static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Tag,
            new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["id"] = ProgVariableTypes.Number,
                ["name"] = ProgVariableTypes.Text,
                ["fullname"] = ProgVariableTypes.Text,
                ["parent"] = ProgVariableTypes.Tag
            },
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["id"] = "The stable identity of this tag.",
                ["name"] = "The tag name without its parent hierarchy.",
                ["fullname"] = "The complete parent-qualified tag name.",
                ["parent"] = "The immediate parent tag, or null for a root tag."
            });
    }

    #region IEditableTag Members

    public void SetName(string name)
    {
        _name = name;
        Changed = true;
    }

    public void Delete()
    {
        using (new FMDB())
        {
            Gameworld.SaveManager.Flush();
            Models.Tag dbitem = FMDB.Context.Tags.Find(Id);
            FMDB.Context.Tags.Remove(dbitem);
            FMDB.Context.SaveChanges();
        }

        Changed = false;
        Gameworld.SaveManager.Abort(this);
        Gameworld.Destroy(this);
    }

    #endregion
}
