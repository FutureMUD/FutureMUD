#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuntimeCharacter = MudSharp.Character.Character;
using MudSharp.Framework;
using MudSharp.Movement;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacterEventCensusTests
{
	[TestMethod]
	public void AppendEventSubscriptionReport_SubscribeRemove_DoesNotFireCallbacksOrLoadState()
	{
		// No world/body exists: a census must read delegates, never resolve character state.
		var character = (RuntimeCharacter)RuntimeHelpers.GetUninitializedObject(typeof(RuntimeCharacter));
		PerceivableEvent callback = _ => Assert.Fail("Census fired an event");
		EventHandler<MoveEventArgs> move = (_, _) => Assert.Fail("Census fired a movement event");
		character.OnQuit += callback;
		character.OnDeleted += callback;
		character.OnJoinCombat += callback;
		character.OnDeath += callback;
		character.OnStartMove += move;
		character.OnStopMove += move;
		var sb = new StringBuilder();
		character.AppendEventSubscriptionReport(sb);
		foreach (var name in new[] { "Quit", "Deleted", "Join combat", "Death", "Start move", "Stop move" })
		{
			StringAssert.Contains(sb.ToString(), $"{name} subscriptions: 1");
		}
		character.OnQuit -= callback;
		character.OnDeleted -= callback;
		character.OnJoinCombat -= callback;
		character.OnDeath -= callback;
		character.OnStartMove -= move;
		character.OnStopMove -= move;
		sb.Clear();
		character.AppendEventSubscriptionReport(sb);
		foreach (var name in new[] { "Quit", "Deleted", "Join combat", "Death", "Start move", "Stop move" })
		{
			StringAssert.Contains(sb.ToString(), $"{name} subscriptions: 0");
		}
	}
}
