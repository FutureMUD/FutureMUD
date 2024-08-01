using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database
{
	public partial class FuturemudDatabaseContext
	{
        protected static void OnModelCreatingThree(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Material>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Absorbency).HasDefaultValueSql("'0.25'");

                entity.Property(e => e.BehaviourType).HasColumnType("int(11)");

                entity.Property(e => e.MaterialDescription)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Organic).HasColumnType("bit(1)");

                entity.Property(e => e.ResidueColour)
                    .HasColumnType("varchar(45)")
                    .HasDefaultValueSql("'white'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ResidueDesc)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ResidueSdesc)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.SolventId).HasColumnType("bigint(20)");

                entity.Property(e => e.SolventVolumeRatio).HasDefaultValueSql("'1'");

                entity.Property(e => e.Type).HasColumnType("int(11)");
            });

            modelBuilder.Entity<MaterialsTags>(entity =>
            {
                entity.HasKey(e => new { e.MaterialId, e.TagId })
                    .HasName("PRIMARY");

                entity.ToTable("Materials_Tags");

                entity.HasIndex(e => e.TagId)
                    .HasDatabaseName("Materials_Tags_Tags_idx");

                entity.Property(e => e.MaterialId).HasColumnType("bigint(20)");

                entity.Property(e => e.TagId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.MaterialsTags)
                    .HasForeignKey(d => d.MaterialId)
                    .HasConstraintName("Materials_Tags_Materials");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.MaterialsTags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("Materials_Tags_Tags");
            });

            modelBuilder.Entity<Merchandise>(entity =>
            {
                entity.HasIndex(e => e.PreferredDisplayContainerId)
                    .HasDatabaseName("FK_Merchandises_GameItems_idx");

                entity.HasIndex(e => e.ShopId)
                    .HasDatabaseName("FK_Merchandises_Shops_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AutoReorderPrice).HasColumnType("decimal(58,29)");

                entity.Property(e => e.AutoReordering).HasColumnType("bit(1)");

                entity.Property(e => e.BasePrice).HasColumnType("decimal(58,29)");

                entity.Property(e => e.BaseBuyModifier)
                      .HasColumnType("decimal(58,29)")
                      .HasDefaultValue(0.3M)
                      ;

                entity.Property(e => e.MinimumConditionToBuy)
                      .HasColumnType("double")
                      .HasDefaultValue(0.95)
                      ;

				entity.Property(e => e.DefaultMerchandiseForItem).HasColumnType("bit(1)");

                entity.Property(e => e.WillSell)
                      .HasColumnType("bit(1)")
                      .HasDefaultValue(true)
                      ;

                entity.Property(e => e.WillBuy)
                      .HasColumnType("bit(1)")
                      .HasDefaultValue(false)
                      ;

				entity.Property(e => e.ItemProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.SkinId).HasColumnType("bigint(20)").IsRequired(false);

                entity.Property(e => e.ListDescription)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MinimumStockLevels).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PreferredDisplayContainerId).HasColumnType("bigint(20)");

                entity.Property(e => e.PreserveVariablesOnReorder).HasColumnType("bit(1)");

                entity.Property(e => e.ShopId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.PreferredDisplayContainer)
                    .WithMany(p => p.Merchandises)
                    .HasForeignKey(d => d.PreferredDisplayContainerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Merchandises_GameItems");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.Merchandises)
                    .HasForeignKey(d => d.ShopId)
                    .HasConstraintName("FK_Merchandises_Shops");
            });

            modelBuilder.Entity<Merit>(entity =>
            {
                entity.HasIndex(e => e.ParentId)
                    .HasDatabaseName("FK_Merits_Merits_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.MeritScope).HasColumnType("int(11)");

                entity.Property(e => e.MeritType).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ParentId).HasColumnType("bigint(20)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Merits_Merits");
            });

            modelBuilder.Entity<MeritsChargenResources>(entity =>
            {
                entity.HasKey(e => new { e.MeritId, e.ChargenResourceId, e.RequirementOnly })
                    .HasName("PRIMARY");

                entity.ToTable("Merits_ChargenResources");

                entity.HasIndex(e => e.ChargenResourceId)
                    .HasDatabaseName("FK_Merits_ChargenResources_ChargenResources_idx");

                entity.Property(e => e.MeritId).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenResourceId).HasColumnType("bigint(20)");

                entity.Property(e => e.RequirementOnly)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Amount).HasColumnType("int(11)");

                entity.HasOne(d => d.ChargenResource)
                    .WithMany(p => p.MeritsChargenResources)
                    .HasForeignKey(d => d.ChargenResourceId)
                    .HasConstraintName("FK_Merits_ChargenResources_ChargenResources");

                entity.HasOne(d => d.Merit)
                    .WithMany(p => p.MeritsChargenResources)
                    .HasForeignKey(d => d.MeritId)
                    .HasConstraintName("FK_Merits_ChargenResources_Merits");
            });

            modelBuilder.Entity<MoveSpeed>(entity =>
            {
                entity.HasIndex(e => e.BodyProtoId)
                    .HasDatabaseName("FK_MoveSpeeds_BodyPrototype");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Alias)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.BodyProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.FirstPersonVerb)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PositionId).HasColumnType("bigint(20)");

                entity.Property(e => e.PresentParticiple)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.StaminaMultiplier).HasDefaultValueSql("'1'");

                entity.Property(e => e.ThirdPersonVerb)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.BodyProto)
                    .WithMany(p => p.MoveSpeeds)
                    .HasForeignKey(d => d.BodyProtoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MoveSpeeds_BodyPrototype");
            });

            modelBuilder.Entity<MutualIntelligability>(entity =>
            {
                entity.HasKey(e => new { e.ListenerLanguageId, e.TargetLanguageId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.TargetLanguageId)
                    .HasDatabaseName("FK_Languages_MutualIntelligabilities_Target_idx");

                entity.Property(e => e.ListenerLanguageId).HasColumnType("bigint(20)");

                entity.Property(e => e.TargetLanguageId).HasColumnType("bigint(20)");

                entity.Property(e => e.IntelligabilityDifficulty).HasColumnType("int(11)");

                entity.HasOne(d => d.ListenerLanguage)
                    .WithMany(p => p.MutualIntelligabilitiesListenerLanguage)
                    .HasForeignKey(d => d.ListenerLanguageId)
                    .HasConstraintName("FK_Languages_MutualIntelligabilities_Listener");

                entity.HasOne(d => d.TargetLanguage)
                    .WithMany(p => p.MutualIntelligabilitiesTargetLanguage)
                    .HasForeignKey(d => d.TargetLanguageId)
                    .HasConstraintName("FK_Languages_MutualIntelligabilities_Target");
            });

            modelBuilder.Entity<NameCulture>(entity =>
            {
                entity.ToTable("NameCulture");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<NewPlayerHint>(entity => {
				entity.ToTable("NewPlayerHints");
				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.Priority).HasColumnType("int(11)");
				entity.Property(e => e.CanRepeat).HasColumnType("bit(1)");
				entity.Property(e => e.Text)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

                entity.HasOne(e => e.FilterProg)
                .WithMany()
                .HasForeignKey(e => e.FilterProgId)
                .HasConstraintName("FK_NewPlayerHints_FutureProgs");
			});

            modelBuilder.Entity<NonCardinalExitTemplate>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.DestinationInboundPreface)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DestinationOutboundPreface)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.InboundVerb)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.OriginInboundPreface)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.OriginOutboundPreface)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.OutboundVerb)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Npc>(entity =>
            {
                entity.ToTable("NPCs");

                entity.HasIndex(e => e.BodyguardCharacterId)
                    .HasDatabaseName("FK_NPCs_Characters_Bodyguard_idx");

                entity.HasIndex(e => e.CharacterId)
                    .HasDatabaseName("FK_NPCs_Characters");

                entity.HasIndex(e => new { e.TemplateId, e.TemplateRevnum })
                    .HasDatabaseName("FK_NPCs_NPCTemplates");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BodyguardCharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.TemplateId).HasColumnType("bigint(20)");

                entity.Property(e => e.TemplateRevnum).HasColumnType("int(11)");

                entity.HasOne(d => d.BodyguardCharacter)
                    .WithMany(p => p.NpcsBodyguardCharacter)
                    .HasForeignKey(d => d.BodyguardCharacterId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_NPCs_Characters_Bodyguard");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.NpcsCharacter)
                    .HasForeignKey(d => d.CharacterId)
                    .HasConstraintName("FK_NPCs_Characters");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.Npcs)
                    .HasForeignKey(d => new { d.TemplateId, d.TemplateRevnum })
                    .HasConstraintName("FK_NPCs_NPCTemplates");
            });

            modelBuilder.Entity<NpcsArtificialIntelligences>(entity =>
            {
                entity.HasKey(e => new { e.ArtificialIntelligenceId, e.Npcid })
                    .HasName("PRIMARY");

                entity.ToTable("NPCs_ArtificialIntelligences");

                entity.HasIndex(e => e.Npcid)
                    .HasDatabaseName("FK_NPCs_ArtificialIntelligences_NPCs");

                entity.Property(e => e.ArtificialIntelligenceId).HasColumnType("bigint(20)");

                entity.Property(e => e.Npcid)
                    .HasColumnName("NPCId")
                    .HasColumnType("bigint(20)");

                entity.HasOne(d => d.ArtificialIntelligence)
                    .WithMany(p => p.NpcsArtificialIntelligences)
                    .HasForeignKey(d => d.ArtificialIntelligenceId)
                    .HasConstraintName("FK_NPCs_ArtificialIntelligences_ArtificialIntelligences");

                entity.HasOne(d => d.Npc)
                    .WithMany(p => p.NpcsArtificialIntelligences)
                    .HasForeignKey(d => d.Npcid)
                    .HasConstraintName("FK_NPCs_ArtificialIntelligences_NPCs");
            });

            modelBuilder.Entity<NPCSpawner>(entity =>
            {
	            entity.ToTable("NPCSpawners");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.TargetTemplateId).IsRequired(false).HasColumnType("bigint(20)");
                entity.Property(e => e.OnSpawnProgId).IsRequired(false).HasColumnType("bigint(20)");
                entity.Property(e => e.CountsAsProgId).IsRequired(false).HasColumnType("bigint(20)");
                entity.Property(e => e.IsActiveProgId).IsRequired(false).HasColumnType("bigint(20)");
                entity.Property(e => e.TargetCount).HasColumnType("int(11)");
                entity.Property(e => e.MinimumCount).HasColumnType("int(11)");
                entity.Property(e => e.SpawnStrategy).HasColumnType("int(11)");
                entity.Property(e => e.Name)
	                .IsRequired()
	                .HasColumnType("varchar(255)")
	                .HasCharSet("utf8")
	                .UseCollation("utf8_general_ci");

                entity.HasOne(e => e.CountsAsProg)
	                .WithMany()
	                .HasForeignKey(e => e.CountsAsProgId)
	                .OnDelete(DeleteBehavior.SetNull)
	                .HasConstraintName("FK_NPCSpawners_CountsAsProg");

                entity.HasOne(e => e.IsActiveProg)
	                .WithMany()
	                .HasForeignKey(e => e.IsActiveProgId)
	                .OnDelete(DeleteBehavior.SetNull)
	                .HasConstraintName("FK_NPCSpawners_IsActiveProg");

                entity.HasOne(e => e.OnSpawnProg)
	                .WithMany()
	                .HasForeignKey(e => e.OnSpawnProgId)
	                .OnDelete(DeleteBehavior.SetNull)
	                .HasConstraintName("FK_NPCSpawners_OnSpawnProg");
            });

            modelBuilder.Entity<NPCSpawnerCell>(entity =>
            {
	            entity.ToTable("NPCSpawnerCells");
	            entity.Property(e => e.CellId).HasColumnType("bigint(20)");
	            entity.Property(e => e.NPCSpawnerId).HasColumnType("bigint(20)");
	            entity.HasKey(e => new { e.NPCSpawnerId, e.CellId })
		            .HasName("PRIMARY");

	            entity.HasOne(e => e.NPCSpawner)
		            .WithMany(e => e.Cells)
		            .HasForeignKey(e => e.NPCSpawnerId)
		            .OnDelete(DeleteBehavior.Cascade)
		            .HasConstraintName("FK_NPCSpawnerCells_NPCSpawner");

	            entity.HasOne(e => e.Cell)
		            .WithMany()
		            .HasForeignKey(e => e.CellId)
		            .OnDelete(DeleteBehavior.Cascade)
		            .HasConstraintName("FK_NPCSpawnerCells_Cell");
            });

            modelBuilder.Entity<NPCSpawnerZone>(entity =>
            {
	            entity.ToTable("NPCSpawnerZones");
	            entity.Property(e => e.ZoneId).HasColumnType("bigint(20)");
	            entity.Property(e => e.NPCSpawnerId).HasColumnType("bigint(20)");
	            entity.HasKey(e => new { e.NPCSpawnerId, e.ZoneId })
		            .HasName("PRIMARY");

	            entity.HasOne(e => e.NPCSpawner)
		            .WithMany(e => e.Zones)
		            .HasForeignKey(e => e.NPCSpawnerId)
		            .OnDelete(DeleteBehavior.Cascade)
		            .HasConstraintName("FK_NPCSpawnerZones_NPCSpawner");

	            entity.HasOne(e => e.Zone)
		            .WithMany()
		            .HasForeignKey(e => e.ZoneId)
		            .OnDelete(DeleteBehavior.Cascade)
		            .HasConstraintName("FK_NPCSpawnerZones_Zone");
            });

            modelBuilder.Entity<NpcTemplate>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.RevisionNumber })
                    .HasName("PRIMARY");

                entity.ToTable("NPCTemplates");

                entity.HasIndex(e => e.EditableItemId)
                    .HasDatabaseName("FK_NPCTemplates_EditableItems");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.EditableItem)
                    .WithMany(p => p.Npctemplates)
                    .HasForeignKey(d => d.EditableItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NPCTemplates_EditableItems");
            });

            modelBuilder.Entity<NpcTemplatesArtificalIntelligences>(entity =>
            {
                entity.HasKey(e => new { e.NpcTemplateRevisionNumber, e.NpcTemplateId, e.AiId })
                    .HasName("PRIMARY");

                entity.ToTable("NPCTemplates_ArtificalIntelligences");

                entity.HasIndex(e => e.AiId)
                    .HasDatabaseName("FK_NTAI_ArtificalIntelligences");

                entity.HasIndex(e => new { e.NpcTemplateId, e.NpcTemplateRevisionNumber })
                    .HasDatabaseName("FK_NTAI_NPCTemplates");

                entity.Property(e => e.NpcTemplateRevisionNumber)
                    .HasColumnName("NPCTemplateRevisionNumber")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NpcTemplateId)
                    .HasColumnName("NPCTemplateId")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AiId)
                    .HasColumnName("AIId")
                    .HasColumnType("bigint(20)");

                entity.HasOne(d => d.Ai)
                    .WithMany(p => p.NpctemplatesArtificalIntelligences)
                    .HasForeignKey(d => d.AiId)
                    .HasConstraintName("FK_NTAI_ArtificalIntelligences");

                entity.HasOne(d => d.Npctemplate)
                    .WithMany(p => p.NpctemplatesArtificalIntelligences)
                    .HasForeignKey(d => new { d.NpcTemplateId, d.NpcTemplateRevisionNumber })
                    .HasConstraintName("FK_NTAI_NPCTemplates");
            });

            modelBuilder.Entity<Patrol>(entity =>
            {
                entity.ToTable("Patrols");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.PatrolRouteId).HasColumnType("bigint(20)");
                entity.Property(e => e.LegalAuthorityId).HasColumnType("bigint(20)");
                entity.Property(e => e.PatrolPhase).HasColumnType("int(11)");
                entity.Property(e => e.LastMajorNodeId).HasColumnType("bigint(20)");
                entity.Property(e => e.NextMajorNodeId).HasColumnType("bigint(20)");
                entity.Property(e => e.PatrolLeaderId).HasColumnType("bigint(20)");

                entity.HasIndex(e => e.PatrolRouteId).HasDatabaseName("FK_Patrols_PatrolRoutes_idx");
                entity.HasIndex(e => e.LegalAuthorityId).HasDatabaseName("FK_Patrols_LegalAuthorities_idx");
                entity.HasIndex(e => e.LastMajorNodeId).HasDatabaseName("FK_Patrols_LastMajorNode_idx");
                entity.HasIndex(e => e.NextMajorNodeId).HasDatabaseName("FK_Patrols_NextMajorNode_idx");
                entity.HasIndex(e => e.PatrolLeaderId).HasDatabaseName("FK_Patrols_Characters_idx");

                entity
                .HasOne(e => e.PatrolRoute)
                .WithMany(e => e.Patrols)
                .HasForeignKey(e => e.PatrolRouteId)
                .HasConstraintName("FK_Patrols_PatrolRoutes");

                entity
                .HasOne(e => e.LegalAuthority)
                .WithMany(e => e.Patrols)
                .HasForeignKey(e => e.LegalAuthorityId)
                .HasConstraintName("FK_Patrols_LegalAuthorities");

                entity
                .HasOne(e => e.LastMajorNode)
                .WithMany()
                .HasForeignKey(e => e.LastMajorNodeId)
                .HasConstraintName("FK_Patrols_LastMajorNode");

                entity
                .HasOne(e => e.NextMajorNode)
                .WithMany()
                .HasForeignKey(e => e.NextMajorNodeId)
                .HasConstraintName("FK_Patrols_NextMajorNode");

                entity
                .HasOne(e => e.PatrolLeader)
                .WithMany()
                .HasForeignKey(e => e.PatrolLeaderId)
                .HasConstraintName("FK_Patrols_Characters");
            });

            modelBuilder.Entity<PatrolMember>(entity =>
            {
                entity.ToTable("PatrolMembers");
                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
                entity.Property(e => e.PatrolId).HasColumnType("bigint(20)");
                entity.HasKey(e => new { e.PatrolId, e.CharacterId }).HasName("PRIMARY");

                entity
                .HasOne(e => e.Character)
                .WithMany()
                .HasForeignKey(e => e.CharacterId)
                .HasConstraintName("FK_PatrolMembers_Characters");

                entity
                .HasOne(e => e.Patrol)
                .WithMany(e => e.PatrolMembers)
                .HasForeignKey(e => e.PatrolId)
                .HasConstraintName("FK_PatrolsMembers_Patrols");
            });

            modelBuilder.Entity<PatrolRoute>(entity => {
                entity.ToTable("PatrolRoutes");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.LegalAuthorityId).HasColumnType("bigint(20)");
                entity.Property(e => e.Name).HasColumnType("varchar(100)");
                entity.Property(e => e.PatrolStrategy).HasColumnType("varchar(100)");
                entity.Property(e => e.Priority).HasColumnType("int(11)");
                entity.Property(e => e.IsReady).HasColumnType("bit(1)").HasDefaultValueSql("b'0'");
                entity.Property(e => e.LingerTimeMajorNode).HasColumnType("DOUBLE");
                entity.Property(e => e.LingerTimeMinorNode).HasColumnType("DOUBLE");

                entity.HasIndex(e => e.LegalAuthorityId)
                    .HasDatabaseName("FK_PatrolRoutes_LegalAuthorities_idx");
                entity
                    .HasOne(e => e.LegalAuthority)
                    .WithMany(e => e.PatrolRoutes)
                    .HasForeignKey(e => e.LegalAuthorityId)
                    .HasConstraintName("FK_PatrolRoutes_LegalAuthorities");
            });

            modelBuilder.Entity<PatrolRouteTimeOfDay>(entity => {
                entity.ToTable("PatrolRoutesTimesOfDay");
                entity.Property(e => e.PatrolRouteId).HasColumnType("bigint(20)");
                entity.Property(e => e.TimeOfDay).HasColumnType("int(11)");
                entity.HasKey(e => new { e.PatrolRouteId, e.TimeOfDay }).HasName("PRIMARY");
                entity.HasIndex(e => e.PatrolRouteId).HasDatabaseName("FK_PatrolRoutesTimesOfDay_PatrolRoutes_idx");
                entity
                    .HasOne(e => e.PatrolRoute)
                    .WithMany(e => e.TimesOfDay)
                    .HasForeignKey(e => e.PatrolRouteId)
                    .HasConstraintName("FK_PatrolRoutesTimesOfDay_PatrolRoutes");
            });

            modelBuilder.Entity<PatrolRouteNode>(entity => {
                entity.ToTable("PatrolRoutesNodes");
                entity.Property(e => e.PatrolRouteId).HasColumnType("bigint(20)");
                entity.Property(e => e.CellId).HasColumnType("bigint(20)");
                entity.Property(e => e.Order).HasColumnType("int(11)");
                entity.HasKey(e => new { e.PatrolRouteId, e.CellId }).HasName("PRIMARY");
                entity.HasIndex(e => e.PatrolRouteId).HasDatabaseName("FK_PatrolRoutesNodes_PatrolRoutes_idx");
                entity.HasIndex(e => e.CellId).HasDatabaseName("FK_PatrolRoutesNodes_Cells_idx");

                entity
                    .HasOne(e => e.PatrolRoute)
                    .WithMany(e => e.PatrolRouteNodes)
                    .HasForeignKey(e => e.PatrolRouteId)
                    .HasConstraintName("FK_PatrolRoutesNodes_PatrolRoutes");

                entity
                    .HasOne(e => e.Cell)
                    .WithMany()
                    .HasForeignKey(e => e.CellId)
                    .HasConstraintName("FK_PatrolRoutesNodes_Cells");
            });

            modelBuilder.Entity<PatrolRouteNumbers>(entity => {
                entity.ToTable("PatrolRoutesNumbers");
                entity.Property(e => e.PatrolRouteId).HasColumnType("bigint(20)");
                entity.Property(e => e.EnforcementAuthorityId).HasColumnType("bigint(20)");
                entity.Property(e => e.NumberRequired).HasColumnType("int(11)");
                entity.HasKey(e => new { e.PatrolRouteId, e.EnforcementAuthorityId }).HasName("PRIMARY");
                entity.HasIndex(e => e.PatrolRouteId).HasDatabaseName("FK_PatrolRoutesNumbers_PatrolRoutes_idx");
                entity.HasIndex(e => e.EnforcementAuthorityId).HasDatabaseName("FK_PatrolRoutesNumbers_EnforcementAuthorities_idx");

                entity
                    .HasOne(e => e.PatrolRoute)
                    .WithMany(e => e.PatrolRouteNumbers)
                    .HasForeignKey(e => e.PatrolRouteId)
                    .HasConstraintName("FK_PatrolRoutesNumbers_PatrolRoutes");

                entity
                    .HasOne(e => e.EnforcementAuthority)
                    .WithMany()
                    .HasForeignKey(e => e.EnforcementAuthorityId)
                    .HasConstraintName("FK_PatrolRoutesNumbers_EnforcementAuthorities");
            });

            modelBuilder.Entity<Paygrade>(entity =>
            {
                entity.HasIndex(e => e.ClanId)
                    .HasDatabaseName("FK_Paygrades_Clans");

                entity.HasIndex(e => e.CurrencyId)
                    .HasDatabaseName("FK_Paygrades_Currencies");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Abbreviation)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ClanId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PayAmount).HasColumnType("decimal(58,29)");

                entity.HasOne(d => d.Clan)
                    .WithMany(p => p.Paygrades)
                    .HasForeignKey(d => d.ClanId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Paygrades_Clans");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Paygrades)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Paygrades_Currencies");
            });

            modelBuilder.Entity<PerceiverMerit>(entity =>
            {
                entity.HasIndex(e => e.BodyId)
                    .HasDatabaseName("FK_PerceiverMerits_Bodies_idx");

                entity.HasIndex(e => e.CharacterId)
                    .HasDatabaseName("FK_PerceiverMerits_Characters_idx");

                entity.HasIndex(e => e.GameItemId)
                    .HasDatabaseName("FK_PerceiverMerits_GameItems_idx");

                entity.HasIndex(e => e.MeritId)
                    .HasDatabaseName(" FK_PerceiverMerits_Merits_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");

                entity.Property(e => e.MeritId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Body)
                    .WithMany(p => p.PerceiverMerits)
                    .HasForeignKey(d => d.BodyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PerceiverMerits_Bodies");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.PerceiverMerits)
                    .HasForeignKey(d => d.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PerceiverMerits_Characters");

                entity.HasOne(d => d.Merit)
                    .WithMany(p => p.PerceiverMerits)
                    .HasForeignKey(d => d.MeritId)
                    .HasConstraintName(" FK_PerceiverMerits_Merits");
            });

            modelBuilder.Entity<PopulationBloodModel>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<PopulationBloodModelsBloodtype>(entity =>
            {
                entity.HasKey(e => new { e.BloodtypeId, e.PopulationBloodModelId })
                    .HasName("PRIMARY");

                entity.ToTable("PopulationBloodModels_Bloodtypes");

                entity.HasIndex(e => e.PopulationBloodModelId)
                    .HasDatabaseName("FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels_idx");

                entity.Property(e => e.BloodtypeId).HasColumnType("bigint(20)");

                entity.Property(e => e.PopulationBloodModelId).HasColumnType("bigint(20)");

                entity.Property(e => e.Weight).HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Bloodtype)
                    .WithMany(p => p.PopulationBloodModelsBloodtypes)
                    .HasForeignKey(d => d.BloodtypeId)
                    .HasConstraintName("FK_PopulationBloodModels_Bloodtypes_Bloodtypes");

                entity.HasOne(d => d.PopulationBloodModel)
                    .WithMany(p => p.PopulationBloodModelsBloodtypes)
                    .HasForeignKey(d => d.PopulationBloodModelId)
                    .HasConstraintName("FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels");
            });

            modelBuilder.Entity<ProgSchedule>(entity =>
            {
                entity.HasIndex(e => e.FutureProgId)
                    .HasDatabaseName("FK_ProgSchedules_FutureProgs_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.IntervalModifier).HasColumnType("int(11)");

                entity.Property(e => e.IntervalOther).HasColumnType("int(11)");

                entity.Property(e => e.IntervalType).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ReferenceDate)
                    .IsRequired()
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ReferenceTime)
                    .IsRequired()
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasOne(d => d.FutureProg)
                    .WithMany(p => p.ProgSchedules)
                    .HasForeignKey(d => d.FutureProgId)
                    .HasConstraintName("FK_ProgSchedules_FutureProgs");
            });

            modelBuilder.Entity<ProjectAction>(entity =>
            {
                entity.HasIndex(e => e.ProjectPhaseId)
                    .HasDatabaseName("FK_ProjectActions_ProjectPhases_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ProjectPhaseId).HasColumnType("bigint(20)");

                entity.Property(e => e.SortOrder).HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.ProjectPhase)
                    .WithMany(p => p.ProjectActions)
                    .HasForeignKey(d => d.ProjectPhaseId)
                    .HasConstraintName("FK_ProjectActions_ProjectPhases");
            });

            modelBuilder.Entity<ProjectLabourImpact>(entity =>
            {
                entity.HasIndex(e => e.ProjectLabourRequirementId)
                    .HasDatabaseName("FK_ProjectLabourImpacts_ProjectLabourRequirements_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ProjectLabourRequirementId).HasColumnType("bigint(20)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.ProjectLabourRequirement)
                    .WithMany(p => p.ProjectLabourImpacts)
                    .HasForeignKey(d => d.ProjectLabourRequirementId)
                    .HasConstraintName("FK_ProjectLabourImpacts_ProjectLabourRequirements");
            });

            modelBuilder.Entity<ProjectLabourRequirement>(entity =>
            {
                entity.HasIndex(e => e.ProjectPhaseId)
                    .HasDatabaseName("FK_ProjectLabourRequirements_ProjectPhases_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MaximumSimultaneousWorkers).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ProjectPhaseId).HasColumnType("bigint(20)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.ProjectPhase)
                    .WithMany(p => p.ProjectLabourRequirements)
                    .HasForeignKey(d => d.ProjectPhaseId)
                    .HasConstraintName("FK_ProjectLabourRequirements_ProjectPhases");
            });

            modelBuilder.Entity<ProjectMaterialRequirement>(entity =>
            {
                entity.HasIndex(e => e.ProjectPhaseId)
                    .HasDatabaseName("FK_ProjectMaterialRequirements_ProjectPhases_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.IsMandatoryForProjectCompletion)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ProjectPhaseId).HasColumnType("bigint(20)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.ProjectPhase)
                    .WithMany(p => p.ProjectMaterialRequirements)
                    .HasForeignKey(d => d.ProjectPhaseId)
                    .HasConstraintName("FK_ProjectMaterialRequirements_ProjectPhases");
            });

            modelBuilder.Entity<ProjectPhase>(entity =>
            {
                entity.HasIndex(e => new { e.ProjectId, e.ProjectRevisionNumber })
                    .HasDatabaseName("FK_ProjectPhases_Projects_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PhaseNumber).HasColumnType("int(11)");

                entity.Property(e => e.ProjectId).HasColumnType("bigint(20)");

                entity.Property(e => e.ProjectRevisionNumber).HasColumnType("int(11)");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectPhases)
                    .HasForeignKey(d => new { d.ProjectId, d.ProjectRevisionNumber })
                    .HasConstraintName("FK_ProjectPhases_Projects");
            });

            modelBuilder.Entity<Models.Project>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.RevisionNumber })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.EditableItemId)
                    .HasDatabaseName("FK_Projects_EditableItems_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");

                entity.Property(e => e.AppearInJobsList).HasColumnType("bit(1)").HasDefaultValue(false);

                entity.Property(e => e.Definition)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.EditableItem)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.EditableItemId)
                    .HasConstraintName("FK_Projects_EditableItems");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Properties");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
                entity.Property(e => e.LeaseId).HasColumnType("bigint(20)");
                entity.Property(e => e.LeaseOrderId).HasColumnType("bigint(20)");
                entity.Property(e => e.SaleOrderId).HasColumnType("bigint(20)");
                entity.Property(e => e.ApplyCriminalCodeInProperty).HasColumnType("bit(1)");
                entity.Property(e => e.LastSaleValue).HasColumnType("decimal(58,29)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DetailedDescription)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastChangeOfOwnership)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.EconomicZone)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.EconomicZoneId)
                    .HasConstraintName("FK_Properties_EconomicZones");

                entity.HasOne(d => d.Lease)
                    .WithMany()
                    .HasForeignKey(d => d.LeaseId)
                    .HasConstraintName("FK_Properties_Lease");

                entity.HasOne(d => d.LeaseOrder)
                    .WithMany()
                    .HasForeignKey(d => d.LeaseOrderId)
                    .HasConstraintName("FK_Properties_LeaseOrder");

                entity.HasOne(d => d.SaleOrder)
                    .WithMany()
                    .HasForeignKey(d => d.SaleOrderId)
                    .HasConstraintName("FK_Properties_SaleOrder");
            });

            modelBuilder.Entity<PropertyKey>(entity =>
            {
                entity.ToTable("PropertyKeys");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.PropertyId).HasColumnType("bigint(20)");
                entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");
                entity.Property(e => e.IsReturned).HasColumnType("bit(1)");
                entity.Property(e => e.CostToReplace).HasColumnType("decimal(58,29)");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.AddedToPropertyOnDate)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(e => e.Property)
                    .WithMany(e => e.PropertyKeys)
                    .HasForeignKey(e => e.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PropertyKeys_Property");

                entity.HasOne(e => e.GameItem)
                    .WithMany()
                    .HasForeignKey(e => e.GameItemId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PropertyKeys_GameItems");

            });

            modelBuilder.Entity<PropertyLease>(entity =>
            {
                entity.ToTable("PropertyLeases");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.PropertyId).HasColumnType("bigint(20)");
                entity.Property(e => e.LeaseOrderId).HasColumnType("bigint(20)");
                entity.Property(e => e.AutoRenew).HasColumnType("bit(1)");
                entity.Property(e => e.BondReturned).HasColumnType("bit(1)");
                entity.Property(e => e.PricePerInterval).HasColumnType("decimal(58,29)");
                entity.Property(e => e.BondPayment).HasColumnType("decimal(58,29)");
                entity.Property(e => e.PaymentBalance).HasColumnType("decimal(58,29)");

                entity.Property(e => e.LeaseholderReference)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.LeaseStart)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.LeaseEnd)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.LastLeasePayment)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.Interval)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.TenantInfo)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Property)
                    .WithMany(e => e.PropertyLeases)
                    .HasForeignKey(d => d.PropertyId)
                    .HasConstraintName("FK_PropertyLeases_Property");

                entity.HasOne(d => d.LeaseOrder)
                    .WithMany(e => e.PropertyLeases)
                    .HasForeignKey(d => d.LeaseOrderId)
                    .HasConstraintName("FK_PropertyLeases_PropertyLeaseOrders");
            });

            modelBuilder.Entity<PropertyLeaseOrder>(entity =>
            {
                entity.ToTable("PropertyLeaseOrders");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.PropertyId).HasColumnType("bigint(20)");
                entity.Property(e => e.CanLeaseProgCharacterId).HasColumnType("bigint(20)");
                entity.Property(e => e.CanLeaseProgClanId).HasColumnType("bigint(20)");
                entity.Property(e => e.AllowAutoRenew).HasColumnType("bit(1)");
                entity.Property(e => e.AutomaticallyRelistAfterLeaseTerm).HasColumnType("bit(1)");
                entity.Property(e => e.AllowLeaseNovation).HasColumnType("bit(1)");
                entity.Property(e => e.ListedForLease).HasColumnType("bit(1)");
                entity.Property(e => e.MinimumLeaseDurationDays).HasColumnType("double");
                entity.Property(e => e.MaximumLeaseDurationDays).HasColumnType("double");
                entity.Property(e => e.PricePerInterval).HasColumnType("decimal(58,29)");
                entity.Property(e => e.BondRequired).HasColumnType("decimal(58,29)");
                entity.Property(e => e.FeeIncreasePercentageAfterLeaseTerm).HasColumnType("decimal(58,29)");

                entity.Property(e => e.Interval)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PropertyOwnerConsentInfo)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Property)
                    .WithMany(e => e.LeaseOrders)
                    .HasForeignKey(d => d.PropertyId)
                    .HasConstraintName("FK_PropertyLeaseOrders_Property");

                entity.HasOne(d => d.CanLeaseProgCharacter)
                    .WithMany()
                    .HasForeignKey(d => d.CanLeaseProgCharacterId)
                    .HasConstraintName("FK_PropertyLeaseOrders_FutureProgs_Character");

                entity.HasOne(d => d.CanLeaseProgClan)
                    .WithMany()
                    .HasForeignKey(d => d.CanLeaseProgClanId)
                    .HasConstraintName("FK_PropertyLeaseOrders_FutureProgs_Clan");
            });

            modelBuilder.Entity<PropertySaleOrder>(entity =>
            {
                entity.ToTable("PropertySalesOrders");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.PropertyId).HasColumnType("bigint(20)");
                entity.Property(e => e.OrderStatus).HasColumnType("int(11)");
                entity.Property(e => e.DurationOfListingDays).HasColumnType("double");
                entity.Property(e => e.ReservePrice).HasColumnType("decimal(58,29)");

                entity.Property(e => e.StartOfListing)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PropertyOwnerConsentInfo)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Property)
                    .WithMany()
                    .HasForeignKey(d => d.PropertyId)
                    .HasConstraintName("FK_PropertySaleOrders_Property");
            });

            modelBuilder.Entity<PropertyLocation>(entity =>
            {
                entity.ToTable("PropertyLocations");
                entity.HasKey(x => new { x.PropertyId, x.CellId }).HasName("PRIMARY");

                entity.Property(e => e.PropertyId).HasColumnType("bigint(20)");
                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Property)
                    .WithMany(e => e.PropertyLocations)
                    .HasForeignKey(d => d.PropertyId)
                    .HasConstraintName("FK_PropertyLocations_Property");
                entity.HasOne(d => d.Cell)
                    .WithMany()
                    .HasForeignKey(d => d.CellId)
                    .HasConstraintName("FK_PropertyLocations_Cell");
            });

            modelBuilder.Entity<PropertyOwner>(entity =>
            {
                entity.ToTable("PropertyOwners");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.PropertyId).HasColumnType("bigint(20)");
                entity.Property(e => e.FrameworkItemId).HasColumnType("bigint(20)");
                entity.Property(e => e.RevenueAccountId).IsRequired(false).HasColumnType("bigint(20)");
                entity.Property(e => e.ShareOfOwnership).HasColumnType("decimal(58,29)");
                entity.Property(e => e.FrameworkItemType)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(e => e.Property)
                    .WithMany(e => e.PropertyOwners)
                    .HasForeignKey(e => e.PropertyId)
                    .HasConstraintName("FK_PropertyOwners_Properties");

                entity.HasOne(e => e.RevenueAccount)
                    .WithMany()
                    .HasForeignKey(e => e.RevenueAccountId)
                    .HasConstraintName("FK_PropertyOwners_BankAccounts");
            });

            modelBuilder.Entity<RaceButcheryProfile>(entity =>
            {
                entity.HasIndex(e => e.CanButcherProgId)
                    .HasDatabaseName("FK_RaceButcheryProfiles_FutureProgs_Can_idx");

                entity.HasIndex(e => e.RequiredToolTagId)
                    .HasDatabaseName("FK_RaceButcheryProfiles_Tags_idx");

                entity.HasIndex(e => e.WhyCannotButcherProgId)
                    .HasDatabaseName("FK_RaceButcheryProfiles_FutureProgs_Why_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CanButcherProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.DifficultySkin).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.RequiredToolTagId).HasColumnType("bigint(20)");

                entity.Property(e => e.Verb).HasColumnType("int(11)");

                entity.Property(e => e.WhyCannotButcherProgId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.CanButcherProg)
                    .WithMany(p => p.RaceButcheryProfilesCanButcherProg)
                    .HasForeignKey(d => d.CanButcherProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_RaceButcheryProfiles_FutureProgs_Can");

                entity.HasOne(d => d.RequiredToolTag)
                    .WithMany(p => p.RaceButcheryProfiles)
                    .HasForeignKey(d => d.RequiredToolTagId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_RaceButcheryProfiles_Tags");

                entity.HasOne(d => d.WhyCannotButcherProg)
                    .WithMany(p => p.RaceButcheryProfilesWhyCannotButcherProg)
                    .HasForeignKey(d => d.WhyCannotButcherProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_RaceButcheryProfiles_FutureProgs_Why");
            });

            modelBuilder.Entity<RaceButcheryProfilesBreakdownChecks>(entity =>
            {
                entity.HasKey(e => new { e.RaceButcheryProfileId, e.Subcageory })
                    .HasName("PRIMARY");

                entity.ToTable("RaceButcheryProfiles_BreakdownChecks");

                entity.HasIndex(e => e.TraitDefinitionId)
                    .HasDatabaseName("FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions_idx");

                entity.Property(e => e.RaceButcheryProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.Subcageory)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Difficulty).HasColumnType("int(11)");

                entity.Property(e => e.TraitDefinitionId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.RaceButcheryProfile)
                    .WithMany(p => p.RaceButcheryProfilesBreakdownChecks)
                    .HasForeignKey(d => d.RaceButcheryProfileId)
                    .HasConstraintName("FK_RaceButcheryProfiles_BreakdownChecks_RaceButcheryProfiles");

                entity.HasOne(d => d.TraitDefinition)
                    .WithMany(p => p.RaceButcheryProfilesBreakdownChecks)
                    .HasForeignKey(d => d.TraitDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions");
            });

            modelBuilder.Entity<RaceButcheryProfilesBreakdownEmotes>(entity =>
            {
                entity.HasKey(e => new { e.RaceButcheryProfileId, e.Subcategory, e.Order })
                    .HasName("PRIMARY");

                entity.ToTable("RaceButcheryProfiles_BreakdownEmotes");

                entity.Property(e => e.RaceButcheryProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.Subcategory)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Order).HasColumnType("int(11)");

                entity.Property(e => e.Emote)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.RaceButcheryProfile)
                    .WithMany(p => p.RaceButcheryProfilesBreakdownEmotes)
                    .HasForeignKey(d => d.RaceButcheryProfileId)
                    .HasConstraintName("FK_RaceButcheryProfiles_BreakdownEmotes_RaceButcheryProfiles");
            });

            modelBuilder.Entity<RaceButcheryProfilesButcheryProducts>(entity =>
            {
                entity.HasKey(e => new { e.RaceButcheryProfileId, e.ButcheryProductId })
                    .HasName("PRIMARY");

                entity.ToTable("RaceButcheryProfiles_ButcheryProducts");

                entity.HasIndex(e => e.ButcheryProductId)
                    .HasDatabaseName("FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts_idx");

                entity.Property(e => e.RaceButcheryProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.ButcheryProductId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ButcheryProduct)
                    .WithMany(p => p.RaceButcheryProfilesButcheryProducts)
                    .HasForeignKey(d => d.ButcheryProductId)
                    .HasConstraintName("FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts");

                entity.HasOne(d => d.RaceButcheryProfile)
                    .WithMany(p => p.RaceButcheryProfilesButcheryProducts)
                    .HasForeignKey(d => d.RaceButcheryProfileId)
                    .HasConstraintName("FK_RaceButcheryProfiles_ButcheryProducts_RaceButcheryProfiles");
            });

            modelBuilder.Entity<RaceButcheryProfilesSkinningEmotes>(entity =>
            {
                entity.HasKey(e => new { e.RaceButcheryProfileId, e.Subcategory, e.Order })
                    .HasName("PRIMARY");

                entity.ToTable("RaceButcheryProfiles_SkinningEmotes");

                entity.Property(e => e.RaceButcheryProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.Subcategory)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Order).HasColumnType("int(11)");

                entity.Property(e => e.Emote)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.RaceButcheryProfile)
                    .WithMany(p => p.RaceButcheryProfilesSkinningEmotes)
                    .HasForeignKey(d => d.RaceButcheryProfileId)
                    .HasConstraintName("FK_RaceButcheryProfiles_SkinningEmotes_RaceButcheryProfiles");
            });

            modelBuilder.Entity<RaceEdibleForagableYields>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.YieldType })
                    .HasName("PRIMARY");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.YieldType)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.EatEmote)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasDefaultValueSql("'@ eat|eats {0} from the location.'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Race)
                    .WithMany(p => p.RaceEdibleForagableYields)
                    .HasForeignKey(d => d.RaceId)
                    .HasConstraintName("FK_RaceEdibleForagableYields_Races");
            });

            modelBuilder.Entity<Race>(entity =>
            {
                entity.HasIndex(e => e.AttributeBonusProgId)
                    .HasDatabaseName("FK_Races_AttributeBonusProg");

                entity.HasIndex(e => e.AvailabilityProgId)
                    .HasDatabaseName("FK_Races_AvailabilityProg");

                entity.HasIndex(e => e.BaseBodyId)
                    .HasDatabaseName("FK_Races_BodyProtos");

                entity.HasIndex(e => e.BloodLiquidId)
                    .HasDatabaseName("FK_Races_Liquids_Blood_idx");

                entity.HasIndex(e => e.BloodModelId)
                    .HasDatabaseName("FK_Races_BloodModels_idx");

                entity.HasIndex(e => e.CorpseModelId)
                    .HasDatabaseName("FK_Races_CorpseModels_idx");

                entity.HasIndex(e => e.DefaultHealthStrategyId)
                    .HasDatabaseName("FK_Races_HealthStrategies_idx");

                entity.HasIndex(e => e.NaturalArmourMaterialId)
                    .HasDatabaseName("FK_Races_Materials_idx");

                entity.HasIndex(e => e.NaturalArmourTypeId)
                    .HasDatabaseName("FK_Races_ArmourTypes_idx");

                entity.HasIndex(e => e.ParentRaceId)
                    .HasDatabaseName("FK_Races_Races");

                entity.HasIndex(e => e.RaceButcheryProfileId)
                    .HasDatabaseName("FK_Races_RaceButcheryProfiles_idx");

                entity.HasIndex(e => e.SweatLiquidId)
                    .HasDatabaseName("FK_Races_Liqiuds_Sweat_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AdultAge)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'21'");

                entity.Property(e => e.AllowedGenders)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AttributeBonusProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.AttributeTotalCap).HasColumnType("int(11)");

                entity.Property(e => e.AvailabilityProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.BaseBodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.BiteWeight).HasDefaultValueSql("'1000'");

                entity.Property(e => e.BloodLiquidId).HasColumnType("bigint(20)");

                entity.Property(e => e.BloodModelId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartHealthMultiplier).HasDefaultValueSql("'1'");

                entity.Property(e => e.BodypartSizeModifier).HasColumnType("int(11)");

				entity.Property(e => e.HungerRate).HasColumnType("double").HasDefaultValue(1.0);

				entity.Property(e => e.ThirstRate).HasColumnType("double").HasDefaultValue(1.0);
				entity.Property(e => e.TrackIntensityVisual).HasColumnType("double").HasDefaultValue(1.0);
				entity.Property(e => e.TrackIntensityOlfactory).HasColumnType("double").HasDefaultValue(1.0);
				entity.Property(e => e.TrackingAbilityVisual).HasColumnType("double").HasDefaultValue(1.0);
				entity.Property(e => e.TrackingAbilityOlfactory).HasColumnType("double").HasDefaultValue(0.0);

				entity.Property(e => e.BreathingVolumeExpression)
                    .IsRequired()
                    .HasColumnType("varchar(500)")
                    .HasDefaultValueSql("'7'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CanAttack)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.CanClimb)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.CanDefend)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.CanEatCorpses)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.CanEatMaterialsOptIn)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.CanSwim)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.CanUseWeapons)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.ChildAge)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'3'");

                entity.Property(e => e.CommunicationStrategyType)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasDefaultValueSql("'humanoid'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CorpseModelId).HasColumnType("bigint(20)");

                entity.Property(e => e.DefaultHandedness)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'3'");

                entity.Property(e => e.DefaultHealthStrategyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DiceExpression)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.EatCorpseEmoteText)
                    .IsRequired()
                    .HasColumnType("varchar(500)")
                    .HasDefaultValueSql("'@ eat|eats {0}$1'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ElderAge)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'55'");

                entity.Property(e => e.HandednessOptions)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasDefaultValueSql("'1 3'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.HoldBreathLengthExpression)
                    .IsRequired()
                    .HasColumnType("varchar(500)")
                    .HasDefaultValueSql("'120'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.IlluminationPerceptionMultiplier).HasDefaultValueSql("'1'");

                entity.Property(e => e.IndividualAttributeCap).HasColumnType("int(11)");

                entity.Property(e => e.MaximumDragWeightExpression)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MaximumLiftWeightExpression)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MinimumSleepingPosition)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'4'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.NaturalArmourMaterialId).HasColumnType("bigint(20)");

                entity.Property(e => e.NaturalArmourQuality).HasColumnType("bigint(20)");

                entity.Property(e => e.NaturalArmourTypeId).HasColumnType("bigint(20)");

                entity.Property(e => e.NeedsToBreathe)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.BreathingModel)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci")
                    .HasDefaultValueSql("'simple'")
                    ;

                entity.Property(e => e.ParentRaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.RaceButcheryProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.RaceUsesStamina)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.SizeProne)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'5'");

                entity.Property(e => e.SizeSitting)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'6'");

                entity.Property(e => e.SizeStanding)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'6'");

                entity.Property(e => e.SweatLiquidId).HasColumnType("bigint(20)");

                entity.Property(e => e.SweatRateInLitresPerMinute).HasDefaultValueSql("'0.8'");

                entity.Property(e => e.TemperatureRangeCeiling).HasDefaultValueSql("'40'");

                entity.Property(e => e.VenerableAge)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'75'");

                entity.Property(e => e.YoungAdultAge)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'16'");

                entity.Property(e => e.YouthAge)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'10'");

                entity.Property(e => e.DefaultHeightWeightModelMaleId)
	                .HasColumnType("bigint(20)");
                entity.Property(e => e.DefaultHeightWeightModelFemaleId)
	                .HasColumnType("bigint(20)");
                entity.Property(e => e.DefaultHeightWeightModelNeuterId)
	                .HasColumnType("bigint(20)");
                entity.Property(e => e.DefaultHeightWeightModelNonBinaryId)
	                .HasColumnType("bigint(20)");

                entity.HasOne(d => d.DefaultHeightWeightModelMale)
	                .WithMany()
	                .HasForeignKey(d => d.DefaultHeightWeightModelMaleId)
	                .HasConstraintName("FK_Races_HeightWeightModelsMale");
                entity.HasOne(d => d.DefaultHeightWeightModelFemale)
	                .WithMany()
	                .HasForeignKey(d => d.DefaultHeightWeightModelFemaleId)
	                .HasConstraintName("FK_Races_HeightWeightModelsFemale");
                entity.HasOne(d => d.DefaultHeightWeightModelNeuter)
	                .WithMany()
	                .HasForeignKey(d => d.DefaultHeightWeightModelNeuterId)
	                .HasConstraintName("FK_Races_HeightWeightModelsNeuter");
                entity.HasOne(d => d.DefaultHeightWeightModelNonBinary)
	                .WithMany()
	                .HasForeignKey(d => d.DefaultHeightWeightModelNonBinaryId)
	                .HasConstraintName("FK_Races_HeightWeightModelsNonBinary");


                entity.HasOne(d => d.AttributeBonusProg)
                    .WithMany(p => p.RacesAttributeBonusProg)
                    .HasForeignKey(d => d.AttributeBonusProgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Races_AttributeBonusProg");

                entity.HasOne(d => d.AvailabilityProg)
                    .WithMany(p => p.RacesAvailabilityProg)
                    .HasForeignKey(d => d.AvailabilityProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Races_AvailabilityProg");

                entity.HasOne(d => d.BaseBody)
                    .WithMany(p => p.Races)
                    .HasForeignKey(d => d.BaseBodyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Races_BodyProtos");

                entity.HasOne(d => d.BloodLiquid)
                    .WithMany(p => p.RacesBloodLiquid)
                    .HasForeignKey(d => d.BloodLiquidId)
                    .HasConstraintName("FK_Races_Liquids_Blood");

                entity.HasOne(d => d.BloodModel)
                    .WithMany(p => p.Races)
                    .HasForeignKey(d => d.BloodModelId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Races_BloodModels");

                entity.HasOne(d => d.CorpseModel)
                    .WithMany(p => p.Races)
                    .HasForeignKey(d => d.CorpseModelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Races_CorpseModels");

                entity.HasOne(d => d.DefaultHealthStrategy)
                    .WithMany(p => p.Races)
                    .HasForeignKey(d => d.DefaultHealthStrategyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Races_HealthStrategies");

                entity.HasOne(d => d.NaturalArmourMaterial)
                    .WithMany(p => p.Races)
                    .HasForeignKey(d => d.NaturalArmourMaterialId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Races_Materials");

                entity.HasOne(d => d.NaturalArmourType)
                    .WithMany(p => p.Races)
                    .HasForeignKey(d => d.NaturalArmourTypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Races_ArmourTypes");

                entity.HasOne(d => d.ParentRace)
                    .WithMany(p => p.InverseParentRace)
                    .HasForeignKey(d => d.ParentRaceId)
                    .HasConstraintName("FK_Races_Races");

                entity.HasOne(d => d.RaceButcheryProfile)
                    .WithMany(p => p.Races)
                    .HasForeignKey(d => d.RaceButcheryProfileId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Races_RaceButcheryProfiles");

                entity.HasOne(d => d.SweatLiquid)
                    .WithMany(p => p.RacesSweatLiquid)
                    .HasForeignKey(d => d.SweatLiquidId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Races_Liqiuds_Sweat");
            });

            modelBuilder.Entity<RacesAdditionalBodyparts>(entity =>
            {
                entity.HasKey(e => new { e.Usage, e.RaceId, e.BodypartId })
                    .HasName("PRIMARY");

                entity.ToTable("Races_AdditionalBodyparts");

                entity.HasIndex(e => e.BodypartId)
                    .HasDatabaseName("FK_Races_AdditionalBodyparts_BodypartProto");

                entity.HasIndex(e => e.RaceId)
                    .HasDatabaseName("FK_Races_AdditionalBodyparts_Races");

                entity.Property(e => e.Usage)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Bodypart)
                    .WithMany(p => p.RacesAdditionalBodyparts)
                    .HasForeignKey(d => d.BodypartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Races_AdditionalBodyparts_BodypartProto");

                entity.HasOne(d => d.Race)
                    .WithMany(p => p.RacesAdditionalBodyparts)
                    .HasForeignKey(d => d.RaceId)
                    .HasConstraintName("FK_Races_AdditionalBodyparts_Races");
            });

            modelBuilder.Entity<RacesAdditionalCharacteristics>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.CharacteristicDefinitionId })
                    .HasName("PRIMARY");

                entity.ToTable("Races_AdditionalCharacteristics");

                entity.HasIndex(e => e.CharacteristicDefinitionId)
                    .HasDatabaseName("FK_RAC_CharacteristicDefinitions");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.CharacteristicDefinitionId).HasColumnType("bigint(20)");

                entity.Property(e => e.Usage)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.CharacteristicDefinition)
                    .WithMany(p => p.RacesAdditionalCharacteristics)
                    .HasForeignKey(d => d.CharacteristicDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RAC_CharacteristicDefinitions");

                entity.HasOne(d => d.Race)
                    .WithMany(p => p.RacesAdditionalCharacteristics)
                    .HasForeignKey(d => d.RaceId)
                    .HasConstraintName("FK_RAC_Races");
            });

            modelBuilder.Entity<RacesAttributes>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.AttributeId })
                    .HasName("PRIMARY");

                entity.ToTable("Races_Attributes");

                entity.HasIndex(e => e.AttributeId)
                    .HasDatabaseName("FK_Races_Attributes_TraitDefinitions");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.AttributeId).HasColumnType("bigint(20)");

                entity.Property(e => e.IsHealthAttribute)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.RacesAttributes)
                    .HasForeignKey(d => d.AttributeId)
                    .HasConstraintName("FK_Races_Attributes_TraitDefinitions");

                entity.HasOne(d => d.Race)
                    .WithMany(p => p.RacesAttributes)
                    .HasForeignKey(d => d.RaceId)
                    .HasConstraintName("FK_Races_Attributes_Races");
            });

            modelBuilder.Entity<RacesBreathableGases>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.GasId })
                    .HasName("PRIMARY");

                entity.ToTable("Races_BreathableGases");

                entity.HasIndex(e => e.GasId)
                    .HasDatabaseName("FK_Races-BreathableGases_Gases_idx");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.GasId).HasColumnType("bigint(20)");

                entity.Property(e => e.Multiplier).HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Gas)
                    .WithMany(p => p.RacesBreathableGases)
                    .HasForeignKey(d => d.GasId)
                    .HasConstraintName("FK_Races_BreathableGases_Gases");

                entity.HasOne(d => d.Race)
                    .WithMany(p => p.RacesBreathableGases)
                    .HasForeignKey(d => d.RaceId)
                    .HasConstraintName("FK_Races_BreathableGases_Races");
            });

            modelBuilder.Entity<RacesBreathableLiquids>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.LiquidId })
                    .HasName("PRIMARY");

                entity.ToTable("Races_BreathableLiquids");

                entity.HasIndex(e => e.LiquidId)
                    .HasDatabaseName("FK_Races_BreathableLiquids_Liquids_idx");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.LiquidId).HasColumnType("bigint(20)");

                entity.Property(e => e.Multiplier).HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Liquid)
                    .WithMany(p => p.RacesBreathableLiquids)
                    .HasForeignKey(d => d.LiquidId)
                    .HasConstraintName("FK_Races_BreathableLiquids_Liquids");

                entity.HasOne(d => d.Race)
                    .WithMany(p => p.RacesBreathableLiquids)
                    .HasForeignKey(d => d.RaceId)
                    .HasConstraintName("FK_Races_BreathableLiquids_Races");
            });

            modelBuilder.Entity<RacesChargenResources>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.ChargenResourceId })
                    .HasName("PRIMARY");

                entity.ToTable("Races_ChargenResources");

                entity.HasIndex(e => e.ChargenResourceId)
                    .HasDatabaseName("FK_Races_ChargenResources_ChargenResources");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenResourceId).HasColumnType("bigint(20)");

                entity.Property(e => e.RequirementOnly)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Amount).HasColumnType("int(11)");

                //entity.HasOne(d => d.ChargenResource)
                //    .WithMany(p => p.RacesChargenResources)
                //    .HasForeignKey(d => d.ChargenResourceId)
                //    .HasConstraintName("FK_Races_ChargenResources_ChargenResources");

                //entity.HasOne(d => d.Race)
                //    .WithMany(p => p.RacesChargenResources)
                //    .HasForeignKey(d => d.RaceId)
                //    .HasConstraintName("FK_Races_ChargenResources_Races");
            });

            modelBuilder.Entity<RacesEdibleMaterials>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.MaterialId })
                    .HasName("PRIMARY");

                entity.ToTable("Races_EdibleMaterials");

                entity.HasIndex(e => e.MaterialId)
                    .HasDatabaseName("FK_Races_EdibleMaterials_Materials_idx");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.MaterialId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.RacesEdibleMaterials)
                    .HasForeignKey(d => d.MaterialId)
                    .HasConstraintName("FK_Races_EdibleMaterials_Materials");

                entity.HasOne(d => d.Race)
                    .WithMany(p => p.RacesEdibleMaterials)
                    .HasForeignKey(d => d.RaceId)
                    .HasConstraintName("FK_Races_EdibleMaterials_Races");
            });

            modelBuilder.Entity<RacesCombatActions>(entity =>
            {
	            entity.HasKey(e => new { e.RaceId, e.CombatActionId })
	                  .HasName("PRIMARY");

	            entity.ToTable("Races_CombatActions");
	            entity.Property(e => e.RaceId).HasColumnType("bigint(20)");
	            entity.Property(e => e.CombatActionId).HasColumnType("bigint(20)");
	            entity.HasOne(d => d.Race)
	                  .WithMany(p => p.RacesCombatActions)
	                  .HasForeignKey(d => d.RaceId)
	                  .HasConstraintName("FK_Races_CombatActions_Races")
	                  .OnDelete(DeleteBehavior.Cascade);
	            entity.HasOne(d => d.CombatAction)
	                  .WithMany(p => p.RacesCombatActions)
	                  .HasForeignKey(d => d.CombatActionId)
	                  .HasConstraintName("FK_Races_CombatActions_CombatActions")
	                  .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RacesWeaponAttacks>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.WeaponAttackId, e.BodypartId })
                    .HasName("PRIMARY");

                entity.ToTable("Races_WeaponAttacks");

                entity.HasIndex(e => e.BodypartId)
                    .HasDatabaseName("FK_Races_WeaponAttacks_BodypartProto_idx");

                entity.HasIndex(e => e.WeaponAttackId)
                    .HasDatabaseName("FK_Races_WeaponAttacks_WeaponAttacks_idx");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.WeaponAttackId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartId).HasColumnType("bigint(20)");

                entity.Property(e => e.Quality).HasColumnType("int(11)");

                entity.HasOne(d => d.Bodypart)
                    .WithMany(p => p.RacesWeaponAttacks)
                    .HasForeignKey(d => d.BodypartId)
                    .HasConstraintName("FK_Races_WeaponAttacks_BodypartProto");

                entity.HasOne(d => d.Race)
                    .WithMany(p => p.RacesWeaponAttacks)
                    .HasForeignKey(d => d.RaceId)
                    .HasConstraintName("FK_Races_WeaponAttacks_Races");

                entity.HasOne(d => d.WeaponAttack)
                    .WithMany(p => p.RacesWeaponAttacks)
                    .HasForeignKey(d => d.WeaponAttackId)
                    .HasConstraintName("FK_Races_WeaponAttacks_WeaponAttacks");
            });

            modelBuilder.Entity<RandomNameProfile>(entity =>
            {
                entity.HasIndex(e => e.NameCultureId)
                    .HasDatabaseName("FK_RandomNameProfiles_NameCulture");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Gender).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.NameCultureId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.NameCulture)
                    .WithMany(p => p.RandomNameProfiles)
                    .HasForeignKey(d => d.NameCultureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RandomNameProfiles_NameCulture");
            });

            modelBuilder.Entity<RandomNameProfilesDiceExpressions>(entity =>
            {
                entity.HasKey(e => new { e.RandomNameProfileId, e.NameUsage })
                    .HasName("PRIMARY");

                entity.ToTable("RandomNameProfiles_DiceExpressions");

                entity.Property(e => e.RandomNameProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.NameUsage).HasColumnType("int(11)");

                entity.Property(e => e.DiceExpression)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.RandomNameProfile)
                    .WithMany(p => p.RandomNameProfilesDiceExpressions)
                    .HasForeignKey(d => d.RandomNameProfileId)
                    .HasConstraintName("FK_RandomNameProfiles_DiceExpressions_RandomNameProfiles");
            });

            modelBuilder.Entity<RandomNameProfilesElements>(entity =>
            {
                entity.HasKey(e => new { e.RandomNameProfileId, e.NameUsage, e.Name })
                    .HasName("PRIMARY");

                entity.ToTable("RandomNameProfiles_Elements");

                entity.Property(e => e.RandomNameProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.NameUsage).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Weighting).HasColumnType("int(11)");

                entity.HasOne(d => d.RandomNameProfile)
                    .WithMany(p => p.RandomNameProfilesElements)
                    .HasForeignKey(d => d.RandomNameProfileId)
                    .HasConstraintName("FK_RandomNameProfiles_Elements_RandomNameProfiles");
            });

            modelBuilder.Entity<RangedCover>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ActionDescriptionString)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.CoverExtent).HasColumnType("int(11)");

                entity.Property(e => e.CoverStaysWhileMoving).HasColumnType("bit(1)");

                entity.Property(e => e.CoverType).HasColumnType("int(11)");

                entity.Property(e => e.DescriptionString)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.HighestPositionState).HasColumnType("int(11)");

                entity.Property(e => e.MaximumSimultaneousCovers).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");
            });

            modelBuilder.Entity<RangedWeaponTypes>(entity =>
            {
                entity.HasIndex(e => e.FireTraitId)
                    .HasDatabaseName("FK_RangedWeaponTypes_TraitDefinitions_Fire_idx");

                entity.HasIndex(e => e.OperateTraitId)
                    .HasDatabaseName("FK_RangedWeaponTypes_TraitDefinitions_Operate_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AccuracyBonusExpression)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.AimBonusLostPerShot).HasDefaultValueSql("'1'");

                entity.Property(e => e.AlwaysRequiresTwoHandsToWield)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.AmmunitionCapacity).HasColumnType("int(11)");

                entity.Property(e => e.AmmunitionLoadType).HasColumnType("int(11)");

                entity.Property(e => e.BaseAimDifficulty).HasColumnType("int(11)");

                entity.Property(e => e.Classification).HasColumnType("int(11)");

                entity.Property(e => e.DamageBonusExpression)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.DefaultRangeInRooms).HasColumnType("int(11)");

                entity.Property(e => e.FireDelay).HasDefaultValueSql("'0.5'");

                entity.Property(e => e.FireTraitId).HasColumnType("bigint(20)");

                entity.Property(e => e.FireableInMelee).HasColumnType("bit(1)");

                entity.Property(e => e.LoadDelay).HasDefaultValueSql("'0.5'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.OperateTraitId).HasColumnType("bigint(20)");

                entity.Property(e => e.RangedWeaponType).HasColumnType("int(11)");

                entity.Property(e => e.ReadyDelay).HasDefaultValueSql("'0.1'");

                entity.Property(e => e.RequiresFreeHandToReady)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.SpecificAmmunitionGrade)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasOne(d => d.FireTrait)
                    .WithMany(p => p.RangedWeaponTypesFireTrait)
                    .HasForeignKey(d => d.FireTraitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RangedWeaponTypes_TraitDefinitions_Fire");

                entity.HasOne(d => d.OperateTrait)
                    .WithMany(p => p.RangedWeaponTypesOperateTrait)
                    .HasForeignKey(d => d.OperateTraitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RangedWeaponTypes_TraitDefinitions_Operate");
            });

            modelBuilder.Entity<Rank>(entity =>
            {
                entity.HasIndex(e => e.ClanId)
                    .HasDatabaseName("FK_Ranks_Clans");

                entity.HasIndex(e => new { e.InsigniaGameItemId, e.InsigniaGameItemRevnum })
                    .HasDatabaseName("FK_Ranks_GameItemProtos");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ClanId).HasColumnType("bigint(20)");

                entity.Property(e => e.InsigniaGameItemId).HasColumnType("bigint(20)");

                entity.Property(e => e.InsigniaGameItemRevnum).HasColumnType("int(11)");

                entity.Property(e => e.FameType).HasColumnType("int(11)").HasDefaultValueSql("0");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Privileges).HasColumnType("bigint(20)");

                entity.Property(e => e.RankNumber).HasColumnType("int(11)");

                entity.Property(e => e.RankPath)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Clan)
                    .WithMany(p => p.Ranks)
                    .HasForeignKey(d => d.ClanId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ranks_Clans");

                entity.HasOne(d => d.InsigniaGameItem)
                    .WithMany(p => p.Ranks)
                    .HasForeignKey(d => new { d.InsigniaGameItemId, d.InsigniaGameItemRevnum })
                    .HasConstraintName("FK_Ranks_GameItemProtos");
            });

            modelBuilder.Entity<RanksAbbreviations>(entity =>
            {
                entity.HasKey(e => new { e.RankId, e.Abbreviation })
                    .HasName("PRIMARY");

                entity.ToTable("Ranks_Abbreviations");

                entity.HasIndex(e => e.FutureProgId)
                    .HasDatabaseName("FK_Ranks_Abbreviations_FutureProgs");

                entity.Property(e => e.RankId).HasColumnType("bigint(20)");

                entity.Property(e => e.Abbreviation)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Order).HasColumnType("int(11)");

                entity.HasOne(d => d.FutureProg)
                    .WithMany(p => p.RanksAbbreviations)
                    .HasForeignKey(d => d.FutureProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Ranks_Abbreviations_FutureProgs");

                entity.HasOne(d => d.Rank)
                    .WithMany(p => p.RanksAbbreviations)
                    .HasForeignKey(d => d.RankId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ranks_Abbreviations_Ranks");
            });

            modelBuilder.Entity<RanksPaygrade>(entity =>
            {
                entity.HasKey(e => new { e.RankId, e.PaygradeId })
                    .HasName("PRIMARY");

                entity.ToTable("Ranks_Paygrades");

                entity.HasIndex(e => e.PaygradeId)
                    .HasDatabaseName("FK_Ranks_Paygrades_Paygrades");

                entity.Property(e => e.RankId).HasColumnType("bigint(20)");

                entity.Property(e => e.PaygradeId).HasColumnType("bigint(20)");

                entity.Property(e => e.Order).HasColumnType("int(11)");

                entity.HasOne(d => d.Paygrade)
                    .WithMany(p => p.RanksPaygrades)
                    .HasForeignKey(d => d.PaygradeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ranks_Paygrades_Paygrades");

                entity.HasOne(d => d.Rank)
                    .WithMany(p => p.RanksPaygrades)
                    .HasForeignKey(d => d.RankId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ranks_Paygrades_Ranks");
            });

            modelBuilder.Entity<RanksTitle>(entity =>
            {
                entity.HasKey(e => new { e.RankId, e.Title })
                    .HasName("PRIMARY");

                entity.ToTable("Ranks_Titles");

                entity.HasIndex(e => e.FutureProgId)
                    .HasDatabaseName("FK_Ranks_Titles_FutureProgs");

                entity.Property(e => e.RankId).HasColumnType("bigint(20)");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Order).HasColumnType("int(11)");

                entity.HasOne(d => d.FutureProg)
                    .WithMany(p => p.RanksTitles)
                    .HasForeignKey(d => d.FutureProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Ranks_Titles_FutureProgs");

                entity.HasOne(d => d.Rank)
                    .WithMany(p => p.RanksTitles)
                    .HasForeignKey(d => d.RankId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ranks_Titles_Ranks");
            });
        }

    }
}
