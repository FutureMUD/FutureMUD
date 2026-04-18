using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SaveNoSaveTests
{
    [TestMethod]
    public void SetNoSave_SaveableItemAlreadyQueued_RemovesItFromSaveManager()
    {
        SaveManager saveManager = new();
        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.SaveManager).Returns(saveManager);
        TestSaveableItem item = new(gameworld.Object);

        item.Changed = true;
        Assert.IsTrue(saveManager.IsQueued(item));

        item.SetNoSave(true);

        Assert.IsTrue(item.GetNoSave());
        Assert.IsFalse(item.Changed);
        Assert.IsFalse(saveManager.IsQueued(item));
    }

    [TestMethod]
    public void InitialiseItem_NoSave_DoesNotInsertOrForceAnId()
    {
        SaveManager saveManager = new();
        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.SaveManager).Returns(saveManager);
        TestLateInitialisingItem item = new(gameworld.Object);

        saveManager.AddInitialisation(item);
        Assert.IsTrue(saveManager.IsQueued(item));

        item.SetNoSave(true);
        Action action = item.InitialiseItem();
        action();

        Assert.IsFalse(saveManager.IsQueued(item));
        Assert.AreEqual(0, item.DatabaseInsertCalls);
        Assert.AreEqual(0L, item.Id);
        Assert.IsFalse(item.IdHasBeenRegistered);
    }

    private sealed class TestSaveableItem : SaveableItem
    {
        public TestSaveableItem(IFuturemud gameworld)
        {
            Gameworld = gameworld;
            _name = "Test Saveable";
        }

        public override string FrameworkItemType => "TestSaveable";

        public override void Save()
        {
            Changed = false;
        }
    }

    private sealed class TestLateInitialisingItem : LateInitialisingItem
    {
        public TestLateInitialisingItem(IFuturemud gameworld)
        {
            Gameworld = gameworld;
            _name = "Test Late";
        }

        public int DatabaseInsertCalls { get; private set; }
        public override string FrameworkItemType => "TestLateInitialising";

        public override void Save()
        {
            Changed = false;
        }

        public override object DatabaseInsert()
        {
            DatabaseInsertCalls++;
            return new object();
        }

        public override void SetIDFromDatabase(object dbitem)
        {
            _id = 42;
        }
    }
}
