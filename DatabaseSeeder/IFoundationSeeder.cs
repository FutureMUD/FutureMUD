#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder;

/// <summary>Applies the Database Seeder's stock metadata and question defaults.</summary>
public interface IFoundationSeeder : IDatabaseSeeder
{
	SeederMetadata IDatabaseSeeder.Metadata => SeederMetadataRegistry.GetMetadata(this);
	IEnumerable<SeederQuestion> IDatabaseSeeder.Questions => SeederQuestionRegistry.GetQuestions(this, SeederQuestions);
}
