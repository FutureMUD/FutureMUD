using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Save;

#nullable enable

namespace MudSharp.Communication;

public sealed class WritingCollectionEntry : IWritingCollectionEntry
{
	public WritingCollectionEntry(int page, int order, ICanBeRead readable)
	{
		Page = page;
		Order = order;
		Readable = readable;
	}

	public int Page { get; set; }
	public int Order { get; set; }
	public ICanBeRead Readable { get; }
}

public sealed class WritingCollection : LateInitialisingItem, IWritingCollection
{
	private readonly List<WritingCollectionEntry> _entries = new();

	public WritingCollection(string name, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_name = name;
		Description = string.Empty;
		DefaultTitle = string.Empty;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public WritingCollection(MudSharp.Models.WritingCollection dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		IdInitialised = true;
		_name = dbitem.Name;
		Description = dbitem.Description ?? string.Empty;
		DefaultTitle = dbitem.DefaultTitle ?? string.Empty;
		foreach (var item in dbitem.WritingCollectionEntries.OrderBy(x => x.PageNumber).ThenBy(x => x.DisplayOrder))
		{
			ICanBeRead? readable = item.WritingId.HasValue
				? gameworld.Writings.Get(item.WritingId.Value)
				: item.DrawingId.HasValue
					? gameworld.Drawings.Get(item.DrawingId.Value)
					: null;
			if (readable is null)
			{
				continue;
			}

			_entries.Add(new WritingCollectionEntry(item.PageNumber, item.DisplayOrder, readable));
		}
	}

	public override string FrameworkItemType => "WritingCollection";
	public string Description { get; private set; }
	public string DefaultTitle { get; private set; }
	public IEnumerable<IWritingCollectionEntry> Entries => _entries.OrderBy(x => x.Page).ThenBy(x => x.Order);
	public int PageCount => _entries.Select(x => x.Page).DefaultIfEmpty(0).Max();

	public int DocumentLengthForPage(int page)
	{
		return _entries.Where(x => x.Page == page).Sum(x => x.Readable.DocumentLength);
	}

	public void Rename(string name)
	{
		_name = name;
		Changed = true;
	}

	public void SetDescription(string description)
	{
		Description = description;
		Changed = true;
	}

	public void SetDefaultTitle(string title)
	{
		DefaultTitle = title;
		Changed = true;
	}

	public WritingCollectionEntry AddEntry(int page, ICanBeRead readable)
	{
		if (page < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(page), "Writing collection pages must be positive.");
		}

		var entry = new WritingCollectionEntry(page, NextOrder(page), readable);
		_entries.Add(entry);
		Changed = true;
		return entry;
	}

	public bool RemoveEntry(int index)
	{
		var entry = Entries.ElementAtOrDefault(index - 1) as WritingCollectionEntry;
		if (entry is null)
		{
			return false;
		}

		_entries.Remove(entry);
		ReorderPage(entry.Page);
		Changed = true;
		return true;
	}

	public bool MoveEntry(int index, int page, int? order = null)
	{
		if (page < 1)
		{
			return false;
		}

		var entry = Entries.ElementAtOrDefault(index - 1) as WritingCollectionEntry;
		if (entry is null)
		{
			return false;
		}

		var oldPage = entry.Page;
		entry.Page = page;
		entry.Order = order ?? NextOrder(page);
		ReorderPage(oldPage);
		ReorderPage(page);
		Changed = true;
		return true;
	}

	public void ClearEntries()
	{
		_entries.Clear();
		Changed = true;
	}

	public void ReplaceEntries(IEnumerable<(int Page, ICanBeRead Readable)> entries)
	{
		_entries.Clear();
		foreach (var entry in entries.OrderBy(x => x.Page))
		{
			AddEntry(entry.Page, entry.Readable);
		}
		Changed = true;
	}

	private int NextOrder(int page)
	{
		return _entries.Where(x => x.Page == page).Select(x => x.Order).DefaultIfEmpty(0).Max() + 1;
	}

	private void ReorderPage(int page)
	{
		var order = 1;
		foreach (var entry in _entries.Where(x => x.Page == page).OrderBy(x => x.Order))
		{
			entry.Order = order++;
		}
	}

	public override object DatabaseInsert()
	{
		var dbitem = new MudSharp.Models.WritingCollection
		{
			Name = Name,
			Description = Description,
			DefaultTitle = DefaultTitle
		};
		foreach (var entry in Entries)
		{
			dbitem.WritingCollectionEntries.Add(CreateDatabaseEntry(entry));
		}

		FMDB.Context.WritingCollections.Add(dbitem);
		return dbitem;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.WritingCollections
		                   .Include(x => x.WritingCollectionEntries)
		                   .First(x => x.Id == Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.DefaultTitle = DefaultTitle;
		FMDB.Context.WritingCollectionEntries.RemoveRange(dbitem.WritingCollectionEntries);
		dbitem.WritingCollectionEntries.Clear();
		foreach (var entry in Entries)
		{
			dbitem.WritingCollectionEntries.Add(CreateDatabaseEntry(entry));
		}

		Changed = false;
	}

	private MudSharp.Models.WritingCollectionEntry CreateDatabaseEntry(IWritingCollectionEntry entry)
	{
		return new MudSharp.Models.WritingCollectionEntry
		{
			PageNumber = entry.Page,
			DisplayOrder = entry.Order,
			WritingId = entry.Readable is Language.IWriting writing ? (long?)writing.Id : null,
			DrawingId = entry.Readable is IDrawing drawing ? (long?)drawing.Id : null
		};
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((MudSharp.Models.WritingCollection)dbitem).Id;
	}
}