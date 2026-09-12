using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.Magic;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using MudSharp.Magic.Vancian;
using Prog = MudSharp.FutureProg.FutureProg;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class VancianExampleProgTests
{
	public sealed record Example(string Name, string Kind, string[] Parameters, string Body);
	internal static string RepositoryRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName,"Design Documents"))) directory = directory.Parent;
		return directory?.FullName ?? throw new InvalidOperationException("Could not find the repository documentation.");
	}
	[TestMethod]
	public void EveryDocumentedPolicyCompilesAndExecutesItsDeclaredContract()
	{
		FutureProgTestBootstrap.EnsureInitialised(); var f = new VancianTestFixture(); var register = new Mock<IVariableRegister>();
		f.World.SetupGet(x => x.VariableRegister).Returns(register.Object);
		register.Setup(x => x.GetType(ProgVariableTypes.Character,"wizard_known_changed")).Returns(ProgVariableTypes.DateTime);
		register.Setup(x => x.GetType(ProgVariableTypes.Character,"vancian_level")).Returns(ProgVariableTypes.Number);
		register.Setup(x => x.GetType(ProgVariableTypes.Character,"vancian_progression_allowed")).Returns(ProgVariableTypes.Boolean);
		var changed = DateTime.UtcNow.AddHours(-25);
		register.Setup(x => x.GetValue(It.IsAny<IProgVariable>(),"wizard_known_changed")).Returns(() => new DateTimeVariable(changed));
		register.Setup(x => x.GetValue(It.IsAny<IProgVariable>(),"vancian_level")).Returns(new NumberVariable(3));
		register.Setup(x => x.GetValue(It.IsAny<IProgVariable>(),"vancian_progression_allowed")).Returns(new BooleanVariable(true));
		register.Setup(x => x.SetValue(It.IsAny<IProgVariable>(),"wizard_known_changed",It.IsAny<IProgVariable>())).Callback<IProgVariable,string,IProgVariable>((_,_,value) => changed = (DateTime)value.GetObject).Returns(true);
		var examples = JsonSerializer.Deserialize<Example[]>(File.ReadAllText(Path.Combine(RepositoryRoot(),"Design Documents/Magic/Vancian_Example_Progs.json")))!;
		var compiled = new Dictionary<string,Prog>();
		foreach (var example in examples)
		{
			var signature = VancianPolicy.Signatures[example.Kind];
			var prog = new Prog(f.World.Object,example.Name,signature.Return,signature.Parameters.Select((type,i) => Tuple.Create(type,example.Parameters[i])),example.Body);
			Assert.IsTrue(prog.Compile(),$"{example.Name}: {prog.CompileError}"); Assert.IsTrue(VancianPolicy.ValidSignature(prog,example.Kind)); compiled.Add(example.Name,prog);
		}
		object[] known = [f.Actor.Object,f.Capability.Object,Array.AsReadOnly(Array.Empty<IMagicSpell>()),Array.AsReadOnly(new[] { f.Spells[0] })];
		Assert.AreEqual(true,compiled["vancian_choose_once"].Execute(known));
		Assert.AreEqual(true,compiled["vancian_daily_change"].Execute(known));
		Assert.IsTrue(compiled["vancian_record_change"].ExecuteWithStatus(out _,known));
		Assert.AreEqual(false,compiled["vancian_daily_change"].Execute(known));
		Assert.AreEqual(false,compiled["vancian_fixed_known"].Execute(known));
		Assert.AreEqual(true,compiled["vancian_free_change"].Execute(known));
		Assert.AreEqual(true,compiled["vancian_progression_change"].Execute(known));
		Assert.AreEqual(3m,compiled["vancian_example_level"].Execute(f.Actor.Object,f.Capability.Object));
		Assert.AreEqual(2m,compiled["vancian_example_count"].Execute(f.Actor.Object,f.Capability.Object,3,1));
		Assert.AreEqual(0m,compiled["vancian_example_count"].Execute(f.Actor.Object,f.Capability.Object,0,1));
		Assert.AreEqual(3m,compiled["vancian_example_limit"].Execute(f.Actor.Object,f.Capability.Object,3,1));
		Assert.AreEqual(true,compiled["vancian_example_candidates"].Execute(f.Actor.Object,f.Capability.Object,f.Spells[0]));
	}
	[TestMethod]
	public void EveryPublicVancianFunctionCompilesWithItsExactDeclaredTypes()
	{
		FutureProgTestBootstrap.EnsureInitialised(); var f = new VancianTestFixture();
		foreach (var contract in VancianFunction.Contracts)
		{
			var names = contract.Parameters.Select((type,i) => Tuple.Create(type,$"arg{i}")).ToArray();
			var call = $"return {contract.Name}({string.Join(", ",names.Select(x => "@" + x.Item2))})";
			var prog = new Prog(f.World.Object,"example",contract.ReturnType,names,call);
			Assert.IsTrue(prog.Compile(),$"{contract.Name}: {prog.CompileError}");
		}
	}
}
