using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Corpses;

namespace MudSharp_Unit_Tests.Health.Corpses;

[TestClass]
public class StandardCorpseModelTests
{
	private sealed class TestCorpseModel : StandardCorpseModel
	{
		public TestCorpseModel(MudSharp.Models.CorpseModel model, IFuturemud gameworld)
			: base(model, gameworld)
		{
		}
	}

	[TestMethod]
	public void Describe_WrappedInsertedDescription_DoesNotCapitaliseVisualContinuation()
	{
		const string definition = """
		                          <CorpseModel>
		                            <Ranges>
		                              <Range state="0" lower="0" upper="1" />
		                            </Ranges>
		                            <Terrains default="1" />
		                            <Descriptions>
		                              <ShortDescriptions><Description state="0">a corpse</Description></ShortDescriptions>
		                              <FullDescriptions><Description state="0">the remains are:
		                          @@desc</Description></FullDescriptions>
		                              <ContentsDescriptions><Description state="0">contents</Description></ContentsDescriptions>
		                              <PartDescriptions><Description state="0">part</Description></PartDescriptions>
		                            </Descriptions>
		                          </CorpseModel>
		                          """;
		var model = new TestCorpseModel(new MudSharp.Models.CorpseModel
		{
			Id = 1,
			Name = "Test Corpse Model",
			Description = "Test",
			Definition = definition
		}, new Mock<IFuturemud>().Object);
		var character = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		var voyeur = new Mock<IPerceiver>();
		character.Setup(x => x.ApparentGender(voyeur.Object)).Returns(Gendering.Get(Gender.Indeterminate));
		character.Setup(x => x.HowSeen(voyeur.Object, false, DescriptionType.Full, false,
			PerceiveIgnoreFlags.None))
			.Returns("The first sentence.\r\nordinary wrapped continuation remains lowercase.");
		body.Setup(x => x.VisibleWounds(voyeur.Object, WoundExaminationType.Look))
			.Returns(Array.Empty<IWound>());

		var result = model.Describe(DescriptionType.Full, DecayState.Fresh, character.Object, body.Object,
			voyeur.Object, 0.0);

		StringAssert.StartsWith(result, "The remains are:");
		StringAssert.Contains(result, "\r\nordinary wrapped continuation remains lowercase.");
		Assert.IsFalse(result.Contains("\r\nOrdinary wrapped continuation"));
	}
}
