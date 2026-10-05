#nullable enable
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using MudSharp.Database;

namespace FutureMUD.GatheringNativePersistenceHarness;
internal static partial class GNHProgram
{
	internal static int FiveStockMain(string[] args)
	{
		if(args.FirstOrDefault() is not ("--five-stock-run" or "--five-stock-reader")) return Main(args);
		try { OwnedConnections.Install(); return args[0]=="--five-stock-run" ? RunFiveStockChecks() : ReadFiveStock(args[1]); }
		catch(Exception error) { Console.Error.WriteLine(error); return 1; }
	}
	private sealed record FiveStockReader(string Database,FixtureIds Fixture,DateTime Now,long[] Spells,long Light,long Vessel,double Volume,Guid Origin,Guid[] Operations,Guid Unproven,DateTime UnprovenExpiry,long ForeignGear);

	private static int RunFiveStockChecks()
	{
		using var globals=new ConsumableGlobals(); using var database=TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine("FIVE-STOCK-database="+database.Name);
		var fixture=FixtureSeed.Create(database,"five_stock_lane",true);
		var clock=new HarnessClock(); using var time=RuntimeClock.Push(clock);
		using(var db=NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed=NativeRuntime.Load(fixture,database.ConnectionString,true); ConfigureCastingWorld(seed,database.ConnectionString,true);
		SeedRetirementPrototypes(database,seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database,seed.World.Materials.First().Id);
		SeedConsumables(database,fixture,seed.World.Materials.First().Id);
		var host=PrepareRetirementHost(database,fixture,clock,wielding:true,consumablesAnatomy:true); var native=host.Native; var actor=native.Actor; var world=native.World;
		var effects=new EffectScheduler(world,clock); native.WorldMock.SetupGet(x=>x.EffectScheduler).Returns(effects);
		var owned=new SpellOwnedItemService(world); native.WorldMock.SetupGet(x=>x.SpellOwnedItems).Returns(owned);
		var expressions=(All<ITraitExpression>)world.TraitExpressions; var spells=(All<IMagicSpell>)world.MagicSpells; var progs=(All<IFutureProg>)world.FutureProgs;
		native.WorldMock.Setup(x=>x.Add(It.IsAny<ITraitExpression>())).Callback<ITraitExpression>(x=>expressions.Add(x));
		native.WorldMock.Setup(x=>x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(x=>spells.Add(x));
		native.WorldMock.Setup(x=>x.Add(It.IsAny<IFutureProg>())).Callback<IFutureProg>(x=>progs.Add(x));
		native.WorldMock.SetupGet(x=>x.AlwaysFalseProg).Returns(progs.GetByName("AlwaysFalse")!);
		// Use ordinary native attribute capacity, sufficient for the unmodified x1.5
		// source-efficiency overreach cost (112.5), rather than the old fixture cap 100.
		var capacityAttribute=world.Traits.GetByName("ARM02 Agility")!;Require(actor.AddTrait(capacityAttribute,19),"Native capacity attribute");
		MudSharp.Models.TraitExpression capacityRow;using(var db=NewIndependentContext(database.ConnectionString)){capacityRow=new(){Name="Five stock authored capacity",Expression="80+2*variable"};db.TraitExpressions.Add(capacityRow);db.SaveChanges();}
		expressions.Add(new TraitExpression(capacityRow,world));
		Require(native.Resource.BuildingCommand(actor,new StringStack($"capattribute {capacityAttribute.Id} {capacityRow.Id} raw")) && native.Resource.ResourceCap(actor)==118,"Native builder capacity 118");
		// Labeled native fixture mappings, never asserted to be recovered guild/race IDs.
		MudSharp.Models.FutureProg eligibilityRow;
		using(var db=NewIndependentContext(database.ConnectionString)) {
			eligibilityRow=new(){FunctionName="fiveStockMendEligibility",FunctionText="return lowercase(@caster.location.terrain.name) != \"nilaz plane\"",
				ReturnTypeDefinition=ProgVariableTypes.Boolean.ToStorageString(),FunctionComment="Labeled fixture only; no native undead/defiler IDs asserted.",Category="Harness",Subcategory="Five Stock"};
			foreach(var name in new[]{"target","caster"}) eligibilityRow.FutureProgsParameters.Add(new(){ParameterIndex=eligibilityRow.FutureProgsParameters.Count,ParameterName=name,ParameterTypeDefinition=ProgVariableTypes.Character.ToStorageString()});
			db.FutureProgs.Add(eligibilityRow);db.SaveChanges();
		}
		var eligibility=new FutureProg(eligibilityRow,world);
		Require(eligibility.Compile(),"Mend fixture eligibility failed: "+eligibility.CompileError); progs.Add(eligibility);
		var cap=(SkillLevelBasedMagicCapability)native.Capability; var trait=world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var mendTrait=world.Traits.GetByName("ARM02 Secondary Proficiency")!;
		VerifyRejectedStockPolicyCreation(native,database.ConnectionString,mendTrait,eligibility);
		var water=world.Liquids.GetByName("ARM03C2 water")!;
		var lightPrototype=host.Prototypes.Values.Single(x=>x.Name=="ARM03B2B C2 light");
		var builderMessages=new List<string>(); var output=new Mock<MudSharp.PerceptionEngine.IOutputHandler>();
		output.Setup(x=>x.Send(It.IsAny<string>(),It.IsAny<bool>(),It.IsAny<bool>())).Callback<string,bool,bool>((text,_,__)=>builderMessages.Add(text)).Returns(true);
		MagicSpell Build(string type,string name,string extra="") {
			var original=actor.OutputHandler; SetPrivateMember(actor,"OutputHandler",output.Object);
			var command=$"stock {type} {cap.School.Id} {(type=="mend-flesh"?mendTrait.Id:trait.Id)} {native.Resource.Id} {extra}";
			try { EditableItemHelper.MagicSpellHelper.EditableNewAction(actor,new StringStack(command)); }
			finally { SetPrivateMember(actor,"OutputHandler",original); }
			Require(spells.Any(x=>x.Name==name),"Ordinary stock builder refused "+type+": "+string.Join(';',builderMessages));
			var spell=(MagicSpell)spells.Single(x=>x.Name==name); Require(spell.ReadyForGame && spell.GradeConfigurationErrors().Count==0,"Stock not ready "+name+": "+string.Join(';',spell.GradeConfigurationErrors()));
			var count=spells.Count; EditableItemHelper.MagicSpellHelper.EditableNewAction(actor,new StringStack(command)); Require(count==spells.Count,"Duplicate stock created rows "+type);
			return spell;
		}
		var sense=Build("sense-enchantment",ArmageddonSenseEnchantmentStock.Name);
		var unravel=Build("unravel-enchantment",ArmageddonUnravelEnchantmentStock.Name);
		var mend=Build("mend-flesh",ArmageddonMendFleshStock.Name,eligibility.Id.ToString());
		var draw=Build("draw-water",ArmageddonDrawWaterStock.Name,$"{water.Id} none");
		var hover=Build("hovering-light",ArmageddonHoveringLightStock.Name,lightPrototype.Id.ToString());
		var all=new[]{sense,unravel,mend,draw,hover};
		foreach(var spell in all) {
			Require(spell.BuildingCommand(actor,new StringStack("grades practice difficulty easy")),"Practice edit refused");
			using(new FMDB()) {spell.Save(); FMDB.Context.SaveChanges();}
			using var db=NewIndependentContext(database.ConnectionString); var reloaded=new MagicSpell(db.MagicSpells.AsNoTracking().Single(x=>x.Id==spell.Id),world);
			Require(reloaded.ReadyForGame && reloaded.StockIdentity==spell.StockIdentity && reloaded.GradeProfile!.Practice!.MaximumGrade is null,"Stock persistence lost editable schema "+spell.Name);
			spells.Remove(spell); spells.Add(reloaded);
			foreach(var value in new[]{$"casting trait {trait.Id}",$"casting resources {native.Resource.Id} {native.Resource.Id} passive",$"casting entry add {spell.Id}",$"casting entry skill {spell.Id} {(spell==sense||spell==unravel?60:30)} {(spell==mend?60:90)} relative",$"casting entry starting {spell.Id} on","casting enable on"})
				Require(cap.BuildingCommand(actor,new StringStack(value)),"Capability refused "+value);
			if(spell==mend)Require(cap.BuildingCommand(actor,new StringStack($"casting entry trait {spell.Id} {mendTrait.Id}")),"Dedicated Mend spell-skill binding");
		}
		all=all.Select(x=>(MagicSpell)spells.Get(x.Id)).ToArray(); sense=all[0];unravel=all[1];mend=all[2];draw=all[3];hover=all[4];
		actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(null,true); actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]);
		var staff=new Mock<ICharacter>(); staff.SetupGet(x=>x.Id).Returns(999); staff.Setup(x=>x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		var casting=new MagicCastingService(world,clock:()=>RuntimeClock.UtcNow,random:()=>0.1,flush:()=>FlushCasting(native)); native.WorldMock.SetupGet(x=>x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff.Object,actor,cap.Id,"Five stock native lane").Allowed,"Stock enrolment failed."); actor.SetTraitValue(trait,90);actor.SetTraitValue(mendTrait,60);
		var state=new MagicCastingStateStore(); foreach(var spell in all)state.Write(acquired:casting.Acquisition(actor,spell.Id)! with{ControlledGrade=7});
		var operations=new List<Guid>();
		MagicCastingResult Cast(MagicSpell spell,int grade,string target="me",bool applied=true) {
			actor.RemoveAllEffects<MagicSpellLockout>(null,true); actor.AddResource(native.Resource,118); FlushCasting(native);
			var before=actor.MagicResourceAmounts[native.Resource]; var quote=casting.Quote(new(actor,cap.Id,spell.Id,grade,false,target));
			Require(quote.Allowed,"Quote refused "+spell.Name+": "+quote.Reason);
			var result=casting.Cast(new(actor,cap.Id,spell.Id,grade,false,target));
			Require(result.Status==MagicCastingStatus.Succeeded,"Cast failed "+spell.Name+": "+result.Message);
			Require(actor.MagicResourceAmounts[native.Resource]==before-quote.Invocation!.Costs.Single().Amount,"Cost differs from quote "+spell.Name);
			var receipt=XElement.Parse(state.Operation(result.OperationId!.Value)!.Definition);
			Require((bool)receipt.Attribute("applied")! == applied,"Incorrect intended-operation report "+spell.Name); operations.Add(result.OperationId.Value);
			Console.WriteLine($"FIVE-STOCK-paid=passed key:{spell.StockIdentity} grade:{grade} cost:{before-actor.MagicResourceAmounts[native.Resource]} applied:{applied}"); return result;
		}
		void Refuse(MagicSpell spell,string target,string reason) {
			actor.RemoveAllEffects<MagicSpellLockout>(null,true); actor.AddResource(native.Resource,118); FlushCasting(native);var balance=actor.MagicResourceAmounts[native.Resource];
			using var beforeDb=NewIndependentContext(database.ConnectionString); var count=beforeDb.MagicCastingOperations.Count(); var itemCount=beforeDb.GameItems.Count();
			var result=casting.Cast(new(actor,cap.Id,spell.Id,1,false,target));
			using var after=NewIndependentContext(database.ConnectionString);
			Require(result.Status==MagicCastingStatus.Refused && result.OperationId is null && balance==actor.MagicResourceAmounts[native.Resource] && count==after.MagicCastingOperations.Count() && itemCount==after.GameItems.Count(),"Target refusal spent or created "+reason+": "+result.Message);
			Console.WriteLine("FIVE-STOCK-refusal=passed key:"+spell.StockIdentity+" reason:"+reason+" prepayment-conserved");
		}
		foreach(var spell in all)Refuse(spell,"no_such_target","missing target");
		var terrain=Mock.Get(actor.Location.CurrentOverlay.Terrain);
		terrain.SetupGet(x=>x.Type).Returns(ProgVariableTypes.Terrain);terrain.SetupGet(x=>x.GetObject).Returns(terrain.Object);
		Mock.Get(actor.Location).SetupGet(x=>x.Type).Returns(ProgVariableTypes.Location);Mock.Get(actor.Location).SetupGet(x=>x.GetObject).Returns(actor.Location);
		Mock.Get(actor.Location).Setup(x=>x.GetProperty("terrain")).Returns(terrain.Object);
		void TerrainName(string name){terrain.SetupGet(x=>x.Name).Returns(name);terrain.Setup(x=>x.GetProperty("name")).Returns(new MudSharp.FutureProg.Variables.TextVariable(name));}
		TerrainName("Silt");
		Require(((MudSharp.Magic.SpellTriggers.CastingTriggerCharacter)sense.Trigger).TargetFilterProg.Execute<bool?>(actor,actor)==false,"Compiled Silt filter did not evaluate false on native caster");
		Refuse(sense,"me","source Silt");TerrainName("Desert");
		VerifyLiveStockTargetPolicy(native,database.ConnectionString,sense,"me",Refuse);
		Cast(sense,1); Require(actor.EffectsOfType<SpellDetectMagickEffect>().Count()==1,"Sense did not attach native detection");
		var detection=actor.EffectsOfType<MagicSpellParent>().Single(x=>x.Spell.Id==sense.Id); Require(effects.OriginalDuration(detection)==TimeSpan.FromSeconds(3000),"Sense low lifetime");
		Cast(unravel,1);Require(!actor.EffectsOfType<SpellDetectMagickEffect>().Any(),"Unravel did not remove low detection");
		Cast(sense,7); detection=actor.EffectsOfType<MagicSpellParent>().Single(x=>x.Spell.Id==sense.Id);Require(effects.OriginalDuration(detection)==TimeSpan.FromSeconds(21000),"Sense high lifetime");
		Cast(unravel,1,applied:false); Require(actor.EffectsOfType<SpellDetectMagickEffect>().Any(),"Losing dispel contest removed stronger enchantment");
		Cast(unravel,7);Require(!actor.EffectsOfType<SpellDetectMagickEffect>().Any(),"Unravel high contest did not remove detection"); Cast(unravel,7,applied:false);
		TerrainName("Nilaz Plane"); Refuse(mend,"me","selected Nilaz eligibility");TerrainName("Desert");
		VerifyLiveStockTargetPolicy(native,database.ConnectionString,mend,"me",Refuse);
		var wounds=native.Body.Wounds.Where(x=>x.CanBeTreated(TreatmentType.Mend)!=Difficulty.Impossible).ToArray();Require(wounds.Length>0,"Native Mend fixture missing wound");
		var damaged=wounds.Sum(x=>x.CurrentDamage);Cast(mend,1);Require(wounds.Sum(x=>x.CurrentDamage)==damaged-2,"Mend low budget");
		Cast(mend,7);Require(!native.Body.Wounds.Any(x=>x.CurrentDamage>0),"Mend high budget did not repair native wounds");Cast(mend,7,applied:false);
		GameItem New(string name){var item=(GameItem)host.Prototypes.Values.Single(x=>x.Name=="ARM03B2B C2 "+name).CreateNew(actor);world.Add(item);actor.Location.Insert(item,true);item.Login();world.SaveManager.Flush();return item;}
		var vessel=New("vessel");var container=vessel.GetItemType<ILiquidContainer>()!; Refuse(draw,"vessel","closed container");container.Open();
		TerrainName("Silt");Refuse(draw,"vessel","source Silt with accessible open container");
		TerrainName("Fire Plane");Refuse(draw,"vessel","source Fire Plane with accessible open container");TerrainName("Desert");
		VerifyLiveStockTargetPolicy(native,database.ConnectionString,draw,"vessel",Refuse,container);
		Cast(draw,1,"vessel");Require(container.LiquidVolume==500,"Draw ordinary low amount");Cast(draw,7,"vessel");Require(container.LiquidVolume==4000,"Draw ordinary high amount");
		Require(draw.BuildingCommand(actor,new StringStack($"effect 1 bonusplane {world.DefaultPlane.Id} 2")),"Water bonus plane builder");
		Cast(draw,7,"vessel");Require(container.LiquidVolume==5000,"Draw bonus and capacity clamp");Refuse(draw,"vessel","full container");
		int OriginCount(){using var db=NewIndependentContext(database.ConnectionString);return db.MagicSpellLifecycles.Count();}
		var originCount=OriginCount();var needsBefore=actor.NeedsModel.WaterLitres;Require(native.Body.SilentDrink(container,250),"Native water drink");Require(container.LiquidVolume==4750 && actor.NeedsModel.WaterLitres>needsBefore && OriginCount()==originCount,"Water consumption created lifecycle or lost conservation");
		container.RemoveLiquidAmount(container.LiquidVolume,actor,"fixture");container.MergeLiquid(new LiquidMixture(world.Liquids.GetByName("ARM03C2 oil")!,250,world),actor,"fixture");Refuse(draw,"vessel","incompatible oil");Require(container.LiquidVolume==250 && container.LiquidMixture!.Instances.Single().Liquid.Id!=water.Id,"Draw mutated foreign mixture");
		container.RemoveLiquidAmount(container.LiquidVolume,actor,"fixture");Cast(draw,1,"vessel");Require(container.LiquidVolume==1000,"Configured water bonus low amount");
		var gear=Enumerable.Range(0,5).Select(_=>New("gear")).ToArray();foreach(var item in gear){item.Get(native.Body);native.Body.WearExternally(item);}
		Require(gear.All(native.Body.WornItems.Contains),"Foreign full-slot gear fixture");Refuse(hover,"me","occupied native wear profile");
		foreach(var item in gear.Skip(1)){native.Body.RemoveItem(item,null!,true);native.Body.Drop(item,silent:true);}
		Cast(hover,1);var lowLight=host.Items.Single(x=>x.SpellCreationOrigin is{} origin && origin.DeadlineUtc==RuntimeClock.UtcNow.AddSeconds(1800));
		Require(native.Body.WornItems.Contains(lowLight) && lowLight.GetItemType<IProduceLight>()!.CurrentIllumination==40,"Low hovering light not worn/lit");
		native.Body.RemoveItem(lowLight,null!,true);Require(!native.Body.WornItems.Contains(lowLight),"Light unwear failed");native.Body.Drop(lowLight,silent:true); var lowDeadline=lowLight.SpellCreationOrigin!.DeadlineUtc;
		Require(lowLight.SpellCreationOrigin.DeadlineUtc==lowDeadline && !lowLight.Deleted,"Unwear custody reset or destroyed light");
		Cast(hover,7);var highLight=host.Items.Single(x=>x.SpellCreationOrigin?.DeadlineUtc==RuntimeClock.UtcNow.AddSeconds(12600));
		Require(native.Body.WornItems.Contains(highLight) && actor.IlluminationProvided==40,"High hovering light native illumination");
		state.Write(acquired:casting.Acquisition(actor,hover.Id)! with{ControlledGrade=1,NextMasteryUtc=DateTime.UnixEpoch});
		actor.RemoveAllEffects<MagicSpellLockout>(null,true);actor.AddResource(native.Resource,118);
		var mastery=casting.Cast(new(actor,cap.Id,hover.Id,2,true,"me"));Require(mastery.Status==MagicCastingStatus.Succeeded && casting.Acquisition(actor,hover.Id)!.ControlledGrade==2 && (bool)XElement.Parse(state.Operation(mastery.OperationId!.Value)!.Definition).Attribute("applied")!,"Actual prepared hovering light did not qualify ordinary next-grade mastery: "+mastery.Status+" "+mastery.Message);
		state.Write(acquired:casting.Acquisition(actor,hover.Id)! with{ControlledGrade=1,NextMasteryUtc=DateTime.UnixEpoch});
		var itemsBefore=host.Items.Select(x=>x.Id).ToHashSet();var originsBefore=OriginCount();
		native.WorldMock.Setup(x=>x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(item=>{if(!host.Items.Has(item.Id))host.Items.Add(item);if(item.SpellCreationOrigin is not null)throw new InvalidOperationException("Five stock injected post-commit placement failure");});
		actor.RemoveAllEffects<MagicSpellLockout>(null,true);actor.AddResource(native.Resource,118);var failedBalance=actor.MagicResourceAmounts[native.Resource];
		var failedQuote=casting.Quote(new(actor,cap.Id,hover.Id,2,true,"me"));Require(failedQuote.Allowed,"Partial creation quote");
		var partial=casting.Cast(new(actor,cap.Id,hover.Id,2,true,"me"));
		native.WorldMock.Setup(x=>x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(item=>{if(!host.Items.Has(item.Id))host.Items.Add(item);});
		Require(partial.Status==MagicCastingStatus.NeedsReview && OriginCount()==originsBefore+1 && host.Items.Any(x=>!itemsBefore.Contains(x.Id)) && actor.MagicResourceAmounts[native.Resource]==failedBalance-failedQuote.Invocation!.Costs.Single().Amount && casting.Acquisition(actor,hover.Id)!.ControlledGrade==1,"Partial committed native light creation did not retain paid quarantine/no mastery");
		var committedPartial=host.Items.Single(x=>!itemsBefore.Contains(x.Id));var paid=actor.MagicResourceAmounts[native.Resource];
		Require(casting.Cast(new(actor,cap.Id,hover.Id,2,true,"me",OriginId:partial.OperationId)).Status==MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource]==paid && OriginCount()==originsBefore+1,"Partial creation replayed or refunded");
		Require(casting.ReconcileOperation(staff.Object,actor,partial.OperationId!.Value,"Audited retained exact committed light; no replay/refund").Allowed,"Partial native quarantine acknowledgement");
		committedPartial.Delete();Require(host.Store.Find(committedPartial.SpellCreationOrigin!.LifecycleId)!.State==SpellLifecycleState.Completed,"Partial light exact cleanup");
		state.Write(acquired:casting.Acquisition(actor,hover.Id)! with{ControlledGrade=7});
		Console.WriteLine("FIVE-STOCK-prepared-mastery-partial=passed actual-paid-light-next-grade-mastery post-commit-native-placement-failure exact-retained-row paid-quarantine no-mastery no-replay/refund audited-exact-cleanup");
		// Relative cap-60 practice on the actual stock, paid at start and completed once at native deadlines.
		var improver=ConfigurePracticeImprovement(native,database.ConnectionString,true);Require(improver.BuildingCommand(actor,new StringStack("interval 20")),"Practice improvement interval");
		// The shared practice fixture replaces trait definitions. Rebind native body traits and
		// the authored capacity to those same definitions, as normal world reload does.
		using(var db=NewIndependentContext(database.ConnectionString)) {
			SetPrivateField(native.Body,"_traits",db.Traits.AsNoTracking().Where(x=>x.BodyId==native.Body.Id).ToArray()
				.Select(x=>CastingRequired(world.Traits.Get(x.TraitDefinitionId)).LoadTrait(x,native.Body)).ToList());
		}
		Require(native.Resource.BuildingCommand(actor,new StringStack($"capattribute {capacityAttribute.Id} {capacityRow.Id} raw")) && native.Resource.ResourceCap(actor)==118,"Reloaded native practice capacity");
		mendTrait=world.Traits.GetByName("ARM02 Secondary Proficiency")!;
		actor.SetTraitValue(mendTrait,30);Require(casting.RawSkillImprovementCap(actor,mendTrait.Id)==60,"Mend's independent native skill cap");state.Write(acquired:casting.Acquisition(actor,mend.Id)! with{ControlledGrade=1,NextMasteryUtc=DateTime.UnixEpoch});
		for(var grade=1;grade<=7;grade++) {
			clock.Advance(TimeSpan.FromMinutes(11));actor.AddResource(native.Resource,118);actor.RemoveAllEffects<MagicSpellLockout>(null,true);
			var before=actor.MagicResourceAmounts[native.Resource];var quote=casting.Quote(new(actor,cap.Id,mend.Id,grade,grade>1,"",MagicCastingMode.Practice));Require(quote.Allowed,"Mend practice quote: "+quote.Reason);
			MudSharp.Commands.Modules.MagicModule.MagicGeneric(actor,$"{cap.School.SchoolVerb} practice \"{mend.Name}\" grade {grade}{(grade>1?" overreach":"")} via {cap.Id}");
			var action=actor.EffectsOfType<MagicPracticeAction>().Single();Require(actor.MagicResourceAmounts[native.Resource]==before-quote.Invocation!.Costs.Single().Amount,"Mend practice start payment");
			clock.Advance(TimeSpan.FromSeconds(30));action.ExpireEffect();action.ExpireEffect();
			Require(state.Operation(action.OperationId)!.Stage=="Completed" && actor.TraitRawValue(mendTrait)==Math.Min(60,30+grade*10) && casting.Acquisition(actor,mend.Id)!.ControlledGrade==grade,$"Mend cap60 practice failed at grade {grade}: raw {actor.TraitRawValue(mendTrait)}, controlled {casting.Acquisition(actor,mend.Id)!.ControlledGrade}, stage {state.Operation(action.OperationId)!.Stage}");
			Console.WriteLine($"FIVE-STOCK-Mend-practice=passed grade:{grade} raw:{actor.TraitRawValue(mendTrait)} cap:60 real-native-skill accelerated-clock");
		}
		Require(mend.GradeProfile!.Practice!.MaximumGrade is null,"Mend practice stock authored unintended grade maximum");
		// Keep a fresh high light and detection for independent persistence/expiry reading after the practice clock advance.
		if(!highLight.Deleted)owned.ReconcileRetirements(RuntimeClock.UtcNow); Cast(hover,7);highLight=host.Items.Single(x=>x.SpellCreationOrigin?.DeadlineUtc==RuntimeClock.UtcNow.AddSeconds(12600));
		trait=world.Traits.GetByName("ARM02 Earth Proficiency")!;actor.SetTraitValue(trait,90);Cast(sense,1);
		Require(unravel.BuildingCommand(actor,new StringStack("effect 1 mode shorten")) && unravel.BuildingCommand(actor,new StringStack("effect 1 shorten 10")),"Unproven shorten builder");
		state.Write(acquired:casting.Acquisition(actor,unravel.Id)! with{ControlledGrade=1,NextMasteryUtc=DateTime.UnixEpoch});
		var unprovenParent=actor.EffectsOfType<MagicSpellParent>().Single(x=>x.Spell.Id==sense.Id);var originalExpiry=((IEffectExpiryObserver)effects).ScheduledExpiry(unprovenParent)!.Value;
		var opaque=new Mock<IEffectScheduler>();opaque.Setup(x=>x.IsScheduled(It.IsAny<IEffect>())).Returns<IEffect>(effects.IsScheduled);
		opaque.Setup(x=>x.RemainingDuration(It.IsAny<IEffect>())).Returns<IEffect>(effects.RemainingDuration);opaque.Setup(x=>x.OriginalDuration(It.IsAny<IEffect>())).Returns<IEffect>(effects.OriginalDuration);
		opaque.Setup(x=>x.Reschedule(It.IsAny<IEffect>(),It.IsAny<TimeSpan>())).Callback<IEffect,TimeSpan>(effects.Reschedule);
		opaque.Setup(x=>x.Unschedule(It.IsAny<IEffect>(),It.IsAny<bool>(),It.IsAny<bool>())).Callback<IEffect,bool,bool>(effects.Unschedule);
		native.WorldMock.SetupGet(x=>x.EffectScheduler).Returns(opaque.Object);actor.RemoveAllEffects<MagicSpellLockout>(null,true);actor.AddResource(native.Resource,118);
		var unproven=casting.Cast(new(actor,cap.Id,unravel.Id,2,true,"me"));native.WorldMock.SetupGet(x=>x.EffectScheduler).Returns(effects);
		Require(unproven.Status==MagicCastingStatus.Succeeded && state.Operation(unproven.OperationId!.Value)!.Stage=="Completed" && !(bool)XElement.Parse(state.Operation(unproven.OperationId.Value)!.Definition).Attribute("applied")! && casting.Acquisition(actor,unravel.Id)!.ControlledGrade==1 && ((IEffectExpiryObserver)effects).ScheduledExpiry(unprovenParent)==originalExpiry.AddSeconds(-10),"Opaque native mutation receipt/mastery");
		FlushCasting(native);
		using(var db=NewIndependentContext(database.ConnectionString)) {
			foreach(var spell in all){using(new FMDB()){spell.Save();FMDB.Context.SaveChanges();}}
			if(!db.CellsGameItems.Any(x=>x.GameItemId==vessel.Id))db.CellsGameItems.Add(new(){CellId=fixture.CellId,GameItemId=vessel.Id});db.SaveChanges();
		}
		var descriptor=new FiveStockReader(database.Name,fixture,RuntimeClock.UtcNow,all.Select(x=>x.Id).ToArray(),highLight.Id,vessel.Id,container.LiquidVolume,highLight.SpellCreationOrigin!.LifecycleId,operations.ToArray(),unproven.OperationId.Value,originalExpiry.AddSeconds(-10),gear[0].Id);
		RunItemReaderProcess(descriptor,"--five-stock-reader");
		Console.WriteLine("FIVE-STOCK-acceptance=passed five-builder-stock-spells paid-low-high actual-operation-reporting cap60-practice separate-process-persistence expiry-custody-conservation");return 0;
	}

	private static int ReadFiveStock(string encoded)
	{
		var input=JsonSerializer.Deserialize<FiveStockReader>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var globals=new ConsumableGlobals();using var database=TestDatabase.OpenExistingOwned(input.Database);ConfigureNativeDatabase(database.ConnectionString);
		var clock=new HarnessClock();clock.Advance(input.Now-clock.GetUtcNow().UtcDateTime);using var time=RuntimeClock.Push(clock);
		var host=PrepareRetirementHost(database,input.Fixture,clock,wielding:true,consumablesAnatomy:true);var native=host.Native;var world=native.World;
		var effects=new EffectScheduler(world,clock);native.WorldMock.SetupGet(x=>x.EffectScheduler).Returns(effects);
		SpellDetectMagickEffect.InitialiseEffectType();
		var owned=new SpellOwnedItemService(world);native.WorldMock.SetupGet(x=>x.SpellOwnedItems).Returns(owned);
		using(var db=NewIndependentContext(database.ConnectionString)) {
			Require(input.Spells.All(id=>world.MagicSpells.Get(id) is not null),"Fresh process missing stock rows");
			// The shared minimal world loads only its Rejuvenation progs. Load the actual
			// selected stock policies as normal boot would, before validating or resolving them.
			var filterIds=input.Spells.Select(id=>(long)world.MagicSpells.Get(id).Trigger.SaveToXml().Element("TargetFilterProg")!).Where(id=>id!=0).ToArray();
			foreach(var model in db.FutureProgs.Include(x=>x.FutureProgsParameters).AsNoTracking().Where(x=>filterIds.Contains(x.Id))) {
				var prog=new FutureProg(model,world);Require(prog.Compile(),"Fresh stock policy compile: "+prog.CompileError);
				if(!world.FutureProgs.Has(prog.Id))((All<IFutureProg>)world.FutureProgs).Add(prog);
			}
			native.Body.LoadInventory(db.Bodies.Include(x=>x.BodiesGameItems).Single(x=>x.Id==native.Body.Id));
			foreach(var id in db.CellsGameItems.Where(x=>x.CellId==input.Fixture.CellId).Select(x=>x.GameItemId).ToArray()) {var item=world.TryGetItem(id,true)!;if(item.InInventoryOf is null && item.ContainedIn is null)native.Actor.Location.Insert(item,true);}
			native.Actor.RestoreCastingEffects(db.Characters.AsNoTracking().Single(x=>x.Id==native.Actor.Id).EffectData);
		}
		// Complete the ordinary login phase: loaded timed effects are cached until scheduled.
		typeof(MudSharp.Framework.PerceivedItem).GetMethod("ScheduleCachedEffects",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(native.Actor,null);
		var light=world.TryGetItem(input.Light,true)!;var vessel=world.TryGetItem(input.Vessel,true)!;
		foreach(var item in host.Items.ToArray())item.FinaliseLoadTimeTasks();
		Require(native.Body.WornItems.Contains(light) && light.GetItemType<IProduceLight>()!.CurrentIllumination==40 && vessel.GetItemType<ILiquidContainer>()!.LiquidVolume==input.Volume,"Fresh process lost native light custody/volume");
		Require(input.Spells.All(id=>world.MagicSpells.Get(id).ReadyForGame) && new MagicCastingStateStore().Operation(input.Operations.Last())!.Stage=="Completed","Fresh stock/operation persistence");
		var parent=native.Actor.EffectsOfType<MagicSpellParent>().Single(x=>x.Spell.Id==input.Spells[0]);
		Require(native.Actor.EffectsOfType<SpellDetectMagickEffect>().Any() && ((IEffectExpiryObserver)effects).ScheduledExpiry(parent)==input.UnprovenExpiry,"Fresh detection or actually shortened expiry missing");
		var casting=new MagicCastingService(world,clock:()=>RuntimeClock.UtcNow,random:()=>throw new InvalidOperationException("Completed unproven operation must not reroll"),flush:()=>FlushCasting(native));native.WorldMock.SetupGet(x=>x.MagicCasting).Returns(casting);
		var before=native.Actor.MagicResourceAmounts[native.Resource];var expiry=((IEffectExpiryObserver)effects).ScheduledExpiry(parent);var operation=new MagicCastingStateStore().Operation(input.Unproven)!;
		Require(operation.Stage=="Completed" && !(bool)XElement.Parse(operation.Definition).Attribute("applied")!,"Unproven completed receipt did not persist");
		var retry=casting.Cast(new(native.Actor,operation.CapabilityId,input.Spells[1],2,true,"me",OriginId:input.Unproven));
		Require(retry.Status==MagicCastingStatus.Refused && native.Actor.MagicResourceAmounts[native.Resource]==before && ((IEffectExpiryObserver)effects).ScheduledExpiry(parent)==expiry && new MagicCastingStateStore().Operation(input.Unproven)!.Stage=="Completed","Fresh completed-unproven retry reran/refunded/shortened again");
		Console.WriteLine("FIVE-STOCK-unproven-reader=passed completed-paid-mutation persisted-actual-expiry applied-false no-mastery fresh-process-origin-retry-refused no-refund/no-reroll/no-second-mutation");
		var deadline=light.SpellCreationOrigin!.DeadlineUtc;native.Body.RemoveItem(light,null!,true);native.Body.Drop(light,silent:true);Require(light.SpellCreationOrigin.DeadlineUtc==deadline,"Fresh custody reset deadline");
		FlushCasting(native); // Persist the ordinary unwear/drop custody before guarded retirement.
		clock.Advance(TimeSpan.FromSeconds(12601));owned.ReconcileRetirements(RuntimeClock.UtcNow);world.SaveManager.Flush();
		var retired=host.Store.Find(input.Origin)!;
		Require(light.Deleted && retired.State==SpellLifecycleState.Completed && !vessel.Deleted && vessel.GetItemType<ILiquidContainer>()!.LiquidVolume==input.Volume && native.Body.WornItems.Any(x=>x.Id==input.ForeignGear),$"Expiry: lightDeleted={light.Deleted}, state={retired.State}, diagnostic={retired.Diagnostic}, vesselDeleted={vessel.Deleted}, volume={vessel.GetItemType<ILiquidContainer>()!.LiquidVolume}, foreignWorn={native.Body.WornItems.Any(x=>x.Id==input.ForeignGear)}");
		effects.CheckSchedules();Require(!native.Actor.EffectsOfType<SpellDetectMagickEffect>().Any(),"Native saved detection failed to expire");
		Console.WriteLine("FIVE-STOCK-reader=passed fresh-process stock-definitions operation-receipts worn-light liquid-volume custody-deadline exact-expiry permanent-water-conserved");return 0;
	}
}
