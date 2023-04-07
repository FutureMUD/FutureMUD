using System.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

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
			var dbitem = new Models.Tag();
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

	public bool IsA(ITag otherTag)
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
		return (bool?)ShouldSeeProg?.Execute(actor) != false;
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
			var dbitem = FMDB.Context.Tags.Find(Id);
			dbitem.Name = _name;
			dbitem.ParentId = Parent?.Id;
			dbitem.ShouldSeeProgId = ShouldSeeProg?.Id;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public override string FrameworkItemType => "Tag";

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
			var dbitem = FMDB.Context.Tags.Find(Id);
			FMDB.Context.Tags.Remove(dbitem);
			FMDB.Context.SaveChanges();
		}

		Changed = false;
		Gameworld.SaveManager.Abort(this);
		Gameworld.Destroy(this);
	}

	#endregion
}