using Microsoft.EntityFrameworkCore;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Database
{
    public partial class FuturemudDatabaseContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			ConfigureDiscordIntegration(modelBuilder);
			ConfigureSkillSelectionGroups(modelBuilder);
            // Split the OnModelCreating into multiple functions for editor performance reasons
            OnModelCreatingOne(modelBuilder);
            OnModelCreatingTwo(modelBuilder);
            OnModelCreatingThree(modelBuilder);
            OnModelCreatingFour(modelBuilder);
            OnModelCreatingFive(modelBuilder);
            OnModelCreatingSix(modelBuilder);
			OnModelCreatingSeven(modelBuilder);
			OnModelCreatingEight(modelBuilder);

			ConfigureNativeLanguages(modelBuilder);
			ConfigureVancianMagic(modelBuilder);
			ConfigureMagicCasting(modelBuilder);
			ConfigureSpellLifecycles(modelBuilder);
			ConfigureBodyRetirements(modelBuilder);
			ConfigureCharacterArchives(modelBuilder);
			ConfigureEnvironmentalMagic(modelBuilder);
			ConfigureMagicGathering(modelBuilder);
			ConfigureRegionalGathering(modelBuilder);
			ConfigureRacialMerits(modelBuilder);
			ConfigureAgeDescriptions(modelBuilder);
			ConfigureClanAddresses(modelBuilder);
			modelBuilder.Entity<StagedPerformanceRecord>(entity =>
			{
				entity.ToTable("StagedPerformanceRecords");
				entity.HasKey(x => x.Id);
				entity.Property(x => x.Kind).HasMaxLength(16).IsRequired();
				entity.Property(x => x.Definition).HasColumnType("longtext").IsRequired();
			});
            OnModelCreatingPartial(modelBuilder);
			ConfigureRoomSpatialOwnership(modelBuilder);
        }
    }
}
