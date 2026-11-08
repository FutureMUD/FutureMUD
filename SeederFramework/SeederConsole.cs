#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DatabaseSeeder;

internal sealed class SeederConsole(Assembly productAssembly, string connectionString
#if DEBUG
	, IReadOnlyList<SeederReplayProfile> replayProfiles, ProductReplayRunner replayRunner, string replayWarning
#endif
	, Assembly? foundationAssembly = null
)
{
    internal static FuturemudDatabaseContext CreateContext(string connectionString, bool useLazyLoading = false)
    {
        DbContextOptionsBuilder<FuturemudDatabaseContext> builder = new();
        if (useLazyLoading)
        {
            builder.UseLazyLoadingProxies();
        }

        builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        return new FuturemudDatabaseContext(builder.Options);
    }

    private static bool IsInteractiveConsole => !(Console.IsInputRedirected || Console.IsOutputRedirected);

    internal static void SafeClear()
    {
        if (!IsInteractiveConsole)
        {
            return;
        }

        try
        {
            Console.ResetColor();
            Console.Clear();
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WT_SESSION")) ||
                Environment.GetEnvironmentVariable("TERM") is { Length: > 0 } term && term != "dumb")
            {
                Console.Write("\u001b[3J\u001b[H");
            }
        }
        catch
        {
            // Some redirected or hosted consoles do not support clearing.
        }
    }

    private static void WaitForReturnToMenu()
    {
        if (IsInteractiveConsole)
        {
            Console.ReadKey(intercept: true);
            return;
        }

        Console.ReadLine();
    }

    internal void ShowMainMenu()
    {
        string errorMessage = string.Empty;
        bool showBlockedSeeders = false;
        while (true)
        {
            SafeClear();
            Console.WriteLine("Loading seeders...");
            List<IDatabaseSeeder> seeders = SeederPackageDiscovery.GetEnabledSeeders(productAssembly)
				.Concat(foundationAssembly is null ? [] : SeederPackageDiscovery.GetEnabledSeeders(foundationAssembly)).ToList();
            SeederDependencyPlan dependencyPlan = SeederPackagePlan.GetDependencyPlan(seeders);
            if (dependencyPlan.Errors.Any())
            {
                throw new InvalidOperationException(
                    $"The database seeder dependency registry is invalid: {string.Join("; ", dependencyPlan.Errors)}");
            }

            Dictionary<Type, int> dependencyOrder = dependencyPlan.OrderedSeeders
                .Select((seeder, index) => new { SeederType = seeder.GetType(), Index = index })
                .ToDictionary(x => x.SeederType, x => x.Index);

            int i = 1;
            List<(IDatabaseSeeder Seeder, SeederAssessment Assessment)> assessedSeeders;
            List<(IDatabaseSeeder Seeder, SeederAssessment Assessment)> visibleSeeders;
            int blockedSeederCount;
            using (FuturemudDatabaseContext context = CreateContext(connectionString, useLazyLoading: true))
            {
                assessedSeeders = seeders
                    .Select(seeder => (Seeder: seeder, Assessment: seeder.AssessSeedData(context)))
                    .OrderBy(x => GetMenuSortRank(x.Assessment.Status))
                    .ThenBy(x => dependencyOrder[x.Seeder.GetType()])
                    .ThenBy(x => x.Seeder.Name)
                    .ToList();
                visibleSeeders = assessedSeeders
                    .Where(x => ShowSeederInMainMenu(x.Assessment.Status, showBlockedSeeders))
                    .ToList();
                blockedSeederCount = assessedSeeders.Count(x => x.Assessment.Status == SeederAssessmentStatus.Blocked);

                SafeClear();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    ConsoleLayoutHelper.WriteWrapped(errorMessage);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                string prompt = blockedSeederCount switch
                {
                    <= 0 => "Please enter the number of the package you wish to import, or QUIT to exit: ",
                    _ when showBlockedSeeders => "Please enter the number of the package you wish to import, VALID to hide blocked packages, or QUIT to exit: ",
                    _ => "Please enter the number of the package you wish to import, INVALID to show blocked packages, or QUIT to exit: "
                };
                ConsoleLayoutHelper.WriteWrapped(prompt);
                Console.WriteLine();
                foreach ((IDatabaseSeeder Seeder, SeederAssessment Assessment) assessedSeeder in visibleSeeders)
                {
                    Console.ForegroundColor = GetAssessmentColour(assessedSeeder.Assessment.Status);
                    foreach (string line in ConsoleLayoutHelper.FormatMenuEntry(
                                 i++,
                                 assessedSeeder.Seeder.Name,
                                 GetAssessmentLabel(assessedSeeder.Assessment.Status),
                                 assessedSeeder.Seeder.Tagline))
                    {
                        Console.WriteLine(line);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
#if DEBUG
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine();
                Console.WriteLine("REPLAY - run a complete standard Debug replay profile");
                Console.ForegroundColor = ConsoleColor.White;
#endif
            }

            string blockedSeederSummary = GetBlockedSeederSummary(blockedSeederCount, showBlockedSeeders);
            if (!string.IsNullOrWhiteSpace(blockedSeederSummary))
            {
                Console.WriteLine();
                Console.ForegroundColor = showBlockedSeeders ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;
                ConsoleLayoutHelper.WriteWrapped(blockedSeederSummary);
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine();
            Console.Write("Your choice: ");
            string? choice = Console.ReadLine();
            if (choice is null)
            {
                return;
            }

            if (choice.EqualToAny("quit", "q", "exit", "stop"))
            {
                return;
            }

#if DEBUG
			if (choice.EqualToAny("replay", "profile", "replayprofile"))
			{
				ShowReplayProfiles(seeders);
				errorMessage = string.Empty;
				continue;
			}
#endif

            if (choice.EqualToAny("invalid", "blocked"))
            {
                if (blockedSeederCount == 0)
                {
                    errorMessage = "There are no blocked packages to show.";
                    continue;
                }

                showBlockedSeeders = true;
                errorMessage = string.Empty;
                continue;
            }

            if (choice.EqualToAny("valid", "hideinvalid", "hide invalid"))
            {
                showBlockedSeeders = false;
                errorMessage = string.Empty;
                continue;
            }

            IDatabaseSeeder? pick = uint.TryParse(choice, out uint value)
                ? visibleSeeders.ElementAtOrDefault((int)value - 1).Seeder
                : visibleSeeders.Select(x => x.Seeder).FirstOrDefault(x => x.Name.EqualTo(choice)) ??
                  visibleSeeders.Select(x => x.Seeder)
                      .FirstOrDefault(x => x.Name.StartsWith(choice, StringComparison.OrdinalIgnoreCase));

            if (pick == null)
            {
                if (!showBlockedSeeders && assessedSeeders.Any(x =>
                        x.Assessment.Status == SeederAssessmentStatus.Blocked &&
                        (x.Seeder.Name.EqualTo(choice) ||
                         x.Seeder.Name.StartsWith(choice, StringComparison.OrdinalIgnoreCase))))
                {
                    errorMessage = "That package is currently blocked by missing prerequisites. Type INVALID to show blocked packages.";
                    continue;
                }

                errorMessage = "That is not a valid selection.";
                continue;
            }

            ShowSeeder(pick);
            errorMessage = string.Empty;
        }
    }

#if DEBUG
	private void ShowReplayProfiles(IReadOnlyList<IDatabaseSeeder> seeders)
	{
		SafeClear();
		Console.ForegroundColor = ConsoleColor.DarkYellow;
		ConsoleLayoutHelper.WriteWrapped(
			replayWarning);
		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine();
		Console.WriteLine($"Target database: {DebugSeederConnection.GetDatabaseName(connectionString)}");
		Console.WriteLine();

		for (var i = 0; i < replayProfiles.Count; i++)
		{
			SeederReplayProfile profile = replayProfiles[i];
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"{i + 1}. {profile.Name} ({profile.Id})");
			Console.ForegroundColor = ConsoleColor.White;
			ConsoleLayoutHelper.WriteWrapped(profile.Description, indent: "   ");
		}

		Console.WriteLine();
		Console.Write("Choose a profile by number or ID, or type BACK: ");
		string? choice = Console.ReadLine();
		if (choice is null || choice.EqualToAny("back", "b", "quit", "q", "exit"))
		{
			return;
		}

		SeederReplayProfile? selectedProfile = uint.TryParse(choice, out uint profileNumber)
			? replayProfiles.ElementAtOrDefault((int)profileNumber - 1)
			: replayProfiles.FirstOrDefault(x => x.Id.EqualTo(choice));
		if (selectedProfile is null)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			ConsoleLayoutHelper.WriteWrapped("That is not a valid replay profile.");
			Console.ForegroundColor = ConsoleColor.White;
			WaitForReturnToMenu();
			return;
		}

		Console.WriteLine();
		Console.ForegroundColor = ConsoleColor.DarkYellow;
		ConsoleLayoutHelper.WriteWrapped(
			$"Type REPLAY to run {selectedProfile.Name} against {DebugSeederConnection.GetDatabaseName(connectionString)}. The run stops on the first failure and does not reset the database.");
		Console.ForegroundColor = ConsoleColor.White;
		Console.Write("> ");
		string? confirmation = Console.ReadLine();
		if (!string.Equals(confirmation, "replay", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		SafeClear();
		Console.WriteLine($"Starting {selectedProfile.Name} replay...");
		SeederReplayRunResult result = replayRunner.Run(
			selectedProfile,
			seeders,
			() => CreateContext(connectionString, useLazyLoading: true),
			productAssembly.GetName().Version ?? new Version(1, 0, 0),
			message => ConsoleLayoutHelper.WriteWrapped(message));
		Console.WriteLine();
		if (!result.Validation.IsValid)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			ConsoleLayoutHelper.WriteWrapped("The replay profile needs maintenance:");
			foreach (string error in result.Validation.Errors)
			{
				ConsoleLayoutHelper.WriteWrapped($" - {error}", indent: "   ");
			}
		}
		else if (result.Success)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			ConsoleLayoutHelper.WriteWrapped(
				$"Replay completed successfully. Ran {result.CompletedSeeders.Count:N0} seeders: {string.Join(", ", result.CompletedSeeders)}.");
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Red;
			ConsoleLayoutHelper.WriteWrapped(result.Failure ?? "Replay failed.");
			if (result.Exception is not null)
			{
				ConsoleLayoutHelper.WriteWrapped(result.Exception.ToString());
			}
		}

		Console.ForegroundColor = ConsoleColor.White;
		ConsoleLayoutHelper.WriteWrapped(
			$"Completed steps: {(result.CompletedSeeders.Any() ? string.Join(", ", result.CompletedSeeders) : "none")}.");
		if (!string.IsNullOrWhiteSpace(result.FailedSeeder))
		{
			ConsoleLayoutHelper.WriteWrapped($"Failed step: {result.FailedSeeder}.");
		}

		if (result.UnstartedSeeders.Any())
		{
			ConsoleLayoutHelper.WriteWrapped($"Unstarted steps: {string.Join(", ", result.UnstartedSeeders)}.");
		}

		Console.WriteLine();
		Console.WriteLine("Press any key to return to the main menu.");
		WaitForReturnToMenu();
	}
#endif

    private void ShowSeeder(IDatabaseSeeder seeder)
    {
        SafeClear();
        Console.WriteLine("Loading package...");
        using FuturemudDatabaseContext context = CreateContext(connectionString, useLazyLoading: true);
        SeederAssessment assessment = seeder.AssessSeedData(context);
        SafeClear();
        ConsoleLayoutHelper.WritePrompt(
            $"Package: #A{seeder.Name}#F\nTagline: #A{seeder.Tagline}#F\n\n#3{seeder.FullDescription}#F");

        Console.ForegroundColor = GetAssessmentColour(assessment.Status);
        Console.WriteLine($"Status: {GetAssessmentLabel(assessment.Status)}");
        Console.ForegroundColor = ConsoleColor.White;
        ConsoleLayoutHelper.WriteWrapped(assessment.Explanation);
        Console.WriteLine();

        if (assessment.MissingPrerequisites.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Missing prerequisites:");
            foreach (string prerequisite in assessment.MissingPrerequisites)
            {
                ConsoleLayoutHelper.WriteWrapped($" - {prerequisite}", indent: "   ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        if (assessment.Warnings.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (string warning in assessment.Warnings)
            {
                ConsoleLayoutHelper.WriteWrapped($"Warning: {warning}", indent: "         ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        if (assessment.Notes.Any())
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (string note in assessment.Notes)
            {
                ConsoleLayoutHelper.WriteWrapped(note);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        if (assessment.Status == SeederAssessmentStatus.Blocked)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            ConsoleLayoutHelper.WriteWrapped(
                "This package cannot be installed until the listed prerequisites are satisfied.");
            Console.ForegroundColor = ConsoleColor.White;
            WaitForReturnToMenu();
            return;
        }

        Console.WriteLine("Do you want to install this package?");
        while (true)
        {
            "Please type #3yes#F or #3no#F: ".WriteLineConsole();
            Console.WriteLine();
            string? answer = Console.ReadLine();
            if (answer is null)
            {
                return;
            }

            if (answer.EqualToAny("yes", "y"))
            {
                DoSeederQuestions(context, seeder);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Press any key to return to main menu.");
                Console.ForegroundColor = ConsoleColor.White;
                WaitForReturnToMenu();
                return;
            }

            if (answer.EqualToAny("no", "n", "quit", "abort", "exit"))
            {
                return;
            }
        }
    }

    private void DoSeederQuestions(FuturemudDatabaseContext context, IDatabaseSeeder seeder)
    {
        DictionaryWithDefault<string, string> answers = new();
        List<SeederQuestion> questions = seeder.Questions.ToList();
        string banner = $"Initial Setup questions for the {seeder.Name} seeder";
        string topline = banner;
        foreach (SeederQuestion? question in questions)
        {
            if (!SeederQuestionWorkflow.IsActive(question, context, answers))
            {
                continue;
            }

            string errorText = "";
            string? rememberedAnswer = SeederAnswerMemory.GetRememberedAnswer(context, seeder, question, answers);
            string? resolvedDefaultAnswer = null;
            if (!string.IsNullOrWhiteSpace(rememberedAnswer))
            {
                SeederQuestionValidationResult rememberedValidation =
                    SeederQuestionWorkflow.Validate(question, rememberedAnswer, context);
                if (rememberedValidation.Success)
                {
                    if (question.AutoReuseLastAnswer)
                    {
                        answers[question.Id] = rememberedAnswer;
                        SafeClear();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        ConsoleLayoutHelper.WriteWrapped(topline);
                        Console.ForegroundColor = ConsoleColor.White;
                        ConsoleLayoutHelper.WriteWrapped($"Reusing previous answer for {question.Id}: {rememberedAnswer}");
                        Thread.Sleep(1000);
                        continue;
                    }

                    resolvedDefaultAnswer = rememberedAnswer;
                }
                else
                {
                    errorText =
                        $"The previously remembered answer for {question.Id} is no longer valid and will be ignored.{(string.IsNullOrWhiteSpace(rememberedValidation.Error) ? "" : $"\n{rememberedValidation.Error}")}";
                }
            }

            while (true)
            {
                SafeClear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                ConsoleLayoutHelper.WriteWrapped(topline);
                Console.ForegroundColor = ConsoleColor.White;
                if (!string.IsNullOrEmpty(errorText))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    ConsoleLayoutHelper.WriteWrapped(errorText);
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    errorText = "";
                }

                ConsoleQuestionDisplay display = question.ResolveDisplay(context, answers);
                if (!string.IsNullOrWhiteSpace(display.DefaultAnswer) &&
                    string.IsNullOrWhiteSpace(resolvedDefaultAnswer))
                {
                    SeederQuestionValidationResult defaultValidation =
                        SeederQuestionWorkflow.Validate(question, display.DefaultAnswer, context);
                    if (defaultValidation.Success)
                    {
                        resolvedDefaultAnswer = display.DefaultAnswer;
                    }
                    else
                    {
                        errorText =
                            $"The suggested default answer for {question.Id} is no longer valid and will be ignored.{(string.IsNullOrWhiteSpace(defaultValidation.Error) ? "" : $"\n{defaultValidation.Error}")}";
                    }
                }

                ConsoleLayoutHelper.WritePrompt(display.Prompt);
                if (!string.IsNullOrWhiteSpace(resolvedDefaultAnswer))
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    ConsoleLayoutHelper.WriteWrapped($"Suggested default: {resolvedDefaultAnswer} (press Enter to accept)");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine();
                Console.Write("> ");
                string? answer = Console.ReadLine();
                if (answer is null)
                {
                    return;
                }

                if (answer.EqualToAny("quit", "back", "exit"))
                {
                    return;
                }

                answer = ResolveQuestionAnswer(answer, resolvedDefaultAnswer);

                SeederQuestionValidationResult answerValidation =
                    SeederQuestionWorkflow.Validate(question, answer, context);
                if (!answerValidation.Success)
                {
                    errorText = answerValidation.Error;
                    continue;
                }

                answers[question.Id] = answer;
                break;
            }
        }

        SafeClear();
        $"Applying the data from the #2{seeder.Name}#F seeder...".WriteLineConsole();
        SeederExecutionResult execution = SeederExecutionService.Execute(
            context,
            seeder,
            questions,
            answers,
            productAssembly.GetName().Version ?? new Version(1, 0, 0));
        if (execution.Success)
        {
            ConsoleLayoutHelper.WriteWrapped(execution.Message ?? string.Empty);
            return;
        }

        Console.ForegroundColor = ConsoleColor.Red;
#if DEBUG
		ConsoleLayoutHelper.WriteWrapped($"The {seeder.Name} seeder failed: {execution.Exception}");
#else
		ConsoleLayoutHelper.WriteWrapped($"The {seeder.Name} seeder failed: {execution.Exception?.Message}");
#endif
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static string ResolveQuestionAnswer(string answer, string? defaultAnswer)
    {
        return string.IsNullOrWhiteSpace(answer) && !string.IsNullOrWhiteSpace(defaultAnswer)
            ? defaultAnswer
            : answer;
    }

    internal static bool ShowSeederInMainMenu(SeederAssessmentStatus status, bool showBlockedSeeders)
    {
        return showBlockedSeeders || status != SeederAssessmentStatus.Blocked;
    }

    internal static string GetBlockedSeederSummary(int blockedSeederCount, bool showBlockedSeeders)
    {
        if (blockedSeederCount <= 0)
        {
            return string.Empty;
        }

        string packageText = blockedSeederCount == 1 ? "package" : "packages";
        string pronounText = blockedSeederCount == 1 ? "it" : "them";
        string verbText = blockedSeederCount == 1 ? "is" : "are";
        string thereVerbText = blockedSeederCount == 1 ? "is" : "are";
        string hiddenActionText = blockedSeederCount == 1 ? "see it" : "see a list of them";
        return showBlockedSeeders
            ? $"{blockedSeederCount:N0} blocked {packageText} {verbText} currently visible. Type VALID to hide {pronounText} again."
            : $"There {thereVerbText} {blockedSeederCount:N0} {packageText} reporting prerequisites not met. Type INVALID to {hiddenActionText}.";
    }

    private static int GetMenuSortRank(SeederAssessmentStatus status)
    {
        return status switch
        {
            SeederAssessmentStatus.ReadyToInstall => 0,
            SeederAssessmentStatus.UpdateAvailable => 1,
            SeederAssessmentStatus.AdditiveInstallAvailable => 2,
            SeederAssessmentStatus.InstalledCurrent => 3,
            _ => 4
        };
    }

    private static ConsoleColor GetAssessmentColour(SeederAssessmentStatus status)
    {
        return status switch
        {
            SeederAssessmentStatus.Blocked => ConsoleColor.DarkRed,
            SeederAssessmentStatus.ReadyToInstall => ConsoleColor.Green,
            SeederAssessmentStatus.AdditiveInstallAvailable => ConsoleColor.Cyan,
            SeederAssessmentStatus.UpdateAvailable => ConsoleColor.Yellow,
            _ => ConsoleColor.DarkYellow
        };
    }

    private static string GetAssessmentLabel(SeederAssessmentStatus status)
    {
        return status switch
        {
            SeederAssessmentStatus.Blocked => "Blocked",
            SeederAssessmentStatus.ReadyToInstall => "Ready",
            SeederAssessmentStatus.AdditiveInstallAvailable => "Additive",
            SeederAssessmentStatus.UpdateAvailable => "Update",
            _ => "Current"
        };
    }
}
