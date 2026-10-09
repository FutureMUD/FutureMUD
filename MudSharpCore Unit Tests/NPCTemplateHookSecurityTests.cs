using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class NPCTemplateHookSecurityTests
{
	private static (NPCTemplateBase Template, Mock<IFuturemud> World, IHook Hook) Fixture()
	{
		var hook = new Mock<IHook>();
		hook.SetupGet(x => x.Id).Returns(17);
		hook.SetupGet(x => x.Name).Returns("command-intercept");
		hook.SetupGet(x => x.Type).Returns(EventType.CommandInput);
		var hooks = new All<IHook>();
		hooks.Add(hook.Object);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Hooks).Returns(hooks);
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		var record = new MudSharp.Models.NpcTemplate
		{
			Id = 18, Name = "guard", Definition = "<Definition/>",
			EditableItem = new MudSharp.Models.EditableItem
			{
				BuilderDate = DateTime.UtcNow, RevisionStatus = (int)RevisionStatus.UnderDesign
			}
		};
		var template = new Mock<NPCTemplateBase>(record, world.Object) { CallBase = true };
		return (template.Object, world, hook.Object);
	}

	private static Mock<ICharacter> Actor(PermissionLevel permission)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.PermissionLevel).Returns(permission);
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		return actor;
	}

	private static List<long> HookIds(NPCTemplateBase template) =>
		(List<long>)typeof(NPCTemplateBase).GetField("_templateHookIds", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(template)!;

	[TestMethod]
	[DataRow(PermissionLevel.JuniorAdmin, "add")]
	[DataRow(PermissionLevel.Admin, "add")]
	[DataRow(PermissionLevel.JuniorAdmin, "remove")]
	[DataRow(PermissionLevel.Admin, "remove")]
	public void BuildingCommandHook_BelowSeniorAdmin_DeniesBeforeHookLookup(PermissionLevel permission, string action)
	{
		var (template, world, hook) = Fixture();
		if (action == "remove")
		{
			HookIds(template).Add(hook.Id);
		}

		var before = HookIds(template).ToArray();
		Assert.IsFalse(template.BuildingCommand(Actor(permission).Object, new StringStack($"hook {action} 17")));
		CollectionAssert.AreEqual(before, HookIds(template));
		Assert.IsFalse(template.Changed);
		world.VerifyGet(x => x.Hooks, Times.Never);
	}

	[TestMethod]
	[DataRow(PermissionLevel.SeniorAdmin)]
	[DataRow(PermissionLevel.HighAdmin)]
	[DataRow(PermissionLevel.Founder)]
	public void BuildingCommandHook_SeniorAdminOrHigher_CanAddAndRemove(PermissionLevel permission)
	{
		var (template, _, hook) = Fixture();
		var actor = Actor(permission).Object;
		Assert.IsTrue(template.BuildingCommand(actor, new StringStack("hook add 17")));
		CollectionAssert.AreEqual(new[] { hook.Id }, HookIds(template));
		Assert.IsTrue(template.Changed);
		Assert.IsTrue(template.BuildingCommand(actor, new StringStack("hook remove 17")));
		Assert.AreEqual(0, HookIds(template).Count);
	}
}
