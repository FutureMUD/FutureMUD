#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Models;
using System;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodypartLimbSeverDescriptionSeederTests
{
	[TestMethod]
	public void UseLimbSeverDescription_EyesAndEarsUseDirectDescriptions()
	{
		Assert.IsFalse(SeederBodyUtilities.UseLimbSeverDescription(BodypartTypeEnum.Eye, "reye"));
		Assert.IsFalse(SeederBodyUtilities.UseLimbSeverDescription(BodypartTypeEnum.Ear, "rinnerear"));
		Assert.IsFalse(SeederBodyUtilities.UseLimbSeverDescription(BodypartTypeEnum.Wear, "rear"));
		Assert.IsTrue(SeederBodyUtilities.UseLimbSeverDescription(BodypartTypeEnum.Wear, "rarm"));
	}

	[TestMethod]
	public void RefreshLimbSeverDescriptionFlags_OnlyUpdatesRequestedStockBodies()
	{
		using FuturemudDatabaseContext context = BuildContext();
		BodyProto stockBody = CreateBody(1, "Stock Body");
		BodyProto customBody = CreateBody(2, "Custom Body");
		context.BodyProtos.AddRange(stockBody, customBody);
		context.BodypartProtos.AddRange(
			new BodypartProto
			{
				Id = 1,
				BodyId = stockBody.Id,
				Name = "reye",
				Description = "right eye",
				BodypartType = (int)BodypartTypeEnum.Eye,
				UseLimbSeverDescription = true
			},
			new BodypartProto
			{
				Id = 2,
				BodyId = stockBody.Id,
				Name = "rear",
				Description = "right ear",
				BodypartType = (int)BodypartTypeEnum.Wear,
				UseLimbSeverDescription = true
			},
			new BodypartProto
			{
				Id = 3,
				BodyId = stockBody.Id,
				Name = "rarm",
				Description = "right arm",
				BodypartType = (int)BodypartTypeEnum.Wear,
				UseLimbSeverDescription = false
			},
			new BodypartProto
			{
				Id = 4,
				BodyId = customBody.Id,
				Name = "leye",
				Description = "left eye",
				BodypartType = (int)BodypartTypeEnum.Eye,
				UseLimbSeverDescription = true
			});
		context.SaveChanges();

		SeederBodyUtilities.RefreshLimbSeverDescriptionFlags(context, [stockBody]);

		Assert.IsFalse(context.BodypartProtos.Find(1L)!.UseLimbSeverDescription);
		Assert.IsFalse(context.BodypartProtos.Find(2L)!.UseLimbSeverDescription);
		Assert.IsTrue(context.BodypartProtos.Find(3L)!.UseLimbSeverDescription);
		Assert.IsTrue(context.BodypartProtos.Find(4L)!.UseLimbSeverDescription);
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static BodyProto CreateBody(long id, string name)
	{
		return new BodyProto
		{
			Id = id,
			Name = name,
			WielderDescriptionPlural = "wielders",
			WielderDescriptionSingle = "wielder",
			ConsiderString = "body",
			LegDescriptionSingular = "leg",
			LegDescriptionPlural = "legs"
		};
	}
}
