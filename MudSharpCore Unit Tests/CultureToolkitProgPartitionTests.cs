#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Knowledge;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitProgPartitionTests
{
	[TestMethod]
	public void SplitProgsRespectMysqlBytesAndExecuteMaximumAndDeduplicatedKnowledgeUnion()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var padding = "// " + new string('x', 30000) + "\n";
		var conflicts = new List<string>();
		var maximum = CultureToolkitProgPartitions.Upsert(context, "medieval", "fixture.maximum", "FixtureMaximum", "",
			new[] { padding + "return 0.5\n", padding + "return 0.9\n" }, "return 0", ProgVariableTypes.Number, [], conflicts);
		var union = CultureToolkitProgPartitions.Upsert(context, "medieval", "fixture.union", "FixtureUnion", "var knowledges as knowledge collection\n",
			new[] { padding + "additem knowledges ToKnowledge(1)\n", padding + "additem knowledges ToKnowledge(1)\n" },
			"return @knowledges", ProgVariableTypes.Knowledge | ProgVariableTypes.Collection, [], conflicts, "knowledges");
		Assert.AreEqual(2, maximum.SupportingIds.Count);
		Assert.AreEqual(2, union.SupportingIds.Count);
		Assert.IsTrue(context.FutureProgs.AsEnumerable().All(x => Encoding.UTF8.GetByteCount(x.FunctionText) <= 65535));
		MudSharp.FutureProg.FutureProg.Initialise();
		var world = new Mock<IFuturemud>();
		var progs = new All<IFutureProg>();
		var knowledges = new All<IKnowledge>();
		var knowledge = new Mock<IKnowledge>();
		knowledge.SetupGet(x => x.Id).Returns(1);
		knowledge.SetupGet(x => x.Type).Returns(ProgVariableTypes.Knowledge);
		knowledge.SetupGet(x => x.GetObject).Returns(knowledge.Object);
		knowledges.Add(knowledge.Object);
		world.SetupGet(x => x.Knowledges).Returns(knowledges);
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		foreach (var definition in context.FutureProgs.Include(x => x.FutureProgsParameters))
			progs.Add(new MudSharp.FutureProg.FutureProg(definition, world.Object));
		foreach (var prog in progs) Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.AreEqual(0.9, progs.Get(maximum.Main.Id)!.ExecuteDouble(), 0.0001);
		Assert.AreEqual(1, progs.Get(union.Main.Id)!.ExecuteCollection<IKnowledge>().Count());
		Assert.AreEqual(0, conflicts.Count);
	}
}
