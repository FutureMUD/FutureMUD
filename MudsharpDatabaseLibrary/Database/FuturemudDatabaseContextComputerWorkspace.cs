using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureCharacterComputerWorkspace(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<CharacterComputerExecutable>(entity =>
		{
			entity.ToTable("CharacterComputerExecutables");

			entity.HasKey(e => e.Id)
			      .HasName("PRIMARY");

			entity.HasIndex(e => e.OwnerCharacterId)
			      .HasDatabaseName("FK_CharacterComputerExecutables_Characters_idx");

			entity.HasIndex(e => new { e.OwnerCharacterId, e.Name })
			      .HasDatabaseName("IX_CharacterComputerExecutables_Owner_Name");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.OwnerCharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.OwnerHostItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.OwnerStorageItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.ExecutableKind).HasColumnType("int(11)");
			entity.Property(e => e.CompilationContext).HasColumnType("int(11)");
			entity.Property(e => e.CompilationStatus).HasColumnType("int(11)");
			entity.Property(e => e.AutorunOnBoot).HasColumnType("bit(1)");
			entity.Property(e => e.CreatedAtUtc).HasColumnType("datetime");
			entity.Property(e => e.LastModifiedAtUtc).HasColumnType("datetime");
			entity.Property(e => e.Name)
			      .IsRequired()
			      .HasColumnType("varchar(255)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.ReturnTypeDefinition)
			      .IsRequired()
			      .HasColumnType("varchar(255)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.SourceCode)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.CompileError)
			      .IsRequired()
			      .HasColumnType("text")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.HasOne(d => d.OwnerCharacter)
			      .WithMany(p => p.CharacterComputerExecutables)
			      .HasForeignKey(d => d.OwnerCharacterId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_CharacterComputerExecutables_Characters");
		});

		modelBuilder.Entity<CharacterComputerExecutableParameter>(entity =>
		{
			entity.ToTable("CharacterComputerExecutableParameters");

			entity.HasKey(e => e.Id)
			      .HasName("PRIMARY");

			entity.HasIndex(e => e.CharacterComputerExecutableId)
			      .HasDatabaseName("FK_CharacterComputerExecutableParameters_Executables_idx");

			entity.HasIndex(e => new { e.CharacterComputerExecutableId, e.ParameterIndex })
			      .IsUnique()
			      .HasDatabaseName("IX_CharacterComputerExecutableParameters_Executable_Index");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterComputerExecutableId).HasColumnType("bigint(20)");
			entity.Property(e => e.ParameterIndex).HasColumnType("int(11)");
			entity.Property(e => e.ParameterName)
			      .IsRequired()
			      .HasColumnType("varchar(255)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.ParameterTypeDefinition)
			      .IsRequired()
			      .HasColumnType("varchar(255)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.HasOne(d => d.CharacterComputerExecutable)
			      .WithMany(p => p.Parameters)
			      .HasForeignKey(d => d.CharacterComputerExecutableId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_CharacterComputerExecutableParameters_CharacterComputerExecutables");
		});

		modelBuilder.Entity<CharacterComputerProgramProcess>(entity =>
		{
			entity.ToTable("CharacterComputerProgramProcesses");

			entity.HasKey(e => e.Id)
			      .HasName("PRIMARY");

			entity.HasIndex(e => e.CharacterComputerExecutableId)
			      .HasDatabaseName("FK_CharacterComputerProgramProcesses_Executables_idx");

			entity.HasIndex(e => e.OwnerCharacterId)
			      .HasDatabaseName("FK_CharacterComputerProgramProcesses_Characters_idx");

			entity.HasIndex(e => new { e.OwnerCharacterId, e.Status, e.WakeTimeUtc })
			      .HasDatabaseName("IX_CharacterComputerProgramProcesses_Owner_Status_Wake");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterComputerExecutableId).HasColumnType("bigint(20)");
			entity.Property(e => e.OwnerCharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.WaitType).HasColumnType("int(11)");
			entity.Property(e => e.PowerLossBehaviour).HasColumnType("int(11)");
			entity.Property(e => e.WakeTimeUtc).HasColumnType("datetime");
			entity.Property(e => e.StartedAtUtc).HasColumnType("datetime");
			entity.Property(e => e.LastUpdatedAtUtc).HasColumnType("datetime");
			entity.Property(e => e.EndedAtUtc).HasColumnType("datetime");
			entity.Property(e => e.ProcessName)
			      .IsRequired()
			      .HasColumnType("varchar(255)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.WaitArgument)
			      .HasColumnType("varchar(1000)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.StateJson)
			      .IsRequired()
			      .HasColumnType("longtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.ResultJson)
			      .HasColumnType("longtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.LastError)
			      .HasColumnType("text")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.HasOne(d => d.CharacterComputerExecutable)
			      .WithMany(p => p.Processes)
			      .HasForeignKey(d => d.CharacterComputerExecutableId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_CharacterComputerProgramProcesses_CharacterComputerExecutables");

			entity.HasOne(d => d.OwnerCharacter)
			      .WithMany(p => p.CharacterComputerProgramProcesses)
			      .HasForeignKey(d => d.OwnerCharacterId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_CharacterComputerProgramProcesses_Characters");
		});
	}
}
