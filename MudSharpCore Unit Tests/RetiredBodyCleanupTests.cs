#nullable enable

using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems;
using RuntimeCharacter = MudSharp.Character.Character;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RetiredBodyCleanupTests
{
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

	[DataTestMethod]
	[DataRow("<Definition><OriginalBody>17</OriginalBody></Definition>", true)]
	[DataRow("<Definition><OriginalBodyId>17</OriginalBodyId></Definition>", true)]
	[DataRow("<Effects><Effect><BackupBodyId>17</BackupBodyId></Effect></Effects>", true)]
	[DataRow("<Effect><SourceBody>17</SourceBody></Effect>", true)]
	[DataRow("<Definition bodyId='17'/>", true)]
	[DataRow("<Definition><OriginalBody>171</OriginalBody></Definition>", false)]
	[DataRow("<Definition><OriginalCharacter>17</OriginalCharacter></Definition>", false)]
	[DataRow("<Definition><BodyDescription>a seventeenth body</BodyDescription></Definition>", false)]
	[DataRow("<Definition><Bodypart>17</Bodypart><OverridenBodypart>17</OverridenBodypart></Definition>", false)]
	[DataRow("<Definition><BodyPrototypeId>17</BodyPrototypeId></Definition>", false)]
	[DataRow("<Definition>", true)]
	[DataRow("", false)]
	public void HasReferenceOrUncertainty_SerializedDependency_PreservesBody(string xml, bool expected)
	{
		Assert.AreEqual(expected, RetiredBodyReferencePolicy.HasReferenceOrUncertainty(xml, 17));
	}
}
