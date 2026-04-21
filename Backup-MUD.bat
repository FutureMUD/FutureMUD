@echo off
set CUR_YYYY=%date:~10,4%
set CUR_MM=%date:~4,2%
set CUR_DD=%date:~7,2%
set CUR_HH=%time:~0,2%
if %CUR_HH% lss 10 (set CUR_HH=0%time:~1,1%)

set CUR_NN=%time:~3,2%
set CUR_SS=%time:~6,2%
set CUR_MS=%time:~9,2%

SET backupdir=C:\Users\luker\.codex\worktrees\e3e6\FutureMUD\DatabaseSeeder\bin\Debug\net10.0\Backups
SET mysqluername=futuremud
SET mysqlpassword=rpiengine2020
SET database=rpi_engine

"C:\Program Files\MySQL\MySQL Workbench 8.0\mysqldump.exe" -u%mysqluername% -p%mysqlpassword% %database% > %backupdir%\%database%_%CUR_YYYY%%CUR_MM%%CUR_DD%-%CUR_HH%%CUR_NN%%CUR_SS%.sql