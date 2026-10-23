#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Magic;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicSchoolHierarchyTests
{
	[TestMethod]
	public void ParentCommand_RejectsSelfAndDescendantsButAllowsReparentingAndClearing()
	{
		var f = new Fixture();
		var root = f.Add(1);
		var child = f.Add(2, 1);
		var leaf = f.Add(3, 2);
		var other = f.Add(4);
		Assert.IsFalse(root.BuildingCommand(f.Actor, new StringStack("parent 1")));
		Assert.IsFalse(root.BuildingCommand(f.Actor, new StringStack("parent 3")));
		Assert.IsNull(root.ParentSchool);
		Assert.IsFalse(root.Changed);
		Assert.IsTrue(leaf.IsChildSchool(root));
		Assert.IsFalse(root.IsChildSchool(leaf));
		Assert.IsTrue(child.BuildingCommand(f.Actor, new StringStack("parent 4")));
		Assert.IsTrue(leaf.IsChildSchool(other));
		Assert.IsFalse(leaf.IsChildSchool(root));
		Assert.IsTrue(child.BuildingCommand(f.Actor, new StringStack("parent none")));
		Assert.IsNull(child.ParentSchool);
		Assert.IsFalse(leaf.IsChildSchool(other));
	}

	[TestMethod]
	[Timeout(5000)]
	public void ParentCommand_CorruptLoadedAncestryTerminatesAndCannotBeInherited()
	{
		var f = new Fixture();
		var a = f.Add(1, 2);
		f.Add(2, 1);
		var branch = f.Add(3, 1);
		var unrelated = f.Add(4);
		Assert.IsFalse(a.IsChildSchool(unrelated));
		Assert.IsFalse(branch.IsChildSchool(unrelated));
		Assert.IsFalse(unrelated.BuildingCommand(f.Actor, new StringStack("parent 3")));
		Assert.IsNull(unrelated.ParentSchool);
		Assert.IsTrue(a.BuildingCommand(f.Actor, new StringStack("parent none")));
		Assert.IsFalse(a.IsChildSchool(branch));
	}

	private sealed class Fixture
	{
		private readonly Mock<IFuturemud> _world = new();
		private readonly All<IMagicSchool> _schools = new();
		public ICharacter Actor { get; } = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<IOutputHandler>());

		public Fixture()
		{
			_world.SetupGet(x => x.MagicSchools).Returns(_schools);
			_world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		}

		public MagicSchool Add(long id, long? parent = null)
		{
			var school = new MagicSchool(new MudSharp.Models.MagicSchool
			{
				Id = id, Name = $"School{id}", ParentSchoolId = parent,
				SchoolVerb = "invoke", SchoolAdjective = "magical", PowerListColour = "cyan"
			}, _world.Object);
			_schools.Add(school);
			return school;
		}
	}
}
