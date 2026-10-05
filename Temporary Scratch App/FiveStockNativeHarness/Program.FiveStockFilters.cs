#nullable enable
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
namespace FutureMUD.GatheringNativePersistenceHarness;
internal static partial class GNHProgram
{
	private static void VerifyRejectedStockPolicyCreation(NativeRuntime native,string connection,ITraitDefinition trait,IFutureProg prog)
	{
		using var before=NewIndependentContext(connection);
		var counts=(before.MagicSpells.Count(),before.TraitExpressions.Count(),before.FutureProgs.Count());
		var stored=before.FutureProgs.AsNoTracking().Single(x=>x.Id==prog.Id);
		var text=prog.FunctionText;var comment=prog.FunctionComment;var category=prog.Category;var type=prog.ReturnType;
		try {
			prog.FunctionText="this is deliberately invalid futureprog";Require(!prog.Compile(),"Invalid selected fixture compiled");
			var refused=false;try {ArmageddonMendFleshStock.Create(native.World,native.Capability.School,trait,native.Resource,prog);}catch(InvalidOperationException){refused=true;}
			using var after=NewIndependentContext(connection);
			Require(refused && counts==(after.MagicSpells.Count(),after.TraitExpressions.Count(),after.FutureProgs.Count()),"Invalid selected policy created content rows");
			var persisted=after.FutureProgs.AsNoTracking().Single(x=>x.Id==prog.Id);
			Require(persisted.FunctionText==stored.FunctionText && persisted.FunctionComment==stored.FunctionComment && persisted.ReturnTypeDefinition==stored.ReturnTypeDefinition && prog.FunctionComment==comment && prog.Category==category && prog.ReturnType==type,"Validation persisted or changed unrelated authored prog data");
		} finally {prog.FunctionText=text;Require(prog.Compile(),"Restore selected eligibility policy");}
		Console.WriteLine("FIVE-STOCK-filter-creation=passed invalid-compiled-selected-prog-refused-before-row-creation no-spell-expression-support-prog-rows no-authored-prog-persistence");
	}
	private static void VerifyLiveStockTargetPolicy(NativeRuntime native,string connection,MagicSpell spell,string target,Action<MagicSpell,string,string> refuse,ILiquidContainer? container=null)
	{
		var world=native.World;var progs=(All<IFutureProg>)world.FutureProgs;var spells=(All<IMagicSpell>)world.MagicSpells;
		var filterId=(long)spell.Trigger.SaveToXml().Element("TargetFilterProg")!;var prog=progs.Get(filterId)!;var text=prog.FunctionText;
		var wounds=native.Body.Wounds.Sum(x=>x.CurrentDamage);var volume=container?.LiquidVolume;
		try {
			prog.FunctionText="this is deliberately invalid futureprog";Require(!prog.Compile() && !spell.ReadyForGame && spell.WhyNotReadyForGame(native.Actor).Contains("Target filter"),"Later invalidation remained ready");
			refuse(spell,target,"later invalidated configured eligibility");
			prog.FunctionText=text;Require(prog.Compile() && spell.ReadyForGame,"Restored healthy policy remained invalid");
			progs.Remove(prog);Require(!spell.ReadyForGame && (long)spell.Trigger.SaveToXml().Element("TargetFilterProg")! == filterId,"Live deletion forgot configured policy ID");
			using(var db=NewIndependentContext(connection)) {
				var loaded=new MagicSpell(db.MagicSpells.AsNoTracking().Single(x=>x.Id==spell.Id),world);
				Require(!loaded.ReadyForGame && (long)loaded.Trigger.SaveToXml().Element("TargetFilterProg")! == filterId && (long)loaded.Trigger.Clone().SaveToXml().Element("TargetFilterProg")! == filterId,"Reload/clone erased missing configured policy");
				spells.Remove(spell);spells.Add(loaded);try {refuse(loaded,target,"missing nonzero eligibility after database reload");}finally {spells.Remove(loaded);spells.Add(spell);}
			}
			foreach(var failed in new[]{false,true}) {
				var injected=new Mock<IFutureProg>();injected.SetupGet(x=>x.Id).Returns(filterId);injected.SetupGet(x=>x.ReturnType).Returns(prog.ReturnType);
				injected.SetupGet(x=>x.CompileError).Returns("");injected.Setup(x=>x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns<IEnumerable<ProgVariableTypes>>(prog.MatchesParameters);
				object result=failed?true:null!;injected.Setup(x=>x.ExecuteWithStatus(out result,It.IsAny<object[]>())).Returns(!failed);
				progs.Add(injected.Object);try {Require(spell.ReadyForGame,"Runtime failure fixture must retain healthy structural signature");refuse(spell,target,failed?"failed policy execution":"null policy result");}finally {progs.Remove(injected.Object);}
			}
			Require(native.Body.Wounds.Sum(x=>x.CurrentDamage)==wounds && container?.LiquidVolume==volume,"Refused policy changed wounds/liquid");
		} finally {prog.FunctionText=text;Require(prog.Compile(),"Restore live eligibility");if(progs.Get(filterId) is null)progs.Add(prog);}
		Console.WriteLine("FIVE-STOCK-filter-policy=passed key:"+spell.StockIdentity+" invalidation missing-db-reload preserved-id null-result failed-execution prepayment-reserves-and-operation-receipts-conserved wounds-and-liquid-unchanged restored-healthy");
	}
}
