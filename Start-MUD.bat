set MUDDIR=C:\Users\luker\.codex\worktrees\e3e6\FutureMUD\DatabaseSeeder\bin\Debug\net10.0
set CODEDIR=%MUDDIR%\Binaries
set SERVER=localhost
set loopcount=100
cd /D %MUDDIR%
del %MUDDIR%\STOP-REBOOTING
del %MUDDIR%\BOOTING
:loop
xcopy %CODEDIR%\*.exe %MUDDIR%\ /C /Y
xcopy %CODEDIR%\*.dll %MUDDIR%\ /C /Y
xcopy %CODEDIR%\*.pdb %MUDDIR%\ /C /Y
xcopy %CODEDIR%\*.json %MUDDIR%\ /C /Y
%MUDDIR%\MudSharp.exe "MySql.Data.MySqlClient" "server=localhost;port=3307;database=rpi_engine;uid=futuremud;password=rpiengine2020;Default Command Timeout=300000;"
if exist %MUDDIR%\STOP-REBOOTING goto :endloop
if exist %MUDDIR%\BOOTING goto :crashed
echo MUD Crashed - will attempt to reboot %loopcount%0 more times.
set /a loopcount=%loopcount%-1
if %loopcount%==0 goto exitloop
goto loop
:crashed
echo Mud crashed during boot up sequence, will not attempt to restart
goto :exitloop
:endloop
echo Mud was shut down and requested boot loop to end.
:exitloop