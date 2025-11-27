@echo off
REM Regenerate Nethereum C# classes from smart contract ABI
REM This batch file calls the PowerShell script

powershell.exe -ExecutionPolicy Bypass -File "%~dp0regenerate-nethereum.ps1"
pause
