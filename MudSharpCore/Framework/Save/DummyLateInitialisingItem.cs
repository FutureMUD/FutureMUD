using System;

namespace MudSharp.Framework.Save;

public class DummyLateInitialisingItem : LateInitialisingItem
{
	public DummyLateInitialisingItem(IFuturemud gameworld, Func<object> databaseInsertAction)
	{
		Gameworld = gameworld;
		DatabaseInsertAction = databaseInsertAction;
	}

	public Func<object> DatabaseInsertAction { get; set; }

	public override string FrameworkItemType => "DummyLateInitialisingItem";

	public override void Save()
	{
		// Do nothing
	}

	public override object DatabaseInsert()
	{
		return DatabaseInsertAction();
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		// Do nothing
	}
}