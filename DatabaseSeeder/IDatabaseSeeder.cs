using System;
using System.Collections.Generic;
using MudSharp.Database;

namespace DatabaseSeeder;

public enum ShouldSeedResult
{
	PrerequisitesNotMet,
	MayAlreadyBeInstalled,
	ExtraPackagesAvailable,
	ReadyToInstall
}

public interface IDatabaseSeeder
{
	IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
		Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
	{
		get;
	}

	IEnumerable<SeederQuestion> Questions => SeederQuestionRegistry.GetQuestions(this, SeederQuestions);

	int SortOrder { get; }
	string Name { get; }
	string Tagline { get; }
	string FullDescription { get; }
	bool Enabled => true;
	bool SafeToRunMoreThanOnce => false;
	SeederMetadata Metadata => SeederMetadataRegistry.GetMetadata(this);
	string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers);
	ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context);
	SeederAssessment AssessSeedData(FuturemudDatabaseContext context) => SeederMetadataRegistry.Assess(this, context);
}
