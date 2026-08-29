using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	protected static void OnModelCreatingEight(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<SignedLanguage>(entity =>
		{
			entity.ToTable("SignedLanguages");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).HasColumnType("bigint(20)").ValueGeneratedOnAdd();
			entity.Property(x => x.Name).IsRequired().HasMaxLength(200).HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(x => x.UnknownLanguageDescription).IsRequired().HasMaxLength(500).HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(x => x.LanguageObfuscationFactor).HasDefaultValue(0.2);
			entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UX_SignedLanguages_Name");
			entity.HasIndex(x => x.DifficultyModelId).HasDatabaseName("FK_SignedLanguages_DifficultyModels_idx");
			entity.HasIndex(x => x.LinkedTraitId).HasDatabaseName("FK_SignedLanguages_TraitDefinitions_idx");
			entity.HasOne(x => x.DifficultyModel).WithMany().HasForeignKey(x => x.DifficultyModelId)
				.OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_SignedLanguages_DifficultyModels");
			entity.HasOne(x => x.LinkedTrait).WithMany().HasForeignKey(x => x.LinkedTraitId)
				.OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_SignedLanguages_TraitDefinitions");
		});

		modelBuilder.Entity<SignedLanguageVariety>(entity =>
		{
			entity.ToTable("SignedLanguageVarieties");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).HasColumnType("bigint(20)").ValueGeneratedOnAdd();
			entity.Property(x => x.Name).IsRequired().HasMaxLength(200).HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(x => x.Description).IsRequired().HasMaxLength(2000).HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(x => x.Suffix).IsRequired().HasMaxLength(500).HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(x => x.VagueSuffix).IsRequired().HasMaxLength(500).HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.HasIndex(x => new { x.SignedLanguageId, x.Name }).IsUnique().HasDatabaseName("UX_SignedLanguageVarieties_Language_Name");
			entity.HasOne(x => x.SignedLanguage).WithMany(x => x.Varieties).HasForeignKey(x => x.SignedLanguageId)
				.OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_SignedLanguageVarieties_SignedLanguages");
		});

		modelBuilder.Entity<SignedLanguageMutualIntelligibility>(entity =>
		{
			entity.ToTable("SignedLanguageMutualIntelligibilities");
			entity.HasKey(x => new { x.ListenerLanguageId, x.TargetLanguageId }).HasName("PRIMARY");
			entity.HasIndex(x => x.TargetLanguageId).HasDatabaseName("FK_SignedLanguageMutual_Target_idx");
			entity.HasOne(x => x.ListenerLanguage).WithMany(x => x.MutualIntelligibilitiesListenerLanguage)
				.HasForeignKey(x => x.ListenerLanguageId).OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_SignedLanguageMutual_Listener");
			entity.HasOne(x => x.TargetLanguage).WithMany(x => x.MutualIntelligibilitiesTargetLanguage)
				.HasForeignKey(x => x.TargetLanguageId).OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_SignedLanguageMutual_Target");
		});

		modelBuilder.Entity<SignedLanguageArticulationProfile>(entity =>
		{
			entity.ToTable("SignedLanguageArticulationProfiles");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).HasColumnType("bigint(20)").ValueGeneratedOnAdd();
			entity.Property(x => x.Name).IsRequired().HasMaxLength(200).HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.HasIndex(x => new { x.SignedLanguageId, x.Name }).IsUnique().HasDatabaseName("UX_SignedLanguageArticulationProfiles_Language_Name");
			entity.HasIndex(x => x.BodyPrototypeId).HasDatabaseName("FK_SignedLanguageArticulationProfiles_BodyProtos_idx");
			entity.HasOne(x => x.SignedLanguage).WithMany(x => x.ArticulationProfiles).HasForeignKey(x => x.SignedLanguageId)
				.OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_SignedLanguageArticulationProfiles_Languages");
			entity.HasOne(x => x.BodyPrototype).WithMany().HasForeignKey(x => x.BodyPrototypeId)
				.OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_SignedLanguageArticulationProfiles_BodyProtos");
		});

		modelBuilder.Entity<SignedLanguageArticulationRequirement>(entity =>
		{
			entity.ToTable("SignedLanguageArticulationRequirements");
			entity.HasKey(x => new { x.ArticulationProfileId, x.BodypartShapeId }).HasName("PRIMARY");
			entity.HasIndex(x => x.BodypartShapeId).HasDatabaseName("FK_SignedLanguageArticulationRequirements_BodypartShapes_idx");
			entity.HasOne(x => x.ArticulationProfile).WithMany(x => x.Requirements).HasForeignKey(x => x.ArticulationProfileId)
				.OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_SignedLanguageArticulationRequirements_Profiles");
			entity.HasOne(x => x.BodypartShape).WithMany().HasForeignKey(x => x.BodypartShapeId)
				.OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_SignedLanguageArticulationRequirements_BodypartShapes");
		});

		modelBuilder.Entity<CharactersSignedLanguage>(entity =>
		{
			entity.ToTable("Characters_SignedLanguages");
			entity.HasKey(x => new { x.CharacterId, x.SignedLanguageId }).HasName("PRIMARY");
			entity.HasIndex(x => x.SignedLanguageId).HasDatabaseName("FK_Characters_SignedLanguages_Languages_idx");
			entity.HasOne(x => x.Character).WithMany(x => x.CharactersSignedLanguages).HasForeignKey(x => x.CharacterId)
				.OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Characters_SignedLanguages_Characters");
			entity.HasOne(x => x.SignedLanguage).WithMany(x => x.CharactersSignedLanguages).HasForeignKey(x => x.SignedLanguageId)
				.OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Characters_SignedLanguages_Languages");
		});

		modelBuilder.Entity<CharacterSignedLanguageVariety>(entity =>
		{
			entity.ToTable("Characters_SignedLanguageVarieties");
			entity.HasKey(x => new { x.CharacterId, x.SignedLanguageVarietyId }).HasName("PRIMARY");
			entity.HasIndex(x => x.SignedLanguageVarietyId).HasDatabaseName("FK_Characters_SignedLanguageVarieties_Varieties_idx");
			entity.HasOne(x => x.Character).WithMany(x => x.CharactersSignedLanguageVarieties).HasForeignKey(x => x.CharacterId)
				.OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Characters_SignedLanguageVarieties_Characters");
			entity.HasOne(x => x.SignedLanguageVariety).WithMany(x => x.CharactersSignedLanguageVarieties)
				.HasForeignKey(x => x.SignedLanguageVarietyId).OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_Characters_SignedLanguageVarieties_Varieties");
		});

		modelBuilder.Entity<Character>(entity =>
		{
			entity.HasIndex(x => x.CurrentSignedLanguageId).HasDatabaseName("FK_Characters_CurrentSignedLanguage_idx");
			entity.HasIndex(x => x.CurrentSignedLanguageVarietyId).HasDatabaseName("FK_Characters_CurrentSignedLanguageVariety_idx");
			entity.HasOne(x => x.CurrentSignedLanguage).WithMany(x => x.CharactersCurrentSignedLanguage)
				.HasForeignKey(x => x.CurrentSignedLanguageId).OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_Characters_CurrentSignedLanguage");
			entity.HasOne(x => x.CurrentSignedLanguageVariety).WithMany(x => x.CharactersCurrentSignedLanguageVariety)
				.HasForeignKey(x => x.CurrentSignedLanguageVarietyId).OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_Characters_CurrentSignedLanguageVariety");
		});
	}
}
