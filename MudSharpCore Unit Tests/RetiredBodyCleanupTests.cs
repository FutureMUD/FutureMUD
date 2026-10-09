#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.Framework.Scheduling;
using RuntimeCharacter = MudSharp.Character.Character;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RetiredBodyCleanupTests
{
	private static RuntimeCharacter RetirementCharacter(IBody current, params IBody[] owned)
	{
		var character = (RuntimeCharacter)RuntimeHelpers.GetUninitializedObject(typeof(RuntimeCharacter));
		typeof(RuntimeCharacter).GetField("<Body>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(character, current);
		typeof(RuntimeCharacter).GetField("_forms", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(character, owned.Select(x => (ICharacterForm)new CharacterForm(x, $"body{x.Id}")).ToList());
		var sources = typeof(RuntimeCharacter).GetField("_formSources", BindingFlags.Instance | BindingFlags.NonPublic)!;
		sources.SetValue(character, Activator.CreateInstance(sources.FieldType));
		character.SetNoSave(true);
		return character;
	}

	private static Mock<IBody> RetirementBody(long id)
	{
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Id).Returns(id);
		return body;
	}

	private static void Retire(RuntimeCharacter character, IBody body) => typeof(RuntimeCharacter)
		.GetMethod("RetireBodyForm", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(character, [body]);

	private static Dictionary<long, DateTime>? Pending(RuntimeCharacter character) => (Dictionary<long, DateTime>?)
		typeof(RuntimeCharacter).GetField("_pendingBodyRetirements", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(character);

	[TestMethod]
	public void RetireBodyForm_BorrowedBodyWithSameActor_RefusesWithoutRemovingOwnedForms()
	{
		var current = RetirementBody(18);
		var borrowed = RetirementBody(17);
		var character = RetirementCharacter(current.Object, current.Object);
		borrowed.SetupGet(x => x.Actor).Returns(character);
		var exception = Assert.ThrowsException<TargetInvocationException>(() => Retire(character, borrowed.Object));
		Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
		Assert.IsNull(Pending(character));
		Assert.AreSame(current.Object, character.Forms.Single().Body);
	}

	[TestMethod]
	public void RetireBodyForm_CurrentOwnedBody_RefusesWithoutRecordingRetirement()
	{
		var current = RetirementBody(18);
		var character = RetirementCharacter(current.Object, current.Object);
		var exception = Assert.ThrowsException<TargetInvocationException>(() => Retire(character, current.Object));
		Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
		Assert.IsNull(Pending(character));
		Assert.AreSame(current.Object, character.Forms.Single().Body);
	}

	private sealed class RetirementTimeProvider : TimeProvider
	{
		public override DateTimeOffset GetUtcNow() => new(2026, 10, 2, 12, 0, 0, TimeSpan.Zero);
	}

	[TestMethod]
	public void RetireBodyForm_OwnedInactiveBody_RecordsExactUtcProvenanceBeforeRemovingForm()
	{
		using var clock = RuntimeClock.Push(new RetirementTimeProvider());
		var current = RetirementBody(18);
		var retired = RetirementBody(17);
		var character = RetirementCharacter(current.Object, current.Object, retired.Object);
		Retire(character, retired.Object);
		Assert.AreEqual(new DateTime(2026, 10, 2, 12, 0, 0, DateTimeKind.Utc), Pending(character)![17]);
		Assert.AreEqual(1, Pending(character)!.Count);
		Assert.AreSame(current.Object, character.Forms.Single().Body);
		Assert.AreSame(current.Object, character.CurrentBody);
	}

	[TestMethod]
	public void TryCleanupRetiredBody_ForeignPossession_RefusesBeforeAnyDestructiveOrDatabaseAction()
	{
		var character = (RuntimeCharacter)RuntimeHelpers.GetUninitializedObject(typeof(RuntimeCharacter));
		var item = new Mock<IGameItem>(MockBehavior.Strict);
		var body = new Mock<IBody>(MockBehavior.Strict);
		body.SetupGet(x => x.AllItems).Returns([item.Object]);
		Assert.IsFalse(character.TryCleanupRetiredBody(body.Object));
		item.VerifyNoOtherCalls();
		body.VerifyGet(x => x.AllItems, Times.Once);
		body.VerifyNoOtherCalls();
	}

}
