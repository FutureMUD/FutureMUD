﻿using Microsoft.EntityFrameworkCore;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Database
{
	public partial class FuturemudDatabaseContext
	{
		protected static void OnModelCreatingFive(ModelBuilder modelBuilder) {
			modelBuilder.Entity<ScriptedEvent>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.Name)
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.CharacterId).HasColumnType("bigint(20)").IsRequired(false);
				entity.Property(e => e.CharacterFilterProgId).HasColumnType("bigint(20)").IsRequired(false);
				entity.Property(e => e.IsReady).HasColumnType("bit(1)");
				entity.Property(e => e.IsFinished).HasColumnType("bit(1)");
				entity.Property(e => e.IsTemplate).HasColumnType("bit(1)");
				entity.Property(e => e.EarliestDate).HasColumnType("datetime");

				entity.HasOne(d => d.Character)
					.WithMany(p => p.ScriptedEvents)
					.HasForeignKey(d => d.CharacterId)
					.HasConstraintName("FK_ScriptedEvents_Characters");

				entity.HasOne(d => d.CharacterFilterProg)
					.WithMany()
					.HasForeignKey(d => d.CharacterFilterProgId)
					.HasConstraintName("FK_ScriptedEvents_FutureProgs");
			});

			modelBuilder.Entity<ScriptedEventFreeTextQuestion>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.ScriptedEventId).HasColumnType("bigint(20)");
				entity.Property(e => e.Question)
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.Answer)
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.ScriptedEvent)
					.WithMany(p => p.FreeTextQuestions)
					.HasForeignKey(d => d.ScriptedEventId)
					.HasConstraintName("FK_ScriptedEventFreeTextQuestions_ScriptedEvents");
			});

			modelBuilder.Entity<ScriptedEventMultipleChoiceQuestion>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.ScriptedEventId).HasColumnType("bigint(20)");
				entity.Property(e => e.ChosenAnswerId).HasColumnType("bigint(20)").IsRequired(false);
				entity.Property(e => e.Question)
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.ScriptedEvent)
					.WithMany(p => p.MultipleChoiceQuestions)
					.HasForeignKey(d => d.ScriptedEventId)
					.HasConstraintName("FK_ScriptedEventMultipleChoiceQuestions_ScriptedEvents");

				entity.HasOne(d => d.ChosenAnswer)
					.WithMany()
					.HasForeignKey(d => d.ChosenAnswerId)
					.HasConstraintName("FK_ScriptedEventMultipleChoiceQuestions_ScriptedEventMultipleChoiceQuestionAnswers");
			});

			modelBuilder.Entity<ScriptedEventMultipleChoiceQuestionAnswer>(entity => {
				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.ScriptedEventMultipleChoiceQuestionId).HasColumnType("bigint(20)");
				entity.Property(e => e.AnswerFilterProgId).HasColumnType("bigint(20)").IsRequired(false);
				entity.Property(e => e.AfterChoiceProgId).HasColumnType("bigint(20)").IsRequired(false);
				entity.Property(e => e.DescriptionBeforeChoice)
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.DescriptionAfterChoice)
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.ScriptedEventMultipleChoiceQuestion)
					.WithMany(p => p.Answers)
					.HasForeignKey(d => d.ScriptedEventMultipleChoiceQuestionId)
					.HasConstraintName("FK_ScriptedEventMultipleChoiceQuestionAnswers_ScriptedEventMultipleChoiceQuestions");

				entity.HasOne(d => d.AfterChoiceProg)
					.WithMany()
					.HasForeignKey(d => d.AfterChoiceProgId)
					.HasConstraintName("FK_ScriptedEventMultipleChoiceQuestionAnswers_FutureProgs_After");

				entity.HasOne(d => d.AnswerFilterProg)
					.WithMany()
					.HasForeignKey(d => d.AnswerFilterProgId)
					.HasConstraintName("FK_ScriptedEventMultipleChoiceQuestionAnswers_FutureProgs_Filter");
			});
		}
	}
}