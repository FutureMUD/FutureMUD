#nullable enable

using System;
using System.IO;

namespace DatabaseSeeder;

internal enum StartupScriptGenerationResult
{
	Created,
	Updated,
	Unchanged,
	PreservedCustom
}

internal static class StartupScriptGenerator
{
	private const string GeneratedSignature = "FutureMUD generated startup script";
	private const int TemplateVersion = 2;
	private const int MaximumStartAttempts = 100;
	private const int RestartDelaySeconds = 5;

	internal static StartupScriptGenerationResult EnsureStartScript(string installationDirectory,
		string connectionString, bool isWindows)
	{
		Directory.CreateDirectory(Path.Combine(installationDirectory, "Binaries"));

		string fileName = isWindows ? "Start-MUD.bat" : "Start-MUD.sh";
		string scriptPath = Path.Combine(installationDirectory, fileName);
		string script = isWindows
			? BuildWindowsScript(installationDirectory, connectionString)
			: BuildLinuxScript(connectionString);
		StartupScriptGenerationResult result = EnsureScript(scriptPath, script, isWindows);

		if (!isWindows && !OperatingSystem.IsWindows() &&
			result != StartupScriptGenerationResult.PreservedCustom)
		{
			File.SetUnixFileMode(
				scriptPath,
				UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
				UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
				UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
		}

		return result;
	}

	internal static string BuildWindowsScript(string installationDirectory, string connectionString)
	{
		return $"""
@echo off
rem {GeneratedSignature} v{TemplateVersion}
setlocal
set "MUDDIR={installationDirectory}"
set "CODEDIR=%MUDDIR%\Binaries"
set /a loopcount={MaximumStartAttempts}
cd /d "%MUDDIR%" || exit /b 1
del /q "%MUDDIR%\STOP-REBOOTING" 2>nul
del /q "%MUDDIR%\BOOTING" 2>nul
:loop
if exist "%CODEDIR%\MudSharp.exe" goto :applyupdate
goto :startmud
:applyupdate
xcopy "%CODEDIR%\*" "%MUDDIR%\" /E /I /H /Y /Q >nul
if errorlevel 1 goto :copyfailed
:startmud
if not exist "%MUDDIR%\MudSharp.exe" goto :missingengine
"%MUDDIR%\MudSharp.exe" "MySql.Data.MySqlClient" "{connectionString}"
set "exitcode=%ERRORLEVEL%"
if exist "%MUDDIR%\STOP-REBOOTING" goto :stopped
if exist "%MUDDIR%\BOOTING" goto :bootfailed
set /a loopcount=%loopcount%-1
if %loopcount% LEQ 0 goto :exhausted
echo MUD exited unexpectedly - will attempt to restart %loopcount% more times in {RestartDelaySeconds} seconds.
timeout /t {RestartDelaySeconds} /nobreak >nul
goto :loop
:copyfailed
echo Failed to apply the staged update from "%CODEDIR%".
exit /b 1
:missingengine
echo Could not find "%MUDDIR%\MudSharp.exe".
exit /b 1
:bootfailed
echo MUD exited during its boot sequence. It will not be restarted.
if "%exitcode%"=="0" exit /b 1
exit /b %exitcode%
:stopped
echo MUD was shut down and requested the boot loop end.
exit /b 0
:exhausted
echo MUD exited unexpectedly too many times. It will not be restarted.
if "%exitcode%"=="0" exit /b 1
exit /b %exitcode%
""";
	}

	internal static string BuildLinuxScript(string connectionString)
	{
		return $"""
#!/bin/sh
# {GeneratedSignature} v{TemplateVersion}

SCRIPT_DIR=$(CDPATH= cd "$(dirname "$0")" && pwd)
if [ -z "$SCRIPT_DIR" ]; then
	echo "Could not determine the FutureMUD installation directory."
	exit 1
fi

cd "$SCRIPT_DIR" || exit 1
echo "The working directory is now $SCRIPT_DIR"
echo "Starting the game engine. Will attempt {MaximumStartAttempts} starts."
rm -f "$SCRIPT_DIR/BOOTING"
rm -f "$SCRIPT_DIR/STOP-REBOOTING"

attempt=0
max_attempts={MaximumStartAttempts}
while [ "$attempt" -lt "$max_attempts" ]
do
	if [ -f "$SCRIPT_DIR/Binaries/MudSharp" ]
	then
		cp -Rpf "$SCRIPT_DIR/Binaries/." "$SCRIPT_DIR/" || exit 1
		chmod +x "$SCRIPT_DIR/MudSharp" || exit 1
	fi

	if [ ! -x "$SCRIPT_DIR/MudSharp" ]
	then
		echo "Could not find an executable MudSharp engine in $SCRIPT_DIR."
		exit 1
	fi

	"$SCRIPT_DIR/MudSharp" "MySql.Data.MySqlClient" "{connectionString}"
	exit_code=$?

	if [ -f "$SCRIPT_DIR/BOOTING" ]
	then
		echo "Server exited during its boot sequence. It will not be restarted."
		if [ "$exit_code" -eq 0 ]; then exit 1; fi
		exit "$exit_code"
	fi

	if [ -f "$SCRIPT_DIR/STOP-REBOOTING" ]
	then
		echo "Server was shut down with a request to end the boot loop."
		exit 0
	fi

	attempt=$((attempt + 1))
	if [ "$attempt" -ge "$max_attempts" ]
	then
		echo "Server exited unexpectedly too many times. It will not be restarted."
		if [ "$exit_code" -eq 0 ]; then exit 1; fi
		exit "$exit_code"
	fi

	echo "Server exited unexpectedly - retry $attempt of $max_attempts in {RestartDelaySeconds} seconds."
	sleep {RestartDelaySeconds}
done
""";
	}

	private static StartupScriptGenerationResult EnsureScript(string scriptPath, string script, bool isWindows)
	{
		if (!File.Exists(scriptPath))
		{
			File.WriteAllText(scriptPath, script);
			return StartupScriptGenerationResult.Created;
		}

		string existing = File.ReadAllText(scriptPath);
		if (string.IsNullOrWhiteSpace(existing))
		{
			File.WriteAllText(scriptPath, script);
			return StartupScriptGenerationResult.Created;
		}

		if (existing.Equals(script, StringComparison.Ordinal))
		{
			return StartupScriptGenerationResult.Unchanged;
		}

		if (!existing.Contains(GeneratedSignature, StringComparison.Ordinal) &&
			!IsLegacyGeneratedScript(existing, isWindows))
		{
			return StartupScriptGenerationResult.PreservedCustom;
		}

		File.WriteAllText(scriptPath, script);
		return StartupScriptGenerationResult.Updated;
	}

	private static bool IsLegacyGeneratedScript(string script, bool isWindows)
	{
		return isWindows
			? script.Contains("set CODEDIR=%MUDDIR%\\Binaries", StringComparison.OrdinalIgnoreCase) &&
			  script.Contains("set loopcount=100", StringComparison.OrdinalIgnoreCase) &&
			  script.Contains("xcopy %CODEDIR%\\*.exe", StringComparison.OrdinalIgnoreCase) &&
			  script.Contains("%MUDDIR%\\MudSharp.exe", StringComparison.OrdinalIgnoreCase)
			: script.Contains("SERVER_PORT_BASEDIR=\".\"", StringComparison.Ordinal) &&
			  script.Contains("for i in 'seq 1 100'", StringComparison.Ordinal) &&
			  script.Contains("$SERVER_PORT_BASEDIR/MudSharp", StringComparison.Ordinal);
	}
}
