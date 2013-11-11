@echo off
cls

set SName=localhost
set UName=sa
set Pwd=ciborg22
set DbName=nopCommerceVoda2


if exist BackupVoda2.log del BackupVoda2.log

@echo on

sqlcmd -S %SName% -U %UName% -P %Pwd% -d %DbName% -I -i BackupVoda2.sql

pause "Press any key to finish..."

